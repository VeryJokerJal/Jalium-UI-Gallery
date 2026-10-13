using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Keeps the Design Tokens header readable at phone widths without placing
/// responsive layout logic in the view's code-behind.
/// </summary>
public static class DesignTokensLayoutBehavior
{
    internal const double CompactHeaderBreakpoint = 480;

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(DesignTokensLayoutBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    internal static void ApplyResponsiveLayout(
        Grid root,
        double width)
    {
        if (root.FindName("ThemeActions") is not StackPanel themeActions)
        {
            return;
        }

        themeActions.Visibility =
            width > 0 && width < CompactHeaderBreakpoint
                ? Visibility.Collapsed
                : Visibility.Visible;
    }

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Grid root)
        {
            return;
        }

        root.Loaded -= OnLoaded;
        root.SizeChanged -= OnSizeChanged;
        if (args.NewValue is true)
        {
            root.Loaded += OnLoaded;
            root.SizeChanged += OnSizeChanged;
            if (root.IsLoaded)
            {
                ApplyResponsiveLayout(root, root.ActualWidth);
            }
        }
    }

    private static void OnLoaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is Grid root)
        {
            ApplyResponsiveLayout(root, root.ActualWidth);
        }
    }

    private static void OnSizeChanged(
        object sender,
        SizeChangedEventArgs args)
    {
        if (sender is Grid root)
        {
            ApplyResponsiveLayout(root, args.NewSize.Width);
        }
    }
}
