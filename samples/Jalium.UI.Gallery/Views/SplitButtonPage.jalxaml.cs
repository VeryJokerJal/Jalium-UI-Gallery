using Jalium.UI.Controls;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for SplitButtonPage.jalxaml demonstrating SplitButton usage and API.
/// </summary>
public partial class SplitButtonPage : Page
{
    private string _selectedTarget = "Jalium.UI.Controls";
    private int _runCount;

    public SplitButtonPage()
    {
        InitializeComponent();
        InitializeBuildCommandSample();
        UpdateRunStatus();
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
