using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A command button whose destination selection is independent of keyboard focus.</summary>
[TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
[TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
public class GalleryNavigationButton : Button
{
    private bool _hasTemplate;
    public static readonly DependencyProperty DestinationProperty = DependencyProperty.Register(
        nameof(Destination), typeof(string), typeof(GalleryNavigationButton), new PropertyMetadata(string.Empty, OnDestinationChanged));
    public static readonly DependencyProperty SelectedDestinationProperty = DependencyProperty.Register(
        nameof(SelectedDestination), typeof(string), typeof(GalleryNavigationButton), new PropertyMetadata(string.Empty, OnDestinationChanged));
    private static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsSelected), typeof(bool), typeof(GalleryNavigationButton), new PropertyMetadata(false));
    public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

    public GalleryNavigationButton()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryNavigationButton);
    }

    public string Destination { get => (string)GetValue(DestinationProperty)!; set => SetValue(DestinationProperty, value); }
    public string SelectedDestination { get => (string)GetValue(SelectedDestinationProperty)!; set => SetValue(SelectedDestinationProperty, value); }
    public bool IsSelected => (bool)GetValue(IsSelectedProperty)!;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _hasTemplate = true;
        UpdateSelection();
    }

    protected override void OnTemplateChanging()
    {
        _hasTemplate = false;
        base.OnTemplateChanging();
    }

    private void UpdateSelection()
    {
        SetValue(IsSelectedPropertyKey, !string.IsNullOrEmpty(Destination) &&
            string.Equals(Destination, SelectedDestination, StringComparison.OrdinalIgnoreCase));
        if (_hasTemplate)
            VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Unselected", false);
    }

    private static void OnDestinationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GalleryNavigationButton)d).UpdateSelection();
}
