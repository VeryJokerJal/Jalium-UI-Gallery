using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Headless;

/// <summary>
/// Makes the production Gallery visual tree deterministic when it is laid out
/// and rendered without an operating-system window or UI message loop.
/// </summary>
public static class GalleryHeadlessSupport
{
    private static readonly MapTileSource s_deterministicTileSource = new()
    {
        UrlTemplate = string.Empty,
        Attribution = "Local deterministic tiles",
        MinZoom = 0,
        MaxZoom = 20,
        TileSize = 256,
    };

    /// <summary>Waits for artwork required by the first catalog viewport.</summary>
    public static void WaitForStartupAssets() =>
        Themes.SaasCardBackgrounds.WaitForStartupPrefetch();

    /// <summary>
    /// Applies shell navigation and responsive state synchronously without
    /// calling <see cref="Window.Show"/> or constructing a platform window.
    /// </summary>
    public static void PrepareShell(
        MainWindow window,
        string pageTag,
        double logicalWidth)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);

        if (window.DataContext is MainWindowViewModel viewModel)
        {
            viewModel.CurrentPageTag = pageTag;
        }

        GalleryShellBehavior.SetCurrentPageTag(window, pageTag);
        GalleryShellBehavior.ApplyResponsiveShell(window, logicalWidth);

        foreach (var host in Descendants<GalleryNavigationHost>(window))
        {
            host.PageTag = pageTag;
        }
    }

    /// <summary>Releases event subscriptions installed by <see cref="PrepareShell"/>.</summary>
    public static void ReleaseShell(MainWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        GalleryShellBehavior.SetIsEnabled(window, false);
    }

    /// <summary>
    /// Removes external/native-child dependencies that cannot be represented in
    /// an offline bitmap while preserving their layout slots and surrounding UI.
    /// </summary>
    public static void PrepareDeterministicContent(
        DependencyObject root,
        string pageTag)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);

        var prepareWebView = string.Equals(pageTag, "webview", StringComparison.OrdinalIgnoreCase);
        var prepareMap = string.Equals(pageTag, "mapview", StringComparison.OrdinalIgnoreCase);
        if (!prepareWebView && !prepareMap)
        {
            return;
        }

        foreach (var current in Descendants<DependencyObject>(root, includeRoot: true))
        {
            if (prepareWebView && current is WebView webView)
            {
                webView.Source = null;
                webView.Visibility = Visibility.Hidden;
            }
            else if (prepareMap && current is MapView mapView)
            {
                mapView.TileSource = s_deterministicTileSource;
            }
        }
    }

    private static IEnumerable<T> Descendants<T>(
        DependencyObject root,
        bool includeRoot = false)
        where T : DependencyObject
    {
        var pending = new Stack<DependencyObject>();
        if (includeRoot)
        {
            pending.Push(root);
        }
        else
        {
            PushChildren(root, pending);
        }

        while (pending.Count > 0)
        {
            var current = pending.Pop();
            if (current is T match)
            {
                yield return match;
            }

            PushChildren(current, pending);
        }
    }

    private static void PushChildren(
        DependencyObject root,
        Stack<DependencyObject> pending)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var index = count - 1; index >= 0; index--)
        {
            if (VisualTreeHelper.GetChild(root, index) is { } child)
            {
                pending.Push(child);
            }
        }
    }
}
