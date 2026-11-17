using Jalium.UI.Controls.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>Default theme layer for the reusable Gallery controls.</summary>
public sealed partial class GalleryControlThemes : ResourceDictionary
{
    private static readonly Lazy<GalleryControlThemes> Default = new(() =>
    {
        var themes = new GalleryControlThemes();
        ThemeManager.RegisterControlThemes(themes);
        return themes;
    });

    public GalleryControlThemes() => InitializeComponent();

    public static void EnsureRegistered() => _ = Default.Value;

    public static DataTemplate? FindPreviewTemplate(string key) => FindDataTemplate(key);

    public static Style? FindStyle(string key)
    {
        EnsureRegistered();
        return Default.Value.TryGetValue(key, out var value) ? value as Style : null;
    }

    public static DataTemplate? FindDataTemplate(string key)
    {
        EnsureRegistered();
        return Default.Value.TryGetValue(key, out var value) ? value as DataTemplate : null;
    }
}
