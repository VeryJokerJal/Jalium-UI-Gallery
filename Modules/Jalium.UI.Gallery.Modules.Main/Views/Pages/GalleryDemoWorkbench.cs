using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Markup;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>A templated workbench whose live content stays in the View layer.</summary>
[ContentProperty(nameof(DemoContent))]
[TemplatePart(Name = "PART_DemoContentPresenter", Type = typeof(ContentPresenter))]
public class GalleryDemoWorkbench : GalleryPage
{
    public static readonly DependencyProperty InspectorWidthProperty = DependencyProperty.Register(
        nameof(InspectorWidth), typeof(double), typeof(GalleryDemoWorkbench), new PropertyMetadata(300d),
        value => value is double width && double.IsFinite(width) && width >= 0);
    public double InspectorWidth { get => (double)GetValue(InspectorWidthProperty)!; set => SetValue(InspectorWidthProperty, value); }

    public static readonly DependencyProperty DemoContentProperty = DependencyProperty.Register(
        nameof(DemoContent), typeof(object), typeof(GalleryDemoWorkbench), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty DemoContentTemplateProperty = DependencyProperty.Register(
        nameof(DemoContentTemplate), typeof(DataTemplate), typeof(GalleryDemoWorkbench), new PropertyMetadata(null));
    public static readonly DependencyProperty DemoContentTemplateSelectorProperty = DependencyProperty.Register(
        nameof(DemoContentTemplateSelector), typeof(DataTemplateSelector), typeof(GalleryDemoWorkbench), new PropertyMetadata(null));
    public object? DemoContent { get => GetValue(DemoContentProperty); set => SetValue(DemoContentProperty, value); }
    public DataTemplate? DemoContentTemplate { get => (DataTemplate?)GetValue(DemoContentTemplateProperty); set => SetValue(DemoContentTemplateProperty, value); }
    public DataTemplateSelector? DemoContentTemplateSelector { get => (DataTemplateSelector?)GetValue(DemoContentTemplateSelectorProperty); set => SetValue(DemoContentTemplateSelectorProperty, value); }
}
