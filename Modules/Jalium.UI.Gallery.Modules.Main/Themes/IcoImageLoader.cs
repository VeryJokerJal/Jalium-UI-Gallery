using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// 从 Windows ICO 字节数据中提取一帧并转换为 <see cref="BitmapImage"/> 可直接解码的格式。
/// ICO 文件由多个帧组成，每一帧可以是 PNG 压缩数据或一段"不含 BITMAPFILEHEADER 的 DIB"。
/// 该加载器自动挑选质量最高的一帧（优先 PNG，再按像素数与位深度加权），并对 DIB 帧补齐
/// 文件头、修正 biHeight（ICO 里包含了额外的 AND 掩码，biHeight 是真实高度的两倍），最终
/// 返回可被底层解码器消化的 PNG 或 BMP 字节。
/// </summary>
internal static class IcoImageLoader
{
    public static BitmapImage Load(byte[] icoData)
    {
        var frame = ExtractBestFrame(icoData);
        return BitmapImage.FromBytes(frame);
    }

    public static byte[] ExtractBestFrame(byte[] icoData)
    {
        ArgumentNullException.ThrowIfNull(icoData);

        if (icoData.Length < 6)
            throw new InvalidDataException("ICO 数据过短，无法读取头部。");

        var reserved = ReadUInt16(icoData, 0);
        var type = ReadUInt16(icoData, 2);
        var count = ReadUInt16(icoData, 4);

        if (reserved != 0 || type != 1 || count == 0)
            throw new InvalidDataException("不是有效的 ICO 文件（头部校验失败）。");

        const int HeaderSize = 6;
        const int EntrySize = 16;

        if (icoData.Length < HeaderSize + EntrySize * count)
            throw new InvalidDataException("ICO 图像目录被截断。");

        var bestIndex = -1;
        long bestScore = -1;

        for (var i = 0; i < count; i++)
        {
            var entryOffset = HeaderSize + i * EntrySize;
            var width = icoData[entryOffset];
            var height = icoData[entryOffset + 1];
            var bpp = ReadUInt16(icoData, entryOffset + 6);
            var size = (int)ReadUInt32(icoData, entryOffset + 8);
            var data = (int)ReadUInt32(icoData, entryOffset + 12);

            if (size <= 0 || data < 0 || (long)data + size > icoData.Length)
                continue;

            var effWidth = width == 0 ? 256 : width;
            var effHeight = height == 0 ? 256 : height;
            var isPng = IsPngSignature(icoData, data);

            long score = (long)effWidth * effHeight * Math.Max(bpp == 0 ? 32 : bpp, 1);
            if (isPng)
                score += 1L << 40; // PNG 一律优先（即便尺寸略小）

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        if (bestIndex < 0)
            throw new InvalidDataException("ICO 中未找到可用的图像帧。");

        var bestEntry = HeaderSize + bestIndex * EntrySize;
        var bestWidth = icoData[bestEntry];
        var bestHeight = icoData[bestEntry + 1];
        var bestBpp = ReadUInt16(icoData, bestEntry + 6);
        var bestSize = (int)ReadUInt32(icoData, bestEntry + 8);
        var bestDataOffset = (int)ReadUInt32(icoData, bestEntry + 12);
        var effectiveWidth = bestWidth == 0 ? 256 : bestWidth;
        var effectiveHeight = bestHeight == 0 ? 256 : bestHeight;

        if (IsPngSignature(icoData, bestDataOffset))
        {
            var png = new byte[bestSize];
            Array.Copy(icoData, bestDataOffset, png, 0, bestSize);
            return png;
        }

        return WrapDibAsBmp(icoData, bestDataOffset, bestSize, effectiveWidth, effectiveHeight, bestBpp);
    }

    private static byte[] WrapDibAsBmp(byte[] icoData, int dataOffset, int dataSize, int width, int height, int bitsPerPixel)
    {
        if (dataSize < 40)
            throw new InvalidDataException("ICO 帧 DIB 头不完整。");

        var headerSize = (int)ReadUInt32(icoData, dataOffset);
        if (headerSize < 40 || headerSize > dataSize)
            throw new InvalidDataException($"ICO 帧 DIB 头长度非法：{headerSize}。");

        var biBitCount = bitsPerPixel != 0 ? bitsPerPixel : ReadUInt16(icoData, dataOffset + 14);
        var biCompression = ReadUInt32(icoData, dataOffset + 16);

        var colorTableEntries = biBitCount switch
        {
            1 => 2,
            4 => 16,
            8 => 256,
            _ => 0,
        };
        var colorTableSize = colorTableEntries * 4;

        var xorRowStride = ((width * biBitCount + 31) / 32) * 4;
        var xorSize = xorRowStride * height;

        var andRowStride = ((width + 31) / 32) * 4;
        var andSize = andRowStride * height;

        var pixelDataSize = xorSize;
        var totalBodySize = headerSize + colorTableSize + pixelDataSize;

        // ICO 中一个 DIB 帧的 body：BITMAPINFOHEADER + (可选)调色板 + XOR 像素 + AND 掩码。
        // 我们只保留 XOR 像素，并把 biHeight 修正为实际高度。
        var file = new byte[14 + totalBodySize];

        // BITMAPFILEHEADER
        file[0] = (byte)'B';
        file[1] = (byte)'M';
        WriteUInt32(file, 2, (uint)file.Length);
        WriteUInt16(file, 6, 0);
        WriteUInt16(file, 8, 0);
        WriteUInt32(file, 10, (uint)(14 + headerSize + colorTableSize));

        // BITMAPINFOHEADER + 可选调色板
        var bodyToCopy = Math.Min(headerSize + colorTableSize, dataSize);
        Array.Copy(icoData, dataOffset, file, 14, bodyToCopy);

        // 修正 biHeight（ICO 中等于真实高度 * 2 用于容纳 AND 掩码，独立 BMP 下需改回真实值）
        WriteInt32(file, 14 + 8, height);
        // biSizeImage 若为 0 保持 0；为保险起见写入 xorSize
        WriteUInt32(file, 14 + 20, (uint)xorSize);

        // XOR 像素数据
        var pixelSrcOffset = dataOffset + headerSize + colorTableSize;
        if (pixelSrcOffset + xorSize > icoData.Length)
            throw new InvalidDataException("ICO 帧像素数据长度不足。");

        Array.Copy(icoData, pixelSrcOffset, file, 14 + headerSize + colorTableSize, xorSize);

        _ = andSize; // AND 掩码被有意丢弃
        _ = biCompression;

        return file;
    }

    private static bool IsPngSignature(byte[] data, int offset)
    {
        if (offset < 0 || offset + 8 > data.Length)
            return false;

        return data[offset] == 0x89
            && data[offset + 1] == 0x50
            && data[offset + 2] == 0x4E
            && data[offset + 3] == 0x47
            && data[offset + 4] == 0x0D
            && data[offset + 5] == 0x0A
            && data[offset + 6] == 0x1A
            && data[offset + 7] == 0x0A;
    }

    private static ushort ReadUInt16(byte[] data, int offset)
        => (ushort)(data[offset] | (data[offset + 1] << 8));

    private static uint ReadUInt32(byte[] data, int offset)
        => (uint)(data[offset]
                | (data[offset + 1] << 8)
                | (data[offset + 2] << 16)
                | (data[offset + 3] << 24));

    private static void WriteUInt16(byte[] data, int offset, ushort value)
    {
        data[offset] = (byte)(value & 0xFF);
        data[offset + 1] = (byte)((value >> 8) & 0xFF);
    }

    private static void WriteUInt32(byte[] data, int offset, uint value)
    {
        data[offset] = (byte)(value & 0xFF);
        data[offset + 1] = (byte)((value >> 8) & 0xFF);
        data[offset + 2] = (byte)((value >> 16) & 0xFF);
        data[offset + 3] = (byte)((value >> 24) & 0xFF);
    }

    private static void WriteInt32(byte[] data, int offset, int value)
        => WriteUInt32(data, offset, unchecked((uint)value));
}
