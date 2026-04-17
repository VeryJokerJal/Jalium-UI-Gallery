using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for XamlUICommandPage.jalxaml demonstrating custom XamlUICommand patterns.
/// Shows how to define application-specific commands with custom labels, icons, and accelerators,
/// and wire them to UI elements like toolbar buttons.
/// </summary>
public partial class XamlUICommandPage : Page
{
    /// <summary>
    /// Represents a custom XAML UI command with user-defined properties.
    /// </summary>
    private sealed class XamlUICommand
    {
        public string Label { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
        public string Accelerator { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Action? ExecuteAction { get; set; }
    }

    private readonly List<string> _commandHistory = new();

    public XamlUICommandPage()
    {
        InitializeComponent();
        BuildXamlCommands();
        LoadCodeExamples();
    }

    private void BuildXamlCommands()
    {
        if (XamlCommandContainer == null) return;

        var commands = new XamlUICommand[]
        {
            new()
            {
                Label = "Add to Favorites",
                Icon = "\u2605",
                Accelerator = "Ctrl+D",
                Description = "Add the current item to your favorites list."
            },
            new()
            {
                Label = "Share via Link",
                Icon = "\u2197",
                Accelerator = "Ctrl+Shift+L",
                Description = "Generate a shareable link for the current item."
            },
            new()
            {
                Label = "Export as PDF",
                Icon = "\u2B07",
                Accelerator = "Ctrl+Shift+E",
                Description = "Export the current document as a PDF file."
            },
            new()
            {
                Label = "Toggle Dark Mode",
                Icon = "\u263D",
                Accelerator = "Ctrl+Shift+D",
                Description = "Switch between light and dark color themes."
            },
            new()
            {
                Label = "Run Diagnostics",
                Icon = "\u2699",
                Accelerator = "F5",
                Description = "Run diagnostic checks on the current project."
            },
            new()
            {
                Label = "Sync to Cloud",
                Icon = "\u2601",
                Accelerator = "Ctrl+Shift+S",
                Description = "Synchronize local changes with the cloud storage."
            }
        };

        var mainPanel = new StackPanel { Orientation = Orientation.Vertical };

        // Toolbar-style button row
        var toolbarBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D)),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 4, 8, 4),
            Margin = new Thickness(0, 0, 0, 16)
        };
        var toolbarPanel = new StackPanel { Orientation = Orientation.Horizontal };

        foreach (var cmd in commands)
        {
            var capturedCmd = cmd;
            cmd.ExecuteAction = () => ExecuteCommand(capturedCmd);

            // Toolbar icon button
            var toolbarButton = new Button
            {
                Content = cmd.Icon,
                Width = 36,
                Height = 32,
                Margin = new Thickness(2, 0, 2, 0),
                FontSize = 16,
                ToolTip = $"{cmd.Label} ({cmd.Accelerator})"
            };
            toolbarButton.Click += (s, e) => cmd.ExecuteAction?.Invoke();
            toolbarPanel.Children.Add(toolbarButton);
        }

        toolbarBorder.Child = toolbarPanel;
        mainPanel.Children.Add(toolbarBorder);

        // Detailed command list
        var listTitleText = new TextBlock
        {
            Text = "Command Definitions",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
            Margin = new Thickness(0, 0, 0, 12)
        };
        mainPanel.Children.Add(listTitleText);

        foreach (var cmd in commands)
        {
            var cardBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25)),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var cardStack = new StackPanel { Orientation = Orientation.Horizontal };

            // Icon
            var iconText = new TextBlock
            {
                Text = cmd.Icon,
                FontSize = 20,
                Foreground = new SolidColorBrush(Color.FromRgb(0x60, 0xCD, 0xFF)),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 36,
                Margin = new Thickness(0, 0, 12, 0)
            };
            cardStack.Children.Add(iconText);

            // Label + Description column
            var infoStack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 280
            };

            var labelRow = new StackPanel { Orientation = Orientation.Horizontal };
            var labelText = new TextBlock
            {
                Text = cmd.Label,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.White),
                Margin = new Thickness(0, 0, 8, 0)
            };
            labelRow.Children.Add(labelText);

            // Accelerator badge
            var accelBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x3D, 0x3D, 0x3D)),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5, 1, 5, 1),
                VerticalAlignment = VerticalAlignment.Center
            };
            var accelText = new TextBlock
            {
                Text = cmd.Accelerator,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(0xBB, 0xBB, 0xBB)),
                FontFamily = "Consolas"
            };
            accelBorder.Child = accelText;
            labelRow.Children.Add(accelBorder);

            infoStack.Children.Add(labelRow);

            var descText = new TextBlock
            {
                Text = cmd.Description,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0)
            };
            infoStack.Children.Add(descText);

            cardStack.Children.Add(infoStack);

            // Execute button
            var capturedCmd = cmd;
            var executeButton = new Button
            {
                Content = "Execute",
                Width = 80,
                Height = 28,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 0, 0)
            };
            executeButton.Click += (s, e) => capturedCmd.ExecuteAction?.Invoke();
            cardStack.Children.Add(executeButton);

            cardBorder.Child = cardStack;
            mainPanel.Children.Add(cardBorder);
        }

        XamlCommandContainer.Child = mainPanel;
    }

    private const string XamlExample = @"<!-- Define custom XamlUICommand in resources -->
