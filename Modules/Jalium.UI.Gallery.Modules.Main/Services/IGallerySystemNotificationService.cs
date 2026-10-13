using Jalium.UI.Notifications;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGallerySystemNotificationService
{
    bool IsSupported { get; }

    string SupportMessage { get; }

    void Show(string title, string body);

    void ShowPriority(NotificationPriority priority);

    void ShowWithActions(IProgress<string> progress);

    void ShowAdvanced(
        string tag,
        string group,
        bool silent,
        double expirationSeconds);

    void Remove(string tag, string group);

    void ClearAll();
}
