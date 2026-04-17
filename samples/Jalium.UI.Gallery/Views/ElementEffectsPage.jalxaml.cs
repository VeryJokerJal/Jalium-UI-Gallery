using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Views;

public partial class ElementEffectsPage : Page
{
    private OuterGlowEffect _outerGlowEffect = new()
    {
        GlowSize = 10,
        GlowColor = Color.FromArgb(255, 0, 120, 212),
        Intensity = 1.0,
        Opacity = 0.9
    };

    private InnerShadowEffect _innerShadowEffect = new()
    {
        BlurRadius = 8,
        ShadowDepth = 5,
        Color = Color.FromArgb(180, 0, 0, 0),
        Direction = 315
    };

    private EmbossEffect _embossEffect = new()
    {
        Amount = 1.5,
        LightAngle = 45,
        Relief = 0.44,
        Width = 1.0
    };

    public ElementEffectsPage()
    {
        InitializeComponent();
        SetupEffects();
        SetupSliders();
        SetupButtons();
        LoadCodeExamples();
    }

    private void SetupEffects()
    {
        if (OuterGlowDemo != null)
            OuterGlowDemo.Effect = _outerGlowEffect;

        if (InnerShadowDemo != null)
            InnerShadowDemo.Effect = _innerShadowEffect;

        if (EmbossDemo != null)
            EmbossDemo.Effect = _embossEffect;
    }

    private void SetupSliders()
    {
        if (GlowSizeSlider != null)
        {
            GlowSizeSlider.ValueChanged += (s, e) =>
            {
                if (GlowSizeText != null)
                    GlowSizeText.Text = ((int)e.NewValue).ToString();
                _outerGlowEffect.GlowSize = e.NewValue;
            };
        }

        if (GlowIntensitySlider != null)
        {
            GlowIntensitySlider.ValueChanged += (s, e) =>
            {
                var intensity = e.NewValue / 10.0;
                if (GlowIntensityText != null)
                    GlowIntensityText.Text = intensity.ToString("F1");
                _outerGlowEffect.Intensity = intensity;
            };
        }

        if (InnerShadowBlurSlider != null)
        {
            InnerShadowBlurSlider.ValueChanged += (s, e) =>
            {
                if (InnerShadowBlurText != null)
                    InnerShadowBlurText.Text = ((int)e.NewValue).ToString();
                _innerShadowEffect.BlurRadius = e.NewValue;
            };
        }

        if (InnerShadowDepthSlider != null)
        {
            InnerShadowDepthSlider.ValueChanged += (s, e) =>
            {
                if (InnerShadowDepthText != null)
                    InnerShadowDepthText.Text = ((int)e.NewValue).ToString();
                _innerShadowEffect.ShadowDepth = e.NewValue;
            };
        }

        if (EmbossAmountSlider != null)
        {
            EmbossAmountSlider.ValueChanged += (s, e) =>
            {
                var amount = e.NewValue / 10.0;
                if (EmbossAmountText != null)
                    EmbossAmountText.Text = amount.ToString("F1");
                _embossEffect.Amount = amount;
            };
        }

        if (EmbossAngleSlider != null)
        {
            EmbossAngleSlider.ValueChanged += (s, e) =>
            {
                if (EmbossAngleText != null)
                    EmbossAngleText.Text = ((int)e.NewValue).ToString();
                _embossEffect.LightAngle = e.NewValue;
            };
        }

        if (SaturationSlider != null)
        {
            SaturationSlider.ValueChanged += (s, e) =>
            {
                var saturation = e.NewValue / 10.0;
                if (SaturationText != null)
                    SaturationText.Text = saturation.ToString("F1");
                ApplyColorMatrix(ColorMatrixEffect.CreateSaturation(saturation));
            };
        }
    }

