using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>A live cross-platform HLSL example with one constant register.</summary>
public sealed class GalleryInvertEffect : ShaderEffect
{
    public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
        nameof(Amount), typeof(double), typeof(GalleryInvertEffect),
        new PropertyMetadata(1.0, PixelShaderConstantCallback(0)));

    public double Amount
    {
        get => (double)GetValue(AmountProperty)!;
        set => SetValue(AmountProperty, value);
    }

    public GalleryInvertEffect()
    {
        PixelShader = new PixelShader
        {
            SourceHlsl = """
                Texture2D image : register(t0);
                SamplerState imageSampler : register(s0);
                cbuffer Constants : register(b0) { float4 amount; }
                float4 main(float2 uv : TEXCOORD0) : SV_Target
                {
                    float4 color = image.Sample(imageSampler, uv);
                    return float4(lerp(color.rgb, color.a - color.rgb, amount.x), color.a);
                }
                """
        };
        UpdateShaderValue(AmountProperty);
    }
}
