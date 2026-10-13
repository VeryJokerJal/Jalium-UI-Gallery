using System.Reflection;
using System.Text;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
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
        var selector = new TreeSelector();

        TreeSelectorBehavior.SetNodes(selector, new[] { rootNode });

        Assert.Null(selector.ItemsSource);
        var root = Assert.IsType<TreeSelectorItem>(
            Assert.Single(selector.Items.Cast<object>()));
        var city = Assert.IsType<TreeSelectorItem>(
            Assert.Single(root.Items.Cast<object>()));
        var leaf = Assert.IsType<TreeSelectorItem>(
            Assert.Single(city.Items.Cast<object>()));

        Assert.Equal("华东地区", root.Header);
        Assert.Equal("上海", city.Header);
        Assert.Equal("浦东新区", leaf.Header);
        Assert.True(root.IsExpanded);
        Assert.True(city.IsExpanded);
        Assert.Same(rootNode, root.DataContext);
        Assert.Same(cityNode, city.DataContext);
        Assert.Same(leafNode, leaf.DataContext);

        TreeSelectorBehavior.SetNodes(selector, null);

        Assert.Empty(selector.Items);
    }

    [Fact]
    public void SelectionEvent_PublishesSelectedAndCheckedCountsSeparately()
    {
        var sink = new RecordingTreeSelectorStateSink();
        var selector = new TreeSelector
        {
            DataContext = sink,
            SelectionMode = SelectionMode.Multiple
        };
        TreeSelectorBehavior.SetStateKey(selector, "multiple");

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

        TreeSelectorBehavior.SetNodes(
            selector,
            new[] { new SelectionTreeNode("华南地区") });

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
        var selector = new TreeSelector
        {
            DataContext = sink,
            SelectionMode = SelectionMode.Multiple,
            ShowCheckBoxes = true,
            CheckCascadeMode = TreeSelectorCheckCascadeMode.Cascade
        };
        TreeSelectorBehavior.SetStateKey(selector, "cascade");
        TreeSelectorBehavior.SetNodes(
            selector,
            new[]
            {
                new SelectionTreeNode(
                    "华东地区",
                    true,
                    new SelectionTreeNode("上海"),
                    new SelectionTreeNode("江苏"))
            });
        var root = Assert.IsType<TreeSelectorItem>(
            Assert.Single(selector.Items.Cast<object>()));

        root.IsChecked = true;

        Assert.All(
            root.Items.Cast<TreeSelectorItem>(),
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
            var window = new Window
            {
                Width = 1000,
                Height = 1400,
                Content = page
            };
            app.MainWindow = window;

            window.Measure(new Size(1000, 1400));
            window.Arrange(new Rect(0, 0, 1000, 1400));
            window.UpdateLayout();

            selector = Assert.IsType<TreeSelector>(
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
            var window = new Window { Width = 1000, Height = 1800, Content = page };
            app.MainWindow = window;
            SettleLayout(window);

            selector = Assert.IsType<TreeSelector>(page.FindName(selectorName));
            popup = Assert.IsType<Popup>(selector.FindName("PART_Popup"));
            popup.ShouldConstrainToRootBounds = true;
            var trigger = Assert.IsType<Border>(selector.FindName("PART_TriggerBorder"));
            var overlay = typeof(Window).GetProperty(
                "OverlayLayer", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
            var dismiss = overlay.GetType().GetMethod("TryHandleLightDismiss")!;
            var closedCount = 0;
            selector.DropDownClosed += (_, _) => closedCount++;
            var selected = Assert.IsType<TreeSelectorItem>(selector.Items[0]).Header!;
            selector.SelectedItems.Add(selected);
            if (selector.ShowCheckBoxes)
            {
                Assert.IsType<TreeSelectorItem>(selector.Items[0]).IsChecked = true;
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
        var field = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
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
