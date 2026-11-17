using Jalium.UI.Controls;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class JsonTreeViewerPageFixture
{
    [Theory]
    [InlineData(960, false)]
    [InlineData(390, true)]
    public void InteractiveEditorFitsTheViewportAndOffersTypedEditing(double width, bool compact)
    {
        var page = new JsonTreeViewerPage { DataContext = new JsonTreeViewerViewModel() };
        using var stream = new MemoryStream();
        var defaults = ThemeManager.XamlLoader!(stream, "Themes/Generic.jalxaml", typeof(Control).Assembly)!;
        ThemeManager.RegisterControlThemes(new ResourceDictionary { [typeof(TreeViewItem)] = defaults[typeof(TreeViewItem)] });
        page.Resources.MergedDictionaries.Add(defaults);
        page.Measure(new Size(width, double.PositiveInfinity));
        page.Arrange(new Rect(0, 0, width, page.DesiredSize.Height));
        page.UpdateLayout();
        var editorPanel = Assert.IsType<StackPanel>(page.FindName("PART_InteractiveEditorPanel"));
        var treePanel = Assert.IsType<StackPanel>(page.FindName("PART_InteractiveTreePanel"));
        Assert.Equal(compact ? 2 : 1, Grid.GetColumnSpan(editorPanel));
        Assert.Equal(compact ? 0 : 1, Grid.GetColumn(treePanel));
        Assert.Equal(compact ? 1 : 0, Grid.GetRow(treePanel));
        Assert.InRange(editorPanel.ActualWidth, 1, width);
        Assert.InRange(treePanel.ActualWidth, 1, width);
        var tree = Assert.IsType<JsonTreeViewer>(page.FindName("InteractiveJsonTree"));
        Assert.True(tree.IsEditable);
        var count = tree.RootNode!.Children.Single(node => node.Key == "count");
        Assert.True(tree.BeginEditNode(count));
        var row = Descendants<JsonTreeNodePresenter>(tree).Single(item => ReferenceEquals(item.Node, count));
        row.EditText = "64";
        Assert.True(row.CommitEdit());
        Assert.Equal(64L, count.Value);
        using var json = System.Text.Json.JsonDocument.Parse(((JsonTreeViewerViewModel)page.DataContext!).InteractiveJson);
        Assert.Equal(64, json.RootElement.GetProperty("count").GetInt32());
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
