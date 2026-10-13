using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// Applies the catalog/detail breakpoint before Grid measures its children.
/// The shell measures this page against its current viewport, so both the
/// breakpoint and the star-sized catalog rows settle in the same layout pass.
/// </summary>
public sealed class HomeWorkspaceGrid : Grid
{
    internal const double SplitViewWidth = 1040;
    internal const double DetailColumnWidth = 376;

    private const string EmbeddedScrollTemplateKey =
        "HomeCatalogEmbeddedScrollTemplate";
    private const string PageScrollTemplateKey =
        "HomeCatalogPageScrollTemplate";
    private const string EmbeddedScrollItemsPanelKey =
        "HomeCatalogEmbeddedScrollItemsPanel";
    private const string PageScrollItemsPanelKey =
        "HomeCatalogPageScrollItemsPanel";

    private Grid? _catalog;
    private ColumnDefinition? _detailColumn;
    private Grid? _detailHost;
    private Border? _detail;
    private HomeCatalogListBox? _componentList;
    private TextBlock? _previewSubtitle;
    private TextBlock? _directOpenSubtitle;
    private bool? _usesEmbeddedCatalogScroll;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (double.IsFinite(availableSize.Width))
        {
            ApplyLayout(availableSize.Width);
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (double.IsFinite(finalSize.Width))
        {
            ApplyLayout(finalSize.Width);
        }

        return base.ArrangeOverride(finalSize);
    }

    private void ApplyLayout(double width)
    {
        if (!TryResolveParts())
        {
            return;
        }

        var hasVisibleItems =
            _detail!.Visibility == Visibility.Visible;
        var splitView = width >= SplitViewWidth;
        SetScrollableRowHeights(splitView);
        SetCatalogScrollMode(splitView);
        SetVisibility(_detailHost!, splitView);
        SetVisibility(_previewSubtitle!, splitView);
        SetVisibility(_directOpenSubtitle!, !splitView);

        if (splitView)
        {
            SetDetailColumnWidth(
                hasVisibleItems
                    ? DetailColumnWidth
                    : 0);
            SetGridPosition(_detailHost!, column: 1, row: 0);
            SetIfChanged(
                _detail,
                margin: new Thickness(16, 0, 0, 0),
                borderThickness: new Thickness(1),
                padding: new Thickness(20, 18, 12, 20));
            SetMargin(_catalog!, new Thickness(0));
            return;
        }

        SetDetailColumnWidth(0);
        SetGridPosition(_detailHost!, column: 0, row: 1);
        SetIfChanged(
            _detail,
            margin: new Thickness(0, 24, 0, 0),
            borderThickness: new Thickness(1),
            padding: new Thickness(8, 18, 8, 20));
        SetMargin(_catalog!, new Thickness(0));
    }

    private void SetScrollableRowHeights(bool splitView)
    {
        var height = splitView
            ? new GridLength(1, GridUnitType.Star)
            : GridLength.Auto;

        if (RowDefinitions.Count > 0 &&
            RowDefinitions[0].Height != height)
        {
            RowDefinitions[0].Height = height;
        }

        if (_catalog!.RowDefinitions.Count > 2 &&
            _catalog.RowDefinitions[2].Height != height)
        {
            _catalog.RowDefinitions[2].Height = height;
        }
    }

    private bool TryResolveParts()
    {
        _catalog ??= FindName("CatalogPanel") as Grid;
        _detailColumn ??=
            FindName("DetailColumn") as ColumnDefinition;
        _detailHost ??= FindName("DetailHost") as Grid;
        _detail ??= FindName("DetailPanel") as Border;
        _componentList ??=
            FindName("ComponentList") as HomeCatalogListBox;
        _previewSubtitle ??=
            FindName("CatalogPreviewSubtitle") as TextBlock;
        _directOpenSubtitle ??=
            FindName("CatalogDirectOpenSubtitle") as TextBlock;

        return _catalog != null &&
               _detailColumn != null &&
               _detailHost != null &&
               _detail != null &&
               _componentList != null &&
               _previewSubtitle != null &&
               _directOpenSubtitle != null;
    }

    private void SetCatalogScrollMode(bool useEmbeddedScroll)
    {
        _componentList!.IsDirectActivationEnabled = !useEmbeddedScroll;

        if (_usesEmbeddedCatalogScroll == useEmbeddedScroll)
        {
            return;
        }

        var templateKey = useEmbeddedScroll
            ? EmbeddedScrollTemplateKey
            : PageScrollTemplateKey;
        var itemsPanelKey = useEmbeddedScroll
            ? EmbeddedScrollItemsPanelKey
            : PageScrollItemsPanelKey;
        if (TryFindResource(templateKey) is not ControlTemplate template ||
            TryFindResource(itemsPanelKey) is not ItemsPanelTemplate itemsPanel)
        {
            return;
        }

        // The page-scrolling template deliberately has no nested ScrollViewer.
        // This lets HomeCatalogPanel observe the shell viewport and keep the
        // stacked catalog virtualized even though it reports its full extent.
        if (!ReferenceEquals(_componentList!.Template, template))
        {
            _componentList.Template = template;
        }

        if (!ReferenceEquals(_componentList.ItemsPanel, itemsPanel))
        {
            _componentList.ItemsPanel = itemsPanel;
        }

        _usesEmbeddedCatalogScroll = useEmbeddedScroll;
    }

    private static void SetVisibility(
        UIElement element,
        bool isVisible)
    {
        var visibility = isVisible
            ? Visibility.Visible
            : Visibility.Collapsed;
        if (element.Visibility != visibility)
        {
            element.Visibility = visibility;
        }
    }

    private void SetDetailColumnWidth(double width)
    {
        var value = new GridLength(width);
        if (_detailColumn!.Width != value)
        {
            _detailColumn.Width = value;
        }
    }

    private static void SetGridPosition(
        UIElement element,
        int column,
        int row)
    {
        if (GetColumn(element) != column)
        {
            SetColumn(element, column);
        }

        if (GetRow(element) != row)
        {
            SetRow(element, row);
        }
    }

    private static void SetIfChanged(
        Border detail,
        Thickness margin,
        Thickness borderThickness,
        Thickness padding)
    {
        SetMargin(detail, margin);

        if (detail.BorderThickness != borderThickness)
        {
            detail.BorderThickness = borderThickness;
        }

        if (detail.Padding != padding)
        {
            detail.Padding = padding;
        }
    }

    private static void SetMargin(
        FrameworkElement element,
        Thickness margin)
    {
        if (element.Margin != margin)
        {
            element.Margin = margin;
        }
    }
}
