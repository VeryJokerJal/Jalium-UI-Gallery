using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A reusable, themeable section with a header, description and arbitrary demo content.</summary>
[TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "PART_Description", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
[TemplateVisualState(Name = "WithDescription", GroupName = "DescriptionStates")]
[TemplateVisualState(Name = "WithoutDescription", GroupName = "DescriptionStates")]
public class GallerySection : HeaderedContentControl
{
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(GallerySection),
        new PropertyMetadata(string.Empty, OnDescriptionChanged));

    public GallerySection()
    {
        UseTemplateContentManagement();
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GallerySection);
    }

    public string Description
    {
        get => (string?)GetValue(DescriptionProperty) ?? string.Empty;
        set => SetValue(DescriptionProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateDescriptionState();
    }

    private void UpdateDescriptionState() => VisualStateManager.GoToState(this,
        string.IsNullOrWhiteSpace(Description) ? "WithoutDescription" : "WithDescription", false);

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GallerySection)d).UpdateDescriptionState();
}
