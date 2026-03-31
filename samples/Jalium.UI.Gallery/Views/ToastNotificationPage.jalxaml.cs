using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ToastNotificationPage.jalxaml demonstrating toast notification functionality.
/// </summary>
public partial class ToastNotificationPage : Page
{
    public ToastNotificationPage()
    {
        InitializeComponent();

        // Basic toast buttons
        if (InfoToastButton != null)
            InfoToastButton.Click += (s, e) => ShowToast(ToastSeverity.Information, "Information", "This is an informational message.");

        if (SuccessToastButton != null)
            SuccessToastButton.Click += (s, e) => ShowToast(ToastSeverity.Success, "Success", "Operation completed successfully.");

        if (WarningToastButton != null)
            WarningToastButton.Click += (s, e) => ShowToast(ToastSeverity.Warning, "Warning", "Please review before proceeding.");

        if (ErrorToastButton != null)
            ErrorToastButton.Click += (s, e) => ShowToast(ToastSeverity.Error, "Error", "An error occurred during the operation.");

        // Action toast button
        if (ShowActionToastButton != null)
            ShowActionToastButton.Click += OnShowActionToastClick;

        // Custom toast button
        if (ShowCustomToastButton != null)
            ShowCustomToastButton.Click += OnShowCustomToastClick;

        // Dismiss all button
        if (DismissAllButton != null)
            DismissAllButton.Click += (s, e) => ToastHost?.DismissAll();

        // Duration slider
        if (DurationSlider != null)
            DurationSlider.ValueChanged += OnDurationChanged;

        // Position combobox
        if (PositionComboBox != null)
            PositionComboBox.SelectionChanged += OnPositionChanged;
    }

    private void ShowToast(ToastSeverity severity, string title, string message)
    {
        if (ToastHost == null) return;

        var duration = GetConfiguredDuration();
        var toast = ToastHost.Show(severity, title, message, duration);
        toast.IsClosable = ShowCloseCheckBox?.IsChecked ?? true;
        toast.IsAutoDismissEnabled = PauseOnHoverCheckBox?.IsChecked ?? true;
    }

    private void OnShowActionToastClick(object? sender, EventArgs e)
    {
        if (ToastHost == null) return;

        var duration = GetConfiguredDuration();
        var toast = new ToastNotificationItem
        {
            Severity = ToastSeverity.Information,
            Title = "Download Complete",
            Message = "Your file has been downloaded.",
            IsClosable = ShowCloseCheckBox?.IsChecked ?? true,
            IsAutoDismissEnabled = PauseOnHoverCheckBox?.IsChecked ?? true,
            Duration = duration
        };

        toast.ActionButtonClick += (s, args) =>
        {
            // Show a follow-up toast when action is clicked
            ToastHost.ShowSuccess("File Opened", "The file was opened successfully.");
        };

        ToastHost.ShowToast(toast);
    }

    private void OnShowCustomToastClick(object? sender, EventArgs e)
    {
        if (ToastHost == null) return;

        var duration = GetConfiguredDuration();
        var toast = ToastHost.Show(
            ToastSeverity.Information,
            "Custom Toast",
            $"Duration: {duration.TotalSeconds:F0}s — Position: {ToastHost.Position}",
            duration);

        toast.IsClosable = ShowCloseCheckBox?.IsChecked ?? true;
        toast.IsAutoDismissEnabled = PauseOnHoverCheckBox?.IsChecked ?? true;
    }

    private TimeSpan GetConfiguredDuration()
    {
        var seconds = DurationSlider?.Value ?? 5;
        return TimeSpan.FromSeconds(seconds);
    }

    private void OnDurationChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DurationText != null)
        {
            DurationText.Text = $"{e.NewValue:F0}s";
        }
    }

    private void OnPositionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ToastHost == null || PositionComboBox == null) return;

        ToastHost.Position = PositionComboBox.SelectedIndex switch
        {
            0 => ToastPosition.TopRight,
            1 => ToastPosition.TopLeft,
            2 => ToastPosition.TopCenter,
            3 => ToastPosition.BottomRight,
            4 => ToastPosition.BottomLeft,
            5 => ToastPosition.BottomCenter,
            _ => ToastPosition.TopRight
        };
    }
}
