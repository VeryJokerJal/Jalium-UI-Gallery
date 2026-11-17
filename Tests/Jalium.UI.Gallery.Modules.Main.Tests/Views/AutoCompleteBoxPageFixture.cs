using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class AutoCompleteBoxPageFixture
{
    [Theory]
    [InlineData(960)]
    [InlineData(390)]
    public void SearchAndSelectionBindingsSurviveThemeChangesAtBothPageWidths(double width)
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var model = new AutoCompleteBoxViewModel();
        var page = new AutoCompleteBoxPage { DataContext = model };
        var window = new Window { Width = width, Height = 1000, Content = page };
        var theme = new GalleryThemeService();
        try
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(window, new object[] { Visibility.Visible });
            Layout(window, width);
            var box = Assert.IsType<AutoCompleteBox>(page.FindName("DemoAutoComplete"));
            var input = box.FindName("PART_ContentHost");
            var list = Assert.IsType<ListBox>(box.FindName("PART_SuggestionsList"));
            Assert.InRange(box.ActualWidth, 1, 300);
            Assert.NotNull(BindingOperations.GetBindingExpression(box, AutoCompleteBox.TextProperty));
            model.SearchText = "py";
            Layout(window, width);
            Assert.Equal("py", box.Text);
            Assert.Equal("Python", Assert.Single(box.FilteredItems));
            Assert.True(box.IsDropDownOpen);
            theme.SetDark(true);
            Layout(window, width);
            Assert.Same(input, box.FindName("PART_ContentHost"));
            Assert.Same(list, box.FindName("PART_SuggestionsList"));
            Press(box, Key.Enter);
            Assert.Equal("Python", model.SelectedLanguage);
            Assert.Equal("Python", model.SearchText);
            Assert.Equal("Python", Assert.IsType<TextBlock>(page.FindName("SelectedItemText")).Text);
            Assert.False(box.IsDropDownOpen);
            model.SearchText = "ja";
            Assert.Null(model.SelectedLanguage);
            Assert.Equal(new[] { "JavaScript", "Java" }, box.FilteredItems);
            Press(box, Key.Down);
            Press(box, Key.Enter);
            Assert.Equal("Java", model.SelectedLanguage);
            Assert.Equal("Java", model.SearchText);
            Assert.Contains("ItemsSource=\"{Binding Languages}\"", model.XamlExample);
            Assert.Same(box.SuggestionItemContainerStyle, list.ItemContainerStyle);
        }
        finally
        {
            window.Content = null;
            theme.SetDark(false);
            LocalizationManager.SetLanguage(language);
            GC.KeepAlive(app);
        }
    }

    private static void Layout(Window window, double width)
    {
        window.Measure(new Size(width, 1000));
        window.Arrange(new Rect(0, 0, width, 1000));
        window.UpdateLayout();
    }
    private static void Press(UIElement target, Key key)
        => target.RaiseEvent((KeyEventArgs)Activator.CreateInstance(typeof(KeyEventArgs), BindingFlags.Instance | BindingFlags.NonPublic,
            null, new object[] { UIElement.KeyDownEvent, key, ModifierKeys.None, true, false, 0 }, null)!);
}
