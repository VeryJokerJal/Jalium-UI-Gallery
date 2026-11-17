using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class TitleBarViewModel
{
    private bool _showMinimizeButton = true;
    private bool _showMaximizeButton = true;
    private bool _showCloseButton = true;
    private bool _isMaximized;
    private string _titleBarStatus = "Status: Ready";

    public TitleBarViewModel()
    {
        MinimizeCommand = new RelayCommand(() =>
            TitleBarStatus = "Status: Minimize clicked (demo only)");
        MaximizeRestoreCommand = new RelayCommand(ToggleMaximized);
        CloseCommand = new RelayCommand(() =>
            TitleBarStatus = "Status: Close clicked (demo only)");
    }

    public ICommand MinimizeCommand { get; }

    public ICommand MaximizeRestoreCommand { get; }

    public ICommand CloseCommand { get; }

    public bool ShowMinimizeButton
    {
        get => _showMinimizeButton;
        set => SetDemoOption(
            ref _showMinimizeButton,
            value,
            nameof(ShowMinimizeButton));
    }

    public bool ShowMaximizeButton
    {
        get => _showMaximizeButton;
        set => SetDemoOption(
            ref _showMaximizeButton,
            value,
            nameof(ShowMaximizeButton));
    }

    public bool ShowCloseButton
    {
        get => _showCloseButton;
        set => SetDemoOption(
            ref _showCloseButton,
            value,
            nameof(ShowCloseButton));
    }

    public bool IsMaximized
    {
        get => _isMaximized;
        set => SetDemoOption(ref _isMaximized, value, nameof(IsMaximized));
    }

    public string TitleBarStatus
    {
        get => _titleBarStatus;
        private set => SetProperty(ref _titleBarStatus, value);
    }

    private void ToggleMaximized()
    {
        var nextState = !IsMaximized;
        if (SetProperty(ref _isMaximized, nextState, nameof(IsMaximized)))
        {
            TitleBarStatus =
                $"Status: Maximize/Restore clicked (IsMaximized={nextState})";
        }
    }

    private void SetDemoOption(ref bool field, bool value, string propertyName)
    {
        if (SetProperty(ref field, value, propertyName))
        {
            TitleBarStatus = "Status: Demo options updated";
        }
    }
}

public sealed partial class ToolBarViewModel
{
    private string _lastAction = "No action performed";

    public ToolBarViewModel()
    {
        ExecuteCommand = new RelayCommand<string>(action =>
            LastAction = $"{action ?? "Unknown"} clicked");
    }

    public ICommand ExecuteCommand { get; }

    public string LastAction
    {
        get => _lastAction;
        private set => SetProperty(ref _lastAction, value);
    }
}

public sealed partial class RibbonViewModel
{
    private string _ribbonStatus = "Click ribbon buttons to see interactions...";
    private bool _isRibbonMinimized;
    private int _selectedTabIndex;

    public RibbonViewModel()
    {
        ExecuteCommand = new RelayCommand<string>(action =>
            RibbonStatus = $"{action ?? "Unknown"} clicked");
    }

    public ICommand ExecuteCommand { get; }

    public string RibbonStatus
    {
        get => _ribbonStatus;
        private set => SetProperty(ref _ribbonStatus, value);
    }

    public bool IsRibbonMinimized
    {
        get => _isRibbonMinimized;
        set => SetProperty(ref _isRibbonMinimized, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }
}

public sealed partial class PopupViewModel
{
    private bool _isBasicPopupOpen;
    private bool _isFormPopupOpen;
    private string _itemName = string.Empty;
    private string _description = string.Empty;

    public PopupViewModel()
    {
        ToggleBasicPopupCommand = new RelayCommand(() =>
            IsBasicPopupOpen = !IsBasicPopupOpen);
        CloseBasicPopupCommand = new RelayCommand(() =>
            IsBasicPopupOpen = false);
        ToggleFormPopupCommand = new RelayCommand(() =>
            IsFormPopupOpen = !IsFormPopupOpen);
        CancelFormCommand = new RelayCommand(CloseForm);
        SaveFormCommand = new RelayCommand(SaveForm);
    }

    public ICommand ToggleBasicPopupCommand { get; }

    public ICommand CloseBasicPopupCommand { get; }

    public ICommand ToggleFormPopupCommand { get; }

    public ICommand CancelFormCommand { get; }

    public ICommand SaveFormCommand { get; }

    public bool IsBasicPopupOpen
    {
        get => _isBasicPopupOpen;
        set => SetProperty(ref _isBasicPopupOpen, value);
    }

    public bool IsFormPopupOpen
    {
        get => _isFormPopupOpen;
        set => SetProperty(ref _isFormPopupOpen, value);
    }

    public string ItemName
    {
        get => _itemName;
        set => SetProperty(ref _itemName, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private void SaveForm()
    {
        ItemName = string.Empty;
        Description = string.Empty;
        IsFormPopupOpen = false;
    }

    private void CloseForm() => IsFormPopupOpen = false;
}
