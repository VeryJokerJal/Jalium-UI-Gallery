using Jalium.UI.Gallery.Modules.Main.Localization;
using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class MainWindowResponsiveFixture
{
    [Fact]
    public void Loaded_UsesRequestedViewportUntilRootArrangeCatchesUp()
    {
        var window = new MainWindow();
        window.UpdateViewport(1500);
        window.Measure(new Size(1500, 920));
        window.Arrange(new Rect(0, 0, 1500, 920));
        window.Width = 390;

        try
        {
            typeof(FrameworkElement).GetMethod("SetLoadedState", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(window, [true]);
            Assert.True(window.IsMobileShell);
            Assert.False(window.IsShowTitleBar);
            Assert.Equal(ScrollBarVisibility.Auto,
                GetField<ScrollViewer>(window, "ContentHost").VerticalScrollBarVisibility);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void NativeResize_TransfersScrollOwnershipBeforeTheNextArrange()
    {
        var window = new MainWindow();
        var host = GetField<ScrollViewer>(window, "ContentHost");
        var search = GetField<Grid>(window, "TitleBarSearchContainer");
        window.UpdateViewport(1500);
        window.Measure(new Size(1500, 920));
        window.Arrange(new Rect(0, 0, 1500, 920));
        var acceptNativeSize = typeof(Window).GetMethod(
            "OnSizeChanged", BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, types: [typeof(int), typeof(int)], modifiers: null);
        Assert.NotNull(acceptNativeSize);

        try
        {
            foreach (int width in new[] { 1090, 1500, 390, 1500 })
            {
                // Native resize publishes the accepted size before layout updates
                // ActualWidth. The last resize packet must be sufficient by itself.
                acceptNativeSize.Invoke(window, [width, 780]);
                Assert.Equal(width, window.Width);
                Assert.Equal(780, window.Height);
                Assert.Equal(width == 1500 ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto,
                    host.VerticalScrollBarVisibility);
                Assert.Equal(width < 720, window.IsMobileShell);
                if (width == 1090) Assert.Equal(330, search.Width);
                window.Measure(new Size(width, 780));
                window.Arrange(new Rect(0, 0, width, 780));
            }
        }
        finally { window.Close(); }
    }

    [Theory]
    [InlineData(360, true)]
    [InlineData(411, true)]
    [InlineData(719, true)]
    [InlineData(720, false)]
    [InlineData(1280, false)]
    public void ShellBreakpoint_MatchesPortraitPhoneWidths(double width, bool expectedMobile)
    {
        var window = new GalleryWindow();
        try
        {
            window.UpdateViewport(width);
            Assert.Equal(expectedMobile, window.IsMobileShell);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void ApplyResponsiveShell_SwitchesBetweenMobileChromeAndDesktopSidebar()
    {
        var window = new MainWindow();
        var navigation = GetField<NavigationView>(window, "NavigationView");
        var mobileHeader = GetField<Border>(window, "MobileHeader");
        var mobileBottomBar = GetField<Border>(window, "MobileBottomBar");
        var rootShellGrid = GetField<Grid>(window, "RootShellGrid");
        var contentHost = GetField<ScrollViewer>(window, "ContentHost");
        var contentFrame = GetField<Presentation.GalleryNavigationHost>(window, "ContentFrame");

        Assert.Equal(
            FillViewportMode.Vertical,
            ScrollViewer.GetFillViewport(contentFrame));

        window.UpdateViewport(411);

        Assert.True(window.IsMobileShell);
        Assert.False(window.IsShowTitleBar);
        Assert.False(navigation.IsPaneOpen);
        Assert.Equal(0, navigation.CompactPaneLength);
        Assert.Equal(Visibility.Visible, mobileHeader.Visibility);
        Assert.Equal(Visibility.Visible, mobileBottomBar.Visibility);
        Assert.Equal(new Thickness(0), contentHost.Padding);
        Assert.Equal(new Thickness(14, 16, 14, 24), contentFrame.Padding);
        Assert.Equal(ScrollBarVisibility.Disabled, contentHost.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Auto, contentHost.VerticalScrollBarVisibility);
        Assert.Equal(1, Grid.GetRow(navigation));
        Assert.Equal(1, Grid.GetRowSpan(navigation));
        Assert.Same(rootShellGrid, mobileBottomBar.VisualParent);
        Assert.Equal(2, Grid.GetRow(mobileBottomBar));
        Assert.Equal(VerticalAlignment.Bottom, mobileBottomBar.VerticalAlignment);
        Assert.Equal(new Thickness(14, 8, 14, 14), mobileBottomBar.Margin);
        Assert.Equal(new CornerRadius(34), mobileBottomBar.CornerRadius);
        Assert.Equal(80, Panel.GetZIndex(mobileBottomBar));
        if (SystemParameters.UIEffects && !SystemParameters.HighContrast)
        {
            Assert.IsType<DropShadowEffect>(mobileBottomBar.Effect);
            Assert.IsType<FrostedGlassEffect>(mobileBottomBar.BackdropEffect);
            Assert.IsType<FrostedGlassEffect>(mobileHeader.BackdropEffect);
        }
        else
        {
            Assert.Null(mobileBottomBar.Effect);
            Assert.Null(mobileBottomBar.BackdropEffect);
            Assert.Null(mobileHeader.BackdropEffect);
        }

        window.UpdateViewport(1280);

        Assert.False(window.IsMobileShell);
        Assert.True(window.IsShowTitleBar);
        Assert.True(navigation.IsPaneOpen);
        Assert.Equal(Visibility.Collapsed, mobileHeader.Visibility);
        Assert.Equal(Visibility.Collapsed, mobileBottomBar.Visibility);
        Assert.Equal(new Thickness(0), contentHost.Padding);
        Assert.Equal(new Thickness(24, 22, 24, 28), contentFrame.Padding);
        Assert.Equal(ScrollBarVisibility.Disabled, contentHost.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Auto, contentHost.VerticalScrollBarVisibility);
        Assert.Equal(0, Grid.GetRow(navigation));
        Assert.Equal(3, Grid.GetRowSpan(navigation));

        window.UpdateViewport(1500);

        Assert.Equal(
            ScrollBarVisibility.Disabled,
            contentHost.VerticalScrollBarVisibility);

        window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty,
            GalleryComponentCatalog.Items[0].PageTag);

        Assert.Equal(
            ScrollBarVisibility.Auto,
            contentHost.VerticalScrollBarVisibility);

        window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty, "home");

        Assert.Equal(
            ScrollBarVisibility.Disabled,
            contentHost.VerticalScrollBarVisibility);
        window.Close();
    }

    [Fact]
    public void MobilePrimaryNavigation_UsesCompactHeaderAwayFromCatalog()
    {
        var window = new MainWindow();
        var language = LocalizationManager.CurrentLanguage;
        try
        {
            LocalizationManager.SetLanguage(GalleryLanguage.English);
            var headerLayout = GetField<Grid>(window, "MobileHeaderLayout");
            var searchPanel = GetField<Grid>(window, "MobileSearchPanel");
            var pageTitle = GetField<TextBlock>(window, "MobilePageTitle");

            window.UpdateViewport(411);
            window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty,
                "design-tokens");

            Assert.Equal("Design Tokens", pageTitle.Text);
            Assert.Equal(Visibility.Collapsed, searchPanel.Visibility);
            Assert.False(window.IsCatalogPage);
            Assert.Equal(2, headerLayout.RowDefinitions.Count);

            window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty, "home");

            Assert.Equal("Components", pageTitle.Text);
            Assert.Equal(Visibility.Visible, searchPanel.Visibility);
            Assert.True(window.IsCatalogPage);
            Assert.Equal(42, searchPanel.Height);
            Assert.Equal(new Thickness(0, 10, 0, 0), searchPanel.Margin);
        }
        finally { window.Close(); LocalizationManager.SetLanguage(language); }
    }

    [Fact]
    public void DesktopBackButton_ShowsOnDetailPagesOnly()
    {
        var window = new MainWindow();
        var desktopBack = GetField<Button>(window, "DesktopBackButton");
        var componentTag = GalleryComponentCatalog.Items[0].PageTag;

        window.UpdateViewport(1280);
        window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty, componentTag);

        // A component detail page opened from the catalog is the case the desktop
        // shell had no way back from.
        Assert.Equal(Visibility.Visible, desktopBack.Visibility);

        window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty, "design-tokens");

        // Primary navigation roots are reachable from the pane, so no back chrome.
        Assert.Equal(Visibility.Collapsed, desktopBack.Visibility);

        window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty, componentTag);

        Assert.Equal(Visibility.Visible, desktopBack.Visibility);
        Assert.Null(GetField<NavigationView>(window, "NavigationView").SelectedItem);
        Assert.All(GetField<NavigationView>(window, "NavigationView").MenuItems.OfType<NavigationViewItem>(),
            item => Assert.False(item.IsSelected));
        window.Close();
    }

    [Fact]
    public void DesktopBackButton_StaysHiddenInMobileShell()
    {
        var window = new MainWindow();
        var desktopBack = GetField<Button>(window, "DesktopBackButton");
        var mobileBack = GetField<Button>(window, "MobileBackButton");
        var componentTag = GalleryComponentCatalog.Items[0].PageTag;

        window.UpdateViewport(411);
        window.SetCurrentValue(GalleryWindow.CurrentPageTagProperty, componentTag);

        // The mobile header owns the back affordance at this width; showing both
        // would double it up in the same shell.
        Assert.Equal(Visibility.Visible, mobileBack.Visibility);
        Assert.Equal(Visibility.Collapsed, desktopBack.Visibility);

        window.UpdateViewport(1280);

        Assert.Equal(Visibility.Visible, desktopBack.Visibility);
        window.Close();
    }

    [Fact]
    public void MobileNavigationPalette_HasDistinctLightAndDarkMaterials()
    {
        var initial = GalleryTheme.CurrentMode;
        var resources = new ResourceDictionary();
        GalleryTheme.RegisterResources(resources);
        try
        {
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;
            var surface = Assert.IsType<SolidColorBrush>(resources["GalleryMobileSelectedSurface"]);
            var border = Assert.IsType<SolidColorBrush>(resources["GalleryMobileSelectedBorder"]);
            Assert.Equal(Color.FromArgb(0xF2, 0xD9, 0xEF, 0xEC), surface.Color);
            Assert.Equal(Color.FromArgb(0x66, 8, 0x94, 0x8A), border.Color);
            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;
            Assert.Same(surface, resources["GalleryMobileSelectedSurface"]);
            Assert.Equal(Color.FromArgb(0xF5, 5, 8, 10), surface.Color);
            Assert.Equal(Color.FromArgb(0x38, 255, 255, 255), border.Color);
        }
        finally { GalleryTheme.CurrentMode = initial; }
    }

    [Theory]
    [InlineData(360, Visibility.Collapsed)]
    [InlineData(479, Visibility.Collapsed)]
    [InlineData(480, Visibility.Visible)]
    [InlineData(900, Visibility.Visible)]
    public void DesignTokensHeader_HidesThemeSegmentsAtCompactWidths(
        double width,
        Visibility expected)
    {
        var page = new Jalium.UI.Gallery.Modules.Main.Views.Pages.DesignTokensPage();
        page.Measure(new Size(width, 1200));
        page.Arrange(new Rect(0, 0, width, 1200));
        var themeActions = Assert.IsType<StackPanel>(page.FindName("ThemeActions"));
        Assert.Equal(expected, themeActions.Visibility);
        page.Measure(new Size(900, 1200));
        page.Arrange(new Rect(0, 0, 900, 1200));
        Assert.Equal(Visibility.Visible, themeActions.Visibility);
    }

    private static T GetField<T>(object owner, string name)
        where T : class
    {
        var window = Assert.IsAssignableFrom<Jalium.UI.Gallery.Modules.Main.Controls.GalleryWindow>(owner);
        return Assert.IsAssignableFrom<T>(window.FindPart(name));
    }
}
