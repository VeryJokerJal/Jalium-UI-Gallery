using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Localization;
using KeyGesture = Jalium.UI.Input.KeyGesture;
using StandardUICommand = Jalium.UI.Input.StandardUICommand;
using StandardUICommandKind = Jalium.UI.Input.StandardUICommandKind;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed record StandardCommandItem(
    StandardUICommand Command,
    string KeyboardShortcut)
{
    public string Label => Command.Label;

    public string IconGlyph => Command.IconSource as string ?? string.Empty;

    public string Description => Command.Description;
}

public sealed partial class StandardUICommandViewModel
{
    private string _commandStatus = "No command executed yet.";

    public StandardUICommandViewModel()
    {
        ExecuteCommand = new RelayCommand<string>(label =>
            CommandStatus = $"Executed: {label ?? "Unknown"}");
        Commands =
        [
            Item(StandardUICommandKind.Cut),
            Item(StandardUICommandKind.Copy),
            Item(StandardUICommandKind.Paste),
            Item(StandardUICommandKind.Delete),
            Item(StandardUICommandKind.Undo),
            Item(StandardUICommandKind.Redo),
            Item(StandardUICommandKind.SelectAll),
            Item(StandardUICommandKind.Save)
        ];
    }

    public IReadOnlyList<StandardCommandItem> Commands { get; }

    public ICommand ExecuteCommand { get; }

    public string CommandStatus
    {
        get => _commandStatus;
        private set => SetProperty(ref _commandStatus, value);
    }

    private StandardCommandItem Item(StandardUICommandKind kind)
    {
        var command = new StandardUICommand(kind)
        {
            Command = ExecuteCommand
        };
        var gesture = command.KeyboardAccelerators
            .OfType<KeyGesture>()
            .FirstOrDefault();
        var shortcut = gesture is null ? string.Empty : GalleryShortcutText.Format(gesture);
        return new StandardCommandItem(command, shortcut);
    }
}

public sealed partial class MenuBarViewModel
{
    private string _menuStatus = "No menu action yet.";
    private bool _isStatusBarVisible = true;
    private bool _isWordWrapEnabled;

    public MenuBarViewModel()
    {
        ExecuteMenuCommand = new RelayCommand<string>(path =>
            MenuStatus = $"{path ?? "Unknown"} clicked");
    }

    public ICommand ExecuteMenuCommand { get; }

    public string MenuStatus
    {
        get => _menuStatus;
        private set => SetProperty(ref _menuStatus, value);
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set
        {
            if (SetProperty(ref _isStatusBarVisible, value))
            {
                MenuStatus =
                    $"View > Status Bar toggled: {value}";
            }
        }
    }

    public bool IsWordWrapEnabled
    {
        get => _isWordWrapEnabled;
        set
        {
            if (SetProperty(ref _isWordWrapEnabled, value))
            {
                MenuStatus =
                    $"View > Word Wrap toggled: {value}";
            }
        }
    }
}

public sealed partial class CommandBarViewModel
{
    private string _basicStatus = "Click a command to see its action.";
    private string _secondaryStatus =
        "Click the overflow button to see secondary commands.";

    public CommandBarViewModel()
    {
        ExecuteBasicCommand = new RelayCommand<string>(action =>
            BasicStatus = $"{action ?? "Unknown"} command executed.");
        ExecuteSecondaryCommand = new RelayCommand<string>(action =>
            SecondaryStatus = $"{action ?? "Unknown"} command executed.");
    }

    public ICommand ExecuteBasicCommand { get; }

    public ICommand ExecuteSecondaryCommand { get; }

    public string BasicStatus
    {
        get => _basicStatus;
        private set => SetProperty(ref _basicStatus, value);
    }

    public string SecondaryStatus
    {
        get => _secondaryStatus;
        private set => SetProperty(ref _secondaryStatus, value);
    }
}

public sealed partial class CommandBarFlyoutViewModel
{
    private string _basicStatus =
        "Click the button to open the CommandBarFlyout.";
    private string _mixedStatus =
        "Click the button to open the flyout with mixed commands.";

    public CommandBarFlyoutViewModel()
    {
        OpenBasicFlyoutCommand = new RelayCommand(() =>
            BasicStatus = "CommandBarFlyout opened.");
        OpenMixedFlyoutCommand = new RelayCommand(() =>
            MixedStatus = "Mixed CommandBarFlyout opened.");
        ExecuteBasicCommand = new RelayCommand<string>(action =>
            BasicStatus = $"{action ?? "Unknown"} command executed.");
        ExecuteMixedCommand = new RelayCommand<string>(action =>
            MixedStatus = $"{action ?? "Unknown"} command executed.");
    }

    public ICommand OpenBasicFlyoutCommand { get; }

    public ICommand OpenMixedFlyoutCommand { get; }

    public ICommand ExecuteBasicCommand { get; }

    public ICommand ExecuteMixedCommand { get; }

    public string BasicStatus
    {
        get => _basicStatus;
        private set => SetProperty(ref _basicStatus, value);
    }

    public string MixedStatus
    {
        get => _mixedStatus;
        private set => SetProperty(ref _mixedStatus, value);
    }
}

public sealed partial class MenuFlyoutViewModel
{
    private string _basicStatus = "Click the button to open the menu.";
    private string _iconStatus = "Click the button to open the icon menu.";
    private string _checkStatus = "Toggle view options on/off.";
    private bool _isToolbarVisible = true;
    private bool _isStatusBarVisible = true;
    private bool _isSidebarVisible;
    private bool _isMiniMapVisible;
    private bool _areBreadcrumbsVisible = true;

    public MenuFlyoutViewModel()
    {
        ExecuteBasicCommand = new RelayCommand<string>(action =>
            BasicStatus = $"{action ?? "Unknown"} selected");
        ExecuteIconCommand = new RelayCommand<string>(action =>
            IconStatus = $"{action ?? "Unknown"} action triggered");
    }

    public ICommand ExecuteBasicCommand { get; }

    public ICommand ExecuteIconCommand { get; }

    public string BasicStatus
    {
        get => _basicStatus;
        private set => SetProperty(ref _basicStatus, value);
    }

    public string IconStatus
    {
        get => _iconStatus;
        private set => SetProperty(ref _iconStatus, value);
    }

    public string CheckStatus
    {
        get => _checkStatus;
        private set => SetProperty(ref _checkStatus, value);
    }

    public bool IsToolbarVisible
    {
        get => _isToolbarVisible;
        set => SetOption(ref _isToolbarVisible, value, "Toolbar");
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => SetOption(ref _isStatusBarVisible, value, "Status Bar");
    }

    public bool IsSidebarVisible
    {
        get => _isSidebarVisible;
        set => SetOption(ref _isSidebarVisible, value, "Sidebar");
    }

    public bool IsMiniMapVisible
    {
        get => _isMiniMapVisible;
        set => SetOption(ref _isMiniMapVisible, value, "Minimap");
    }

    public bool AreBreadcrumbsVisible
    {
        get => _areBreadcrumbsVisible;
        set => SetOption(ref _areBreadcrumbsVisible, value, "Breadcrumbs");
    }

    private void SetOption(ref bool field, bool value, string label)
    {
        if (SetProperty(ref field, value))
        {
            CheckStatus = $"{label}: {(value ? "Visible" : "Hidden")}";
        }
    }
}
