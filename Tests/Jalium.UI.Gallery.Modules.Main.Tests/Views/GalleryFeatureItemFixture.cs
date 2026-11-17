using Jalium.UI.Automation;
using Jalium.UI.Automation.Peers;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryFeatureItemFixture
{
    [Theory]
    [InlineData(160)]
    [InlineData(260)]
    public void LongLocalizedTextWrapsWithinTheItemAndKeepsItsAccessibleName(double width)
    {
        var app = new App();
        var item = new GalleryFeatureItem { Text = "复用 DataGrid 的文本、复选框与模板列；窄容器中的文字应自然换行，并保持内容完整可读。" };
        Layout();
        var text = Assert.IsType<TextBlock>(item.FindName("PART_Text"));
        Assert.InRange(text.ActualWidth, 1, width - 30);
        Assert.True(text.ActualHeight > text.FontSize * 2);
        Assert.False(item.Focusable);
        var peer = FrameworkElementAutomationPeer.CreatePeerForElement(item)!;
        Assert.Equal(AutomationControlType.Text, peer.GetAutomationControlType());
        Assert.Equal(item.Text, peer.GetName());
        Assert.Empty(peer.GetChildren() ?? []);
        AutomationProperties.SetName(item, "Feature");
        Assert.Equal("Feature", peer.GetName());
        var original = item.Template;
        item.IsEnabled = false;
        Assert.Equal("Disabled", VisualStateManager.GetCurrentStateName(item, "CommonStates"));
        var replacement = new ControlTemplate(typeof(GalleryFeatureItem));
        replacement.SetVisualTree(() => new Border());
        item.Template = replacement;
        item.Text = "另一项需要换行的较长文本，在替换模板后仍然完整呈现，并在主题切换后保留可读性。";
        item.Template = original;
        item.IsEnabled = true;
        var theme = new GalleryThemeService();
        try
        {
            theme.SetDark(true);
            Layout();
            var current = Assert.IsType<TextBlock>(item.FindName("PART_Text"));
            Assert.NotSame(text, current);
            Assert.Equal(item.Text, current.Text);
            Assert.InRange(current.ActualWidth, 1, width - 30);
            Assert.True(current.ActualHeight > current.FontSize * 2);
            Assert.Equal("Normal", VisualStateManager.GetCurrentStateName(item, "CommonStates"));
        }
        finally { theme.SetDark(false); GC.KeepAlive(app); }
        void Layout()
        {
            item.Measure(new Size(width, 1000));
            item.Arrange(new Rect(0, 0, width, item.DesiredSize.Height));
            item.UpdateLayout();
        }
    }
}
