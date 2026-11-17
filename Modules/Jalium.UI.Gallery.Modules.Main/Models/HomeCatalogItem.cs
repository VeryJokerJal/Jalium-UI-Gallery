using Jalium.UI.Gallery.Modules.Main.Localization;

namespace Jalium.UI.Gallery.Modules.Main.Models;

/// <summary>
/// Immutable presentation model for one component in the Gallery catalog.
/// <see cref="Category"/> stays invariant — filtering, search, and the card
/// artwork all key off it — while <see cref="CategoryLabel"/> and
/// <see cref="Description"/> resolve against the active language on every read.
/// </summary>
public sealed record HomeCatalogItem(
    string Key,
    string Title,
    string Category,
    string PageTag,
    string ComponentTypeName,
    Type PageType,
    string Snippet,
    string PreviewKind)
{
    public string CategoryLabel => LocalizationManager.GetCategory(Category);

    public string Description =>
        LocalizationManager.GetDescription(
            LocalizationManager.FindDescriptionKey(PageTag),
            Title);
}
