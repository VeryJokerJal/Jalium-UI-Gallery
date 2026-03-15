using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for SwipeControlPage.jalxaml demonstrating SwipeControl functionality.
/// Simulates swipe actions using buttons since SwipeControl requires touch input.
/// </summary>
public partial class SwipeControlPage : Page
{
    private readonly List<string> _items = new()
    {
        "Meeting with Design Team",
        "Review pull request #42",
        "Update project documentation",
        "Deploy staging build",
        "Weekly standup notes"
    };

    public SwipeControlPage()
    {
        InitializeComponent();
        BuildSwipeItems();
    }

    private void BuildSwipeItems()
    {
        if (SwipeContainer == null) return;

        var listPanel = new StackPanel { Orientation = Orientation.Vertical };

        foreach (var item in _items)
        {
            var itemBorder = CreateSwipeItemRow(item);
            listPanel.Children.Add(itemBorder);
        }

        SwipeContainer.Child = listPanel;
    }

    private Border CreateSwipeItemRow(string itemText)
    {
        var rowBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x3D, 0x3D, 0x3D)),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(12, 8, 12, 8),
            MinHeight = 48
        };

        var rowStack = new StackPanel { Orientation = Orientation.Horizontal };

        // Left action: Archive (blue)
        var archiveButton = new Button
        {
            Content = "\u2709 Archive",
            Width = 80,
            Height = 32,
            Margin = new Thickness(0, 0, 8, 0),
            Background = new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4)),
            Foreground = new SolidColorBrush(Color.White),
            FontSize = 11
        };
        archiveButton.Click += (s, e) =>
        {
            UpdateStatus($"Archived: \"{itemText}\"");
            rowBorder.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x3A, 0x6A));
        };
        rowStack.Children.Add(archiveButton);

        // Item content
        var contentText = new TextBlock
        {
            Text = itemText,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.White),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0, 8, 0),
            Width = 250
        };
        rowStack.Children.Add(contentText);

        // Right action: Delete (red)
        var deleteButton = new Button
        {
            Content = "\u2716 Delete",
            Width = 80,
            Height = 32,
            Margin = new Thickness(8, 0, 0, 0),
            Background = new SolidColorBrush(Color.FromRgb(0xC4, 0x2B, 0x1C)),
            Foreground = new SolidColorBrush(Color.White),
            FontSize = 11
        };
        deleteButton.Click += (s, e) =>
        {
            UpdateStatus($"Deleted: \"{itemText}\"");
            rowBorder.Visibility = Visibility.Collapsed;
        };
        rowStack.Children.Add(deleteButton);

        // Flag action (secondary right)
        var flagButton = new Button
        {
            Content = "\u2691 Flag",
            Width = 64,
            Height = 32,
            Margin = new Thickness(4, 0, 0, 0),
            Background = new SolidColorBrush(Color.FromRgb(0xC1, 0x8A, 0x00)),
            Foreground = new SolidColorBrush(Color.White),
            FontSize = 11
        };
        flagButton.Click += (s, e) =>
        {
            UpdateStatus($"Flagged: \"{itemText}\"");
            contentText.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07));
        };
        rowStack.Children.Add(flagButton);

        rowBorder.Child = rowStack;
        return rowBorder;
    }

    private void UpdateStatus(string message)
    {
        if (SwipeStatusText != null)
        {
            SwipeStatusText.Text = message;
        }
        System.Diagnostics.Debug.WriteLine($"[SwipeControlPage] {message}");
    }
}
