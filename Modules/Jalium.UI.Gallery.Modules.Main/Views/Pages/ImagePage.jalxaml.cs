using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for ImagePage.jalxaml demonstrating Image control functionality.
/// </summary>
public partial class ImagePage : Page
{
    private const string ImageUrl = "http://img.netbian.com/file/2024/0816/221229zX0E3.jpg";
    private const string SvgUrl = "C:/Users/suppe/Downloads/igw-f-warning-triangle.svg";

    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Bitmap Image from URL -->
    <Image Source=""https://example.com/photo.jpg""
           Stretch=""Uniform""
           Height=""300""
           Margin=""0,0,0,16""/>

    <!-- SVG Image from URL (auto-detected by .svg extension) -->
    <Image Source=""https://dev.w3.org/SVG/tools/svgweb/samples/svg-files/tiger.svg""
           Stretch=""Uniform""
           Height=""400""
           Margin=""0,0,0,16""/>

    <!-- Styled Image with CornerRadius and Border -->
    <Image Source=""https://example.com/photo.jpg""
           Stretch=""UniformToFill""
           CornerRadius=""16""
           Width=""150"" Height=""150""/>

    <!-- Bordered SVG Image -->
    <Image Source=""https://example.com/icon.svg""
           Stretch=""Uniform""
           BorderBrush=""#0078D4""
           BorderThickness=""3""
           CornerRadius=""8""
           Width=""150"" Height=""150""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class ImageSample : Page
{
    public ImageSample()
    {
        InitializeComponent();
        LoadImages();
    }

    private void LoadImages()
    {
        // Create a BitmapImage from URL
        var bitmap = new BitmapImage(
            new Uri(""https://example.com/photo.jpg""));

        var image = new Image
        {
            Source = bitmap,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        // Create an SvgImage from URL
        var svg = new SvgImage(
            new Uri(""https://dev.w3.org/SVG/tools/svgweb/samples/svg-files/tiger.svg""));

        var svgImage = new Image
        {
            Source = svg,
            Stretch = Stretch.Uniform,
            Height = 400
        };

        // Handle SVG loaded event (async HTTP)
        svg.OnSvgLoaded += (s, e) =>
        {
            StatusText.Text = $""SVG loaded: {svg.Width}x{svg.Height}"";
        };

        // Load SVG from string
        var inlineSvg = SvgImage.FromSvgString(
            ""<svg viewBox='0 0 100 100'><circle cx='50' cy='50' r='40' fill='red'/></svg>"");

        // Load SVG from file
        var fileSvg = SvgImage.FromFile(""assets/icon.svg"");
    }
}";

    public ImagePage()
    {
        InitializeComponent();
        LoadCodeExamples();
        LoadDemoImage();
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }

    private void LoadDemoImage()
    {
        var bitmapImage = new BitmapImage(new Uri(ImageUrl));

        // Main demo image
        if (ImageContainer != null)
        {
            var image = new Image
            {
                Source = bitmapImage,
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            ImageContainer.Child = image;
        }

        // Update status text when loaded
        bitmapImage.OnImageLoaded += (s, e) =>
        {
            if (ImageStatusText != null)
                ImageStatusText.Text = $"Image loaded: {bitmapImage.Width:F0} x {bitmapImage.Height:F0} pixels";

            // Once image is loaded, populate stretch mode and styled demos
            PopulateStretchModes(bitmapImage);
            PopulateStyledImages(bitmapImage);
        };

        // SVG demo
        LoadSvgDemo();
    }

    private void LoadSvgDemo()
    {
        // Use default constructor and register event handler BEFORE setting URI.
        // For local files, LoadFromFile is synchronous — the OnSvgLoaded event
        // fires during the UriSource setter. If we used the Uri constructor,
        // the event would fire before the handler is registered, so
        // PopulateSvgStretchModes would never be called and the stretch demos
        // would stay empty.
        var svgImage = new SvgImage();

        // Register handler first so it's ready for synchronous (file) loads
        svgImage.OnSvgLoaded += (s, e) =>
        {
            if (SvgStatusText != null)
                SvgStatusText.Text = $"SVG loaded: {svgImage.Width:F0} x {svgImage.Height:F0} (vector)";

            PopulateSvgStretchModes(svgImage);
        };

        // Now set the URI — for local files this loads synchronously and fires OnSvgLoaded
        svgImage.UriSource = new Uri(SvgUrl);

        // Main SVG demo image
        if (SvgImageContainer != null)
        {
            var image = new Image
            {
                Source = svgImage,
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            SvgImageContainer.Child = image;
        }
    }

    private void PopulateSvgStretchModes(SvgImage source)
    {
        if (SvgStretchNoneContainer != null)
        {
            SvgStretchNoneContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        if (SvgStretchFillContainer != null)
        {
            SvgStretchFillContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        if (SvgStretchUniformContainer != null)
        {
            SvgStretchUniformContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        if (SvgStretchUniformToFillContainer != null)
        {
            SvgStretchUniformToFillContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }
    }

    private void PopulateStretchModes(BitmapImage source)
    {
        // Stretch.None
        if (StretchNoneContainer != null)
        {
            StretchNoneContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        // Stretch.Fill
        if (StretchFillContainer != null)
        {
            StretchFillContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Stretch.Uniform
        if (StretchUniformContainer != null)
        {
            StretchUniformContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Stretch.UniformToFill
        if (StretchUniformToFillContainer != null)
        {
            StretchUniformToFillContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }
    }

    private void PopulateStyledImages(BitmapImage source)
    {
        // Rounded corners (CornerRadius=16)
        if (RoundedImageContainer != null)
        {
            RoundedImageContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.UniformToFill,
                CornerRadius = new CornerRadius(16),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Circle (CornerRadius=75 on 150x150 container)
        if (CircleImageContainer != null)
        {
            CircleImageContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.UniformToFill,
                CornerRadius = new CornerRadius(75),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Bordered
        if (BorderedImageContainer != null)
        {
            BorderedImageContainer.Child = new Image
            {
                Source = source,
                Stretch = Stretch.UniformToFill,
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)),
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(8),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }
    }
}
