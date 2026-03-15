using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class MarkdownPage : Page
{
    private const string DocsSample = """
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

```xaml
<Markdown BaseUri="https://jalium.dev/docs/">
## Hello from Markdown
</Markdown>
```

Read more in the [control reference](/controls/markdown).
""";

    private const string ReleaseNotesSample = """
# 26.7 Release Notes

### New Controls

- `Markdown` for document-style content
- `QRCode` for compact visual payload encoding

### Improvements

1. Better theme integration for control styles
2. More gallery coverage for recent additions
3. XAML type registration updates for new controls

```csharp
public sealed class MarkdownRelease
{
    public string Language { get; } = "csharp";
}
```

> Tip: keep control demos close to real workflows so API decisions stay grounded.
""";

    private const string ChecklistSample = """
# Launch Checklist

- [x] Add the control to Jalium.UI
- [x] Register the type for XAML parsing
- [x] Add default theme resources
- [x] Update the Gallery with a demo page
- [ ] Write deeper scenario docs

## Next

Use Markdown when you want:

- headings
- inline emphasis
- tables
- links to local or remote docs
""";

    public MarkdownPage()
    {
        InitializeComponent();

        if (StyledMarkdown != null)
        {
            StyledMarkdown.LinkForeground = BrushFromHex("#7DD3FC");
            StyledMarkdown.CodeBackground = BrushFromHex("#162133");
            StyledMarkdown.QuoteBackground = BrushFromHex("#162133");
            StyledMarkdown.QuoteBorderBrush = BrushFromHex("#38BDF8");
            StyledMarkdown.TableHeaderBackground = BrushFromHex("#1E293B");
            StyledMarkdown.TableBorderBrush = BrushFromHex("#334155");
            StyledMarkdown.Text = """
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
        }

        ShowSample("Docs Sample", DocsSample);
    }

    private void OnDocsSampleClick(object sender, RoutedEventArgs e)
    {
        ShowSample("Docs Sample", DocsSample);
    }

    private void OnReleaseNotesClick(object sender, RoutedEventArgs e)
    {
        ShowSample("Release Notes", ReleaseNotesSample);
    }

    private void OnChecklistClick(object sender, RoutedEventArgs e)
    {
        ShowSample("Checklist", ChecklistSample);
    }

    private void ShowSample(string title, string markdown)
    {
        if (InteractiveMarkdown != null)
        {
            InteractiveMarkdown.Text = markdown;
        }

        if (PreviewStatusText != null)
        {
            PreviewStatusText.Text = $"Showing: {title}";
        }
    }

    private static SolidColorBrush BrushFromHex(string hex)
    {
        var value = hex.TrimStart('#');
        if (value.Length != 6)
        {
            return new SolidColorBrush(Color.White);
        }

        return new SolidColorBrush(Color.FromRgb(
            Convert.ToByte(value.Substring(0, 2), 16),
            Convert.ToByte(value.Substring(2, 2), 16),
            Convert.ToByte(value.Substring(4, 2), 16)));
    }
}
