using System.Windows.Input;
using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class NavigationViewCommandBehavior
{
    public static readonly DependencyProperty NavigateCommandProperty =
        DependencyProperty.RegisterAttached(
            "NavigateCommand",
            typeof(ICommand),
            typeof(NavigationViewCommandBehavior),
            new PropertyMetadata(null, OnNavigateCommandChanged));

    public static ICommand? GetNavigateCommand(DependencyObject element) =>
        element.GetValue(NavigateCommandProperty) as ICommand;

    public static void SetNavigateCommand(
        DependencyObject element,
        ICommand? value) =>
        element.SetValue(NavigateCommandProperty, value);

    private static void OnNavigateCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not NavigationView navigationView)
        {
            return;
        }

        if (args.OldValue is ICommand)
        {
            navigationView.SelectionChanged -= OnSelectionChanged;
        }

        if (args.NewValue is ICommand)
        {
            navigationView.SelectionChanged += OnSelectionChanged;
        }
    }

    private static void OnSelectionChanged(
        object? sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (sender is not NavigationView navigationView ||
            args.SelectedItem?.Tag is not string pageTag)
        {
            return;
        }

        var command = GetNavigateCommand(navigationView);
        if (command?.CanExecute(pageTag) == true)
        {
            command.Execute(pageTag);
        }
    }
}
