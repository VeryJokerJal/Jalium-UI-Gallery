namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface IDocumentViewerStateSink
{
    void UpdatePageState(int currentPage, int pageCount);

    void UpdateSearchResult(int resultCount);

    void ClearSearchResult();
}
