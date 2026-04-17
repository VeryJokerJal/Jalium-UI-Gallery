using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Views;

public partial class ShaderEffectsPage : Page
{
    private const string XamlExample =
@"<!-- BlurEffect with adjustable radius -->
<Border x:Name=""BlurDemo""
        Background=""#404040""
        Width=""200"" Height=""100""
        CornerRadius=""4"">
    <TextBlock Text=""Blur Demo""
               Foreground=""#FFFFFF""
               HorizontalAlignment=""Center""
               VerticalAlignment=""Center"" />
</Border>

<!-- DropShadowEffect on a card -->
<Border x:Name=""ShadowDemo""
        Background=""#0078D4""
        Width=""150"" Height=""80""
        CornerRadius=""8"">
    <TextBlock Text=""Shadow Demo""
               Foreground=""#FFFFFF""
               HorizontalAlignment=""Center""
               VerticalAlignment=""Center"" />
</Border>

<!-- Slider controls for interactive adjustment -->
<Slider x:Name=""BlurRadiusSlider""
        Minimum=""0"" Maximum=""50"" Value=""10"" />
<Slider x:Name=""ShadowDepthSlider""
        Minimum=""0"" Maximum=""30"" Value=""5"" />";

    private const string CSharpExample =
@"using Jalium.UI.Controls;
using Jalium.UI.Media.Effects;

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

        // Apply BlurEffect to a Border element
        BlurDemo.Effect = _blurEffect;

        // Apply DropShadowEffect to another element
        ShadowDemo.Effect = _dropShadowEffect;

        // Interactive slider control
        BlurRadiusSlider.ValueChanged += (s, e) =>
        {
            _blurEffect.Radius = e.NewValue;
            BlurRadiusText.Text = ((int)e.NewValue).ToString();
        };

        ShadowDepthSlider.ValueChanged += (s, e) =>
        {
            _dropShadowEffect.ShadowDepth = e.NewValue;
            ShadowDepthText.Text = ((int)e.NewValue).ToString();
        };
    }
}";

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
