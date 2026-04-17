using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for StatusBarPage.jalxaml demonstrating StatusBar functionality.
/// </summary>
public partial class StatusBarPage : Page
{
    private const string XamlExample =
@"<StatusBar Height=""28"" HorizontalAlignment=""Stretch"">
    <StatusBarItem Content=""Ready"" />
    <Separator />
    <StatusBarItem Content=""Line: 42"" />
    <Separator />
    <StatusBarItem Content=""Column: 15"" />
    <Separator />
    <StatusBarItem Content=""UTF-8"" />
    <Separator />
    <StatusBarItem>
        <StackPanel Orientation=""Horizontal"">
            <TextBlock Text=""Errors: "" />
            <TextBlock Text=""0"" Foreground=""#00CC00"" />
        </StackPanel>
    </StatusBarItem>
    <Separator />
    <StatusBarItem HorizontalAlignment=""Right"">
        <ProgressBar Width=""120"" Height=""14""
                     Value=""75"" Maximum=""100"" />
    </StatusBarItem>
</StatusBar>";

    private const string CSharpExample =
@"using Jalium.UI.Controls;

public partial class EditorWindow : Window
{
    public EditorWindow()
    {
        InitializeComponent();
        UpdateStatusBar();
    }

    private void UpdateStatusBar()
    {
        if (StatusText != null)
            StatusText.Content = ""Ready"";

        if (LineText != null)
            LineText.Content = $""Line: {_currentLine}"";

        if (ColumnText != null)
            ColumnText.Content = $""Col: {_currentColumn}"";

        if (EncodingText != null)
            EncodingText.Content = _encoding.WebName.ToUpper();
    }

    private void OnCaretPositionChanged(object? sender, EventArgs e)
    {
        _currentLine = editor.CaretLine;
        _currentColumn = editor.CaretColumn;
        UpdateStatusBar();
    }
}";

    public StatusBarPage()
    {
        InitializeComponent();
        LoadCodeExamples();
    }

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
