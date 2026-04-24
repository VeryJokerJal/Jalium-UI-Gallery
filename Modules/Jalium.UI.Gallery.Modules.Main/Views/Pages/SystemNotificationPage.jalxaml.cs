using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Notifications;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for SystemNotificationPage.jalxaml demonstrating OS-level system notifications.
/// </summary>
public partial class SystemNotificationPage : Page
{
    public SystemNotificationPage()
    {
        InitializeComponent();

        // Initialize SystemNotificationManager
        var mgr = SystemNotificationManager.Current;
        mgr.Initialize("Jalium.UI.Gallery", "Jalium.UI Gallery");

        // Platform support
        if (SupportedText != null)
        {
            SupportedText.Text = mgr.IsSupported
                ? "System notifications are supported on this platform."
                : "System notifications are NOT supported on this platform.";
            SupportedText.Foreground = new Media.SolidColorBrush(
                mgr.IsSupported ? Media.Color.FromRgb(0x4C, 0xAF, 0x50) : Media.Color.FromRgb(0xF4, 0x43, 0x36));
        }

        // Basic notification
        if (SendBasicButton != null)
            SendBasicButton.Click += OnSendBasicClick;

        // Priority buttons
        if (LowPriorityButton != null)
            LowPriorityButton.Click += (s, e) => SendWithPriority(NotificationPriority.Low);
        if (NormalPriorityButton != null)
            NormalPriorityButton.Click += (s, e) => SendWithPriority(NotificationPriority.Normal);
        if (HighPriorityButton != null)
            HighPriorityButton.Click += (s, e) => SendWithPriority(NotificationPriority.High);

        // Action notification
        if (SendActionButton != null)
            SendActionButton.Click += OnSendActionClick;

        // Advanced options
        if (SendAdvancedButton != null)
            SendAdvancedButton.Click += OnSendAdvancedClick;
        if (RemoveTagButton != null)
            RemoveTagButton.Click += OnRemoveTagClick;
        if (ClearAllButton != null)
            ClearAllButton.Click += (s, e) => mgr.ClearAll();

        // Expiration slider
        if (ExpirationSlider != null)
            ExpirationSlider.ValueChanged += OnExpirationChanged;

        LoadCodeExamples();
    }

    private void OnSendBasicClick(object? sender, EventArgs e)
    {
        var title = BasicTitleBox?.Text ?? "Hello";
        var body = BasicBodyBox?.Text ?? "Notification body";
        SystemNotificationManager.Current.Show(title, body);
    }

    private void SendWithPriority(NotificationPriority priority)
    {
        var content = new NotificationContent
        {
            Title = $"{priority} Priority",
            Body = $"This notification was sent with {priority} priority.",
            Priority = priority
        };
        SystemNotificationManager.Current.Show(content);
    }

    private void OnSendActionClick(object? sender, EventArgs e)
    {
        var content = new NotificationContent
        {
            Title = "Download Complete",
            Body = "report_2026.pdf has been downloaded successfully.",
            Actions =
            {
                new NotificationAction("open", "Open File"),
                new NotificationAction("folder", "Show in Folder")
            }
        };

        var handle = SystemNotificationManager.Current.Show(content);
        handle.Activated += (s, args) =>
        {
            var action = args.ActionId ?? "body";
            Dispatcher.Invoke(() =>
            {
                if (ActionResultText != null)
                    ActionResultText.Text = $"Activated: ActionId = \"{action}\"";
            });
        };
        handle.Dismissed += (s, args) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (ActionResultText != null)
                    ActionResultText.Text = $"Dismissed: Reason = {args.Reason}";
            });
        };
    }

    private void OnSendAdvancedClick(object? sender, EventArgs e)
    {
        var seconds = ExpirationSlider?.Value ?? 10;
        var content = new NotificationContent
        {
            Title = "Tagged Notification",
            Body = $"Tag: {TagBox?.Text}, Group: {GroupBox?.Text}",
            Tag = TagBox?.Text,
            Group = GroupBox?.Text,
            Silent = SilentCheckBox?.IsChecked ?? false,
            Expiration = seconds > 0 ? TimeSpan.FromSeconds(seconds) : null
        };
        SystemNotificationManager.Current.Show(content);
    }

    private void OnRemoveTagClick(object? sender, EventArgs e)
    {
        var tag = TagBox?.Text;
        var group = GroupBox?.Text;
        if (!string.IsNullOrEmpty(tag))
            SystemNotificationManager.Current.Remove(tag, string.IsNullOrEmpty(group) ? null : group);
    }

    private void OnExpirationChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (ExpirationText != null)
            ExpirationText.Text = e.NewValue > 0 ? $"{e.NewValue:F0}s" : "Default";
    }

    private const string XamlExample = @"<!-- System notifications are code-only (no XAML element).
     Use SystemNotificationManager from code-behind. -->
<Button x:Name=""SendButton"" Content=""Send Notification""/>
<Button x:Name=""ClearButton"" Content=""Clear All""/>";

    private const string CSharpExample = @"// Initialize once at app startup
var mgr = SystemNotificationManager.Current;
mgr.Initialize(""MyApp"", ""My Application"");

// Simple notification
mgr.Show(""Hello"", ""This is a system notification."");

// Notification with full content
var content = new NotificationContent
{
    Title = ""Download Complete"",
    Body = ""report.pdf has been downloaded."",
    Priority = NotificationPriority.Normal,
    Silent = false,
    Tag = ""download-1"",
    Group = ""downloads"",
    Expiration = TimeSpan.FromSeconds(10),
    Actions =
    {
        new NotificationAction(""open"", ""Open File""),
        new NotificationAction(""folder"", ""Show in Folder"")
    }
};

// Show and handle activation
var handle = mgr.Show(content);
handle.Activated += (s, args) =>
{
    if (args.ActionId == ""open"")
        Process.Start(filePath);
};
handle.Dismissed += (s, args) =>
{
    Debug.WriteLine($""Dismissed: {args.Reason}"");
};

// Remove specific notification by tag
mgr.Remove(""download-1"", ""downloads"");

// Clear all notifications
mgr.ClearAll();

// Check platform support
if (mgr.IsSupported)
    mgr.Show(""Supported"", ""Notifications work!"");";

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }
}
