using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Documents;
using Jalium.UI.Media;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class DocumentViewerViewModel
    : IDocumentViewerStateSink
{
    private readonly Dictionary<DocumentViewerAction, RelayCommand> _commands = new();
    private int _currentPage;
    private int _pageCount;
    private int _searchCount;
    private bool _hasSearched;
    private static readonly double MinimumZoom = (double)DocumentViewer.MinZoomProperty.GetMetadata(typeof(DocumentViewer)).DefaultValue!;
    private static readonly double MaximumZoom = (double)DocumentViewer.MaxZoomProperty.GetMetadata(typeof(DocumentViewer)).DefaultValue!;
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

    private DocumentViewerViewModel? _previewState;
    public DocumentViewerViewModel() : this(CreateSampleDocument()) { }

    private DocumentViewerViewModel(FlowDocument document)
    {
        SampleDocument = document;
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
        FindCommand = new RelayCommand(Find, CanFind);
        FindNextCommand = Action(DocumentViewerAction.FindNext);
        FindPreviousCommand = Action(DocumentViewerAction.FindPrevious);
        ClearSearchCommand = Action(DocumentViewerAction.ClearSearch);
    }

    /// <summary>Shared semantic document for the demo and its independent preview.</summary>
    public FlowDocument SampleDocument { get; }

    /// <summary>An independent presentation state sharing the semantic document.</summary>
    public DocumentViewerViewModel PreviewState => _previewState ??= new DocumentViewerViewModel(SampleDocument);

    private static FlowDocument CreateSampleDocument()
    {
        var document = FlowDocument.FromText(string.Join("\n\n", Enumerable.Range(1, 12).Select(index =>
            $"Jalium UI · Document {index}\nA document viewer keeps content, pagination and navigation together. " +
            "Use the toolbar to change pages, fit the document or zoom into the text.\n" +
            "文档示例支持分页、缩放和搜索。Search for document to find the original text across pages.")));
        document.PageWidth = 560;
        document.PageHeight = 720;
        document.PagePadding = new Thickness(28);
        document.FontSize = 16;
        document.Background = Brushes.White;
        document.Foreground = Brushes.Black;
        return document;
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
        set { if (SetProperty(ref _searchText, value)) ((RelayCommand)FindCommand).NotifyCanExecuteChanged(); }
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
        set { if (SetProperty(ref _zoom, value)) NotifyCommands(); }
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

    public void UpdatePageState(int currentPage, int pageCount)
    {
        _currentPage = currentPage;
        _pageCount = pageCount;
        PageInfo = $"Page {currentPage} / {pageCount}";
        if (pageCount == 0)
        {
            _searchCount = 0;
            _hasSearched = false;
            SearchResult = "Enter text and click Find to search.";
        }
        NotifyCommands();
    }

    public void UpdateSearchResult(int resultCount)
    {
        _hasSearched = true;
        _searchCount = Math.Max(0, resultCount);
        SearchResult = resultCount > 0 ? $"Found {resultCount} result(s)." : "No results found.";
        NotifyCommands();
    }

    public void ClearSearchResult()
    {
        _hasSearched = false;
        _searchCount = 0;
        SearchResult = "Search cleared.";
        NotifyCommands();
    }

    private ICommand Action(DocumentViewerAction action)
    {
        var command = new RelayCommand(() =>
        {
            if (CanRequest(action)) Request(action);
        }, () => CanRequest(action));
        _commands.Add(action, command);
        return command;
    }

    private bool CanFind() => _pageCount > 0 && !string.IsNullOrWhiteSpace(SearchText);

    private bool CanRequest(DocumentViewerAction action) => _pageCount > 0 && (action switch
    {
        DocumentViewerAction.FirstPage or DocumentViewerAction.PreviousPage => _currentPage > 1,
        DocumentViewerAction.NextPage or DocumentViewerAction.LastPage => _currentPage < _pageCount,
        DocumentViewerAction.ZoomIn => Zoom < MaximumZoom,
        DocumentViewerAction.ZoomOut => Zoom > MinimumZoom,
        DocumentViewerAction.FindNext or DocumentViewerAction.FindPrevious => _searchCount > 0,
        DocumentViewerAction.ClearSearch => _hasSearched,
        _ => true
    });

    private void NotifyCommands()
    {
        foreach (var command in _commands.Values) command.NotifyCanExecuteChanged();
        ((RelayCommand)FindCommand).NotifyCanExecuteChanged();
    }

    private void Find()
    {
        if (!CanFind()) return;
        ViewerRequest = new DocumentViewerRequest(++_requestSequence, DocumentViewerAction.Find,
            SearchText, MatchCase, MatchWholeWord);
    }

    private void Request(DocumentViewerAction action) =>
        ViewerRequest = new DocumentViewerRequest(++_requestSequence, action);
}
