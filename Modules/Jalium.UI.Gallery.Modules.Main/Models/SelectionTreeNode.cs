namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed class SelectionTreeNode
{
    public SelectionTreeNode(
        string label,
        bool isExpanded = false,
        params SelectionTreeNode[] children)
    {
        Label = label;
        IsExpanded = isExpanded;
        Children = children;
    }

    public string Label { get; }

    public bool IsExpanded { get; }

    public IReadOnlyList<SelectionTreeNode> Children { get; }

    public override string ToString() => Label;
}
