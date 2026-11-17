using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class ToastHostBehavior
{
    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(ToastRequest),
            typeof(ToastHostBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static ToastRequest? GetRequest(DependencyObject element) =>
        element.GetValue(RequestProperty) as ToastRequest;

    public static void SetRequest(
        DependencyObject element,
        ToastRequest? value) =>
        element.SetValue(RequestProperty, value);

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not ToastNotificationHost host ||
            args.NewValue is not ToastRequest request)
        {
            return;
        }

        if (request.DismissAll)
        {
            host.DismissAll();
            return;
        }

        var toast = new ToastNotificationItem
        {
            Severity = request.Severity,
            Title = request.Title,
            Message = request.Message,
            Duration = request.Duration,
            IsClosable = request.IsClosable,
            IsAutoDismissEnabled = request.IsAutoDismissEnabled
        };
        if (request.HasAction)
        {
            toast.ActionButtonClick += (_, _) =>
                host.ShowSuccess(
                    "File Opened",
                    "The file was opened successfully.");
        }

        host.ShowToast(toast);
    }
}
