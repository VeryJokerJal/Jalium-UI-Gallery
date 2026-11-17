using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Converts ViewModel requests into EditControl document operations and reports
/// document/caret metadata without exposing the control to the ViewModel.
/// </summary>
public static class EditControlBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(EditControlBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(EditorRequest),
            typeof(EditControlBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    public static void SetRequest(
        DependencyObject element,
        EditorRequest? value) =>
        element.SetValue(RequestProperty, value);

    public static EditorRequest? GetRequest(
        DependencyObject element) =>
        element.GetValue(RequestProperty) as EditorRequest;

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not EditControl editor)
        {
            return;
        }

        editor.Document.Changed -= OnDocumentChanged;
        editor.CaretPositionChanged -= OnCaretPositionChanged;
        editor.Loaded -= OnLoaded;

        if (args.NewValue is true)
        {
            editor.Document.Changed += OnDocumentChanged;
            editor.CaretPositionChanged += OnCaretPositionChanged;
            editor.Loaded += OnLoaded;
            PublishState(editor);
        }
    }

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not EditControl editor ||
            args.NewValue is not EditorRequest request)
        {
            return;
        }

        switch (request.Action)
        {
            case EditorAction.Undo:
                editor.Undo();
                break;
            case EditorAction.Redo:
                editor.Redo();
                break;
            case EditorAction.SelectAll:
                editor.SelectAll();
                break;
        }

        PublishState(editor);
    }

    private static void OnLoaded(
        object? sender,
        RoutedEventArgs args)
    {
        if (sender is EditControl editor)
        {
            PublishState(editor);
        }
    }

    private static void OnDocumentChanged(
        object? sender,
        EventArgs args)
    {
        if (sender is EditControl editor)
        {
            PublishState(editor);
        }
    }

    private static void OnCaretPositionChanged(
        object? sender,
        EventArgs args)
    {
        if (sender is EditControl editor)
        {
            PublishState(editor);
        }
    }

    private static void PublishState(EditControl editor)
    {
        var offset = Math.Min(
            editor.CaretOffset,
            editor.Document.TextLength);
        var line = editor.Document.GetLineByOffset(offset);
        (editor.DataContext as IEditControlStateSink)
            ?.UpdateEditorPosition(
                editor.Document.LineCount,
                editor.Document.TextLength,
                line.LineNumber,
                offset - line.Offset);
    }
}
