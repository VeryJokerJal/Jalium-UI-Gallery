using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ShapesPage.jalxaml demonstrating shape controls functionality.
/// </summary>
public partial class ShapesPage : Page
{
    private const string XamlExample = """
        <StackPanel Orientation="Horizontal">
            <!-- Basic Rectangle -->
            <Rectangle Width="100" Height="60"
                       Fill="#0078D4"
                       Margin="0,0,16,0"/>

            <!-- Rounded Rectangle -->
            <Rectangle Width="100" Height="60"
                       Fill="#F7630C"
                       RadiusX="10" RadiusY="10"
                       Margin="0,0,16,0"/>

            <!-- Circle -->
            <Ellipse Width="60" Height="60"
                     Fill="#00CC6A"
                     Margin="0,0,16,0"/>

            <!-- Line -->
            <Canvas Width="150" Height="60">
                <Line X1="0" Y1="10" X2="150" Y2="50"
                      Stroke="#E81123" StrokeThickness="3"/>
            </Canvas>

            <!-- Polygon (Star) -->
            <Polygon Points="30,0 36,20 60,20 42,32 50,55 30,42 10,55 18,32 0,20 24,20"
                     Fill="#FFB900"/>

            <!-- Path (Heart) -->
            <Path Data="M 50,30 C 50,0 0,0 0,30 C 0,60 50,80 50,80
                         C 50,80 100,60 100,30 C 100,0 50,0 50,30"
                  Fill="#E81123" Width="50" Height="50" Stretch="Uniform"/>
        </StackPanel>
        """;

    private const string CSharpExample = """
        using Jalium.UI.Controls;
        using Jalium.UI.Media;

        // Create shapes programmatically
        var rect = new Rectangle
        {
            Width = 100,
            Height = 60,
            Fill = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            RadiusX = 10,
            RadiusY = 10
        };

        var ellipse = new Ellipse
        {
            Width = 60,
            Height = 60,
            Fill = new SolidColorBrush(Color.FromRgb(0, 204, 106)),
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 2
        };

        var line = new Line
        {
            X1 = 0, Y1 = 0,
            X2 = 100, Y2 = 50,
            Stroke = new SolidColorBrush(Color.FromRgb(232, 17, 35)),
            StrokeThickness = 3
        };

        var polygon = new Polygon();
        polygon.Points.Add(new Point(30, 0));
        polygon.Points.Add(new Point(60, 60));
        polygon.Points.Add(new Point(0, 60));
        polygon.Fill = new SolidColorBrush(Color.FromRgb(255, 185, 0));
        """;

    public ShapesPage()
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
