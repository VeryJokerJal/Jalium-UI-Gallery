using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class ThemeColorsPageFixture : IDisposable
{
    private readonly GalleryThemeMode _originalTheme = GalleryTheme.CurrentMode;
    private readonly GalleryLanguage _originalLanguage = LocalizationManager.CurrentLanguage;

    public ThemeColorsPageFixture()
    {
        ResetApplicationState();
        // Containers need the same default templates as the running Gallery;
        // unstyled ListBoxItems have no ContentPresenter and measure as one DIP.
        _ = new App();
    }

    public void Dispose()
    {
        LocalizationManager.SetLanguage(_originalLanguage);
        GalleryTheme.CurrentMode = _originalTheme;
        ResetApplicationState();
    }

    private static void ResetApplicationState()
    {
        typeof(Application).GetField("_current", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
        typeof(ThemeManager).GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
    }

    [Fact]
    public void ViewportHeightBehavior_RetriesAfterLateVisualAttachment()
    {
        var target = new Grid
        {
            Margin = new Thickness(24)
        };
        target.Children.Add(new Border
        {
            Height = 120
        });
        ViewportHeightBehavior.SetIsEnabled(target, true);

        var body = new StackPanel();
        body.Children.Add(target);
        var host = new ScrollViewer
        {
            Content = body,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        Layout(host, width: 1000, height: 720);

        Assert.False(double.IsNaN(target.Height));
        Assert.Equal(
            Math.Max(240, host.ViewportHeight - 48),
            target.Height,
            precision: 3);
    }

    [Fact]
    public void Page_GivesTheResourceListAVisibleViewport()
    {
        var viewModel = CreateViewModel();
        var page = new ThemeColorsPage
        {
            DataContext = viewModel
        };
        var body = new StackPanel();
        body.Children.Add(page);
        var host = new ScrollViewer
        {
            Content = body,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        Layout(host, width: 1100, height: 720);

        var root = Assert.IsType<Grid>(
            page.FindName("ThemeColorsRoot"));
        var list = Assert.IsType<ListBox>(
            page.FindName("ResourceList"));

        Assert.True(
            root.ActualHeight >= 560,
            $"root={root.ActualHeight}, height={root.Height}, " +
            $"page={page.ActualHeight}, pageHeight={page.Height}, " +
            $"hostViewport={host.ViewportHeight}");
        Assert.True(
            list.ActualHeight >= 300,
            $"list={list.ActualHeight}, root={root.ActualHeight}");
        Assert.Equal(3, list.Items.Count);
    }

    [Fact]
    public void Page_VirtualizesTheFullThemeResourceSet()
    {
        const int resourceCount = 566;
        var viewModel = CreateLargeViewModel(resourceCount);
        var page = new ThemeColorsPage
        {
            DataContext = viewModel
        };
        var body = new StackPanel();
        body.Children.Add(page);
        var host = new ScrollViewer
        {
            Content = body,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        Layout(host, width: 1100, height: 720);

        var list = Assert.IsType<ListBox>(
            page.FindName("ResourceList"));
        var panel = Assert.IsType<VirtualizingStackPanel>(
            FindDescendant<VirtualizingStackPanel>(list));
        var realizedCount = Enumerable
            .Range(0, resourceCount)
            .Count(index =>
                list.ItemContainerGenerator
                    .ContainerFromIndex(index) != null);

        Assert.True(VirtualizingPanel.GetIsVirtualizing(list));
        Assert.Equal(
            VirtualizationMode.Recycling,
            VirtualizingPanel.GetVirtualizationMode(list));
        Assert.Equal(resourceCount, list.Items.Count);
        Assert.InRange(realizedCount, 1, 40);
        Assert.Equal(realizedCount, panel.VisualChildrenCount);
    }

    [Fact]
    public void Page_RecoversTheInitialViewportWithoutScrollInput()
    {
        const int resourceCount = 566;
        var page = new ThemeColorsPage
        {
            DataContext = CreateLargeViewModel(resourceCount)
        };

        // This is the workbench's first-frame layout shape: the outer
        // ScrollViewer measures its child without a vertical bound, then the
        // page receives a finite arrange slot.
        page.Measure(
            new Size(1100, double.PositiveInfinity));
        page.Arrange(new Rect(0, 0, 1100, 640));

        var list = Assert.IsType<ListBox>(
            page.FindName("ResourceList"));
        var panel = Assert.IsType<VirtualizingStackPanel>(
            FindDescendant<VirtualizingStackPanel>(list));

        Assert.Single(panel.Children);
        Assert.False(panel.IsMeasureValid);

        // The corrective layout pass is driven by Arrange invalidation, not
        // by a scroll offset change. Headless layout has no LayoutManager, so
        // mirror its upward invalidation propagation explicitly.
        for (Visual? current = panel;
             current is not null;
             current = current.VisualParent)
        {
            if (current is UIElement element)
            {
                element.InvalidateMeasure();
            }
        }
        page.Measure(
            new Size(1100, double.PositiveInfinity));
        page.Arrange(new Rect(0, 0, 1100, 640));

        Assert.InRange(
            panel.Children.Count,
            2,
            resourceCount - 1);
        Assert.NotNull(
            list.ItemContainerGenerator.ContainerFromIndex(1));
        Assert.Null(
            list.ItemContainerGenerator.ContainerFromIndex(500));
    }

    [Fact]
    public void Filters_KeepResourceAndEmptyStateInSync()
    {
        var viewModel = CreateViewModel();

        Assert.True(viewModel.HasResources);
        Assert.False(viewModel.HasNoResources);
        Assert.Equal("3 keys", viewModel.CountText);

        viewModel.ShowGradients = true;

        var gradient = Assert.Single(viewModel.Resources);
        Assert.Equal(
            ThemeResourceKind.GradientBrush,
            gradient.Entry.Kind);

        viewModel.FilterText = "missing";

        Assert.False(viewModel.HasResources);
        Assert.True(viewModel.HasNoResources);
        Assert.Equal("No matching keys.", viewModel.EmptyMessage);
        Assert.Equal("0 / 3 keys", viewModel.CountText);
    }

    private static ThemeColorsViewModel CreateViewModel()
    {
        var resources = new Mock<IGalleryThemeResourceService>();
        resources.SetupGet(service => service.CurrentThemeKey)
            .Returns("Dark");
        resources.Setup(service => service.GetResources("Dark"))
            .Returns(
            [
                new ThemeResourceEntry(
                    "AccentColor",
                    ThemeResourceKind.Color,
                    "#FF08948A",
                    new SolidColorBrush(
                        Color.FromRgb(0x08, 0x94, 0x8A))),
                new ThemeResourceEntry(
                    "AccentBrush",
                    ThemeResourceKind.SolidBrush,
                    "#FF08948A",
                    new SolidColorBrush(
                        Color.FromRgb(0x08, 0x94, 0x8A))),
                new ThemeResourceEntry(
                    "AccentGradient",
                    ThemeResourceKind.GradientBrush,
                    "#FF08948A → #FF5FD4C9",
                    CreateGradient())
            ]);

        return new ThemeColorsViewModel(
            resources.Object,
            Mock.Of<IGalleryClipboardService>());
    }

    private static ThemeColorsViewModel CreateLargeViewModel(
        int resourceCount)
    {
        var preview = new SolidColorBrush(
            Color.FromRgb(0x08, 0x94, 0x8A));
        var entries = Enumerable
            .Range(0, resourceCount)
            .Select(index =>
                new ThemeResourceEntry(
                    $"Resource{index:000}",
                    ThemeResourceKind.SolidBrush,
                    "#FF08948A",
                    preview))
            .ToArray();
        var resources = new Mock<IGalleryThemeResourceService>();
        resources.SetupGet(service => service.CurrentThemeKey)
            .Returns("Dark");
        resources.Setup(service => service.GetResources("Dark"))
            .Returns(entries);

        return new ThemeColorsViewModel(
            resources.Object,
            Mock.Of<IGalleryClipboardService>());
    }

    private static LinearGradientBrush CreateGradient()
    {
        var gradient = new LinearGradientBrush();
        gradient.GradientStops.Add(
            new GradientStop(
                Color.FromRgb(0x08, 0x94, 0x8A),
                0));
        gradient.GradientStops.Add(
            new GradientStop(
                Color.FromRgb(0x5F, 0xD4, 0xC9),
                1));
        return gradient;
    }

    private static void Layout(
        ScrollViewer host,
        double width,
        double height)
    {
        var available = new Size(width, height);
        for (var pass = 0; pass < 3; pass++)
        {
            host.InvalidateMeasure();
            host.Measure(available);
            host.Arrange(new Rect(0, 0, width, height));
            host.UpdateLayout();
        }
    }

    private static T? FindDescendant<T>(Visual root)
        where T : Visual
    {
        for (var index = 0;
             index < root.VisualChildrenCount;
             index++)
        {
            if (root.GetVisualChild(index) is not Visual child)
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

}