<Page.Resources>
    <XamlUICommand x:Name=""AddToFavoritesCommand""
                   Label=""Add to Favorites""
                   Description=""Add the current item to favorites.""
                   ExecuteRequested=""OnAddToFavorites"">
        <XamlUICommand.IconSource>
            <SymbolIconSource Symbol=""Favorite""/>
        </XamlUICommand.IconSource>
        <XamlUICommand.KeyboardAccelerators>
            <KeyboardAccelerator Modifiers=""Control"" Key=""D""/>
        </XamlUICommand.KeyboardAccelerators>
    </XamlUICommand>
</Page.Resources>

<!-- Use the command in a toolbar -->
<CommandBar>
    <AppBarButton Command=""{StaticResource AddToFavoritesCommand}""/>
</CommandBar>";

    private const string CSharpExample = @"// Define custom commands with actions
var commands = new[]
{
    new XamlUICommand
    {
        Label = ""Add to Favorites"",
        Icon = ""\u2605"",
        Accelerator = ""Ctrl+D"",
        Description = ""Add item to favorites.""
    },
    new XamlUICommand
    {
        Label = ""Share via Link"",
        Icon = ""\u2197"",
        Accelerator = ""Ctrl+Shift+L"",
        Description = ""Generate a shareable link.""
    },
    new XamlUICommand
    {
        Label = ""Export as PDF"",
        Icon = ""\u2B07"",
        Accelerator = ""Ctrl+Shift+E"",
        Description = ""Export as PDF file.""
    }
};

// Wire each command to a toolbar button
foreach (var cmd in commands)
{
    cmd.ExecuteAction = () =>
        Debug.WriteLine($""{cmd.Label} executed."");

    var button = new Button
    {
        Content = cmd.Icon,
        ToolTip = $""{cmd.Label} ({cmd.Accelerator})""
    };
    button.Click += (s, e) => cmd.ExecuteAction();
}";

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

    private void ExecuteCommand(XamlUICommand command)
    {
        var message = $"{command.Icon} {command.Label} executed.";

        _commandHistory.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {command.Label}");
        if (_commandHistory.Count > 5)
            _commandHistory.RemoveAt(_commandHistory.Count - 1);

        if (CommandOutputText != null)
        {
            CommandOutputText.Text = message;
        }

        if (CommandHistoryText != null)
        {
            CommandHistoryText.Text = "Recent: " + string.Join(" | ", _commandHistory);
        }

        System.Diagnostics.Debug.WriteLine($"[XamlUICommandPage] {message}");
    }
}
