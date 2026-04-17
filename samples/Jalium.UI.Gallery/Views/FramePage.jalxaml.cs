using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

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

        LoadCodeExamples();
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

    private const string XamlExample =
@"<!-- Basic Frame -->
<Frame x:Name=""DemoFrame""
       Width=""400"" Height=""200""/>

<!-- Navigation Buttons -->
<StackPanel Orientation=""Horizontal"">
    <Button x:Name=""NavigatePage1Button""
            Content=""Page 1"" Width=""80"" Height=""28""/>
    <Button x:Name=""NavigatePage2Button""
            Content=""Page 2"" Width=""80"" Height=""28""/>
    <Button x:Name=""NavigatePage3Button""
            Content=""Page 3"" Width=""80"" Height=""28""/>
</StackPanel>

<!-- Back/Forward Navigation -->
<StackPanel Orientation=""Horizontal"">
    <Button x:Name=""BackButton"" Content=""Back""/>
    <Button x:Name=""ForwardButton"" Content=""Forward""/>
</StackPanel>";

    private const string CSharpExample =
@"// Navigate to a page within a Frame
var frame = new Frame();
frame.Content = new MyPage();

// Navigate with content
private void NavigateToPage(int pageNumber)
{
    var content = new Border
    {
        Background = new SolidColorBrush(
            Color.FromArgb(255, 0, 120, 212)),
        Child = new TextBlock
        {
            Text = $""Page {pageNumber}"",
            FontSize = 24,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        }
    };
    DemoFrame.Content = content;
}

// Back/Forward navigation
private void GoBack()
{
    if (frame.CanGoBack)
        frame.GoBack();
}

private void GoForward()
{
    if (frame.CanGoForward)
        frame.GoForward();
}";

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
}
