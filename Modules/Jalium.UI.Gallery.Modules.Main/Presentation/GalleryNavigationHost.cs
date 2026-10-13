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
public sealed class GalleryNavigationHost : Border
{
    public static readonly DependencyProperty PageTagProperty =
        DependencyProperty.Register(
            nameof(PageTag),
            typeof(string),
            typeof(GalleryNavigationHost),
            new PropertyMetadata("home", OnPageTagChanged));

    private string? _renderedPageTag;
    private readonly Dictionary<string, UIElement> _pageCache =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly PageCachePanel _pageHost = new();

    public GalleryNavigationHost()
    {
        Child = _pageHost;
        Loaded += OnLoadedRetry;
        ActivateInitialPage();
    }

    public string PageTag
    {
        get => (string)(GetValue(PageTagProperty) ?? "home");
        set => SetValue(PageTagProperty, value);
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
        var resolver = services?.GetService<IGalleryPageResolver>();
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
            _pageHost.Activate(content, IsShellPage(pageTag));
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
        if (_pageHost.ActiveContent is not UIElement currentContent)
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

    /// <summary>
    /// Keeps the small set of shell pages attached to one resource scope. Only
    /// the active page contributes to desired size; inactive pages stay fully
    /// bound but are collapsed so input and accessibility expose the active page.
    /// </summary>
    private sealed class PageCachePanel : Panel
    {
        private readonly Dictionary<UIElement, Border> _slots =
            new(ReferenceEqualityComparer.Instance);
        private readonly HashSet<UIElement> _retainedPages =
            new(ReferenceEqualityComparer.Instance);

        private UIElement? _transientPage;
        private Border? _activeSlot;

        public UIElement? ActiveContent { get; private set; }

        public void Activate(UIElement content, bool retain)
        {
            ArgumentNullException.ThrowIfNull(content);

            if (ReferenceEquals(ActiveContent, content))
            {
                return;
            }

            EvictPreviousTransient(content);
            if (retain)
            {
                _retainedPages.Add(content);
            }
            else
            {
                _transientPage = content;
            }

            if (!_slots.TryGetValue(content, out var slot))
            {
                slot = new Border
                {
                    Child = content,
                    Background = Brushes.Transparent,
                    Visibility = Visibility.Collapsed
                };
                _slots.Add(content, slot);
                Children.Add(slot);
            }

            if (_activeSlot != null)
            {
                _activeSlot.Visibility = Visibility.Collapsed;
                SetZIndex(_activeSlot, 0);
            }

            ActiveContent = content;
            _activeSlot = slot;
            SetZIndex(slot, 1);
            slot.Visibility = Visibility.Visible;

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_activeSlot == null)
            {
                return default;
            }

            _activeSlot.Measure(availableSize);
            return _activeSlot.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _activeSlot?.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private void EvictPreviousTransient(UIElement incoming)
        {
            if (_transientPage == null ||
                ReferenceEquals(_transientPage, incoming) ||
                _retainedPages.Contains(_transientPage))
            {
                return;
            }

            if (_slots.Remove(_transientPage, out var slot))
            {
                Children.Remove(slot);
            }
        }
    }
}
