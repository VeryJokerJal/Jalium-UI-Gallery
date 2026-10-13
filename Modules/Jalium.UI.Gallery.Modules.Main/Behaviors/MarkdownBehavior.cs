using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class MarkdownBehavior
{
    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(MarkdownRequest),
            typeof(MarkdownBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static MarkdownRequest? GetRequest(DependencyObject element) =>
        element.GetValue(RequestProperty) as MarkdownRequest;

    public static void SetRequest(
        DependencyObject element,
        MarkdownRequest? value) =>
        element.SetValue(RequestProperty, value);

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Markdown markdown ||
            args.NewValue is not MarkdownRequest request)
        {
            return;
        }

        var sink = markdown.DataContext as IMarkdownStateSink;
        switch (request.Action)
        {
            case MarkdownAction.CopyPlainText:
                markdown.CopyAsPlainText();
                sink?.ReportCopy(
                    "Copied plain text",
                    markdown.SelectedText,
                    markdown.GetPlainText());
                break;
            case MarkdownAction.CopyMarkdown:
                markdown.CopyAsMarkdownText();
                sink?.ReportCopy(
                    "Copied Markdown source",
                    markdown.SelectedText,
                    markdown.GetMarkdownText());
                break;
            case MarkdownAction.CopyRichText:
                markdown.CopyAsRichText();
                sink?.ReportCopy(
                    "Copied rich text (HTML + RTF)",
                    markdown.SelectedText,
                    markdown.GetPlainText());
                break;
            case MarkdownAction.SelectAll:
                markdown.SelectAll();
                sink?.ReportSelectAll();
                break;
        }
    }
}
