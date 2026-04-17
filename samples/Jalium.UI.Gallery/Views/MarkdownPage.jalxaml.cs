using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
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

    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic Markdown rendering -->
    <Markdown x:Name=""BasicMarkdown""
              Height=""300""
              Margin=""0,0,0,16""/>

    <!-- Markdown with BaseUri for resolving relative links -->
    <Markdown x:Name=""DocsMarkdown""
              Height=""400""
              BaseUri=""https://jalium.dev/docs/""
              Margin=""0,0,0,16""/>

    <!-- Styled Markdown with custom theme -->
    <Markdown x:Name=""StyledMarkdown""
              Height=""300""
              Background=""#0F172A""
              Foreground=""#E2E8F0""
              LinkForeground=""#7DD3FC""
              CodeBackground=""#162133""
              QuoteBackground=""#162133""
              QuoteBorderBrush=""#38BDF8""
              TableHeaderBackground=""#1E293B""
              TableBorderBrush=""#334155""/>

    <!-- Markdown with button-driven content switching -->
    <Border Background=""#1E1E1E"" CornerRadius=""4"" Padding=""16"">
        <StackPanel Orientation=""Vertical"">
            <WrapPanel Margin=""0,0,0,8"">
                <Button Content=""Docs"" Click=""OnDocsSampleClick"" Margin=""0,0,8,0""/>
                <Button Content=""Release Notes"" Click=""OnReleaseNotesClick"" Margin=""0,0,8,0""/>
                <Button Content=""Checklist"" Click=""OnChecklistClick""/>
            </WrapPanel>
            <Markdown x:Name=""InteractiveMarkdown"" Height=""360""/>
        </StackPanel>
    </Border>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class MarkdownDemoPage : Page
{
    public MarkdownDemoPage()
    {
        InitializeComponent();
        SetupMarkdown();
    }

    private void SetupMarkdown()
    {
        // Set markdown text directly
        BasicMarkdown.Text = ""# Hello World\n\nThis is **bold** and *italic* text."";

        // Configure BaseUri for relative link resolution
        DocsMarkdown.BaseUri = ""https://jalium.dev/docs/"";
        DocsMarkdown.Text = @""# API Reference

See the [controls guide](./controls) for more details.

| Property | Type | Description |
| --- | --- | --- |
| Text | string | Markdown source |
| BaseUri | string | Base for relative URLs |

```csharp
var md = new Markdown();
md.Text = ""# Hello"";
```"";

        // Apply custom styling for dark theme
        StyledMarkdown.Background = BrushFromHex(""#0F172A"");
        StyledMarkdown.Foreground = BrushFromHex(""#E2E8F0"");
        StyledMarkdown.LinkForeground = BrushFromHex(""#7DD3FC"");
        StyledMarkdown.CodeBackground = BrushFromHex(""#162133"");
        StyledMarkdown.QuoteBackground = BrushFromHex(""#162133"");
        StyledMarkdown.QuoteBorderBrush = BrushFromHex(""#38BDF8"");
        StyledMarkdown.TableHeaderBackground = BrushFromHex(""#1E293B"");
        StyledMarkdown.TableBorderBrush = BrushFromHex(""#334155"");
        StyledMarkdown.Text = ""# Styled Content\n\n> Custom themed markdown."";
    }

    private void OnDocsSampleClick(object sender, RoutedEventArgs e)
    {
        InteractiveMarkdown.Text = ""# Documentation\n\nMarkdown content here..."";
    }

    private static SolidColorBrush BrushFromHex(string hex)
    {
        var v = hex.TrimStart('#');
        return new SolidColorBrush(Color.FromRgb(
            Convert.ToByte(v.Substring(0, 2), 16),
            Convert.ToByte(v.Substring(2, 2), 16),
            Convert.ToByte(v.Substring(4, 2), 16)));
    }
}";

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
