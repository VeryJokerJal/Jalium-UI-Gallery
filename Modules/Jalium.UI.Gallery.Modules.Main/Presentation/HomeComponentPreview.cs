using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Hosting;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// A reusable live-preview presenter. Independent DataTemplates own its visuals;
/// changing a selection never extracts or reparents another control's template parts.
/// </summary>
[TemplatePart(Name = "PART_PreviewPresenter", Type = typeof(ContentPresenter))]
[TemplateVisualState(Name = "Empty", GroupName = "PreviewStates")]
[TemplateVisualState(Name = "Ready", GroupName = "PreviewStates")]
public class HomeComponentPreview : ContentControl
{
    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
        nameof(Item), typeof(HomeCatalogItem), typeof(HomeComponentPreview), new PropertyMetadata(null, OnPreviewChanged));
    public static readonly DependencyProperty IsDetailedProperty = DependencyProperty.Register(
        nameof(IsDetailed), typeof(bool), typeof(HomeComponentPreview), new PropertyMetadata(true, OnPreviewChanged));
    public static readonly DependencyProperty PreviewTemplateProperty = DependencyProperty.Register(
        nameof(PreviewTemplate), typeof(DataTemplate), typeof(HomeComponentPreview), new PropertyMetadata(null, OnPreviewChanged));
    private HomeCatalogItem? _dataItem;
    private object? _previewData;

    public HomeComponentPreview()
    {
        UseTemplateContentManagement();
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(HomeComponentPreview);
    }

    public HomeCatalogItem? Item { get => (HomeCatalogItem?)GetValue(ItemProperty); set => SetValue(ItemProperty, value); }
    public bool IsDetailed { get => (bool)GetValue(IsDetailedProperty)!; set => SetValue(IsDetailedProperty, value); }
    public DataTemplate? PreviewTemplate { get => (DataTemplate?)GetValue(PreviewTemplateProperty); set => SetValue(PreviewTemplateProperty, value); }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateState();
    }

    private static void OnPreviewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((HomeComponentPreview)d).Rebuild();
    private void Rebuild()
    {
        if (Item is not { } item)
        {
            _dataItem = null;
            _previewData = null;
            SetCurrentValue(ContentProperty, null);
            UpdateState();
            return;
        }

        var featured = GalleryControlThemes.FindPreviewTemplate("Gallery.PreviewKind." + item.PreviewKind);
        var template = PreviewTemplate ?? featured ?? GalleryControlThemes.FindPreviewTemplate("Gallery.Preview." + item.PageType.Name)
            ?? throw new InvalidOperationException($"No preview template is registered for {item.PageType.FullName}.");
        if (featured != null && PreviewTemplate == null)
            _previewData = new HomePreviewOptions(IsDetailed);
        else if (!ReferenceEquals(_dataItem, item) || _previewData is HomePreviewOptions)
            _previewData = ResolveData(item);
        _dataItem = item;
        SetCurrentValue(ContentTemplateProperty, template);
        SetCurrentValue(ContentProperty, _previewData ?? item);
        UpdateState();
    }

    private static object ResolveData(HomeCatalogItem item)
    {
        var services = Application.Current?.Services;
        if (services?.GetService(typeof(ViewRegistry)) is ViewRegistry registry &&
            registry.TryGetViewModelType(item.PageType, out var viewModelType) && viewModelType != null)
            return services.GetService(viewModelType)
                ?? throw new InvalidOperationException($"The preview ViewModel {viewModelType.FullName} is not registered.");
        // Detached previews can still instantiate the declared live controls from
        // immutable catalog data, without creating an application or service scope.
        return item;
    }

    private void UpdateState() => VisualStateManager.GoToState(this, Item == null ? "Empty" : "Ready", false);
}
