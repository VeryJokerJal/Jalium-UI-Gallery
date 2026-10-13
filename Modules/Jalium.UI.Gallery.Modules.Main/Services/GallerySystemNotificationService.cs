using Jalium.UI.Notifications;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public sealed class GallerySystemNotificationService
    : IGallerySystemNotificationService
{
    private readonly SystemNotificationManager _manager;

    public GallerySystemNotificationService()
    {
        _manager = SystemNotificationManager.Current;
        try
        {
            _manager.Initialize(
                "Jalium.UI.Gallery",
                "Jalium.UI Gallery");
            SupportMessage = _manager.IsSupported
                ? "System notifications are supported on this platform."
                : "System notifications are not supported on this platform.";
        }
        catch (Exception exception)
        {
            SupportMessage =
                "System notifications are not available. Reason: " +
                exception.Message;
        }

        IsSupported = _manager.IsSupported;
    }

    public bool IsSupported { get; }

    public string SupportMessage { get; }

    public void Show(string title, string body) =>
        _manager.Show(title, body);

    public void ShowPriority(NotificationPriority priority) =>
        _manager.Show(new NotificationContent
        {
            Title = $"{priority} Priority",
            Body =
                $"This notification was sent with {priority} priority.",
            Priority = priority
        });

    public void ShowWithActions(IProgress<string> progress)
    {
        var content = new NotificationContent
        {
            Title = "Download Complete",
            Body =
                "report_2026.pdf has been downloaded successfully.",
            Actions =
            {
                new NotificationAction("open", "Open File"),
                new NotificationAction("folder", "Show in Folder")
            }
        };
        var handle = _manager.Show(content);
        handle.Activated += (_, args) =>
            progress.Report(
                $"Activated: ActionId = \"{args.ActionId ?? "body"}\"");
        handle.Dismissed += (_, args) =>
            progress.Report($"Dismissed: Reason = {args.Reason}");
    }

    public void ShowAdvanced(
        string tag,
        string group,
        bool silent,
        double expirationSeconds) =>
        _manager.Show(new NotificationContent
        {
            Title = "Tagged Notification",
            Body = $"Tag: {tag}, Group: {group}",
            Tag = tag,
            Group = group,
            Silent = silent,
            Expiration = expirationSeconds > 0
                ? TimeSpan.FromSeconds(expirationSeconds)
                : null
        });

    public void Remove(string tag, string group)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            _manager.Remove(
                tag,
                string.IsNullOrWhiteSpace(group) ? null : group);
        }
    }

    public void ClearAll() => _manager.ClearAll();
}
