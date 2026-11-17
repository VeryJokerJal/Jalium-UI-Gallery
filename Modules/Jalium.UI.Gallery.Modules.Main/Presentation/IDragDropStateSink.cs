using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface IDragDropStateSink
{
    void OnDragEntered(
        DragDropTargetRole role,
        string targetName);

    void OnDragLeft(
        DragDropTargetRole role,
        string targetName);

    void OnTextDropped(
        DragDropTargetRole role,
        string targetName,
        string text);

    void OnFilesDropped(
        string targetName,
        IReadOnlyList<string> paths);

    string? CreateDragOutFile();

    void OnDragOutCompleted(string effect);
}
