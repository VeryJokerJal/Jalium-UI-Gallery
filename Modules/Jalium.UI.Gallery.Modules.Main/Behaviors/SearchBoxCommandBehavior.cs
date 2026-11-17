using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class SearchBoxCommandBehavior
{
    public static readonly DependencyProperty SubmitCommandProperty =
        DependencyProperty.RegisterAttached(
            "SubmitCommand",
            typeof(ICommand),
            typeof(SearchBoxCommandBehavior),
            new PropertyMetadata(null, OnSubmitCommandChanged));

    public static ICommand? GetSubmitCommand(DependencyObject element) =>
        element.GetValue(SubmitCommandProperty) as ICommand;

    public static void SetSubmitCommand(
        DependencyObject element,
        ICommand? value) =>
        element.SetValue(SubmitCommandProperty, value);

    private static void OnSubmitCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TextBox textBox)
        {
            return;
        }

        if (args.OldValue is ICommand)
        {
            textBox.KeyDown -= OnKeyDown;
        }

        if (args.NewValue is ICommand)
        {
            textBox.KeyDown += OnKeyDown;
        }
    }

    private static void OnKeyDown(object? sender, KeyEventArgs args)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (args.Key == Key.Enter)
        {
            var command = GetSubmitCommand(textBox);
            if (command?.CanExecute(null) == true)
            {
                command.Execute(null);
                args.Handled = true;
            }
        }
        else if (args.Key == Key.Escape &&
                 !string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = string.Empty;
            args.Handled = true;
        }
    }
}
