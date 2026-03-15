using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for StandardUICommandPage.jalxaml demonstrating StandardUICommand patterns.
/// Shows predefined commands with their standard label, icon, keyboard shortcut, and description.
/// </summary>
public partial class StandardUICommandPage : Page
{
    /// <summary>
    /// Represents a standard UI command with predefined label, icon, shortcut, and description.
    /// </summary>
    private sealed class StandardUICommand
    {
        public string Label { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
        public string KeyboardShortcut { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    private static readonly StandardUICommand[] StandardCommands =
    {
        new()
        {
            Label = "Cut",
            Icon = "\u2702",
            KeyboardShortcut = "Ctrl+X",
            Description = "Remove the selected content and put it on the clipboard."
        },
        new()
        {
            Label = "Copy",
            Icon = "\u2398",
            KeyboardShortcut = "Ctrl+C",
            Description = "Copy the selected content to the clipboard."
        },
        new()
        {
            Label = "Paste",
            Icon = "\u2399",
            KeyboardShortcut = "Ctrl+V",
            Description = "Insert the contents of the clipboard at the current location."
        },
        new()
        {
            Label = "Delete",
            Icon = "\u2716",
            KeyboardShortcut = "Del",
            Description = "Remove the selected content permanently."
        },
        new()
        {
            Label = "Undo",
            Icon = "\u21B6",
            KeyboardShortcut = "Ctrl+Z",
            Description = "Reverse the most recent action."
        },
        new()
        {
            Label = "Redo",
            Icon = "\u21B7",
            KeyboardShortcut = "Ctrl+Y",
            Description = "Repeat the most recently undone action."
        },
        new()
        {
            Label = "Select All",
            Icon = "\u2610",
            KeyboardShortcut = "Ctrl+A",
            Description = "Select all content in the current context."
        },
        new()
        {
            Label = "Save",
            Icon = "\u2605",
            KeyboardShortcut = "Ctrl+S",
            Description = "Save the current document or file."
        }
    };

    public StandardUICommandPage()
    {
        InitializeComponent();
        BuildCommandDisplay();
    }

    private void BuildCommandDisplay()
    {
        if (CommandContainer == null) return;

        var mainPanel = new StackPanel { Orientation = Orientation.Vertical };

        // Header row
        var headerRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 12)
        };

        var headerIcon = new TextBlock
        {
            Text = "Icon",
            Width = 50,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88))
        };
        headerRow.Children.Add(headerIcon);

        var headerLabel = new TextBlock
        {
            Text = "Label",
            Width = 90,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88))
        };
        headerRow.Children.Add(headerLabel);

        var headerShortcut = new TextBlock
        {
            Text = "Shortcut",
            Width = 80,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88))
        };
        headerRow.Children.Add(headerShortcut);

        var headerDesc = new TextBlock
        {
            Text = "Description",
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88))
        };
        headerRow.Children.Add(headerDesc);

        mainPanel.Children.Add(headerRow);

        // Separator
        var separatorBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x3D, 0x3D, 0x3D)),
            Height = 1,
            Margin = new Thickness(0, 0, 0, 8)
        };
        mainPanel.Children.Add(separatorBorder);

        // Command rows
        foreach (var cmd in StandardCommands)
        {
            var rowPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 4)
            };

            // Icon as a clickable button
            var iconButton = new Button
            {
                Content = cmd.Icon,
                Width = 40,
                Height = 32,
                Margin = new Thickness(0, 0, 10, 0),
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var capturedLabel = cmd.Label;
            iconButton.Click += (s, e) => UpdateStatus($"Executed: {capturedLabel}");
            rowPanel.Children.Add(iconButton);

            // Label
            var labelText = new TextBlock
            {
                Text = cmd.Label,
                Width = 90,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.White),
                VerticalAlignment = VerticalAlignment.Center
            };
            rowPanel.Children.Add(labelText);

            // Keyboard shortcut
            var shortcutBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x3D, 0x3D, 0x3D)),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(6, 2, 6, 2),
                Margin = new Thickness(0, 0, 12, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            var shortcutText = new TextBlock
            {
                Text = cmd.KeyboardShortcut,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
                FontFamily = "Consolas"
            };
            shortcutBorder.Child = shortcutText;
            rowPanel.Children.Add(shortcutBorder);

            // Description
            var descText = new TextBlock
            {
                Text = cmd.Description,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Width = 320
            };
            rowPanel.Children.Add(descText);

            mainPanel.Children.Add(rowPanel);
        }

        CommandContainer.Child = mainPanel;
    }

    private void UpdateStatus(string message)
    {
        if (CommandStatusText != null)
        {
            CommandStatusText.Text = message;
        }
        System.Diagnostics.Debug.WriteLine($"[StandardUICommandPage] {message}");
    }
}
