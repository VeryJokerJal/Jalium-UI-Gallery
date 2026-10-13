using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Selection and execution state for the SplitButton demo.</summary>
public sealed class SplitButtonViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <SplitButton Content="Run"
                     Command="{Binding RunCommand}">
            <SplitButton.Flyout>
                <MenuFlyout>
                    <MenuFlyoutItem Text="Debug"
                                    Command="{Binding SelectTargetCommand}"
                                    CommandParameter="Debug"/>
                </MenuFlyout>
            </SplitButton.Flyout>
        </SplitButton>
        """;

    private const string CSharpCode = """
        // SplitButtonViewModel.cs
        public sealed class SplitButtonViewModel : ViewModelBase
        {
            public ICommand RunCommand { get; }
            public ICommand SelectTargetCommand { get; }

            // Primary and flyout actions both execute bindable commands.
        }
        """;

    private string _selectedTarget = "Jalium.UI.Controls";
    private int _runCount;
    private string _buildStatusText = "Ready.";
    private bool _isBuildFlyoutOpen;

    public SplitButtonViewModel()
    {
        RunCommand = new RelayCommand(RegisterRun);
        SelectTargetCommand = new RelayCommand<string>(SelectTarget);
        BuildCommand = new RelayCommand(() => BuildStatusText = "Build command executed.");
        SelectBuildCommand = new RelayCommand<string>(
            value => BuildStatusText = $"Menu selected: {value}");
        OpenBuildFlyoutCommand = new RelayCommand(() =>
            IsBuildFlyoutOpen = true);
        CloseBuildFlyoutCommand = new RelayCommand(() =>
            IsBuildFlyoutOpen = false);
    }

    public string SelectedTarget
    {
        get => _selectedTarget;
        private set
        {
            if (SetProperty(ref _selectedTarget, value))
            {
                OnPropertyChanged(nameof(RunStatusText));
            }
        }
    }

    public int RunCount
    {
        get => _runCount;
        private set
        {
            if (SetProperty(ref _runCount, value))
            {
                OnPropertyChanged(nameof(RunStatusText));
            }
        }
    }

    public string RunStatusText =>
        $"Selected target: {SelectedTarget} | Runs: {RunCount}";

    public string BuildStatusText
    {
        get => _buildStatusText;
        private set => SetProperty(ref _buildStatusText, value);
    }

    public ICommand RunCommand { get; }

    public ICommand SelectTargetCommand { get; }

    public ICommand BuildCommand { get; }

    public ICommand SelectBuildCommand { get; }

    public ICommand OpenBuildFlyoutCommand { get; }

    public ICommand CloseBuildFlyoutCommand { get; }

    public bool IsBuildFlyoutOpen
    {
        get => _isBuildFlyoutOpen;
        set => SetProperty(ref _isBuildFlyoutOpen, value);
    }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    private void RegisterRun() => RunCount++;

    private void SelectTarget(string? target)
    {
        if (!string.IsNullOrWhiteSpace(target))
        {
            SelectedTarget = target;
        }
    }
}
