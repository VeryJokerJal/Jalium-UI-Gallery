namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface IDocumentViewerStateSink
{
    void UpdatePageState(int currentPage, int pageCount, double zoom);

    void UpdateSearchResult(int resultCount);

    void ClearSearchResult();
}
