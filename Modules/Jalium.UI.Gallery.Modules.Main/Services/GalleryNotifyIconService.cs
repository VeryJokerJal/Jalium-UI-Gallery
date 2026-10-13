using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Services;

/// <summary>Owns the platform NotifyIcon so ViewModels do not manipulate Views.</summary>
internal sealed class GalleryNotifyIconService : IGalleryNotifyIconService, IDisposable
{
    private NotifyIcon? _notifyIcon;
    private bool _contextMenuEnabled = true;

    public event EventHandler<string>? Activity;

    public void Show(string tooltip)
    {
        var icon = EnsureNotifyIcon();
        icon.Text = string.IsNullOrWhiteSpace(tooltip)
            ? "Jalium.UI Gallery"
            : tooltip;
        icon.Visible = true;
        Report("Icon shown");
    }

    public void Hide()
    {
        if (_notifyIcon == null)
        {
            return;
        }

        _notifyIcon.Visible = false;
        Report("Icon hidden");
    }

    public void ShowBalloon(
        int timeoutSeconds,
        string title,
        string text,
        BalloonTipIcon icon)
    {
        var notifyIcon = EnsureNotifyIcon();
        notifyIcon.Visible = true;
        notifyIcon.ShowBalloonTip(
            Math.Max(1, timeoutSeconds),
            title,
            text,
            icon);
        Report($"BalloonTip: [{icon}] {title}");
    }

    public void SetContextMenuEnabled(bool enabled)
    {
        _contextMenuEnabled = enabled;
        if (_notifyIcon != null)
        {
            _notifyIcon.ContextMenu = enabled ? CreateContextMenu() : null;
        }
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
        _notifyIcon = null;
    }

    private NotifyIcon EnsureNotifyIcon()
    {
        if (_notifyIcon != null)
        {
            return _notifyIcon;
        }

        _notifyIcon = new NotifyIcon
        {
            Text = "Jalium.UI Gallery"
        };
        _notifyIcon.Click += (_, _) => Report("Click");
        _notifyIcon.DoubleClick += (_, _) => Report("DoubleClick");
        _notifyIcon.BalloonTipShown += (_, _) => Report("BalloonTipShown");
        _notifyIcon.BalloonTipClicked += (_, _) => Report("BalloonTipClicked");
        _notifyIcon.BalloonTipClosed += (_, _) => Report("BalloonTipClosed");
        _notifyIcon.ContextMenu =
            _contextMenuEnabled ? CreateContextMenu() : null;
        return _notifyIcon;
    }

    private ContextMenu CreateContextMenu()
    {
        var menu = new ContextMenu();
        menu.Items.Add(CreateMenuItem("Show Window"));
        menu.Items.Add(CreateMenuItem("Settings"));
        menu.Items.Add(CreateMenuItem("About"));
        menu.Items.Add(new Separator());
        menu.Items.Add(CreateMenuItem("Exit"));
        return menu;
    }

    private MenuItem CreateMenuItem(string label)
    {
        var item = new MenuItem { Header = label };
        item.Click += (_, _) => Report($"Menu: {label}");
        return item;
    }

    private void Report(string message)
    {
        Activity?.Invoke(this, message);
    }
}

internal sealed class NullGalleryNotifyIconService : IGalleryNotifyIconService
{
    public static NullGalleryNotifyIconService Instance { get; } = new();

    public event EventHandler<string>? Activity
    {
        add { }
        remove { }
    }

    public void Show(string tooltip) { }

    public void Hide() { }

    public void ShowBalloon(
        int timeoutSeconds,
        string title,
        string text,
        BalloonTipIcon icon)
    {
    }

    public void SetContextMenuEnabled(bool enabled) { }
}
