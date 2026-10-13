using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class MainWindowResponsiveFixture
{
    [Theory]
    [InlineData(360, true)]
    [InlineData(411, true)]
    [InlineData(719, true)]
    [InlineData(720, false)]
    [InlineData(1280, false)]
    public void ShellBreakpoint_MatchesPortraitPhoneWidths(double width, bool expectedMobile)
    {
        Assert.Equal(
            expectedMobile,
            GalleryShellBehavior.ShouldUseMobileShell(width));
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
        var contentFrame = GetField<Border>(window, "ContentFrame");

        Assert.Equal(
            FillViewportMode.Vertical,
            ScrollViewer.GetFillViewport(contentFrame));

        GalleryShellBehavior.ApplyResponsiveShell(window, 411);

        Assert.True(GalleryShellBehavior.GetIsMobileShell(window));
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

        GalleryShellBehavior.ApplyResponsiveShell(window, 1280);

        Assert.False(GalleryShellBehavior.GetIsMobileShell(window));
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

        GalleryShellBehavior.ApplyResponsiveShell(window, 1500);

        Assert.Equal(
            ScrollBarVisibility.Disabled,
            contentHost.VerticalScrollBarVisibility);

        GalleryShellBehavior.SetCurrentPageTag(
            window,
            GalleryComponentCatalog.Items[0].PageTag);

        Assert.Equal(
            ScrollBarVisibility.Auto,
            contentHost.VerticalScrollBarVisibility);

        GalleryShellBehavior.SetCurrentPageTag(window, "home");

        Assert.Equal(
            ScrollBarVisibility.Disabled,
            contentHost.VerticalScrollBarVisibility);
        window.Close();
    }

    [Fact]
    public void MobilePrimaryNavigation_UsesCompactHeaderAwayFromCatalog()
    {
        var window = new MainWindow();
        var headerLayout = GetField<Grid>(window, "MobileHeaderLayout");
        var searchPanel = GetField<Grid>(window, "MobileSearchPanel");
        var pageTitle = GetField<TextBlock>(window, "MobilePageTitle");

        GalleryShellBehavior.ApplyResponsiveShell(window, 411);
        GalleryShellBehavior.SetCurrentPageTag(
            window,
            "design-tokens");

        Assert.Equal("Design Tokens", pageTitle.Text);
        Assert.Equal(Visibility.Collapsed, searchPanel.Visibility);
        Assert.Equal(0, headerLayout.RowDefinitions[1].Height.Value);
        Assert.Equal(0, headerLayout.RowDefinitions[2].Height.Value);

        GalleryShellBehavior.SetCurrentPageTag(window, "home");

        Assert.Equal("Components", pageTitle.Text);
        Assert.Equal(Visibility.Visible, searchPanel.Visibility);
        Assert.Equal(10, headerLayout.RowDefinitions[1].Height.Value);
        Assert.Equal(42, headerLayout.RowDefinitions[2].Height.Value);
        window.Close();
    }

    [Fact]
    public void DesktopBackButton_ShowsOnDetailPagesOnly()
    {
        var window = new MainWindow();
        var desktopBack = GetField<Button>(window, "DesktopBackButton");
        var componentTag = GalleryComponentCatalog.Items[0].PageTag;

        GalleryShellBehavior.ApplyResponsiveShell(window, 1280);
        GalleryShellBehavior.SetCurrentPageTag(window, componentTag);

        // A component detail page opened from the catalog is the case the desktop
        // shell had no way back from.
        Assert.Equal(Visibility.Visible, desktopBack.Visibility);

        GalleryShellBehavior.SetCurrentPageTag(window, "design-tokens");

        // Primary navigation roots are reachable from the pane, so no back chrome.
        Assert.Equal(Visibility.Collapsed, desktopBack.Visibility);

        GalleryShellBehavior.SetCurrentPageTag(window, componentTag);

        Assert.Equal(Visibility.Visible, desktopBack.Visibility);
        window.Close();
    }

    [Fact]
    public void DesktopBackButton_StaysHiddenInMobileShell()
    {
        var window = new MainWindow();
        var desktopBack = GetField<Button>(window, "DesktopBackButton");
        var mobileBack = GetField<Button>(window, "MobileBackButton");
        var componentTag = GalleryComponentCatalog.Items[0].PageTag;

        GalleryShellBehavior.ApplyResponsiveShell(window, 411);
        GalleryShellBehavior.SetCurrentPageTag(window, componentTag);

        // The mobile header owns the back affordance at this width; showing both
        // would double it up in the same shell.
        Assert.Equal(Visibility.Visible, mobileBack.Visibility);
        Assert.Equal(Visibility.Collapsed, desktopBack.Visibility);

        GalleryShellBehavior.ApplyResponsiveShell(window, 1280);

        Assert.Equal(Visibility.Visible, desktopBack.Visibility);
        window.Close();
    }

    [Fact]
    public void MobileNavigationPalette_HasDistinctLightAndDarkMaterials()
    {
        var light = GalleryShellBehavior.GetMobileNavigationPalette(
            darkMode: false,
            isAndroid: true);
        var dark = GalleryShellBehavior.GetMobileNavigationPalette(
            darkMode: true,
            isAndroid: true);

        Assert.Equal(Color.FromRgb(0xFB, 0xFC, 0xFC), light.Material);
        Assert.Equal(Color.FromArgb(0xD0, 0xC8, 0xD2, 0xD9), light.Border);
        Assert.Equal(Color.FromArgb(0xF2, 0xD9, 0xEF, 0xEC), light.SelectedSurface);
        Assert.Equal(Color.FromArgb(0x66, 0x08, 0x94, 0x8A), light.SelectedBorder);
        Assert.Equal(Color.FromRgb(0x35, 0x40, 0x52), light.IdleForeground);
        Assert.Equal(Color.FromRgb(0x1B, 0x1F, 0x22), dark.Material);
        Assert.Equal(Color.FromArgb(0xF5, 0x05, 0x08, 0x0A), dark.SelectedSurface);
        Assert.Equal(Color.FromRgb(0xCF, 0xD7, 0xD9), dark.IdleForeground);
        Assert.NotEqual(light.Border, dark.Border);
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
        var root = new Grid();
        var themeActions = new StackPanel
        {
            Name = "ThemeActions"
        };
        root.Children.Add(themeActions);
        root.RegisterName(themeActions.Name, themeActions);

        DesignTokensLayoutBehavior.ApplyResponsiveLayout(root, width);

        Assert.Equal(expected, themeActions.Visibility);
    }

    private static T GetField<T>(object owner, string name)
        where T : class
    {
        var field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        return Assert.IsAssignableFrom<T>(field!.GetValue(owner));
    }
}
