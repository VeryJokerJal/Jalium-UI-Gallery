using System.Collections.Concurrent;
using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Resolves the compact SaaS illustrations used by the component catalog.
/// Image sources are cached because filtering and search rebuild card controls.
/// </summary>
internal static class SaasCardBackgrounds
{
    private const string ResourcePrefix =
        "Jalium.UI.Gallery.Modules.Main.Assets.SaasCardBackgrounds.";

    private static readonly Assembly ResourceAssembly = typeof(SaasCardBackgrounds).Assembly;
    private static readonly ConcurrentDictionary<string, ImageSource> SourceCache =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly IReadOnlyList<string> EmbeddedPageTags = ResourceAssembly
        .GetManifestResourceNames()
        .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal) &&
                       name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        .Select(name => name[ResourcePrefix.Length..^4])
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    internal static IReadOnlyList<string> AssetPageTags => EmbeddedPageTags;

    public static Image CreateImage(string pageTag)
    {
        return new Image
        {
            Source = GetSource(pageTag),
            Stretch = Stretch.UniformToFill,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsHitTestVisible = false
        };
    }

    internal static ImageSource GetSource(string pageTag)
    {
        ValidatePageTag(pageTag);
        return SourceCache.GetOrAdd(
            pageTag,
            static tag => ImageSourceLoader.FromBytes(ReadAssetBytes(tag)));
    }

    internal static byte[] ReadAssetBytes(string pageTag)
    {
        ValidatePageTag(pageTag);
        var resourceName = GetResourceName(pageTag);
        using var stream = ResourceAssembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"SaaS card background '{resourceName}' was not embedded.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    internal static string GetResourceName(string pageTag)
    {
        ValidatePageTag(pageTag);
        return $"{ResourcePrefix}{pageTag}.png";
    }

    private static void ValidatePageTag(string pageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);
        if (!pageTag.All(char.IsAsciiLetterOrDigit))
        {
            throw new ArgumentException(
                "Gallery page tags may only contain ASCII letters and digits.",
                nameof(pageTag));
        }
    }
}
