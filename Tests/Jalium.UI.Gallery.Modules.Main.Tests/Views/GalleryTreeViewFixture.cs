using System.Collections.ObjectModel;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryTreeViewFixture
{
    public GalleryTreeViewFixture()
    {
        // Each test resets the process-wide theme registry. Restore the native
        // container templates before checking the rendered hierarchy.
        using var stream = new MemoryStream();
        var defaults = ThemeManager.XamlLoader!(stream,"Themes/Generic.jalxaml",typeof(Control).Assembly)!;
        ThemeManager.RegisterControlThemes(new ResourceDictionary { [typeof(TreeView)] = defaults[typeof(TreeView)], [typeof(TreeViewItem)] = defaults[typeof(TreeViewItem)] });
    }
    [Fact]
    public void StandardItemsSourceAndHierarchicalTemplateRealizeChildrenOnExpansion()
    {
        var leaf = new SelectionTreeNode("Leaf"); var node = new SelectionTreeNode("Root",false,leaf);
        var tree = new GalleryTreeView { ItemsSource = new[] { node } }; Layout(tree);
        Assert.Same(node,tree.Items[0]); Assert.IsType<HierarchicalDataTemplate>(tree.ItemTemplate);
        var root = Container(tree,0); Assert.Same(node,root.Header); Assert.False(root.IsExpanded); Assert.Empty(root.Items);
        Assert.True(tree.Template != null,"GalleryTreeView is missing its inherited default template");
        Assert.True(root.Template != null,"TreeViewItem is missing its native default template");
        root.IsExpanded = true; Layout(tree); var child = Container(root,0); Assert.Same(leaf,child.Header);
        Assert.Contains(Walk(tree).OfType<TextBlock>(), text => text.Text == "Leaf");
    }

    [Fact]
    public void InitiallyExpandedSamplesKeepTheCompleteHierarchy()
    {
        var vm = new TreeViewViewModel(); var tree = new GalleryTreeView { ItemsSource = vm.ExpandedItems }; Layout(tree);
        var project = Container(tree,0); Assert.True(project.IsExpanded); Assert.Equal("Project",Assert.IsType<SelectionTreeNode>(project.Header).Label);
        var source = Container(project,0); Assert.True(source.IsExpanded); Assert.Equal("src",Assert.IsType<SelectionTreeNode>(source.Header).Label);
        Assert.Equal("App.cs",Assert.IsType<SelectionTreeNode>(Container(source,0).Header).Label);
    }

    [Fact]
    public void SelectionKeepsTheExistingStateSinkAndHonorsTheOptionalCommand()
    {
        var vm = new TreeViewViewModel(); var received = new List<object?>();
        var tree = new GalleryTreeView { DataContext = vm,ItemsSource = vm.CategoryItems,PublishSelection = true,SelectionChangedCommand = new RelayCommand(value => received.Add(value)) }; Layout(tree);
        Container(tree,0).IsSelected = true;
        Assert.Equal("Selected: Animals",vm.SelectedTreeStatus); Assert.Same(vm.CategoryItems[0],Assert.Single(received));
        tree.SelectionChangedCommand = new RelayCommand(value => received.Add(value),_ => false); Container(tree,1).IsSelected = true;
        Assert.Equal("Selected: Plants",vm.SelectedTreeStatus); Assert.Single(received);
        tree.PublishSelection = false; Container(tree,0).IsSelected = true; Assert.Equal("Selected: Plants",vm.SelectedTreeStatus);
    }

    [Fact]
    public void SourceReplacementAndObservableRootsUseTheFrameworkContainerLifecycle()
    {
        var nodes = new ObservableCollection<SelectionTreeNode> { new("A") }; var tree = new GalleryTreeView { ItemsSource = nodes }; Layout(tree);
        Assert.Equal("A",Assert.IsType<SelectionTreeNode>(Container(tree,0).Header).Label);
        nodes.Clear(); nodes.Add(new("B",true,new SelectionTreeNode("B1"))); Layout(tree);
        var root = Container(tree,0); Assert.Equal("B",Assert.IsType<SelectionTreeNode>(root.Header).Label); Assert.True(root.IsExpanded); Assert.Single(root.Items.Cast<object>());
        tree.ItemsSource = null; Layout(tree); Assert.Empty(tree.Items);
    }

    [Fact]
    public void ConsumersCanReplaceItemAndControlTemplatesWithoutLosingTheSource()
    {
        var nodes = new[] { new SelectionTreeNode("Custom") }; var tree = new GalleryTreeView { ItemsSource = nodes }; Layout(tree);
        var itemTemplate = new DataTemplate(); itemTemplate.SetVisualTree(() => new TextBlock { Text = "Consumer header" }); tree.ItemTemplate = itemTemplate; Layout(tree);
        Assert.Contains(Walk(tree).OfType<TextBlock>(), text => text.Text == "Consumer header");
        var original = tree.Template; var blank = new ControlTemplate(typeof(GalleryTreeView)); blank.SetVisualTree(() => new Border()); tree.Template = blank; Layout(tree);
        Assert.Same(nodes[0],tree.Items[0]); tree.Template = original; Layout(tree); Assert.Same(nodes[0],Container(tree,0).Header);
    }

    [Fact]
    public void IndependentPreviewUsesTheSameReusableTreeControl()
    {
        var vm = new TreeViewViewModel(); var template = Assert.IsType<DataTemplate>(GalleryControlThemes.FindPreviewTemplate("Gallery.Preview.TreeViewPage"));
        var presenter = new ContentPresenter { Content = vm,ContentTemplate = template }; Layout(presenter,640);
        var tree = Assert.Single(Walk(presenter).OfType<GalleryTreeView>()); Assert.Same(vm.FileSystemItems,tree.ItemsSource);
    }

    private static TreeViewItem Container(ItemsControl owner,int index) => Assert.IsType<TreeViewItem>(owner.ItemContainerGenerator.ContainerFromIndex(index));
    private static IEnumerable<Visual> Walk(Visual root) { yield return root; for (int index = 0; index < root.VisualChildrenCount; index++) if (root.GetVisualChild(index) is { } child) foreach (var item in Walk(child)) yield return item; }
    private static void Layout(UIElement element,double width = 300) { for (int pass = 0; pass < 3; pass++) { element.InvalidateMeasure(); element.Measure(new Size(width,450)); element.Arrange(new Rect(0,0,width,450)); element.UpdateLayout(); } }
}
