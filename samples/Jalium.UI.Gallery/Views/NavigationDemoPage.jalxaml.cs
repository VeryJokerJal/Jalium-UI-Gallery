using Jalium.UI.Controls;
using Jalium.UI.Controls.Navigation;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class NavigationDemoPage : Page
{
    private readonly NavigationService _navigationService;

    public NavigationDemoPage()
    {
        InitializeComponent();
        _navigationService = new NavigationService();
        SetupButtons();
        SetupNavigationEvents();
    }

    private void SetupButtons()
    {
        if (BackButton != null)
        {
            BackButton.Click += (s, e) =>
            {
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                    UpdateStatus("Navigated back");
                }
                else
                {
                    UpdateStatus("Cannot go back - no history");
                }
            };
        }

        if (ForwardButton != null)
        {
            ForwardButton.Click += (s, e) =>
            {
                if (_navigationService.CanGoForward)
                {
                    _navigationService.GoForward();
                    UpdateStatus("Navigated forward");
                }
                else
                {
                    UpdateStatus("Cannot go forward - no forward history");
                }
            };
        }

        if (RefreshButton != null)
        {
            RefreshButton.Click += (s, e) =>
            {
                _navigationService.Refresh();
                UpdateStatus("Page refreshed");
            };
        }

        if (OpenNavigationWindowButton != null)
        {
            OpenNavigationWindowButton.Click += (s, e) =>
            {
                // Create and show a NavigationWindow demo
                var navWindow = new NavigationWindow
                {
                    Title = "NavigationWindow Demo",
                    Width = 600,
                    Height = 400,
                    ShowsNavigationUI = true
                };

                // Navigate to a simple page
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
                                Foreground = new SolidColorBrush(Color.White),
                                Margin = new Thickness(20)
                            },
                            new TextBlock
                            {
                                Text = "This window has built-in navigation chrome.",
                                FontSize = 14,
                                Foreground = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160)),
                                Margin = new Thickness(20, 0, 20, 20)
                            }
                        }
                    }
                };

                navWindow.Navigate(page);
                navWindow.Show();

                UpdateStatus("Opened NavigationWindow");
            };
        }
    }

    private void SetupNavigationEvents()
    {
        _navigationService.Navigating += (s, e) =>
        {
            UpdateStatus($"Navigating (Mode: {e.NavigationMode})...");
        };

        _navigationService.Navigated += (s, e) =>
        {
            UpdateStatus($"Navigation completed (Mode: {e.NavigationMode})");
        };

        _navigationService.NavigationFailed += (s, e) =>
        {
            UpdateStatus($"Navigation failed: {e.Exception.Message}");
        };
    }

    private void UpdateStatus(string message)
    {
        if (NavigationStatus != null)
        {
            NavigationStatus.Text = $"Navigation Status: {message}";
        }
    }
}
