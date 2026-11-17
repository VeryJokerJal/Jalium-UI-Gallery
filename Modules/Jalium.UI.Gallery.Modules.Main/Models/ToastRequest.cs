using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed record ToastRequest(
    long Sequence,
    ToastSeverity Severity,
    string Title,
    string Message,
    TimeSpan Duration,
    bool IsClosable,
    bool IsAutoDismissEnabled,
    bool HasAction = false,
    bool DismissAll = false);
