using Jalium.UI.Ink;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum InkCanvasAction
{
    Clear
}

public sealed record InkCanvasDrawingSettings(
    Color StrokeColor,
    double StrokeWidth,
    bool IsHighlighter,
    BrushType BrushType,
    StrokeTaperMode TaperMode);

public sealed record InkCanvasRequest(
    InkCanvasAction Action,
    long Sequence);

public enum EditorAction
{
    Undo,
    Redo,
    SelectAll
}

public sealed record EditorRequest(
    EditorAction Action,
    long Sequence);

public enum TreeDataGridAction
{
    ExpandAll,
    CollapseAll
}

public sealed record TreeDataGridRequest(
    TreeDataGridAction Action,
    long Sequence);

public enum TextAnimationEffect
{
    RiseSettle,
    FadeIn,
    Typewriter,
    WaveBounce,
    PulsingBlur
}

public enum TextGpuEffect
{
    None,
    Blur4,
    Blur10
}

public enum ColorMatrixPreset
{
    None,
    Saturation,
    Grayscale,
    Sepia,
    Invert,
    HueRotate
}
