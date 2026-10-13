using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class TreeViewViewModel : ITreeViewStateSink
{
    private string _selectedTreeStatus = "Selected: (none)";

    public TreeViewViewModel()
    {
        FileSystemItems = new[]
        {
            Node(
                "Documents",
                Node(
                    "Work",
                    Node("Report.docx"),
                    Node("Presentation.pptx"),
                    Node("Budget.xlsx")),
                Node(
                    "Personal",
                    Node("Notes.txt"),
                    Node("Photos"))),
            Node(
                "Downloads",
                Node("installer.exe"),
                Node("archive.zip")),
            Node("Desktop", Node("Shortcut.lnk"))
        };
        CategoryItems = new[]
        {
            Node(
                "Animals",
                Node(
                    "Mammals",
                    Node("Dog"),
                    Node("Cat"),
                    Node("Elephant")),
                Node(
                    "Birds",
                    Node("Eagle"),
                    Node("Sparrow"))),
            Node(
                "Plants",
                Node("Trees"),
                Node("Flowers"),
                Node("Grass"))
        };
        ExpandedItems = new[]
        {
            ExpandedNode(
                "Project",
                ExpandedNode(
                    "src",
                    Node("App.cs"),
                    Node("MainWindow.cs"),
                    Node(
                        "Components",
                        Node("Button.cs"),
                        Node("TextBox.cs"))),
                Node("tests", Node("UnitTests.cs")),
                Node("README.md"))
        };
    }

    public IReadOnlyList<SelectionTreeNode> FileSystemItems { get; }

    public IReadOnlyList<SelectionTreeNode> CategoryItems { get; }

    public IReadOnlyList<SelectionTreeNode> ExpandedItems { get; }

    public string SelectedTreeStatus
    {
        get => _selectedTreeStatus;
        private set => SetProperty(ref _selectedTreeStatus, value);
    }

    public void UpdateTreeSelection(string? label) =>
        SelectedTreeStatus =
            $"Selected: {label ?? "(none)"}";

    private static SelectionTreeNode Node(
        string label,
        params SelectionTreeNode[] children) =>
        new(label, false, children);

    private static SelectionTreeNode ExpandedNode(
        string label,
        params SelectionTreeNode[] children) =>
        new(label, true, children);
}

public sealed partial class TreeSelectorViewModel
    : ITreeSelectorStateSink
{
    private string _singleSelectionStatus = "未选择";
    private string _multipleSelectionStatus =
        "已选择 0 项 · 已勾选 0 项";
    private string _cascadeSelectionStatus = "已勾选 0 项";

    public TreeSelectorViewModel()
    {
        Regions = new[]
        {
            new SelectionTreeNode(
                "华东地区",
                true,
                new SelectionTreeNode(
                    "上海",
                    true,
                    new SelectionTreeNode("浦东新区"),
                    new SelectionTreeNode("徐汇区"),
                    new SelectionTreeNode("静安区")),
                new SelectionTreeNode(
                    "江苏",
                    false,
                    new SelectionTreeNode("南京"),
                    new SelectionTreeNode("苏州")),
                new SelectionTreeNode(
                    "浙江省",
                    false,
                    new SelectionTreeNode("杭州"),
                    new SelectionTreeNode("宁波"))),
            new SelectionTreeNode(
                "华南地区",
                false,
                new SelectionTreeNode("广州"),
                new SelectionTreeNode("深圳"))
        };
    }

    public IReadOnlyList<SelectionTreeNode> Regions { get; }

    public string SingleSelectionStatus
    {
        get => _singleSelectionStatus;
        private set =>
            SetProperty(ref _singleSelectionStatus, value);
    }

    public string MultipleSelectionStatus
    {
        get => _multipleSelectionStatus;
        private set =>
            SetProperty(ref _multipleSelectionStatus, value);
    }

    public string CascadeSelectionStatus
    {
        get => _cascadeSelectionStatus;
        private set =>
            SetProperty(ref _cascadeSelectionStatus, value);
    }

    public void UpdateTreeSelectorState(
        string key,
        string? label,
        int selectedCount,
        int checkedCount)
    {
        if (string.Equals(
            key,
            "single",
            StringComparison.Ordinal))
        {
            SingleSelectionStatus = label is null
                ? "未选择"
                : $"已选择: {label}";
            return;
        }

        if (string.Equals(
            key,
            "multiple",
            StringComparison.Ordinal))
        {
            MultipleSelectionStatus =
                $"已选择 {selectedCount} 项 · 已勾选 {checkedCount} 项";
            return;
        }

        if (string.Equals(
            key,
            "cascade",
            StringComparison.Ordinal))
        {
            CascadeSelectionStatus = $"已勾选 {checkedCount} 项";
        }
    }
}
