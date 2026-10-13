using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// Virtualizes the component catalog inside its own scroll viewport and
/// derives an exact, responsive card width before realizing any containers.
/// </summary>
public sealed class HomeCatalogPanel : VirtualizingWrapPanel
{
    internal const double TwoColumnWidth = 640;
    internal const double ThreeColumnWidth = 780;
    private const double CardHeight = 198;
    private const double CardSpacing = 12;

    public int Columns { get; private set; } = 1;

    public HomeCatalogPanel()
    {
        ItemHeight = CardHeight;
        HorizontalSpacing = CardSpacing;
        VerticalSpacing = CardSpacing;
        UseAncestorScrollViewer = false;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var columns = GetColumnCount(availableSize.Width);
        Columns = columns;
        if (double.IsFinite(availableSize.Width) &&
            availableSize.Width > 0)
        {
            var itemWidth = Math.Max(
                1,
                (availableSize.Width -
                 ((columns - 1) * CardSpacing)) /
                columns);
            if (double.IsNaN(ItemWidth) ||
                Math.Abs(ItemWidth - itemWidth) > 0.01)
            {
                ItemWidth = itemWidth;
            }
        }

        return base.MeasureOverride(availableSize);
    }

    private static int GetColumnCount(double width)
    {
        if (!double.IsFinite(width) || width < TwoColumnWidth)
        {
            return 1;
        }

        return width >= ThreeColumnWidth ? 3 : 2;
    }
}
