using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for ListViewPage.jalxaml demonstrating list view functionality.
/// </summary>
public partial class ListViewPage : Page
{
    private int _itemCounter = 4;

    public ListViewPage()
    {
        InitializeComponent();

        // Set up GridView data
        SetupGridViewData();

        // Set up event handlers
        if (AddItemButton != null)
        {
            AddItemButton.Click += OnAddItemClick;
        }

        if (RemoveItemButton != null)
        {
            RemoveItemButton.Click += OnRemoveItemClick;
        }

        if (InteractiveListView != null)
        {
            InteractiveListView.SelectionChanged += OnSelectionChanged;
        }

        LoadCodeExamples();
    }

    private void SetupGridViewData()
    {
        if (GridListView == null) return;

        var files = new List<FileInfo>
        {
            new("Document.docx", "Word Document", "245 KB", "2026-01-15"),
            new("Budget.xlsx", "Excel Spreadsheet", "128 KB", "2026-02-01"),
            new("Photo.png", "PNG Image", "1.2 MB", "2026-01-20"),
            new("Readme.md", "Markdown File", "4 KB", "2026-02-10"),
            new("App.exe", "Application", "3.8 MB", "2025-12-05"),
            new("Config.json", "JSON File", "2 KB", "2026-02-14"),
            new("Notes.txt", "Text File", "12 KB", "2026-01-28"),
            new("Backup.zip", "ZIP Archive", "56 MB", "2025-11-30")
        };

        GridListView.ItemsSource = files;
    }

    private void OnAddItemClick(object? sender, EventArgs e)
    {
        if (InteractiveListView != null && NewItemTextBox != null)
        {
            var text = string.IsNullOrWhiteSpace(NewItemTextBox.Text)
                ? $"Item {_itemCounter++}"
                : NewItemTextBox.Text;

            InteractiveListView.Items.Add(new ListViewItem { Content = text });
            NewItemTextBox.Text = string.Empty;
        }
    }

    private void OnRemoveItemClick(object? sender, EventArgs e)
    {
        if (InteractiveListView != null && InteractiveListView.SelectedItem != null)
        {
            InteractiveListView.Items.Remove(InteractiveListView.SelectedItem);
        }
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectionText != null && InteractiveListView != null)
        {
            if (InteractiveListView.SelectedItem is ListViewItem item)
            {
                SelectionText.Text = $"Selected: {item.Content}";
            }
            else
            {
                SelectionText.Text = "No item selected";
            }
        }
    }

    private record FileInfo(string Name, string Type, string Size, string Modified);

    private const string XamlExample =
@"<!-- Basic ListView -->
<ListView x:Name=""BasicListView"" Width=""300"" Height=""200"">
    <ListViewItem Content=""Item 1""/>
    <ListViewItem Content=""Item 2""/>
    <ListViewItem Content=""Item 3""/>
</ListView>

<!-- ListView with GridView columns -->
<ListView x:Name=""GridListView"" Width=""500"" Height=""200"">
    <ListView.View>
        <GridView>
            <GridViewColumn Header=""Name"" Width=""150""/>
            <GridViewColumn Header=""Type"" Width=""100""/>
            <GridViewColumn Header=""Size"" Width=""80""/>
            <GridViewColumn Header=""Modified"" Width=""120""/>
        </GridView>
    </ListView.View>
</ListView>

<!-- Selection modes -->
<ListView SelectionMode=""Single"" />
<ListView SelectionMode=""Multiple"" />
<ListView SelectionMode=""Extended"" />";

    private const string CSharpExample =
@"// Populate ListView with data
var files = new List<FileInfo>
{
    new(""Document.docx"", ""Word"", ""245 KB"", ""2026-01-15""),
    new(""Budget.xlsx"", ""Excel"", ""128 KB"", ""2026-02-01""),
    new(""Photo.png"", ""Image"", ""1.2 MB"", ""2026-01-20"")
};
GridListView.ItemsSource = files;

// Add items dynamically
var item = new ListViewItem { Content = ""New Item"" };
InteractiveListView.Items.Add(item);

// Remove selected item
if (listView.SelectedItem != null)
    listView.Items.Remove(listView.SelectedItem);

// Handle selection changes
listView.SelectionChanged += (s, e) =>
{
    if (listView.SelectedItem is ListViewItem selected)
    {
        statusText.Text = $""Selected: {selected.Content}"";
    }
};

// Record for GridView binding
private record FileInfo(
    string Name, string Type,
    string Size, string Modified);";

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
}
