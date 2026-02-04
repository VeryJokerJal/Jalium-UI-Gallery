using Jalium.UI.Controls;
using Jalium.UI.Controls.Ink;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for InkCanvasPage.jalxaml demonstrating InkCanvas control functionality.
/// </summary>
public partial class InkCanvasPage : Page
{
    public InkCanvasPage()
    {
        InitializeComponent();
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        // Mode buttons
        if (InkModeButton != null)
            InkModeButton.Click += OnInkModeClick;

        if (EraseModeButton != null)
            EraseModeButton.Click += OnEraseModeClick;

        if (ClearButton != null)
            ClearButton.Click += OnClearClick;

        // Color buttons
        if (BlackColorButton != null)
            BlackColorButton.Click += (s, e) => SetColor(Color.FromRgb(0, 0, 0));

        if (RedColorButton != null)
            RedColorButton.Click += (s, e) => SetColor(Color.FromRgb(232, 17, 35));

        if (BlueColorButton != null)
            BlueColorButton.Click += (s, e) => SetColor(Color.FromRgb(0, 120, 212));

        if (GreenColorButton != null)
            GreenColorButton.Click += (s, e) => SetColor(Color.FromRgb(0, 204, 106));

        if (OrangeColorButton != null)
            OrangeColorButton.Click += (s, e) => SetColor(Color.FromRgb(247, 99, 12));

        if (PurpleColorButton != null)
            PurpleColorButton.Click += (s, e) => SetColor(Color.FromRgb(136, 108, 228));

        // Width slider
        if (WidthSlider != null)
            WidthSlider.ValueChanged += OnWidthSliderChanged;

        // Highlighter checkbox
        if (HighlighterCheckBox != null)
            HighlighterCheckBox.Checked += OnHighlighterChanged;

        if (HighlighterCheckBox != null)
            HighlighterCheckBox.Unchecked += OnHighlighterChanged;

        // Stroke collection events
        if (MainInkCanvas != null)
        {
            MainInkCanvas.StrokeCollected += OnStrokeCollected;
            MainInkCanvas.StrokesChanged += OnStrokesChanged;
        }

        // Brush type buttons
        if (RoundBrushButton != null)
            RoundBrushButton.Click += (s, e) => SetBrushType(BrushType.Round, "圆笔");

        if (CalligraphyBrushButton != null)
            CalligraphyBrushButton.Click += (s, e) => SetBrushType(BrushType.Calligraphy, "毛笔");

        if (PenBrushButton != null)
            PenBrushButton.Click += (s, e) => SetBrushType(BrushType.Pen, "书写笔");

        if (AirbrushButton != null)
            AirbrushButton.Click += (s, e) => SetBrushType(BrushType.Airbrush, "喷枪");

        if (OilBrushButton != null)
            OilBrushButton.Click += (s, e) => SetBrushType(BrushType.Oil, "油画笔");

        if (CrayonBrushButton != null)
            CrayonBrushButton.Click += (s, e) => SetBrushType(BrushType.Crayon, "蜡笔");

        if (MarkerBrushButton != null)
            MarkerBrushButton.Click += (s, e) => SetBrushType(BrushType.Marker, "记号笔");

        if (PencilBrushButton != null)
            PencilBrushButton.Click += (s, e) => SetBrushType(BrushType.Pencil, "铅笔");

        if (WatercolorBrushButton != null)
            WatercolorBrushButton.Click += (s, e) => SetBrushType(BrushType.Watercolor, "水彩笔");

        // Taper mode buttons
        if (AnimNoneButton != null)
            AnimNoneButton.Click += OnAnimNoneClick;

        if (AnimGrowInButton != null)
            AnimGrowInButton.Click += OnAnimGrowInClick;

        if (AnimShrinkOutButton != null)
            AnimShrinkOutButton.Click += OnAnimShrinkOutClick;
    }

    private void OnInkModeClick(object? sender, EventArgs e)
    {
        if (MainInkCanvas != null)
        {
            MainInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            UpdateStatus();
        }
    }

