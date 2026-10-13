namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Optional hook for the small amount of page state that is derived from raw colors rather
/// than the shared GalleryTheme brushes. Theme switches preserve the page instance and call
/// this hook instead of rebuilding its visual tree.
/// </summary>
public interface IGalleryThemeAware
{
    void RefreshTheme();
}
