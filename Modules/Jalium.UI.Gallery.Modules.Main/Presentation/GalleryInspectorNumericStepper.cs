using Jalium.UI.Automation;
using Jalium.UI.Automation.Peers;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>A templated range editor with repeat buttons, keyboard input and range automation.</summary>
[TemplatePart(Name = "PART_DecreaseButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_IncreaseButton", Type = typeof(ButtonBase))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
public class GalleryInspectorNumericStepper : RangeBase
{
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(GalleryInspectorNumericStepper), new PropertyMetadata(string.Empty, OnDisplayChanged));
    public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
        nameof(Suffix), typeof(string), typeof(GalleryInspectorNumericStepper), new PropertyMetadata(string.Empty, OnDisplayChanged));
    public static readonly DependencyProperty StepProperty = SmallChangeProperty.AddOwner(
        typeof(GalleryInspectorNumericStepper), new PropertyMetadata(1d, null,
            (_, value) => value is double step && double.IsFinite(step) && step > 0 ? step : 1d));
    private static readonly DependencyPropertyKey DisplayValuePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(DisplayValue), typeof(string), typeof(GalleryInspectorNumericStepper), new PropertyMetadata("0"));
    public static readonly DependencyProperty DisplayValueProperty = DisplayValuePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey CanDecreasePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(CanDecrease), typeof(bool), typeof(GalleryInspectorNumericStepper), new PropertyMetadata(false));
    public static readonly DependencyProperty CanDecreaseProperty = CanDecreasePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey CanIncreasePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(CanIncrease), typeof(bool), typeof(GalleryInspectorNumericStepper), new PropertyMetadata(true));
    public static readonly DependencyProperty CanIncreaseProperty = CanIncreasePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey DecreaseLabelPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(DecreaseLabel), typeof(string), typeof(GalleryInspectorNumericStepper), new PropertyMetadata("Decrease"));
    public static readonly DependencyProperty DecreaseLabelProperty = DecreaseLabelPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey IncreaseLabelPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IncreaseLabel), typeof(string), typeof(GalleryInspectorNumericStepper), new PropertyMetadata("Increase"));
    public static readonly DependencyProperty IncreaseLabelProperty = IncreaseLabelPropertyKey.DependencyProperty;
    private ButtonBase? _decreaseButton, _increaseButton;

    static GalleryInspectorNumericStepper()
    {
        MaximumProperty.OverrideMetadata(typeof(GalleryInspectorNumericStepper), new PropertyMetadata(100d));
        ValueProperty.OverrideMetadata(typeof(GalleryInspectorNumericStepper),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }

    public GalleryInspectorNumericStepper()
    {
        GalleryControlThemes.EnsureRegistered();
        DefaultStyleKey = typeof(GalleryInspectorNumericStepper);
        AddHandler(MouseDownEvent, new MouseButtonEventHandler((_, e) =>
        {
            if (IsEnabled && e.ChangedButton == MouseButton.Left) Focus();
        }), handledEventsToo: true);
        UpdateDisplay();
    }

    public string Label { get => (string?)GetValue(LabelProperty) ?? string.Empty; set => SetValue(LabelProperty, value); }
    public string Suffix { get => (string?)GetValue(SuffixProperty) ?? string.Empty; set => SetValue(SuffixProperty, value); }
    public double Step { get => (double)GetValue(StepProperty)!; set => SetValue(StepProperty, value); }
    public string DisplayValue => (string)GetValue(DisplayValueProperty)!;
    public bool CanDecrease => (bool)GetValue(CanDecreaseProperty)!;
    public bool CanIncrease => (bool)GetValue(CanIncreaseProperty)!;
    public string DecreaseLabel => (string)GetValue(DecreaseLabelProperty)!;
    public string IncreaseLabel => (string)GetValue(IncreaseLabelProperty)!;

    // Preserve the inspector's existing convenient notification API while also
    // raising RangeBase.ValueChanged for routed-event and automation consumers.
    public new event Action<double>? ValueChanged;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _decreaseButton = GetTemplateChild("PART_DecreaseButton") as ButtonBase;
        _increaseButton = GetTemplateChild("PART_IncreaseButton") as ButtonBase;
        if (_decreaseButton != null) _decreaseButton.Click += OnDecrease;
        if (_increaseButton != null) _increaseButton.Click += OnIncrease;
        UpdateDisplay();
    }

    protected override void OnTemplateChanging()
    {
        if (_decreaseButton != null) _decreaseButton.Click -= OnDecrease;
        if (_increaseButton != null) _increaseButton.Click -= OnIncrease;
        _decreaseButton = _increaseButton = null;
        base.OnTemplateChanging();
    }

    protected override void OnValueChanged(double oldValue, double newValue)
    {
        base.OnValueChanged(oldValue, newValue);
        UpdateDisplay();
        ValueChanged?.Invoke(newValue);
    }
    protected override void OnMinimumChanged(double oldValue, double newValue) { base.OnMinimumChanged(oldValue, newValue); UpdateDisplay(); }
    protected override void OnMaximumChanged(double oldValue, double newValue) { base.OnMaximumChanged(oldValue, newValue); UpdateDisplay(); }
    protected override void OnIsEnabledChanged(bool oldValue, bool newValue) { base.OnIsEnabledChanged(oldValue, newValue); UpdateDisplay(); }
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);
        VisualStateManager.GoToState(this, "Focused", false);
    }
    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnLostKeyboardFocus(e);
        VisualStateManager.GoToState(this, "Unfocused", false);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled || !IsEnabled) return;
        switch (e.Key)
        {
            case Key.Up: case Key.Right: ChangeValue(Step); break;
            case Key.Down: case Key.Left: ChangeValue(-Step); break;
            case Key.PageUp: ChangeValue(LargeChange); break;
            case Key.PageDown: ChangeValue(-LargeChange); break;
            case Key.Home: SetCurrentValue(ValueProperty, Minimum); break;
            case Key.End: SetCurrentValue(ValueProperty, Maximum); break;
            default: return;
        }
        e.Handled = true;
    }

    protected override AutomationPeer? OnCreateAutomationPeer() => new StepperAutomationPeer(this);
    private void OnDecrease(object sender, RoutedEventArgs e) => ChangeValue(-Step);
    private void OnIncrease(object sender, RoutedEventArgs e) => ChangeValue(Step);
    private void ChangeValue(double delta)
    {
        if (IsEnabled) SetCurrentValue(ValueProperty, Math.Clamp(Value + delta, Minimum, Maximum));
    }
    private static void OnDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((GalleryInspectorNumericStepper)d).UpdateDisplay();
    private void UpdateDisplay()
    {
        SetValue(DisplayValuePropertyKey, $"{Value:0}{Suffix}");
        SetValue(CanDecreasePropertyKey, IsEnabled && Value > Minimum);
        SetValue(CanIncreasePropertyKey, IsEnabled && Value < Maximum);
        SetValue(DecreaseLabelPropertyKey, $"Decrease {Label}".TrimEnd());
        SetValue(IncreaseLabelPropertyKey, $"Increase {Label}".TrimEnd());
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);
        VisualStateManager.GoToState(this, IsKeyboardFocusWithin ? "Focused" : "Unfocused", false);
    }

    private sealed class StepperAutomationPeer(GalleryInspectorNumericStepper owner) : RangeBaseAutomationPeer(owner)
    {
        protected override string GetClassNameCore() => nameof(GalleryInspectorNumericStepper);
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Spinner;
        protected override string GetNameCore() => string.IsNullOrEmpty(base.GetNameCore()) ? owner.Label : base.GetNameCore();
    }
}
