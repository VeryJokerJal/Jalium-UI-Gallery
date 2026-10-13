using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class NavigationDemoViewModel
{
    private readonly IGalleryNavigationService _navigationService;
    private string _navigationStatus = "Navigation Status: Ready";

    public NavigationDemoViewModel(IGalleryNavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        _navigationService = navigationService;

        GoBackCommand = new RelayCommand(GoBack);
        GoForwardCommand = new RelayCommand(GoForward);
        RefreshCommand = new RelayCommand(Refresh);
        OpenNavigationWindowCommand = new RelayCommand(OpenNavigationWindow);
    }

    public ICommand GoBackCommand { get; }

    public ICommand GoForwardCommand { get; }

    public ICommand RefreshCommand { get; }

    public ICommand OpenNavigationWindowCommand { get; }

    public string NavigationStatus
    {
        get => _navigationStatus;
        private set => SetProperty(ref _navigationStatus, value);
    }

    private void GoBack()
    {
        NavigationStatus = _navigationService.TryGoBack()
            ? "Navigation Status: Navigated back"
            : "Navigation Status: Cannot go back - no history";
    }

    private void GoForward()
    {
        NavigationStatus = _navigationService.TryGoForward()
            ? "Navigation Status: Navigated forward"
            : "Navigation Status: Cannot go forward - no forward history";
    }

    private void Refresh()
    {
        try
        {
            _navigationService.Refresh();
            NavigationStatus = "Navigation Status: Page refreshed";
        }
        catch (Exception exception)
        {
            NavigationStatus =
                $"Navigation Status: Refresh failed: {exception.Message}";
        }
    }

    private void OpenNavigationWindow()
    {
        _navigationService.ShowNavigationWindowDemo();
        NavigationStatus = "Navigation Status: Opened NavigationWindow";
    }
}
