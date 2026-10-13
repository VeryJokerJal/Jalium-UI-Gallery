using Jalium.UI;
using Jalium.UI.Automation;
using Jalium.UI.Controls;
using Jalium.UI.Controls.TextEffects;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Headless;

internal static class HeadlessSemanticSnapshot
{
    private const int MaxEntries = 5000;
    private const int MaxTextLength = 4000;

    public static IReadOnlyList<HeadlessSemanticText> Capture(
        UIElement root,
        double dpiScale,
        int pixelWidth,
        int pixelHeight)
    {
        ArgumentNullException.ThrowIfNull(root);

        var result = new List<HeadlessSemanticText>();
        var pending = new Stack<DependencyObject>();
        var visited = new HashSet<DependencyObject>(ReferenceEqualityComparer.Instance);
        pending.Push(root);

        while (pending.Count > 0 && result.Count < MaxEntries)
        {
            var current = pending.Pop();
            if (!visited.Add(current))
            {
                continue;
            }

            if (current is UIElement { Visibility: Visibility.Visible } element &&
                element.Opacity > 0 &&
                TryGetBounds(element, root, dpiScale, pixelWidth, pixelHeight, out var bounds))
            {
                var values = GetTextValues(element);
                foreach (var (source, text) in values)
                {
                    result.Add(new HeadlessSemanticText(
                        Normalize(text),
                        source,
                        element.GetType().Name,
                        (element as FrameworkElement)?.Name.NullIfEmpty(),
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Height));
                    if (result.Count >= MaxEntries)
                    {
                        break;
                    }
                }
            }

            var childCount = VisualTreeHelper.GetChildrenCount(current);
            for (var index = childCount - 1; index >= 0; index--)
            {
                if (VisualTreeHelper.GetChild(current, index) is { } child)
                {
                    pending.Push(child);
                }
            }
        }

        return result;
    }

    private static IReadOnlyList<(string Source, string Text)> GetTextValues(UIElement element)
    {
        var values = new List<(string Source, string Text)>(3);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        void Add(string source, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && seen.Add(value))
            {
                values.Add((source, value));
            }
        }

        Add("automation-name", AutomationProperties.GetName(element));

        switch (element)
        {
            case TextBlock textBlock:
                Add("text", textBlock.Text);
                break;
            case AccessText accessText:
                Add("text", accessText.Text);
                break;
            case TextBox textBox:
                Add("text", textBox.Text);
                break;
            case AutoCompleteBox autoCompleteBox:
                Add("text", autoCompleteBox.Text);
                break;
            case ComboBox comboBox:
                Add("text", comboBox.Text);
                break;
            case EditControl editControl:
                Add("text", editControl.Text);
                break;
            case Markdown markdown:
                Add("text", markdown.Text);
                break;
            case TextEffectPresenter presenter:
                Add("text", presenter.Text);
                break;
        }

        if (element is HeaderedContentControl headered && headered.Header is string header)
        {
            Add("header", header);
        }

        if (element is ContentControl { Content: string content })
        {
            Add("content", content);
        }

        return values;
    }

    private static bool TryGetBounds(
        UIElement element,
        UIElement root,
        double dpiScale,
        int pixelWidth,
        int pixelHeight,
        out (double X, double Y, double Width, double Height) bounds)
    {
        bounds = default;
        if (element.RenderSize.Width <= 0 || element.RenderSize.Height <= 0)
        {
            return false;
        }

        Rect logicalBounds;
        try
        {
            var local = new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height);
            logicalBounds = ReferenceEquals(element, root)
                ? local
                : element.TransformToAncestor(root).TransformBounds(local);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        var pixelBounds = new Rect(
            logicalBounds.X * dpiScale,
            logicalBounds.Y * dpiScale,
            logicalBounds.Width * dpiScale,
            logicalBounds.Height * dpiScale);
        pixelBounds = Rect.Intersect(pixelBounds, new Rect(0, 0, pixelWidth, pixelHeight));
        if (pixelBounds.IsEmpty || pixelBounds.Width <= 0 || pixelBounds.Height <= 0)
        {
            return false;
        }

        bounds = (
            Math.Round(pixelBounds.X, 2),
            Math.Round(pixelBounds.Y, 2),
            Math.Round(pixelBounds.Width, 2),
            Math.Round(pixelBounds.Height, 2));
        return true;
    }

    private static string Normalize(string text)
    {
        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();
        return normalized.Length <= MaxTextLength
            ? normalized
            : normalized[..MaxTextLength] + "…";
    }

    private static string? NullIfEmpty(this string? value) =>
        string.IsNullOrEmpty(value) ? null : value;
}
