using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for AppBarSeparatorPage.jalxaml demonstrating AppBarSeparator functionality.
/// </summary>
public partial class AppBarSeparatorPage : Page
{
    public AppBarSeparatorPage()
    {
        InitializeComponent();
        SetupIcons();
        SetupEventHandlers();
        LoadCodeExamples();
    }

    private void SetupIcons()
    {
        SetButtonIcon(NewButton, "\uD83D\uDCC4", "New");       // 📄
        SetButtonIcon(OpenButton, "\uD83D\uDCC2", "Open");     // 📂
        SetButtonIcon(SaveButton, "\uD83D\uDCBE", "Save");     // 💾
        SetButtonIcon(CutButton, "\u2702", "Cut");              // ✂
        SetButtonIcon(CopyButton, "\uD83D\uDCCB", "Copy");     // 📋
        SetButtonIcon(PasteButton, "\uD83D\uDCCB", "Paste");   // 📋
        SetButtonIcon(UndoButton, "\u21B6", "Undo");            // ↶
        SetButtonIcon(RedoButton, "\u21B7", "Redo");            // ↷
    }

    private static void SetButtonIcon(AppBarButton? button, string iconText, string tooltip)
    {
        if (button == null) return;

        button.Content = new TextBlock
        {
            Text = iconText,
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        button.ToolTip = tooltip;
    }

    private void SetupEventHandlers()
    {
        SetupClickHandler(NewButton, "New file created.");
        SetupClickHandler(OpenButton, "Open file dialog triggered.");
        SetupClickHandler(SaveButton, "File saved.");
        SetupClickHandler(CutButton, "Selection cut to clipboard.");
        SetupClickHandler(CopyButton, "Selection copied to clipboard.");
        SetupClickHandler(PasteButton, "Clipboard content pasted.");
        SetupClickHandler(UndoButton, "Last action undone.");
        SetupClickHandler(RedoButton, "Last undo redone.");
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

    private const string XamlExample = @"<!-- Toolbar with separators between groups -->
<StackPanel Orientation=""Horizontal"">
    <AppBarButton Label=""New"" IsCompact=""True"" />
    <AppBarButton Label=""Open"" IsCompact=""True"" />
    <AppBarButton Label=""Save"" IsCompact=""True"" />

    <AppBarSeparator Margin=""4"" />

    <AppBarButton Label=""Cut"" IsCompact=""True"" />
    <AppBarButton Label=""Copy"" IsCompact=""True"" />
    <AppBarButton Label=""Paste"" IsCompact=""True"" />

    <AppBarSeparator Margin=""4"" />

    <AppBarButton Label=""Undo"" IsCompact=""True"" />
    <AppBarButton Label=""Redo"" IsCompact=""True"" />
</StackPanel>";

    private const string CSharpExample = @"// Create a toolbar with separators
var toolbar = new StackPanel { Orientation = Orientation.Horizontal };

// File operations group
toolbar.Children.Add(new AppBarButton { Label = ""New"" });
toolbar.Children.Add(new AppBarButton { Label = ""Open"" });
toolbar.Children.Add(new AppBarButton { Label = ""Save"" });

// Add separator between groups
toolbar.Children.Add(new AppBarSeparator
{
    Margin = new Thickness(4)
});

// Edit operations group
toolbar.Children.Add(new AppBarButton { Label = ""Cut"" });
toolbar.Children.Add(new AppBarButton { Label = ""Copy"" });
toolbar.Children.Add(new AppBarButton { Label = ""Paste"" });";

    private void SetupClickHandler(AppBarButton? button, string message)
    {
        if (button != null)
            button.Click += (_, _) =>
            {
                if (ToolbarStatusText != null)
                    ToolbarStatusText.Text = message;
            };
    }
}
