using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryWindowBackdropService
{
    bool TrySetBackdrop(WindowBackdropMode mode);
}
