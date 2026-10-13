using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class MarkdownViewModel : IMarkdownStateSink
{
    private long _requestSequence;
    private string _interactiveText;
    private string _previewStatus = "Showing: Docs Sample";
    private string _liveText = LiveEditorSample;
    private string _copyStatus =
        "Select some text above, or use the buttons to copy the whole document.";
    private MarkdownRequest? _copyRequest;

    public MarkdownViewModel()
    {
        _interactiveText = DocsSample;
        SelectSampleCommand = new RelayCommand<string>(SelectSample);
        CopyPlainTextCommand = Copy(MarkdownAction.CopyPlainText);
        CopyMarkdownCommand = Copy(MarkdownAction.CopyMarkdown);
        CopyRichTextCommand = Copy(MarkdownAction.CopyRichText);
        SelectAllCommand = Copy(MarkdownAction.SelectAll);
    }

    public ICommand SelectSampleCommand { get; }

    public ICommand CopyPlainTextCommand { get; }

    public ICommand CopyMarkdownCommand { get; }

    public ICommand CopyRichTextCommand { get; }

    public ICommand SelectAllCommand { get; }

    public string InteractiveText
    {
        get => _interactiveText;
        private set => SetProperty(ref _interactiveText, value);
    }

    public string PreviewStatus
    {
        get => _previewStatus;
        private set => SetProperty(ref _previewStatus, value);
    }

    public string LiveText
    {
        get => _liveText;
        set => SetProperty(ref _liveText, value);
    }

    public string CopyStatus
    {
        get => _copyStatus;
        private set => SetProperty(ref _copyStatus, value);
    }

    public MarkdownRequest? CopyRequest
    {
        get => _copyRequest;
        private set => SetProperty(ref _copyRequest, value);
    }

    public string StyledText { get; } =
        """
        # Team Notes

        This block uses native Markdown styling properties to feel more like a documentation panel.

        > Markdown is useful when app content needs structure, not just plain text.

        | Scenario | Benefit |
        | --- | --- |
        | Release notes | Easy to author |
        | Help content | Link-friendly |
        | Status summaries | Readable formatting |

        ```config
        theme = dark
        accent = cyan
        ```
        """;

    public string CopyDemoText { get; } =
        GalleryShortcutText.Expand("""
        # Selection & Copy

        Select any of this text with the mouse, then press **{Shortcut.Copy}** or right-click for the copy menu. You can also copy the whole document with the buttons below.

        - Plain text drops every Markdown marker
        - Markdown keeps the `**markers**`
        - Rich text pastes formatted into Word, mail, and other apps

        > Use `GetPlainText()` to feed content to a translation service.
        """);

    public string MermaidText { get; } =
        """
        # Diagrams in Markdown

        Fenced **mermaid** code blocks render as diagrams automatically.

        ```mermaid
        flowchart LR
            A[Write doc] --> B{Has mermaid?}
            B -->|yes| C[Render diagram]
            B -->|no| D[Render code]
        ```

        Pie charts work the same way:

        ```mermaid
        pie title Build time
            "Compile" : 55
            "Link" : 25
            "Test" : 20
        ```
        """;

    public void ReportCopy(
        string action,
        string? selectedText,
        string? wholeDocument)
    {
        var hasSelection = !string.IsNullOrEmpty(selectedText);
        var preview = (hasSelection ? selectedText : wholeDocument) ??
                      string.Empty;
        preview = preview
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();
        if (preview.Length > 90)
        {
            preview = preview[..90] + "…";
        }

        CopyStatus =
            $"{action} ({(hasSelection ? "selection" : "whole document")}).  " +
            $"Preview: {preview}";
    }

    public void ReportSelectAll() =>
        CopyStatus =
            GalleryShortcutText.Expand(
                "Selected the whole document — press {Shortcut.Copy} or right-click to copy.");

    private ICommand Copy(MarkdownAction action) =>
        new RelayCommand(() =>
            CopyRequest = new MarkdownRequest(++_requestSequence, action));

    private void SelectSample(string? sample)
    {
        (var title, var text) = sample switch
        {
            "ReleaseNotes" => ("Release Notes", ReleaseNotesSample),
            "Checklist" => ("Checklist", ChecklistSample),
            _ => ("Docs Sample", DocsSample)
        };
        InteractiveText = text;
        PreviewStatus = $"Showing: {title}";
    }

    private const string DocsSample =
        """
        # Jalium.UI Markdown

        Render **rich documents** directly inside your app with the `Markdown` control.

        ## Highlights

        - Native Markdown parsing and rendering
        - Relative link support through `BaseUri`
        - Tables, task lists, and fenced code blocks

        ## Sample Table

        | Feature | Status |
        | --- | --- |
        | Tables | Ready |
        | Task lists | Ready |
        | Syntax fences | Ready |

        Read more in the [control reference](/controls/markdown).
        """;

    private const string ReleaseNotesSample =
        """
        # 26.7 Release Notes

        ### New Controls

        - `Markdown` for document-style content
        - `QRCode` for compact visual payload encoding

        ### Improvements

        1. Better theme integration for control styles
        2. More gallery coverage for recent additions
        3. XAML type registration updates for new controls

        > Keep control demos close to real workflows.
        """;

    private const string ChecklistSample =
        """
        # Launch Checklist

        - [x] Add the control to Jalium.UI
        - [x] Register the type for XAML parsing
        - [x] Add default theme resources
        - [x] Update the Gallery with a demo page
        - [ ] Write deeper scenario docs

        ## Next

        Use Markdown for headings, emphasis, tables, and links.
        """;

    private const string LiveEditorSample =
        """
        # Live Markdown Editor

        Type **Markdown** on the left and watch it render here.

        ## Try It Out

        - Edit any line to update the preview
        - Add *emphasis*, `inline code`, or [links](/controls/markdown)
        - Build tables and lists

        | Syntax | Result |
        | --- | --- |
        | `**text**` | bold |
        | `*text*` | italic |

        > The rendered output refreshes as you type.
        """;
}
