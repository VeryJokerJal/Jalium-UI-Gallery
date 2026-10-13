using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class ColorMatrixEffectBehavior
{
    public static readonly DependencyProperty PresetProperty =
        DependencyProperty.RegisterAttached(
            "Preset",
            typeof(ColorMatrixPreset),
            typeof(ColorMatrixEffectBehavior),
            new PropertyMetadata(
                ColorMatrixPreset.None,
                OnEffectChanged));

    public static readonly DependencyProperty SaturationProperty =
        DependencyProperty.RegisterAttached(
            "Saturation",
            typeof(double),
            typeof(ColorMatrixEffectBehavior),
            new PropertyMetadata(1d, OnEffectChanged));

    public static void SetPreset(
        DependencyObject element,
        ColorMatrixPreset value) =>
        element.SetValue(PresetProperty, value);

    public static ColorMatrixPreset GetPreset(
        DependencyObject element) =>
        (ColorMatrixPreset)(element.GetValue(PresetProperty) ??
            ColorMatrixPreset.None);

    public static void SetSaturation(
        DependencyObject element,
        double value) =>
        element.SetValue(SaturationProperty, value);

    public static double GetSaturation(DependencyObject element) =>
        (double)(element.GetValue(SaturationProperty) ?? 1d);

    private static void OnEffectChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        element.Effect = GetPreset(element) switch
        {
            ColorMatrixPreset.Saturation =>
                ColorMatrixEffect.CreateSaturation(
                    GetSaturation(element)),
            ColorMatrixPreset.Grayscale =>
                ColorMatrixEffect.CreateGrayscale(),
            ColorMatrixPreset.Sepia =>
                ColorMatrixEffect.CreateSepia(),
            ColorMatrixPreset.Invert =>
                ColorMatrixEffect.CreateInvert(),
            ColorMatrixPreset.HueRotate =>
                ColorMatrixEffect.CreateHueRotation(90),
            _ => null
        };
    }
}
