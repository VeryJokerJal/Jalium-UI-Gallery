using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Shared design-system resource dictionary. The reusable page-scaffold styles
/// (cards, demo surfaces, typography ramp, chips, dividers) live in the
/// companion <c>GalleryStyles.jalxaml</c>; this partial just triggers XAML
/// parsing at construction. It is merged application-wide from
/// <c>App.jalxaml</c> so every sample page can reference the styles by key.
/// </summary>
public partial class GalleryStyles : ResourceDictionary
{
    public GalleryStyles()
    {
        InitializeComponent();
    }
}
