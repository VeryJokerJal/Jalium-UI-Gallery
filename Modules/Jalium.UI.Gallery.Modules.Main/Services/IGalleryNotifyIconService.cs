using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Services;

/// <summary>View-independent operations exposed by the system tray integration.</summary>
public interface IGalleryNotifyIconService
{
    event EventHandler<string>? Activity;

    void Show(string tooltip);

    void Hide();

    void ShowBalloon(
        int timeoutSeconds,
        string title,
        string text,
        BalloonTipIcon icon);

    void SetContextMenuEnabled(bool enabled);
}
