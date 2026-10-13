using System.Collections;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class TreeViewBehavior
{
    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.RegisterAttached(
            "Items",
            typeof(IEnumerable),
            typeof(TreeViewBehavior),
            new PropertyMetadata(null, OnItemsChanged));

    public static readonly DependencyProperty PublishSelectionProperty =
        DependencyProperty.RegisterAttached(
            "PublishSelection",
            typeof(bool),
            typeof(TreeViewBehavior),
            new PropertyMetadata(false, OnPublishSelectionChanged));

    public static void SetItems(
        DependencyObject element,
        IEnumerable? value) =>
        element.SetValue(ItemsProperty, value);

    public static IEnumerable? GetItems(DependencyObject element) =>
        element.GetValue(ItemsProperty) as IEnumerable;

    public static void SetPublishSelection(
        DependencyObject element,
        bool value) =>
        element.SetValue(PublishSelectionProperty, value);

    public static bool GetPublishSelection(DependencyObject element) =>
        (bool)(element.GetValue(PublishSelectionProperty) ?? false);

    private static void OnItemsChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TreeView tree)
        {
            return;
        }

        tree.Items.Clear();
        if (args.NewValue is not IEnumerable nodes)
        {
            return;
        }

        foreach (var node in nodes.OfType<SelectionTreeNode>())
        {
            tree.Items.Add(CreateItem(node));
        }
    }

    private static TreeViewItem CreateItem(SelectionTreeNode node)
    {
        var item = new TreeViewItem
        {
            Header = node.Label,
            IsExpanded = node.IsExpanded
        };
        foreach (var child in node.Children)
        {
            item.Items.Add(CreateItem(child));
        }

        return item;
    }

    private static void OnPublishSelectionChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TreeView tree)
        {
            return;
        }

        tree.SelectedItemChanged -= OnSelectedItemChanged;
        if (args.NewValue is true)
        {
            tree.SelectedItemChanged += OnSelectedItemChanged;
        }
    }

    private static void OnSelectedItemChanged(
        object? sender,
        RoutedPropertyChangedEventArgs<object?> args)
    {
        if (sender is TreeView tree)
        {
            var label = args.NewValue switch
            {
                TreeViewItem item => item.Header?.ToString(),
                SelectionTreeNode node => node.Label,
                _ => args.NewValue?.ToString()
            };
            (tree.DataContext as ITreeViewStateSink)
                ?.UpdateTreeSelection(label);
        }
    }
}

public static class TreeSelectorBehavior
{
    public static readonly DependencyProperty NodesProperty =
        DependencyProperty.RegisterAttached(
            "Nodes",
            typeof(IEnumerable),
            typeof(TreeSelectorBehavior),
            new PropertyMetadata(null, OnNodesChanged));

    public static readonly DependencyProperty StateKeyProperty =
        DependencyProperty.RegisterAttached(
            "StateKey",
            typeof(string),
            typeof(TreeSelectorBehavior),
            new PropertyMetadata(string.Empty, OnStateKeyChanged));

    public static void SetNodes(
        DependencyObject element,
        IEnumerable? value) =>
        element.SetValue(NodesProperty, value);

    public static IEnumerable? GetNodes(DependencyObject element) =>
        element.GetValue(NodesProperty) as IEnumerable;

    public static void SetStateKey(
        DependencyObject element,
        string value) =>
        element.SetValue(StateKeyProperty, value);

    public static string GetStateKey(DependencyObject element) =>
        element.GetValue(StateKeyProperty) as string ??
        string.Empty;

