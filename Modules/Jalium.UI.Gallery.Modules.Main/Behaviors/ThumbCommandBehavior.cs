using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts Thumb drag events to ViewModel commands without View code-behind.
/// </summary>
public static class ThumbCommandBehavior
{
    public static readonly DependencyProperty DragStartedCommandProperty =
        DependencyProperty.RegisterAttached(
            "DragStartedCommand",
            typeof(ICommand),
            typeof(ThumbCommandBehavior),
            new PropertyMetadata(null, OnDragStartedCommandChanged));

    public static readonly DependencyProperty DragDeltaCommandProperty =
        DependencyProperty.RegisterAttached(
            "DragDeltaCommand",
            typeof(ICommand),
            typeof(ThumbCommandBehavior),
            new PropertyMetadata(null, OnDragDeltaCommandChanged));

    public static readonly DependencyProperty DragCompletedCommandProperty =
        DependencyProperty.RegisterAttached(
            "DragCompletedCommand",
            typeof(ICommand),
            typeof(ThumbCommandBehavior),
            new PropertyMetadata(null, OnDragCompletedCommandChanged));

    public static void SetDragStartedCommand(
        DependencyObject element,
        ICommand? value) =>
        element.SetValue(DragStartedCommandProperty, value);

    public static ICommand? GetDragStartedCommand(
        DependencyObject element) =>
        element.GetValue(DragStartedCommandProperty) as ICommand;

    public static void SetDragDeltaCommand(
        DependencyObject element,
        ICommand? value) =>
        element.SetValue(DragDeltaCommandProperty, value);

    public static ICommand? GetDragDeltaCommand(
        DependencyObject element) =>
        element.GetValue(DragDeltaCommandProperty) as ICommand;

    public static void SetDragCompletedCommand(
        DependencyObject element,
        ICommand? value) =>
        element.SetValue(DragCompletedCommandProperty, value);

    public static ICommand? GetDragCompletedCommand(
        DependencyObject element) =>
        element.GetValue(DragCompletedCommandProperty) as ICommand;

    private static void OnDragStartedCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Thumb thumb)
        {
            return;
        }

        thumb.DragStarted -= OnDragStarted;
        if (args.NewValue is ICommand)
        {
            thumb.DragStarted += OnDragStarted;
        }
    }

    private static void OnDragDeltaCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Thumb thumb)
        {
            return;
        }

        thumb.DragDelta -= OnDragDelta;
        if (args.NewValue is ICommand)
        {
            thumb.DragDelta += OnDragDelta;
        }
    }

    private static void OnDragCompletedCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Thumb thumb)
        {
            return;
        }

        thumb.DragCompleted -= OnDragCompleted;
        if (args.NewValue is ICommand)
        {
            thumb.DragCompleted += OnDragCompleted;
        }
    }

    private static void OnDragStarted(
        object sender,
        DragStartedEventArgs args) =>
        Execute(GetDragStartedCommand((DependencyObject)sender), null);

    private static void OnDragDelta(
        object sender,
        DragDeltaEventArgs args) =>
        Execute(
            GetDragDeltaCommand((DependencyObject)sender),
            args.HorizontalChange);

    private static void OnDragCompleted(
        object sender,
        DragCompletedEventArgs args) =>
        Execute(GetDragCompletedCommand((DependencyObject)sender), null);

    private static void Execute(ICommand? command, object? parameter)
    {
        if (command?.CanExecute(parameter) == true)
        {
            command.Execute(parameter);
        }
    }
}
