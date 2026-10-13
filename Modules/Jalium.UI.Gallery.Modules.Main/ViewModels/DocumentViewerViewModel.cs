using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class DocumentViewerViewModel
    : IDocumentViewerStateSink
{
    private long _requestSequence;
    private DocumentViewerRequest? _viewerRequest;
    private string _searchText = string.Empty;
    private bool _matchCase;
    private bool _matchWholeWord;
    private string _pageInfo = "Page 1 / 1";
    private string _searchResult =
        "Enter text and click Find to search.";
    private double _zoom = 100;
    private double _pageSpacing = 10;
    private bool _showPageBorders = true;
    private DocumentViewerPageDisplay _pageDisplay =
        DocumentViewerPageDisplay.OnePage;

    public DocumentViewerViewModel()
    {
        PageDisplayOptions =
            Enum.GetValues<DocumentViewerPageDisplay>();
        FirstPageCommand = Action(DocumentViewerAction.FirstPage);
        PreviousPageCommand = Action(DocumentViewerAction.PreviousPage);
        NextPageCommand = Action(DocumentViewerAction.NextPage);
        LastPageCommand = Action(DocumentViewerAction.LastPage);
        ZoomInCommand = Action(DocumentViewerAction.ZoomIn);
        ZoomOutCommand = Action(DocumentViewerAction.ZoomOut);
        FitWidthCommand = Action(DocumentViewerAction.FitWidth);
        FitPageCommand = Action(DocumentViewerAction.FitPage);
        PrintCommand = Action(DocumentViewerAction.Print);
        FindCommand = new RelayCommand(Find);
        FindNextCommand = Action(DocumentViewerAction.FindNext);
        FindPreviousCommand = Action(DocumentViewerAction.FindPrevious);
        ClearSearchCommand = Action(DocumentViewerAction.ClearSearch);
    }

    public IReadOnlyList<DocumentViewerPageDisplay> PageDisplayOptions
    {
        get;
    }

    public ICommand FirstPageCommand { get; }

    public ICommand PreviousPageCommand { get; }

    public ICommand NextPageCommand { get; }

    public ICommand LastPageCommand { get; }

    public ICommand ZoomInCommand { get; }

    public ICommand ZoomOutCommand { get; }

    public ICommand FitWidthCommand { get; }

    public ICommand FitPageCommand { get; }

    public ICommand PrintCommand { get; }

    public ICommand FindCommand { get; }

    public ICommand FindNextCommand { get; }

    public ICommand FindPreviousCommand { get; }

    public ICommand ClearSearchCommand { get; }

    public DocumentViewerRequest? ViewerRequest
    {
        get => _viewerRequest;
        private set => SetProperty(ref _viewerRequest, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool MatchCase
    {
        get => _matchCase;
        set => SetProperty(ref _matchCase, value);
    }

    public bool MatchWholeWord
    {
        get => _matchWholeWord;
        set => SetProperty(ref _matchWholeWord, value);
    }

    public string PageInfo
    {
        get => _pageInfo;
        private set => SetProperty(ref _pageInfo, value);
    }

    public string SearchResult
    {
        get => _searchResult;
        private set => SetProperty(ref _searchResult, value);
    }

    public double Zoom
    {
        get => _zoom;
        set => SetProperty(ref _zoom, value);
    }

    public double PageSpacing
    {
        get => _pageSpacing;
        set => SetProperty(ref _pageSpacing, value);
    }

    public bool ShowPageBorders
    {
        get => _showPageBorders;
        set => SetProperty(ref _showPageBorders, value);
    }

    public DocumentViewerPageDisplay PageDisplay
    {
        get => _pageDisplay;
        set => SetProperty(ref _pageDisplay, value);
    }

    public void UpdatePageState(
        int currentPage,
        int pageCount,
        double zoom)
    {
        PageInfo = $"Page {currentPage} / {pageCount}";
        Zoom = zoom;
    }

    public void UpdateSearchResult(int resultCount) =>
        SearchResult = resultCount > 0
            ? $"Found {resultCount} result(s)."
            : "No results found.";

    public void ClearSearchResult() =>
        SearchResult = "Search cleared.";

    private ICommand Action(DocumentViewerAction action) =>
        new RelayCommand(() => Request(action));

    private void Find() =>
        ViewerRequest = new DocumentViewerRequest(
            ++_requestSequence,
            DocumentViewerAction.Find,
            SearchText,
            MatchCase,
            MatchWholeWord);

    private void Request(DocumentViewerAction action) =>
        ViewerRequest = new DocumentViewerRequest(
            ++_requestSequence,
            action);
}
