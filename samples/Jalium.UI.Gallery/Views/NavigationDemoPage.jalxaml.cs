using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
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
        LoadCodeExamples();
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

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }

    private const string XamlExample =
@"<!-- NavigationWindow with built-in navigation chrome -->
<NavigationWindow Title=""My App""
                   Width=""800"" Height=""600""
                   ShowsNavigationUI=""True""
                   Source=""Views/HomePage.jalxaml""/>

<!-- Page with navigation buttons -->
<Page Title=""Navigation Demo"">
    <StackPanel Orientation=""Horizontal"">
        <Button x:Name=""BackButton"" Content=""Go Back"" Width=""100""/>
        <Button x:Name=""ForwardButton"" Content=""Go Forward"" Width=""100""/>
        <Button x:Name=""RefreshButton"" Content=""Refresh"" Width=""100""/>
    </StackPanel>
</Page>

<!-- Frame for embedding navigation within a page -->
<Frame x:Name=""ContentFrame""
       NavigationUIVisibility=""Hidden""
       Source=""Views/InitialPage.jalxaml""/>";

    private const string CSharpExample =
@"// Get NavigationService and navigate
var navService = NavigationService.GetNavigationService(this);

// Navigate to a new page
navService.Navigate(new Uri(""Views/DetailPage.jalxaml"",
    UriKind.Relative));

// Navigate with custom content state
var entry = new JournalEntry(uri, ""Page Title"");
entry.CustomContentState = new MyContentState();
navService.AddBackEntry(entry.CustomContentState);

// Handle navigation events
navService.Navigating += (s, e) =>
{
    // e.NavigationMode: New, Back, Forward, Refresh
    Console.WriteLine($""Navigating: {e.NavigationMode}"");
};

navService.Navigated += (s, e) =>
{
    Console.WriteLine(""Navigation completed"");
};

// Back/Forward navigation
if (navService.CanGoBack)
    navService.GoBack();

if (navService.CanGoForward)
    navService.GoForward();

// Open a NavigationWindow
var navWindow = new NavigationWindow
{
    Title = ""Demo"",
    Width = 600, Height = 400,
    ShowsNavigationUI = true
};
navWindow.Navigate(new MyPage());
navWindow.Show();";
}
