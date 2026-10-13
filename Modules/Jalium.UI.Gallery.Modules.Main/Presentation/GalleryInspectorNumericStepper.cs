using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// View-only numeric editor matching the original workbench inspector.
/// Its Value dependency property is intended for TwoWay ViewModel binding.
/// </summary>
public sealed class GalleryInspectorNumericStepper : ContentControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(GalleryInspectorNumericStepper),
            new PropertyMetadata(string.Empty, OnDisplayPropertyChanged));

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(GalleryInspectorNumericStepper),
            new PropertyMetadata(0d));

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(GalleryInspectorNumericStepper),
            new PropertyMetadata(100d));

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(
            nameof(Step),
            typeof(double),
            typeof(GalleryInspectorNumericStepper),
            new PropertyMetadata(1d));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(GalleryInspectorNumericStepper),
            new PropertyMetadata(0d, OnValueChanged));

    public static readonly DependencyProperty SuffixProperty =
        DependencyProperty.Register(
            nameof(Suffix),
            typeof(string),
            typeof(GalleryInspectorNumericStepper),
            new PropertyMetadata(string.Empty, OnDisplayPropertyChanged));

    private readonly TextBlock _labelText;
    private readonly TextBlock _valueText;
    private readonly Button _decreaseButton;
    private readonly Button _increaseButton;
    private bool _initialized;

    public GalleryInspectorNumericStepper()
    {
        var root = new Grid
        {
            Margin = new Thickness(0, 0, 0, 10)
        };
        root.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        root.ColumnDefinitions.Add(
            new ColumnDefinition { Width = GridLength.Auto });

        _labelText = new TextBlock
        {
            FontSize = 11,
            Foreground = GalleryTheme.TextSecondaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        root.Children.Add(_labelText);

        var controls = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        _decreaseButton = CreateButton("-");
        _decreaseButton.Click += (_, _) => ChangeValue(-Step);
        controls.Children.Add(_decreaseButton);

        _valueText = new TextBlock
        {
            Width = 54,
            FontSize = 10,
            Foreground = GalleryTheme.TextMutedBrush,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        controls.Children.Add(_valueText);

        _increaseButton = CreateButton("+");
        _increaseButton.Click += (_, _) => ChangeValue(Step);
        controls.Children.Add(_increaseButton);

        Grid.SetColumn(controls, 1);
        root.Children.Add(controls);
        Content = root;
        _initialized = true;
        UpdateDisplay();
    }

    public event Action<double>? ValueChanged;

    public string Label
    {
        get => GetValue(LabelProperty) as string ?? string.Empty;
        set => SetValue(LabelProperty, value);
    }

    public double Minimum
    {
        get => (double)(GetValue(MinimumProperty) ?? 0d);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)(GetValue(MaximumProperty) ?? 100d);
        set => SetValue(MaximumProperty, value);
    }

    public double Step
    {
        get => (double)(GetValue(StepProperty) ?? 1d);
        set => SetValue(StepProperty, value);
    }

    public double Value
    {
        get => (double)(GetValue(ValueProperty) ?? 0d);
        set => SetValue(ValueProperty, value);
    }

    public string Suffix
    {
        get => GetValue(SuffixProperty) as string ?? string.Empty;
        set => SetValue(SuffixProperty, value);
    }

    private static Button CreateButton(string content) =>
        new()
        {
            Content = content,
            Width = 28,
            Height = 28,
            MinWidth = 28,
            MinHeight = 28,
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            Foreground = GalleryTheme.TextSecondaryBrush
        };

    private void ChangeValue(double delta)
    {
        var next = Math.Clamp(Value + delta, Minimum, Maximum);
        SetCurrentValue(ValueProperty, next);
    }

    private static void OnDisplayPropertyChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is GalleryInspectorNumericStepper stepper)
        {
            stepper.UpdateDisplay();
        }
    }

    private static void OnValueChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not GalleryInspectorNumericStepper stepper)
        {
            return;
        }

        stepper.UpdateDisplay();
        if (stepper._initialized &&
            args.NewValue is double value)
        {
            stepper.ValueChanged?.Invoke(value);
        }
    }

    private void UpdateDisplay()
    {
        if (!_initialized)
        {
            return;
        }

        _labelText.Text = Label;
        _valueText.Text = $"{Value:0}{Suffix}";
        _decreaseButton.ToolTip = $"Decrease {Label}";
        _increaseButton.ToolTip = $"Increase {Label}";
    }
}
