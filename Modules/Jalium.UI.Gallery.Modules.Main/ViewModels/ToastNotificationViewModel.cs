using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class ToastNotificationViewModel
{
    private long _sequence;
    private ToastPosition _position = ToastPosition.TopRight;
    private double _durationSeconds = 5;
    private bool _showCloseButton = true;
    private bool _autoDismiss = true;
    private ToastRequest? _request;

    public ToastNotificationViewModel()
    {
        Positions = Enum.GetValues<ToastPosition>();
        ShowToastCommand = new RelayCommand<string>(Show);
        ShowActionToastCommand = new RelayCommand(() =>
            Publish(
                ToastSeverity.Information,
                "Download Complete",
                "Your file has been downloaded.",
                hasAction: true));
        ShowCustomToastCommand = new RelayCommand(() =>
            Publish(
                ToastSeverity.Information,
                "Custom Toast",
                $"Duration: {DurationSeconds:F0}s — Position: {Position}"));
        DismissAllCommand = new RelayCommand(() =>
            Request = new ToastRequest(
                ++_sequence,
                ToastSeverity.Information,
                string.Empty,
                string.Empty,
                TimeSpan.Zero,
                true,
                true,
                DismissAll: true));
    }

    public IReadOnlyList<ToastPosition> Positions { get; }

    public ICommand ShowToastCommand { get; }

    public ICommand ShowActionToastCommand { get; }

    public ICommand ShowCustomToastCommand { get; }

    public ICommand DismissAllCommand { get; }

    public ToastPosition Position
    {
        get => _position;
        set => SetProperty(ref _position, value);
    }

    public double DurationSeconds
    {
        get => _durationSeconds;
        set => SetProperty(ref _durationSeconds, Math.Round(value));
    }

    public bool ShowCloseButton
    {
        get => _showCloseButton;
        set => SetProperty(ref _showCloseButton, value);
    }

    public bool AutoDismiss
    {
        get => _autoDismiss;
        set => SetProperty(ref _autoDismiss, value);
    }

    public ToastRequest? Request
    {
        get => _request;
        private set => SetProperty(ref _request, value);
    }

    private void Show(string? severity)
    {
        var parsed = severity switch
        {
            "Success" => ToastSeverity.Success,
            "Warning" => ToastSeverity.Warning,
            "Error" => ToastSeverity.Error,
            _ => ToastSeverity.Information
        };
        var message = parsed switch
        {
            ToastSeverity.Success =>
                "Operation completed successfully.",
            ToastSeverity.Warning =>
                "Please review before proceeding.",
            ToastSeverity.Error =>
                "An error occurred during the operation.",
            _ => "This is an informational message."
        };
        Publish(parsed, severity ?? "Information", message);
    }

    private void Publish(
        ToastSeverity severity,
        string title,
        string message,
        bool hasAction = false) =>
        Request = new ToastRequest(
            ++_sequence,
            severity,
            title,
            message,
            TimeSpan.FromSeconds(DurationSeconds),
            ShowCloseButton,
            AutoDismiss,
            hasAction);
}
