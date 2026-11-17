using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryThemeResourceService
{
    string CurrentThemeKey { get; }

    IReadOnlyList<ThemeResourceEntry> GetResources(
        string previewThemeKey);
}
