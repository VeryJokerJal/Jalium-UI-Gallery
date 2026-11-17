using Jalium.UI;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

internal static class GalleryThemeTuner
{
    private static Color? _customAccent;
    private static Color? _customSurface;

    public static IReadOnlyList<Color> AccentPalette { get; } =
        new[]
        {
            Color.FromRgb(0x08, 0x94, 0x8A),
            Color.FromRgb(0x4F, 0x46, 0xE5),
            Color.FromRgb(0x02, 0x84, 0xC7),
            Color.FromRgb(0x16, 0xA3, 0x4A),
            Color.FromRgb(0xD9, 0x77, 0x06),
            Color.FromRgb(0xE1, 0x1D, 0x48)
        };

    public static IReadOnlyList<Color> SurfacePalette { get; } =
        new[]
        {
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0xF4, 0xF7, 0xF8),
            Color.FromRgb(0xF0, 0xFD, 0xFA),
            Color.FromRgb(0xEF, 0xF6, 0xFF),
            Color.FromRgb(0xFF, 0xFB, 0xEB),
            Color.FromRgb(0x18, 0x20, 0x24)
        };

    public static void ApplyAccent(Color accent)
    {
        if (Application.Current?.Resources is not { } resources)
        {
            return;
        }

        ApplyAccent(resources, accent);
    }

    internal static void ApplyAccent(ResourceDictionary resources, Color accent)
    {
        ArgumentNullException.ThrowIfNull(resources);

        _customAccent = accent;

        // Mutate the shared brush instances that RegisterResources already published
        // instead of writing replacements into the dictionary. Replacing the entries
        // only redirected {DynamicResource GalleryAccent} in markup and left every
        // code-built visual on GalleryTheme.AccentPrimaryBrush -- so tuning the accent
        // recoloured the markup half of the app and silently skipped the catalog
        // preview tiles, the page eyebrows and the drag-drop feedback.
        // Not touching the dictionary also avoids a collection write, which
        // synchronously re-evaluates the style stack for the entire visual tree.
        GalleryTheme.OverrideAccentBrushes(
            accent,
            AccentLightOf(accent),
            AccentSecondaryOf(accent),
            AccentSoftOf(accent),
            TextOnAccentOf(accent));
    }

    public static void ApplySurface(Color surface)
    {
        if (Application.Current?.Resources is null)
        {
            return;
        }

        _customSurface = surface;
        ApplySurfaceCore(surface);
    }

    public static void Reset()
    {
        _customAccent = null;
        _customSurface = null;

        // Put the shared brushes back on the mode-derived palette before re-publishing,
        // otherwise the overridden colours survive a reset (the dictionary already
        // references these very instances, so re-registering alone changes nothing).
        GalleryTheme.RestoreSharedBrushes();

        if (Application.Current?.Resources is { } resources)
        {
            GalleryTheme.RegisterResources(resources);
        }
    }

    /// <summary>
    /// Re-applies the active Theme Studio overrides after a Dark/Light flip.
    /// <c>GalleryTheme.RefreshSharedBrushes</c> re-derives every shared brush from the
    /// new mode, which necessarily discards the tuned accent soft-wash and the tuned
    /// surfaces, so the overrides have to be layered back on top afterwards. No
    /// dictionary write and no allocation happens on this path.
    /// </summary>
    public static void RefreshForCurrentMode()
    {
        if (Application.Current?.Resources is { } resources)
        {
            RefreshForCurrentMode(resources);
        }
    }

    internal static void RefreshForCurrentMode(ResourceDictionary resources)
    {
        ArgumentNullException.ThrowIfNull(resources);

        if (_customAccent is { } accent)
        {
            GalleryTheme.OverrideAccentBrushes(
                accent,
                AccentLightOf(accent),
                AccentSecondaryOf(accent),
                AccentSoftOf(accent),
                TextOnAccentOf(accent));
        }

        if (_customSurface is { } surface)
        {
            ApplySurfaceCore(surface);
        }
    }

    private static void ApplySurfaceCore(Color surface)
    {
        var blendTarget = IsDark(surface)
            ? Color.FromRgb(0x00, 0x00, 0x00)
            : Color.FromRgb(0xFF, 0xFF, 0xFF);

        GalleryTheme.OverrideSurfaceBrushes(
            surface,
            Mix(surface, blendTarget, 0.08),
            Mix(surface, blendTarget, 0.14));
    }

    private static Color AccentLightOf(Color accent) =>
        Mix(accent, Color.FromRgb(0xFF, 0xFF, 0xFF), 0.18);

    private static Color AccentSecondaryOf(Color accent) =>
        Mix(accent, Color.FromRgb(0x00, 0x00, 0x00), 0.16);

    private static Color AccentSoftOf(Color accent)
    {
        var alpha = GalleryTheme.CurrentMode == GalleryThemeMode.Dark
            ? (byte)0x42
            : (byte)0x30;
        return Color.FromArgb(alpha, accent.R, accent.G, accent.B);
    }

    internal static Color TextOnAccentOf(Color accent)
    {
        var dark = Color.FromRgb(0x10, 0x18, 0x27);
        var light = Color.FromRgb(0xFF, 0xFF, 0xFF);
        return ContrastRatio(accent, light) >= ContrastRatio(accent, dark)
            ? light
            : dark;
    }

    public static Color Mix(Color from, Color to, double amount)
    {
        var clamped = Math.Clamp(amount, 0, 1);
        return Color.FromArgb(
            Lerp(from.A, to.A, clamped),
            Lerp(from.R, to.R, clamped),
            Lerp(from.G, to.G, clamped),
            Lerp(from.B, to.B, clamped));
    }

    public static bool IsDark(Color color)
    {
        var luminance = (0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B);
        return luminance < 142;
    }

    public static string ToHex(Color color) =>
        color.A == 0xFF
            ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
            : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

    private static double ContrastRatio(Color first, Color second)
    {
        var lighter = Math.Max(RelativeLuminance(first), RelativeLuminance(second));
        var darker = Math.Min(RelativeLuminance(first), RelativeLuminance(second));
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double RelativeLuminance(Color color)
    {
        static double Channel(byte value)
        {
            var normalized = value / 255d;
            return normalized <= 0.04045
                ? normalized / 12.92
                : Math.Pow((normalized + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Channel(color.R)) +
               (0.7152 * Channel(color.G)) +
               (0.0722 * Channel(color.B));
    }

    private static byte Lerp(byte from, byte to, double amount) =>
        (byte)Math.Round(from + ((to - from) * amount));
}
