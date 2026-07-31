using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Support;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Gallery.Services.Interfaces;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.Views;

/// <summary>
/// Primary shell window for the Jalium.UI Gallery. The layout root lives in the
/// companion <c>MainWindow.jalxaml</c>; code-behind handles navigation, search,
/// theme behavior, and page instantiation.
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer? _titleBarSearchTimer;
    private HomePage? _homePage;
    private string _currentPageTag = "home";

    // Component page factories live in GalleryComponentCatalog so the
    // component grid and the navigable page set cannot drift apart.

    private readonly ViewAViewModel _viewModel;

    public MainWindow(IMessageService messageService)
    {
        InitializeComponent();
        PaneLogoImage.Source = LoadEmbeddedImage("Jalium.UI.Gallery.Modules.Main.Assets.logo.png");
        _viewModel = MainModule.CreateViewA(messageService);
        DataContext = _viewModel;

        InitializeTitleBarSearch();
        UpdateThemeToggleVisual();
        ApplyGalleryChromePalette();
        NavigationView?.UpdateMenuItems();

        GalleryTheme.ModeChanged += OnGalleryModeChanged;
        Closed += (_, _) => GalleryTheme.ModeChanged -= OnGalleryModeChanged;

        NavigateToPage("home");
        SelectNavigationItem("home");

        SystemBackdrop = WindowBackdropType.None;
    }

    private void OnGalleryModeChanged(object? sender, EventArgs e)
    {
        // Push the new gallery palette into Application.Resources so every
        // {DynamicResource Gallery...} reference in jalxaml re-binds itself.
        if (Application.Current is { } app)
        {
            GalleryTheme.RegisterResources(app.Resources);
        }

        UpdateThemeToggleVisual();
        ApplyGalleryChromePalette();

        if (NavigationView != null)
        {
            // The framework theme switch re-applies NavigationView's template,
            // which leaves _menuItemsPanel / _footerItemsPanel holding brand-new
            // (empty) panels. Without this forced refresh the items still live in
            // the previous template parts and the sidebar appears unresponsive.
            NavigationView.UpdateMenuItems();
        }

        SectionBadge.Visibility = _currentPageTag == "section"
            ? Visibility.Visible
            : Visibility.Collapsed;

        // Rebuild the currently visible page so its brushes flip too.
        NavigateToPage(_currentPageTag);
    }

    private void ToggleTheme()
    {
        var nextFramework = ThemeManager.CurrentTheme == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;

        // Detach every NavigationViewItem from its current panel *before* the
        // framework re-applies NavigationView's control template. Without this
        // step, the old template root is removed wholesale but its child items
        // keep their _parent pointer, which causes the subsequent RefreshMenuItems
        // to throw on the first Add ("Visual already has a parent") — the exact
        // symptom we saw as "sidebar loses most items after theme switch".
        DetachAllNavigationItemsFromVisualTree();

        ThemeManager.ApplyTheme(nextFramework);

        // The items are intentionally detached during the framework theme swap,
        // so refresh their implicit styles against the newly-loaded dictionary.
        foreach (var item in NavigationView.MenuItems.OfType<NavigationViewItem>())
        {
            item.UpdateDefaultStyle();
        }

        GalleryTheme.CurrentMode = nextFramework == ThemeVariant.Dark
            ? GalleryThemeMode.Dark
            : GalleryThemeMode.Light;
    }

    /// <summary>
    /// Remove each registered NavigationViewItem from its current panel before
    /// the framework replaces the NavigationView template. UpdateMenuItems will
    /// then attach it to the freshly-created menu panel.
    /// </summary>
    private void DetachAllNavigationItemsFromVisualTree()
    {
        if (NavigationView == null)
        {
            return;
        }

        foreach (var item in NavigationView.MenuItems.OfType<NavigationViewItem>())
        {
            if (item.VisualParent is Panel panel)
            {
                panel.Children.Remove(item);
            }
        }
    }

    private void OnThemeToggleClick(object sender, RoutedEventArgs e)
    {
        ToggleTheme();
    }

    private void UpdateThemeToggleVisual()
    {
        var isDark = GalleryTheme.CurrentMode == GalleryThemeMode.Dark;
        ThemeMoonIcon.Visibility = isDark ? Visibility.Visible : Visibility.Collapsed;
        ThemeSunIcon.Visibility = isDark ? Visibility.Collapsed : Visibility.Visible;
        ThemeToggleLabel.Text = isDark ? "Dark theme" : "Light theme";
    }

    private void ApplyGalleryChromePalette()
    {
        Background = GalleryTheme.BackgroundDarkBrush;
        Foreground = GalleryTheme.TextPrimaryBrush;
        NavigationView.PaneBackground = GalleryTheme.BackgroundMediumBrush;
        NavigationView.ContentBackground = GalleryTheme.TransparentBrush;
        ContentHost.Background = GalleryTheme.ShellBackgroundBrush;
    }

    private void OnUtilityLinkClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string url })
        {
            OpenExternalUrl(url);
        }
    }

    private static ImageSource LoadEmbeddedImage(string resourceName)
    {
        using var stream = typeof(MainWindow).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' was not found.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return ImageSourceLoader.FromBytes(memory.ToArray());
    }

    private static void OpenExternalUrl(string url)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Gallery] Failed to open URL '{url}': {ex}");
        }
    }

    private void InitializeTitleBarSearch()
    {
        _titleBarSearchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(80)
        };
        _titleBarSearchTimer.Tick += OnTitleBarSearchTimerTick;
        UpdateTitleBarSearchPlaceholder();
    }

    private void OnTitleBarSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateTitleBarSearchPlaceholder();

        if (_titleBarSearchTimer == null)
        {
            ApplyTitleBarSearch();
            return;
        }

        _titleBarSearchTimer.Stop();
        _titleBarSearchTimer.Start();
    }

    private void OnTitleBarSearchTimerTick(object? sender, EventArgs e)
    {
        _titleBarSearchTimer?.Stop();
        ApplyTitleBarSearch();
    }

    private void ApplyTitleBarSearch()
    {
        _titleBarSearchTimer?.Stop();

        var query = TitleBarSearchBox.Text ?? string.Empty;
        if (_currentPageTag != "home")
        {
            NavigateToPage("home");
            SelectNavigationItem("home");
            return;
        }

        _homePage?.SetSearchQuery(query);
    }

    private void UpdateTitleBarSearchPlaceholder()
    {
        TitleBarSearchPlaceholder.Visibility = string.IsNullOrEmpty(TitleBarSearchBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void OnTitleBarSearchKeyDown(object? sender, RoutedEventArgs e)
    {
        if (e is not KeyEventArgs keyArgs) return;

        if (keyArgs.Key == Key.Enter)
        {
            ApplyTitleBarSearch();
            if (_homePage?.TryOpenBestMatch() == true)
            {
                keyArgs.Handled = true;
            }

            return;
        }

        if (keyArgs.Key == Key.Escape && !string.IsNullOrWhiteSpace(TitleBarSearchBox.Text))
        {
            TitleBarSearchBox.Text = string.Empty;
            keyArgs.Handled = true;
        }
    }

    private void OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem?.Tag is string tag &&
            !string.Equals(_currentPageTag, tag, StringComparison.Ordinal))
        {
            NavigateToPage(tag);
        }
    }

    private void OnPageNavigationRequested(object? sender, NavigationRequestEventArgs e)
    {
        NavigateToPage(e.PageTag);
        if (NavigationView != null)
        {
            NavigationView.SelectedItem = null;
        }
    }

    private void SelectNavigationItem(string pageTag)
    {
        if (NavigationView == null) return;

        foreach (var item in NavigationView.MenuItems.OfType<NavigationViewItem>())
        {
            if (string.Equals(item.Tag as string, pageTag, StringComparison.Ordinal))
            {
                NavigationView.SelectedItem = item;
                break;
            }
        }
    }

    private void NavigateToPage(string pageTag)
    {
        if (NavigationView == null) return;

        _currentPageTag = pageTag;
        _homePage = null;

        SectionBadge.Visibility = pageTag == "section" ? Visibility.Visible : Visibility.Collapsed;

        UIElement? pageContent = null;
        string? homeSearchQuery = null;

        if (GalleryFeatureAvailability.TryGetUnavailableReason(
                pageTag,
                out var unavailableFeature,
                out var unavailableReason,
                out var unavailableGuidance))
        {
            SetNavigationContent(new PlatformUnavailablePage(
                unavailableFeature,
                unavailableReason,
                unavailableGuidance));
            return;
        }

        // Special handling for pages that need navigation event wiring
        if (pageTag == "home")
        {
            var homePage = new HomePage();
            homePage.NavigationRequested += OnPageNavigationRequested;
            homeSearchQuery = TitleBarSearchBox.Text ?? string.Empty;
            _homePage = homePage;
            pageContent = homePage;
        }
        else if (pageTag == "design-tokens")
        {
            pageContent = new DesignTokensPage();
        }
        else if (pageTag == "theme-studio")
        {
            pageContent = new ThemeStudioPage();
        }
        else if (GalleryComponentCatalog.PageFactories.TryGetValue(pageTag, out var factory))
        {
            try
            {
                pageContent = factory();
            }
            catch (Exception ex)
            {
                // Page creation failed - show error details
                System.Diagnostics.Debug.WriteLine($"[Gallery] Failed to create page '{pageTag}': {ex}");
                SetNavigationContent(new NavigationStatePage(pageTag, ex));
                return;
            }
        }

        SetNavigationContent(pageContent ?? new NavigationStatePage(pageTag));

        if (_homePage is { } attachedHomePage &&
            !string.IsNullOrWhiteSpace(homeSearchQuery))
        {
            Dispatcher.InvokeAsync(
                () =>
                {
                    if (ReferenceEquals(_homePage, attachedHomePage) &&
                        string.Equals(_currentPageTag, "home", StringComparison.Ordinal))
                    {
                        attachedHomePage.SetSearchQuery(TitleBarSearchBox.Text ?? string.Empty);
                    }
                },
                DispatcherPriority.Background);
        }
    }

    private void SetNavigationContent(UIElement content)
    {
        if (NavigationView == null) return;

        CleanupContentInputState();

        if (ContentHost.Content is UIElement outgoingContent)
        {
            outgoingContent.Visibility = Visibility.Collapsed;
        }

        ContentHost.Content = content;
        ContentHost.ScrollToVerticalOffset(0);
        ContentHost.ScrollToHorizontalOffset(0);
        if (!ReferenceEquals(NavigationView.Content, ContentHost))
        {
            NavigationView.SetContent(ContentHost);
        }

        RelaxConstrainedTextControlWidths(content);
    }

    private void CleanupContentInputState()
    {
        if (ContentHost.Content is not UIElement currentContent)
        {
            return;
        }

        if (Keyboard.FocusedElement is UIElement focused &&
            IsDescendantOf(focused, currentContent))
        {
            Keyboard.ClearFocus();
        }

        var captured = UIElement.MouseCapturedElement;
        if (captured != null && IsDescendantOf(captured, currentContent))
        {
            captured.ReleaseMouseCapture();
        }
    }

    private static bool IsDescendantOf(UIElement element, UIElement root)
    {
        Visual? current = element;
        while (current != null)
        {
            if (ReferenceEquals(current, root))
            {
                return true;
            }

            current = current.VisualParent;
        }

        return false;
    }

    private static void RelaxConstrainedTextControlWidths(Visual? root)
    {
        if (root == null)
        {
            return;
        }

        if (root is FrameworkElement element)
        {
            RelaxConstrainedTextControlWidth(element);
        }

        for (var i = 0; i < root.VisualChildrenCount; i++)
        {
            RelaxConstrainedTextControlWidths(root.GetVisualChild(i));
        }
    }

    private static void RelaxConstrainedTextControlWidth(FrameworkElement element)
    {
        var widthLimit = GetLocalWidthLimit(element);
        if (double.IsNaN(widthLimit) || widthLimit <= 0)
        {
            return;
        }

        var representativeText = GetRepresentativeText(element);
        if (string.IsNullOrWhiteSpace(representativeText))
        {
            return;
        }

        var requiredWidth = EstimateRequiredWidth(element, representativeText);
        if (requiredWidth <= 0 || widthLimit + 4 >= requiredWidth)
        {
            return;
        }

        ClearLocalWidthConstraint(element, FrameworkElement.WidthProperty);
        ClearLocalWidthConstraint(element, FrameworkElement.MaxWidthProperty);
    }

    private static double GetLocalWidthLimit(FrameworkElement element)
    {
        var hasLimit = false;
        var widthLimit = double.PositiveInfinity;

        if (TryGetLocalFiniteDouble(element, FrameworkElement.WidthProperty, out var width))
        {
            widthLimit = width;
            hasLimit = true;
        }

        if (TryGetLocalFiniteDouble(element, FrameworkElement.MaxWidthProperty, out var maxWidth))
        {
            widthLimit = hasLimit ? Math.Min(widthLimit, maxWidth) : maxWidth;
            hasLimit = true;
        }

        return hasLimit ? widthLimit : double.NaN;
    }

    private static bool TryGetLocalFiniteDouble(DependencyObject target, DependencyProperty property, out double value)
    {
        value = 0;
        var localValue = target.ReadLocalValue(property);
        if (ReferenceEquals(localValue, DependencyProperty.UnsetValue) ||
            localValue is not double numericValue ||
            double.IsNaN(numericValue) ||
            double.IsInfinity(numericValue) ||
            numericValue <= 0)
        {
            return false;
        }

        value = numericValue;
        return true;
    }

    private static void ClearLocalWidthConstraint(DependencyObject target, DependencyProperty property)
    {
        if (!ReferenceEquals(target.ReadLocalValue(property), DependencyProperty.UnsetValue))
        {
            target.ClearValue(property);
        }
    }

    private static string? GetRepresentativeText(FrameworkElement element)
    {
        return element switch
        {
            AppBarButton appBarButton => NormalizeText(appBarButton.Label),
            AppBarToggleButton appBarToggleButton => NormalizeText(appBarToggleButton.Label),
            ComboBox comboBox => GetComboBoxRepresentativeText(comboBox),
            SplitButton splitButton => ExtractDisplayText(splitButton.Content),
            ButtonBase buttonBase => ExtractDisplayText(buttonBase.Content),
            _ => null
        };
    }

    private static string? GetComboBoxRepresentativeText(ComboBox comboBox)
    {
        var fontSize = comboBox.FontSize > 0 ? comboBox.FontSize : 14;
        string? representativeText = null;

        representativeText = TakeWiderText(representativeText, NormalizeText(comboBox.PlaceholderText), fontSize);
        representativeText = TakeWiderText(representativeText, NormalizeText(comboBox.Text), fontSize);
        representativeText = TakeWiderText(representativeText, ExtractDisplayText(comboBox.SelectedItem), fontSize);

        foreach (var item in comboBox.Items)
        {
            representativeText = TakeWiderText(representativeText, ExtractDisplayText(item), fontSize);
        }

        return representativeText;
    }

    private static string? TakeWiderText(string? current, string? candidate, double fontSize)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return current;
        }

        if (string.IsNullOrWhiteSpace(current))
        {
            return candidate;
        }

        return EstimateTextWidth(candidate, fontSize) > EstimateTextWidth(current, fontSize)
            ? candidate
            : current;
    }

    private static string? ExtractDisplayText(object? content)
    {
        return content switch
        {
            null => null,
            string text => NormalizeText(text),
            TextBlock textBlock => NormalizeText(textBlock.Text),
            AccessText accessText => NormalizeText(accessText.Text),
            ComboBoxItem comboBoxItem => ExtractDisplayText(comboBoxItem.Content),
            UIElement => null,
            _ => NormalizeText(content.ToString())
        };
    }

    private static string? NormalizeText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text.Trim();
    }

    private static double EstimateRequiredWidth(FrameworkElement element, string text)
    {
        var fontSize = element switch
        {
            AppBarButton or AppBarToggleButton => 10,
            Control { FontSize: > 0 } sizedControl => sizedControl.FontSize,
            _ => 14
        };

        var paddingWidth = element is Control control
            ? control.Padding.Left + control.Padding.Right
            : 0;

        var extraWidth = 14.0;
        if (element is ComboBox)
        {
            extraWidth += 30;
        }
        else if (element is SplitButton)
        {
            extraWidth += 34;
        }
        else if (element is AppBarButton or AppBarToggleButton)
        {
            extraWidth += 12;
        }

        return EstimateTextWidth(text, fontSize) + paddingWidth + extraWidth;
    }

    private static double EstimateTextWidth(string text, double fontSize)
    {
        var widthUnits = 0.0;

        foreach (var ch in text)
        {
            if (char.IsWhiteSpace(ch))
            {
                widthUnits += 0.35;
            }
            else if (IsWideCharacter(ch))
            {
                widthUnits += 1.0;
            }
            else if (char.IsUpper(ch))
            {
                widthUnits += 0.72;
            }
            else if (char.IsDigit(ch))
            {
                widthUnits += 0.62;
            }
            else if (char.IsPunctuation(ch))
            {
                widthUnits += 0.45;
            }
            else
            {
                widthUnits += 0.58;
            }
        }

        return Math.Max(widthUnits, 1) * fontSize;
    }

    private static bool IsWideCharacter(char ch)
    {
        return (ch >= '\u1100' && ch <= '\u11FF') ||
               (ch >= '\u2E80' && ch <= '\uA4CF') ||
               (ch >= '\uAC00' && ch <= '\uD7A3') ||
               (ch >= '\uF900' && ch <= '\uFAFF') ||
               (ch >= '\uFE10' && ch <= '\uFE6F') ||
               (ch >= '\uFF01' && ch <= '\uFF60') ||
               (ch >= '\uFFE0' && ch <= '\uFFE6');
    }

}
