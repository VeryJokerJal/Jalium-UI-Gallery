using System.Runtime.CompilerServices;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

/// <summary>
/// The former responsive code-behind tests now exercise the state that moved
/// into HomeViewModel.
/// </summary>
public sealed class HomePageResponsiveFixture
{
    [Theory]
    [InlineData(1039, 0, 1, 0)]
    [InlineData(1040, 376, 0, 1)]
    public void Workspace_SelectsDetailLayoutDuringFirstMeasure(
        double width,
        double expectedDetailWidth,
        int expectedRow,
        int expectedColumn)
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };

        page.Measure(new Size(width, 800));

        var detail = Assert.IsType<Border>(
            page.FindName("DetailPanel"));
        var detailHost = Assert.IsType<Grid>(
            page.FindName("DetailHost"));
        var detailColumn = Assert.IsType<ColumnDefinition>(
            page.FindName("DetailColumn"));
        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var previewSubtitle = Assert.IsType<TextBlock>(
            page.FindName("CatalogPreviewSubtitle"));
        var directOpenSubtitle = Assert.IsType<TextBlock>(
            page.FindName("CatalogDirectOpenSubtitle"));
        var workspace = Assert.IsType<HomeWorkspaceGrid>(
            page.FindName("HomeWorkspace"));
        var catalog = Assert.IsType<Grid>(
            page.FindName("CatalogPanel"));
        var expectedScrollableHeight = expectedDetailWidth > 0
            ? new GridLength(1, GridUnitType.Star)
            : GridLength.Auto;

        Assert.Equal(
            new GridLength(expectedDetailWidth),
            detailColumn.Width);
        Assert.Equal(expectedRow, Grid.GetRow(detailHost));
        Assert.Equal(expectedColumn, Grid.GetColumn(detailHost));
        Assert.Equal(
            expectedScrollableHeight,
            workspace.RowDefinitions[0].Height);
        Assert.Equal(
            expectedScrollableHeight,
            catalog.RowDefinitions[2].Height);
        Assert.Equal(new Thickness(0), catalog.Margin);
        Assert.Equal(
            expectedDetailWidth > 0
                ? Visibility.Visible
                : Visibility.Collapsed,
            detailHost.Visibility);
        Assert.Equal(
            expectedDetailWidth > 0
                ? Visibility.Visible
                : Visibility.Collapsed,
            previewSubtitle.Visibility);
        Assert.Equal(
            expectedDetailWidth > 0
                ? Visibility.Collapsed
                : Visibility.Visible,
            directOpenSubtitle.Visibility);
        Assert.Equal(
            expectedDetailWidth == 0,
            componentList.IsDirectActivationEnabled);

        if (expectedDetailWidth > 0)
        {
            Assert.Equal(
                new Thickness(16, 0, 0, 0),
                detail.Margin);
            Assert.Equal(
                new Thickness(20, 18, 12, 20),
                detail.Padding);
            Assert.Equal(new Thickness(1), detail.BorderThickness);
            Assert.Equal(new CornerRadius(16), detail.CornerRadius);
        }
        else
        {
            Assert.Equal(
                new Thickness(0, 24, 0, 0),
                detail.Margin);
            Assert.Equal(
                new Thickness(8, 18, 8, 20),
                detail.Padding);
            Assert.Equal(new Thickness(1), detail.BorderThickness);
        }
    }

    [Fact]
    public void Workspace_TracksShellViewportWithoutLaggingHeightFeedback()
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };
        var frame = new Border
        {
            Padding = new Thickness(24, 22, 24, 28),
            Child = page
        };
        var host = new ScrollViewer
        {
            Content = frame,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        ScrollViewer.SetFillViewport(
            frame,
            FillViewportMode.Vertical);

        var workspace = Assert.IsType<HomeWorkspaceGrid>(
            page.FindName("HomeWorkspace"));

        foreach (var height in new double[] { 800, 1000, 720, 800 })
        {
            host.InvalidateMeasure();
            host.Measure(new Size(1400, height));
            host.Arrange(new Rect(0, 0, 1400, height));

            Assert.True(double.IsNaN(workspace.Height));
            Assert.Equal(height - 50, workspace.ActualHeight, 3);
            Assert.Equal(0, host.VerticalOffset, 3);
            Assert.Equal(
                host.ViewportHeight,
                host.ExtentHeight,
                precision: 3);
        }
    }

    [Fact]
    public void SplitWorkspace_OnlyCardListOwnsVerticalScrolling()
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };
        var frame = new Border
        {
            Padding = new Thickness(24, 22, 24, 28),
            Child = page
        };
        var host = new ScrollViewer
        {
            Content = frame,
            VerticalScrollBarVisibility =
                ScrollBarVisibility.Disabled,
            HorizontalScrollBarVisibility =
                ScrollBarVisibility.Disabled
        };
        ScrollViewer.SetFillViewport(
            frame,
            FillViewportMode.Vertical);

        host.Measure(new Size(1400, 800));
        host.Arrange(new Rect(0, 0, 1400, 800));

        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var panel = Assert.IsType<HomeCatalogPanel>(
            FindDescendant<HomeCatalogPanel>(componentList));
        var detail = Assert.IsType<Border>(
            page.FindName("DetailPanel"));
        var filter = Assert.IsType<ScrollViewer>(
            page.FindName("FilterScrollViewer"));
        var cardScroller = Assert.IsType<ScrollViewer>(
            FindDescendant<ScrollViewer>(componentList));
        var detailOrigin = detail.GetAbsolutePosition();
        var filterOrigin = filter.GetAbsolutePosition();

        Assert.Equal(
            ScrollBarVisibility.Disabled,
            host.VerticalScrollBarVisibility);
        Assert.Equal(0, host.VerticalOffset, precision: 3);
        Assert.Equal(
            ScrollBarVisibility.Auto,
            cardScroller.VerticalScrollBarVisibility);
        Assert.False(panel.UseAncestorScrollViewer);
        Assert.InRange(componentList.ActualHeight, 1, 800);
        Assert.InRange(
            cardScroller.ViewportHeight,
            1,
            componentList.ActualHeight);
        Assert.True(
            cardScroller.ExtentHeight >
            cardScroller.ViewportHeight);
        Assert.True(cardScroller.ScrollableHeight > 0);

        cardScroller.ScrollToVerticalOffset(
            Math.Min(240, cardScroller.ScrollableHeight));

        Assert.True(cardScroller.VerticalOffset > 0);
        Assert.Equal(
            detailOrigin,
            detail.GetAbsolutePosition());
        Assert.Equal(
            filterOrigin,
            filter.GetAbsolutePosition());
        Assert.Equal(0, host.VerticalOffset, precision: 3);

        // HomeCatalogPanel intentionally treats non-finite offsets as invalid and coerces them
        // to zero. Scrolling to the catalog end must therefore stay finite and must not jump back
        // to the first card.
        cardScroller.ScrollToBottom();

        Assert.True(cardScroller.VerticalOffset > 0);
        Assert.Equal(
            cardScroller.ScrollableHeight,
            cardScroller.VerticalOffset,
            precision: 3);

        cardScroller.ScrollToTop();
        cardScroller.IsScrollInertiaEnabled = false;
        for (var index = 0; index < 600; index++)
        {
            cardScroller.RaiseEvent(new MouseWheelEventArgs(
                UIElement.MouseWheelEvent,
                new Point(8, 8),
                delta: -120,
                leftButton: MouseButtonState.Released,
                middleButton: MouseButtonState.Released,
                rightButton: MouseButtonState.Released,
                xButton1: MouseButtonState.Released,
                xButton2: MouseButtonState.Released,
                modifiers: ModifierKeys.None,
                timestamp: index + 1));
        }

        Assert.Equal(
            cardScroller.ScrollableHeight,
            cardScroller.VerticalOffset,
            precision: 3);
    }

    [Fact]
    public void StackedWorkspace_TransfersVerticalScrollingToTheShell()
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };
        var frame = new Border
        {
            Padding = new Thickness(24, 22, 24, 28),
            Child = page
        };
        var host = new ScrollViewer
        {
            Content = frame,
            IsScrollInertiaEnabled = false,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility =
                ScrollBarVisibility.Disabled
        };
        ScrollViewer.SetFillViewport(
            frame,
            FillViewportMode.Vertical);

        host.Measure(new Size(900, 800));
        host.Arrange(new Rect(0, 0, 900, 800));

        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var panel = Assert.IsType<HomeCatalogPanel>(
            FindDescendant<HomeCatalogPanel>(componentList));

        Assert.Null(FindDescendant<ScrollViewer>(componentList));
        Assert.True(panel.UseAncestorScrollViewer);
        Assert.True(componentList.IsDirectActivationEnabled);
        Assert.Equal(
            Visibility.Collapsed,
            Assert.IsType<Grid>(page.FindName("DetailHost")).Visibility);
        Assert.True(
            componentList.ActualHeight > host.ViewportHeight);
        Assert.True(host.ScrollableHeight > 0);
        Assert.InRange(
            panel.Children.Count,
            1,
            24);

        var wheel = new MouseWheelEventArgs(
            UIElement.MouseWheelEvent,
            new Point(10, 10),
            delta: -120,
            leftButton: MouseButtonState.Released,
            middleButton: MouseButtonState.Released,
            rightButton: MouseButtonState.Released,
            xButton1: MouseButtonState.Released,
            xButton2: MouseButtonState.Released,
            modifiers: ModifierKeys.None,
            timestamp: 1);
        Assert.NotEmpty(panel.Children);
        panel.Children[0].RaiseEvent(wheel);

        Assert.True(wheel.Handled);
        Assert.True(host.VerticalOffset > 0);
    }

    [Fact]
    public void StackedWorkspace_WidthDragKeepsOneVirtualizedCatalogHost()
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };
        var frame = new Border
        {
            Padding = new Thickness(24, 22, 24, 28),
            Child = page
        };
        var host = new ScrollViewer
        {
            Content = frame,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        ScrollViewer.SetFillViewport(
            frame,
            FillViewportMode.Vertical);

        HomeCatalogPanel? firstPanel = null;
        foreach (var width in new double[]
                 {
                     420,
                     468,
                     536,
                     612,
                     700,
                     584,
                     432
                 })
        {
            host.InvalidateMeasure();
            host.Measure(new Size(width, 800));
            host.Arrange(new Rect(0, 0, width, 800));

            var panel = Assert.IsType<HomeCatalogPanel>(
                FindDescendant<HomeCatalogPanel>(page));
            firstPanel ??= panel;

            Assert.Same(firstPanel, panel);
            Assert.True(panel.UseAncestorScrollViewer);
            Assert.InRange(
                panel.Children.Count,
                1,
                24);
        }
    }

    [Theory]
    [InlineData(PointerDeviceType.Touch)]
    [InlineData(PointerDeviceType.Mouse)]
    [InlineData(PointerDeviceType.Pen)]
    public void StackedWorkspace_TapOpensTheTouchedCardDirectly(
        PointerDeviceType pointerDeviceType)
    {
        var navigation = new Mock<IGalleryNavigationCoordinator>();
        var viewModel = new HomeViewModel(navigation.Object);
        var page = CreateHostedPage(
            viewModel,
            width: 420,
            out _);
        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var expected = HomeCatalog.Items[1];
        var container = Assert.IsType<ListBoxItem>(
            componentList.ItemContainerGenerator.ContainerFromIndex(1));

        RaisePointerTap(container, pointerDeviceType);

        Assert.Same(expected, viewModel.SelectedItem);
        navigation.Verify(
            coordinator => coordinator.RequestNavigation(expected.PageTag),
            Times.Once);
    }

    [Fact]
    public void StackedWorkspace_PanDoesNotOpenACard()
    {
        var navigation = new Mock<IGalleryNavigationCoordinator>();
        var page = CreateHostedPage(
            new HomeViewModel(navigation.Object),
            width: 420,
            out _);
        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var container = Assert.IsType<ListBoxItem>(
            componentList.ItemContainerGenerator.ContainerFromIndex(0));

        RaisePointerGesture(
            container,
            start: new Point(20, 20),
            end: new Point(20, 40));

        navigation.Verify(
            coordinator => coordinator.RequestNavigation(
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void StackedWorkspace_EnterOpensTheSelectedCard()
    {
        var navigation = new Mock<IGalleryNavigationCoordinator>();
        var viewModel = new HomeViewModel(navigation.Object);
        var page = CreateHostedPage(
            viewModel,
            width: 420,
            out _);
        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var expected = HomeCatalog.Items[2];
        componentList.SelectedItem = expected;
        var key = CreateKeyDown(Key.Enter);

        componentList.RaiseEvent(key);

        Assert.True(key.Handled);
        navigation.Verify(
            coordinator => coordinator.RequestNavigation(expected.PageTag),
            Times.Once);
    }

    [Fact]
    public void SplitWorkspace_TapKeepsInspectorSelectionWithoutNavigating()
    {
        var navigation = new Mock<IGalleryNavigationCoordinator>();
        var page = CreateHostedPage(
            new HomeViewModel(navigation.Object),
            width: 1400,
            out _);
        var componentList = Assert.IsType<HomeCatalogListBox>(
            page.FindName("ComponentList"));
        var container = Assert.IsType<ListBoxItem>(
            componentList.ItemContainerGenerator.ContainerFromIndex(1));

        RaisePointerTap(container, PointerDeviceType.Touch);

        Assert.False(componentList.IsDirectActivationEnabled);
        navigation.Verify(
            coordinator => coordinator.RequestNavigation(
                It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData(639, 1)]
    [InlineData(640, 2)]
    [InlineData(779, 2)]
    [InlineData(780, 3)]
    [InlineData(800, 3)]
    public void CatalogPanel_SelectsColumnsDuringFirstMeasure(
        double width,
        int expectedColumns)
    {
        var panel = new HomeCatalogPanel();
        for (var index = 0; index < 6; index++)
        {
            panel.Children.Add(new Border
            {
                Width = 200,
                Height = 198
            });
        }

        panel.Measure(new Size(width, double.PositiveInfinity));

        Assert.Equal(expectedColumns, panel.Columns);
    }

    [Fact]
    public void CatalogPanel_UsesSafeSingleColumnForUnboundedMeasure()
    {
        var panel = new HomeCatalogPanel();
        panel.Children.Add(new Border
        {
            Width = 200,
            Height = 198
        });

        panel.Measure(new Size(
            double.PositiveInfinity,
            double.PositiveInfinity));

        Assert.Equal(1, panel.Columns);
        Assert.False(panel.UseAncestorScrollViewer);
    }

    [Fact]
    public void Workspace_ResizeCommitsCatalogColumnsInOneMeasureTransaction()
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };

        page.Measure(new Size(700, 800));
        page.Arrange(new Rect(0, 0, 700, 800));

        var stackedPanel = Assert.IsType<HomeCatalogPanel>(
            FindDescendant<HomeCatalogPanel>(page));
        Assert.Equal(2, stackedPanel.Columns);
        Assert.True(stackedPanel.UseAncestorScrollViewer);

        // A real resize starts a new measure transaction. One transaction must
        // be enough to commit both the new responsive item width and its wrap
        // count; a render-time correction must not be required.
        page.InvalidateMeasure();
        page.Measure(new Size(1400, 800));
        page.Arrange(new Rect(0, 0, 1400, 800));

        var splitPanel = Assert.IsType<HomeCatalogPanel>(
            FindDescendant<HomeCatalogPanel>(page));
        Assert.NotSame(stackedPanel, splitPanel);
        Assert.Equal(3, splitPanel.Columns);
        Assert.False(splitPanel.UseAncestorScrollViewer);
        Assert.True(
            splitPanel.ActualWidth >= HomeCatalogPanel.ThreeColumnWidth);
    }

    [Fact]
    public void GeneratedCardTemplate_MaterializesAttachedPageTagBinding()
    {
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };

        var exception = Record.Exception(
            () => page.Measure(new Size(1280, 800)));

        Assert.Null(exception);
    }

    [Fact]
    public void Jalxaml_PreservesOriginalComponentWorkbenchVisualContract()
    {
        var repositoryRoot = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Views",
            "Pages",
            "HomePage.jalxaml"));

        // The heading moved to localization resources; the contract is that the header
        // block still binds the localized title and subtitle.
        Assert.Contains("Text=\"{DynamicResource Loc.Home.Title}\"", source);
        Assert.Contains("Text=\"{DynamicResource Loc.Home.Subtitle}\"", source);
        Assert.Contains(
            "Text=\"{DynamicResource Loc.Home.DirectOpenSubtitle}\"",
            source);
        Assert.Contains("HomeFilterButton", source);
        Assert.Contains(
            "Content=\"{DynamicResource Loc.Category.Styling}\"",
            source);
        Assert.Contains(
            "ConverterParameter=Styling",
            source);
        Assert.Contains(
            "CommandParameter=\"Styling\"",
            source);
        Assert.Contains(
            "<presentation:HomeWorkspaceGrid",
            source);
        Assert.Contains(
            "x:Name=\"FilterScrollViewer\"",
            source);
        Assert.Contains(
            "HorizontalScrollBarVisibility=\"Auto\"",
            source);
        Assert.Contains(
            "ScrollViewer.CanContentScroll=\"True\"",
            source);
        Assert.Contains(
            "x:Name=\"DetailScrollViewer\"",
            source);
        Assert.DoesNotContain(
            "<WrapPanel HorizontalAlignment=\"Left\"",
            source);
        Assert.DoesNotContain(
            "HomeCatalogLayoutBehavior.IsEnabled",
            source);
        Assert.Contains(
            "x:Key=\"HomeCatalogPageScrollTemplate\"",
            source);
        Assert.Contains(
            "UseAncestorScrollViewer=\"True\"",
            source);
        Assert.Contains(
            "Property=\"Height\" Value=\"198\"",
            source);
        Assert.Contains("SaasCardImageBehavior.PageTag", source);
        Assert.Contains("Name=\"SelectedBadge\"", source);
        Assert.Matches(
            """<Grid\s+Grid\.Row="1"\s+Background="Transparent"\s+Margin="12,0">""",
            source);

        var cardStyleStart = source.IndexOf(
            "<Style x:Key=\"HomeComponentCardItem\"",
            StringComparison.Ordinal);
        var cardStyleEnd = source.IndexOf(
            "</Style>",
            cardStyleStart,
            StringComparison.Ordinal);
        Assert.True(cardStyleStart >= 0 && cardStyleEnd > cardStyleStart);
        var cardStyle = source[cardStyleStart..cardStyleEnd];
        Assert.DoesNotContain(
            "<Setter Property=\"Cursor\" Value=\"Hand\" />",
            cardStyle);
        Assert.Contains("x:Name=\"DetailPanel\"", source);
        Assert.Contains("x:Name=\"DetailHost\"", source);
        Assert.Contains("<presentation:HomeCatalogListBox", source);
        Assert.Contains(
            "ActivateItemCommand=\"{Binding OpenItemCommand}\"",
            source);
        Assert.Contains(
            "Style=\"{StaticResource GalleryPrimaryButton}\"",
            source);
        Assert.Contains("Text=\"{DynamicResource Loc.Home.MarkupHeading}\"", source);
        Assert.Contains("Text=\"{DynamicResource Loc.Home.TokensHeading}\"", source);
        Assert.DoesNotContain("Text=\"Component catalog\"", source);
        Assert.DoesNotContain("Content=\"Open best match\"", source);
    }

    [Fact]
    public void SearchQuery_FiltersCatalogAndKeepsSelectionValid()
    {
        var viewModel = CreateViewModel();

        viewModel.SearchQuery = "QRCode";

        var item = Assert.Single(viewModel.VisibleItems);
        Assert.Equal("qrcode", item.PageTag);
        Assert.Same(item, viewModel.SelectedItem);
        Assert.Equal(item.Key, viewModel.SelectedItemKey);
        Assert.Equal("1 component", viewModel.ResultCountText);
    }

    [Fact]
    public void CategoryCommand_FiltersThroughBindableState()
    {
        var viewModel = CreateViewModel();

        viewModel.SetFilterCommand.Execute("Visuals");

        Assert.Equal("Visuals", viewModel.ActiveFilter);
        Assert.NotEmpty(viewModel.VisibleItems);
        Assert.All(
            viewModel.VisibleItems,
            item => Assert.Equal("Visuals", item.Category));
    }

    [Fact]
    public void CssCategory_FiltersTheStylingCatalogPages()
    {
        var viewModel = CreateViewModel();

        viewModel.SetFilterCommand.Execute("Styling");

        Assert.Equal("Styling", viewModel.ActiveFilter);
        Assert.Equal(5, viewModel.VisibleItems.Count);
        Assert.All(
            viewModel.VisibleItems,
            item => Assert.Equal("Styling", item.Category));
        Assert.Contains(
            viewModel.VisibleItems,
            item => item.PageTag == "css");
    }

    [Fact]
    public void DeviceCommand_UpdatesPreviewTarget()
    {
        var viewModel = CreateViewModel();

        viewModel.SetDeviceCommand.Execute("Mobile");

        Assert.Equal("Mobile", viewModel.ActiveDevice);
        Assert.Contains("Mobile", viewModel.DeviceOptions);
    }

    [Fact]
    public void DetailedPreviews_FitInsideTheMobileContentArea()
    {
        var availableSize = new Size(174, 184);
        var page = new HomePage
        {
            DataContext = CreateViewModel()
        };
        var viewport = Assert.IsType<Border>(
            page.FindName("PreviewViewport"));

        Assert.True(viewport.ClipToBounds);

        foreach (var item in HomeCatalog.Items)
        {
            var preview = new HomeComponentPreview
            {
                Item = item,
                IsDetailed = true,
                Width = availableSize.Width,
                Height = availableSize.Height
            };

            preview.Measure(availableSize);
            preview.Arrange(new Rect(
                0,
                0,
                availableSize.Width,
                availableSize.Height));

            var content = Assert.IsAssignableFrom<FrameworkElement>(
                preview.Content);
            Assert.True(preview.ClipToBounds);
            Assert.True(
                content.ActualWidth <= availableSize.Width,
                $"{item.Title} ({item.PreviewKind}) arranged at " +
                $"{content.ActualWidth:0.##} px inside a " +
                $"{availableSize.Width:0.##} px preview.");
        }
    }

    [Fact]
    public void CatalogItems_UseLiveControlsOrTheirActualComponentPage()
    {
        var livePreviewKinds = new HashSet<string>(
            StringComparer.Ordinal)
        {
            "buttons",
            "inputs",
            "cards",
            "checkboxes",
            "switches",
            "badges",
            "select",
            "progress",
            "alerts"
        };

        foreach (var item in HomeCatalog.Items)
        {
            var preview = new HomeComponentPreview
            {
                Item = item,
                IsDetailed = true
            };

            Assert.NotNull(preview.Content);
            Assert.False(
                preview.Content is TextBlock,
                $"{item.Title} fell back to a text-only preview.");
            Assert.False(
                preview.Content is Image,
                $"{item.Title} fell back to an image preview.");
            if (livePreviewKinds.Contains(item.PreviewKind))
            {
                continue;
            }

            var actualPreview = Assert.IsType<Viewbox>(
                preview.Content);
            var actualPage = Assert.IsAssignableFrom<Page>(
                actualPreview.Child);
            Assert.Equal(
                item.PageType,
                actualPage.GetType());
            Assert.IsAssignableFrom<FrameworkElement>(
                actualPage.Content);
            Assert.True(actualPreview.ClipToBounds);
        }
    }

    [Fact]
    public void ExpanderPreview_RemovesCardAndDemoSurfaceDecorations()
    {
        var item = Assert.Single(
            HomeCatalog.Items,
            candidate => candidate.PageTag == "expander");
        var preview = new HomeComponentPreview
        {
            Item = item,
            IsDetailed = true
        };

        var actualPreview = Assert.IsType<Viewbox>(preview.Content);
        var actualPage = Assert.IsAssignableFrom<Page>(
            actualPreview.Child);
        var controls = Assert.IsType<StackPanel>(
            actualPage.Content);
        var expanders = controls.Children
            .OfType<Expander>()
            .ToArray();

        Assert.Equal(2, expanders.Length);
        Assert.Contains(
            expanders,
            expander => expander.IsExpanded);
    }

    [Fact]
    public void OpenSelectedCommand_RequestsNavigationFromViewModel()
    {
        var navigation = new Mock<IGalleryNavigationCoordinator>();
        var viewModel = new HomeViewModel(navigation.Object);
        var expected = HomeCatalog.Items[12];
        viewModel.SelectedItem = expected;

        viewModel.OpenSelectedCommand.Execute(null);

        navigation.Verify(
            coordinator => coordinator.RequestNavigation(expected.PageTag),
            Times.Once);
    }

    [Fact]
    public void EmptySearch_DisablesNavigationCommandsUntilResultsReturn()
    {
        var viewModel = CreateViewModel();

        viewModel.SearchQuery = "definitely-not-a-gallery-component";

        Assert.False(viewModel.HasVisibleItems);
        Assert.Empty(viewModel.VisibleItems);
        Assert.False(viewModel.OpenSelectedCommand.CanExecute(null));
        Assert.False(viewModel.OpenBestMatchCommand.CanExecute(null));

        viewModel.SearchQuery = string.Empty;

        Assert.True(viewModel.HasVisibleItems);
        Assert.True(viewModel.OpenSelectedCommand.CanExecute(null));
        Assert.True(viewModel.OpenBestMatchCommand.CanExecute(null));
    }

    private static HomePage CreateHostedPage(
        HomeViewModel viewModel,
        double width,
        out ScrollViewer host)
    {
        var page = new HomePage
        {
            DataContext = viewModel
        };
        var frame = new Border
        {
            Padding = new Thickness(24, 22, 24, 28),
            Child = page
        };
        host = new ScrollViewer
        {
            Content = frame,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        ScrollViewer.SetFillViewport(
            frame,
            FillViewportMode.Vertical);
        host.Measure(new Size(width, 800));
        host.Arrange(new Rect(0, 0, width, 800));
        return page;
    }

    private static void RaisePointerTap(
        UIElement target,
        PointerDeviceType pointerDeviceType) =>
        RaisePointerGesture(
            target,
            start: new Point(20, 20),
            end: new Point(20, 20),
            pointerDeviceType: pointerDeviceType);

    private static KeyEventArgs CreateKeyDown(Key key)
    {
        var constructor = typeof(KeyEventArgs).GetConstructor(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic,
            binder: null,
            types:
            [
                typeof(RoutedEvent),
                typeof(Key),
                typeof(ModifierKeys),
                typeof(bool),
                typeof(bool),
                typeof(int)
            ],
            modifiers: null);
        Assert.NotNull(constructor);
        return Assert.IsType<KeyEventArgs>(constructor.Invoke(
        [
            UIElement.KeyDownEvent,
            key,
            ModifierKeys.None,
            true,
            false,
            1
        ]));
    }

    private static void RaisePointerGesture(
        UIElement target,
        Point start,
        Point end,
        PointerDeviceType pointerDeviceType = PointerDeviceType.Touch)
    {
        const uint pointerId = 1;
        target.RaiseEvent(new PointerDownEventArgs(
            CreatePointer(
                pointerId,
                start,
                isPressed: true,
                updateKind: PointerUpdateKind.LeftButtonPressed,
                timestamp: 1,
                pointerDeviceType: pointerDeviceType),
            ModifierKeys.None,
            timestamp: 1)
        {
            RoutedEvent = UIElement.PointerDownEvent
        });

        if (start != end)
        {
            target.RaiseEvent(new PointerMoveEventArgs(
                CreatePointer(
                    pointerId,
                    end,
                    isPressed: true,
                    updateKind: PointerUpdateKind.Other,
                    timestamp: 2,
                    pointerDeviceType: pointerDeviceType),
                ModifierKeys.None,
                timestamp: 2)
            {
                RoutedEvent = UIElement.PointerMoveEvent
            });
        }

        target.RaiseEvent(new PointerUpEventArgs(
            CreatePointer(
                pointerId,
                end,
                isPressed: false,
                updateKind: PointerUpdateKind.LeftButtonReleased,
                timestamp: 3,
                pointerDeviceType: pointerDeviceType),
            ModifierKeys.None,
            timestamp: 3)
        {
            RoutedEvent = UIElement.PointerUpEvent
        });
    }

    private static PointerPoint CreatePointer(
        uint pointerId,
        Point position,
        bool isPressed,
        PointerUpdateKind updateKind,
        ulong timestamp,
        PointerDeviceType pointerDeviceType) =>
        new(
            pointerId,
            position,
            pointerDeviceType,
            isPressed,
            new PointerPointProperties
            {
                // The cross-platform bridge currently marks pen input as
                // non-primary, while touch and mouse are primary.
                IsPrimary = pointerDeviceType != PointerDeviceType.Pen,
                IsLeftButtonPressed = isPressed,
                PointerUpdateKind = updateKind
            },
            timestamp);

    private static HomeViewModel CreateViewModel() =>
        new(Mock.Of<IGalleryNavigationCoordinator>());

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

    private static string FindRepositoryRoot(
        [CallerFilePath] string sourceFile = "")
    {
        var directory = new DirectoryInfo(
            Path.GetDirectoryName(sourceFile)
            ?? throw new InvalidOperationException(
                "Unable to locate test source."));

        while (directory != null &&
               !File.Exists(Path.Combine(
                   directory.FullName,
                   "Jalium.UI.Gallery.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                "Unable to locate repository root.");
    }
}
