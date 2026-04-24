using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for NotifyIconPage.jalxaml demonstrating system tray icon functionality.
/// </summary>
public partial class NotifyIconPage : Page
{
    private NotifyIcon? _notifyIcon;
    private readonly List<string> _eventLog = new();

    public NotifyIconPage()
    {
        InitializeComponent();

        // Show/Hide icon
        if (ShowIconButton != null)
            ShowIconButton.Click += OnShowIconClick;
        if (HideIconButton != null)
            HideIconButton.Click += OnHideIconClick;

        // Balloon tip
        if (ShowBalloonButton != null)
            ShowBalloonButton.Click += OnShowBalloonClick;
        if (BalloonTimeoutSlider != null)
            BalloonTimeoutSlider.ValueChanged += OnBalloonTimeoutChanged;

        // Context menu toggle
        if (EnableContextMenuCheckBox != null)
            EnableContextMenuCheckBox.Checked += (s, e) => UpdateContextMenu(true);
        if (EnableContextMenuCheckBox != null)
            EnableContextMenuCheckBox.Unchecked += (s, e) => UpdateContextMenu(false);

        // Clear log
        if (ClearLogButton != null)
            ClearLogButton.Click += (s, e) =>
            {
                _eventLog.Clear();
                if (EventLogText != null)
                    EventLogText.Text = "Log cleared.";
            };

        LoadCodeExamples();
    }

    private NotifyIcon EnsureNotifyIcon()
    {
        if (_notifyIcon != null) return _notifyIcon;

        _notifyIcon = new NotifyIcon
        {
            Text = TooltipBox?.Text ?? "Jalium.UI Gallery"
        };

        _notifyIcon.Click += (s, e) => LogEvent("Click");
        _notifyIcon.DoubleClick += (s, e) => LogEvent("DoubleClick");
        _notifyIcon.BalloonTipShown += (s, e) => LogEvent("BalloonTipShown");
        _notifyIcon.BalloonTipClicked += (s, e) => LogEvent("BalloonTipClicked");
        _notifyIcon.BalloonTipClosed += (s, e) => LogEvent("BalloonTipClosed");

        UpdateContextMenu(EnableContextMenuCheckBox?.IsChecked ?? true);

        return _notifyIcon;
    }

    private void OnShowIconClick(object? sender, EventArgs e)
    {
        var icon = EnsureNotifyIcon();
        icon.Text = TooltipBox?.Text ?? "Jalium.UI Gallery";
        icon.Visible = true;
        LogEvent("Icon shown");
    }

    private void OnHideIconClick(object? sender, EventArgs e)
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            LogEvent("Icon hidden");
        }
    }

    private void OnShowBalloonClick(object? sender, EventArgs e)
    {
        var icon = EnsureNotifyIcon();
        if (!icon.Visible) icon.Visible = true;

        var title = BalloonTitleBox?.Text ?? "Balloon Tip";
        var text = BalloonTextBox?.Text ?? "Message";
        var timeout = (int)(BalloonTimeoutSlider?.Value ?? 5);
        var balloonIcon = (BalloonIconComboBox?.SelectedIndex ?? 1) switch
        {
            0 => BalloonTipIcon.None,
            1 => BalloonTipIcon.Info,
            2 => BalloonTipIcon.Warning,
            3 => BalloonTipIcon.Error,
            _ => BalloonTipIcon.Info
        };

        icon.ShowBalloonTip(timeout, title, text, balloonIcon);
        LogEvent($"BalloonTip: [{balloonIcon}] {title}");
    }

    private void UpdateContextMenu(bool enabled)
    {
        if (_notifyIcon == null) return;

        if (enabled)
        {
            var menu = new ContextMenu();
            var showItem = new MenuItem { Header = "Show Window" };
            showItem.Click += (s, e) => LogEvent("Menu: Show Window");
            var settingsItem = new MenuItem { Header = "Settings" };
            settingsItem.Click += (s, e) => LogEvent("Menu: Settings");
            var aboutItem = new MenuItem { Header = "About" };
            aboutItem.Click += (s, e) => LogEvent("Menu: About");
            var separator = new Separator();
            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, e) => LogEvent("Menu: Exit");

            menu.Items.Add(showItem);
            menu.Items.Add(settingsItem);
            menu.Items.Add(aboutItem);
            menu.Items.Add(separator);
            menu.Items.Add(exitItem);
            _notifyIcon.ContextMenu = menu;
        }
        else
        {
            _notifyIcon.ContextMenu = null;
        }
    }

    private void LogEvent(string eventName)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] {eventName}";
        _eventLog.Add(entry);
        // Keep last 20 entries
        if (_eventLog.Count > 20)
            _eventLog.RemoveAt(0);

        Dispatcher.Invoke(() =>
        {
            if (EventLogText != null)
                EventLogText.Text = string.Join("\n", _eventLog);
        });
    }

    private void OnBalloonTimeoutChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (BalloonTimeoutText != null)
            BalloonTimeoutText.Text = $"{e.NewValue:F0}s";
    }

    private const string XamlExample = @"<!-- NotifyIcon is a code-only component (no visual tree).
     Create and configure from code-behind. -->
<StackPanel>
    <Button x:Name=""ShowIconButton"" Content=""Show Icon""/>
    <Button x:Name=""HideIconButton"" Content=""Hide Icon""/>
    <Button x:Name=""ShowBalloonButton"" Content=""Show Balloon""/>
</StackPanel>";

    private const string CSharpExample = @"// Create a system tray icon
var notifyIcon = new NotifyIcon
{
    Text = ""My Application"",
    Visible = true
};

// Handle click events
notifyIcon.Click += (s, e) =>
    Debug.WriteLine(""Tray icon clicked"");
notifyIcon.DoubleClick += (s, e) =>
    mainWindow.Activate();

// Show a balloon tip notification
notifyIcon.ShowBalloonTip(
    timeout: 5,
    title: ""Update Available"",
    text: ""A new version is ready to install."",
    icon: BalloonTipIcon.Info);

// Handle balloon tip events
notifyIcon.BalloonTipClicked += (s, e) =>
    Process.Start(""https://example.com/update"");

// Add a context menu
var menu = new ContextMenu();
var showItem = new MenuItem { Header = ""Show"" };
showItem.Click += (s, e) => mainWindow.Show();
var exitItem = new MenuItem { Header = ""Exit"" };
exitItem.Click += (s, e) => Application.Current.Shutdown();
menu.Items.Add(showItem);
menu.Items.Add(new Separator());
menu.Items.Add(exitItem);
notifyIcon.ContextMenu = menu;

// Hide or dispose when done
notifyIcon.Visible = false;
notifyIcon.Dispose();";

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
