using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A reusable dialog field with templated busy and validation states.</summary>
[TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Ready", GroupName = "BusyStates")]
[TemplateVisualState(Name = "Busy", GroupName = "BusyStates")]
[TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
[TemplateVisualState(Name = "Invalid", GroupName = "ValidationStates")]
public class GalleryDialogForm : Control
{
    static GalleryDialogForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryDialogForm), new PropertyMetadata(typeof(GalleryDialogForm)));
    public GalleryDialogForm() => GalleryControlThemes.EnsureRegistered();

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(GalleryDialogForm), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(GalleryDialogForm), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(GalleryDialogForm), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        nameof(PlaceholderText), typeof(string), typeof(GalleryDialogForm), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
        nameof(ErrorMessage), typeof(string), typeof(GalleryDialogForm), new PropertyMetadata(string.Empty, OnStateChanged));
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy), typeof(bool), typeof(GalleryDialogForm), new PropertyMetadata(false, OnStateChanged));
    private static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasError), typeof(bool), typeof(GalleryDialogForm), new PropertyMetadata(false));
    public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

    public string Description { get => (string?)GetValue(DescriptionProperty) ?? string.Empty; set => SetValue(DescriptionProperty, value); }
    public string Label { get => (string?)GetValue(LabelProperty) ?? string.Empty; set => SetValue(LabelProperty, value); }
    public string Text { get => (string?)GetValue(TextProperty) ?? string.Empty; set => SetValue(TextProperty, value); }
    public string PlaceholderText { get => (string?)GetValue(PlaceholderTextProperty) ?? string.Empty; set => SetValue(PlaceholderTextProperty, value); }
    public string ErrorMessage { get => (string?)GetValue(ErrorMessageProperty) ?? string.Empty; set => SetValue(ErrorMessageProperty, value); }
    public bool IsBusy { get => (bool)GetValue(IsBusyProperty)!; set => SetValue(IsBusyProperty, value); }
    public bool HasError => (bool)GetValue(HasErrorProperty)!;

    public bool FocusInput(bool selectAll = false)
    {
        if (GetTemplateChild("PART_TextBox") is not TextBox input || !input.Focus()) return false;
        if (selectAll) input.SelectAll();
        return true;
    }
    public override void OnApplyTemplate() { base.OnApplyTemplate(); UpdateStates(); }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsEnabledProperty) UpdateStates();
    }
    private static void OnStateChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) => ((GalleryDialogForm)target).UpdateStates();
    private void UpdateStates()
    {
        SetValue(HasErrorPropertyKey, !string.IsNullOrWhiteSpace(ErrorMessage));
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);
        VisualStateManager.GoToState(this, IsBusy ? "Busy" : "Ready", false);
        VisualStateManager.GoToState(this, HasError ? "Invalid" : "Valid", false);
    }
}
