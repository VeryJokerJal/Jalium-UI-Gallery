using Jalium.UI;
using Jalium.UI.Markup;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Releases a Razor section when its owning view leaves the visual tree.
/// This is view lifecycle infrastructure and contains no application state.
/// </summary>
public static class RazorSectionCleanupBehavior
{
    public static readonly DependencyProperty SectionNameProperty =
        DependencyProperty.RegisterAttached(
            "SectionName",
            typeof(string),
            typeof(RazorSectionCleanupBehavior),
            new PropertyMetadata(null, OnSectionNameChanged));

    public static void SetSectionName(
        DependencyObject element,
        string value) =>
        element.SetValue(SectionNameProperty, value);

    public static string? GetSectionName(
        DependencyObject element) =>
        element.GetValue(SectionNameProperty) as string;

    private static void OnSectionNameChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        element.Unloaded -= OnUnloaded;
        if (!string.IsNullOrWhiteSpace(args.NewValue as string))
        {
            element.Unloaded += OnUnloaded;
        }
    }

    private static void OnUnloaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is FrameworkElement element &&
            GetSectionName(element) is { Length: > 0 } sectionName)
        {
            RazorExpressionRegistry.UnregisterSection(sectionName);
        }
    }
}
