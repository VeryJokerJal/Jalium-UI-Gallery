using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;
using Jalium.UI.Navigation;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryNavigationService : IGalleryNavigationService
{
    private readonly NavigationService _navigationService = new();

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
        var navigationWindow = new NavigationWindow
        {
            Title = "NavigationWindow Demo",
            Width = 600,
            Height = 400,
            ShowsNavigationUI = true
        };

        var page = new Page
        {
            Title = "Demo Page",
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = "NavigationWindow Demo",
                        FontSize = 24,
                        Foreground = GalleryTheme.TextPrimaryBrush,
                        Margin = new Thickness(20)
                    },
                    new TextBlock
                    {
                        Text = "This window has built-in navigation chrome.",
                        FontSize = 14,
                        Foreground = GalleryTheme.TextTertiaryBrush,
                        Margin = new Thickness(20, 0, 20, 20)
                    }
                }
            }
        };

        navigationWindow.Navigate(page);
        navigationWindow.Show();
    }
}
