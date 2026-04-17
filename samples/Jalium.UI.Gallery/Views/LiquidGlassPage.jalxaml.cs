using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

public partial class LiquidGlassPage : Page
{
    private const string XamlExample =
@"<!-- Default Liquid Glass -->
<Border x:Name=""GlassOverlay""
        Margin=""30""
        CornerRadius=""16""
        LiquidGlass=""True""
        LiquidGlassInteractive=""True""
        Background=""#200078D4""
        HorizontalAlignment=""Stretch""
        VerticalAlignment=""Stretch"">
    <TextBlock Text=""Liquid Glass""
               Foreground=""#FFFFFF""
               FontSize=""16""
               HorizontalAlignment=""Center""
               VerticalAlignment=""Center"" />
</Border>

<!-- Chromatic Aberration Liquid Glass -->
<Border CornerRadius=""16""
        LiquidGlass=""True""
        LiquidGlassBlurRadius=""12""
        LiquidGlassRefractionAmount=""80""
        LiquidGlassChromaticAberration=""1.5""
        Background=""#18FFFFFF"">
    <TextBlock Text=""Chromatic Glass""
               Foreground=""#FFFFFF"" />
</Border>

<!-- Interactive parameter controls -->
<Slider x:Name=""BlurSlider""
        Minimum=""0"" Maximum=""30"" Value=""8"" />
<Slider x:Name=""RefractionSlider""
        Minimum=""0"" Maximum=""150"" Value=""60"" />
<Slider x:Name=""ChromaticSlider""
        Minimum=""0"" Maximum=""5"" Value=""0"" />";

    private const string CSharpExample =
@"using Jalium.UI.Controls;

public partial class LiquidGlassPage : Page
{
    public LiquidGlassPage()
    {
        InitializeComponent();
        SetupSliders();
    }

    private void SetupSliders()
    {
        BlurSlider.ValueChanged += (s, e) => UpdateGlass();
        RefractionSlider.ValueChanged += (s, e) => UpdateGlass();
        ChromaticSlider.ValueChanged += (s, e) => UpdateGlass();
    }

    private void UpdateGlass()
    {
        if (InteractiveGlassDemo == null) return;

        var blur = BlurSlider?.Value ?? 8;
        var refraction = RefractionSlider?.Value ?? 60;
        var chromatic = ChromaticSlider?.Value ?? 0;

        // Update Liquid Glass properties dynamically
        InteractiveGlassDemo.LiquidGlassBlurRadius = blur;
        InteractiveGlassDemo.LiquidGlassRefractionAmount =
            refraction;
        InteractiveGlassDemo.LiquidGlassChromaticAberration =
            chromatic;

        StatusLabel.Text =
            $""Blur={blur:F0} Refraction={refraction:F0} "" +
            $""Chromatic={chromatic:F2}"";
    }
}";

    public LiquidGlassPage()
    {
        InitializeComponent();
        SetupSliders();
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
