using Jalium.UI.Controls;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class PropertyGridPageFixture
{
    [Theory]
    [InlineData(960, false)]
    [InlineData(390, true)]
    public void SharedObjectEditsSynchronizeAllExamplesAndTheComparisonFitsTheViewport(double width, bool compact)
    {
        var model = new PropertyGridViewModel();
        var page = new PropertyGridPage { DataContext = model };
        using var stream = new MemoryStream();
        var defaults = ThemeManager.XamlLoader!(stream, "Themes/Generic.jalxaml", typeof(Control).Assembly)!;
        ThemeManager.RegisterControlThemes(new ResourceDictionary { [typeof(TextBox)] = defaults[typeof(TextBox)] });
        page.Resources.MergedDictionaries.Add(defaults);
        page.Measure(new Size(width, double.PositiveInfinity));
        page.Arrange(new Rect(0, 0, width, page.DesiredSize.Height));
        page.UpdateLayout();
        var categorizedPanel = Assert.IsType<StackPanel>(page.FindName("PART_CategorizedPanel"));
        var alphabeticalPanel = Assert.IsType<StackPanel>(page.FindName("PART_AlphabeticalPanel"));
        Assert.Equal(compact ? 2 : 1, Grid.GetColumnSpan(categorizedPanel));
        Assert.Equal(compact ? 0 : 1, Grid.GetColumn(alphabeticalPanel));
        Assert.Equal(compact ? 1 : 0, Grid.GetRow(alphabeticalPanel));
        Assert.InRange(categorizedPanel.ActualWidth, 1, width);
        Assert.InRange(alphabeticalPanel.ActualWidth, 1, width);
        var basic = Assert.IsType<PropertyGrid>(page.FindName("BasicPropertyGrid"));
        var editor = Descendants<PropertyGridEditor>(basic).Single(item => item.Property?.Name == nameof(PropertyGridButtonSample.Content));
        Assert.True(editor.BeginEdit());
        editor.EditText = "Updated button";
        Assert.True(editor.CommitEdit());
        Assert.Equal("Updated button", model.BasicSample.Content);
        foreach (var name in new[] { "CategorizedGrid", "AlphabeticalGrid" })
        {
            var grid = Assert.IsType<PropertyGrid>(page.FindName(name));
            var sibling = Descendants<PropertyGridEditor>(grid).Single(item => item.Property?.Name == nameof(PropertyGridButtonSample.Content));
            Assert.Equal("Updated button", sibling.ValueText);
        }
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject root) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i)!;
            if (child is T target) yield return target;
            foreach (var nested in Descendants<T>(child)) yield return nested;
        }
    }
}
