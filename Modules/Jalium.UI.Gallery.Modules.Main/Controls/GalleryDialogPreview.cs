using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>Replaceable content for an expanded review dialog.</summary>
public class GalleryDialogPreview : Control
{
    static GalleryDialogPreview() => DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryDialogPreview), new PropertyMetadata(typeof(GalleryDialogPreview)));
    public GalleryDialogPreview() => GalleryControlThemes.EnsureRegistered();
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(GalleryDialogPreview), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PreviewTextProperty = DependencyProperty.Register(
        nameof(PreviewText), typeof(string), typeof(GalleryDialogPreview), new PropertyMetadata(string.Empty));
    public string Description { get => (string?)GetValue(DescriptionProperty) ?? string.Empty; set => SetValue(DescriptionProperty, value); }
    public string PreviewText { get => (string?)GetValue(PreviewTextProperty) ?? string.Empty; set => SetValue(PreviewTextProperty, value); }
}
