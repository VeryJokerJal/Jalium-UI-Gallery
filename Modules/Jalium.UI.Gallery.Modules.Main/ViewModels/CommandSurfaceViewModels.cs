using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class AppBarButtonViewModel
{
    private string _basicStatus = "Click any button to see its action.";
    private string _compactStatus = "Compact buttons show icon only, hover for tooltip with label.";

    public AppBarButtonViewModel()
    {
        BasicCommand = new RelayCommand<string>(action =>
            BasicStatus = $"{action ?? "Unknown"} button clicked.");
        CompactCommand = new RelayCommand<string>(action =>
            CompactStatus = $"Compact {action ?? "Unknown"} clicked.");
    }

    public ICommand BasicCommand { get; }

    public ICommand CompactCommand { get; }

    public string BasicStatus
    {
        get => _basicStatus;
        private set => SetProperty(ref _basicStatus, value);
    }

    public string CompactStatus
    {
        get => _compactStatus;
        private set => SetProperty(ref _compactStatus, value);
    }
}

public sealed partial class AppBarSeparatorViewModel
{
    private string? _lastAction;

    public AppBarSeparatorViewModel()
    {
        ExecuteToolbarActionCommand = new RelayCommand<string>(ExecuteToolbarAction);
        LocalizationManager.LanguageChanged += OnLanguageChanged;
    }

    public ICommand ExecuteToolbarActionCommand { get; }

    /// <summary>
    /// Resolved on read rather than stored, so a language switch re-renders the
    /// status line without the demo having to be clicked again.
    /// </summary>
    public string ToolbarStatus => _lastAction is null
        ? LocalizationManager.Get("Loc.P.AppBarSeparator.St0")
        : LocalizationManager.Get(_lastAction switch
        {
            "New" => "Loc.P.AppBarSeparator.St1",
            "Open" => "Loc.P.AppBarSeparator.St2",
            "Save" => "Loc.P.AppBarSeparator.St3",
            "Cut" => "Loc.P.AppBarSeparator.St4",
            "Copy" => "Loc.P.AppBarSeparator.St5",
            "Paste" => "Loc.P.AppBarSeparator.St6",
            "Undo" => "Loc.P.AppBarSeparator.St7",
            "Redo" => "Loc.P.AppBarSeparator.St8",
            _ => "Loc.P.AppBarSeparator.St9"
        });

    private void ExecuteToolbarAction(string? action)
    {
        _lastAction = action ?? string.Empty;
        OnPropertyChanged(nameof(ToolbarStatus));
    }

    private void OnLanguageChanged(object? sender, EventArgs args) =>
        OnPropertyChanged(nameof(ToolbarStatus));
}

public sealed partial class AppBarToggleButtonViewModel
{
    private bool _isBold;
    private bool _isItalic;
    private bool _isUnderline;
    private bool _isWifiEnabled;
    private bool _isBluetoothEnabled;
    private bool _isAirplaneModeEnabled;

    public bool IsBold
    {
        get => _isBold;
        set
        {
            if (SetProperty(ref _isBold, value))
            {
                NotifyFormattingChanged();
            }
        }
    }

    public bool IsItalic
    {
        get => _isItalic;
        set
        {
            if (SetProperty(ref _isItalic, value))
            {
                NotifyFormattingChanged();
            }
        }
    }

    public bool IsUnderline
    {
        get => _isUnderline;
        set
        {
            if (SetProperty(ref _isUnderline, value))
            {
                NotifyFormattingChanged();
            }
        }
    }

    public bool IsWifiEnabled
    {
        get => _isWifiEnabled;
        set
        {
            if (SetProperty(ref _isWifiEnabled, value))
            {
                OnPropertyChanged(nameof(ConnectivityStatus));
            }
        }
    }

    public bool IsBluetoothEnabled
    {
        get => _isBluetoothEnabled;
        set
        {
            if (SetProperty(ref _isBluetoothEnabled, value))
            {
                OnPropertyChanged(nameof(ConnectivityStatus));
            }
        }
    }

    public bool IsAirplaneModeEnabled
    {
        get => _isAirplaneModeEnabled;
        set
        {
            if (SetProperty(ref _isAirplaneModeEnabled, value))
            {
                OnPropertyChanged(nameof(ConnectivityStatus));
            }
        }
    }

    public FontWeight PreviewFontWeight =>
        IsBold ? FontWeights.Bold : FontWeights.Normal;

    public FontStyle PreviewFontStyle =>
        IsItalic ? FontStyles.Italic : FontStyles.Normal;

    public string FormattingPreview
    {
        get
        {
            var activeFormats = new List<string>(3);
            if (IsBold)
            {
                activeFormats.Add("Bold");
            }

            if (IsItalic)
            {
                activeFormats.Add("Italic");
            }

            if (IsUnderline)
            {
                activeFormats.Add("Underline");
            }

            return activeFormats.Count > 0
                ? $"Sample text with {string.Join(", ", activeFormats)} formatting"
                : "Sample text with no formatting";
        }
    }

    public string ConnectivityStatus =>
        $"Wi-Fi: {ToState(IsWifiEnabled)} | Bluetooth: {ToState(IsBluetoothEnabled)} | Airplane Mode: {ToState(IsAirplaneModeEnabled)}";

    private static string ToState(bool value) => value ? "On" : "Off";

    private void NotifyFormattingChanged()
    {
        OnPropertyChanged(nameof(PreviewFontWeight));
        OnPropertyChanged(nameof(PreviewFontStyle));
        OnPropertyChanged(nameof(FormattingPreview));
    }
}

public sealed partial class ExpanderViewModel
{
    private bool _isControlledExpanderOpen;

    public ExpanderViewModel()
    {
        ExpandCommand = new RelayCommand(() => IsControlledExpanderOpen = true);
        CollapseCommand = new RelayCommand(() => IsControlledExpanderOpen = false);
        ToggleCommand = new RelayCommand(() =>
            IsControlledExpanderOpen = !IsControlledExpanderOpen);
    }

    public ICommand ExpandCommand { get; }

    public ICommand CollapseCommand { get; }

    public ICommand ToggleCommand { get; }

    public bool IsControlledExpanderOpen
    {
        get => _isControlledExpanderOpen;
        set => SetProperty(ref _isControlledExpanderOpen, value);
    }
}
