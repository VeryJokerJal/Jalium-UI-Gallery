using Jalium.UI.Controls.TextEffects;
using Jalium.UI.Controls.TextEffects.Effects;
using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class TextEffectPresenterBehavior
{
    public static readonly DependencyProperty AnimationEffectProperty =
        DependencyProperty.RegisterAttached(
            "AnimationEffect",
            typeof(TextAnimationEffect),
            typeof(TextEffectPresenterBehavior),
            new PropertyMetadata(
                TextAnimationEffect.RiseSettle,
                OnAnimationEffectChanged));

    public static readonly DependencyProperty GpuEffectProperty =
        DependencyProperty.RegisterAttached(
            "GpuEffect",
            typeof(TextGpuEffect),
            typeof(TextEffectPresenterBehavior),
            new PropertyMetadata(
                TextGpuEffect.None,
                OnGpuEffectChanged));

    public static void SetAnimationEffect(
        DependencyObject element,
        TextAnimationEffect value) =>
        element.SetValue(AnimationEffectProperty, value);

    public static TextAnimationEffect GetAnimationEffect(
        DependencyObject element) =>
        (TextAnimationEffect)(element.GetValue(AnimationEffectProperty) ??
            TextAnimationEffect.RiseSettle);

    public static void SetGpuEffect(
        DependencyObject element,
        TextGpuEffect value) =>
        element.SetValue(GpuEffectProperty, value);

    public static TextGpuEffect GetGpuEffect(
        DependencyObject element) =>
        (TextGpuEffect)(element.GetValue(GpuEffectProperty) ??
            TextGpuEffect.None);

    private static void OnAnimationEffectChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TextEffectPresenter presenter ||
            args.NewValue is not TextAnimationEffect effect)
        {
            return;
        }

        presenter.TextEffect = effect switch
        {
            TextAnimationEffect.FadeIn => new FadeInEffect(),
            TextAnimationEffect.Typewriter =>
                new TypewriterEffect { CharacterDelayMs = 55 },
            TextAnimationEffect.WaveBounce => new WaveBounceEffect(),
            TextAnimationEffect.PulsingBlur =>
                new PulsingBlurTextEffect
                {
                    AmplitudePx = 8,
                    BaselinePx = 1,
                    PeriodMs = 2400
                },
            _ => new RiseSettleEffect()
        };
    }

    private static void OnGpuEffectChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TextEffectPresenter presenter ||
            args.NewValue is not TextGpuEffect effect)
        {
            return;
        }

        presenter.Effect = effect switch
        {
            TextGpuEffect.Blur4 =>
                new Jalium.UI.Media.Effects.BlurEffect(4),
            TextGpuEffect.Blur10 =>
                new Jalium.UI.Media.Effects.BlurEffect(10),
            _ => null
        };
    }
}
