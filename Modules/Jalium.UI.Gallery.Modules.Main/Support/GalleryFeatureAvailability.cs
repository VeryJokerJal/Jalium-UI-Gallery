namespace Jalium.UI.Gallery.Modules.Main.Support;

internal static class GalleryFeatureAvailability
{
    internal static bool TryGetUnavailableReason(
        string pageTag,
        out string feature,
        out string reason,
        out string guidance)
    {
        feature = string.Empty;
        reason = string.Empty;
        guidance = string.Empty;

        if (OperatingSystem.IsWindows())
            return false;

        switch (pageTag)
        {
            case "webview":
                feature = "WebView";
                reason = "The current WebView control uses Microsoft WebView2, whose native host is Windows-only.";
                guidance = "The rest of the Gallery remains available. A future Linux browser host can use WebKitGTK without changing this navigation contract.";
                return true;

            case "notifyicon":
                feature = "NotifyIcon";
                reason = "A StatusNotifierItem/AppIndicator tray backend is not connected on this Linux desktop.";
                guidance = "Use SystemNotification for desktop notifications; the page is disabled instead of silently pretending that a tray icon was created.";
                return true;

            case "shellintegration":
                feature = "Windows shell integration";
                reason = "Jump lists and taskbar progress are Windows shell contracts and have no portable Linux equivalent.";
                guidance = "Open URI, file dialogs, and other desktop-neutral operations use xdg-desktop-portal elsewhere in the Gallery.";
                return true;

            case "printing" when OperatingSystem.IsLinux():
                feature = "Printing";
                reason = "This synchronous sample cannot guarantee that the current desktop session exposes a usable xdg Print portal.";
                guidance = "Printing is capability-gated so selecting this page cannot invoke a Windows print API or block a GUI process on console input.";
                return true;

            default:
                return false;
        }
    }
}
