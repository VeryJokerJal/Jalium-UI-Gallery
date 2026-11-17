using System.Collections;
using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class TreeDataGridPageFixture
{
    [Theory]
    [InlineData(960)]
    [InlineData(390)]
    public void DeclarativeColumnsAndControlCommandsStayBoundAcrossLanguagesAndThemes(double width)
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var model = new TreeDataGridViewModel();
        var page = new TreeDataGridPage { DataContext = model };
        var window = new Window { Width = width, Height = 1000, Content = page };
        var theme = new GalleryThemeService();
        try
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(window, new object[] { Visibility.Visible });
            Layout();
            var grid = Assert.IsType<TreeDataGrid>(page.FindName("DemoTreeDataGrid"));
            var expand = Assert.IsType<Button>(page.FindName("ExpandAllButton"));
            var collapse = Assert.IsType<Button>(page.FindName("CollapseAllButton"));
            Assert.Equal(3, grid.Columns.Count);
            Assert.Equal(250, grid.Columns[0].Width.Value);
            Assert.Equal(250, grid.Columns[0].ActualWidth);
            Assert.InRange(Assert.IsType<StackPanel>(grid.FindName("PART_ColumnHeadersHost")).Children.OfType<Jalium.UI.Controls.Primitives.DataGridColumnHeader>()
                .First().ActualWidth, 249, 251);
            Assert.Same(grid.ExpandAllCommand, expand.Command);
            Assert.Same(grid.CollapseAllCommand, collapse.Command);
            Assert.False(collapse.IsEnabled);
            var initialCount = NodeCount(grid);
            expand.PerformClick(); Layout();
            Assert.True(NodeCount(grid) > initialCount);
            Assert.True(collapse.IsEnabled);
            collapse.PerformClick(); Layout();
            Assert.Equal(initialCount, NodeCount(grid));
            grid.SelectedItem = model.Items[0];
            Assert.Same(model.Items[0], model.SelectedItem);
            foreach (var locale in Enum.GetValues<GalleryLanguage>())
            {
                LocalizationManager.SetLanguage(locale);
                Layout();
                Assert.Equal(LocalizationManager.Get("Loc.P.TreeDataGrid.Name"), grid.Columns[0].Header);
                Assert.Equal(LocalizationManager.Get("Loc.P.TreeDataGrid.Name"), Assert.IsType<StackPanel>(grid.FindName("PART_ColumnHeadersHost"))
                    .Children.OfType<Jalium.UI.Controls.Primitives.DataGridColumnHeader>().First().Content);
                Assert.Equal(LocalizationManager.Get("Loc.P.TreeDataGrid.ExpandAll"), expand.Content);
            }
            theme.SetDark(true); Layout();
            Assert.Same(grid, page.FindName("DemoTreeDataGrid"));
            Assert.Same(expand, page.FindName("ExpandAllButton"));
            expand.PerformClick(); Layout();
            Assert.True(NodeCount(grid) > initialCount);
            Assert.DoesNotContain("new TreeDataGrid", model.CSharpExample);
            Assert.Contains("TreeDataGrid.Columns", model.XamlExample);
        }
        finally
        {
            window.Content = null;
            theme.SetDark(false);
            LocalizationManager.SetLanguage(language);
            GC.KeepAlive(app);
        }
        void Layout() { window.Measure(new Size(width, 1000)); window.Arrange(new Rect(0, 0, width, 1000)); window.UpdateLayout(); }
    }

    private static int NodeCount(TreeDataGrid grid) => ((ICollection)typeof(TreeDataGrid)
        .GetField("_flattenedNodes", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(grid)!).Count;
}
