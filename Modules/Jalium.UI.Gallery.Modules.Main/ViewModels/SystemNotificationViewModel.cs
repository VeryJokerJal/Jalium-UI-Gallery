using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Notifications;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class SystemNotificationViewModel
{
    private readonly IGallerySystemNotificationService _notifications;
    private string _basicTitle = "Hello from Jalium.UI";
    private string _basicBody =
        "This is a system notification demo.";
    private string _tag = "demo-tag";
    private string _group = "gallery";
    private double _expirationSeconds = 10;
    private bool _isSilent;
    private string _actionResult =
        "Click a notification action to see the result here.";

    public SystemNotificationViewModel(
        IGallerySystemNotificationService notifications)
    {
        _notifications = notifications;
        SendBasicCommand = new RelayCommand(
            () => _notifications.Show(BasicTitle, BasicBody),
            () => IsSupported);
        SendPriorityCommand =
            new RelayCommand<string>(SendPriority, _ => IsSupported);
        SendWithActionsCommand = new RelayCommand(
            SendWithActions,
            () => IsSupported);
        SendAdvancedCommand = new RelayCommand(
            () => _notifications.ShowAdvanced(
                Tag,
                Group,
                IsSilent,
                ExpirationSeconds),
            () => IsSupported);
        RemoveTagCommand = new RelayCommand(
            () => _notifications.Remove(Tag, Group),
            () => IsSupported);
        ClearAllCommand = new RelayCommand(
            _notifications.ClearAll,
            () => IsSupported);
    }

    public ICommand SendBasicCommand { get; }

    public ICommand SendPriorityCommand { get; }

    public ICommand SendWithActionsCommand { get; }

    public ICommand SendAdvancedCommand { get; }

    public ICommand RemoveTagCommand { get; }

    public ICommand ClearAllCommand { get; }

    public bool IsSupported => _notifications.IsSupported;

    public string SupportMessage => _notifications.SupportMessage;

    public string BasicTitle
    {
        get => _basicTitle;
        set => SetProperty(ref _basicTitle, value);
    }

    public string BasicBody
    {
        get => _basicBody;
        set => SetProperty(ref _basicBody, value);
    }

    public string Tag
    {
        get => _tag;
        set => SetProperty(ref _tag, value);
    }

    public string Group
    {
        get => _group;
        set => SetProperty(ref _group, value);
    }

    public double ExpirationSeconds
    {
        get => _expirationSeconds;
        set => SetProperty(
            ref _expirationSeconds,
            Math.Round(value));
    }

    public bool IsSilent
    {
        get => _isSilent;
        set => SetProperty(ref _isSilent, value);
    }

    public string ActionResult
    {
        get => _actionResult;
        private set => SetProperty(ref _actionResult, value);
    }

    private void SendPriority(string? value)
    {
        var priority = value switch
        {
            "Low" => NotificationPriority.Low,
            "High" => NotificationPriority.High,
            _ => NotificationPriority.Normal
        };
        _notifications.ShowPriority(priority);
    }

    private void SendWithActions()
    {
        var progress = new Progress<string>(
            value => ActionResult = value);
        _notifications.ShowWithActions(progress);
    }
}
