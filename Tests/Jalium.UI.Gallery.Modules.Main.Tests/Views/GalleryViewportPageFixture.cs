using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Markup;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryViewportPageFixture
{
    [Fact]
    public void DefaultTemplateFollowsResizingAndMarginWithoutSettingPageHeight()
    {
        _ = new App();
        var page = new GalleryViewportPage { Content = new Border { Height = 120 }, Margin = new Thickness(24) };
        Assert.False(page.IsViewportBound);
        Assert.True(double.IsNaN(page.ViewportHeight));
        var host = Host(page);
        Layout(host, 720);
        Assert.True(double.IsNaN(page.Height));
        Assert.Equal(host.ViewportHeight - 48, page.ViewportHeight, 3);
        Assert.Equal(page.ViewportHeight, Assert.IsType<Border>(page.FindName("PART_Root")).ActualHeight, 3);
        Assert.Equal("Bound", VisualStateManager.GetCurrentStateName(page, "ViewportStates"));
        Layout(host, 480);
        Assert.Equal(host.ViewportHeight - 48, page.ViewportHeight, 3);
        page.MinimumViewportHeight = 500;
        page.MaxHeight = 450;
        Layout(host, 480);
        Assert.Equal(450, page.ViewportHeight);
        page.MinHeight = 500;
        Layout(host, 480);
        Assert.Equal(500, page.ViewportHeight);
    }

    [Fact]
    public void CallerHeightBindingAndMinimumMaximumConstraintsRemainEffective()
    {
        _ = new App();
        var source = new Border { Height = 360 };
        var page = new GalleryViewportPage();
        page.SetBinding(FrameworkElement.HeightProperty, new Binding(nameof(FrameworkElement.Height))
        {
            Source = source, Mode = BindingMode.TwoWay
        });
        var host = Host(page);
        Layout(host, 720);
        Assert.Equal(360, page.ViewportHeight);
        Assert.Equal(0, page.TransformToAncestor(host).Y, 3);
        source.Height = 400;
        Layout(host, 720);
        Assert.Equal(400, page.ViewportHeight);
        Assert.Equal(400, page.ActualHeight, 3);
        Assert.Equal(0, page.TransformToAncestor(host).Y, 3);
        Assert.NotNull(BindingOperations.GetBindingBase(page, FrameworkElement.HeightProperty));
        page.IsViewportHeightEnabled = false;
        Layout(host, 480);
        Assert.Equal(400, source.Height);
        Assert.Equal(400, page.Height);
        Assert.True(double.IsNaN(page.ViewportHeight));
        page.IsViewportHeightEnabled = true;
        page.MinHeight = 450;
        Layout(host, 480);
        Assert.Equal(450, page.ViewportHeight);
        Assert.Equal(400, source.Height);
    }

    [Fact]
    public void ReparentingAndUnloadingRetireThePreviousViewport()
    {
        _ = new App();
        var page = new GalleryViewportPage();
        var first = Host(page);
        Layout(first, 720);
        first.Content = null;
        var second = Host(page);
        Layout(second, 480);
        Assert.Equal(second.ViewportHeight, page.ViewportHeight, 3);
        Layout(first, 840);
        Assert.Equal(second.ViewportHeight, page.ViewportHeight, 3);
        SetLoadedState(page, true);
        SetLoadedState(page, false);
        Assert.False(page.IsViewportBound);
        Assert.True(double.IsNaN(page.ViewportHeight));
        Layout(second, 600);
        Assert.True(double.IsNaN(page.ViewportHeight));
        SetLoadedState(page, true);
        Layout(second, 600);
        Assert.Equal(second.ViewportHeight, page.ViewportHeight, 3);
    }

    [Fact]
    public void AReplacementTemplateCanChooseItsOwnBodyAndRestoreTheDefault()
    {
        _ = new App();
        var page = new GalleryViewportPage();
        var host = Host(page);
        Layout(host, 720);
        var original = page.Template;
        var custom = new ControlTemplate(typeof(GalleryViewportPage));
        custom.SetVisualTree(() => new Border { Name = "CustomBody", Height = 44 });
        page.Template = custom;
        Layout(host, 720);
        Assert.Equal(44, Assert.IsType<Border>(page.FindName("CustomBody")).ActualHeight);
        Assert.Equal(host.ViewportHeight, page.ViewportHeight, 3);
        page.Template = original;
        Layout(host, 720);
        Assert.Equal(page.ViewportHeight, Assert.IsType<Border>(page.FindName("PART_Root")).ActualHeight, 3);
    }

    private static ScrollViewer Host(GalleryViewportPage page) => new()
    {
        Content = page, VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
    };

    private static void SetLoadedState(FrameworkElement element, bool loaded) =>
        typeof(FrameworkElement).GetMethod("SetLoadedState", System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)!.Invoke(element, [loaded]);

    private static void Layout(ScrollViewer host, double height)
    {
        for (int pass = 0; pass < 4; pass++)
        {
            host.InvalidateMeasure();
            host.Measure(new Size(600, height));
            host.Arrange(new Rect(0, 0, 600, height));
            host.UpdateLayout();
        }
    }
}
