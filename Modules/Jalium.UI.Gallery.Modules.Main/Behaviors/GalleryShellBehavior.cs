using System.Runtime.CompilerServices;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;
using Jalium.UI.Media.Imaging;
using Jalium.UI.Media.Native;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts responsive window chrome, keyboard focus, and platform-specific
/// visuals. Shell navigation state remains bound to MainWindowViewModel.
/// </summary>
public static class GalleryShellBehavior
{
    public const double MobileShellBreakpoint = 720;

    private static readonly ConditionalWeakTable<
        Window,
        ShellController> Controllers = new();

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(GalleryShellBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty CurrentPageTagProperty =
        DependencyProperty.RegisterAttached(
            "CurrentPageTag",
            typeof(string),
            typeof(GalleryShellBehavior),
            new PropertyMetadata(
                "home",
                OnCurrentPageTagChanged));

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    public static void SetCurrentPageTag(
        DependencyObject element,
        string value) =>
        element.SetValue(CurrentPageTagProperty, value);

    public static string GetCurrentPageTag(
        DependencyObject element) =>
        element.GetValue(CurrentPageTagProperty) as string ??
        "home";

    internal static bool ShouldUseMobileShell(double width) =>
        width > 0 && width < MobileShellBreakpoint;

    internal static bool GetIsMobileShell(Window window) =>
        Controllers.TryGetValue(window, out var controller) &&
        controller.IsMobileShell;

    internal static void ApplyResponsiveShell(
        Window window,
        double width)
    {
        var controller = GetOrCreateController(window);
        controller.Start();
        controller.ApplyResponsiveShell(width);
    }

    internal static (
        Color Material,
        Color Border,
        Color SelectedSurface,
        Color SelectedBorder,
        Color IdleForeground) GetMobileNavigationPalette(
        bool darkMode,
        bool isAndroid)
    {
        if (darkMode)
        {
            return (
                isAndroid
                    ? Color.FromRgb(0x1B, 0x1F, 0x22)
                    : Color.FromArgb(
                        0xE8,
                        0x1B,
                        0x1F,
                        0x22),
                Color.FromArgb(0x52, 0xFF, 0xFF, 0xFF),
                Color.FromArgb(0xF5, 0x05, 0x08, 0x0A),
                Color.FromArgb(0x38, 0xFF, 0xFF, 0xFF),
                Color.FromRgb(0xCF, 0xD7, 0xD9));
        }

        return (
            isAndroid
                ? Color.FromRgb(0xFB, 0xFC, 0xFC)
                : Color.FromArgb(0xD9, 0xFB, 0xFC, 0xFC),
            Color.FromArgb(0xD0, 0xC8, 0xD2, 0xD9),
            Color.FromArgb(0xF2, 0xD9, 0xEF, 0xEC),
            Color.FromArgb(0x66, 0x08, 0x94, 0x8A),
            Color.FromRgb(0x35, 0x40, 0x52));
    }

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Window window)
        {
            return;
        }

