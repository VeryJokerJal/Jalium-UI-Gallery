using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class ShellIntegrationViewModel
{
    private readonly IGalleryShellIntegrationService _shellService;
    private double _taskbarProgress = 50;
    private TaskbarProgressMode _taskbarState;

    public ShellIntegrationViewModel(
        IGalleryShellIntegrationService shellService)
    {
        _shellService = shellService;
        var snapshot = shellService.ReadSystemParameters();

        GlassEnabledText =
            $"DWM Glass Enabled: {snapshot.IsGlassEnabled}";
        GlassColorText = $"Glass Color: {snapshot.GlassColor}";
        CaptionHeightText =
            $"Caption Height: {snapshot.CaptionHeight}";
        ResizeBorderText =
            $"Resize Border: {snapshot.ResizeBorder}";

        ApplyJumpListCommand =
            new RelayCommand(ApplyJumpList);
        SetProgressNoneCommand =
            StateCommand(TaskbarProgressMode.None);
        SetProgressNormalCommand =
            StateCommand(TaskbarProgressMode.Normal);
        SetProgressPausedCommand =
            StateCommand(TaskbarProgressMode.Paused);
        SetProgressErrorCommand =
            StateCommand(TaskbarProgressMode.Error);
    }

    public double TaskbarProgress
    {
        get => _taskbarProgress;
        set
        {
            if (!SetProperty(ref _taskbarProgress, value))
            {
                return;
            }

            OnPropertyChanged(nameof(TaskbarProgressText));
            Status = _shellService.TrySetTaskbarProgress(value / 100d)
                ? $"Taskbar progress set to {value:F0}%."
                : "No application window is available.";
        }
    }

    public TaskbarProgressMode TaskbarState
    {
        get => _taskbarState;
        private set
        {
            if (SetProperty(ref _taskbarState, value))
            {
                OnPropertyChanged(nameof(TaskbarStatus));
            }
        }
    }

    public string TaskbarProgressText =>
        $"{TaskbarProgress:F0}%";

    public string TaskbarStatus =>
        $"Taskbar State: {TaskbarState}";

    public string GlassEnabledText { get; }

    public string GlassColorText { get; }

    public string CaptionHeightText { get; }

    public string ResizeBorderText { get; }

    public ICommand ApplyJumpListCommand { get; }

    public ICommand SetProgressNoneCommand { get; }

    public ICommand SetProgressNormalCommand { get; }

    public ICommand SetProgressPausedCommand { get; }

    public ICommand SetProgressErrorCommand { get; }

    private ICommand StateCommand(TaskbarProgressMode mode) =>
        new RelayCommand(() => SetTaskbarState(mode));

    private void ApplyJumpList()
    {
        Status = _shellService.TryApplySampleJumpList()
            ? "Sample JumpList applied."
            : "The application shell is unavailable.";
    }

    private void SetTaskbarState(TaskbarProgressMode mode)
    {
        if (_shellService.TrySetTaskbarState(mode))
        {
            TaskbarState = mode;
            Status = $"Taskbar state changed to {mode}.";
            return;
        }

        Status = "No application window is available.";
    }
}
