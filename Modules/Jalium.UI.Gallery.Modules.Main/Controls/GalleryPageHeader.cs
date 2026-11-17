using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>Localized page heading with the Gallery's typography and spacing.</summary>
public class GalleryPageHeader : Control
{
    public static readonly DependencyProperty EyebrowProperty = DependencyProperty.Register(
        nameof(Eyebrow), typeof(string), typeof(GalleryPageHeader), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(GalleryPageHeader), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(GalleryPageHeader), new PropertyMetadata(string.Empty));

    public GalleryPageHeader()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryPageHeader);
    }

    public string Eyebrow { get => (string?)GetValue(EyebrowProperty) ?? string.Empty; set => SetValue(EyebrowProperty, value); }
    public string Title { get => (string?)GetValue(TitleProperty) ?? string.Empty; set => SetValue(TitleProperty, value); }
    public string Description { get => (string?)GetValue(DescriptionProperty) ?? string.Empty; set => SetValue(DescriptionProperty, value); }
}
