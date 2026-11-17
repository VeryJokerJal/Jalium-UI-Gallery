using System.ComponentModel;
using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Jalium.UI.Media.Native;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>
/// Reusable responsive shell. Templates own chrome, palettes and layout; this control
/// exposes navigation/viewport state and connects optional input and scrolling parts.
/// </summary>
[TemplatePart(Name = "NavigationView", Type = typeof(NavigationView))]
[TemplatePart(Name = "ContentHost", Type = typeof(ScrollViewer))]
[TemplatePart(Name = "ContentFrame", Type = typeof(GalleryNavigationHost))]
[TemplatePart(Name = "TitleBarSearchBox", Type = typeof(TextBox))]
[TemplatePart(Name = "MobileSearchBox", Type = typeof(TextBox))]
[TemplateVisualState(Name = "Desktop", GroupName = "LayoutStates")]
[TemplateVisualState(Name = "Mobile", GroupName = "LayoutStates")]
[TemplateVisualState(Name = "Catalog", GroupName = "PageStates")]
[TemplateVisualState(Name = "PrimaryPage", GroupName = "PageStates")]
[TemplateVisualState(Name = "DetailPage", GroupName = "PageStates")]
[TemplateVisualState(Name = "Light", GroupName = "ThemeStates")]
[TemplateVisualState(Name = "Dark", GroupName = "ThemeStates")]
[TemplateVisualState(Name = "Material", GroupName = "MaterialStates")]
[TemplateVisualState(Name = "Solid", GroupName = "MaterialStates")]
[TemplateVisualState(Name = "CatalogScroll", GroupName = "ScrollStates")]
[TemplateVisualState(Name = "PageScroll", GroupName = "ScrollStates")]
public class GalleryWindow : Window
{
    public static readonly DependencyProperty LeftCommandsTemplateProperty = DependencyProperty.Register(
        nameof(LeftCommandsTemplate), typeof(DataTemplate), typeof(GalleryWindow), new PropertyMetadata(null, OnCommandsTemplateChanged));
    public static readonly DependencyProperty RightCommandsTemplateProperty = DependencyProperty.Register(
        nameof(RightCommandsTemplate), typeof(DataTemplate), typeof(GalleryWindow), new PropertyMetadata(null, OnCommandsTemplateChanged));
    public static readonly DependencyProperty CurrentPageTagProperty = DependencyProperty.Register(
        nameof(CurrentPageTag), typeof(string), typeof(GalleryWindow),
        new FrameworkPropertyMetadata("home", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnContextChanged,
            (_, value) => value is string tag && !string.IsNullOrWhiteSpace(tag) ? tag.Trim().ToLowerInvariant() : "home"));
    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.Register(
        nameof(NavigateCommand), typeof(ICommand), typeof(GalleryWindow), new PropertyMetadata(null));
    public static readonly DependencyProperty MobileBreakpointProperty = DependencyProperty.Register(
        nameof(MobileBreakpoint), typeof(double), typeof(GalleryWindow), new PropertyMetadata(720d, OnContextChanged),
        value => value is double width && double.IsFinite(width) && width > 0);
    public static readonly DependencyProperty EnableMaterialEffectsProperty = DependencyProperty.Register(
        nameof(EnableMaterialEffects), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(true, OnContextChanged));
    public static readonly DependencyProperty LogoSourceProperty = DependencyProperty.Register(
        nameof(LogoSource), typeof(ImageSource), typeof(GalleryWindow), new PropertyMetadata(null));
    private static readonly DependencyPropertyKey IsMobileShellPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsMobileShell), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty IsMobileShellProperty = IsMobileShellPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey HasDesktopUtilitiesPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasDesktopUtilities), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(true));
    public static readonly DependencyProperty HasDesktopUtilitiesProperty = HasDesktopUtilitiesPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey HasBackNavigationPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasBackNavigation), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty HasBackNavigationProperty = HasBackNavigationPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey HasDesktopBackNavigationPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasDesktopBackNavigation), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty HasDesktopBackNavigationProperty = HasDesktopBackNavigationPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey IsCatalogPagePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsCatalogPage), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(true));
    public static readonly DependencyProperty IsCatalogPageProperty = IsCatalogPagePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey IsSectionPagePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsSectionPage), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty IsSectionPageProperty = IsSectionPagePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey IsSplitCatalogLayoutPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsSplitCatalogLayout), typeof(bool), typeof(GalleryWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty IsSplitCatalogLayoutProperty = IsSplitCatalogLayoutPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey DesktopSearchWidthPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(DesktopSearchWidth), typeof(double), typeof(GalleryWindow), new PropertyMetadata(440d));
    public static readonly DependencyProperty DesktopSearchWidthProperty = DesktopSearchWidthPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey ActiveNavigationTagPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ActiveNavigationTag), typeof(string), typeof(GalleryWindow), new PropertyMetadata("home"));
    public static readonly DependencyProperty ActiveNavigationTagProperty = ActiveNavigationTagPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey PageTitlePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(PageTitle), typeof(string), typeof(GalleryWindow), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PageTitleProperty = PageTitlePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey PageEyebrowPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(PageEyebrow), typeof(string), typeof(GalleryWindow), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PageEyebrowProperty = PageEyebrowPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey ContentPaddingPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ContentPadding), typeof(Thickness), typeof(GalleryWindow), new PropertyMetadata(new Thickness(24, 22, 24, 28)));
    public static readonly DependencyProperty ContentPaddingProperty = ContentPaddingPropertyKey.DependencyProperty;

    private NavigationView? _navigation;
    private ScrollViewer? _contentHost;
    private GalleryNavigationHost? _contentFrame;
    private TextBox? _desktopSearch;
    private TextBox? _mobileSearch;
    private bool _connected;
    private bool _hasTemplate;
    private bool _synchronizingNavigation;
    private bool _deepLinkApplied;
    private double _viewportWidth;
    private int _logoLoadVersion;
    private DispatcherOperation? _pendingOrientation;
    private DispatcherOperation? _pendingDeepLink;
    private static readonly Lazy<Task<DecodedImage>> LogoPixels = new(() => Task.Run(DecodeLogo));

    public GalleryWindow()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = GetType();
        Loaded += OnShellLoaded;
        Unloaded += OnShellUnloaded;
        SizeChanged += OnShellSizeChanged;
        OrientationChanged += OnShellOrientationChanged;
        PreviewKeyDown += OnShellPreviewKeyDown;
        KeyDown += OnShellKeyDown;
        UpdateDefaultStyle();
        ApplyTemplate();
    }

    /// <summary>Chrome templates receive this shell as DataContext; the application model is available through DataContext.DataContext.</summary>
    public DataTemplate? LeftCommandsTemplate { get => (DataTemplate?)GetValue(LeftCommandsTemplateProperty); set => SetValue(LeftCommandsTemplateProperty, value); }
    public DataTemplate? RightCommandsTemplate { get => (DataTemplate?)GetValue(RightCommandsTemplateProperty); set => SetValue(RightCommandsTemplateProperty, value); }
    public string CurrentPageTag { get => (string?)GetValue(CurrentPageTagProperty) ?? "home"; set => SetValue(CurrentPageTagProperty, value); }
    public ICommand? NavigateCommand { get => (ICommand?)GetValue(NavigateCommandProperty); set => SetValue(NavigateCommandProperty, value); }
    public double MobileBreakpoint { get => (double)GetValue(MobileBreakpointProperty)!; set => SetValue(MobileBreakpointProperty, value); }
    public bool EnableMaterialEffects { get => (bool)GetValue(EnableMaterialEffectsProperty)!; set => SetValue(EnableMaterialEffectsProperty, value); }
    public ImageSource? LogoSource { get => (ImageSource?)GetValue(LogoSourceProperty); set => SetValue(LogoSourceProperty, value); }
    public bool IsMobileShell => (bool)GetValue(IsMobileShellProperty)!;
    public bool HasDesktopUtilities => (bool)GetValue(HasDesktopUtilitiesProperty)!;
    public bool HasBackNavigation => (bool)GetValue(HasBackNavigationProperty)!;
    public bool HasDesktopBackNavigation => (bool)GetValue(HasDesktopBackNavigationProperty)!;
    public bool IsCatalogPage => (bool)GetValue(IsCatalogPageProperty)!;
    public bool IsSectionPage => (bool)GetValue(IsSectionPageProperty)!;
    public bool IsSplitCatalogLayout => (bool)GetValue(IsSplitCatalogLayoutProperty)!;
    public double DesktopSearchWidth => (double)GetValue(DesktopSearchWidthProperty)!;
    public string ActiveNavigationTag => (string)GetValue(ActiveNavigationTagProperty)!;
    public string PageTitle => (string)GetValue(PageTitleProperty)!;
    public string PageEyebrow => (string)GetValue(PageEyebrowProperty)!;
    public Thickness ContentPadding => (Thickness)GetValue(ContentPaddingProperty)!;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ApplyCommandTemplates();
        _navigation = GetTemplateChild("NavigationView") as NavigationView;
        _contentHost = GetTemplateChild("ContentHost") as ScrollViewer;
        _contentFrame = GetTemplateChild("ContentFrame") as GalleryNavigationHost;
        _hasTemplate = true;
        if (_navigation != null)
        {
            _navigation.UpdateMenuItems();
            _navigation.SelectionChanged += OnNavigationSelectionChanged;
        }
        ConnectEnvironment();
        UpdateViewport(GetRequestedViewportWidth());
    }

    protected override void OnTemplateChanging()
    {
        _hasTemplate = false;
        DisconnectEnvironment();
        if (_navigation != null) _navigation.SelectionChanged -= OnNavigationSelectionChanged;
        _navigation = null;
        _contentHost = null;
        _contentFrame = null;
        _desktopSearch = null;
        _mobileSearch = null;
        ReleaseCommandContext(LeftWindowCommands);
        ReleaseCommandContext(RightWindowCommands);
        SetCurrentValue(LeftWindowCommandsProperty, null);
        SetCurrentValue(RightWindowCommandsProperty, null);
        base.OnTemplateChanging();
    }

    protected override void OnClosed(EventArgs e)
    {
        DisconnectEnvironment();
        base.OnClosed(e);
    }

    /// <summary>Finds an optional part in the body or either independently instantiated command area.</summary>
    public object? FindPart(string name) => base.FindName(name)
        ?? LeftWindowCommands?.FindName(name) ?? RightWindowCommands?.FindName(name);
    public new object? FindName(string name) => FindPart(name);

    /// <summary>Accepts the native viewport before the next arrange catches up.</summary>
    public void UpdateViewport(double width)
    {
        if (!double.IsFinite(width) || width <= 0) return;
        _viewportWidth = width;
        UpdateContext();
    }

    /// <summary>Focuses the active search field, returning to the catalog on compact detail pages.</summary>
    public bool FocusSearch()
    {
        if (IsMobileShell && !IsCatalogPage) Navigate("home");
        var search = IsMobileShell ? _mobileSearch : _desktopSearch;
        if (search?.Focus() != true) return false;
        search.SelectAll();
        return true;
    }

    private void UpdateContext()
    {
        var width = _viewportWidth > 0 ? _viewportWidth : GetRequestedViewportWidth();
        var mobile = OperatingSystem.IsAndroid() || (width > 0 && width < MobileBreakpoint);
        var catalog = string.Equals(CurrentPageTag, "home", StringComparison.OrdinalIgnoreCase);
        var back = !IsPrimaryPage(CurrentPageTag);
        var padding = mobile ? new Thickness(14, 16, 14, 24) : new Thickness(24, 22, 24, 28);
        var showUtilities = !mobile && width >= 1050;
        SetValue(IsMobileShellPropertyKey, mobile);
        SetValue(HasDesktopUtilitiesPropertyKey, showUtilities);
        SetValue(HasBackNavigationPropertyKey, back);
        SetValue(HasDesktopBackNavigationPropertyKey, back && !mobile);
        SetValue(IsCatalogPagePropertyKey, catalog);
        SetValue(IsSectionPagePropertyKey, string.Equals(CurrentPageTag, "section", StringComparison.OrdinalIgnoreCase));
        SetValue(ActiveNavigationTagPropertyKey, back ? "home" : CurrentPageTag.ToLowerInvariant());
        SetValue(ContentPaddingPropertyKey, padding);
        SetValue(DesktopSearchWidthPropertyKey, width >= 1280 ? 440d : Math.Clamp(width - (showUtilities ? 760 : 540), 260, 440));
        // The split catalog gets a stable workspace width independent of scrollbar gutters.
        var paneWidth = mobile ? 0 : (_navigation?.OpenPaneLength ?? 248);
        SetValue(IsSplitCatalogLayoutPropertyKey, !mobile && catalog && width - paneWidth - padding.Left - padding.Right >= HomeWorkspaceGrid.SplitViewWidth);
        var descriptor = GalleryComponentCatalog.Items.FirstOrDefault(item => string.Equals(item.PageTag, CurrentPageTag, StringComparison.OrdinalIgnoreCase));
        if (descriptor != null)
        {
            SetValue(PageEyebrowPropertyKey, descriptor.CategoryLabel.ToUpperInvariant());
            SetValue(PageTitlePropertyKey, descriptor.Title);
        }
        else
        {
            var (eyebrow, title) = CurrentPageTag.ToLowerInvariant() switch
            {
                "overview" => ("Loc.Shell.EyebrowMobile", "Loc.Nav.Overview"),
                "design-tokens" => ("Loc.Shell.EyebrowFoundations", "Loc.Nav.DesignTokens"),
                "theme-studio" => ("Loc.Shell.EyebrowPersonalize", "Loc.Nav.ThemeStudio"),
                _ => ("Loc.Shell.EyebrowMobile", "Loc.Nav.Components")
            };
            SetValue(PageEyebrowPropertyKey, LocalizationManager.Get(eyebrow));
            SetValue(PageTitlePropertyKey, LocalizationManager.Get(title));
        }
        SynchronizeNavigation();
        if (_contentHost != null)
        {
            var scroll = IsSplitCatalogLayout ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
            if (_contentHost.VerticalScrollBarVisibility != scroll)
                _contentHost.ScrollToVerticalOffset(0);
        }
        UpdateStates();
    }

    private void SynchronizeNavigation()
    {
        if (_navigation == null || _synchronizingNavigation) return;
        _synchronizingNavigation = true;
        try
        {
            _navigation.SetCurrentValue(NavigationView.SelectedItemProperty,
                _navigation.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => string.Equals(item.Tag as string, CurrentPageTag, StringComparison.OrdinalIgnoreCase)));
        }
        finally { _synchronizingNavigation = false; }
    }

    private void UpdateStates()
    {
        if (!_hasTemplate) return;
        VisualStateManager.GoToState(this, IsMobileShell ? "Mobile" : "Desktop", false);
        VisualStateManager.GoToState(this, IsCatalogPage ? "Catalog" : HasBackNavigation ? "DetailPage" : "PrimaryPage", false);
        VisualStateManager.GoToState(this, GalleryTheme.CurrentMode == GalleryThemeMode.Dark ? "Dark" : "Light", false);
        var material = EnableMaterialEffects && !OperatingSystem.IsAndroid() && SystemParameters.UIEffects && !SystemParameters.HighContrast;
        VisualStateManager.GoToState(this, material ? "Material" : "Solid", false);
        VisualStateManager.GoToState(this, IsSplitCatalogLayout ? "CatalogScroll" : "PageScroll", false);
    }

    private void ApplyCommandTemplates()
    {
        ReleaseCommandContext(LeftWindowCommands);
        ReleaseCommandContext(RightWindowCommands);
        SetCurrentValue(LeftWindowCommandsProperty, CreateCommands(LeftCommandsTemplate));
        SetCurrentValue(RightWindowCommandsProperty, CreateCommands(RightCommandsTemplate));
        _desktopSearch = FindPart("TitleBarSearchBox") as TextBox;
        _mobileSearch = GetTemplateChild("MobileSearchBox") as TextBox;
    }

    private static void ReleaseCommandContext(FrameworkElement? commands)
    {
        if (commands == null) return;
        BindingOperations.ClearBinding(commands, DataContextProperty);
        commands.ClearValue(DataContextProperty);
    }

    private FrameworkElement? CreateCommands(DataTemplate? template)
    {
        var element = template?.LoadContent() as FrameworkElement;
        if (element != null) element.DataContext = this;
        return element;
    }

    private void ConnectEnvironment()
    {
        if (_connected) return;
        _connected = true;
        GalleryTheme.ModeChanged += OnGalleryModeChanged;
        LocalizationManager.LanguageChanged += OnLanguageChanged;
        SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
        _ = LoadLogoAsync(++_logoLoadVersion);
        if (!_deepLinkApplied && Environment.GetEnvironmentVariable("JALIUM_DEEPLINK") is { Length: > 0 } tag)
        {
            _pendingDeepLink = Dispatcher.BeginInvoke(() =>
            {
                _pendingDeepLink = null;
                if (!_connected) return;
                _deepLinkApplied = true;
                Navigate(tag);
            });
        }
        if (Environment.GetEnvironmentVariable("JALIUM_GALLERY_MOBILE_PREVIEW") is "1" or "true" or "True")
        {
            SetCurrentValue(WidthProperty, 390d);
            SetCurrentValue(HeightProperty, 844d);
        }
    }

    private void DisconnectEnvironment()
    {
        _connected = false;
        ++_logoLoadVersion;
        GalleryTheme.ModeChanged -= OnGalleryModeChanged;
        LocalizationManager.LanguageChanged -= OnLanguageChanged;
        SystemParameters.StaticPropertyChanged -= OnSystemParametersChanged;
        _pendingOrientation?.Abort();
        _pendingOrientation = null;
        _pendingDeepLink?.Abort();
        _pendingDeepLink = null;
    }

    private void Navigate(string pageTag)
    {
        if (NavigateCommand is { } command)
        {
            if (command.CanExecute(pageTag)) command.Execute(pageTag);
        }
        else SetCurrentValue(CurrentPageTagProperty, pageTag);
    }

    private void OnNavigationSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (!_synchronizingNavigation && e.SelectedItem?.Tag is string tag) Navigate(tag);
    }
    private void OnShellLoaded(object sender, RoutedEventArgs e)
    {
        ConnectEnvironment();
        UpdateViewport(GetRequestedViewportWidth());
    }
    private void OnShellUnloaded(object sender, RoutedEventArgs e) => DisconnectEnvironment();
    private void OnShellSizeChanged(object sender, SizeChangedEventArgs e) => UpdateViewport(e.NewSize.Width);
    private void OnShellOrientationChanged(object? sender, EventArgs e)
    {
        _pendingOrientation?.Abort();
        _pendingOrientation = Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            _pendingOrientation = null;
            if (_connected) UpdateViewport(GetRequestedViewportWidth());
        });
    }
    private double GetRequestedViewportWidth() => double.IsFinite(Width) && Width > 0 ? Width : ActualWidth;
    private void OnLanguageChanged(object? sender, EventArgs e) => UpdateContext();
    private void OnSystemParametersChanged(object? sender, PropertyChangedEventArgs e) => UpdateStates();
    private void OnGalleryModeChanged(object? sender, EventArgs e)
    {
        GalleryThemeTuner.RefreshForCurrentMode();
        UpdateStates();
        if (_contentFrame?.ActiveContent is IGalleryThemeAware aware) aware.RefreshTheme();
    }
    private void OnShellPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (IsDialogInput(e)) return;
        if (!e.Handled && !e.IsRepeat && e.Key == Key.K && e.KeyboardModifiers == ModifierKeys.Control)
            e.Handled = FocusSearch();
    }
    private void OnShellKeyDown(object sender, RoutedEventArgs e)
    {
        if (e is not KeyEventArgs key || key.Handled || key.Key is not (Key.BrowserBack or Key.Escape) || !HasBackNavigation) return;
        if (IsDialogInput(key)) return;
        if (_desktopSearch?.IsKeyboardFocusWithin == true || _mobileSearch?.IsKeyboardFocusWithin == true) return;
        Navigate("home");
        key.Handled = true;
    }

    private static bool IsDialogInput(RoutedEventArgs e)
    {
        for (var visual = (e.OriginalSource ?? e.Source) as Visual; visual != null; visual = visual.VisualParent)
            if (visual is ContentDialog) return true;
        return false;
    }
    private async Task LoadLogoAsync(int version)
    {
        if (LogoSource != null) return;
        try
        {
            var decoded = await LogoPixels.Value;
            await Dispatcher.InvokeAsync(() =>
            {
                if (_connected && _logoLoadVersion == version && LogoSource == null)
                    SetCurrentValue(LogoSourceProperty, BitmapImage.FromDecodedImage(decoded));
            });
        }
        catch (Exception error) { System.Diagnostics.Debug.WriteLine($"Gallery logo load failed: {error.Message}"); }
    }

    /// <summary>Loads the shared logo for exports that have no Loaded event or dispatcher loop.</summary>
    internal void EnsureLogoSource()
    {
        if (LogoSource == null)
            SetCurrentValue(LogoSourceProperty, BitmapImage.FromDecodedImage(LogoPixels.Value.GetAwaiter().GetResult()));
    }

    private static DecodedImage DecodeLogo()
    {
        using var stream = typeof(GalleryWindow).Assembly.GetManifestResourceStream("Jalium.UI.Gallery.Modules.Main.Assets.logo.png")
            ?? throw new InvalidOperationException("Embedded Gallery logo was not found.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return new NativeImageDecoder().Decode(memory.GetBuffer().AsSpan(0, checked((int)memory.Length)));
    }
    private static bool IsPrimaryPage(string? tag) => tag?.ToLowerInvariant() is "home" or "design-tokens" or "theme-studio";
    private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((GalleryWindow)d).UpdateContext();
    private static void OnCommandsTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (GalleryWindow)d;
        if (window._hasTemplate) window.ApplyCommandTemplates();
    }
}
