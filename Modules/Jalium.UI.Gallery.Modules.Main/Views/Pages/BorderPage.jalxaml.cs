using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class BorderPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic Border with background -->
    <Border Background=""#0078D4""
            Width=""200"" Height=""80""
            CornerRadius=""8""
            Margin=""0,0,0,16"">
        <TextBlock Text=""Background Only""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </Border>

    <!-- Border with stroke and no fill -->
    <Border BorderBrush=""#0078D4""
            BorderThickness=""2""
            Width=""200"" Height=""80""
            CornerRadius=""8""
            Margin=""0,0,0,16"">
        <TextBlock Text=""Border Only""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </Border>

    <!-- Card-style Border with both -->
    <Border Background=""#2D2D2D""
            BorderBrush=""#3D3D3D""
            BorderThickness=""1""
            CornerRadius=""12""
            Padding=""20""
            Width=""300""
            Margin=""0,0,0,16"">
        <StackPanel Orientation=""Vertical"">
            <TextBlock Text=""Card Title"" FontSize=""16"" FontWeight=""Bold"" Foreground=""#FFFFFF""/>
            <TextBlock Text=""This is a card-style border with padding, corner radius, and both background and border brush.""
                       Foreground=""#AAAAAA"" TextWrapping=""Wrap"" Margin=""0,8,0,0""/>
        </StackPanel>
    </Border>

    <!-- Non-uniform Border Thickness -->
    <Border BorderBrush=""#881798""
            BorderThickness=""0,0,0,3""
            Padding=""12,8""
            Width=""200""
            Margin=""0,0,0,16"">
        <TextBlock Text=""Bottom border only""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""/>
    </Border>

    <!-- Non-uniform Corner Radius -->
    <Border Background=""#107C10""
            CornerRadius=""16,16,0,0""
            Width=""200"" Height=""80""
            Padding=""12"">
        <TextBlock Text=""Top corners rounded""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </Border>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class BorderDemo : Page
{
    public BorderDemo()
    {
        InitializeComponent();
        CreateBorders();
    }

    private void CreateBorders()
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(16) };

        // Simple background border
        var simpleBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            Width = 200,
            Height = 80,
            CornerRadius = new CornerRadius(8),
            Child = new TextBlock
            {
                Text = ""Simple Border"",
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        panel.Children.Add(simpleBorder);

        // Card with non-uniform corner radius
        var card = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 16, 0, 0),
            Width = 300
        };

        var cardContent = new StackPanel { Orientation = Orientation.Vertical };
        cardContent.Children.Add(new TextBlock
        {
            Text = ""Card Title"",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.White)
        });
        cardContent.Children.Add(new TextBlock
        {
            Text = ""Card description with wrapping text."",
            Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 8, 0, 0)
        });
        card.Child = cardContent;
        panel.Children.Add(card);

        // Non-uniform border thickness (underline effect)
        var underlined = new Border
        {
            BorderBrush = new SolidColorBrush(Color.FromRgb(136, 23, 152)),
            BorderThickness = new Thickness(0, 0, 0, 3),
            Padding = new Thickness(12, 8, 12, 8),
            Margin = new Thickness(0, 16, 0, 0),
            Child = new TextBlock
            {
                Text = ""Underline style"",
                Foreground = new SolidColorBrush(Color.White)
            }
        };
        panel.Children.Add(underlined);

        ContentArea.Child = panel;
    }
}";

    public BorderPage()
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
