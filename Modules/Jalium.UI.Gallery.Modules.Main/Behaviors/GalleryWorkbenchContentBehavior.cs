using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Places the runtime-resolved component View into the declarative workbench
/// presenter. The component never enters the ViewModel layer.
/// </summary>
public static class GalleryWorkbenchContentBehavior
{
    public static readonly DependencyProperty DemoContentProperty =
        DependencyProperty.RegisterAttached(
            "DemoContent",
            typeof(UIElement),
            typeof(GalleryWorkbenchContentBehavior),
            new PropertyMetadata(null, OnDemoContentChanged));

    private static readonly DependencyProperty LoadedAttachedProperty =
        DependencyProperty.RegisterAttached(
            "LoadedAttached",
            typeof(bool),
            typeof(GalleryWorkbenchContentBehavior),
            new PropertyMetadata(false));

    public static void SetDemoContent(
        DependencyObject element,
        UIElement? value) =>
        element.SetValue(DemoContentProperty, value);

    public static UIElement? GetDemoContent(
        DependencyObject element) =>
        element.GetValue(DemoContentProperty) as UIElement;

    private static void OnDemoContentChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FrameworkElement root)
        {
            return;
        }

        Apply(root);
        if (!(bool)(root.GetValue(LoadedAttachedProperty) ?? false))
        {
            root.SetValue(LoadedAttachedProperty, true);
            root.Loaded += OnLoaded;
        }
    }

    private static void OnLoaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is FrameworkElement root)
        {
            Apply(root);
        }
    }

    private static void Apply(FrameworkElement root)
    {
        var content = GetDemoContent(root);
        if (root.FindName("DemoContentPresenter") is
            ContentPresenter presenter)
        {
            presenter.Content = content;
        }

        if (content is Control demoControl)
        {
            demoControl.Foreground =
                GalleryTheme.TextPrimaryBrush;
        }

        if (content != null)
        {
            ApplyDefaultButtonForegrounds(content);
        }
    }

    private static void ApplyDefaultButtonForegrounds(
        DependencyObject element)
    {
        if (element is Button button &&
            button.IsEnabled &&
            ReferenceEquals(
                button.ReadLocalValue(Control.ForegroundProperty),
                DependencyProperty.UnsetValue))
        {
            button.Foreground =
                GalleryTheme.TextPrimaryBrush;
        }

        var childCount =
            VisualTreeHelper.GetChildrenCount(element);
        for (var index = 0; index < childCount; index++)
        {
            if (VisualTreeHelper.GetChild(element, index) is
                DependencyObject child)
            {
                ApplyDefaultButtonForegrounds(child);
            }
        }
    }
}
