using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;

namespace Jalium.UI.Gallery.Headless;

internal sealed record HeadlessPageDescriptor(
    string PageTag,
    string Title,
    string Category,
    Type PageType);

internal static class HeadlessCatalog
{
    private static readonly IReadOnlyList<HeadlessPageDescriptor> s_pages = Build();

    private static readonly IReadOnlyDictionary<string, HeadlessPageDescriptor> s_byTag =
        s_pages.ToDictionary(page => page.PageTag, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<HeadlessPageDescriptor> Pages => s_pages;

    public static HeadlessPageDescriptor Require(string pageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);
        if (s_byTag.TryGetValue(pageTag, out var descriptor))
        {
            return descriptor;
        }

        var suggestions = s_pages
            .Where(page =>
                page.PageTag.Contains(pageTag, StringComparison.OrdinalIgnoreCase) ||
                page.Title.Contains(pageTag, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .Select(page => page.PageTag)
            .ToArray();
        var suffix = suggestions.Length == 0
            ? " Use --list to inspect all page tags."
            : $" Did you mean: {string.Join(", ", suggestions)}?";
        throw new CommandLineException($"Unknown Gallery page '{pageTag}'.{suffix}");
    }

    private static IReadOnlyList<HeadlessPageDescriptor> Build()
    {
        var pages = new List<HeadlessPageDescriptor>
        {
            new("home", "Components", "Gallery", typeof(HomePage)),
            new("design-tokens", "Design Tokens", "Foundations", typeof(DesignTokensPage)),
            new("theme-studio", "Theme Studio", "Foundations", typeof(ThemeStudioPage)),
        };
        pages.AddRange(HomeCatalog.Items.Select(item => new HeadlessPageDescriptor(
            item.PageTag,
            item.Title,
            item.Category,
            item.PageType)));
        return pages;
    }
}
