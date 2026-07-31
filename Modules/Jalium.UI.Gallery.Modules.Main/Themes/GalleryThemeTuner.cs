using Jalium.UI;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

internal static class GalleryThemeTuner
{
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

        var light = Mix(accent, Color.FromRgb(0xFF, 0xFF, 0xFF), 0.18);
        var secondary = Mix(accent, Color.FromRgb(0x00, 0x00, 0x00), 0.16);
        var softAlpha = GalleryTheme.CurrentMode == GalleryThemeMode.Dark ? (byte)0x42 : (byte)0x30;

        resources[GalleryTheme.KeyAccentPrimary] = new SolidColorBrush(accent);
        resources[GalleryTheme.KeyAccentLight] = new SolidColorBrush(light);
        resources[GalleryTheme.KeyAccentSecondary] = new SolidColorBrush(secondary);
        resources[GalleryTheme.KeyAccentSoft] = new SolidColorBrush(
            Color.FromArgb(softAlpha, accent.R, accent.G, accent.B));
    }

    public static void ApplySurface(Color surface)
    {
        if (Application.Current?.Resources is not { } resources)
        {
            return;
        }

        var blendTarget = IsDark(surface)
            ? Color.FromRgb(0x00, 0x00, 0x00)
            : Color.FromRgb(0xFF, 0xFF, 0xFF);

        resources[GalleryTheme.KeyCardBackground] = new SolidColorBrush(surface);
        resources[GalleryTheme.KeyCardInnerBackground] = new SolidColorBrush(Mix(surface, blendTarget, 0.08));
        resources[GalleryTheme.KeyCardAltBackground] = new SolidColorBrush(Mix(surface, blendTarget, 0.14));
    }

    public static void Reset()
    {
        if (Application.Current?.Resources is { } resources)
        {
            GalleryTheme.RegisterResources(resources);
        }
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

    private static byte Lerp(byte from, byte to, double amount) =>
        (byte)Math.Round(from + ((to - from) * amount));
}
