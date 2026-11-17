using Jalium.UI.Automation;
using Jalium.UI.Automation.Peers;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A wrapping feature label with a themeable decorative marker.</summary>
[TemplatePart(Name = "PART_Text", Type = typeof(TextBlock))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
public class GalleryFeatureItem : Control
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(GalleryFeatureItem), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register(
        nameof(MarkerBrush), typeof(Brush), typeof(GalleryFeatureItem), new PropertyMetadata(null));
    public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register(
        nameof(MarkerSize), typeof(double), typeof(GalleryFeatureItem), new PropertyMetadata(0.0),
        value => value is double size && double.IsFinite(size) && size >= 0);
    public static readonly DependencyProperty MarkerMarginProperty = DependencyProperty.Register(
        nameof(MarkerMargin), typeof(Thickness), typeof(GalleryFeatureItem), new PropertyMetadata(new Thickness(0)));

    static GalleryFeatureItem() => FocusableProperty.OverrideMetadata(
        typeof(GalleryFeatureItem), new PropertyMetadata(false));

    public GalleryFeatureItem()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryFeatureItem);
        ClearValue(FocusableProperty);
        IsEnabledChanged += (_, _) => UpdateCommonState();
    }

    public string Text { get => (string?)GetValue(TextProperty) ?? string.Empty; set => SetValue(TextProperty, value); }
    public Brush? MarkerBrush { get => (Brush?)GetValue(MarkerBrushProperty); set => SetValue(MarkerBrushProperty, value); }
    public double MarkerSize { get => (double)GetValue(MarkerSizeProperty)!; set => SetValue(MarkerSizeProperty, value); }
    public Thickness MarkerMargin { get => (Thickness)GetValue(MarkerMarginProperty)!; set => SetValue(MarkerMarginProperty, value); }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateCommonState();
    }

    private void UpdateCommonState() => VisualStateManager.GoToState(this,
        IsEnabled ? "Normal" : "Disabled", false);

    protected override AutomationPeer? OnCreateAutomationPeer() => new FeatureItemAutomationPeer(this);

    private sealed class FeatureItemAutomationPeer(GalleryFeatureItem owner) : FrameworkElementAutomationPeer(owner)
    {
        protected override string GetClassNameCore() => nameof(GalleryFeatureItem);
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;
        protected override string GetNameCore() =>
            string.IsNullOrEmpty(AutomationProperties.GetName(owner)) ? owner.Text : AutomationProperties.GetName(owner);
        protected override List<AutomationPeer> GetChildrenCore() => [];
    }
}
