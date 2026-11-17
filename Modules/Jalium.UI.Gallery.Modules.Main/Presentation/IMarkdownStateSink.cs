namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface IMarkdownStateSink
{
    void ReportCopy(
        string action,
        string? selectedText,
        string? wholeDocument);

    void ReportSelectAll();
}
