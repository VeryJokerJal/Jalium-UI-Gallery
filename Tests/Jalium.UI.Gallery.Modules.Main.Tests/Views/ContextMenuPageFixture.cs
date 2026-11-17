using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;
public sealed class ContextMenuPageFixture
{
    [Theory]
    [InlineData(960)]
    [InlineData(390)]
    public void RealMenuCommandsAndLocalizedEmptyStateFollowTheCurrentViewModel(double width)
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var model = new ContextMenuViewModel();
        var page = new ContextMenuPage
        {
            DataContext = model
        };
        var window = new Window
        {
            Width = width,
            Height = 1100,
            Content = page
        };
        var theme = new GalleryThemeService();
        try
        {
            if (OperatingSystem.IsMacOS())
                typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(window, new object[] { Visibility.Visible });
            Layout();
            var area = Assert.IsAssignableFrom<Control>(page.FindName("InteractiveArea"));
            var menu = Assert.IsType<ContextMenu>(ContextMenuService.GetContextMenu(area));
            var empty = Assert.IsType<TextBlock>(page.FindName("EmptyActionText"));
            var selected = Assert.IsType<TextBlock>(page.FindName("SelectedActionText"));
            Assert.Equal(Visibility.Visible, empty.Visibility);
            Assert.Equal(LocalizationManager.Get("Loc.P.ContextMenu.Menu5"), empty.Text);
            Assert.Equal(Visibility.Collapsed, selected.Visibility);
            ContextMenuService.Open(area, menu, new Point(30, 50));
            Layout();
            var item = Assert.IsType<MenuItem>(menu.Items[1]);
            Assert.Same(model.SelectActionCommand, item.Command);
            Invoke(item);
            Assert.Equal("Action 2", model.SelectedAction);
            Assert.False(menu.IsOpen);
            Assert.Equal("Action 2", selected.Text);
            Assert.Equal(Visibility.Collapsed, empty.Visibility);
            theme.SetDark(true);
            var replacement = new ContextMenuViewModel();
            page.DataContext = replacement;
            Layout();
            ContextMenuService.Open(area, menu, new Point(30, 50));
            Layout();
            Assert.Same(replacement.SelectActionCommand, Assert.IsType<MenuItem>(menu.Items[2]).Command);
            Invoke((MenuItem)menu.Items[2]);
            Assert.Equal("Action 3", replacement.SelectedAction);
            Assert.Equal("Action 2", model.SelectedAction);
            Assert.Equal("Action 3", selected.Text);
            Assert.False(menu.IsOpen);
            Assert.DoesNotContain("Click +=", replacement.CSharpExample);
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
            window.Measure(new Size(width, 1100));
            window.Arrange(new Rect(0, 0, width, 1100));
            window.UpdateLayout();
        }
    }

    private static void Invoke(MenuItem item) => item.RaiseEvent(new KeyEventArgs(UIElement.KeyDownEvent, Key.Enter, ModifierKeys.None, true, false, 0));
}
