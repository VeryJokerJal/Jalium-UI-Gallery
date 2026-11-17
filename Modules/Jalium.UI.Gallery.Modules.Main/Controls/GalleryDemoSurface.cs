using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>Reusable demo canvas whose appearance can be replaced through its template.</summary>
[TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
public class GalleryDemoSurface : ContentControl
{
    public GalleryDemoSurface()
    {
        UseTemplateContentManagement();
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryDemoSurface);
    }
}
