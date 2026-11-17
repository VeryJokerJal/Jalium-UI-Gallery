using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Services.Interfaces;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>
/// View model for the application shell. Shell state and user actions live here;
/// <c>MainWindow</c> is responsible only for rendering and platform-specific view work.
/// </summary>
public sealed class MainWindowViewModel : RegionViewModelBase
{
    private readonly IMessageService? _messageService;
    private readonly IGalleryThemeService? _themeService;
    private readonly IGalleryLocalizationService? _localizationService;
    private readonly IGalleryNavigationCoordinator? _navigationCoordinator;
    private readonly IExternalUriService? _externalUriService;
    private readonly HomeViewModel? _homeViewModel;
    private string _message;
    private string _currentPageTag = "home";
    private string _searchQuery = string.Empty;

    /// <summary>Creates design/test state when the View is used outside a hosted app.</summary>
    public MainWindowViewModel()
    {
        _message = "Jalium.UI Gallery";
        RefreshCommand = new RelayCommand(Refresh);
        ToggleThemeCommand = new RelayCommand(ToggleTheme);
        SwitchLanguageCommand = new RelayCommand(SwitchLanguage);
        NavigateCommand = new RelayCommand<string>(Navigate);
        OpenExternalUriCommand = new RelayCommand<string>(OpenExternalUri);
        OpenBestMatchCommand = new RelayCommand(OpenBestMatch);
    }

    public MainWindowViewModel(
        IMessageService messageService,
        IGalleryThemeService themeService,
        IGalleryLocalizationService localizationService,
        IGalleryNavigationCoordinator navigationCoordinator,
        IExternalUriService externalUriService,
        HomeViewModel homeViewModel) : this()
    {
        ArgumentNullException.ThrowIfNull(messageService);
        ArgumentNullException.ThrowIfNull(themeService);
        ArgumentNullException.ThrowIfNull(localizationService);
        ArgumentNullException.ThrowIfNull(navigationCoordinator);
        ArgumentNullException.ThrowIfNull(externalUriService);
        ArgumentNullException.ThrowIfNull(homeViewModel);
        _messageService = messageService;
        _themeService = themeService;
        _localizationService = localizationService;
        _navigationCoordinator = navigationCoordinator;
        _externalUriService = externalUriService;
        _homeViewModel = homeViewModel;
        _themeService.ThemeChanged += OnThemeChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;
        _navigationCoordinator.NavigationRequested += OnNavigationRequested;
        _message = messageService.GetMessage();
    }

    /// <summary>Latest message shown on the view.</summary>
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    /// <summary>Pulls a fresh greeting from the message service.</summary>
    public ICommand RefreshCommand { get; }

    /// <summary>Switches between the light and dark application palettes.</summary>
    public ICommand ToggleThemeCommand { get; }

    /// <summary>Advances the display language to the next shipped one.</summary>
    public ICommand SwitchLanguageCommand { get; }

    public ICommand NavigateCommand { get; }

    public ICommand OpenExternalUriCommand { get; }

    public ICommand OpenBestMatchCommand { get; }

    public string CurrentPageTag
    {
        get => _currentPageTag;
        set => SetProperty(ref _currentPageTag, value);
    }

    public string SearchQuery
    {
        get => _homeViewModel?.SearchQuery ?? _searchQuery;
        set
        {
            var normalized = value ?? string.Empty;
            if (_homeViewModel != null)
            {
                if (string.Equals(
                        _homeViewModel.SearchQuery,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return;
                }

                _homeViewModel.SearchQuery = normalized;
                OnPropertyChanged();
                return;
            }

            SetProperty(ref _searchQuery, normalized);
        }
    }

    public bool IsDarkTheme => _themeService?.IsDark == true;

    public bool IsLightTheme => !IsDarkTheme;

    public string ThemeLabel => LocalizationManager.Get(
        IsDarkTheme ? "Loc.Shell.ThemeDark" : "Loc.Shell.ThemeLight");

    /// <summary>
    /// The active language spelled in its own script, so the switcher reads the
    /// same whatever language the shell is currently in.
    /// </summary>
    public string LanguageLabel => CurrentLanguageInfo.NativeName;

    /// <summary>One-glyph form of <see cref="LanguageLabel"/> for the mobile header.</summary>
    public string LanguageShortLabel => CurrentLanguageInfo.ShortLabel;

    private GalleryLanguageInfo CurrentLanguageInfo =>
        _localizationService?.CurrentInfo ?? LocalizationManager.CurrentLanguageInfo;

    private void Refresh()
    {
        if (_messageService != null)
        {
            Message = _messageService.GetMessage();
        }
    }

    private void ToggleTheme()
    {
        _themeService?.Toggle();
    }

    private void SwitchLanguage()
    {
        _localizationService?.SwitchToNext();
    }

    private void Navigate(string? pageTag)
    {
        if (!string.IsNullOrWhiteSpace(pageTag))
        {
            _navigationCoordinator?.RequestNavigation(pageTag);
        }
    }

    private void OpenExternalUri(string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri) || _externalUriService == null)
        {
            return;
        }

        try
        {
            _externalUriService.Open(uri);
        }
        catch (Exception exception)
        {
            Message = $"Unable to open link: {exception.Message}";
        }
    }

    private void OpenBestMatch()
    {
        if (_homeViewModel?.OpenBestMatchCommand.CanExecute(null) == true)
        {
            _homeViewModel.OpenBestMatchCommand.Execute(null);
        }
    }

    public override void Destroy()
    {
        if (_navigationCoordinator != null)
        {
            _navigationCoordinator.NavigationRequested -= OnNavigationRequested;
        }

        if (_themeService != null)
        {
            _themeService.ThemeChanged -= OnThemeChanged;
        }

        if (_localizationService != null)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
        }

        base.Destroy();
    }

    private void OnThemeChanged(
        object? sender,
        EventArgs args)
    {
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(IsLightTheme));
        OnPropertyChanged(nameof(ThemeLabel));
    }

    private void OnLanguageChanged(
        object? sender,
        EventArgs args)
    {
        OnPropertyChanged(nameof(ThemeLabel));
        OnPropertyChanged(nameof(LanguageLabel));
        OnPropertyChanged(nameof(LanguageShortLabel));
    }

    private void OnNavigationRequested(
        object? sender,
        GalleryNavigationRequestedEventArgs args) =>
        CurrentPageTag = args.PageTag;
}
