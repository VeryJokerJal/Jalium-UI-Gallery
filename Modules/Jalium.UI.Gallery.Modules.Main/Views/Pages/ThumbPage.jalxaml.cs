using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for ThumbPage.jalxaml demonstrating Thumb functionality.
/// </summary>
public partial class ThumbPage : Page
{
    private const string XamlExample =
@"<!-- Thumb inside a Canvas track -->
<Border Background=""#1E1E1E"" Height=""40"" CornerRadius=""4"">
    <Canvas>
        <Thumb x:Name=""DemoThumb""
               Width=""20"" Height=""36""
               Background=""#0078D4""
               DragStarted=""OnThumbDragStarted""
               DragDelta=""OnThumbDragDelta""
               DragCompleted=""OnThumbDragCompleted"" />
    </Canvas>
</Border>

<!-- Thumb used inside a custom Slider template -->
<Slider>
    <Slider.Template>
        <ControlTemplate TargetType=""Slider"">
            <Grid>
                <Border Background=""#3D3D3D""
                        Height=""4""
                        CornerRadius=""2""
                        VerticalAlignment=""Center"" />
                <Track x:Name=""PART_Track"">
                    <Track.Thumb>
                        <Thumb Width=""16"" Height=""16""
                               Background=""#0078D4"" />
                    </Track.Thumb>
                </Track>
            </Grid>
        </ControlTemplate>
    </Slider.Template>
</Slider>";

    private const string CSharpExample =
@"using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

public partial class ThumbDemoPage : Page
{
    private double _thumbLeft;
    private double _thumbTop = 2;

    public ThumbDemoPage()
    {
        InitializeComponent();

        if (DemoThumb != null)
        {
            Canvas.SetLeft(DemoThumb, _thumbLeft);
            Canvas.SetTop(DemoThumb, _thumbTop);

            DemoThumb.DragDelta += OnThumbDragDelta;
            DemoThumb.DragStarted += OnThumbDragStarted;
            DemoThumb.DragCompleted += OnThumbDragCompleted;
        }
    }

    private void OnThumbDragStarted(object sender,
        DragStartedEventArgs e)
    {
        StatusText.Text = ""Dragging..."";
    }

    private void OnThumbDragDelta(object sender,
        DragDeltaEventArgs e)
    {
        var parent = DemoThumb.VisualParent as Canvas;
        var maxX = (parent?.RenderSize.Width ?? 400)
                   - DemoThumb.Width;
        _thumbLeft = Math.Clamp(
            _thumbLeft + e.HorizontalChange, 0, maxX);
        Canvas.SetLeft(DemoThumb, _thumbLeft);
        PositionText.Text = $""X: {_thumbLeft:F0}"";
    }

    private void OnThumbDragCompleted(object sender,
        DragCompletedEventArgs e)
    {
        StatusText.Text = $""Dropped at X={_thumbLeft:F0}"";
    }
}";

    private double _thumbLeft;
    private double _thumbTop = 2; // Center vertically in the 40px track

    public ThumbPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        if (DemoThumb != null)
        {
            Canvas.SetLeft(DemoThumb, _thumbLeft);
            Canvas.SetTop(DemoThumb, _thumbTop);

            DemoThumb.DragDelta += OnThumbDragDelta;
            DemoThumb.DragStarted += OnThumbDragStarted;
            DemoThumb.DragCompleted += OnThumbDragCompleted;
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

    private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
    {
        UpdatePositionText();
    }

    private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
    {
        if (DemoThumb == null) return;

        // Get the Canvas parent to clamp bounds
        var parent = DemoThumb.VisualParent as Canvas;
        var maxX = (parent?.RenderSize.Width ?? 400) - DemoThumb.Width;

        _thumbLeft = Math.Clamp(_thumbLeft + e.HorizontalChange, 0, Math.Max(0, maxX));
        Canvas.SetLeft(DemoThumb, _thumbLeft);

        UpdatePositionText();
    }

    private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
    {
        UpdatePositionText();
    }

    private void UpdatePositionText()
    {
        if (ThumbPositionText != null)
        {
            ThumbPositionText.Text = $"Position: {_thumbLeft:F0}, {_thumbTop:F0}";
        }
    }
}
