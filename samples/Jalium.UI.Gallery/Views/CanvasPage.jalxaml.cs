using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

public partial class CanvasPage : Page
{
    private const string XamlExample = @"<!-- Canvas with absolute positioning -->
<Canvas Width=""400"" Height=""300"" Background=""#1E1E1E"">
    <!-- Position elements using Canvas.Left and Canvas.Top -->
    <Border Canvas.Left=""10"" Canvas.Top=""10""
            Width=""80"" Height=""60""
            Background=""#0078D4"" CornerRadius=""4"">
        <TextBlock Text=""Top-Left""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""
                   FontSize=""11""/>
    </Border>

    <Border Canvas.Left=""150"" Canvas.Top=""50""
            Width=""100"" Height=""80""
            Background=""#107C10"" CornerRadius=""4"">
        <TextBlock Text=""Center""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </Border>

    <!-- Use Canvas.Right and Canvas.Bottom for right/bottom anchoring -->
    <Border Canvas.Right=""20"" Canvas.Bottom=""20""
            Width=""90"" Height=""60""
            Background=""#881798"" CornerRadius=""4"">
        <TextBlock Text=""Bottom-Right""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""
                   FontSize=""11""/>
    </Border>

    <!-- ZIndex controls layering order -->
    <Border Canvas.Left=""100"" Canvas.Top=""100""
            Width=""120"" Height=""80""
            Background=""#CA5010"" CornerRadius=""4""
            Panel.ZIndex=""2"">
        <TextBlock Text=""ZIndex=2 (on top)""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""
                   FontSize=""11""/>
    </Border>

    <Border Canvas.Left=""140"" Canvas.Top=""120""
            Width=""120"" Height=""80""
            Background=""#008272"" CornerRadius=""4""
            Panel.ZIndex=""1"">
        <TextBlock Text=""ZIndex=1 (behind)""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""
                   FontSize=""11""/>
    </Border>
</Canvas>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class CanvasDemo : Page
{
    public CanvasDemo()
    {
        InitializeComponent();
        BuildCanvasLayout();
    }

    private void BuildCanvasLayout()
    {
        var canvas = new Canvas
        {
            Width = 400,
            Height = 300,
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30))
        };

        // Create and position elements programmatically
        var blueBox = CreateBox(""Top-Left"", Color.FromRgb(0, 120, 212), 80, 60);
        Canvas.SetLeft(blueBox, 10);
        Canvas.SetTop(blueBox, 10);
        canvas.Children.Add(blueBox);

        var greenBox = CreateBox(""Center"", Color.FromRgb(16, 124, 16), 100, 80);
        Canvas.SetLeft(greenBox, 150);
        Canvas.SetTop(greenBox, 50);
        canvas.Children.Add(greenBox);

        // Use Right/Bottom for anchoring from the opposite edges
        var purpleBox = CreateBox(""Bottom-Right"", Color.FromRgb(136, 23, 152), 90, 60);
        Canvas.SetRight(purpleBox, 20);
        Canvas.SetBottom(purpleBox, 20);
        canvas.Children.Add(purpleBox);

        // Control layering with ZIndex
        var orangeBox = CreateBox(""Front"", Color.FromRgb(202, 80, 16), 100, 70);
        Canvas.SetLeft(orangeBox, 100);
        Canvas.SetTop(orangeBox, 100);
        Panel.SetZIndex(orangeBox, 2);
        canvas.Children.Add(orangeBox);

        var tealBox = CreateBox(""Behind"", Color.FromRgb(0, 130, 114), 100, 70);
        Canvas.SetLeft(tealBox, 130);
        Canvas.SetTop(tealBox, 120);
        Panel.SetZIndex(tealBox, 1);
        canvas.Children.Add(tealBox);

        // Animate position changes
        AnimateElement(blueBox);

        ContentArea.Child = canvas;
    }

    private Border CreateBox(string text, Color color, double width, double height)
    {
        return new Border
        {
            Width = width,
            Height = height,
            Background = new SolidColorBrush(color),
            CornerRadius = new CornerRadius(4),
            Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12
            }
        };
    }

    private async void AnimateElement(UIElement element)
    {
        // Simple position animation example
        for (int i = 0; i < 100; i++)
        {
            Canvas.SetLeft(element, 10 + i * 2);
            await Task.Delay(16); // ~60fps
        }
    }
}";

    public CanvasPage()
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
