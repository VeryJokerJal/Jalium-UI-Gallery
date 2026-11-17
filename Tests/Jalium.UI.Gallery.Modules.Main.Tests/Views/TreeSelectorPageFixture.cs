using System.Reflection;
using System.Text;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class TreeSelectorPageFixture
{
    [Fact]
    public void RestoringTheSelectorTemplateKeepsRenderedDataHeaders()
    {
        ResetApplicationState(); var app = new App(); GalleryTreeSelector? selector = null;
        try
        {
            var page = new TreeSelectorPage { DataContext = new TreeSelectorViewModel() };
            var window = new DisplayedTestWindow { Width = 1000, Height = 1800, Content = page }; app.MainWindow = window; SettleLayout(window);
            selector = Assert.IsType<GalleryTreeSelector>(page.FindName("MultipleSelector"));
            selector.IsDropDownOpen = true; SettleLayout(window);
            var popup = Assert.IsType<Popup>(selector.FindName("PART_Popup"));
            Assert.Contains("华东地区", AllText(GetPrivateField<FrameworkElement>(popup, "_popupRoot")));
            selector.SelectedItems.Add(selector.Items[0]!); Assert.True(selector.IsDropDownOpen);
            var original = selector.Template; var minimal = new ControlTemplate(typeof(GalleryTreeSelector)); minimal.SetVisualTree(() => new Border());
            selector.Template = minimal; selector.ApplyTemplate(); Assert.False(popup.IsOpen); Assert.True(selector.IsDropDownOpen);
            selector.Template = original; selector.ApplyTemplate(); SettleLayout(window);
            selector.IsDropDownOpen = true; SettleLayout(window);
            popup = Assert.IsType<Popup>(selector.FindName("PART_Popup"));
            var text = AllText(GetPrivateField<FrameworkElement>(popup, "_popupRoot")).ToArray();
            Assert.True(text.Contains("华东地区"), "Recreated popup text: " + string.Join(" | ",text));
            Assert.Contains("上海", text); Assert.Contains("浦东新区", text);
        }
        finally { if (selector != null) selector.IsDropDownOpen = false; ResetApplicationState(); }
    }

    private static IEnumerable<string> AllText(Visual visual)
    {
        if (visual is TextBlock text) yield return text.Text ?? "";
        for (int index = 0; index < visual.VisualChildrenCount; index++)
            if (visual.GetVisualChild(index) is { } child)
                foreach (var value in AllText(child)) yield return value;
    }
    [Fact]
    public void SearchSetBeforeOpeningSurvivesTheFirstPopupContainerGeneration()
    {
        ResetApplicationState(); var app = new App(); GalleryTreeSelector? selector = null;
        try
        {
            var page = new TreeSelectorPage { DataContext = new TreeSelectorViewModel() };
            var window = new DisplayedTestWindow { Width = 1000, Height = 1800, Content = page }; app.MainWindow = window; SettleLayout(window);
            selector = Assert.IsType<GalleryTreeSelector>(page.FindName("SearchSelector"));
            selector.SearchText = "杭"; selector.IsDropDownOpen = true; SettleLayout(window);
            var east = Container(selector, 0); var zhejiang = Container(east, 2); var hangzhou = Container(zhejiang, 0);
            Assert.Equal(Visibility.Collapsed, Container(selector, 1).Visibility);
            Assert.True(east.IsExpanded); Assert.True(zhejiang.IsExpanded);
            Assert.Equal(Visibility.Visible, hangzhou.Visibility);
            Assert.Equal(Visibility.Visible, Assert.IsType<Grid>(east.FindName("PART_ExpanderBorder")).Visibility);
            selector.IsDropDownOpen = false; selector.IsDropDownOpen = true; SettleLayout(window);
            Assert.Equal(Visibility.Collapsed, Container(selector, 1).Visibility);
            Assert.True(Container(Container(selector, 0), 2).IsExpanded);
            selector.SearchText = ""; SettleLayout(window);
            Assert.Equal(Visibility.Visible, Container(selector, 1).Visibility);
        }
        finally { if (selector != null) selector.IsDropDownOpen = false; ResetApplicationState(); }
    }
    [Fact]
    public void OptionalCommandReceivesTypedStateAndHonorsCanExecute()
    {
        var values = new List<object?>();
        var selector = new GalleryTreeSelector { StateKey = "custom", StateChangedCommand = new RelayCommand(value => values.Add(value)) };
        selector.SelectedItem = new SelectionTreeNode("上海");
        selector.RaiseEvent(new SelectionChangedEventArgs(TreeSelector.SelectionChangedEvent, Array.Empty<object>(), new[] { selector.SelectedItem }));
        var state = Assert.IsType<GalleryTreeSelectorState>(Assert.Single(values));
        Assert.Equal("custom", state.Key); Assert.Equal("上海", state.Label); Assert.Equal(1, state.SelectedCount);
        selector.StateChangedCommand = new RelayCommand(value => values.Add(value), _ => false);
        selector.RaiseEvent(new SelectionChangedEventArgs(TreeSelector.SelectionChangedEvent, Array.Empty<object>(), Array.Empty<object>()));
        Assert.Single(values);
    }

    [Fact]
    public void CustomItemTemplateCanReplaceTheGalleryHierarchy()
    {
        var node = new SelectionTreeNode("root", true, new SelectionTreeNode("leaf"));
        var selector = new GalleryTreeSelector { ItemsSource = new[] { node } };
        var root = Container(selector, 0); Assert.Single(root.Items.Cast<object>());
        var template = new DataTemplate(); template.SetVisualTree(() => new TextBlock { Text = "Consumer" });
        selector.ItemTemplate = template;
        Assert.Same(template, Container(selector, 0).HeaderTemplate);
        Assert.Empty(Container(selector, 0).Items);
        Assert.Same(node, selector.Items[0]);
    }

    [Fact]
    public void CodeSampleUsesTheDataTemplateContract()
    {
        var example = new TreeSelectorViewModel().XamlExample;
        Assert.Contains("HierarchicalDataTemplate", example);
        Assert.Contains("ItemsSource=", example);
        Assert.DoesNotContain("TreeSelectorBehavior", example);
    }
    [Fact]
    public void Nodes_AreProjectedToNativeContainers_WithTheWholeHierarchy()
    {
        var leafNode = new SelectionTreeNode("浦东新区");
        var cityNode = new SelectionTreeNode(
            "上海",
            true,
            leafNode);
        var rootNode = new SelectionTreeNode(
            "华东地区",
            true,
            cityNode);
        var selector = new GalleryTreeSelector();

        selector.ItemsSource = new[] { rootNode };

        Assert.Same(rootNode, Assert.Single(selector.Items.Cast<object>()));
        var root = Container(selector, 0);
        var city = Container(root, 0);
        var leaf = Container(city, 0);

        Assert.Same(rootNode, root.Header);
        Assert.Same(cityNode, city.Header);
        Assert.Same(leafNode, leaf.Header);
        Assert.True(root.IsExpanded);
        Assert.True(city.IsExpanded);
        Assert.Same(rootNode, root.DataContext);
        Assert.Same(cityNode, city.DataContext);
        Assert.Same(leafNode, leaf.DataContext);

        selector.ItemsSource = null;

        Assert.Empty(selector.Items);
    }

    [Fact]
    public void SelectionEvent_PublishesSelectedAndCheckedCountsSeparately()
    {
        var sink = new RecordingTreeSelectorStateSink();
        var selector = new GalleryTreeSelector
        {
            DataContext = sink,
            SelectionMode = SelectionMode.Multiple
        };
        selector.StateKey = "multiple";

        selector.SelectedItem = "上海";
        selector.SelectedItems.Add("上海");
        selector.CheckedItems.Add("浦东新区");
        selector.RaiseEvent(new SelectionChangedEventArgs(
            TreeSelector.SelectionChangedEvent,
            Array.Empty<object>(),
            new object[] { "上海" }));

        Assert.Equal(
            new PublishedTreeSelectorState(
                "multiple",
                "上海",
                1,
                1),
            sink.LastState);

        selector.ItemsSource = new[] { new SelectionTreeNode("华南地区") };

        Assert.Empty(selector.SelectedItems);
        Assert.Empty(selector.CheckedItems);
        Assert.Equal(
            new PublishedTreeSelectorState(
                "multiple",
                null,
                0,
                0),
            sink.LastState);
    }

    [Fact]
    public void CascadeCheckEvent_PublishesEveryCheckedDescendant()
    {
        var sink = new RecordingTreeSelectorStateSink();
        var selector = new GalleryTreeSelector
        {
            DataContext = sink,
            SelectionMode = SelectionMode.Multiple,
            ShowCheckBoxes = true,
            CheckCascadeMode = TreeSelectorCheckCascadeMode.Cascade
        };
        selector.StateKey = "cascade";
        selector.ItemsSource = new[]
            {
                new SelectionTreeNode(
                    "华东地区",
                    true,
                    new SelectionTreeNode("上海"),
                    new SelectionTreeNode("江苏"))
            };
        var root = Container(selector, 0);

        root.IsChecked = true;

        Assert.All(
            Enumerable.Range(0, root.Items.Count).Select(index => Container(root, index)),
            item => Assert.True(item.IsChecked));
        Assert.Equal(
            new PublishedTreeSelectorState(
                "cascade",
                null,
                0,
                3),
            sink.LastState);
    }

    [Fact]
    public void CascadePopup_FirstOpenMeasuresTheExpandedHierarchy()
    {
        var originalLanguage = LocalizationManager.CurrentLanguage;
        var originalTheme = GalleryTheme.CurrentMode;
        ResetApplicationState();
        var app = new App();
        TreeSelector? selector = null;

        try
        {
            var page = new TreeSelectorPage
            {
                DataContext = new TreeSelectorViewModel()
            };
            var window = new DisplayedTestWindow
            {
                Width = 1000,
                Height = 1400,
                Content = page
            };
            app.MainWindow = window;

            window.Measure(new Size(1000, 1400));
            window.Arrange(new Rect(0, 0, 1000, 1400));
            window.UpdateLayout();

            selector = Assert.IsType<GalleryTreeSelector>(
                page.FindName("CascadeSelector"));
            selector.IsDropDownOpen = true;

            window.Measure(new Size(1000, 1400));
            window.Arrange(new Rect(0, 0, 1000, 1400));
            Jalium.UI.Threading.Dispatcher.CurrentDispatcher.ProcessQueue();

            var popup = GetPrivateField<Popup>(selector, "_popup");
            var popupRoot = GetPrivateField<FrameworkElement>(
                popup,
                "_popupRoot");

            Assert.True(
                popupRoot.Height >= 200,
                $"The expanded hierarchy was clipped to a " +
                $"{popupRoot.Height:0.##}-DIP popup." +
                Environment.NewLine +
                DescribeVisualTree(popupRoot));
            Assert.Equal(
                selector.ActualWidth,
                popupRoot.Width,
                precision: 2);
            Assert.Equal(
                8,
                CountDescendants<TreeSelectorItem>(popupRoot));
            var scroll = Assert.IsType<ScrollViewer>(FindDescendant<ScrollViewer>(popupRoot));
            Assert.Equal(selector.MaxDropDownHeight, scroll.MaxHeight);
            selector.MaxDropDownHeight = 280;
            Assert.Equal(280, scroll.MaxHeight); // The owner remains the source after Popup reparents its child.
            Assert.True(selector.ShowCheckBoxes);
            Assert.Equal(
                Visibility.Visible,
                Assert.IsType<CheckBox>(
                    FindDescendant<CheckBox>(popupRoot)).Visibility);
        }
        finally
        {
            if (selector is not null)
            {
                selector.IsDropDownOpen = false;
            }

            LocalizationManager.SetLanguage(originalLanguage);
            GalleryTheme.CurrentMode = originalTheme;
            ResetApplicationState();
        }
    }

    [Theory]
    [InlineData("SingleSelector")]
    [InlineData("MultipleSelector")]
    [InlineData("SearchSelector")]
    [InlineData("CascadeSelector")]
    public void Popup_CanReopenAfterOutsideClickAndTriggerClose(string selectorName)
    {
        var originalLanguage = LocalizationManager.CurrentLanguage;
        var originalTheme = GalleryTheme.CurrentMode;
        ResetApplicationState();
        var app = new App();
        Popup? popup = null;
        TreeSelector? selector = null;

        try
        {
            var page = new TreeSelectorPage { DataContext = new TreeSelectorViewModel() };
            var window = new DisplayedTestWindow { Width = 1000, Height = 1800, Content = page };
            app.MainWindow = window;
            SettleLayout(window);

            selector = Assert.IsType<GalleryTreeSelector>(page.FindName(selectorName));
            popup = Assert.IsType<Popup>(selector.FindName("PART_Popup"));
            popup.ShouldConstrainToRootBounds = true;
            var trigger = Assert.IsType<Border>(selector.FindName("PART_TriggerBorder"));
            var overlay = typeof(Window).GetProperty(
                "OverlayLayer", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
            var dismiss = overlay.GetType().GetMethod("TryHandleLightDismiss")!;
            var closedCount = 0;
            selector.DropDownClosed += (_, _) => closedCount++;
            var selected = selector.Items[0]!;
            selector.SelectedItems.Add(selected);
            if (selector.ShowCheckBoxes)
            {
                Container(selector, 0).IsChecked = true;
            }
            var checkedItems = selector.CheckedItems.Cast<object>().ToArray();

            for (var cycle = 0; cycle < 3; cycle++)
            {
                ClickTrigger(trigger);
                SettleLayout(window);
                Assert.True(selector.IsDropDownOpen);
                Assert.True(popup.IsOpen);
                var root = GetPrivateField<FrameworkElement>(popup, "_popupRoot");
                Assert.True(root.Height >= 200);
                Assert.Equal(8, CountDescendants<TreeSelectorItem>(root));

                // Exercise the same light-dismiss path that Window uses for an outside click.
                if (cycle != 1)
                {
                    Assert.Equal(true, dismiss.Invoke(overlay, new object[] { new Point(999, 1) }));
                }
                else
                {
                    ClickTrigger(trigger);
                }

                SettleLayout(window);
                Assert.False(popup.IsOpen);
                Assert.False(selector.IsDropDownOpen);
                Assert.Equal(cycle + 1, closedCount);
                Assert.Equal(new[] { selected }, selector.SelectedItems.Cast<object>());
                Assert.Equal(checkedItems, selector.CheckedItems.Cast<object>());
            }
        }
        finally
        {
            if (popup is not null) popup.IsOpen = false;
            if (selector is not null) selector.IsDropDownOpen = false;
            LocalizationManager.SetLanguage(originalLanguage);
            GalleryTheme.CurrentMode = originalTheme;
            ResetApplicationState();
        }
    }

    private static TreeSelectorItem Container(ItemsControl owner, int index)
    {
        var generator = owner.ItemContainerGenerator;
        if (generator.ContainerFromIndex(index) is TreeSelectorItem current) return current;
        using var generation = generator.StartAt(generator.GeneratorPositionFromIndex(index), GeneratorDirection.Forward, true);
        var container = Assert.IsType<TreeSelectorItem>(generator.GenerateNext(out var created));
        if (created) generator.PrepareItemContainer(container);
        return container;
    }

    private static void ClickTrigger(Border trigger) =>
        trigger.RaiseEvent(new MouseButtonEventArgs(
            UIElement.MouseDownEvent, new Point(10, 10),
            MouseButton.Left, MouseButtonState.Pressed, 1,
            MouseButtonState.Pressed, MouseButtonState.Released,
            MouseButtonState.Released, MouseButtonState.Released,
            MouseButtonState.Released, ModifierKeys.None, 0));

    private static void SettleLayout(Window window)
    {
        window.Measure(new Size(window.Width, window.Height));
        window.Arrange(new Rect(0, 0, window.Width, window.Height));
        window.UpdateLayout();
        Jalium.UI.Threading.Dispatcher.CurrentDispatcher.ProcessQueue();
    }

    private static T GetPrivateField<T>(
        object instance,
        string fieldName)
        where T : class
    {
        var type = instance.GetType();
        FieldInfo? field = null;
        while (type != null && field == null)
        {
            field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            type = type.BaseType;
        }
        Assert.NotNull(field);
        return Assert.IsAssignableFrom<T>(field!.GetValue(instance));
    }

    private static string DescribeVisualTree(DependencyObject root)
    {
        var builder = new StringBuilder();
        Append(root, 0);
        return builder.ToString();

        void Append(DependencyObject current, int depth)
        {
            builder.Append(' ', depth * 2);
            builder.Append(current.GetType().Name);
            if (current is FrameworkElement element)
            {
                builder.Append(
                    $" desired={element.DesiredSize} " +
                    $"actual={element.ActualWidth:0.##}x" +
                    $"{element.ActualHeight:0.##} " +
                    $"visibility={element.Visibility}");
            }

            builder.AppendLine();
            for (var index = 0;
                 index < VisualTreeHelper.GetChildrenCount(current);
                 index++)
            {
                if (VisualTreeHelper.GetChild(current, index) is { } child)
                {
                    Append(child, depth + 1);
                }
            }
        }
    }

    private static T? FindDescendant<T>(DependencyObject root)
        where T : DependencyObject
    {
        for (var index = 0;
             index < VisualTreeHelper.GetChildrenCount(root);
             index++)
        {
            if (VisualTreeHelper.GetChild(root, index) is not { } child)
            {
                continue;
            }

            if (child is T match)
            {
                return match;
            }

            if (FindDescendant<T>(child) is { } descendant)
            {
                return descendant;
            }
        }

        return null;
    }

    private static int CountDescendants<T>(DependencyObject root)
        where T : DependencyObject
    {
        var count = 0;
        for (var index = 0;
             index < VisualTreeHelper.GetChildrenCount(root);
             index++)
        {
            if (VisualTreeHelper.GetChild(root, index) is not { } child)
            {
                continue;
            }

            if (child is T)
            {
                count++;
            }

            count += CountDescendants<T>(child);
        }

        return count;
    }

    // Match a displayed logical owner without creating AppKit windows on an xUnit
    // worker. Native popup presentation is exercised separately by the Gallery UI.
    private sealed class DisplayedTestWindow : Window
    {
        public DisplayedTestWindow()
        {
            if (OperatingSystem.IsMacOS())
            {
                typeof(Window).GetMethod("SetMacOSVisibilityForDisplay",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                    .Invoke(this, [Visibility.Visible]);
            }
        }
    }

    private static void ResetApplicationState()
    {
        var currentField = typeof(Application).GetField(
            "_current",
            BindingFlags.NonPublic | BindingFlags.Static);
        currentField?.SetValue(null, null);

        var resetMethod = typeof(ThemeManager).GetMethod(
            "Reset",
            BindingFlags.NonPublic | BindingFlags.Static);
        resetMethod?.Invoke(null, null);
    }

    private readonly record struct PublishedTreeSelectorState(
        string Key,
        string? Label,
        int SelectedCount,
        int CheckedCount);

    private sealed class RecordingTreeSelectorStateSink
        : ITreeSelectorStateSink
    {
        public PublishedTreeSelectorState LastState { get; private set; }

        public void UpdateTreeSelectorState(
            string key,
            string? label,
            int selectedCount,
            int checkedCount) =>
            LastState = new PublishedTreeSelectorState(
                key,
                label,
                selectedCount,
                checkedCount);
    }
}
