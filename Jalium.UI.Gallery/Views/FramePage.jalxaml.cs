using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for FramePage.jalxaml demonstrating frame navigation functionality.
/// </summary>
public partial class FramePage : Page
{
    private int _currentPage = 0;

    public FramePage()
    {
        InitializeComponent();

        // Set up navigation buttons
        if (NavigatePage1Button != null)
            NavigatePage1Button.Click += (s, e) => NavigateToPage(1);
        if (NavigatePage2Button != null)
            NavigatePage2Button.Click += (s, e) => NavigateToPage(2);
        if (NavigatePage3Button != null)
            NavigatePage3Button.Click += (s, e) => NavigateToPage(3);

        // Set up back/forward buttons
        if (BackButton != null)
            BackButton.Click += (s, e) => GoBack();
        if (ForwardButton != null)
            ForwardButton.Click += (s, e) => GoForward();

        // Initialize with first page
        NavigateToPage(1);
    }

    private void NavigateToPage(int pageNumber)
    {
        _currentPage = pageNumber;
        UpdateNavigationHistory();

        if (DemoFrame != null)
        {
            // Create a simple page content
            var content = new Border
            {
                Background = new Jalium.UI.Media.SolidColorBrush(
                    pageNumber == 1 ? Jalium.UI.Media.Color.FromArgb(255, 0, 120, 212) :
                    pageNumber == 2 ? Jalium.UI.Media.Color.FromArgb(255, 0, 204, 106) :
                    Jalium.UI.Media.Color.FromArgb(255, 247, 99, 12)),
                Child = new TextBlock
                {
                    Text = $"Page {pageNumber}",
                    FontSize = 24,
                    Foreground = new Jalium.UI.Media.SolidColorBrush(Jalium.UI.Media.Color.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            DemoFrame.Content = content;
        }
    }

    private void GoBack()
    {
        if (_currentPage > 1)
        {
            NavigateToPage(_currentPage - 1);
        }
    }

    private void GoForward()
    {
        if (_currentPage < 3)
        {
            NavigateToPage(_currentPage + 1);
        }
    }

    private void UpdateNavigationHistory()
    {
        if (NavigationHistoryText != null)
        {
            NavigationHistoryText.Text = $"Current page: {_currentPage}";
        }
    }
}
