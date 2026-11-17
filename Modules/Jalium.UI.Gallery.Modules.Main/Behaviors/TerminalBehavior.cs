using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class TerminalBehavior
{
    public static readonly DependencyProperty ObserveProcessProperty =
        DependencyProperty.RegisterAttached(
            "ObserveProcess",
            typeof(bool),
            typeof(TerminalBehavior),
            new PropertyMetadata(false, OnObserveProcessChanged));

    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(TerminalRequest),
            typeof(TerminalBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static bool GetObserveProcess(DependencyObject element) =>
        (bool)(element.GetValue(ObserveProcessProperty) ?? false);

    public static void SetObserveProcess(
        DependencyObject element,
        bool value) =>
        element.SetValue(ObserveProcessProperty, value);

    public static TerminalRequest? GetRequest(DependencyObject element) =>
        element.GetValue(RequestProperty) as TerminalRequest;

    public static void SetRequest(
        DependencyObject element,
        TerminalRequest? value) =>
        element.SetValue(RequestProperty, value);

    private static void OnObserveProcessChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Terminal terminal)
        {
            return;
        }

        terminal.TitleChanged -= OnTitleChanged;
        terminal.ProcessExited -= OnProcessExited;
        if (args.NewValue is true)
        {
            terminal.TitleChanged += OnTitleChanged;
            terminal.ProcessExited += OnProcessExited;
        }
    }

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Terminal terminal ||
            args.NewValue is not TerminalRequest request)
        {
            return;
        }

        switch (request.Action)
        {
            case TerminalAction.Clear:
                terminal.Clear();
                break;
            case TerminalAction.Copy:
                terminal.Copy();
                break;
            case TerminalAction.Paste:
                terminal.Paste();
                break;
            case TerminalAction.Restart:
                terminal.StopShell();
                terminal.Clear();
                terminal.StartShell();
                Sink(terminal)?.SetProcessRunning();
                break;
            case TerminalAction.Write:
                terminal.Write(request.Output);
                break;
            case TerminalAction.ClearAndWrite:
                terminal.Clear();
                terminal.Write(request.Output);
                break;
        }
    }

    private static void OnTitleChanged(object? sender, RoutedEventArgs args)
    {
        if (sender is Terminal terminal)
        {
            Sink(terminal)?.SetTerminalTitle(terminal.Title);
        }
    }

    private static void OnProcessExited(
        object? sender,
        TerminalProcessExitedEventArgs args)
    {
        if (sender is Terminal terminal)
        {
            Sink(terminal)?.SetProcessExited(args.ExitCode);
        }
    }

    private static ITerminalStateSink? Sink(Terminal terminal) =>
        terminal.DataContext as ITerminalStateSink;
}
