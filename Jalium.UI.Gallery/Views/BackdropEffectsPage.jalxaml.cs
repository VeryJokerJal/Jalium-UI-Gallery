using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class BackdropEffectsPage : Page
{
    public BackdropEffectsPage()
    {
        InitializeComponent();
        SetupBackdropEffects();
        SetupSystemBackdropButtons();
    }

    private void SetupBackdropEffects()
    {
        // Use generated fields directly (x:Name generates fields in partial class)
        // Material effects
        if (BlurEffectDemo != null)
            BlurEffectDemo.BackdropEffect = new BlurEffect(20f);

        if (AcrylicEffectDemo != null)
            AcrylicEffectDemo.BackdropEffect = new AcrylicEffect(
                Color.FromArgb(200, 30, 30, 40),
                tintOpacity: 0.6f,
                blurRadius: 30f);

        if (MicaEffectDemo != null)
            MicaEffectDemo.BackdropEffect = new MicaEffect();

        if (FrostedGlassEffectDemo != null)
            FrostedGlassEffectDemo.BackdropEffect = new FrostedGlassEffect(
                blurRadius: 15f,
                noiseIntensity: 0.04f,
                tintColor: Color.White,
                tintOpacity: 0.3f);

        // Color adjustment effects
        if (GrayscaleEffectDemo != null)
            GrayscaleEffectDemo.BackdropEffect = ColorAdjustmentEffect.CreateGrayscale(1.0f);
        if (SepiaEffectDemo != null)
            SepiaEffectDemo.BackdropEffect = ColorAdjustmentEffect.CreateSepia(0.8f);
        if (InvertEffectDemo != null)
            InvertEffectDemo.BackdropEffect = ColorAdjustmentEffect.CreateInvert(1.0f);
        if (HueRotateEffectDemo != null)
            HueRotateEffectDemo.BackdropEffect = ColorAdjustmentEffect.CreateHueRotate(90f);
    }

    private void SetupSystemBackdropButtons()
    {
        if (BackdropNoneButton != null)
            BackdropNoneButton.Click += (s, e) => SetWindowBackdrop(WindowBackdropType.None);

        if (BackdropMicaButton != null)
            BackdropMicaButton.Click += (s, e) => SetWindowBackdrop(WindowBackdropType.Mica);

        if (BackdropMicaAltButton != null)
            BackdropMicaAltButton.Click += (s, e) => SetWindowBackdrop(WindowBackdropType.MicaAlt);

        if (BackdropAcrylicButton != null)
            BackdropAcrylicButton.Click += (s, e) => SetWindowBackdrop(WindowBackdropType.Acrylic);
    }

    private void SetWindowBackdrop(WindowBackdropType backdropType)
    {
        // Find the parent window
        var window = FindParentWindow();
        if (window != null)
        {
            //window.Background = new SolidColorBrush(Color.FromArgb(100, 240, 240, 240));
            window.SystemBackdrop = backdropType;

            // Update status text
            if (SystemBackdropStatus != null)
            {
                SystemBackdropStatus.Text = $"Current: {backdropType}";
            }
        }
    }

    private Window? FindParentWindow()
    {
        Visual? current = this;
        while (current != null)
        {
            if (current is Window window)
                return window;
            current = current.VisualParent;
        }
        return null;
    }
}
