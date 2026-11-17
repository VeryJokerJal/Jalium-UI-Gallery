namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed class GalleryTreeNode
{
    public GalleryTreeNode(
        string name,
        string type,
        long size,
        IReadOnlyList<GalleryTreeNode>? children = null)
    {
        Name = name;
        Type = type;
        Size = size;
        Children = children ?? Array.Empty<GalleryTreeNode>();
    }

    public string Name { get; }

    public string Type { get; }

    public long Size { get; }

    public IReadOnlyList<GalleryTreeNode> Children { get; }

    public string SizeDisplay => Size > 0
        ? $"{Size:N0} B"
        : string.Empty;
}