    private void SetupButtons()
    {
        if (GrayscaleBtn != null)
            GrayscaleBtn.Click += (s, e) =>
            {
                ApplyColorMatrix(ColorMatrixEffect.CreateGrayscale());
                if (SaturationSlider != null) SaturationSlider.Value = 0;
            };

        if (SepiaBtn != null)
            SepiaBtn.Click += (s, e) => ApplyColorMatrix(ColorMatrixEffect.CreateSepia());

        if (InvertBtn != null)
            InvertBtn.Click += (s, e) => ApplyColorMatrix(ColorMatrixEffect.CreateInvert());

        if (HueRotateBtn != null)
            HueRotateBtn.Click += (s, e) => ApplyColorMatrix(ColorMatrixEffect.CreateHueRotation(90));

        if (ResetColorBtn != null)
            ResetColorBtn.Click += (s, e) =>
            {
                ClearColorMatrix();
                if (SaturationSlider != null) SaturationSlider.Value = 10;
            };
    }

    private void ApplyColorMatrix(ColorMatrixEffect effect)
    {
        var demos = new[] { ColorDemo1, ColorDemo2, ColorDemo3, ColorDemo4, ColorDemo5, ColorDemo6 };
        foreach (var demo in demos)
        {
            if (demo != null)
                demo.Effect = effect;
        }
    }

    private void ClearColorMatrix()
    {
        var demos = new[] { ColorDemo1, ColorDemo2, ColorDemo3, ColorDemo4, ColorDemo5, ColorDemo6 };
        foreach (var demo in demos)
        {
            if (demo != null)
                demo.Effect = null;
        }
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

    private const string XamlExample =
@"<!-- OuterGlowEffect applied to an element -->
<Border Background=""#0078D4"" Width=""160"" Height=""60"" CornerRadius=""8"">
    <Border.Effect>
        <OuterGlowEffect GlowSize=""10""
                          GlowColor=""#0078D4""
                          Intensity=""1.0""
                          Opacity=""0.9""/>
    </Border.Effect>
    <TextBlock Text=""Outer Glow"" Foreground=""#FFFFFF""
               HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
</Border>

<!-- InnerShadowEffect -->
<Border Background=""#3D3D50"" Width=""220"" Height=""100"" CornerRadius=""12"">
    <Border.Effect>
        <InnerShadowEffect BlurRadius=""8""
                            ShadowDepth=""5""
                            Color=""#B4000000""
                            Direction=""315""/>
    </Border.Effect>
</Border>

<!-- EffectGroup composing multiple effects -->
<Border Background=""#CC5500"" Width=""180"" Height=""70"" CornerRadius=""8"">
    <Border.Effect>
        <EffectGroup>
            <OuterGlowEffect GlowColor=""Orange"" GlowSize=""12""/>
            <DropShadowEffect ShadowDepth=""6"" BlurRadius=""8""/>
        </EffectGroup>
    </Border.Effect>
</Border>";

    private const string CSharpExample =
@"// Create and apply an OuterGlowEffect
var glowEffect = new OuterGlowEffect
{
    GlowSize = 10,
    GlowColor = Color.FromArgb(255, 0, 120, 212),
    Intensity = 1.0,
    Opacity = 0.9
};
element.Effect = glowEffect;

// Create an InnerShadowEffect
var innerShadow = new InnerShadowEffect
{
    BlurRadius = 8,
    ShadowDepth = 5,
    Color = Color.FromArgb(180, 0, 0, 0),
    Direction = 315
};

// Create an EmbossEffect
var emboss = new EmbossEffect
{
    Amount = 1.5,
    LightAngle = 45,
    Relief = 0.44,
    Width = 1.0
};

// ColorMatrixEffect presets
var grayscale = ColorMatrixEffect.CreateGrayscale();
var sepia = ColorMatrixEffect.CreateSepia();
var invert = ColorMatrixEffect.CreateInvert();
var hueRotate = ColorMatrixEffect.CreateHueRotation(90);
var saturation = ColorMatrixEffect.CreateSaturation(1.5);

// Compose multiple effects with EffectGroup
var group = new EffectGroup();
group.Children.Add(new OuterGlowEffect
    { GlowColor = Colors.Orange, GlowSize = 12 });
group.Children.Add(new DropShadowEffect
    { ShadowDepth = 6, BlurRadius = 8 });
element.Effect = group;";
}
