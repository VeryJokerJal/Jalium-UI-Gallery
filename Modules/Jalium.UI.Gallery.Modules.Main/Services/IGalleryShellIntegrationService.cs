using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryShellIntegrationService
{
    bool TryApplySampleJumpList();

    bool TrySetTaskbarProgress(double progress);

    bool TrySetTaskbarState(TaskbarProgressMode mode);

    SystemShellSnapshot ReadSystemParameters();
}
