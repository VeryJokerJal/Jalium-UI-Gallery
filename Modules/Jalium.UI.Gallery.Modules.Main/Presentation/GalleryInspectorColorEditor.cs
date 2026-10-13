using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// Original workbench palette and RGB editor expressed as a bindable custom
/// View control. It contains only local editing visuals; the selected Color is
/// persisted by the owning ViewModel.
/// </summary>
public sealed class GalleryInspectorColorEditor : ContentControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(GalleryInspectorColorEditor),
            new PropertyMetadata(string.Empty, OnLabelChanged));

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(GalleryInspectorColorEditor),
            new PropertyMetadata(
                Jalium.UI.Media.Color.FromRgb(0, 0, 0),
                OnColorChanged));

    public static readonly DependencyProperty PaletteKindProperty =
        DependencyProperty.Register(
            nameof(PaletteKind),
            typeof(string),
            typeof(GalleryInspectorColorEditor),
            new PropertyMetadata("Accent", OnPaletteKindChanged));

    private readonly TextBlock _labelText;
    private readonly TextBlock _hexValue;
    private readonly Border _colorPreview;
    private readonly StackPanel _swatches;
    private readonly GalleryInspectorNumericStepper _red;
    private readonly GalleryInspectorNumericStepper _green;
    private readonly GalleryInspectorNumericStepper _blue;
    private bool _updating;
    private bool _initialized;

    public GalleryInspectorColorEditor()
    {
        var root = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 16)
        };

        var heading = new Grid
        {
            Margin = new Thickness(0, 0, 0, 8)
        };
        heading.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        heading.ColumnDefinitions.Add(
            new ColumnDefinition { Width = GridLength.Auto });

        _labelText = new TextBlock
        {
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextSecondaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        heading.Children.Add(_labelText);

        var valuePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        _colorPreview = new Border
        {
            Width = 16,
            Height = 16,
            CornerRadius = new CornerRadius(3),
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 6, 0)
        };
        valuePanel.Children.Add(_colorPreview);
        _hexValue = new TextBlock
        {
            FontSize = 10,
            Foreground = GalleryTheme.TextMutedBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        valuePanel.Children.Add(_hexValue);
        Grid.SetColumn(valuePanel, 1);
        heading.Children.Add(valuePanel);
        root.Children.Add(heading);

        _swatches = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 10)
        };
        root.Children.Add(_swatches);

        _red = CreateChannel("R");
        _green = CreateChannel("G");
        _blue = CreateChannel("B");
        _red.ValueChanged += _ => ApplyChannels();
        _green.ValueChanged += _ => ApplyChannels();
        _blue.ValueChanged += _ => ApplyChannels();
        root.Children.Add(_red);
        root.Children.Add(_green);
        root.Children.Add(_blue);

        Content = root;
        _initialized = true;
        RebuildSwatches();
        UpdateColor(Color);
        UpdateLabel();
    }

    public string Label
    {
        get => GetValue(LabelProperty) as string ?? string.Empty;
        set => SetValue(LabelProperty, value);
    }

    public Color Color
    {
        get => (Color)(GetValue(ColorProperty) ??
            Jalium.UI.Media.Color.FromRgb(0, 0, 0));
        set => SetValue(ColorProperty, value);
    }

    public string PaletteKind
    {
        get => GetValue(PaletteKindProperty) as string ?? "Accent";
        set => SetValue(PaletteKindProperty, value);
    }

    private static GalleryInspectorNumericStepper CreateChannel(
        string label) =>
        new()
        {
            Label = label,
            Minimum = 0,
            Maximum = 255,
            Step = 5
        };

    private static void OnLabelChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is GalleryInspectorColorEditor editor)
        {
            editor.UpdateLabel();
        }
    }

    private static void OnColorChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is GalleryInspectorColorEditor editor &&
            args.NewValue is Color color)
        {
            editor.UpdateColor(color);
        }
    }

    private static void OnPaletteKindChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is GalleryInspectorColorEditor editor)
        {
            editor.RebuildSwatches();
        }
    }

    private void UpdateLabel()
    {
        if (_initialized)
        {
            _labelText.Text = Label;
        }
    }

    private void UpdateColor(Color color)
    {
        if (!_initialized)
        {
            return;
        }

        _updating = true;
        _red.SetCurrentValue(
            GalleryInspectorNumericStepper.ValueProperty,
            (double)color.R);
        _green.SetCurrentValue(
            GalleryInspectorNumericStepper.ValueProperty,
            (double)color.G);
        _blue.SetCurrentValue(
            GalleryInspectorNumericStepper.ValueProperty,
            (double)color.B);
        _updating = false;

        _hexValue.Text = GalleryThemeTuner.ToHex(color);
        _colorPreview.Background = new SolidColorBrush(color);
    }

    private void ApplyChannels()
    {
        if (_updating)
        {
            return;
        }

        var color = Jalium.UI.Media.Color.FromRgb(
            (byte)Math.Round(_red.Value),
            (byte)Math.Round(_green.Value),
            (byte)Math.Round(_blue.Value));
        SetCurrentValue(ColorProperty, color);
    }

    private void RebuildSwatches()
    {
        if (!_initialized)
        {
            return;
        }

        _swatches.Children.Clear();
        var palette = string.Equals(
                PaletteKind,
                "Surface",
                StringComparison.OrdinalIgnoreCase)
            ? GalleryThemeTuner.SurfacePalette
            : GalleryThemeTuner.AccentPalette;

        foreach (var color in palette)
        {
            var swatch = new Button
            {
                Width = 28,
                Height = 28,
                MinWidth = 28,
                MinHeight = 28,
                Margin = new Thickness(0, 0, 7, 0),
                Background = new SolidColorBrush(color),
                BorderBrush = GalleryTheme.BorderDefaultBrush,
                BorderThickness = new Thickness(1),
                ToolTip = GalleryThemeTuner.ToHex(color)
            };
            swatch.Click += (_, _) =>
                SetCurrentValue(ColorProperty, color);
            _swatches.Children.Add(swatch);
        }
    }
}
