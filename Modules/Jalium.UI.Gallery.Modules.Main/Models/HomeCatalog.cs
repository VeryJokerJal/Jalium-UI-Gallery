namespace Jalium.UI.Gallery.Modules.Main.Models;

public static class HomeCatalog
{
    public static IReadOnlyList<HomeCatalogItem> Items { get; } =
        GalleryComponentCatalog.Items
            .Select(item => new HomeCatalogItem(
                item.PageTag,
                item.Title,
                item.Category,
                item.PageTag,
                item.ComponentTypeName,
                item.PageType,
                item.ExampleMarkup,
                item.PreviewKind))
            .ToArray();
}
