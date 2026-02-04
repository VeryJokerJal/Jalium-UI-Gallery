using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ToastNotificationPage.jalxaml demonstrating toast notification functionality.
/// </summary>
public partial class ToastNotificationPage : Page
{
    public ToastNotificationPage()
    {
        InitializeComponent();

        // Set up toast buttons
        if (InfoToastButton != null)
        {
            InfoToastButton.Click += (s, e) => ShowToast("Information", "This is an informational message.", "info");
        }

        if (SuccessToastButton != null)
        {
            SuccessToastButton.Click += (s, e) => ShowToast("Success", "Operation completed successfully.", "success");
        }

        if (WarningToastButton != null)
        {
            WarningToastButton.Click += (s, e) => ShowToast("Warning", "Please review before proceeding.", "warning");
        }

        if (ErrorToastButton != null)
        {
            ErrorToastButton.Click += (s, e) => ShowToast("Error", "An error occurred during the operation.", "error");
        }

        if (ShowActionToastButton != null)
        {
            ShowActionToastButton.Click += OnShowActionToastClick;
        }

        if (ShowCustomToastButton != null)
        {
            ShowCustomToastButton.Click += OnShowCustomToastClick;
        }

        // Set up duration slider
        if (DurationSlider != null)
        {
            DurationSlider.ValueChanged += OnDurationChanged;
        }
    }

    private void ShowToast(string title, string message, string type)
    {
        // Create a toast notification using the Windows toast API
        var xmlContent = $@"
            <toast>
                <visual>
                    <binding template=""ToastText02"">
                        <text id=""1"">{title}</text>
                        <text id=""2"">{message}</text>
                    </binding>
                </visual>
            </toast>";

        var notification = new ToastNotification(xmlContent);
        notification.Tag = type;

        var notifier = ToastNotificationManager.CreateToastNotifier();
        notifier.Show(notification);
    }

    private void OnShowActionToastClick(object? sender, EventArgs e)
    {
        var xmlContent = @"
            <toast>
                <visual>
                    <binding template=""ToastText02"">
                        <text id=""1"">Download Complete</text>
                        <text id=""2"">Your file has been downloaded.</text>
                    </binding>
                </visual>
                <actions>
                    <action content=""Open"" arguments=""open""/>
                    <action content=""Dismiss"" arguments=""dismiss""/>
                </actions>
            </toast>";

        var notification = new ToastNotification(xmlContent);
        notification.Tag = "download";

        var notifier = ToastNotificationManager.CreateToastNotifier();
        notifier.Show(notification);
    }

    private void OnShowCustomToastClick(object? sender, EventArgs e)
    {
        var duration = DurationSlider?.Value ?? 5;

        var xmlContent = $@"
            <toast duration=""long"">
                <visual>
                    <binding template=""ToastText02"">
                        <text id=""1"">Custom Toast</text>
                        <text id=""2"">This toast demonstrates custom settings.</text>
                    </binding>
                </visual>
            </toast>";

        var notification = new ToastNotification(xmlContent);
        notification.ExpirationTime = DateTimeOffset.Now.AddSeconds(duration);

        var notifier = ToastNotificationManager.CreateToastNotifier();
        notifier.Show(notification);
    }

    private void OnDurationChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DurationText != null)
        {
            DurationText.Text = $"{e.NewValue:F0}s";
        }
    }
}
