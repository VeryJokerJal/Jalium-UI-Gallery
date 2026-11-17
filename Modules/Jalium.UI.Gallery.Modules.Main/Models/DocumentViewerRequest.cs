namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum DocumentViewerAction
{
    FirstPage,
    PreviousPage,
    NextPage,
    LastPage,
    ZoomIn,
    ZoomOut,
    FitWidth,
    FitPage,
    Print,
    Find,
    FindNext,
    FindPrevious,
    ClearSearch
}

public sealed record DocumentViewerRequest(
    long Sequence,
    DocumentViewerAction Action,
    string SearchText = "",
    bool MatchCase = false,
    bool MatchWholeWord = false);
