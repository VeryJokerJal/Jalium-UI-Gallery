using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class SplitButtonFlyoutBehavior
{
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.RegisterAttached(
            "IsOpen",
            typeof(bool),
            typeof(SplitButtonFlyoutBehavior),
            new PropertyMetadata(false, OnIsOpenChanged));

    private static readonly DependencyProperty ClosedHandlerProperty =
        DependencyProperty.RegisterAttached(
            "ClosedHandler",
            typeof(EventHandler),
            typeof(SplitButtonFlyoutBehavior),
            new PropertyMetadata(null));

    public static bool GetIsOpen(DependencyObject element) =>
        (bool)(element.GetValue(IsOpenProperty) ?? false);

    public static void SetIsOpen(DependencyObject element, bool value) =>
        element.SetValue(IsOpenProperty, value);

    private static void OnIsOpenChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not SplitButton splitButton ||
            splitButton.Flyout == null)
        {
            return;
        }

        if (args.NewValue is true)
        {
            EnsureClosedHandler(splitButton);
            splitButton.Flyout.ShowAt(splitButton);
        }
        else
        {
            splitButton.Flyout.Hide();
        }
    }

    private static void EnsureClosedHandler(SplitButton splitButton)
    {
        if (splitButton.GetValue(ClosedHandlerProperty) is EventHandler)
        {
            return;
        }

        EventHandler handler = (_, _) =>
            splitButton.SetCurrentValue(IsOpenProperty, false);
        splitButton.SetValue(ClosedHandlerProperty, handler);
        splitButton.Flyout!.Closed += handler;
    }
}
