using Jalium.UI.Controls;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class ShaderEffectsPage : Page
{
    public ShaderEffectsPage()
    {
        InitializeComponent();
        // ShaderEffect is outside the visual tree. Connect the named slider
        // explicitly so its constants update even without an inherited scope.
        if (BlurPreview.Effect is BlurEffect blur)
        {
            blur.Radius = BlurRadiusSlider.Value;
            BlurRadiusSlider.ValueChanged += (_, _) => blur.Radius = BlurRadiusSlider.Value;
            BoxBlurCheckBox.Checked += (_, _) => blur.KernelType = KernelType.Box;
            BoxBlurCheckBox.Unchecked += (_, _) => blur.KernelType = KernelType.Gaussian;
        }
        if (ShadowPreview.Effect is DropShadowEffect shadow)
        {
            shadow.ShadowDepth = ShadowDepthSlider.Value;
            ShadowDepthSlider.ValueChanged += (_, _) => shadow.ShadowDepth = ShadowDepthSlider.Value;
        }
        if (HlslPreview.Effect is GalleryInvertEffect shader)
        {
            shader.Amount = InvertAmountSlider.Value;
            InvertAmountSlider.ValueChanged += (_, _) => shader.Amount = InvertAmountSlider.Value;
        }
    }
}
