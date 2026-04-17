using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class BackdropEffectsPage : Page
{
    private const string XamlExample =
@"<!-- Blur effect on a Border overlay -->
<Border x:Name=""BlurOverlay""
        Margin=""20""
        CornerRadius=""8""
        HorizontalAlignment=""Stretch""
        VerticalAlignment=""Stretch"">
    <TextBlock Text=""BlurEffect (20px)""
               Foreground=""#FFFFFF""
               HorizontalAlignment=""Center""
               VerticalAlignment=""Center"" />
</Border>

<!-- Acrylic effect on a Border overlay -->
<Border x:Name=""AcrylicOverlay""
        Margin=""20""
        CornerRadius=""8""
        HorizontalAlignment=""Stretch""
        VerticalAlignment=""Stretch"">
    <TextBlock Text=""AcrylicEffect (Blue Tint)""
               Foreground=""#FFFFFF""
               HorizontalAlignment=""Center""
               VerticalAlignment=""Center"" />
</Border>

<!-- Window system backdrop buttons -->
<Button x:Name=""BackdropMicaButton""
        Content=""Mica"" Padding=""16,8"" />
<Button x:Name=""BackdropAcrylicButton""
        Content=""Acrylic"" Padding=""16,8"" />";

    private const string CSharpExample =
@"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class BackdropEffectsPage : Page
{
    public BackdropEffectsPage()
    {
        InitializeComponent();
        SetupBackdropEffects();
    }

    private void SetupBackdropEffects()
    {
        // Simple Gaussian blur
        BlurOverlay.BackdropEffect = new BlurEffect(20f);

        // Acrylic material with tint color and opacity
        AcrylicOverlay.BackdropEffect = new AcrylicEffect(
            Color.FromArgb(200, 30, 30, 40),
            tintOpacity: 0.6f,
            blurRadius: 30f);

        // Mica material (Windows 11 style)
        MicaOverlay.BackdropEffect = new MicaEffect();

        // Frosted glass with noise texture
        FrostedOverlay.BackdropEffect = new FrostedGlassEffect(
            blurRadius: 15f,
            noiseIntensity: 0.04f,
            tintColor: Color.White,
            tintOpacity: 0.3f);

        // Color adjustment effects
        GrayscaleOverlay.BackdropEffect =
            ColorAdjustmentEffect.CreateGrayscale(1.0f);
        SepiaOverlay.BackdropEffect =
            ColorAdjustmentEffect.CreateSepia(0.8f);
        InvertOverlay.BackdropEffect =
            ColorAdjustmentEffect.CreateInvert(1.0f);
        HueRotateOverlay.BackdropEffect =
            ColorAdjustmentEffect.CreateHueRotate(90f);
    }

    private void SetWindowBackdrop(WindowBackdropType type)
    {
        var window = FindParentWindow();
        if (window != null)
            window.SystemBackdrop = type;
    }
}";

    public BackdropEffectsPage()
    {
        InitializeComponent();
        SetupBackdropEffects();
        SetupSystemBackdropButtons();
        LoadCodeExamples();
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
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
