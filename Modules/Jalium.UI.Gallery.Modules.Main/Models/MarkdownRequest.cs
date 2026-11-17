namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum MarkdownAction
{
    CopyPlainText,
    CopyMarkdown,
    CopyRichText,
    SelectAll
}

public sealed record MarkdownRequest(long Sequence, MarkdownAction Action);
