using System.Text;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class HexEditorViewModel
{
    private bool _showAsciiColumn = true;
    private bool _showOffsetColumn = true;
    private bool _showDataInterpretation;
    private double _bytesPerRow = 16;

    public HexEditorViewModel()
    {
        SampleData = GenerateSampleData();
        ReadOnlyData = GenerateReadOnlyData();
    }

    public byte[] SampleData { get; }

    public byte[] ReadOnlyData { get; }

    public bool ShowAsciiColumn
    {
        get => _showAsciiColumn;
        set => SetProperty(ref _showAsciiColumn, value);
    }

    public bool ShowOffsetColumn
    {
        get => _showOffsetColumn;
        set => SetProperty(ref _showOffsetColumn, value);
    }

    public bool ShowDataInterpretation
    {
        get => _showDataInterpretation;
        set => SetProperty(ref _showDataInterpretation, value);
    }

    public double BytesPerRow
    {
        get => _bytesPerRow;
        set => SetProperty(ref _bytesPerRow, Math.Round(value));
    }

    private static byte[] GenerateSampleData()
    {
        var data = new byte[256];
        data[0] = 0x4D;
        data[1] = 0x5A;
        data[2] = 0x90;
        data[3] = 0x00;

        for (var index = 4; index < 64; index++)
        {
            data[index] = (byte)(index * 3 + 0x10);
        }

        var message = Encoding.ASCII.GetBytes(
            "Hello from Jalium.UI HexEditor!");
        Array.Copy(message, 0, data, 64, message.Length);

        for (var index = 64 + message.Length;
             index < data.Length;
             index++)
        {
            data[index] = (byte)(index & 0xFF);
        }

        return data;
    }

    private static byte[] GenerateReadOnlyData()
    {
        var data = new byte[128];
        data[0] = 0x89;
        data[1] = 0x50;
        data[2] = 0x4E;
        data[3] = 0x47;
        data[4] = 0x0D;
        data[5] = 0x0A;
        data[6] = 0x1A;
        data[7] = 0x0A;
        data[8] = 0x00;
        data[9] = 0x00;
        data[10] = 0x00;
        data[11] = 0x0D;
        data[12] = 0x49;
        data[13] = 0x48;
        data[14] = 0x44;
        data[15] = 0x52;

        for (var index = 16; index < data.Length; index++)
        {
            data[index] = (byte)((index * 7 + 0x33) & 0xFF);
        }

        return data;
    }
}