        if (GetIsEnabled(window))
        {
            window.Loaded -= OnWindowLoaded;
            window.Loaded += OnWindowLoaded;
            if (window.IsLoaded)
            {
                StartController(window);
            }
        }
        else
        {
            window.Loaded -= OnWindowLoaded;
            if (Controllers.TryGetValue(
                    window,
                    out var controller))
            {
                controller.Dispose();
                Controllers.Remove(window);
            }
        }
    }

    private static void OnWindowLoaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is Window window &&
            GetIsEnabled(window))
        {
            StartController(window);
        }
    }

    private static void StartController(Window window)
    {
        var controller = GetOrCreateController(window);
        controller.Start();
        controller.ApplyResponsiveShell(window.ActualWidth);
    }

    private static void OnCurrentPageTagChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is Window window &&
            args.NewValue is string pageTag &&
            !string.IsNullOrWhiteSpace(pageTag) &&
            Controllers.TryGetValue(window, out var controller))
        {
            controller.UpdatePageContext(pageTag);
        }
    }

    private static ShellController GetOrCreateController(
        Window window) =>
        Controllers.GetValue(
            window,
            static value => new ShellController(value));

    private sealed class ShellController : IDisposable
    {
        private static readonly Thickness DesktopContentPadding =
            new(24, 22, 24, 28);
        private static readonly Thickness MobileContentPadding =
            new(14, 16, 14, 24);

        private readonly Window _window;
        private readonly NavigationView _navigationView;
        private readonly StackPanel _desktopTitleCommands;
        private readonly Button _desktopBackButton;
        private readonly StackPanel _desktopUtilityCommands;
        private readonly Border _sectionBadge;
        private readonly Grid _titleBarSearchContainer;
        private readonly TextBox _titleBarSearchBox;
        private readonly ScrollViewer _contentHost;
        private readonly Border _contentFrame;
        private readonly Border _mobileBottomBar;
        private readonly Border _mobileHeader;
        private readonly Grid _mobileHeaderLayout;
        private readonly Border _mobileLogoFrame;
        private readonly Image _paneLogoImage;
        private readonly Image _mobileLogoImage;
        private readonly Button _mobileBackButton;
        private readonly Grid _mobileSearchPanel;
        private readonly TextBox _mobileSearchBox;
        private readonly TextBlock _mobilePageEyebrow;
        private readonly TextBlock _mobilePageTitle;
        private readonly Button _mobileComponentsButton;
        private readonly Button _mobileTokensButton;
        private readonly Button _mobileStudioButton;
        private readonly Jalium.UI.Shapes.Path
            _mobileComponentsIcon;
        private readonly Jalium.UI.Shapes.Path _mobileTokensIcon;
        private readonly Jalium.UI.Shapes.Path _mobileStudioIcon;
        private readonly TextBlock _mobileComponentsLabel;
        private readonly TextBlock _mobileTokensLabel;
        private readonly TextBlock _mobileStudioLabel;
        private readonly MobileNavigationBrushPalette
            _darkMobilePalette;
        private readonly MobileNavigationBrushPalette
            _lightMobilePalette;
        private readonly FrostedGlassEffect _darkMobileBackdrop =
            new(
                blurRadius: 18f,
                noiseIntensity: 0.012f,
                tintColor: Color.FromRgb(0x12, 0x17, 0x19),
                tintOpacity: 0.58f);
        private readonly FrostedGlassEffect _lightMobileBackdrop =
            new(
                blurRadius: 18f,
                noiseIntensity: 0.012f,
                tintColor: Color.FromRgb(0xFB, 0xFC, 0xFC),
                tintOpacity: 0.52f);
        private readonly DropShadowEffect _darkMobileShadow =
            new()
            {
                ShadowDepth = 4,
                BlurRadius = 24,
                Direction = 270,
                Color = Color.FromArgb(
                    0x48,
                    0x00,
                    0x00,
                    0x00)
            };
        private readonly DropShadowEffect _lightMobileShadow =
            new()
            {
                ShadowDepth = 3,
                BlurRadius = 20,
                Direction = 270,
                Color = Color.FromArgb(
                    0x24,
                    0x19,
                    0x25,
                    0x2C)
            };
        private bool _started;
        private double _shellWidth;

        public ShellController(Window window)
        {
            _window = window;
            _navigationView =
                Find<NavigationView>("NavigationView");
            _desktopTitleCommands =
                Find<StackPanel>("DesktopTitleCommands");
            _desktopUtilityCommands =
                Find<StackPanel>("DesktopUtilityCommands");
            _desktopBackButton =
                Find<Button>("DesktopBackButton");
            _sectionBadge = Find<Border>("SectionBadge");
            _titleBarSearchContainer =
                Find<Grid>("TitleBarSearchContainer");
            _titleBarSearchBox =
                Find<TextBox>("TitleBarSearchBox");
            _contentHost = Find<ScrollViewer>("ContentHost");
            _contentFrame = Find<Border>("ContentFrame");
            _mobileBottomBar =
                Find<Border>("MobileBottomBar");
            _mobileHeader = Find<Border>("MobileHeader");
            _mobileHeaderLayout =
                Find<Grid>("MobileHeaderLayout");
            _mobileLogoFrame =
                Find<Border>("MobileLogoFrame");
            _paneLogoImage = Find<Image>("PaneLogoImage");
            _mobileLogoImage =
                Find<Image>("MobileLogoImage");
            _mobileBackButton =
                Find<Button>("MobileBackButton");
            _mobileSearchPanel =
                Find<Grid>("MobileSearchPanel");
            _mobileSearchBox =
                Find<TextBox>("MobileSearchBox");
            _mobilePageEyebrow =
                Find<TextBlock>("MobilePageEyebrow");
            _mobilePageTitle =
                Find<TextBlock>("MobilePageTitle");
            _mobileComponentsButton =
                Find<Button>("MobileComponentsButton");
            _mobileTokensButton =
                Find<Button>("MobileTokensButton");
            _mobileStudioButton =
                Find<Button>("MobileStudioButton");
            _mobileComponentsIcon =
                Find<Jalium.UI.Shapes.Path>(
                    "MobileComponentsIcon");
            _mobileTokensIcon =
                Find<Jalium.UI.Shapes.Path>("MobileTokensIcon");
            _mobileStudioIcon =
                Find<Jalium.UI.Shapes.Path>("MobileStudioIcon");
            _mobileComponentsLabel =
                Find<TextBlock>("MobileComponentsLabel");
            _mobileTokensLabel =
                Find<TextBlock>("MobileTokensLabel");
            _mobileStudioLabel =
                Find<TextBlock>("MobileStudioLabel");
            _darkMobilePalette =
                CreateMobileNavigationBrushPalette(
                    darkMode: true,
                    OperatingSystem.IsAndroid());
            _lightMobilePalette =
                CreateMobileNavigationBrushPalette(
                    darkMode: false,
                    OperatingSystem.IsAndroid());
        }

        public bool IsMobileShell { get; private set; }

        public void Start()
        {
            if (_started)
            {
                return;
            }

            _started = true;
            _ = LoadLogoAsync();
            _navigationView.UpdateMenuItems();

            GalleryTheme.ModeChanged += OnGalleryModeChanged;
            LocalizationManager.LanguageChanged += OnLanguageChanged;
            _window.Closed += OnClosed;
            _window.Loaded += OnLoaded;
            _window.SizeChanged += OnSizeChanged;
            _window.OrientationChanged += OnOrientationChanged;
            _window.PreviewKeyDown += OnPreviewKeyDown;
            _window.KeyDown += OnKeyDown;

            ApplyGalleryChromePalette();
            UpdatePageContext(GetCurrentPageTag(_window));
            _window.SystemBackdrop = WindowBackdropType.None;
            ApplyDeepLink();

            if (OperatingSystem.IsAndroid())
            {
                ApplyResponsiveShell(
                    MobileShellBreakpoint - 1);
            }
            else if (IsEnabledEnvironmentVariable(
                         "JALIUM_GALLERY_MOBILE_PREVIEW"))
            {
                _window.Width = 390;
                _window.Height = 844;
                ApplyResponsiveShell(390);
            }
        }

        public void ApplyResponsiveShell(double width)
        {
            if (width <= 0 ||
                double.IsNaN(width) ||
                double.IsInfinity(width))
            {
                return;
            }

            _shellWidth = width;
            var mobile =
                OperatingSystem.IsAndroid() ||
                ShouldUseMobileShell(width);
            IsMobileShell = mobile;

            _window.IsShowTitleBar = !mobile;
            _desktopTitleCommands.Visibility = mobile
                ? Visibility.Collapsed
                : Visibility.Visible;
            _mobileHeader.Visibility = mobile
                ? Visibility.Visible
                : Visibility.Collapsed;
            _mobileBottomBar.Visibility = mobile
                ? Visibility.Visible
                : Visibility.Collapsed;
            _navigationView.CompactPaneLength = 0;

            if (mobile)
            {
                _navigationView.IsPaneOpen = false;
                Grid.SetRow(_navigationView, 1);
                Grid.SetRowSpan(_navigationView, 1);
                _contentFrame.Padding = MobileContentPadding;
                _contentHost.HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Disabled;
                _desktopUtilityCommands.Visibility =
                    Visibility.Collapsed;
            }
            else
            {
                _navigationView.OpenPaneLength = 248;
                _navigationView.IsPaneOpen = true;
                Grid.SetRow(_navigationView, 0);
                Grid.SetRowSpan(_navigationView, 3);
                _contentFrame.Padding = DesktopContentPadding;
                _contentHost.HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Disabled;

                var showUtilities = width >= 1050;
                _desktopUtilityCommands.Visibility =
                    showUtilities
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                _titleBarSearchContainer.Width = width >= 1280
                    ? 440
                    : Math.Clamp(
                        width -
                        (showUtilities ? 760 : 540),
                        260,
                        440);
            }

            UpdatePageContext(GetCurrentPageTag(_window));
        }

        public void UpdatePageContext(string pageTag)
        {
            SelectNavigationItem(pageTag);
            var descriptor =
                GalleryComponentCatalog.Items.FirstOrDefault(
                    item => string.Equals(
                        item.PageTag,
                        pageTag,
                        StringComparison.OrdinalIgnoreCase));

            var showBack =
                descriptor is not null || !IsPrimaryPage(pageTag);
            _mobileLogoFrame.Visibility = showBack
                ? Visibility.Collapsed
                : Visibility.Visible;
            _mobileBackButton.Visibility = showBack
                ? Visibility.Visible
                : Visibility.Collapsed;
            _desktopBackButton.Visibility = showBack && !IsMobileShell
                ? Visibility.Visible
                : Visibility.Collapsed;

            var showSearch = pageTag == "home";
            _mobileSearchPanel.Visibility = showSearch
                ? Visibility.Visible
                : Visibility.Collapsed;
            _mobileHeaderLayout.RowDefinitions[1].Height =
                new GridLength(showSearch ? 10 : 0);
            _mobileHeaderLayout.RowDefinitions[2].Height =
                new GridLength(showSearch ? 42 : 0);

            // These two are written imperatively (the copy depends on which page
            // is showing), so they cannot carry a {DynamicResource} — a refresh
            // would overwrite the page-specific text. OnLanguageChanged re-runs
            // this method instead.
            if (descriptor is not null)
            {
                _mobilePageEyebrow.Text =
                    descriptor.CategoryLabel.ToUpperInvariant();
                _mobilePageTitle.Text = descriptor.Title;
            }
            else
            {
                var (eyebrowKey, titleKey) = pageTag switch
                {
                    "design-tokens" =>
                        ("Loc.Shell.EyebrowFoundations", "Loc.Nav.DesignTokens"),
                    "theme-studio" =>
                        ("Loc.Shell.EyebrowPersonalize", "Loc.Nav.ThemeStudio"),
                    _ =>
                        ("Loc.Shell.EyebrowMobile", "Loc.Nav.Components")
                };

                _mobilePageEyebrow.Text = LocalizationManager.Get(eyebrowKey);
                _mobilePageTitle.Text = LocalizationManager.Get(titleKey);
            }

            _sectionBadge.Visibility = pageTag == "section"
                ? Visibility.Visible
                : Visibility.Collapsed;
            UpdateContentScrollOwnership(pageTag);
            UpdateMobileNavigationVisuals();
        }

        private void UpdateContentScrollOwnership(string pageTag)
        {
            var catalogOwnsVerticalScroll =
                UsesSplitCatalogLayout(pageTag);
            var target = catalogOwnsVerticalScroll
                ? ScrollBarVisibility.Disabled
                : ScrollBarVisibility.Auto;

            if (_contentHost.VerticalScrollBarVisibility == target)
            {
                return;
            }

            // A split catalog is a fixed workspace: its title, filters, and
            // detail inspector stay pinned while ComponentList owns scrolling.
            // Other pages, including the stacked/mobile catalog, still use the
            // shell viewer because their full page is meant to flow vertically.
            _contentHost.ScrollToVerticalOffset(0);
            _contentHost.VerticalScrollBarVisibility = target;
        }

        private bool UsesSplitCatalogLayout(string pageTag)
        {
            if (IsMobileShell ||
                !string.Equals(
                    pageTag,
                    "home",
                    StringComparison.OrdinalIgnoreCase) ||
                !double.IsFinite(_shellWidth) ||
                _shellWidth <= 0)
            {
                return false;
            }

            var paneWidth = _navigationView.IsPaneOpen
                ? _navigationView.OpenPaneLength
                : _navigationView.CompactPaneLength;
            var workspaceWidth =
                _shellWidth -
                paneWidth -
                _contentFrame.Padding.Left -
                _contentFrame.Padding.Right;

            // Evaluate the width the workspace receives after the outer
            // scrollbar is disabled. This makes the breakpoint stable instead
            // of letting the scrollbar gutter itself toggle the decision.
            return workspaceWidth >= HomeWorkspaceGrid.SplitViewWidth;
        }

        public void Dispose()
        {
            if (!_started)
            {
                return;
            }

            _started = false;
            GalleryTheme.ModeChanged -= OnGalleryModeChanged;
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
            _window.Closed -= OnClosed;
            _window.Loaded -= OnLoaded;
            _window.SizeChanged -= OnSizeChanged;
            _window.OrientationChanged -= OnOrientationChanged;
            _window.PreviewKeyDown -= OnPreviewKeyDown;
            _window.KeyDown -= OnKeyDown;
        }

        /// <summary>
        /// JALIUM_DEEPLINK=&lt;pageTag&gt; opens that page instead of the catalog, so a
        /// screenshot harness can reach any sample page without driving the search
        /// box. Applied here rather than from Window.Loaded: by this point the shell
        /// has finished its own first navigation, so the request is not overwritten.
        /// Inert when the variable is unset.
        /// </summary>
        private void ApplyDeepLink()
        {
            var pageTag = Environment.GetEnvironmentVariable("JALIUM_DEEPLINK");
            if (string.IsNullOrWhiteSpace(pageTag))
            {
                return;
            }

            _window.Dispatcher.BeginInvoke(() =>
            {
                if (_window.DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.NavigateCommand.Execute(pageTag);
                }
            });
        }

        private void OnLanguageChanged(
            object? sender,
            EventArgs args) =>
            UpdatePageContext(GetCurrentPageTag(_window));

        private void OnGalleryModeChanged(
            object? sender,
            EventArgs args)
        {
            GalleryThemeTuner.RefreshForCurrentMode();
            ApplyGalleryChromePalette();
            UpdateMobileNavigationVisuals();

            if (_contentFrame.Child is
                IGalleryThemeAware themeAware)
            {
                themeAware.RefreshTheme();
            }
        }

        private void ApplyGalleryChromePalette()
        {
            var darkMode =
                GalleryTheme.CurrentMode == GalleryThemeMode.Dark;
            var useMaterialEffects =
                !OperatingSystem.IsAndroid() &&
                global::Jalium.UI.SystemParameters.UIEffects &&
                !global::Jalium.UI.SystemParameters.HighContrast;
            _window.Background = GalleryTheme.BackgroundDarkBrush;
            _window.Foreground = GalleryTheme.TextPrimaryBrush;
            _navigationView.PaneBackground =
                GalleryTheme.BackgroundMediumBrush;
            _navigationView.ContentBackground =
                GalleryTheme.TransparentBrush;
            _contentHost.Background =
                GalleryTheme.ShellBackgroundBrush;
            _mobileBottomBar.BackdropEffect =
                useMaterialEffects
                    ? darkMode
                        ? _darkMobileBackdrop
                        : _lightMobileBackdrop
                    : null;
            _mobileBottomBar.Effect =
                useMaterialEffects
                    ? darkMode
                        ? _darkMobileShadow
                        : _lightMobileShadow
                    : null;
            _mobileHeader.BackdropEffect =
                useMaterialEffects
                    ? darkMode
                        ? _darkMobileBackdrop
                        : _lightMobileBackdrop
                    : null;
        }

        private void OnLoaded(
            object? sender,
            RoutedEventArgs args) =>
            ApplyResponsiveShell(_window.ActualWidth);

        private void OnSizeChanged(
            object? sender,
            SizeChangedEventArgs args) =>
            ApplyResponsiveShell(_window.ActualWidth);

        private void OnOrientationChanged(
            object? sender,
            EventArgs args) =>
            _window.Dispatcher.InvokeAsync(
                () => ApplyResponsiveShell(
                    _window.ActualWidth),
                DispatcherPriority.Background);

        private void OnPreviewKeyDown(
            object? sender,
            KeyEventArgs args)
        {
            if (args.Handled ||
                args.IsRepeat ||
                args.Key != Key.K ||
                args.KeyboardModifiers != ModifierKeys.Control)
            {
                return;
            }

            if (IsMobileShell &&
                !IsPrimaryPage(GetCurrentPageTag(_window)))
            {
                SetCurrentPageTag(_window, "home");
            }

            var searchBox = IsMobileShell
                ? _mobileSearchBox
                : _titleBarSearchBox;
            searchBox.Focus();
            searchBox.SelectAll();
            args.Handled = true;
        }

        private void OnKeyDown(
            object? sender,
            RoutedEventArgs args)
        {
            if (args is not KeyEventArgs keyArgs ||
                keyArgs.Handled ||
                keyArgs.Key is not
                    (Key.BrowserBack or Key.Escape) ||
                IsPrimaryPage(GetCurrentPageTag(_window)))
            {
                return;
            }

            // Previously gated on IsMobileShell, which left the desktop shell with no
            // keyboard route back off a component detail page. The guard below keeps
            // Escape doing the locally-expected thing while a search box is focused
            // instead of navigating out from under the user.
            if (_titleBarSearchBox.IsKeyboardFocused ||
                _mobileSearchBox.IsKeyboardFocused)
            {
                return;
            }

            SetCurrentPageTag(_window, "home");
            keyArgs.Handled = true;
        }

        private void UpdateMobileNavigationVisuals()
        {
            var darkMode =
                GalleryTheme.CurrentMode == GalleryThemeMode.Dark;
            var palette = darkMode
                ? _darkMobilePalette
                : _lightMobilePalette;
            var useMaterialEffects =
                !OperatingSystem.IsAndroid() &&
                global::Jalium.UI.SystemParameters.UIEffects &&
                !global::Jalium.UI.SystemParameters.HighContrast;
            var material = useMaterialEffects
                ? palette.Material
                : darkMode
                    ? GalleryTheme.BackgroundMediumBrush
                    : GalleryTheme.BackgroundCardBrush;
            _mobileBottomBar.Background = material;
            _mobileHeader.Background = material;
            _mobileBottomBar.BorderBrush = palette.Border;

            var selectedForeground = darkMode
                ? GalleryTheme.AccentLightBrush
                : GalleryTheme.AccentDarkBrush;
            var pageTag = GetCurrentPageTag(_window);
            var activeTag = IsPrimaryPage(pageTag)
                ? pageTag
                : "home";

            ApplyEntry(
                "home",
                _mobileComponentsButton,
                _mobileComponentsIcon,
                _mobileComponentsLabel);
            ApplyEntry(
                "design-tokens",
                _mobileTokensButton,
                _mobileTokensIcon,
                _mobileTokensLabel);
            ApplyEntry(
                "theme-studio",
                _mobileStudioButton,
                _mobileStudioIcon,
                _mobileStudioLabel);

            void ApplyEntry(
                string tag,
                Button button,
                Jalium.UI.Shapes.Path icon,
                TextBlock label)
            {
                var selected = activeTag == tag;
                var foreground = selected
                    ? selectedForeground
                    : palette.IdleForeground;
                button.Background = selected
                    ? palette.SelectedSurface
                    : GalleryTheme.TransparentBrush;
                button.BorderBrush = selected
                    ? palette.SelectedBorder
                    : GalleryTheme.TransparentBrush;
                button.Foreground = foreground;
                icon.Fill = foreground;
                label.Foreground = foreground;
            }
        }

        private void SelectNavigationItem(string pageTag)
        {
            foreach (var item in _navigationView.MenuItems
                         .OfType<NavigationViewItem>())
            {
                if (string.Equals(
                        item.Tag as string,
                        pageTag,
                        StringComparison.Ordinal))
                {
                    _navigationView.SelectedItem = item;
                    return;
                }
            }

            _navigationView.SelectedItem = null;
        }

        private T Find<T>(string name)
            where T : class =>
            _window.FindName(name) as T ??
            throw new InvalidOperationException(
                $"Shell element '{name}' was not found.");

        private void OnClosed(object? sender, EventArgs args)
        {
            Dispose();
            Controllers.Remove(_window);
        }

        private async Task LoadLogoAsync()
        {
            try
            {
                // The source PNG expands to more than two megabytes of BGRA
                // pixels. Native decoder initialization plus that decode used
                // to run synchronously from Window.Loaded, keeping the freshly
                // shown window unresponsive on machines with a cold media DLL.
                // Decode only raw, dispatcher-independent pixels on a worker;
                // create and attach the DispatcherObject on the UI thread.
                var decoded = await Task.Run(
                    static () => DecodeEmbeddedImage(
                        "Jalium.UI.Gallery.Modules.Main.Assets.logo.png"));

                if (!_started)
                {
                    return;
                }

                var logo = BitmapImage.FromDecodedImage(decoded);
                _paneLogoImage.Source = logo;
                _mobileLogoImage.Source = logo;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Gallery logo load failed: {exception.Message}");
            }
        }
    }

    private static MobileNavigationBrushPalette
        CreateMobileNavigationBrushPalette(
            bool darkMode,
            bool isAndroid)
    {
        var colors =
            GetMobileNavigationPalette(darkMode, isAndroid);
        return new MobileNavigationBrushPalette(
            new SolidColorBrush(colors.Material),
            new SolidColorBrush(colors.Border),
            new SolidColorBrush(colors.SelectedSurface),
            new SolidColorBrush(colors.SelectedBorder),
            new SolidColorBrush(colors.IdleForeground));
    }

    private static DecodedImage DecodeEmbeddedImage(
        string resourceName)
    {
        using var stream =
            typeof(GalleryShellBehavior).Assembly
                .GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' was not found.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return new NativeImageDecoder().Decode(memory.GetBuffer().AsSpan(
            0,
            checked((int)memory.Length)));
    }

    private static bool IsPrimaryPage(string pageTag) =>
        pageTag is
            "home" or "design-tokens" or "theme-studio";

    private static bool IsEnabledEnvironmentVariable(
        string name) =>
        Environment.GetEnvironmentVariable(name) is
            "1" or "true" or "True";

    private sealed record MobileNavigationBrushPalette(
        SolidColorBrush Material,
        SolidColorBrush Border,
        SolidColorBrush SelectedSurface,
        SolidColorBrush SelectedBorder,
        SolidColorBrush IdleForeground);
}
