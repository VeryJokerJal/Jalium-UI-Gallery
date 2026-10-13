using Jalium.UI.Controls;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Jalium.UI.Styling;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class CssStylingPage : Page
{
    public CssStylingPage()
    {
        InitializeComponent();
        if (FindName("CssViewportDynamicSlider") is Slider viewportSlider && FindName("CssViewportDemoHost") is FrameworkElement viewportHost)
        {
            void UpdateViewport() => Css.SetViewportMetrics(viewportHost,
                new CssViewportMetrics(new Size(420, 100), new Size(420, 200), new Size(420, viewportSlider.Value)));
            viewportSlider.ValueChanged += (_, _) => UpdateViewport();
            UpdateViewport();
        }
        InitializeImageBrushDemo();
        ImportStylesReady = LoadImportStylesAsync();
        FontStylesReady = LoadFontStylesAsync();
    }

    public Task ImportStylesReady { get; }
    public Task FontStylesReady { get; }

    private void InitializeImageBrushDemo()
    {
        const string resourceName = "Jalium.UI.Gallery.Modules.Main.Assets.logo.png";
        using var stream = typeof(CssStylingPage).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException(resourceName);
        using var bytes = new MemoryStream();
        stream.CopyTo(bytes);
        var source = BitmapImage.FromBytes(bytes.ToArray());

        if (FindName("CssImageBrushEllipse") is Border ellipse)
        {
            ellipse.Background = new ImageBrush(source)
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
            };
        }

        if (FindName("CssImageBrushCorners") is Border corners)
        {
            corners.Background = new ImageBrush(source)
            {
                Stretch = Stretch.UniformToFill,
                TileMode = TileMode.FlipX,
                Viewport = new Rect(0, 0, 0.5, 1),
                RelativeTransform = new TranslateTransform(0.08, 0),
            };
        }
    }

    private async Task LoadFontStylesAsync()
    {
        var sheet = await CssStyleSheet.LoadAsync(new Uri("gallery-font:///font.css"), new FontResources());
        if (FindName("CssFontFaceDemoHost") is FrameworkElement host) Css.GetStyleSheets(host).Add(sheet);
    }

    private sealed class FontResources : ICssResourceResolver
    {
        public ValueTask<CssResource> ResolveAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var name = uri.AbsolutePath switch
            {
                "/font.css" => "Jalium.UI.Gallery.Modules.Main.Assets.CssFonts.font.css",
                "/Ahem.ttf" => "Jalium.UI.Gallery.Modules.Main.Assets.CssFonts.Ahem.ttf",
                "/Ahem.woff2" => "Jalium.UI.Gallery.Modules.Main.Assets.CssFonts.Ahem.woff2",
                _ => throw new FileNotFoundException(uri.ToString()),
            };
            var stream = typeof(CssStylingPage).Assembly.GetManifestResourceStream(name) ?? throw new FileNotFoundException(name);
            return ValueTask.FromResult(new CssResource(uri, stream));
        }
    }

    private async Task LoadImportStylesAsync()
    {
        var sheet = await CssStyleSheet.LoadAsync(new Uri("gallery-css:///main.css"), new ImportResources());
        if (FindName("CssImportDemoHost") is FrameworkElement host) Css.GetStyleSheets(host).Add(sheet);
    }

    private sealed class ImportResources : ICssResourceResolver
    {
        public ValueTask<CssResource> ResolveAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var name = uri.AbsolutePath switch
            {
                "/main.css" => "Jalium.UI.Gallery.Modules.Main.Assets.CssImports.main.css",
                "/base.css" => "Jalium.UI.Gallery.Modules.Main.Assets.CssImports.base.css",
                "/responsive.css" => "Jalium.UI.Gallery.Modules.Main.Assets.CssImports.responsive.css",
                _ => throw new FileNotFoundException(uri.ToString()),
            };
            var stream = typeof(CssStylingPage).Assembly.GetManifestResourceStream(name) ?? throw new FileNotFoundException(name);
            return ValueTask.FromResult(new CssResource(uri,stream));
        }
    }
}
