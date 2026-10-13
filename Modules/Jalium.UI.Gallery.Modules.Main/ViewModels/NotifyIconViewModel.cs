using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>State and commands for the system tray demo.</summary>
public sealed class NotifyIconViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <!-- NotifyIcon is owned by IGalleryNotifyIconService; the page binds its controls. -->
        <TextBox Text="{Binding TooltipText, Mode=TwoWay}"/>
        <Button Content="Show Icon" Command="{Binding ShowIconCommand}"/>
        <Button Content="Hide Icon" Command="{Binding HideIconCommand}"/>
        <Button Content="Show Balloon Tip"
                Command="{Binding ShowBalloonCommand}"/>
        """;

    private const string CSharpCode = """
        // NotifyIconViewModel.cs
        public sealed class NotifyIconViewModel : ViewModelBase
        {
            private readonly IGalleryNotifyIconService _notifyIconService;

            public NotifyIconViewModel(
                IGalleryNotifyIconService notifyIconService)
            {
                _notifyIconService = notifyIconService;
                ShowIconCommand = new RelayCommand(() =>
                    _notifyIconService.Show("Jalium.UI Gallery"));
                ShowBalloonCommand = new RelayCommand(() =>
                    _notifyIconService.ShowBalloon(
                        5,
                        "Build complete",
                        "The operation finished successfully.",
                        BalloonTipIcon.Info));
            }

            public ICommand ShowIconCommand { get; }
            public ICommand ShowBalloonCommand { get; }
        }
        """;

    private readonly IGalleryNotifyIconService _notifyIconService;
    private readonly List<string> _eventLog = new();
    private string _tooltipText = "Jalium.UI Gallery";
    private string _balloonTitle = "Balloon Tip";
    private string _balloonText = "This is a balloon tip from the tray.";
    private BalloonTipIcon _selectedBalloonIcon = BalloonTipIcon.Info;
    private double _balloonTimeout = 5;
    private bool _isContextMenuEnabled = true;

    public NotifyIconViewModel()
        : this(NullGalleryNotifyIconService.Instance)
    {
    }

    public NotifyIconViewModel(IGalleryNotifyIconService notifyIconService)
    {
        _notifyIconService = notifyIconService;
        _notifyIconService.Activity += OnNotifyIconActivity;
        BalloonIcons = Enum.GetValues<BalloonTipIcon>();
        ShowIconCommand = new RelayCommand(ShowIcon);
        HideIconCommand = new RelayCommand(_notifyIconService.Hide);
        ShowBalloonCommand = new RelayCommand(ShowBalloon);
        ClearLogCommand = new RelayCommand(ClearEventLog);
    }

    public IReadOnlyList<BalloonTipIcon> BalloonIcons { get; }

    public string TooltipText
    {
        get => _tooltipText;
        set => SetProperty(ref _tooltipText, value);
    }

    public string BalloonTitle
    {
        get => _balloonTitle;
        set => SetProperty(ref _balloonTitle, value);
    }

    public string BalloonText
    {
        get => _balloonText;
        set => SetProperty(ref _balloonText, value);
    }

    public BalloonTipIcon SelectedBalloonIcon
    {
        get => _selectedBalloonIcon;
        set => SetProperty(ref _selectedBalloonIcon, value);
    }

    public double BalloonTimeout
    {
        get => _balloonTimeout;
        set
        {
            if (SetProperty(ref _balloonTimeout, value))
            {
                OnPropertyChanged(nameof(BalloonTimeoutText));
            }
        }
    }

    public string BalloonTimeoutText => $"{BalloonTimeout:F0}s";

    public bool IsContextMenuEnabled
    {
        get => _isContextMenuEnabled;
        set
        {
            if (SetProperty(ref _isContextMenuEnabled, value))
            {
                _notifyIconService.SetContextMenuEnabled(value);
            }
        }
    }

    public string EventLogText =>
        _eventLog.Count == 0
            ? "Interact with the tray icon to see events..."
            : string.Join("\n", _eventLog);

    public ICommand ShowIconCommand { get; }

    public ICommand HideIconCommand { get; }

    public ICommand ShowBalloonCommand { get; }

    public ICommand ClearLogCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    public override void Destroy()
    {
        _notifyIconService.Activity -= OnNotifyIconActivity;
        base.Destroy();
    }

    private void ShowIcon()
    {
        _notifyIconService.Show(TooltipText);
    }

    private void ShowBalloon()
    {
        _notifyIconService.ShowBalloon(
            Math.Max(1, (int)BalloonTimeout),
            BalloonTitle,
            BalloonText,
            SelectedBalloonIcon);
    }

    private void OnNotifyIconActivity(object? sender, string message)
    {
        _eventLog.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (_eventLog.Count > 20)
        {
            _eventLog.RemoveAt(0);
        }

        OnPropertyChanged(nameof(EventLogText));
    }

    private void ClearEventLog()
    {
        _eventLog.Clear();
        OnPropertyChanged(nameof(EventLogText));
    }
}
