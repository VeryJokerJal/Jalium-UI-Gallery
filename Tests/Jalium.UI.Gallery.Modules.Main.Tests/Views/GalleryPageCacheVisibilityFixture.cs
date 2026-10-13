using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryPageCacheVisibilityFixture
{
    [Fact]
    public void CachedPages_ExposeOnlyActiveControlsAndPreserveResourceScopeOnReturn()
    {
        var type = typeof(GalleryNavigationHost).GetNestedType("PageCachePanel", BindingFlags.NonPublic)!;
        var host = (Panel)Activator.CreateInstance(type, nonPublic: true)!;
        var activate = type.GetMethod("Activate")!;
        var cached = new Button { Content = "缓存首页操作" };
        var transient = new Button { Content = "当前示例操作" };
        host.Resources["cached-resource"] = "保留资源";

        activate.Invoke(host, [cached, true]);
        var cachedSlot = cached.VisualParent;
        Assert.True(cached.IsVisible);
        Assert.Equal("保留资源", cached.FindResource("cached-resource"));

        activate.Invoke(host, [transient, false]);
        Assert.False(cached.IsVisible);
        Assert.True(transient.IsVisible);
        Assert.Same(cachedSlot, cached.VisualParent);
        Assert.Equal(2, host.Children.Count);
        Assert.Equal("保留资源", cached.FindResource("cached-resource"));

        activate.Invoke(host, [cached, true]);
        Assert.True(cached.IsVisible);
        Assert.Same(cachedSlot, cached.VisualParent);
        Assert.Single(host.Children);
        Assert.Null(transient.VisualParent?.VisualParent);
        Assert.Equal("保留资源", cached.FindResource("cached-resource"));
    }
}
