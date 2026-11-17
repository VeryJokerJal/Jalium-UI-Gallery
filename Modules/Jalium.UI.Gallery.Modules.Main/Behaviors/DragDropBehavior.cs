using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Input;
using Jalium.UI.Media.Imaging;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts demo drag sources to the native text/file/image drag loop.
/// Drop state and presentation belong to GalleryDropTarget and its theme.
/// </summary>
public static class DragDropBehavior
{
    private static BitmapImage? s_dragBadge;

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.RegisterAttached(
            "SourceText",
            typeof(string),
            typeof(DragDropBehavior),
            new PropertyMetadata(null, OnSourceChanged));

    public static readonly DependencyProperty UseCustomImageProperty =
        DependencyProperty.RegisterAttached(
            "UseCustomImage",
            typeof(bool),
            typeof(DragDropBehavior),
            new PropertyMetadata(false));

    public static readonly DependencyProperty ShellDragOutProperty =
        DependencyProperty.RegisterAttached(
            "ShellDragOut",
            typeof(bool),
            typeof(DragDropBehavior),
            new PropertyMetadata(false));

    private static readonly DependencyProperty SourceAttachedProperty =
        DependencyProperty.RegisterAttached(
            "SourceAttached",
            typeof(bool),
            typeof(DragDropBehavior),
            new PropertyMetadata(false));

    public static void SetSourceText(
        DependencyObject element,
        string value) =>
        element.SetValue(SourceTextProperty, value);

    public static string? GetSourceText(DependencyObject element) =>
        element.GetValue(SourceTextProperty) as string;

    public static void SetUseCustomImage(
        DependencyObject element,
        bool value) =>
        element.SetValue(UseCustomImageProperty, value);

    public static bool GetUseCustomImage(DependencyObject element) =>
        (bool)(element.GetValue(UseCustomImageProperty) ?? false);

    public static void SetShellDragOut(
        DependencyObject element,
        bool value) =>
        element.SetValue(ShellDragOutProperty, value);

    public static bool GetShellDragOut(DependencyObject element) =>
        (bool)(element.GetValue(ShellDragOutProperty) ?? false);

    private static void OnSourceChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not UIElement element ||
            string.IsNullOrWhiteSpace(args.NewValue as string) ||
            (bool)(element.GetValue(SourceAttachedProperty) ?? false))
        {
            return;
        }

        element.SetValue(SourceAttachedProperty, true);
        element.MouseDown += OnSourceMouseDown;
    }

    private static void OnSourceMouseDown(
        object sender,
        MouseButtonEventArgs args)
    {
        if (sender is not UIElement source ||
            args.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var sink = (source as FrameworkElement)?.DataContext
            as IDragDropStateSink;
        var text = GetSourceText(source) ?? string.Empty;
        var data = new DataObject();

        if (GetShellDragOut(source))
        {
            var path = sink?.CreateDragOutFile();
            if (path is null)
            {
                return;
            }

            data.SetData(DataFormats.FileDrop, new[] { path });
            data.SetText(Path.GetFileName(path));
            var effect = DragDrop.DoShellDragDrop(
                source,
                data,
                DragDropEffects.Copy,
                GetDragBadge(),
                new Point(16, 24));
            sink?.OnDragOutCompleted(effect.ToString());
            return;
        }

        data.SetData("Text", text);
        if (GetUseCustomImage(source))
        {
            DragDrop.DoDragDrop(
                source,
                data,
                DragDropEffects.Copy,
                GetDragBadge(),
                new Point(16, 24));
            return;
        }

        DragDrop.DoDragDrop(
            source,
            data,
            DragDropEffects.Copy);
    }

    private static BitmapImage GetDragBadge()
    {
        if (s_dragBadge is not null)
        {
            return s_dragBadge;
        }

        const int width = 168;
        const int height = 48;
        const int border = 2;
        var pixels = new byte[width * height * 4];
        for (var y = 0; y < height; y++)
        {
            var t = (double)y / (height - 1);
            var red = (byte)(0x25 + ((0x1E - 0x25) * t));
            var green = (byte)(0x63 + ((0x3A - 0x63) * t));
            var blue = (byte)(0xEB + ((0x8A - 0xEB) * t));
            var edgeRow = y < border || y >= height - border;

            for (var x = 0; x < width; x++)
            {
                var edge =
                    edgeRow ||
                    x < border ||
                    x >= width - border;
                var index = ((y * width) + x) * 4;
                pixels[index] = edge ? (byte)0xFF : blue;
                pixels[index + 1] = edge ? (byte)0xFF : green;
                pixels[index + 2] = edge ? (byte)0xFF : red;
                pixels[index + 3] = 0xFF;
            }
        }

        s_dragBadge =
            BitmapImage.FromPixels(pixels, width, height);
        return s_dragBadge;
    }
}
