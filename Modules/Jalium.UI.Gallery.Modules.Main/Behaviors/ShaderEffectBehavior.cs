using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>Applies bound ViewModel parameters to effects outside the visual tree.</summary>
public static class ShaderEffectBehavior
{
    public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.RegisterAttached(
        "BlurRadius", typeof(double), typeof(ShaderEffectBehavior), new PropertyMetadata(double.NaN, OnBlurChanged));
    public static double GetBlurRadius(DependencyObject element) => (double)element.GetValue(BlurRadiusProperty)!;
    public static void SetBlurRadius(DependencyObject element, double value) => element.SetValue(BlurRadiusProperty, value);
    public static readonly DependencyProperty UseBoxBlurProperty = DependencyProperty.RegisterAttached(
        "UseBoxBlur", typeof(bool), typeof(ShaderEffectBehavior), new PropertyMetadata(false, OnBlurChanged));
    public static bool GetUseBoxBlur(DependencyObject element) => (bool)element.GetValue(UseBoxBlurProperty)!;
    public static void SetUseBoxBlur(DependencyObject element, bool value) => element.SetValue(UseBoxBlurProperty, value);
    public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.RegisterAttached(
        "ShadowDepth", typeof(double), typeof(ShaderEffectBehavior), new PropertyMetadata(double.NaN, OnShadowChanged));
    public static double GetShadowDepth(DependencyObject element) => (double)element.GetValue(ShadowDepthProperty)!;
    public static void SetShadowDepth(DependencyObject element, double value) => element.SetValue(ShadowDepthProperty, value);
    public static readonly DependencyProperty InvertAmountProperty = DependencyProperty.RegisterAttached(
        "InvertAmount", typeof(double), typeof(ShaderEffectBehavior), new PropertyMetadata(double.NaN, OnInvertChanged));
    public static double GetInvertAmount(DependencyObject element) => (double)element.GetValue(InvertAmountProperty)!;
    public static void SetInvertAmount(DependencyObject element, double value) => element.SetValue(InvertAmountProperty, value);

    private static void OnBlurChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is not UIElement target) return;
        var effect = target.Effect as BlurEffect ?? new BlurEffect();
        var radius = GetBlurRadius(target);
        effect.Radius = double.IsNaN(radius) ? 10 : radius;
        effect.KernelType = GetUseBoxBlur(target) ? KernelType.Box : KernelType.Gaussian;
        target.Effect = effect;
    }
    private static void OnShadowChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is not UIElement target) return;
        var effect = target.Effect as DropShadowEffect ?? new DropShadowEffect { BlurRadius = 10, Direction = 315, Opacity = 0.8 };
        effect.ShadowDepth = GetShadowDepth(target);
        target.Effect = effect;
    }
    private static void OnInvertChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is not UIElement target) return;
        var effect = target.Effect as GalleryInvertEffect ?? new GalleryInvertEffect();
        effect.Amount = GetInvertAmount(target);
        target.Effect = effect;
    }
}