    private void OnEraseModeClick(object? sender, EventArgs e)
    {
        if (MainInkCanvas != null)
        {
            MainInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            UpdateStatus();
        }
    }

    private void OnClearClick(object? sender, EventArgs e)
    {
        if (MainInkCanvas != null)
        {
            MainInkCanvas.ClearStrokes();
            UpdateStrokeCount();
        }
    }

    private void SetColor(Color color)
    {
        if (MainInkCanvas != null)
        {
            MainInkCanvas.DefaultDrawingAttributes.Color = color;
        }
    }

    private void SetBrushType(BrushType brushType, string displayName)
    {
        if (MainInkCanvas != null)
        {
            MainInkCanvas.DefaultDrawingAttributes.BrushType = brushType;
            UpdateBrushTypeStatus(displayName);
        }
    }

    private void UpdateBrushTypeStatus(string brushTypeName)
    {
        if (BrushTypeStatusText != null)
        {
            BrushTypeStatusText.Text = $"Brush Type: {brushTypeName}";
        }
    }

    private void OnWidthSliderChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (MainInkCanvas != null && WidthText != null)
        {
            var width = Math.Round(e.NewValue);
            MainInkCanvas.DefaultDrawingAttributes.Width = width;
            MainInkCanvas.DefaultDrawingAttributes.Height = width;
            WidthText.Text = $"Width: {width}";
        }
    }

    private void OnHighlighterChanged(object? sender, RoutedEventArgs e)
    {
        if (MainInkCanvas != null && HighlighterCheckBox != null)
        {
            MainInkCanvas.DefaultDrawingAttributes.IsHighlighter = HighlighterCheckBox.IsChecked == true;
        }
    }

    private void OnStrokeCollected(object? sender, InkCanvasStrokeCollectedEventArgs e)
    {
        UpdateStrokeCount();
    }

    private void OnAnimNoneClick(object? sender, EventArgs e)
    {
        if (MainInkCanvas != null)
            MainInkCanvas.DefaultStrokeTaperMode = StrokeTaperMode.None;
        UpdateAnimationStatus();
    }

    private void OnAnimGrowInClick(object? sender, EventArgs e)
    {
        if (MainInkCanvas != null)
            MainInkCanvas.DefaultStrokeTaperMode = StrokeTaperMode.TaperedStart;
        UpdateAnimationStatus();
    }

    private void OnAnimShrinkOutClick(object? sender, EventArgs e)
    {
        if (MainInkCanvas != null)
            MainInkCanvas.DefaultStrokeTaperMode = StrokeTaperMode.TaperedEnd;
        UpdateAnimationStatus();
    }

    private void UpdateAnimationStatus()
    {
        if (AnimationStatusText != null && MainInkCanvas != null)
        {
            var modeName = MainInkCanvas.DefaultStrokeTaperMode switch
            {
                StrokeTaperMode.None => "Normal",
                StrokeTaperMode.TaperedStart => "Tapered Start",
                StrokeTaperMode.TaperedEnd => "Tapered End",
                _ => "Unknown"
            };
            AnimationStatusText.Text = $"Taper: {modeName}";
        }
    }

    private void OnStrokesChanged(object? sender, EventArgs e)
    {
        UpdateStrokeCount();
    }

    private void UpdateStatus()
    {
        if (StatusText != null && MainInkCanvas != null)
        {
            var modeName = MainInkCanvas.EditingMode switch
            {
                InkCanvasEditingMode.Ink => "Ink",
                InkCanvasEditingMode.EraseByStroke => "Erase",
                InkCanvasEditingMode.EraseByPoint => "Erase (Point)",
                InkCanvasEditingMode.Select => "Select",
                _ => "None"
            };
            StatusText.Text = $"Mode: {modeName}";
        }
    }

    private void UpdateStrokeCount()
    {
        if (StrokeCountText != null && MainInkCanvas != null)
        {
            StrokeCountText.Text = $"Strokes: {MainInkCanvas.Strokes.Count}";
        }
    }
}
