using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>
/// Navigation-compatible templated Gallery surface. ViewModels own application
/// state; the control owns its template lifecycle and visual states.
/// </summary>
[TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Wide", GroupName = "LayoutStates")]
[TemplateVisualState(Name = "Compact", GroupName = "LayoutStates")]
public abstract class GalleryPage : Page
{
    static GalleryPage() => Styling.Css.IncludeTemplatePartsProperty.OverrideMetadata(
        typeof(GalleryPage), new PropertyMetadata(true));
    public static readonly DependencyProperty CompactWidthProperty = DependencyProperty.Register(
        nameof(CompactWidth), typeof(double), typeof(GalleryPage),
        new PropertyMetadata(720d, OnStatePropertyChanged),
        value => value is double width && double.IsFinite(width) && width >= 0);

    protected GalleryPage()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = GetType();
        UpdateDefaultStyle();
        ApplyTemplate();
        SizeChanged += (_, _) => UpdateVisualStates(false);
    }

    public double CompactWidth
    {
        get => (double)GetValue(CompactWidthProperty)!;
        set => SetValue(CompactWidthProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualStates(false);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsEnabledProperty || e.Property == IsKeyboardFocusWithinProperty)
            UpdateVisualStates(false);
    }

    protected virtual void UpdateVisualStates(bool useTransitions)
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", useTransitions);
        VisualStateManager.GoToState(this, IsKeyboardFocusWithin ? "Focused" : "Unfocused", useTransitions);
        VisualStateManager.GoToState(this, ActualWidth > 0 && ActualWidth < CompactWidth ? "Compact" : "Wide", useTransitions);
    }

    private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GalleryPage)d).UpdateVisualStates(false);
}
