using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

/// <summary>
/// Rasterises each styling page through the software renderer and asserts it actually painted
/// something — a page whose CSS silently failed to apply, or whose layout collapsed, renders as
/// a flat field of background. Set JALIUM_GALLERY_DUMP to a directory to also write the PNGs
/// and inspect them by eye.
/// </summary>
public sealed class CssPagesVisualDumpFixture
{
    static CssPagesVisualDumpFixture()
    {
        // In the app the page's AutoWireViewModel resolves the view model (which
        // registers the custom vocabulary) before the page's children are built.
        // The tests construct pages directly, so register up front — otherwise the
        // Css.Style declarations compile before the names exist, and headless runs
        // have no application root invalidator to recover them.
        Jalium.UI.Gallery.Modules.Main.Models.GalleryCssExtensions.EnsureRegistered();
    }

    private const int Width = 1100;
    private const int Bpp = 4;

    private static void ResetApplicationState()
    {
        typeof(Application)
            .GetField("_current", BindingFlags.NonPublic | BindingFlags.Static)
            ?.SetValue(null, null);
    }

    private static Application CreateStyledApplication()
    {
        ResetApplicationState();
        var application = new Application();
        GalleryTheme.RegisterResources(application.Resources);
        application.Resources.MergedDictionaries.Add(new GalleryStyles());
        return application;
    }

    private static int RenderAndCountPaintedPixels(string dumpName)
    {
        // The application and its resources must exist before the page is constructed:
        // StaticResource references in markup resolve during InitializeComponent.
        _ = CreateStyledApplication();

        try
        {
            var page = CreatePage(dumpName);
            page.Measure(new Size(Width, double.PositiveInfinity));
            page.UpdateLayout();

            // Applying CSS can grow the tree, so keep measuring until the desired height
            // stops moving — otherwise the bitmap is sized from a stale pass and clips.
            var height = 0.0;
            for (var pass = 0; pass < 4; pass++)
            {
                page.Measure(new Size(Width, double.PositiveInfinity));
                if (Math.Abs(page.DesiredSize.Height - height) < 0.5)
                {
                    break;
                }

                height = page.DesiredSize.Height;
                page.InvalidateMeasure();
            }

            var pixelHeight = (int)Math.Ceiling(Math.Clamp(height, 1, 6000));
            page.Measure(new Size(Width, pixelHeight));
            page.Arrange(new Rect(0, 0, Width, pixelHeight));

            var target = new RenderTargetBitmap(Width, pixelHeight, 96, 96, PixelFormat.Bgra32);
            var background = Color.FromArgb(255, 12, 14, 20);
            target.Clear(background);
            target.Render(page);

            var pixels = new byte[Width * Bpp * pixelHeight];
            target.CopyPixels(new Int32Rect(0, 0, Width, pixelHeight), pixels, Width * Bpp, 0);

            if (Environment.GetEnvironmentVariable("JALIUM_GALLERY_DUMP") is { Length: > 0 } directory)
            {
                Directory.CreateDirectory(directory);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(BitmapImage.FromPixels(pixels, Width, pixelHeight)));
                using var stream = File.Create(Path.Combine(directory, dumpName + ".png"));
                encoder.Save(stream);
            }

            var painted = 0;
            for (var offset = 0; offset < pixels.Length; offset += Bpp)
            {
                if (pixels[offset] != background.B ||
                    pixels[offset + 1] != background.G ||
                    pixels[offset + 2] != background.R)
                {
                    painted++;
                }
            }

            return painted;
        }
        finally
        {
            ResetApplicationState();
        }
    }

    public static TheoryData<string> PageNames => new()
    {
        "CssStyling", "CssSelectors", "CssLayout", "CssOutline", "CssMappings", "FlexPanel",
    };

    private static Page CreatePage(string name) => name switch
    {
        "CssStyling" => new CssStylingPage { DataContext = new CssStylingViewModel() },
        "CssSelectors" => new CssSelectorsPage { DataContext = new CssSelectorsViewModel() },
        "CssLayout" => new CssLayoutPage { DataContext = new CssLayoutViewModel() },
        "CssOutline" => new CssOutlinePage { DataContext = new CssOutlineViewModel() },
        "CssMappings" => new CssMappingsPage { DataContext = new CssMappingsViewModel() },
        "FlexPanel" => new FlexPanelPage { DataContext = new FlexPanelViewModel() },
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown styling page."),
    };

    [Theory]
    [MemberData(nameof(PageNames))]
    public void StylingPage_RendersItsDemos(string name)
    {
        var painted = RenderAndCountPaintedPixels(name);

        // Any real page covers far more than this; the threshold only catches a blank render.
        Assert.True(
            painted > 40_000,
            $"{name} painted only {painted} non-background pixels, which means the page rendered blank.");
    }
}
