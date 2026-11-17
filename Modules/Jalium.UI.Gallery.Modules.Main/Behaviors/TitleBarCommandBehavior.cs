using System.Windows.Input;
using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts the view-only caption events exposed by <see cref="TitleBar"/> to
/// commands. The behavior contains no application state; all decisions remain
/// in the page ViewModel.
/// </summary>
public static class TitleBarCommandBehavior
{
    public static readonly DependencyProperty MinimizeCommandProperty =
        DependencyProperty.RegisterAttached(
            "MinimizeCommand",
            typeof(ICommand),
            typeof(TitleBarCommandBehavior),
            new PropertyMetadata(null, OnMinimizeCommandChanged));

    public static readonly DependencyProperty MaximizeRestoreCommandProperty =
        DependencyProperty.RegisterAttached(
            "MaximizeRestoreCommand",
            typeof(ICommand),
            typeof(TitleBarCommandBehavior),
            new PropertyMetadata(null, OnMaximizeRestoreCommandChanged));

    public static readonly DependencyProperty CloseCommandProperty =
        DependencyProperty.RegisterAttached(
            "CloseCommand",
            typeof(ICommand),
            typeof(TitleBarCommandBehavior),
            new PropertyMetadata(null, OnCloseCommandChanged));

    public static ICommand? GetMinimizeCommand(DependencyObject element) =>
        element.GetValue(MinimizeCommandProperty) as ICommand;

    public static void SetMinimizeCommand(DependencyObject element, ICommand? value) =>
        element.SetValue(MinimizeCommandProperty, value);

    public static ICommand? GetMaximizeRestoreCommand(DependencyObject element) =>
        element.GetValue(MaximizeRestoreCommandProperty) as ICommand;

    public static void SetMaximizeRestoreCommand(DependencyObject element, ICommand? value) =>
        element.SetValue(MaximizeRestoreCommandProperty, value);

    public static ICommand? GetCloseCommand(DependencyObject element) =>
        element.GetValue(CloseCommandProperty) as ICommand;

    public static void SetCloseCommand(DependencyObject element, ICommand? value) =>
        element.SetValue(CloseCommandProperty, value);

    private static void OnMinimizeCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TitleBar titleBar)
        {
            return;
        }

        if (args.OldValue is ICommand)
        {
            titleBar.MinimizeClicked -= OnMinimizeClicked;
        }

        if (args.NewValue is ICommand)
        {
            titleBar.MinimizeClicked += OnMinimizeClicked;
        }
    }

    private static void OnMaximizeRestoreCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TitleBar titleBar)
        {
            return;
        }

        if (args.OldValue is ICommand)
        {
            titleBar.MaximizeRestoreClicked -= OnMaximizeRestoreClicked;
        }

        if (args.NewValue is ICommand)
        {
            titleBar.MaximizeRestoreClicked += OnMaximizeRestoreClicked;
        }
    }

    private static void OnCloseCommandChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TitleBar titleBar)
        {
            return;
        }

        if (args.OldValue is ICommand)
        {
            titleBar.CloseClicked -= OnCloseClicked;
        }

        if (args.NewValue is ICommand)
        {
            titleBar.CloseClicked += OnCloseClicked;
        }
    }

    private static void OnMinimizeClicked(object? sender, EventArgs args)
    {
        if (sender is TitleBar titleBar)
        {
            Execute(GetMinimizeCommand(titleBar));
        }
    }

    private static void OnMaximizeRestoreClicked(object? sender, EventArgs args)
    {
        if (sender is TitleBar titleBar)
        {
            Execute(GetMaximizeRestoreCommand(titleBar));
        }
    }

    private static void OnCloseClicked(object? sender, EventArgs args)
    {
        if (sender is TitleBar titleBar)
        {
            Execute(GetCloseCommand(titleBar));
        }
    }

    private static void Execute(ICommand? command)
    {
        if (command?.CanExecute(null) == true)
        {
            command.Execute(null);
        }
    }
}
