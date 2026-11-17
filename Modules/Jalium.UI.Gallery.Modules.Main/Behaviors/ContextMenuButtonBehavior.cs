using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Opens a declaratively attached ContextMenu from a normal left-button click.
/// </summary>
public static class ContextMenuButtonBehavior
{
    public static readonly DependencyProperty OpenOnClickProperty =
        DependencyProperty.RegisterAttached(
            "OpenOnClick",
            typeof(bool),
            typeof(ContextMenuButtonBehavior),
            new PropertyMetadata(false, OnOpenOnClickChanged));

    public static bool GetOpenOnClick(DependencyObject element) =>
        (bool)(element.GetValue(OpenOnClickProperty) ?? false);

    public static void SetOpenOnClick(
        DependencyObject element,
        bool value) =>
        element.SetValue(OpenOnClickProperty, value);

    private static void OnOpenOnClickChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Button button)
        {
            return;
        }

        button.Click -= OnButtonClick;
        if (args.NewValue is true)
        {
            button.Click += OnButtonClick;
        }
    }

    private static void OnButtonClick(object? sender, RoutedEventArgs args)
    {
        if (sender is not Button button ||
            ContextMenuService.GetContextMenu(button) is not { } menu)
        {
            return;
        }

        menu.DataContext = button.DataContext;
        menu.PlacementTarget = button;
        menu.Placement = PlacementMode.Bottom;
        menu.IsOpen = true;
    }
}
