using Jalium.UI.Controls;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Jalium.UI.Styling;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>View adapters for the CSS viewport and image-brush examples.</summary>
public static class CssStylingDemoBehavior
{
    private static readonly Lazy<BitmapImage> Logo = new(LoadLogo);

    public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.RegisterAttached(
        "ViewportHeight", typeof(double), typeof(CssStylingDemoBehavior),
        new PropertyMetadata(0.0, OnViewportHeightChanged));
    public static double GetViewportHeight(DependencyObject element) => (double)element.GetValue(ViewportHeightProperty)!;
    public static void SetViewportHeight(DependencyObject element, double value) => element.SetValue(ViewportHeightProperty, value);

    public static readonly DependencyProperty ImageExampleProperty = DependencyProperty.RegisterAttached(
        "ImageExample", typeof(string), typeof(CssStylingDemoBehavior), new PropertyMetadata(null, OnImageExampleChanged));
    public static string? GetImageExample(DependencyObject element) => element.GetValue(ImageExampleProperty) as string;
    public static void SetImageExample(DependencyObject element, string? value) => element.SetValue(ImageExampleProperty, value);

    private static void OnViewportHeightChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is FrameworkElement host)
            Css.SetViewportMetrics(host, new CssViewportMetrics(new Size(420, 100), new Size(420, 200), new Size(420, GetViewportHeight(host))));
    }

    private static void OnImageExampleChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is not Border border || args.NewValue is not string example) return;
        var brush = new ImageBrush(Logo.Value)
        {
            Stretch = Stretch.UniformToFill,
            AlignmentX = AlignmentX.Center,
            AlignmentY = AlignmentY.Center,
        };
        if (example == "Corners")
        {
            brush.TileMode = TileMode.FlipX;
            brush.Viewport = new Rect(0, 0, 0.5, 1);
            brush.RelativeTransform = new TranslateTransform(0.08, 0);
        }
        border.Background = brush;
    }

    private static BitmapImage LoadLogo()
    {
        using var stream = typeof(CssStylingDemoBehavior).Assembly.GetManifestResourceStream("Jalium.UI.Gallery.Modules.Main.Assets.logo.png")
            ?? throw new FileNotFoundException("Gallery logo resource");
        using var bytes = new MemoryStream();
        stream.CopyTo(bytes);
        return BitmapImage.FromBytes(bytes.ToArray());
    }
}
