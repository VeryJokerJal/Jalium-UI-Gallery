using System.ComponentModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Headless;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryWindowTemplateFixture
{
    [Fact]
    public void DetachedShellLoadsItsLogoAndPreservesACustomSource()
    {
        var window = new MainWindow();
        DesignerProperties.SetIsInDesignMode(window, true);
        try
        {
            GalleryHeadlessSupport.PrepareShell(window, "home", 390);
            Assert.NotNull(window.LogoSource);
            Assert.Equal(nint.Zero, window.Handle);

            var customLogo = new BitmapImage();
            window.LogoSource = customLogo;
            GalleryHeadlessSupport.PrepareShell(window, "button", 390);
            Assert.Same(customLogo, window.LogoSource);
            Assert.Equal(nint.Zero, window.Handle);
        }
        finally { GalleryHeadlessSupport.ReleaseShell(window); }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DialogKeysCannotNavigateOrOpenTheShellSearch(bool searchShortcut)
    {
        var window = new GalleryWindow { Width = 390, Height = 844, CurrentPageTag = "contentdialog" };
        DesignerProperties.SetIsInDesignMode(window, true);
        var button = new Button { Content = "Cancel" };
        var dialog = new ContentDialog { Visibility = Visibility.Visible };
        var dialogTemplate = new ControlTemplate(typeof(ContentDialog));
        dialogTemplate.SetVisualTree(() => button);
        dialog.Template = dialogTemplate;
        var shellTemplate = new ControlTemplate(typeof(GalleryWindow));
        shellTemplate.SetVisualTree(() => dialog);
        window.Template = shellTemplate;
        try
        {
            window.Measure(new Size(390, 844));
            window.Arrange(new Rect(0, 0, 390, 844));
            Assert.True(window.HasBackNavigation);
            var args = new KeyEventArgs(searchShortcut ? UIElement.PreviewKeyDownEvent : UIElement.KeyDownEvent,
                searchShortcut ? Key.K : Key.Escape, searchShortcut ? ModifierKeys.Control : ModifierKeys.None, true, false, 1);
            button.RaiseEvent(args);
            Assert.Equal("contentdialog", window.CurrentPageTag);
            Assert.False(args.Handled);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void WindowSizeAndLogoAreSuppliedByTheDefaultStyleAndBindableState()
    {
        var window = new GalleryWindow();
        try
        {
            Assert.Equal(1500, window.Width);
            Assert.Equal(920, window.Height);
            var image = new BitmapImage();
            window.LogoSource = image;
            // The test has no Application/theme initialization. Supply the real
            // NavigationView style so PaneHeader is projected into the visual tree.
            using var stream = new MemoryStream();
            var defaults = ThemeManager.XamlLoader!(stream, "Themes/Generic.jalxaml", typeof(Control).Assembly)!;
            Assert.IsType<NavigationView>(window.FindPart("NavigationView")).Style = Assert.IsType<Style>(defaults[typeof(NavigationView)]);
            window.Measure(new Size(1500, 920));
            window.Arrange(new Rect(0, 0, 1500, 920));
            Assert.Same(image, Assert.IsType<Image>(window.FindPart("PaneLogoImage")).Source);
            Assert.Same(image, Assert.IsType<Image>(window.FindPart("MobileLogoImage")).Source);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void MissingPartsStillExposeBindableViewportAndPageState()
    {
        var window = new GalleryWindow();
        try
        {
            var template = new ControlTemplate(typeof(GalleryWindow));
            template.SetVisualTree(() => new Border());
            window.Template = template;
            window.ApplyTemplate();
            window.UpdateViewport(390);
            window.CurrentPageTag = " THEME-STUDIO ";
            Assert.True(window.IsMobileShell);
            Assert.Equal("theme-studio", window.CurrentPageTag);
            Assert.Equal("theme-studio", window.ActiveNavigationTag);
            Assert.False(window.HasBackNavigation);
            Assert.False(window.IsCatalogPage);
            Assert.False(window.FocusSearch());
            window.CurrentPageTag = "button";
            Assert.True(window.HasBackNavigation);
            Assert.Equal("home", window.ActiveNavigationTag);
            window.Template = null;
            window.UpdateViewport(1500);
            Assert.False(window.IsMobileShell);
            Assert.True(window.HasDesktopBackNavigation);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void RetiredNavigationCannotInvokeCommandsAfterTemplateReplacement()
    {
        var window = new GalleryWindow();
        var calls = new List<string?>();
        window.NavigateCommand = new Jalium.UI.Gallery.Core.Mvvm.RelayCommand<string>(calls.Add);
        try
        {
            var oldNavigation = Assert.IsType<NavigationView>(window.FindPart("NavigationView"));
            var original = window.Template;
            window.Template = null;
            oldNavigation.SelectedItem = oldNavigation.MenuItems.OfType<NavigationViewItem>().Last();
            Assert.Empty(calls);
            window.Template = original;
            window.ApplyTemplate();
            var navigation = Assert.IsType<NavigationView>(window.FindPart("NavigationView"));
            Assert.NotSame(oldNavigation, navigation);
            navigation.SelectedItem = navigation.MenuItems.OfType<NavigationViewItem>().Last();
            Assert.Equal(new[] { "theme-studio" }, calls);
            window.ApplyTemplate();
            navigation.SelectedItem = navigation.MenuItems.OfType<NavigationViewItem>().First();
            Assert.Equal(new[] { "theme-studio", "home" }, calls);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void EscapePreservesTheTwoWayPageBindingAndObeysTheNavigationCommand()
    {
        var window = new GalleryWindow();
        var model = new PageModel { Tag = "button" };
        window.SetBinding(GalleryWindow.CurrentPageTagProperty,
            new Binding(nameof(PageModel.Tag)) { Source = model, Mode = BindingMode.TwoWay });
        try
        {
            var key = Escape(window);
            Assert.True(key.Handled);
            Assert.Equal("home", model.Tag);
            Assert.NotNull(window.GetBindingExpression(GalleryWindow.CurrentPageTagProperty));
            model.Tag = "button";
            window.NavigateCommand = new Jalium.UI.Gallery.Core.Mvvm.RelayCommand<string>(_ => throw new InvalidOperationException(), _ => false);
            Escape(window);
            Assert.Equal("button", window.CurrentPageTag);
        }
        finally { Keyboard.ClearFocus(); window.Close(); }
    }

    [Theory]
    [InlineData("design-tokens")]
    [InlineData("theme-studio")]
    [InlineData("button")]
    public void MobileSearchReturnsToTheCatalogFromEveryOtherDestination(string pageTag)
    {
        var window = new GalleryWindow();
        try
        {
            DesignerProperties.SetIsInDesignMode(window, true);
            window.UpdateViewport(390);
            window.CurrentPageTag = pageTag;
            window.FocusSearch();
            Assert.Equal("home", window.CurrentPageTag);
            Assert.True(window.IsCatalogPage);
            Assert.Equal(Visibility.Visible, Assert.IsType<Grid>(window.FindPart("MobileSearchPanel")).Visibility);
            Assert.True(Assert.IsType<TextBox>(window.FindPart("MobileSearchBox")).IsKeyboardFocused);
        }
        finally { Keyboard.ClearFocus(); window.Close(); }
    }

    [Fact]
    public void MaterialCanBeDisabledAndRestoredAcrossThemeChanges()
    {
        var initial = GalleryTheme.CurrentMode;
        var window = new GalleryWindow();
        try
        {
            window.UpdateViewport(390);
            window.EnableMaterialEffects = false;
            var bar = Assert.IsType<Border>(window.FindPart("MobileBottomBar"));
            Assert.Null(bar.BackdropEffect);
            Assert.Null(bar.Effect);
            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;
            Assert.Null(bar.BackdropEffect);
            Assert.Equal("Solid", VisualStateManager.GetCurrentStateName(window, "MaterialStates"));
            window.EnableMaterialEffects = true;
            if (SystemParameters.UIEffects && !SystemParameters.HighContrast && !OperatingSystem.IsAndroid())
                Assert.IsType<FrostedGlassEffect>(bar.BackdropEffect);
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;
            Assert.Equal("Light", VisualStateManager.GetCurrentStateName(window, "ThemeStates"));
        }
        finally { window.Close(); GalleryTheme.CurrentMode = initial; }
    }

    [Fact]
    public void NavigationButtonsKeepSelectionIndependentAcrossInstancesAndTemplates()
    {
        var first = new GalleryNavigationButton { Destination = "home", SelectedDestination = "home" };
        var second = new GalleryNavigationButton { Destination = "theme-studio", SelectedDestination = "home" };
        first.Measure(new Size(200, 60));
        second.Measure(new Size(200, 60));
        Assert.True(first.IsSelected);
        Assert.False(second.IsSelected);
        Assert.Equal("Selected", VisualStateManager.GetCurrentStateName(first, "SelectionStates"));
        var retired = Assert.IsType<Border>(first.FindName("PART_Surface"));
        var original = first.Template;
        first.Template = null;
        first.SelectedDestination = "theme-studio";
        first.Template = original;
        first.ApplyTemplate();
        Assert.False(first.IsSelected);
        Assert.NotSame(retired, first.FindName("PART_Surface"));
        Assert.Equal("Unselected", VisualStateManager.GetCurrentStateName(first, "SelectionStates"));
        Assert.False(second.IsSelected);
    }

    [Fact]
    public void OrientationRefreshIsDeferredCoalescedAndCanceledWhenTheWindowCloses()
    {
        var window = new GalleryWindow();
        var refresh = typeof(GalleryWindow).GetMethod("OnShellOrientationChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        try
        {
            DesignerProperties.SetIsInDesignMode(window, true);
            window.Width = 390;
            window.UpdateViewport(1500);
            refresh.Invoke(window, [window, EventArgs.Empty]);
            refresh.Invoke(window, [window, EventArgs.Empty]);
            Assert.False(window.IsMobileShell);
            window.Dispatcher.ProcessQueue();
            Assert.True(window.IsMobileShell);
            window.Width = 1500;
            refresh.Invoke(window, [window, EventArgs.Empty]);
            window.Close();
            window.Dispatcher.ProcessQueue();
            Assert.True(window.IsMobileShell);
        }
        finally { window.Close(); }
    }

    private static KeyEventArgs Escape(GalleryWindow window)
    {
        var key = new KeyEventArgs(UIElement.KeyDownEvent, Key.Escape, ModifierKeys.None, false, true, 1);
        window.RaiseEvent(key);
        return key;
    }
    private sealed class PageModel : INotifyPropertyChanged
    {
        private string _tag = "home";
        public string Tag { get => _tag; set { _tag = value; PropertyChanged?.Invoke(this, new(nameof(Tag))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
