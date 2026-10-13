using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Reproduces the original detail workbench's responsive Inspector placement.
/// This is strictly View layout behavior and contains no application state.
/// </summary>
public static class GalleryWorkbenchLayoutBehavior
{
    private const double InspectorWidth = 316;

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(GalleryWorkbenchLayoutBehavior),
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
        var inspectorColumn =
            root.FindName("InspectorColumn") as ColumnDefinition;
        var inspectorPanel =
            root.FindName("InspectorPanel") as Border;
        if (inspectorColumn == null ||
            inspectorPanel == null ||
            width <= 0)
        {
            return;
        }

        var sideBySide = width >= 1060;
        inspectorColumn.Width =
            new GridLength(sideBySide ? InspectorWidth : 0);
        Grid.SetColumn(inspectorPanel, sideBySide ? 1 : 0);
        Grid.SetRow(inspectorPanel, sideBySide ? 1 : 2);
        Grid.SetColumnSpan(inspectorPanel, sideBySide ? 1 : 2);
        inspectorPanel.Margin = sideBySide
            ? new Thickness(16, 0, 0, 0)
            : new Thickness(0, 16, 0, 0);
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
