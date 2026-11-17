using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Diagnostics;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// View-side region host. A bound page tag is resolved through the composition
/// service; no shell ViewModel ever stores or constructs a View.
/// </summary>
[TemplateVisualState(Name = "Empty", GroupName = "ContentStates")]
[TemplateVisualState(Name = "Ready", GroupName = "ContentStates")]
public sealed class GalleryNavigationHost : ItemsControl
{
    public static readonly DependencyProperty PageTagProperty =
        DependencyProperty.Register(
            nameof(PageTag),
            typeof(string),
            typeof(GalleryNavigationHost),
            new PropertyMetadata("home", OnPageTagChanged));

    public static readonly DependencyProperty PageResolverProperty = DependencyProperty.Register(
        nameof(PageResolver), typeof(IGalleryPageResolver), typeof(GalleryNavigationHost),
        new PropertyMetadata(null, OnPageResolverChanged));
    private static readonly DependencyPropertyKey ActiveContentPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ActiveContent), typeof(UIElement), typeof(GalleryNavigationHost), new PropertyMetadata(null));
    public static readonly DependencyProperty ActiveContentProperty = ActiveContentPropertyKey.DependencyProperty;

    public IGalleryPageResolver? PageResolver
    {
        get => (IGalleryPageResolver?)GetValue(PageResolverProperty);
        set => SetValue(PageResolverProperty, value);
    }
    public UIElement? ActiveContent => (UIElement?)GetValue(ActiveContentProperty);

    private string? _renderedPageTag;
    private UIElement? _transientPage;
    private readonly Dictionary<string, UIElement> _pageCache =
        new(StringComparer.OrdinalIgnoreCase);

    public GalleryNavigationHost()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryNavigationHost);
        Loaded += OnLoadedRetry;
        ActivateInitialPage();
    }

    public string PageTag
    {
        get => (string)(GetValue(PageTagProperty) ?? "home");
        set => SetValue(PageTagProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateActiveContainers();
        UpdateContentState();
    }

    protected override void OnTemplateChanging()
    {
        CleanupInputState();
        base.OnTemplateChanging();
    }

    protected override bool IsItemItsOwnContainerOverride(object item) => item is GalleryPageContainer;
    protected override DependencyObject GetContainerForItemOverride() => new GalleryPageContainer();
    protected override void PrepareContainerForItem(FrameworkElement element, object item)
    {
        base.PrepareContainerForItem(element, item);
        if (element is GalleryPageContainer container)
            container.SetCurrentValue(GalleryPageContainer.IsActiveProperty, ReferenceEquals(item, ActiveContent));
    }

    private static void OnPageResolverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (GalleryNavigationHost)d;
        host.CleanupInputState();
        host.Items.Clear();
        host._pageCache.Clear();
        host._transientPage = null;
        host._renderedPageTag = null;
        host.SetValue(ActiveContentPropertyKey, null);
        host.ActivateInitialPage();
        host.UpdateContentState();
    }

    private static void OnPageTagChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not GalleryNavigationHost host ||
            args.NewValue is not string pageTag)
        {
            return;
        }

        if (host._renderedPageTag is null)
        {
            host.ActivateInitialPage();
            return;
        }

        host.Navigate(pageTag);
    }

    private void OnLoadedRetry(object? sender, RoutedEventArgs args)
    {
        if (_renderedPageTag is null)
        {
            ActivateInitialPage();
        }

        if (_renderedPageTag is not null)
        {
            Loaded -= OnLoadedRetry;
        }
    }

    private void ActivateInitialPage()
    {
        if (_renderedPageTag is not null)
        {
            return;
        }

        using (StartupDiagnostics.Begin(
            "Gallery.InitialNavigation",
            blocksUiThread: true))
        {
            Navigate(PageTag);
        }

        if (_renderedPageTag is null)
        {
            return;
        }

        Loaded -= OnLoadedRetry;
        StartupDiagnostics.Mark(
            "Gallery.InitialPageActivated",
            blocksUiThread: true);
    }

    private void Navigate(string pageTag)
    {
        if (string.IsNullOrWhiteSpace(pageTag) ||
            string.Equals(
                _renderedPageTag,
                pageTag,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var services = Application.Current?.Services;
        var resolver = PageResolver ?? services?.GetService<IGalleryPageResolver>();
        if (resolver == null)
        {
            return;
        }

        if (_renderedPageTag is null &&
            string.Equals(pageTag, "home", StringComparison.OrdinalIgnoreCase))
        {
            using (StartupDiagnostics.Begin(
                "Gallery.WaitForStartupArtwork",
                blocksUiThread: true))
            {
                SaasCardBackgrounds.WaitForStartupPrefetch();
            }
        }

        CleanupInputState();

        UIElement content;
        using (StartupDiagnostics.Begin(
            "Gallery.ResolvePage",
            blocksUiThread: true))
        {
            content = ResolveContent(resolver, pageTag);
        }

        using (StartupDiagnostics.Begin(
            "Gallery.ActivatePage",
            blocksUiThread: true))
        {
            SetValue(ActiveContentPropertyKey, content);
            if (_transientPage != null && !ReferenceEquals(_transientPage, content))
                Items.Remove(_transientPage);
            _transientPage = IsShellPage(pageTag) ? null : content;
            if (!Items.Contains(content)) Items.Add(content);
            UpdateActiveContainers();
            UpdateContentState();
        }
        _renderedPageTag = pageTag;
        ResetViewport();
    }

    private UIElement ResolveContent(
        IGalleryPageResolver resolver,
        string pageTag)
    {
        if (IsShellPage(pageTag) &&
            _pageCache.TryGetValue(pageTag, out var cached))
        {
            return cached;
        }

        var content = resolver.Resolve(pageTag);
        if (IsShellPage(pageTag))
        {
            _pageCache[pageTag] = content;
        }

        return content;
    }

    private static bool IsShellPage(string pageTag) =>
        pageTag is "home" or "design-tokens" or "theme-studio";

    private void ResetViewport()
    {
        var scrollViewer = FindAncestorScrollViewer();
        scrollViewer?.ScrollToVerticalOffset(0);
        scrollViewer?.ScrollToHorizontalOffset(0);
    }

    private void CleanupInputState()
    {
        if (ActiveContent is not UIElement currentContent)
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

    private ScrollViewer? FindAncestorScrollViewer()
    {
        DependencyObject? current = VisualParent;
        while (current != null)
        {
            if (current is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            current = (current as Visual)?.VisualParent;
        }

        return null;
    }

    private static bool IsDescendantOf(UIElement element, UIElement root)
    {
        DependencyObject? current = element;
        while (current != null)
        {
            if (ReferenceEquals(current, root))
            {
                return true;
            }

            current = (current as Visual)?.VisualParent;
        }

        return false;
    }

    private void UpdateActiveContainers()
    {
        foreach (var item in Items)
            if (ItemContainerGenerator.ContainerFromItem(item) is GalleryPageContainer container)
                container.SetCurrentValue(GalleryPageContainer.IsActiveProperty, ReferenceEquals(item, ActiveContent));
    }
    private void UpdateContentState() => VisualStateManager.GoToState(this, ActiveContent == null ? "Empty" : "Ready", false);
}

/// <summary>A themed page container whose inactive visual tree remains in the resource scope.</summary>
[TemplateVisualState(Name = "Active", GroupName = "ActivationStates")]
[TemplateVisualState(Name = "Inactive", GroupName = "ActivationStates")]
public sealed class GalleryPageContainer : ContentControl
{
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(GalleryPageContainer),
        new PropertyMetadata(false, (d, _) => ((GalleryPageContainer)d).UpdateState()));
    public bool IsActive { get => (bool)GetValue(IsActiveProperty)!; set => SetValue(IsActiveProperty, value); }
    public GalleryPageContainer()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryPageContainer);
        UseTemplateContentManagement();
    }
    public override void OnApplyTemplate() { base.OnApplyTemplate(); UpdateState(); }
    private void UpdateState() => VisualStateManager.GoToState(this, IsActive ? "Active" : "Inactive", false);
}
