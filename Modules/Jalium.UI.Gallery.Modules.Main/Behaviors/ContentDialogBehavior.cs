using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class ContentDialogBehavior
{
    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(ContentDialogRequest),
            typeof(ContentDialogBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static ContentDialogRequest? GetRequest(
        DependencyObject element) =>
        element.GetValue(RequestProperty) as ContentDialogRequest;

    public static void SetRequest(
        DependencyObject element,
        ContentDialogRequest? value) =>
        element.SetValue(RequestProperty, value);

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is ContentDialog dialog &&
            args.NewValue is ContentDialogRequest)
        {
            _ = ShowAsync(dialog);
        }
    }

    private static async Task ShowAsync(ContentDialog dialog)
    {
        var result = await dialog.ShowAsync(
            ContentDialogPlacement.InPlace);
        if (dialog.DataContext is IContentDialogStateSink sink)
        {
            sink.SetInlineResult(result);
        }
    }
}
