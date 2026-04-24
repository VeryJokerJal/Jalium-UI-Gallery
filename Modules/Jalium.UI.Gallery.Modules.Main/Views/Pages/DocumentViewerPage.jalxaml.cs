using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for DocumentViewerPage.jalxaml demonstrating the DocumentViewer control.
/// </summary>
public partial class DocumentViewerPage : Page
{
    public DocumentViewerPage()
    {
        InitializeComponent();

        // Page navigation
        if (FirstPageButton != null)
            FirstPageButton.Click += (s, e) => DemoViewer?.FirstPage();
        if (PrevPageButton != null)
            PrevPageButton.Click += (s, e) => DemoViewer?.PreviousPage();
        if (NextPageButton != null)
            NextPageButton.Click += (s, e) => DemoViewer?.NextPage();
        if (LastPageButton != null)
            LastPageButton.Click += (s, e) => DemoViewer?.LastPage();

        // Zoom
        if (ZoomInButton != null)
            ZoomInButton.Click += (s, e) => DemoViewer?.ZoomIn();
        if (ZoomOutButton != null)
            ZoomOutButton.Click += (s, e) => DemoViewer?.ZoomOut();
        if (FitWidthButton != null)
            FitWidthButton.Click += (s, e) => DemoViewer?.FitToWidth();
        if (FitPageButton != null)
            FitPageButton.Click += (s, e) => DemoViewer?.FitToPage();

        // Print
        if (PrintButton != null)
            PrintButton.Click += (s, e) => DemoViewer?.Print();

        // Search
        if (FindButton != null)
            FindButton.Click += OnFindClick;
        if (FindNextButton != null)
            FindNextButton.Click += (s, e) => DemoViewer?.FindNext();
        if (FindPrevButton != null)
            FindPrevButton.Click += (s, e) => DemoViewer?.FindPrevious();
        if (ClearSearchButton != null)
            ClearSearchButton.Click += (s, e) =>
            {
                DemoViewer?.ClearSearch();
                if (SearchResultText != null)
                    SearchResultText.Text = "Search cleared.";
            };

        // Display options
        if (PageDisplayComboBox != null)
            PageDisplayComboBox.SelectionChanged += OnPageDisplayChanged;
        if (ZoomSlider != null)
            ZoomSlider.ValueChanged += OnZoomSliderChanged;
        if (ShowBordersCheckBox != null)
        {
            ShowBordersCheckBox.Checked += (s, e) =>
            {
                if (DemoViewer != null) DemoViewer.ShowPageBorders = true;
            };
            ShowBordersCheckBox.Unchecked += (s, e) =>
            {
                if (DemoViewer != null) DemoViewer.ShowPageBorders = false;
            };
        }
        if (SpacingSlider != null)
            SpacingSlider.ValueChanged += OnSpacingChanged;

        // DocumentViewer events
        if (DemoViewer != null)
        {
            DemoViewer.PageChanged += OnPageChanged;
            DemoViewer.SearchCompleted += OnSearchCompleted;
        }

        UpdatePageInfo();
        LoadCodeExamples();
    }

    private void OnFindClick(object? sender, EventArgs e)
    {
        var text = SearchBox?.Text;
        if (string.IsNullOrEmpty(text) || DemoViewer == null) return;

        DemoViewer.Find(
            text,
            matchCase: MatchCaseCheckBox?.IsChecked ?? false,
            matchWholeWord: WholeWordCheckBox?.IsChecked ?? false);
    }

    private void OnPageChanged(object? sender, PageChangedEventArgs e)
    {
        UpdatePageInfo();
    }

    private void OnSearchCompleted(object? sender, SearchCompletedEventArgs e)
    {
        if (SearchResultText != null)
            SearchResultText.Text = e.ResultCount > 0
                ? $"Found {e.ResultCount} result(s)."
                : "No results found.";
    }

    private void OnPageDisplayChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DemoViewer == null || PageDisplayComboBox == null) return;

        DemoViewer.PageDisplay = PageDisplayComboBox.SelectedIndex switch
        {
            0 => DocumentViewerPageDisplay.OnePage,
            1 => DocumentViewerPageDisplay.TwoPages,
            2 => DocumentViewerPageDisplay.TwoUpFacing,
            3 => DocumentViewerPageDisplay.Continuous,
            4 => DocumentViewerPageDisplay.ContinuousFacing,
            _ => DocumentViewerPageDisplay.OnePage
        };
    }

    private void OnZoomSliderChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DemoViewer != null)
            DemoViewer.SetZoom(e.NewValue);
        if (ZoomSliderText != null)
            ZoomSliderText.Text = $"{e.NewValue:F0}%";
        if (ZoomText != null)
            ZoomText.Text = $"{e.NewValue:F0}%";
    }

    private void OnSpacingChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DemoViewer != null)
        {
            DemoViewer.VerticalPageSpacing = e.NewValue;
            DemoViewer.HorizontalPageSpacing = e.NewValue;
        }
        if (SpacingText != null)
            SpacingText.Text = $"{e.NewValue:F0}";
    }

    private void UpdatePageInfo()
    {
        if (PageInfoText != null && DemoViewer != null)
            PageInfoText.Text = $"Page {DemoViewer.CurrentPage} / {DemoViewer.PageCount}";
    }

    private const string XamlExample = @"<!-- DocumentViewer with default settings -->
<DocumentViewer x:Name=""Viewer""
                Zoom=""100""
                FitMode=""FitWidth""
                PageDisplay=""OnePage""
                ShowPageBorders=""True""
                HorizontalPageSpacing=""10""
                VerticalPageSpacing=""10""/>

<!-- Zoom range constraints -->
<DocumentViewer MinZoom=""25"" MaxZoom=""400""/>";

    private const string CSharpExample = @"// Page navigation
viewer.FirstPage();
viewer.PreviousPage();
viewer.NextPage();
viewer.LastPage();
viewer.GoToPage(5);

// Zoom control
viewer.ZoomIn();
viewer.ZoomOut();
viewer.SetZoom(150); // 150%
viewer.FitToWidth();
viewer.FitToPage();

// Search within document
viewer.Find(""search text"", matchCase: true, matchWholeWord: false);
viewer.FindNext();
viewer.FindPrevious();
viewer.ClearSearch();

// Handle events
viewer.PageChanged += (s, e) =>
    Debug.WriteLine($""Page {e.OldPageNumber} -> {e.NewPageNumber}"");

viewer.SearchCompleted += (s, e) =>
    Debug.WriteLine($""Found {e.ResultCount} results"");

// Page display modes
viewer.PageDisplay = DocumentViewerPageDisplay.Continuous;
viewer.PageDisplay = DocumentViewerPageDisplay.TwoUpFacing;

// Fit modes
viewer.FitMode = DocumentViewerFitMode.FitWidth;
viewer.FitMode = DocumentViewerFitMode.FitPage;

// Read-only status
int current = viewer.CurrentPage;
int total = viewer.PageCount;
bool canPrev = viewer.CanGoToPreviousPage;
bool canNext = viewer.CanGoToNextPage;

// Print the document
viewer.Print();";

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }
}
