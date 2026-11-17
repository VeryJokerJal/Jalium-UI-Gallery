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

    /// <summary>Optional explicit state receiver for a nested presentation such as a preview.</summary>
    public static readonly DependencyProperty StateSinkProperty = DependencyProperty.RegisterAttached(
        "StateSink", typeof(IDocumentViewerStateSink), typeof(DocumentViewerBehavior),
        new PropertyMetadata(null, OnStateSinkChanged));

    public static IDocumentViewerStateSink? GetStateSink(DependencyObject element) =>
        element.GetValue(StateSinkProperty) as IDocumentViewerStateSink;
    public static void SetStateSink(DependencyObject element, IDocumentViewerStateSink? value) =>
        element.SetValue(StateSinkProperty, value);
    private static void OnStateSinkChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is DocumentViewer viewer && GetIsEnabled(viewer)) PublishPageState(viewer);
    }

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
        viewer.DocumentLoaded -= OnDocumentLoaded;
        viewer.DataContextChanged -= OnDataContextChanged;

        if (args.NewValue is true)
        {
            viewer.PageChanged += OnPageChanged;
            viewer.SearchCompleted += OnSearchCompleted;
            viewer.Loaded += OnLoaded;
            viewer.DocumentLoaded += OnDocumentLoaded;
            viewer.DataContextChanged += OnDataContextChanged;
            PublishPageState(viewer);
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

    private static void OnDocumentLoaded(object? sender, EventArgs args)
    {
        if (sender is DocumentViewer viewer) PublishPageState(viewer);
    }

    private static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is DocumentViewer viewer) PublishPageState(viewer);
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
            viewer.PageCount);

    private static IDocumentViewerStateSink? Sink(DocumentViewer viewer)
    {
        // An explicit receiver may still be resolving its first binding update.
        // Do not publish transient page/zoom state into the inherited model.
        if (Jalium.UI.Data.BindingOperations.GetBindingExpression(viewer, StateSinkProperty) is not null ||
            viewer.ReadLocalValue(StateSinkProperty) != DependencyProperty.UnsetValue)
            return GetStateSink(viewer);
        return viewer.DataContext as IDocumentViewerStateSink;
    }
}
