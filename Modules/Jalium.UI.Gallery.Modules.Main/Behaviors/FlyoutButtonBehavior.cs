using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts a button click to the view-only act of positioning a flyout. The
/// command and all observable state remain on the button's ViewModel binding.
/// </summary>
public static class FlyoutButtonBehavior
{
    public static readonly DependencyProperty FlyoutProperty =
        DependencyProperty.RegisterAttached(
            "Flyout",
            typeof(FlyoutBase),
            typeof(FlyoutButtonBehavior),
            new PropertyMetadata(null, OnFlyoutChanged));

    public static FlyoutBase? GetFlyout(DependencyObject element) =>
        element.GetValue(FlyoutProperty) as FlyoutBase;

    public static void SetFlyout(
        DependencyObject element,
        FlyoutBase? value) =>
        element.SetValue(FlyoutProperty, value);

    private static void OnFlyoutChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Button button)
        {
            return;
        }

        if (args.OldValue is FlyoutBase)
        {
            button.Click -= OnButtonClick;
        }

        if (args.NewValue is FlyoutBase)
        {
            button.Click += OnButtonClick;
        }
    }

    private static void OnButtonClick(object? sender, RoutedEventArgs args)
    {
        if (sender is not Button button ||
            GetFlyout(button) is not { } flyout)
        {
            return;
        }

        if (flyout is CommandBarFlyout commandBarFlyout)
        {
            foreach (var command in commandBarFlyout.PrimaryCommands
                         .Concat(commandBarFlyout.SecondaryCommands)
                         .OfType<FrameworkElement>())
            {
                command.DataContext = button.DataContext;
            }
        }

        flyout.ShowAt(button);
    }
}
