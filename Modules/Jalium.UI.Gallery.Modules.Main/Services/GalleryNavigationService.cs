using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Navigation;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryNavigationService : IGalleryNavigationService
{
    private NavigationService _navigationService = new();

    public bool TryGoBack()
    {
        if (!_navigationService.CanGoBack)
        {
            return false;
        }

        _navigationService.GoBack();
        return true;
    }

    public bool TryGoForward()
    {
        if (!_navigationService.CanGoForward)
        {
            return false;
        }

        _navigationService.GoForward();
        return true;
    }

    public void Refresh() => _navigationService.Refresh();

    public void ShowNavigationWindowDemo()
    {
        var window = CreateNavigationWindowDemo();
        var service = window.NavigationService;
        _navigationService = service;
        window.Closed += (_, _) =>
        {
            if (ReferenceEquals(_navigationService, service)) _navigationService = new NavigationService();
        };
        window.Show();
    }

    internal NavigationWindow CreateNavigationWindowDemo()
    {
        var window = new NavigationWindow
        {
            Width = 600,
            Height = 400,
            Owner = Application.Current?.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        int number = 0;
        void NextPage()
        {
            var next = number + 1;
            var page = new NavigationWindowSamplePage
            {
                DataContext = new NavigationWindowSampleViewModel(next, NextPage)
            };
            if (window.Navigate(page)) number = next;
        }
        NextPage();
        return window;
    }
}
