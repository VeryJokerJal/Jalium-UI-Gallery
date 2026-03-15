using Jalium.UI.Controls;
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
