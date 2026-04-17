using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for CommandBarPage.jalxaml demonstrating CommandBar functionality.
/// </summary>
public partial class CommandBarPage : Page
{
    public CommandBarPage()
    {
        InitializeComponent();
        BuildBasicCommandBar();
        BuildSecondaryCommandBar();
        LoadCodeExamples();
    }

    private void BuildBasicCommandBar()
    {
        if (BasicCommandBarContainer == null) return;

        var commandBar = new CommandBar
        {
            Width = 500,
            Height = 48,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
            OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Collapsed
        };

        // Add primary commands
        var addButton = CreateAppBarButton("\u2795", "Add");
        addButton.Click += (_, _) => UpdateStatus(BasicCommandBarStatus, "Add command executed.");

        var editButton = CreateAppBarButton("\u270F", "Edit");
        editButton.Click += (_, _) => UpdateStatus(BasicCommandBarStatus, "Edit command executed.");

        var deleteButton = CreateAppBarButton("\u2716", "Delete");
        deleteButton.Click += (_, _) => UpdateStatus(BasicCommandBarStatus, "Delete command executed.");

        var refreshButton = CreateAppBarButton("\u21BB", "Refresh");
        refreshButton.Click += (_, _) => UpdateStatus(BasicCommandBarStatus, "Refresh command executed.");

        commandBar.PrimaryCommands.Add(addButton);
        commandBar.PrimaryCommands.Add(editButton);
        commandBar.PrimaryCommands.Add(deleteButton);
        commandBar.PrimaryCommands.Add(refreshButton);

        BasicCommandBarContainer.Children.Add(commandBar);
    }

    private void BuildSecondaryCommandBar()
    {
        if (SecondaryCommandBarContainer == null) return;

        var commandBar = new CommandBar
        {
            Width = 500,
            Height = 48,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
            OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Visible
        };

        // Primary commands
        var shareButton = CreateAppBarButton("\u2197", "Share");
        shareButton.Click += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Share command executed.");

        var favoriteButton = CreateAppBarButton("\u2605", "Favorite");
        favoriteButton.Click += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Favorite command executed.");

        var printButton = CreateAppBarButton("\u2399", "Print");
        printButton.Click += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Print command executed.");

        commandBar.PrimaryCommands.Add(shareButton);
        commandBar.PrimaryCommands.Add(favoriteButton);
        commandBar.PrimaryCommands.Add(printButton);

        // Secondary (overflow) commands
        var settingsButton = CreateAppBarButton("\u2699", "Settings");
        settingsButton.Click += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Settings command executed (from overflow).");

        var helpButton = CreateAppBarButton("\u2753", "Help");
        helpButton.Click += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Help command executed (from overflow).");

        var aboutButton = CreateAppBarButton("\u2139", "About");
        aboutButton.Click += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "About command executed (from overflow).");

        commandBar.SecondaryCommands.Add(settingsButton);
        commandBar.SecondaryCommands.Add(helpButton);
        commandBar.SecondaryCommands.Add(aboutButton);

        // Listen to open/close events
        commandBar.Opened += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Overflow menu opened.");
        commandBar.Closed += (_, _) => UpdateStatus(SecondaryCommandBarStatus, "Overflow menu closed.");

        SecondaryCommandBarContainer.Children.Add(commandBar);
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

    private const string XamlExample = @"<!-- CommandBar with primary commands -->
<CommandBar Width=""500"" Height=""48""
            OverflowButtonVisibility=""Visible"">
    <CommandBar.PrimaryCommands>
        <AppBarButton Label=""Add"" Icon=""Add"" />
        <AppBarButton Label=""Edit"" Icon=""Edit"" />
        <AppBarButton Label=""Delete"" Icon=""Delete"" />
    </CommandBar.PrimaryCommands>
    <CommandBar.SecondaryCommands>
        <AppBarButton Label=""Settings"" />
        <AppBarButton Label=""Help"" />
    </CommandBar.SecondaryCommands>
</CommandBar>";

    private const string CSharpExample = @"// Build a CommandBar programmatically
var commandBar = new CommandBar();
commandBar.OverflowButtonVisibility =
    CommandBarOverflowButtonVisibility.Visible;

// Add primary commands
var addBtn = new AppBarButton { Label = ""Add"" };
addBtn.Icon = new SymbolIcon(Symbol.Add);
addBtn.Click += (s, e) => { /* handle click */ };
commandBar.PrimaryCommands.Add(addBtn);

// Add secondary (overflow) commands
var settingsBtn = new AppBarButton { Label = ""Settings"" };
commandBar.SecondaryCommands.Add(settingsBtn);

// Listen to open/close events
commandBar.Opened += (s, e) => { /* overflow opened */ };
commandBar.Closed += (s, e) => { /* overflow closed */ };";

    private static AppBarButton CreateAppBarButton(string iconText, string label)
    {
        var button = new AppBarButton
        {
            Label = label,
            IsCompact = true,
            Width = 48,
            Height = 48
        };

        button.Content = new TextBlock
        {
            Text = iconText,
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        button.ToolTip = label;
        return button;
    }

    private static void UpdateStatus(TextBlock? statusText, string message)
    {
        if (statusText != null)
            statusText.Text = message;
    }
}
