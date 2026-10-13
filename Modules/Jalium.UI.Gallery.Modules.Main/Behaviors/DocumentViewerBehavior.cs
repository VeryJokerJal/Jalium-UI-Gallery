using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Translates presentation requests into DocumentViewer control operations and
/// publishes control events back through a UI-agnostic state sink.
/// </summary>
public static class DocumentViewerBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(DocumentViewerBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(DocumentViewerRequest),
            typeof(DocumentViewerBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    public static void SetIsEnabled(DependencyObject element, bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static DocumentViewerRequest? GetRequest(
        DependencyObject element) =>
        element.GetValue(RequestProperty) as DocumentViewerRequest;

    public static void SetRequest(
        DependencyObject element,
        DocumentViewerRequest? value) =>
        element.SetValue(RequestProperty, value);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not DocumentViewer viewer)
        {
            return;
        }

        viewer.PageChanged -= OnPageChanged;
        viewer.SearchCompleted -= OnSearchCompleted;
        viewer.Loaded -= OnLoaded;

        if (args.NewValue is true)
        {
            viewer.PageChanged += OnPageChanged;
            viewer.SearchCompleted += OnSearchCompleted;
            viewer.Loaded += OnLoaded;
        }
    }

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not DocumentViewer viewer ||
            args.NewValue is not DocumentViewerRequest request)
        {
            return;
        }

        switch (request.Action)
        {
            case DocumentViewerAction.FirstPage:
                viewer.FirstPage();
                break;
            case DocumentViewerAction.PreviousPage:
                viewer.PreviousPage();
                break;
            case DocumentViewerAction.NextPage:
                viewer.NextPage();
                break;
            case DocumentViewerAction.LastPage:
                viewer.LastPage();
                break;
            case DocumentViewerAction.ZoomIn:
                viewer.ZoomIn();
                break;
            case DocumentViewerAction.ZoomOut:
                viewer.ZoomOut();
                break;
            case DocumentViewerAction.FitWidth:
                viewer.FitToWidth();
                break;
            case DocumentViewerAction.FitPage:
                viewer.FitToPage();
                break;
            case DocumentViewerAction.Print:
                viewer.Print();
                break;
            case DocumentViewerAction.Find:
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    viewer.Find(
                        request.SearchText,
                        request.MatchCase,
                        request.MatchWholeWord);
                }

                break;
            case DocumentViewerAction.FindNext:
                viewer.FindNext();
                break;
            case DocumentViewerAction.FindPrevious:
                viewer.FindPrevious();
                break;
            case DocumentViewerAction.ClearSearch:
                viewer.ClearSearch();
                Sink(viewer)?.ClearSearchResult();
                break;
        }

        PublishPageState(viewer);
    }

    private static void OnLoaded(object? sender, RoutedEventArgs args)
    {
        if (sender is DocumentViewer viewer)
        {
            PublishPageState(viewer);
        }
    }

    private static void OnPageChanged(object? sender, PageChangedEventArgs args)
    {
        if (sender is DocumentViewer viewer)
        {
            PublishPageState(viewer);
        }
    }

    private static void OnSearchCompleted(
        object? sender,
        SearchCompletedEventArgs args)
    {
        if (sender is DocumentViewer viewer)
        {
            Sink(viewer)?.UpdateSearchResult(args.ResultCount);
        }
    }

    private static void PublishPageState(DocumentViewer viewer) =>
        Sink(viewer)?.UpdatePageState(
            viewer.CurrentPage,
            viewer.PageCount,
            viewer.Zoom);

    private static IDocumentViewerStateSink? Sink(DocumentViewer viewer) =>
        viewer.DataContext as IDocumentViewerStateSink;
}
