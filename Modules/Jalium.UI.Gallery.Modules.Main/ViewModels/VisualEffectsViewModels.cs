namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class LiquidGlassViewModel
{
    private double _blurRadius = 8;
    private double _refractionAmount = 60;
    private double _chromaticAberration;

    public double BlurRadius
    {
        get => _blurRadius;
        set
        {
            if (SetProperty(ref _blurRadius, value))
            {
                OnPropertyChanged(nameof(SettingsSummary));
            }
        }
    }

    public double RefractionAmount
    {
        get => _refractionAmount;
        set
        {
            if (SetProperty(ref _refractionAmount, value))
            {
                OnPropertyChanged(nameof(SettingsSummary));
            }
        }
    }

    public double ChromaticAberration
    {
        get => _chromaticAberration;
        set
        {
            if (SetProperty(ref _chromaticAberration, value))
            {
                OnPropertyChanged(nameof(SettingsSummary));
            }
        }
    }

    public string SettingsSummary =>
        $"Blur={BlurRadius:F0}  Refraction={RefractionAmount:F0}  " +
        $"Chromatic={ChromaticAberration:F2}";
}

public sealed partial class ShaderEffectsViewModel
{
    private double _blurRadius = 10;
    private double _shadowDepth = 5;
    private bool _useBoxBlur;
    private double _invertAmount = 0.75;

    public bool UseBoxBlur
    {
        get => _useBoxBlur;
        set => SetProperty(ref _useBoxBlur, value);
    }

    public double InvertAmount
    {
        get => _invertAmount;
        set => SetProperty(ref _invertAmount, value);
    }

    public double BlurRadius
    {
        get => _blurRadius;
        set => SetProperty(ref _blurRadius, value);
    }

    public double ShadowDepth
    {
        get => _shadowDepth;
        set => SetProperty(ref _shadowDepth, value);
    }
}
