using Jalium.UI;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed record ColorTokenSample(
    string Name,
    string Role,
    Brush Preview,
    string Value);

public sealed record TypeTokenSample(
    string Name,
    double Size,
    string Weight,
    string Sample);

public sealed record MetricTokenSample(
    string Name,
    string Value,
    string Usage);

public sealed record SpacingTokenSample(
    string Name,
    double Pixels,
    string Usage)
{
    public string Value => $"{Pixels:0} px";
}

public sealed record RadiusTokenSample(
    string Name,
    double Pixels,
    string Usage)
{
    public string Value => $"{Pixels:0} px";

    public CornerRadius PreviewRadius => new(Pixels);
}
