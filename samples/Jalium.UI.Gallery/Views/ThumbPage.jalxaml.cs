using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ThumbPage.jalxaml demonstrating Thumb functionality.
/// </summary>
public partial class ThumbPage : Page
{
    private double _thumbLeft;
    private double _thumbTop = 2; // Center vertically in the 40px track

    public ThumbPage()
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
