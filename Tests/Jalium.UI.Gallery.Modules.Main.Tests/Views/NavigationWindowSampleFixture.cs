using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Navigation;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class NavigationWindowSampleFixture
{
    [Theory]
    [InlineData(600)]
    [InlineData(390)]
    public void TemplatedSampleCreatesHistoryAndChangesChromeWithoutRecreatingThePage(double width)
    {
        var originalLanguage = LocalizationManager.CurrentLanguage;
        var app = new App();
        var window = new GalleryNavigationService().CreateNavigationWindowDemo();
        try
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(window, new object[] { Visibility.Visible });
            window.Width = width;
            Layout(window, width);
            var first = Assert.IsType<NavigationWindowSamplePage>(window.Content);
            Assert.Equal("01", Assert.IsType<TextBlock>(first.FindName("PART_PageNumber")).Text);
            Assert.False(string.IsNullOrWhiteSpace(first.Title));
            Assert.Equal(first.Title, window.Title);
            Assert.InRange(first.ActualWidth, 1, width);
            Assert.True(Assert.IsType<CheckBox>(first.FindName("PART_ShowNavigation")).IsChecked);
            Assert.IsType<Button>(first.FindName("PART_NextPageButton")).PerformClick();
            Layout(window, width);
            var second = Assert.IsType<NavigationWindowSamplePage>(window.Content);
            Assert.Equal(2, Assert.IsType<NavigationWindowSampleViewModel>(second.DataContext).Number);
            Assert.Equal("02", Assert.IsType<TextBlock>(second.FindName("PART_PageNumber")).Text);
            Assert.True(window.CanGoBack);
            window.GoBackCommand.Execute(null);
            Layout(window, width);
            Assert.Same(first, window.Content);
            Assert.True(window.CanGoForward);
            window.GoForwardCommand.Execute(null);
            Layout(window, width);
            Assert.Same(second, window.Content);
            var chrome = Assert.IsType<CheckBox>(second.FindName("PART_ShowNavigation"));
            chrome.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
            Layout(window, width);
            Assert.False(window.ShowsNavigationUI);
            Assert.Equal(Visibility.Collapsed, Assert.IsAssignableFrom<FrameworkElement>(window.FindName("PART_NavigationBar")).Visibility);
            Assert.Same(second, window.Content);
            chrome.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
            Assert.True(window.ShowsNavigationUI);
            window.RefreshCommand.Execute(null);
            Assert.Same(second, window.Content);
        }
        finally { window.Close(); LocalizationManager.SetLanguage(originalLanguage); GC.KeepAlive(app); }
    }

    private static void Layout(NavigationWindow window, double width)
    {
        window.Measure(new Size(width, 400)); window.Arrange(new Rect(0, 0, width, 400)); window.UpdateLayout();
    }
}
