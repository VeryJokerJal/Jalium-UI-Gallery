using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Themes;

public sealed class GalleryThemeContrastFixture
{
    [Theory]
    [InlineData(GalleryThemeMode.Light)]
    [InlineData(GalleryThemeMode.Dark)]
    public void ReadingTextMeetsContrastOnTheShippedSurfaces(GalleryThemeMode mode)
    {
        var original = GalleryTheme.CurrentMode;
        try
        {
            GalleryTheme.CurrentMode = mode;
            var texts = new[] { GalleryTheme.TextPrimary, GalleryTheme.TextSecondary,
                GalleryTheme.TextTertiary, GalleryTheme.TextMuted };
            var surfaces = new[] { GalleryTheme.BackgroundDark, GalleryTheme.BackgroundMedium,
                GalleryTheme.BackgroundLight, GalleryTheme.BackgroundCard, GalleryTheme.BackgroundCardInner,
                GalleryTheme.BackgroundHover, GalleryTheme.BackgroundPressed };
            foreach (var text in texts)
                foreach (var surface in surfaces)
                    Assert.True(Contrast(text, surface) >= 4.5,
                        $"{mode}: {text} on {surface} falls below 4.5:1.");

            foreach (var surface in surfaces)
                Assert.True(Contrast(GalleryTheme.AccentOnSurface, surface) >= 4.5);
            Assert.True(Contrast(GalleryTheme.TextOnAccent, GalleryTheme.AccentPrimary) >= 4.5);
        }
        finally { GalleryTheme.CurrentMode = original; }
    }

    private static double Contrast(Color first, Color second)
    {
        var a = Luminance(first);
        var b = Luminance(second);
        return (Math.Max(a, b) + 0.05) / (Math.Min(a, b) + 0.05);
    }

    private static double Luminance(Color color) =>
        0.2126 * Linear(color.R) + 0.7152 * Linear(color.G) + 0.0722 * Linear(color.B);

    private static double Linear(byte channel)
    {
        var value = channel / 255d;
        return value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
    }
}
