using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Ink;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts bindable ViewModel state to the mutable InkCanvas drawing API.
/// </summary>
public static class InkCanvasBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(InkCanvasBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty DrawingSettingsProperty =
        DependencyProperty.RegisterAttached(
            "DrawingSettings",
            typeof(InkCanvasDrawingSettings),
            typeof(InkCanvasBehavior),
            new PropertyMetadata(null, OnDrawingAttributeChanged));

    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(InkCanvasRequest),
            typeof(InkCanvasBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    public static void SetDrawingSettings(
        DependencyObject element,
        InkCanvasDrawingSettings? value) =>
        element.SetValue(DrawingSettingsProperty, value);

    public static InkCanvasDrawingSettings? GetDrawingSettings(
        DependencyObject element) =>
        element.GetValue(DrawingSettingsProperty)
            as InkCanvasDrawingSettings;

    public static void SetRequest(
        DependencyObject element,
        InkCanvasRequest? value) =>
        element.SetValue(RequestProperty, value);

    public static InkCanvasRequest? GetRequest(
        DependencyObject element) =>
        element.GetValue(RequestProperty) as InkCanvasRequest;

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not InkCanvas canvas)
        {
            return;
        }

        canvas.StrokeCollected -= OnStrokeCollected;
        canvas.StrokesChanged -= OnStrokesChanged;
        canvas.Loaded -= OnLoaded;

        if (args.NewValue is true)
        {
            canvas.StrokeCollected += OnStrokeCollected;
            canvas.StrokesChanged += OnStrokesChanged;
            canvas.Loaded += OnLoaded;
            ApplyDrawingAttributes(canvas);
            PublishStrokeCount(canvas);
        }
    }

    private static void OnDrawingAttributeChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is InkCanvas canvas)
        {
            ApplyDrawingAttributes(canvas);
        }
    }

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is InkCanvas canvas &&
            args.NewValue is InkCanvasRequest
            {
                Action: InkCanvasAction.Clear
            })
        {
            canvas.ClearStrokes();
            PublishStrokeCount(canvas);
        }
    }

    private static void ApplyDrawingAttributes(InkCanvas canvas)
    {
        var settings = GetDrawingSettings(canvas);
        var width = Math.Max(1, settings?.StrokeWidth ?? 2d);
        canvas.DefaultDrawingAttributes.Color =
            settings?.StrokeColor ?? Color.FromRgb(0, 0, 0);
        canvas.DefaultDrawingAttributes.Width = width;
        canvas.DefaultDrawingAttributes.Height = width;
        canvas.DefaultDrawingAttributes.IsHighlighter =
            settings?.IsHighlighter ?? false;
        canvas.DefaultDrawingAttributes.BrushType =
            settings?.BrushType ?? BrushType.Round;
        canvas.DefaultStrokeTaperMode =
            settings?.TaperMode ?? StrokeTaperMode.None;
    }

    private static void OnLoaded(
        object? sender,
        RoutedEventArgs args)
    {
        if (sender is InkCanvas canvas)
        {
            ApplyDrawingAttributes(canvas);
            PublishStrokeCount(canvas);
        }
    }

    private static void OnStrokeCollected(
        object? sender,
        InkCanvasStrokeCollectedEventArgs args)
    {
        if (sender is InkCanvas canvas)
        {
            PublishStrokeCount(canvas);
        }
    }

    private static void OnStrokesChanged(
        object? sender,
        EventArgs args)
    {
        if (sender is InkCanvas canvas)
        {
            PublishStrokeCount(canvas);
        }
    }

    private static void PublishStrokeCount(InkCanvas canvas) =>
        (canvas.DataContext as IInkCanvasStateSink)
            ?.UpdateStrokeCount(canvas.Strokes.Count);
}
