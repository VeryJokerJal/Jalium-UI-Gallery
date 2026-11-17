using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Templated gallery surface. Its default visual tree is defined in
/// Themes/Pages/OverviewPage.jalxaml.
/// </summary>
[TemplateVisualState(Name = "Pending", GroupName = "EntranceStates")]
[TemplateVisualState(Name = "Entering", GroupName = "EntranceStates")]
[TemplateVisualState(Name = "Visible", GroupName = "EntranceStates")]
[TemplateVisualState(Name = "LightTheme", GroupName = "PaletteStates")]
[TemplateVisualState(Name = "DarkTheme", GroupName = "PaletteStates")]
public sealed class OverviewPage : GalleryPage
{
    public static readonly DependencyProperty IsEntranceAnimationEnabledProperty = DependencyProperty.Register(
        nameof(IsEntranceAnimationEnabled), typeof(bool), typeof(OverviewPage), new PropertyMetadata(true, OnEntranceEnabledChanged));
    private static readonly DependencyPropertyKey HasEnteredPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasEntered), typeof(bool), typeof(OverviewPage), new PropertyMetadata(false));
    public static readonly DependencyProperty HasEnteredProperty = HasEnteredPropertyKey.DependencyProperty;

    public OverviewPage()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public bool IsEntranceAnimationEnabled { get => (bool)GetValue(IsEntranceAnimationEnabledProperty)!; set => SetValue(IsEntranceAnimationEnabledProperty, value); }
    public bool HasEntered => (bool)GetValue(HasEnteredProperty)!;
    private bool CanAnimate => IsEntranceAnimationEnabled && SystemParameters.ClientAreaAnimation && SystemParameters.UIEffects;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdatePalette();
        VisualStateManager.GoToState(this, !HasEntered && !IsLoaded && CanAnimate ? "Pending" : "Visible", false);
    }
    protected override void OnTemplateChanging()
    {
        // Leaving the animated state stops its storyboard before the named
        // targets are retired. A replacement template is immediately readable.
        if (GetTemplateChild("PART_Root") is FrameworkElement root)
            VisualStateManager.GoToElementState(root, "Visible", false);
        base.OnTemplateChanging();
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!ReferenceEquals(e.OriginalSource, this)) return;
        GalleryTheme.ModeChanged -= OnThemeChanged;
        GalleryTheme.ModeChanged += OnThemeChanged;
        SystemParameters.StaticPropertyChanged -= OnSystemParametersChanged;
        SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
        UpdatePalette();
        bool animate = !HasEntered && CanAnimate;
        SetValue(HasEnteredPropertyKey, true);
        VisualStateManager.GoToState(this, animate ? "Entering" : "Visible", false);
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!ReferenceEquals(e.OriginalSource, this)) return;
        GalleryTheme.ModeChanged -= OnThemeChanged;
        SystemParameters.StaticPropertyChanged -= OnSystemParametersChanged;
        VisualStateManager.GoToState(this, "Visible", false);
    }
    private void OnThemeChanged(object? sender, EventArgs e) => UpdatePalette();
    private void UpdatePalette() => VisualStateManager.GoToState(this,
        GalleryTheme.CurrentMode == GalleryThemeMode.Dark ? "DarkTheme" : "LightTheme", false);
    private void OnSystemParametersChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!CanAnimate) VisualStateManager.GoToState(this, "Visible", false);
    }
    private static void OnEntranceEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var page = (OverviewPage)d;
        if (!page.CanAnimate || page.HasEntered) VisualStateManager.GoToState(page, "Visible", false);
        else if (!page.IsLoaded) VisualStateManager.GoToState(page, "Pending", false);
    }
}
