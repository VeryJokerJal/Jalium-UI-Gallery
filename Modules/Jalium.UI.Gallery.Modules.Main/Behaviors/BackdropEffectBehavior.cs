using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Adapts a declarative preset to Jalium's view-only backdrop effect API.
/// The behavior owns no application state.
/// </summary>
public static class BackdropEffectBehavior
{
    public static readonly DependencyProperty PresetProperty =
        DependencyProperty.RegisterAttached(
            "Preset",
            typeof(BackdropEffectPreset),
            typeof(BackdropEffectBehavior),
            new PropertyMetadata(
                BackdropEffectPreset.None,
                OnPresetChanged));

    public static void SetPreset(
        DependencyObject element,
        BackdropEffectPreset value) =>
        element.SetValue(PresetProperty, value);

    public static BackdropEffectPreset GetPreset(
        DependencyObject element) =>
        (BackdropEffectPreset)(element.GetValue(PresetProperty) ??
            BackdropEffectPreset.None);

    private static void OnPresetChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        element.BackdropEffect = GetPreset(element) switch
        {
            BackdropEffectPreset.Blur =>
                new BackdropBlurEffect(20f),
            BackdropEffectPreset.Acrylic =>
                new AcrylicEffect(
                    Color.FromArgb(200, 30, 30, 40),
                    tintOpacity: 0.6f,
                    blurRadius: 30f),
            BackdropEffectPreset.Mica =>
                new MicaEffect(),
            BackdropEffectPreset.FrostedGlass =>
                new FrostedGlassEffect(
                    blurRadius: 15f,
                    noiseIntensity: 0.04f,
                    tintColor: Color.White,
                    tintOpacity: 0.3f),
            BackdropEffectPreset.Grayscale =>
                ColorAdjustmentEffect.CreateGrayscale(1f),
            BackdropEffectPreset.Sepia =>
                ColorAdjustmentEffect.CreateSepia(0.8f),
            BackdropEffectPreset.Invert =>
                ColorAdjustmentEffect.CreateInvert(1f),
            BackdropEffectPreset.HueRotate =>
                ColorAdjustmentEffect.CreateHueRotate(90f),
            _ => null
        };
    }
}
