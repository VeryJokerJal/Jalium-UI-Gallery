namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum TaskbarProgressMode
{
    None,
    Normal,
    Paused,
    Error
}

public sealed record SystemShellSnapshot(
    bool IsGlassEnabled,
    string GlassColor,
    double CaptionHeight,
    string ResizeBorder);
