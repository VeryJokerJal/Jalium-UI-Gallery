using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for TreeSelectorPage — demonstrates the cascade-style drop-down TreeSelector
/// across single-select (path display), multi-select (chip list), and search-enabled modes.
/// </summary>
public partial class TreeSelectorPage : Page
{
    public TreeSelectorPage()
    {
        InitializeComponent();
        CreateContent();
        LoadCodeExamples();
    }

    private void CreateContent()
    {
        if (DemoHost == null) return;

        // ----- Single-select with path display -----
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "单选模式",
            "SelectionMode = Single. The trigger displays the full ancestor path; selecting a leaf auto-closes the popup.",
            BuildSingleSelectionDemo()));

        // ----- Multi-select with chips -----
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "多选模式",
            "SelectionMode = Multiple + ShowCheckBoxes = True. Each selected item appears as a removable chip in the trigger.",
            BuildMultiSelectionDemo()));

        // ----- Searchable -----
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "可搜索",
            "IsSearchEnabled = True. Typing filters the tree to entries that match (along with their ancestors).",
            BuildSearchableDemo()));

        // ----- Cascade with tri-state -----
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "父子级联",
            "CheckCascadeMode = Cascade — checking a parent flips every descendant; mixed children render the parent in the indeterminate state.",
            BuildCascadeDemo()));
    }

    private StackPanel BuildSingleSelectionDemo()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var statusText = GalleryUi.ValueLabel("未选择");
        statusText.Margin = new Thickness(0, 0, 0, 8);
        stack.Children.Add(statusText);

        var selector = new TreeSelector
        {
            SelectionMode = SelectionMode.Single,
            PlaceholderText = "请选择节点",
            Width = 320,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        PopulateRegionTree(selector);
        selector.SelectionChanged += (_, _) =>
        {
            statusText.Text = selector.SelectedItem != null
                ? $"已选择: {selector.SelectedItem}"
                : "未选择";
        };

        stack.Children.Add(selector);
        return stack;
    }

    private StackPanel BuildMultiSelectionDemo()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var statusText = GalleryUi.ValueLabel("已选 0 项");
        statusText.Margin = new Thickness(0, 0, 0, 8);
        stack.Children.Add(statusText);

        var selector = new TreeSelector
        {
            SelectionMode = SelectionMode.Multiple,
            ShowCheckBoxes = true,
            PlaceholderText = "请选择城市 (可多选)",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        PopulateRegionTree(selector);
        selector.SelectionChanged += (_, _) =>
        {
            statusText.Text = $"已选 {selector.SelectedItems.Count} 项";
        };

        stack.Children.Add(selector);
        return stack;
    }

    private StackPanel BuildSearchableDemo()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var hintText = GalleryUi.CaptionText("在触发器中输入文字过滤树,例如 “浦” 或 “杭”。");
        hintText.Margin = new Thickness(0, 0, 0, 8);
        stack.Children.Add(hintText);

        var selector = new TreeSelector
        {
            SelectionMode = SelectionMode.Single,
            IsSearchEnabled = true,
            PlaceholderText = "搜索...",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        PopulateRegionTree(selector);

        stack.Children.Add(selector);
        return stack;
    }

    private StackPanel BuildCascadeDemo()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var hintText = GalleryUi.CaptionText("勾选 “华东地区” 会一次性勾选所有子节点;再单独取消一个,父节点变成半选状态。");
        hintText.Margin = new Thickness(0, 0, 0, 8);
        stack.Children.Add(hintText);

        var selector = new TreeSelector
        {
            SelectionMode = SelectionMode.Multiple,
            ShowCheckBoxes = true,
            CheckCascadeMode = TreeSelectorCheckCascadeMode.Cascade,
            PlaceholderText = "请选择(支持级联)",
            Width = 380,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        PopulateRegionTree(selector);

        stack.Children.Add(selector);
        return stack;
    }

    private static void PopulateRegionTree(TreeSelector selector)
    {
        var east = new TreeSelectorItem { Header = "华东地区", IsExpanded = true };

        var shanghai = new TreeSelectorItem { Header = "上海", IsExpanded = true };
        shanghai.Items.Add(new TreeSelectorItem { Header = "浦东新区" });
        shanghai.Items.Add(new TreeSelectorItem { Header = "徐汇区" });
        shanghai.Items.Add(new TreeSelectorItem { Header = "静安区" });
        east.Items.Add(shanghai);

        var jiangsu = new TreeSelectorItem { Header = "江苏" };
        jiangsu.Items.Add(new TreeSelectorItem { Header = "南京" });
        jiangsu.Items.Add(new TreeSelectorItem { Header = "苏州" });
        east.Items.Add(jiangsu);

        var zhejiang = new TreeSelectorItem { Header = "浙江省" };
        zhejiang.Items.Add(new TreeSelectorItem { Header = "杭州" });
        zhejiang.Items.Add(new TreeSelectorItem { Header = "宁波" });
        east.Items.Add(zhejiang);

        selector.Items.Add(east);

        var south = new TreeSelectorItem { Header = "华南地区" };
        south.Items.Add(new TreeSelectorItem { Header = "广州" });
        south.Items.Add(new TreeSelectorItem { Header = "深圳" });
        selector.Items.Add(south);
    }

    private const string XamlExample =
@"<!-- Single-select with path display -->
<TreeSelector Width=""320""
              SelectionMode=""Single""
              PlaceholderText=""请选择节点"">
    <TreeSelectorItem Header=""华东地区"" IsExpanded=""True"">
        <TreeSelectorItem Header=""上海"" IsExpanded=""True"">
            <TreeSelectorItem Header=""浦东新区""/>
            <TreeSelectorItem Header=""徐汇区""/>
        </TreeSelectorItem>
    </TreeSelectorItem>
</TreeSelector>

<!-- Multi-select with chip trigger -->
<TreeSelector Width=""360""
              SelectionMode=""Multiple""
              ShowCheckBoxes=""True""
              PlaceholderText=""请选择城市"">
    ...
</TreeSelector>

<!-- Searchable + cascade tri-state -->
<TreeSelector Width=""380""
              SelectionMode=""Multiple""
              ShowCheckBoxes=""True""
              CheckCascadeMode=""Cascade""
              IsSearchEnabled=""True""
              PlaceholderText=""搜索..."">
    ...
</TreeSelector>";

    private const string CSharpExample =
@"using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

var selector = new TreeSelector
{
    SelectionMode = SelectionMode.Multiple,
    ShowCheckBoxes = true,
    CheckCascadeMode = TreeSelectorCheckCascadeMode.Cascade,
    IsSearchEnabled = true,
    PlaceholderText = ""请选择...""
};

var east = new TreeSelectorItem
{
    Header = ""华东地区"",
    IsExpanded = true
};
var shanghai = new TreeSelectorItem
{
    Header = ""上海"",
    IsExpanded = true
};
shanghai.Items.Add(new TreeSelectorItem { Header = ""浦东新区"" });
shanghai.Items.Add(new TreeSelectorItem { Header = ""徐汇区"" });
east.Items.Add(shanghai);
selector.Items.Add(east);

selector.SelectionChanged += (_, e) =>
{
    Console.WriteLine($""已选 {selector.SelectedItems.Count} 项"");
};

selector.DropDownOpened += (_, _) =>
{
    Console.WriteLine(""popup 已打开"");
};

// Programmatically remove a chip
selector.RemoveFromSelection(""上海"");

// Programmatically clear search
selector.SearchText = string.Empty;";

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
}
