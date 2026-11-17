using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>A templated, bindable color editor. Palette and RGB changes preserve the owning binding.</summary>
[TemplatePart(Name = "PART_Palette", Type = typeof(ListBox))]
public class GalleryInspectorColorEditor : Control
{
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(GalleryInspectorColorEditor), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color), typeof(Color), typeof(GalleryInspectorColorEditor),
        new FrameworkPropertyMetadata(Jalium.UI.Media.Color.FromRgb(0, 0, 0),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorChanged));
    public static readonly DependencyProperty PaletteKindProperty = DependencyProperty.Register(
        nameof(PaletteKind), typeof(string), typeof(GalleryInspectorColorEditor), new PropertyMetadata("Accent", OnPaletteKindChanged));
    public static readonly DependencyProperty RedProperty = ChannelProperty(nameof(Red));
    public static readonly DependencyProperty GreenProperty = ChannelProperty(nameof(Green));
    public static readonly DependencyProperty BlueProperty = ChannelProperty(nameof(Blue));
    public static readonly DependencyProperty SelectedSwatchProperty = DependencyProperty.Register(
        nameof(SelectedSwatch), typeof(GalleryColorSwatch), typeof(GalleryInspectorColorEditor),
        new PropertyMetadata(null, OnSelectedSwatchChanged));
    private static readonly DependencyPropertyKey HexValuePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HexValue), typeof(string), typeof(GalleryInspectorColorEditor), new PropertyMetadata("#000000"));
    public static readonly DependencyProperty HexValueProperty = HexValuePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey ColorBrushPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ColorBrush), typeof(Brush), typeof(GalleryInspectorColorEditor), new PropertyMetadata(null));
    public static readonly DependencyProperty ColorBrushProperty = ColorBrushPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey SwatchesPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Swatches), typeof(IReadOnlyList<GalleryColorSwatch>), typeof(GalleryInspectorColorEditor), new PropertyMetadata(null));
    public static readonly DependencyProperty SwatchesProperty = SwatchesPropertyKey.DependencyProperty;
    private bool _updating;
    private ListBox? _palette;

    public GalleryInspectorColorEditor()
    {
        GalleryControlThemes.EnsureRegistered();
        RebuildPalette();
        DefaultStyleKey = typeof(GalleryInspectorColorEditor);
    }

    public string Label { get => (string?)GetValue(LabelProperty) ?? string.Empty; set => SetValue(LabelProperty, value); }
    public Color Color { get => (Color)GetValue(ColorProperty)!; set => SetValue(ColorProperty, value); }
    public string PaletteKind { get => (string?)GetValue(PaletteKindProperty) ?? "Accent"; set => SetValue(PaletteKindProperty, value); }
    public double Red { get => (double)GetValue(RedProperty)!; set => SetValue(RedProperty, value); }
    public double Green { get => (double)GetValue(GreenProperty)!; set => SetValue(GreenProperty, value); }
    public double Blue { get => (double)GetValue(BlueProperty)!; set => SetValue(BlueProperty, value); }
    public GalleryColorSwatch? SelectedSwatch { get => (GalleryColorSwatch?)GetValue(SelectedSwatchProperty); set => SetValue(SelectedSwatchProperty, value); }
    public string HexValue => (string)GetValue(HexValueProperty)!;
    public Brush? ColorBrush => (Brush?)GetValue(ColorBrushProperty);
    public IReadOnlyList<GalleryColorSwatch> Swatches => (IReadOnlyList<GalleryColorSwatch>?)GetValue(SwatchesProperty) ?? [];

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _palette = GetTemplateChild("PART_Palette") as ListBox;
        UpdateColor(Color);
    }
    protected override void OnTemplateChanging()
    {
        _palette?.ClearBinding(Jalium.UI.Controls.Primitives.Selector.SelectedItemProperty);
        _palette = null;
        base.OnTemplateChanging();
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled || !IsEnabled || _palette?.IsKeyboardFocusWithin != true || _palette.Items.Count == 0 || e.Key is not (Key.Left or Key.Right)) return;
        var direction = e.Key == Key.Right ? 1 : -1;
        if (FlowDirection == FlowDirection.RightToLeft) direction = -direction;
        var index = Math.Clamp(_palette.SelectedIndex + direction, 0, _palette.Items.Count - 1);
        _palette.SetCurrentValue(Jalium.UI.Controls.Primitives.Selector.SelectedIndexProperty, index);
        if (_palette.ItemContainerGenerator.ContainerFromIndex(index) is UIElement item) item.Focus();
        e.Handled = true;
    }

    private static DependencyProperty ChannelProperty(string name) => DependencyProperty.Register(
        name, typeof(double), typeof(GalleryInspectorColorEditor),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnChannelChanged,
            (_, value) => value is double channel && double.IsFinite(channel) ? Math.Clamp(channel, 0, 255) : 0d));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GalleryInspectorColorEditor)d).UpdateColor((Color)e.NewValue!);
    private static void OnPaletteKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GalleryInspectorColorEditor)d).RebuildPalette();
    private static void OnSelectedSwatchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (GalleryInspectorColorEditor)d;
        if (!editor._updating && e.NewValue is GalleryColorSwatch swatch)
            editor.SetCurrentValue(ColorProperty, swatch.Color);
    }
    private static void OnChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (GalleryInspectorColorEditor)d;
        if (editor._updating) return;
        editor.SetCurrentValue(ColorProperty, Jalium.UI.Media.Color.FromArgb(editor.Color.A,
            (byte)Math.Round(editor.Red), (byte)Math.Round(editor.Green), (byte)Math.Round(editor.Blue)));
    }
    private void UpdateColor(Color color)
    {
        _updating = true;
        try
        {
            SetCurrentValue(RedProperty, (double)color.R);
            SetCurrentValue(GreenProperty, (double)color.G);
            SetCurrentValue(BlueProperty, (double)color.B);
            SetCurrentValue(SelectedSwatchProperty, Swatches.FirstOrDefault(swatch => swatch.Color == color));
            SetValue(HexValuePropertyKey, GalleryThemeTuner.ToHex(color));
            SetValue(ColorBrushPropertyKey, new SolidColorBrush(color));
        }
        finally { _updating = false; }
    }
    private void RebuildPalette()
    {
        var palette = string.Equals(PaletteKind, "Surface", StringComparison.OrdinalIgnoreCase)
            ? GalleryThemeTuner.SurfacePalette : GalleryThemeTuner.AccentPalette;
        SetValue(SwatchesPropertyKey, palette.Select(color => new GalleryColorSwatch(color)).ToArray());
        UpdateColor(Color);
    }
}

/// <summary>Palette data, independent of the visual template and item containers.</summary>
public sealed class GalleryColorSwatch(Color color)
{
    public Color Color { get; } = color;
    public Brush Brush { get; } = new SolidColorBrush(color);
    public string Hex { get; } = GalleryThemeTuner.ToHex(color);
    public override string ToString() => Hex;
}
