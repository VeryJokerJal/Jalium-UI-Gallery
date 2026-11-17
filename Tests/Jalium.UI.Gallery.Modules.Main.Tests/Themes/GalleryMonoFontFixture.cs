using Jalium.UI.Controls;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Themes;

public sealed class GalleryMonoFontFixture
{
    [Fact]
    public void DockCodeFollowsScopedMonoResourcesAndPreservesExplicitFontOverrides()
    {
        var page = new DockLayoutPage();
        using var stream = new MemoryStream();
        page.Resources.MergedDictionaries.Add(ThemeManager.XamlLoader!(stream,
            "Themes/Generic.jalxaml", typeof(Control).Assembly)!);
        page.Resources.MergedDictionaries.Add(new GalleryControlThemes());
        page.Resources["MonoFontFamily"] = new FontFamily("Menlo");
        Layout(page);
        var text = Descendants(page).OfType<TextBlock>().First(block => block.Text == "using Jalium.UI;");
        Assert.Equal("Menlo", text.FontFamily.Source);
        page.Resources["MonoFontFamily"] = new FontFamily("Consolas"); Layout(page);
        Assert.Equal("Consolas", text.FontFamily.Source);
        text.FontFamily = new FontFamily("Courier New");
        page.Resources["MonoFontFamily"] = new FontFamily("Menlo"); Layout(page);
        Assert.Equal("Courier New", text.FontFamily.Source);
    }

    [Fact]
    public void NarrowDockCodeKeepsItsFullContentHorizontallyScrollable()
    {
        var page = new DockLayoutPage();
        using var stream = new MemoryStream();
        page.Resources.MergedDictionaries.Add(ThemeManager.XamlLoader!(stream,
            "Themes/Generic.jalxaml", typeof(Control).Assembly)!);
        page.Resources.MergedDictionaries.Add(new GalleryControlThemes());
        page.Resources["MonoFontFamily"] = new FontFamily("Menlo");
        page.Measure(new Size(480, 1800)); page.Arrange(new Rect(0, 0, 480, 1800)); page.UpdateLayout();
        var text = Descendants(page).OfType<TextBlock>().First(block => block.Text == "using Jalium.UI;");
        ScrollViewer? scroll = null;
        for (var ancestor = VisualTreeHelper.GetParent(text); ancestor is not null; ancestor = VisualTreeHelper.GetParent(ancestor))
            if (ancestor is ScrollViewer candidate) { scroll = candidate; break; }
        Assert.NotNull(scroll); Assert.Equal(ScrollBarVisibility.Auto, scroll.HorizontalScrollBarVisibility);
        Assert.True(scroll.ScrollableWidth > 0);
        scroll.ScrollToHorizontalOffset(scroll.ScrollableWidth); page.UpdateLayout();
        Assert.True(scroll.HorizontalOffset > 0);
    }

    private static IEnumerable<Visual> Descendants(Visual root)
    {
        for (var i = 0; i < root.VisualChildrenCount; i++)
            if (root.GetVisualChild(i) is Visual child)
            {
                yield return child;
                foreach (var nested in Descendants(child)) yield return nested;
            }
    }

    private static void Layout(FrameworkElement element)
    {
        element.InvalidateMeasure(); element.Measure(new Size(1000, 1800));
        element.Arrange(new Rect(0, 0, 1000, 1800)); element.UpdateLayout();
    }
}
