using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Keeps a virtualized page body bounded to its ancestor ScrollViewer's
/// viewport. This is layout adaptation only.
/// </summary>
public static class ViewportHeightBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(ViewportHeightBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    private static readonly DependencyProperty HostProperty =
        DependencyProperty.RegisterAttached(
            "Host",
            typeof(ScrollViewer),
            typeof(ViewportHeightBehavior),
            new PropertyMetadata(null));

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        element.Loaded -= OnLoaded;
        element.Unloaded -= OnUnloaded;
        element.LayoutUpdated -= OnLayoutUpdated;
        DetachHost(element);

        if (args.NewValue is true)
        {
            element.Loaded += OnLoaded;
            element.Unloaded += OnUnloaded;
            EnsureLayoutRetry(element);
            TryAttachHost(element);
        }
    }

    private static void OnLoaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        if (!TryAttachHost(element))
        {
            EnsureLayoutRetry(element);
        }
    }

    private static void OnLayoutUpdated(
        object? sender,
        EventArgs args)
    {
        if (sender is FrameworkElement element &&
            TryAttachHost(element))
        {
            element.LayoutUpdated -= OnLayoutUpdated;
        }
    }

    private static void OnUnloaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is FrameworkElement element)
        {
            element.LayoutUpdated -= OnLayoutUpdated;
            DetachHost(element);
        }
    }

    private static void OnHostSizeChanged(
        object sender,
        SizeChangedEventArgs args)
    {
        if (sender is not ScrollViewer host)
        {
            return;
        }

        foreach (var child in EnumerateChildren(host))
        {
            if (child.GetValue(HostProperty) == host)
            {
                UpdateHeight(child, host);
            }
        }
    }

    private static void DetachHost(FrameworkElement element)
    {
        if (element.GetValue(HostProperty) is ScrollViewer host)
        {
            host.SizeChanged -= OnHostSizeChanged;
            element.ClearValue(HostProperty);
        }
    }

    private static bool TryAttachHost(FrameworkElement element)
    {
        if (element.GetValue(HostProperty) is ScrollViewer current)
        {
            UpdateHeight(element, current);
            return true;
        }

        var host = FindAncestorScrollViewer(element);
        if (host is null)
        {
            return false;
        }

        element.SetValue(HostProperty, host);
        host.SizeChanged += OnHostSizeChanged;
        UpdateHeight(element, host);
        return true;
    }

    private static void EnsureLayoutRetry(FrameworkElement element)
    {
        element.LayoutUpdated -= OnLayoutUpdated;
        element.LayoutUpdated += OnLayoutUpdated;
    }

    private static void UpdateHeight(
        FrameworkElement element,
        ScrollViewer host)
    {
        var viewport = host.ViewportHeight;
        if (viewport <= 0 ||
            double.IsNaN(viewport) ||
            double.IsInfinity(viewport))
        {
            viewport =
                host.ActualHeight -
                host.Padding.Top -
                host.Padding.Bottom;
        }

        if (viewport <= 0 ||
            double.IsNaN(viewport) ||
            double.IsInfinity(viewport))
        {
            return;
        }

        element.Height = Math.Max(
            240,
            viewport -
            element.Margin.Top -
            element.Margin.Bottom);
    }

    private static ScrollViewer? FindAncestorScrollViewer(
        Visual start)
    {
        for (var current = start.VisualParent;
             current is not null;
             current = current.VisualParent)
        {
            if (current is ScrollViewer viewer)
            {
                return viewer;
            }
        }

        return null;
    }

    private static IEnumerable<FrameworkElement> EnumerateChildren(
        Visual root)
    {
        if (root is FrameworkElement element)
        {
            yield return element;
        }

        var childCount =
            VisualTreeHelper.GetChildrenCount(root);
        for (var index = 0; index < childCount; index++)
        {
            if (VisualTreeHelper.GetChild(root, index) is not
                Visual child)
            {
                continue;
            }

            foreach (var descendant in EnumerateChildren(child))
            {
                yield return descendant;
            }
        }
    }
}
