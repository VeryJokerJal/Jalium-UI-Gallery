namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface IInkCanvasStateSink
{
    void UpdateStrokeCount(int count);
}

public interface IEditControlStateSink
{
    void UpdateEditorPosition(
        int lineCount,
        int textLength,
        int line,
        int column);
}

public interface ITreeViewStateSink
{
    void UpdateTreeSelection(string? label);
}

public interface ITreeSelectorStateSink
{
    void UpdateTreeSelectorState(
        string key,
        string? label,
        int selectedCount,
        int checkedCount);
}
