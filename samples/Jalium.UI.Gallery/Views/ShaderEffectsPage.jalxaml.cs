using Jalium.UI.Controls;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Views;

public partial class ShaderEffectsPage : Page
{
    private BlurEffect _blurEffect = new(10);
    private DropShadowEffect _dropShadowEffect = new()
    {
        BlurRadius = 10,
        ShadowDepth = 5,
        Direction = 315,
        Opacity = 0.8
    };

    public ShaderEffectsPage()
    {
        InitializeComponent();
        SetupEffects();
        SetupSliders();
    }

    private void SetupEffects()
    {
        if (BlurEffectDemo != null)
        {
            BlurEffectDemo.Effect = _blurEffect;
        }

        if (DropShadowDemo != null)
        {
            DropShadowDemo.Effect = _dropShadowEffect;
        }
    }

    private void SetupSliders()
    {
        if (BlurRadiusSlider != null)
        {
            BlurRadiusSlider.ValueChanged += (s, e) =>
            {
                if (BlurRadiusText != null)
                    BlurRadiusText.Text = ((int)e.NewValue).ToString();

                _blurEffect.Radius = e.NewValue;
            };
        }

        if (ShadowDepthSlider != null)
        {
            ShadowDepthSlider.ValueChanged += (s, e) =>
            {
                if (ShadowDepthText != null)
                    ShadowDepthText.Text = ((int)e.NewValue).ToString();

                _dropShadowEffect.ShadowDepth = e.NewValue;
            };
        }
    }
}
