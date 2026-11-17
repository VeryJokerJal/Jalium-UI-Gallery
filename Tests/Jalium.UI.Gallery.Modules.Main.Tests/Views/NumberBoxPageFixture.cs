using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class NumberBoxPageFixture
{
    [Theory]
    [InlineData(960)]
    [InlineData(390)]
    public void ValuesAndSpinCommandsStayBoundAcrossThemeChanges(double width)
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var model = new NumberBoxViewModel();
        var page = new NumberBoxPage { DataContext = model };
        var window = new Window { Width = width, Height = 1000, Content = page };
        var theme = new GalleryThemeService();
        try
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(window, new object[] { Visibility.Visible });
            Layout();
            var basic = Assert.IsType<NumberBox>(page.FindName("BasicNumberBox"));
            var inline = Assert.IsType<NumberBox>(page.FindName("InlineNumberBox"));
            var ranged = Assert.IsType<NumberBox>(page.FindName("RangedNumberBox"));
            var stepped = Assert.IsType<NumberBox>(page.FindName("SteppedNumberBox"));
            foreach (var box in new[] { basic, inline, ranged, stepped })
            {
                Assert.InRange(box.ActualWidth, 1, 300);
                Assert.NotNull(BindingOperations.GetBindingExpression(box, NumberBox.ValueProperty));
            }
            var label = Assert.IsType<TextBlock>(page.FindName("BasicValueLabel"));
            Assert.Equal(label.VisualBounds.X, basic.VisualBounds.X, 3);
            Assert.Equal(LocalizationManager.Get("Loc.NumberBox.Basic"), Jalium.UI.Automation.AutomationProperties.GetName(basic));
            Assert.Equal(42, basic.Value);
            model.BasicValue = 71;
            model.RangedValue = 62;
            Layout();
            Assert.Equal(71, basic.Value);
            Assert.Equal(62, ranged.Value);
            var button = Assert.IsType<Jalium.UI.Controls.Primitives.RepeatButton>(inline.FindName("PART_UpSpinButton"));
            button.PerformClick();
            Assert.Equal(11, model.InlineValue);
            theme.SetDark(true);
            Layout();
            Assert.Same(button, inline.FindName("PART_UpSpinButton"));
            button.PerformClick();
            Assert.Equal(12, model.InlineValue);
            Assert.Equal(12, inline.Value);
            Assert.Contains("{Binding RangedValue, Mode=TwoWay}", model.XamlExample);
            Assert.DoesNotContain("InitializeComponent", model.CSharpExample);
        }
        finally
        {
            window.Content = null;
            theme.SetDark(false);
            LocalizationManager.SetLanguage(language);
            GC.KeepAlive(app);
        }
        void Layout()
        {
            window.Measure(new Size(width, 1000)); window.Arrange(new Rect(0, 0, width, 1000)); window.UpdateLayout();
        }
    }
}
