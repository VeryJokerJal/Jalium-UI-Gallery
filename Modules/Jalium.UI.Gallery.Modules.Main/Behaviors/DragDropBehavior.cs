using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Bridges pointer/drag events to bindable ViewModel state. It owns only
/// transient visual feedback and the native drag-loop adaptation.
/// </summary>
public static class DragDropBehavior
{
    // Drag-over feedback replaces the target's themed background for the
    // duration of the drag, so it has to be theme-sourced too: a baked dark
    // navy dropped a light-mode drop target to 1.23:1 and made the file list
    // painted on top of it unreadable. These are GalleryTheme's shared mutable
    // brushes, so a Dark/Light flip recolors the feedback in place.
    private static readonly Brush HighlightBorder =
        GalleryTheme.AccentPrimaryBrush;
    private static readonly Brush HighlightBackground =
        GalleryTheme.AccentSoftBrush;
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

    public static readonly DependencyProperty TargetRoleProperty =
        DependencyProperty.RegisterAttached(
            "TargetRole",
            typeof(DragDropTargetRole),
            typeof(DragDropBehavior),
            new PropertyMetadata(
                DragDropTargetRole.None,
                OnTargetChanged));

    public static readonly DependencyProperty TargetNameProperty =
        DependencyProperty.RegisterAttached(
            "TargetName",
            typeof(string),
            typeof(DragDropBehavior),
            new PropertyMetadata(string.Empty));

    private static readonly DependencyProperty SourceAttachedProperty =
        DependencyProperty.RegisterAttached(
            "SourceAttached",
            typeof(bool),
            typeof(DragDropBehavior),
            new PropertyMetadata(false));

    private static readonly DependencyProperty TargetAttachedProperty =
        DependencyProperty.RegisterAttached(
            "TargetAttached",
            typeof(bool),
            typeof(DragDropBehavior),
            new PropertyMetadata(false));

    private static readonly DependencyProperty OriginalBorderProperty =
        DependencyProperty.RegisterAttached(
            "OriginalBorder",
            typeof(Brush),
            typeof(DragDropBehavior),
            new PropertyMetadata(null));

    private static readonly DependencyProperty OriginalBackgroundProperty =
        DependencyProperty.RegisterAttached(
            "OriginalBackground",
            typeof(Brush),
            typeof(DragDropBehavior),
            new PropertyMetadata(null));

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

    public static void SetTargetRole(
        DependencyObject element,
        DragDropTargetRole value) =>
        element.SetValue(TargetRoleProperty, value);

    public static DragDropTargetRole GetTargetRole(
        DependencyObject element) =>
        (DragDropTargetRole)(element.GetValue(TargetRoleProperty) ??
            DragDropTargetRole.None);

    public static void SetTargetName(
        DependencyObject element,
        string value) =>
        element.SetValue(TargetNameProperty, value);

    public static string GetTargetName(DependencyObject element) =>
        element.GetValue(TargetNameProperty) as string ??
        element.GetType().Name;

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

    private static void OnTargetChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not UIElement element ||
            GetTargetRole(element) == DragDropTargetRole.None ||
            (bool)(element.GetValue(TargetAttachedProperty) ?? false))
        {
            return;
        }

        element.SetValue(TargetAttachedProperty, true);
        element.AllowDrop = true;
        element.DragEnter += OnDragEnter;
        element.DragOver += OnDragOver;
        element.DragLeave += OnDragLeave;
        element.Drop += OnDrop;
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

    private static void OnDragEnter(
        object sender,
        DragEventArgs args)
    {
        if (sender is not UIElement target)
        {
            return;
        }

        Highlight(target);
        ApplyEffects(target, args);
        GetSink(target)?.OnDragEntered(
            GetTargetRole(target),
            GetTargetName(target));
    }

    private static void OnDragOver(
        object sender,
        DragEventArgs args)
    {
        if (sender is UIElement target)
        {
            ApplyEffects(target, args);
        }
    }

    private static void OnDragLeave(
        object sender,
        DragEventArgs args)
    {
        if (sender is not UIElement target)
        {
            return;
        }

        Restore(target);
        GetSink(target)?.OnDragLeft(
            GetTargetRole(target),
            GetTargetName(target));
    }

    private static void OnDrop(
        object sender,
        DragEventArgs args)
    {
        if (sender is not UIElement target)
        {
            return;
        }

        Restore(target);
        var sink = GetSink(target);
        var role = GetTargetRole(target);
        var name = GetTargetName(target);

        if (role == DragDropTargetRole.ShellFiles)
        {
            var files =
                args.Data?.GetData(DataFormats.FileDrop) as string[];
            sink?.OnFilesDropped(name, files ?? []);
            return;
        }

        var text =
            args.Data?.GetData("Text") as string ?? "Unknown";
        sink?.OnTextDropped(role, name, text);
    }

    private static void ApplyEffects(
        UIElement target,
        DragEventArgs args)
    {
        if (GetTargetRole(target) ==
            DragDropTargetRole.ShellFiles)
        {
            if (args.Data.GetDataPresent(DataFormats.FileDrop))
            {
                args.Effects = DragDropEffects.Copy;
                args.SetDropDescription(
                    DropImageType.Copy,
                    "Copy to %1",
                    "Gallery demo");
            }
            else
            {
                args.Effects = DragDropEffects.None;
                args.SetDropDescription(
                    DropImageType.None,
                    "Only files are accepted");
            }

            return;
        }

        args.Effects = DragDropEffects.Copy;
    }

    private static IDragDropStateSink? GetSink(
        UIElement element) =>
        (element as FrameworkElement)?.DataContext
            as IDragDropStateSink;

    private static void Highlight(UIElement element)
    {
        if (element is not Border border)
        {
            return;
        }

        border.SetValue(OriginalBorderProperty, border.BorderBrush);
        border.SetValue(
            OriginalBackgroundProperty,
            border.Background);
        border.BorderBrush = HighlightBorder;
        border.Background = HighlightBackground;
    }

    private static void Restore(UIElement element)
    {
        if (element is not Border border)
        {
            return;
        }

        border.BorderBrush =
            border.GetValue(OriginalBorderProperty) as Brush;
        border.Background =
            border.GetValue(OriginalBackgroundProperty) as Brush;
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
