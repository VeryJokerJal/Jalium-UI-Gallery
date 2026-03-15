using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Views;

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
}
