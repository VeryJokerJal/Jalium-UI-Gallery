using Jalium.UI.Controls;

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
}