    private static void OnNodesChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TreeSelector selector)
        {
            return;
        }

        selector.DropDownOpened -= OnDropDownOpened;
        selector.UnselectAll();
        selector.CheckedItems.Clear();
        selector.ItemsSource = null;
        selector.Items.Clear();
        if (args.NewValue is not IEnumerable<SelectionTreeNode> nodes)
        {
            PublishState(selector);
            return;
        }

        // The current hierarchical contract is an explicit TreeSelectorItem
        // graph. Adding the roots through ItemCollection gives TreeSelector the
        // per-item notifications it uses to establish parent links before its
        // popup is realized; a pre-populated ItemsSource only reports Reset and
        // cannot provide that guarantee yet.
        foreach (var node in nodes)
        {
            selector.Items.Add(CreateItem(node, selector));
        }

        selector.DropDownOpened += OnDropDownOpened;
        PublishState(selector);
    }

    private static TreeSelectorItem CreateItem(
        SelectionTreeNode node,
        TreeSelector selector)
    {
        var item = new TreeSelectorItem
        {
            Header = node.Label,
            IsExpanded = node.IsExpanded,
            DataContext = node
        };
        item.Loaded += (_, _) =>
            SynchronizeCheckBoxVisibility(item, selector);

        if (node.Children.Count > 0)
        {
            foreach (var child in node.Children)
            {
                item.Items.Add(CreateItem(child, selector));
            }
        }

        return item;
    }

    private static void SynchronizeCheckBoxVisibility(
        TreeSelectorItem item,
        TreeSelector selector)
    {
        // The template declares PART_CheckBox as Collapsed. With containers
        // realized for the first time inside a Popup, that template-local value
        // currently lands after TreeSelectorItem.OnApplyTemplate has attempted
        // its initial synchronization. Loaded is the first stable point at
        // which the public ShowCheckBoxes value can be mirrored reliably.
        if (item.FindName("PART_CheckBox") is CheckBox checkBox)
        {
            checkBox.Visibility = selector.ShowCheckBoxes
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private static void OnDropDownOpened(
        object? sender,
        RoutedEventArgs args)
    {
        if (sender is not TreeSelector selector)
        {
            return;
        }

        PreparePopup(selector);

        // Popup children are templated during the opening measure. Defer one
        // turn so the template parts exist, then synchronize every currently
        // realized node. Per-item Loaded handlers cover nodes realized by a
        // later expansion while the popup remains open.
        Jalium.UI.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(() =>
        {
            if (!selector.IsDropDownOpen)
            {
                return;
            }

            PreparePopup(selector);
            SynchronizeCheckBoxVisibility(selector.Items, selector);
        });
    }

    private static void PreparePopup(TreeSelector selector)
    {
        if (selector.FindName("PART_Popup") is not Popup popup)
        {
            return;
        }

        popup.Closed -= OnPopupClosed;
        popup.Closed += OnPopupClosed;
        if (selector.ActualWidth > 0)
        {
            popup.Width = selector.ActualWidth;
        }
    }

    private static void OnPopupClosed(object? sender, EventArgs args)
    {
        if (sender is not Popup popup || popup.IsOpen ||
            popup.TemplatedParent is not TreeSelector selector ||
            !ReferenceEquals(selector.FindName("PART_Popup"), popup))
        {
            return;
        }

        // Light dismiss writes a local IsOpen=false on the Popup. TemplateBinding
        // is one-way, so update the owner first, then remove that local override
        // so its template binding can open the same popup on the next click.
        selector.SetCurrentValue(TreeSelector.IsDropDownOpenProperty, false);
        popup.ClearValue(Popup.IsOpenProperty);
    }

    private static void SynchronizeCheckBoxVisibility(
        IEnumerable items,
        TreeSelector selector)
    {
        foreach (var item in items.OfType<TreeSelectorItem>())
        {
            SynchronizeCheckBoxVisibility(item, selector);
            SynchronizeCheckBoxVisibility(item.Items, selector);
        }
    }

    private static void OnStateKeyChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TreeSelector selector)
        {
            return;
        }

        selector.SelectionChanged -= OnSelectionChanged;
        selector.ItemCheckStateChanged -= OnItemCheckStateChanged;
        if (!string.IsNullOrWhiteSpace(args.NewValue as string))
        {
            selector.SelectionChanged += OnSelectionChanged;
            selector.ItemCheckStateChanged += OnItemCheckStateChanged;
            PublishState(selector);
        }
    }

    private static void OnSelectionChanged(
        object? sender,
        SelectionChangedEventArgs args)
    {
        if (sender is not TreeSelector selector)
        {
            return;
        }

        PublishState(selector);
    }

    private static void OnItemCheckStateChanged(
        object? sender,
        RoutedEventArgs args)
    {
        if (sender is TreeSelector selector)
        {
            PublishState(selector);
        }
    }

    internal static void PublishState(TreeSelector selector)
    {
        var key = GetStateKey(selector);
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var label = selector.SelectedItem switch
        {
            TreeSelectorItem item => item.Header?.ToString(),
            SelectionTreeNode node => node.Label,
            _ => selector.SelectedItem?.ToString()
        };
        (selector.DataContext as ITreeSelectorStateSink)
            ?.UpdateTreeSelectorState(
                key,
                label,
                selector.SelectedItems.Count,
                selector.CheckedItems.Count);
    }
}
