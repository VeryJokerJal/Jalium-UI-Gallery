using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class ShaderEffectsPage : Page
{
    public ShaderEffectsPage()
    {
        InitializeComponent();
        SetupEffects();
        SetupSliders();
    }

    private void SetupEffects()
    {
        // Apply initial blur effect (using backdrop blur effect)
        if (BlurEffectDemo != null)
        {
            BlurEffectDemo.BackdropEffect = new Jalium.UI.Media.BlurEffect(10f);
        }

        // Apply initial drop shadow effect
        // Note: DropShadowEffect would need to be applied via Effect property
        // For now we simulate with BackdropEffect
    }

    private void SetupSliders()
    {
        if (BlurRadiusSlider != null)
        {
            BlurRadiusSlider.ValueChanged += (s, e) =>
            {
                if (BlurRadiusText != null)
                    BlurRadiusText.Text = ((int)e.NewValue).ToString();

                if (BlurEffectDemo != null)
                    BlurEffectDemo.BackdropEffect = new Jalium.UI.Media.BlurEffect((float)e.NewValue);
            };
        }

        if (ShadowDepthSlider != null)
        {
            ShadowDepthSlider.ValueChanged += (s, e) =>
            {
                if (ShadowDepthText != null)
                    ShadowDepthText.Text = ((int)e.NewValue).ToString();
            };
        }
    }
}
