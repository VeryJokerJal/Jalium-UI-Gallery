using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

public partial class ScrollViewerPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Basic Vertical ScrollViewer -->
    <ScrollViewer Height=""200""
                  VerticalScrollBarVisibility=""Auto""
                  HorizontalScrollBarVisibility=""Disabled"">
        <StackPanel Orientation=""Vertical"" Margin=""12"">
            <TextBlock Text=""Scroll down to see more..."" Margin=""0,0,0,8""/>
            <Border Background=""#0078D4"" Height=""40"" Margin=""0,0,0,4"">
                <TextBlock Text=""Item 1"" Foreground=""White""
                           HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
            </Border>
            <Border Background=""#107C10"" Height=""40"" Margin=""0,0,0,4"">
                <TextBlock Text=""Item 2"" Foreground=""White""
                           HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
            </Border>
            <Border Background=""#881798"" Height=""40"" Margin=""0,0,0,4"">
                <TextBlock Text=""Item 3"" Foreground=""White""
                           HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
            </Border>
            <Border Background=""#CA5010"" Height=""40"" Margin=""0,0,0,4"">
                <TextBlock Text=""Item 4"" Foreground=""White""
                           HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
            </Border>
            <Border Background=""#008272"" Height=""40"">
                <TextBlock Text=""Item 5"" Foreground=""White""
                           HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
            </Border>
        </StackPanel>
    </ScrollViewer>

    <!-- ScrollViewer with both scrollbars -->
    <ScrollViewer Height=""300"" Width=""400""
                  VerticalScrollBarVisibility=""Visible""
                  HorizontalScrollBarVisibility=""Auto"">
        <Grid Width=""800"" Height=""600"">
            <TextBlock Text=""Large content area (800x600)""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""/>
        </Grid>
    </ScrollViewer>

    <!-- ScrollViewer with hidden scrollbar (touch-friendly) -->
    <ScrollViewer Height=""150""
                  VerticalScrollBarVisibility=""Hidden"">
        <StackPanel Orientation=""Vertical"">
            <TextBlock Text=""Swipe or use mouse wheel to scroll""/>
            <TextBlock Text=""The scrollbar is hidden but scrolling works""/>
        </StackPanel>
    </ScrollViewer>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;

namespace MyApp;

public partial class ScrollViewerDemo : Page
{
    private ScrollViewer _scrollViewer;

    public ScrollViewerDemo()
    {
        InitializeComponent();
        SetupScrollViewer();
    }

    private void SetupScrollViewer()
    {
        _scrollViewer = new ScrollViewer
        {
            Height = 300,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        var content = new StackPanel { Orientation = Orientation.Vertical };

        // Add many items to create scrollable content
        for (int i = 0; i < 50; i++)
        {
            content.Children.Add(new TextBlock
            {
                Text = $""Item {i + 1}"",
                Margin = new Thickness(8, 4, 8, 4),
                FontSize = 14
            });
        }

        _scrollViewer.Content = content;

        // Subscribe to scroll events
        _scrollViewer.ScrollChanged += OnScrollChanged;
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        // Check if scrolled to bottom
        var sv = (ScrollViewer)sender;
        double scrollableHeight = sv.ExtentHeight - sv.ViewportHeight;

        if (sv.VerticalOffset >= scrollableHeight - 1)
        {
            // User reached the bottom - load more items
            LoadMoreItems();
        }

        // Update scroll position display
        StatusText.Text = $""Offset: {sv.VerticalOffset:F0} / {scrollableHeight:F0}"";
    }

    private void ScrollToTop()
    {
        _scrollViewer.ScrollToVerticalOffset(0);
    }

    private void ScrollToBottom()
    {
        _scrollViewer.ScrollToEnd();
    }

    private void LoadMoreItems()
    {
        // Dynamically add more content
    }
}";

    public ScrollViewerPage()
    {
        InitializeComponent();
        LoadCodeExamples();
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
}
