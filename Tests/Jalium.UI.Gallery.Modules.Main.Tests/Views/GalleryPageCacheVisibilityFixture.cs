using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryPageCacheVisibilityFixture
{
    [Fact]
    public void CachedPagesExposeOnlyActiveControlsAndPreserveResourceScopeOnReturn()
    {
        var resolver = new Resolver();
        var host = new GalleryNavigationHost { PageTag = "theme-studio", PageResolver = resolver };
        host.Resources["cached-resource"] = "保留资源";
        Layout(host);
        var cached = resolver.Pages["theme-studio"];
        var cachedSlot = cached.VisualParent;
        Assert.True(cached.IsVisible);
        Assert.Equal("保留资源", cached.FindResource("cached-resource"));

        host.PageTag = "button";
        Layout(host);
        var transient = resolver.Pages["button"];
        Assert.False(cached.IsVisible);
        Assert.True(transient.IsVisible);
        Assert.Same(cachedSlot, cached.VisualParent);
        Assert.Equal(2, host.Items.Count);
        Assert.Equal("保留资源", cached.FindResource("cached-resource"));

        host.PageTag = "theme-studio";
        Layout(host);
        Assert.True(cached.IsVisible);
        Assert.Same(cachedSlot, cached.VisualParent);
        Assert.Single(host.Items);
        Assert.Null(transient.VisualParent);
        Assert.Equal("保留资源", cached.FindResource("cached-resource"));
        Assert.Equal(2, resolver.ResolveCount);
    }

    [Fact]
    public void ReplacingTheHostTemplateReconnectsCachedPagesAndRetiresTheOldPanel()
    {
        var resolver = new Resolver();
        var host = new GalleryNavigationHost { PageTag = "theme-studio", PageResolver = resolver };
        Layout(host);
        var cached = host.ActiveContent;
        var oldRoot = VisualTreeHelper.GetChild(host, 0);
        var original = host.Template;
        host.Template = null;
        Layout(host);
        host.Template = original;
        Layout(host);
        Assert.Same(cached, host.ActiveContent);
        Assert.True(cached!.IsVisible);
        Assert.Null(oldRoot.VisualParent);
        Assert.NotSame(oldRoot, VisualTreeHelper.GetChild(host, 0));
        Assert.Equal(1, resolver.ResolveCount);
        Assert.Equal("Ready", VisualStateManager.GetCurrentStateName(host, "ContentStates"));
    }

    private static void Layout(FrameworkElement host)
    {
        host.Measure(new Size(500, 600));
        host.Arrange(new Rect(0, 0, 500, 600));
        host.UpdateLayout();
    }
    private sealed class Resolver : IGalleryPageResolver
    {
        public Dictionary<string, Button> Pages { get; } = new();
        public int ResolveCount { get; private set; }
        public UIElement Resolve(string tag)
        {
            ResolveCount++;
            return Pages[tag] = new Button { Content = tag };
        }
    }
}
