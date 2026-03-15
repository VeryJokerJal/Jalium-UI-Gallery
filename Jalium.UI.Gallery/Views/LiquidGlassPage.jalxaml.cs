using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

public partial class LiquidGlassPage : Page
{
    public LiquidGlassPage()
    {
        InitializeComponent();
        SetupSliders();
    }

    private void SetupSliders()
    {
        if (BlurSlider != null)
        {
            BlurSlider.ValueChanged += (s, e) => UpdateInteractiveGlass();
        }

        if (RefractionSlider != null)
        {
            RefractionSlider.ValueChanged += (s, e) => UpdateInteractiveGlass();
        }

        if (ChromaticSlider != null)
        {
            ChromaticSlider.ValueChanged += (s, e) => UpdateInteractiveGlass();
        }
    }

    private void UpdateInteractiveGlass()
    {
        if (InteractiveGlassDemo == null) return;

        var blur = BlurSlider?.Value ?? 8;
        var refraction = RefractionSlider?.Value ?? 60;
        var chromatic = ChromaticSlider?.Value ?? 0;

        InteractiveGlassDemo.LiquidGlassBlurRadius = blur;
        InteractiveGlassDemo.LiquidGlassRefractionAmount = refraction;
        InteractiveGlassDemo.LiquidGlassChromaticAberration = chromatic;

        if (InteractiveLabel != null)
        {
            InteractiveLabel.Text = $"Blur={blur:F0}  Refraction={refraction:F0}  Chromatic={chromatic:F2}";
        }
    }
}
