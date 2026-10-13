using System.Collections;
using System.Runtime.CompilerServices;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Materializes the keyboard-accelerator metadata carried by XamlUICommand as
/// real input bindings on a UI element. InputBinding is not in the visual tree
/// and therefore cannot inherit the page DataContext directly.
/// </summary>
public static class XamlUICommandInputBehavior
{
    private static readonly ConditionalWeakTable<
        UIElement,
        List<InputBinding>> GeneratedBindings = new();

    public static readonly DependencyProperty CommandsProperty =
        DependencyProperty.RegisterAttached(
            "Commands",
            typeof(IEnumerable),
            typeof(XamlUICommandInputBehavior),
            new PropertyMetadata(null, OnCommandsChanged));

    public static void SetCommands(
        DependencyObject element,
        IEnumerable? value) =>
        element.SetValue(CommandsProperty, value);

    public static IEnumerable? GetCommands(DependencyObject element) =>
        element.GetValue(CommandsProperty) as IEnumerable;

    private static void OnCommandsChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        if (element is FrameworkElement frameworkElement)
        {
            frameworkElement.DataContextChanged -= OnDataContextChanged;
            frameworkElement.DataContextChanged += OnDataContextChanged;
        }

        ClearGeneratedBindings(element);
        if (args.NewValue is not IEnumerable commands)
        {
            return;
        }

        AddGeneratedBindings(element, commands);
    }

    private static void AddGeneratedBindings(
        UIElement element,
        IEnumerable commands)
    {
        var generated = GeneratedBindings.GetOrCreateValue(element);
        foreach (var command in commands.OfType<XamlUICommand>())
        {
            foreach (var accelerator in
                     command.KeyboardAccelerators.OfType<KeyGesture>())
            {
                var binding = new KeyBinding(command, accelerator);
                element.InputBindings.Add(binding);
                generated.Add(binding);
            }
        }
    }

    private static void OnDataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs args)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        ClearGeneratedBindings(element);
        if (args.NewValue is not null &&
            GetCommands(element) is { } commands)
        {
            AddGeneratedBindings(element, commands);
        }
    }

    private static void ClearGeneratedBindings(UIElement element)
    {
        if (!GeneratedBindings.TryGetValue(element, out var generated))
        {
            return;
        }

        foreach (var binding in generated)
        {
            element.InputBindings.Remove(binding);
        }

        generated.Clear();
    }
}
