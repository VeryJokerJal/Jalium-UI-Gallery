using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

public partial class TextBlockPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic TextBlock -->
    <TextBlock Text=""Hello, World!""
               FontSize=""14""
               Margin=""0,0,0,8""/>

    <!-- TextBlock with various font sizes -->
    <TextBlock Text=""Caption Text (12px)""
               FontSize=""12""
               Foreground=""#888888""
               Margin=""0,0,0,4""/>
    <TextBlock Text=""Body Text (14px)""
               FontSize=""14""
               Margin=""0,0,0,4""/>
    <TextBlock Text=""Subtitle (18px)""
               FontSize=""18""
               FontWeight=""SemiBold""
               Margin=""0,0,0,4""/>
    <TextBlock Text=""Title (24px)""
               FontSize=""24""
               FontWeight=""Bold""
               Margin=""0,0,0,4""/>
    <TextBlock Text=""Display (32px)""
               FontSize=""32""
               FontWeight=""Bold""
               Margin=""0,0,0,16""/>

    <!-- Text Wrapping -->
    <TextBlock Text=""This is a long paragraph that demonstrates text wrapping. When the text exceeds the available width, it will automatically wrap to the next line.""
               TextWrapping=""Wrap""
               MaxWidth=""400""
               Margin=""0,0,0,16""/>

    <!-- Text Trimming -->
    <TextBlock Text=""This text will be trimmed with ellipsis if it exceeds the width""
               TextTrimming=""CharacterEllipsis""
               MaxWidth=""300""
               Margin=""0,0,0,16""/>

    <!-- TextBlock with foreground colors -->
    <TextBlock Text=""Error: Something went wrong""
               Foreground=""#FF4444""
               FontWeight=""SemiBold""
               Margin=""0,0,0,4""/>
    <TextBlock Text=""Warning: Check your input""
               Foreground=""#FFB900""
               Margin=""0,0,0,4""/>
    <TextBlock Text=""Success: Operation complete""
               Foreground=""#10893E""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class TextBlockSample : Page
{
    public TextBlockSample()
    {
        InitializeComponent();
        CreateTextBlocks();
    }

    private void CreateTextBlocks()
    {
        // Basic TextBlock
        var title = new TextBlock
        {
            Text = ""Welcome"",
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Colors.White)
        };

        // TextBlock with wrapping
        var description = new TextBlock
        {
            Text = ""This is a description that wraps."",
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 400,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA))
        };

        // TextBlock with trimming
        var trimmed = new TextBlock
        {
            Text = ""Very long text that gets trimmed..."",
            TextTrimming = TextTrimming.CharacterEllipsis,
            MaxWidth = 200
        };

        // Update text dynamically
        title.Text = ""Updated Title"";

        // Change foreground at runtime
        title.Foreground = new SolidColorBrush(
            Color.FromRgb(0x00, 0x78, 0xD4));

        // Set text alignment
        title.TextAlignment = TextAlignment.Center;

        ContentPanel.Children.Add(title);
        ContentPanel.Children.Add(description);
        ContentPanel.Children.Add(trimmed);
    }
}";

    public TextBlockPage()
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
