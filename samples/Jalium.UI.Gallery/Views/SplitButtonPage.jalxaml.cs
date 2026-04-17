using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for SplitButtonPage.jalxaml demonstrating SplitButton usage and API.
/// </summary>
public partial class SplitButtonPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic SplitButton with MenuFlyout -->
    <SplitButton x:Name=""PasteSplitButton""
                 Content=""Paste""
                 Click=""OnPasteClick""
                 Width=""160""
                 Margin=""0,0,0,16"">
        <SplitButton.Flyout>
            <MenuFlyout>
                <MenuFlyoutItem Text=""Paste"" Click=""OnPasteClick""/>
                <MenuFlyoutItem Text=""Paste Special..."" Click=""OnPasteSpecialClick""/>
                <MenuFlyoutItem Text=""Paste as Plain Text"" Click=""OnPastePlainClick""/>
            </MenuFlyout>
        </SplitButton.Flyout>
    </SplitButton>

    <!-- SplitButton for color selection -->
    <SplitButton x:Name=""ColorSplitButton""
                 Content=""Highlight""
                 Width=""160""
                 Margin=""0,0,0,16"">
        <SplitButton.Flyout>
            <MenuFlyout>
                <MenuFlyoutItem Text=""Yellow"" Click=""OnColorSelected""/>
                <MenuFlyoutItem Text=""Green"" Click=""OnColorSelected""/>
                <MenuFlyoutItem Text=""Blue"" Click=""OnColorSelected""/>
                <MenuFlyoutItem Text=""Red"" Click=""OnColorSelected""/>
                <MenuFlyoutSeparator/>
                <MenuFlyoutItem Text=""No Highlight"" Click=""OnColorSelected""/>
            </MenuFlyout>
        </SplitButton.Flyout>
    </SplitButton>

    <!-- SplitButton with Command binding -->
    <SplitButton Content=""Save""
                 Command=""{Binding SaveCommand}""
                 Width=""160"">
        <SplitButton.Flyout>
            <MenuFlyout>
                <MenuFlyoutItem Text=""Save"" Command=""{Binding SaveCommand}""/>
                <MenuFlyoutItem Text=""Save As..."" Command=""{Binding SaveAsCommand}""/>
                <MenuFlyoutItem Text=""Save All"" Command=""{Binding SaveAllCommand}""/>
            </MenuFlyout>
        </SplitButton.Flyout>
    </SplitButton>
</StackPanel>";

    private const string CSharpExample = @"using System.Windows.Input;
using Jalium.UI.Controls;

public partial class SplitButtonSample : Page
{
    private string _currentPasteMode = ""Paste"";

    public SplitButtonSample()
    {
        InitializeComponent();
        SetupSplitButtons();
    }

    private void SetupSplitButtons()
    {
        // Handle primary click
        PasteSplitButton.Click += OnPasteClick;

        // Listen for flyout events
        if (PasteSplitButton.Flyout != null)
        {
            PasteSplitButton.Flyout.Opened += (s, e) =>
                StatusText.Text = ""Flyout opened"";
            PasteSplitButton.Flyout.Closed += (s, e) =>
                StatusText.Text = ""Flyout closed"";
        }

        // Programmatically show/hide flyout
        OpenFlyoutBtn.Click += (s, e) =>
            PasteSplitButton.Flyout?.ShowAt(PasteSplitButton);
        CloseFlyoutBtn.Click += (s, e) =>
            PasteSplitButton.Flyout?.Hide();
    }

    private void OnPasteClick(SplitButton sender,
        SplitButtonClickEventArgs args)
    {
        // Execute current paste mode
        ExecutePaste(_currentPasteMode);
    }

    private void OnPasteSpecialClick(object? sender, RoutedEventArgs e)
    {
        _currentPasteMode = ""Paste Special"";
        PasteSplitButton.Content = ""Paste Special"";
        ExecutePaste(_currentPasteMode);
    }

    private void ExecutePaste(string mode)
    {
        StatusText.Text = $""Executed: {mode}"";
    }
}";

    private string _selectedTarget = "Jalium.UI.Controls";
    private int _runCount;

    public SplitButtonPage()
    {
        InitializeComponent();
        InitializeBuildCommandSample();
        UpdateRunStatus();
        LoadCodeExamples();
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

    private void InitializeBuildCommandSample()
    {
        if (BuildSplitButton == null)
            return;

        BuildSplitButton.Command = new DelegateCommand(OnBuildCommandExecuted);
        BuildSplitButton.CommandParameter = "Build command executed";
        BuildSplitButton.Click += OnBuildSplitButtonClick;

        if (BuildSplitButton.Flyout != null)
        {
            BuildSplitButton.Flyout.Opened += OnBuildFlyoutOpened;
            BuildSplitButton.Flyout.Closed += OnBuildFlyoutClosed;
        }
    }

    private void OnRunSplitButtonClick(SplitButton sender, SplitButtonClickEventArgs args)
    {
        _runCount++;
        UpdateRunStatus();
    }

    private void OnRunTargetClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && !string.IsNullOrWhiteSpace(item.Text))
        {
            _selectedTarget = item.Text;
            UpdateRunStatus();
        }
    }

    private void OnBuildSplitButtonClick(SplitButton sender, SplitButtonClickEventArgs args)
    {
        UpdateBuildStatus("Primary action clicked. Command will run next.");
    }

    private void OnBuildMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item)
        {
            UpdateBuildStatus($"Menu selected: {item.Text}");
        }
    }

    private void OnOpenBuildFlyoutClick(object? sender, RoutedEventArgs e)
    {
        if (BuildSplitButton?.Flyout != null)
        {
            BuildSplitButton.Flyout.ShowAt(BuildSplitButton);
        }
    }

    private void OnCloseBuildFlyoutClick(object? sender, RoutedEventArgs e)
    {
        if (BuildSplitButton?.Flyout != null)
        {
            BuildSplitButton.Flyout.Hide();
        }
    }

    private void OnBuildFlyoutOpened(object? sender, EventArgs e)
    {
        UpdateBuildStatus("Flyout opened.");
    }

    private void OnBuildFlyoutClosed(object? sender, EventArgs e)
    {
        UpdateBuildStatus("Flyout closed.");
    }

    private void OnBuildCommandExecuted(object? parameter)
    {
        UpdateBuildStatus(parameter?.ToString() ?? "Build command executed");
    }

    private void UpdateRunStatus()
    {
        if (RunStatusText != null)
        {
            RunStatusText.Text = $"Selected target: {_selectedTarget} | Runs: {_runCount}";
        }
    }

    private void UpdateBuildStatus(string message)
    {
        if (BuildStatusText != null)
        {
            BuildStatusText.Text = message;
        }
    }

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public DelegateCommand(Action<object?> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
