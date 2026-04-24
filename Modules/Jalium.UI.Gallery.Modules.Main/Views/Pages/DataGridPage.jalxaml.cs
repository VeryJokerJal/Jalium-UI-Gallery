using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for DataGridPage.jalxaml demonstrating DataGrid functionality.
/// </summary>
public partial class DataGridPage : Page
{
    public DataGridPage()
    {
        InitializeComponent();

        // Set up demo data
        SetupDemoData();
        LoadCodeExamples();
    }

    private void SetupDemoData()
    {
        if (DemoDataGrid == null) return;

        // Create sample data
        var items = new List<PersonData>
        {
            new("Alice", "Smith", 28, "Engineering"),
            new("Bob", "Johnson", 35, "Marketing"),
            new("Charlie", "Brown", 42, "Sales"),
            new("Diana", "Williams", 31, "Engineering"),
            new("Edward", "Jones", 29, "HR"),
            new("Fiona", "Davis", 38, "Finance"),
            new("George", "Miller", 45, "Operations"),
            new("Hannah", "Wilson", 26, "Engineering")
        };

        DemoDataGrid.ItemsSource = items;
        DemoDataGrid.AutoGenerateColumns = true;
        DemoDataGrid.SelectionChanged += OnDataGridSelectionChanged;
    }

    private void OnDataGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedItemText != null && DemoDataGrid?.SelectedItem is PersonData person)
        {
            SelectedItemText.Text = $"{person.FirstName} {person.LastName}";
        }
        else if (SelectedItemText != null)
        {
            SelectedItemText.Text = "(none)";
        }
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }

    private const string XamlExample = @"<DataGrid x:Name=""DemoDataGrid""
          Height=""200""
          AutoGenerateColumns=""True""
          SelectionMode=""Single""
          HeadersVisibility=""Column""
          GridLinesVisibility=""All""
          ItemsSource=""{Binding Items}"" />";

    private const string CSharpExample = @"// Create and configure a DataGrid
var dataGrid = new DataGrid();
dataGrid.AutoGenerateColumns = true;
dataGrid.ItemsSource = myCollection;
dataGrid.SelectionMode = DataGridSelectionMode.Single;

// Handle selection changes
dataGrid.SelectionChanged += (s, e) =>
{
    var selected = dataGrid.SelectedItem;
    Console.WriteLine($""Selected: {selected}"");
};";

    private record PersonData(string FirstName, string LastName, int Age, string Department);
}
