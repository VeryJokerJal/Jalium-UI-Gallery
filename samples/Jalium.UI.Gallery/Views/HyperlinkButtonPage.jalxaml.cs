using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for HyperlinkButtonPage.jalxaml demonstrating HyperlinkButton functionality.
/// </summary>
public partial class HyperlinkButtonPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic HyperlinkButton with NavigateUri -->
    <HyperlinkButton Content=""Visit Microsoft""
                     NavigateUri=""https://www.microsoft.com""
                     Margin=""0,0,0,8""/>

    <!-- HyperlinkButton without underline -->
    <HyperlinkButton Content=""GitHub Repository""
                     NavigateUri=""https://github.com""
                     IsUnderlined=""False""
                     Margin=""0,0,0,8""/>

    <!-- HyperlinkButton with custom foreground -->
    <HyperlinkButton Content=""Documentation""
                     NavigateUri=""https://docs.example.com""
                     Foreground=""#FF8C00""
                     Margin=""0,0,0,16""/>

    <!-- Inline with text -->
    <StackPanel Orientation=""Horizontal"">
        <TextBlock Text=""By continuing, you agree to our ""
                   VerticalAlignment=""Center""/>
        <HyperlinkButton Content=""Terms of Service""
                         NavigateUri=""https://example.com/tos""
                         VerticalAlignment=""Center""
                         Padding=""0""/>
        <TextBlock Text="" and ""
                   VerticalAlignment=""Center""/>
        <HyperlinkButton Content=""Privacy Policy""
                         NavigateUri=""https://example.com/privacy""
                         VerticalAlignment=""Center""
                         Padding=""0""/>
        <TextBlock Text=""."" VerticalAlignment=""Center""/>
    </StackPanel>

    <!-- HyperlinkButton with click handler -->
    <HyperlinkButton x:Name=""CustomLink""
                     Content=""Custom Action Link""
                     Margin=""0,16,0,0""
                     Click=""OnCustomLinkClick""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class HyperlinkButtonSample : Page
{
    public HyperlinkButtonSample()
    {
        InitializeComponent();
        SetupHyperlinkButtons();
    }

    private void SetupHyperlinkButtons()
    {
        // Create HyperlinkButton programmatically
        var link = new HyperlinkButton
        {
            Content = ""Visit Website"",
            NavigateUri = new Uri(""https://example.com""),
            IsUnderlined = true
        };

        // HyperlinkButton with click handler instead of URI
        var actionLink = new HyperlinkButton
        {
            Content = ""Open Settings"",
            IsUnderlined = false
        };
        actionLink.Click += OnOpenSettingsClick;

        // Styled hyperlink for footer
        var footerLink = new HyperlinkButton
        {
            Content = ""About"",
            Foreground = new SolidColorBrush(Colors.Gray),
            FontSize = 12,
            Padding = new Thickness(0)
        };
        footerLink.NavigateUri = new Uri(""https://example.com/about"");

        ContentPanel.Children.Add(link);
        ContentPanel.Children.Add(actionLink);
        ContentPanel.Children.Add(footerLink);
    }

    private void OnCustomLinkClick(object? sender, RoutedEventArgs e)
    {
        // Handle custom navigation or action
        var dialog = new ContentDialog
        {
            Title = ""Link Clicked"",
            Content = ""You clicked the custom action link!"",
            CloseButtonText = ""OK""
        };
        dialog.ShowAsync();
    }

    private void OnOpenSettingsClick(object? sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(SettingsPage));
    }
}";

    public HyperlinkButtonPage()
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
