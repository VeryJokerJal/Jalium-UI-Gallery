using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class DataGridPageFixture
{
    [Theory]
    [InlineData(960)]
    [InlineData(390)]
    public void EditingCommitsToTheRowAndSelectedSummaryWhileCancelKeepsTheOriginal(double width)
    {
        var app = new App();
        var model = new DataGridViewModel();
        var page = new DataGridPage { DataContext = model };
        var provider = FocusService.Provider;
        Keyboard.Initialize();
        try
        {
            Layout();
            var grid = Assert.IsType<DataGrid>(page.FindName("DemoDataGrid"));
            var summary = Assert.IsType<TextBlock>(page.FindName("SelectedItemText"));
            grid.SelectedItem = model.People[0];
            grid.CurrentCell = new DataGridCellInfo(model.People[0], grid.Columns.First(c => c.Header?.ToString() == nameof(PersonData.FirstName)));
            Assert.True(grid.Focus());
            Assert.True(grid.BeginEdit());
            Layout();
            var editor = Assert.Single(Descendants<TextBox>(grid));
            Assert.Same(editor, Keyboard.FocusedElement);
            Assert.Equal(editor.Text.Length, editor.SelectionLength);
            editor.Text = "Ada";
            Assert.True(grid.CommitEdit());
            Layout();
            Assert.Equal("Ada", model.People[0].FirstName);
            Assert.Equal("Ada Smith", model.SelectedPersonText);
            Assert.Equal("Ada Smith", summary.Text);
            Assert.True(grid.BeginEdit());
            Layout();
            Assert.Single(Descendants<TextBox>(grid)).Text = "Discarded";
            Assert.True(grid.CancelEdit());
            Layout();
            Assert.Equal("Ada", model.People[0].FirstName);
            Assert.Equal("Ada Smith", summary.Text);
            Assert.Empty(Descendants<TextBox>(grid));
        }
        finally { Keyboard.ClearFocus(); FocusService.Provider = provider; GC.KeepAlive(app); }
        void Layout() { page.Measure(new Size(width, 1600)); page.Arrange(new Rect(0, 0, width, 1600)); page.UpdateLayout(); }
    }

    [Fact]
    public void SelectedSummaryStopsObservingThePreviousRow()
    {
        var model = new DataGridViewModel { SelectedPerson = null };
        model.SelectedPerson = model.People[0];
        model.SelectedPerson = model.People[1];
        var changes = 0;
        model.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(model.SelectedPersonText)) ++changes; };
        model.People[0].FirstName = "Other";
        Assert.Equal(0, changes);
        model.People[1].FirstName = "Robert";
        Assert.Equal(1, changes);
        Assert.Equal("Robert Johnson", model.SelectedPersonText);
    }

    private static IEnumerable<T> Descendants<T>(Visual visual) where T : Visual
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(visual); ++index)
        {
            if (VisualTreeHelper.GetChild(visual, index) is not Visual child) continue;
            if (child is T match) yield return match;
            foreach (var descendant in Descendants<T>(child)) yield return descendant;
        }
    }
}
