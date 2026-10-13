using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum ThemeResourceKind
{
    Color,
    SolidBrush,
    GradientBrush
}

public sealed record ThemeResourceEntry(
    string Key,
    ThemeResourceKind Kind,
    string Display,
    Brush Preview)
{
    public string KindLabel => Kind switch
    {
        ThemeResourceKind.Color => "Color",
        ThemeResourceKind.SolidBrush => "SolidColorBrush",
        ThemeResourceKind.GradientBrush => "GradientBrush",
        _ => Kind.ToString()
    };

    public string Detail => $"{KindLabel} · {Display}";

    public string ThemeResourceExpression =>
        "{ThemeResource " + Key + "}";

    public string StaticResourceExpression =>
        "{StaticResource " + Key + "}";

    public string CopyThemeResourceLabel =>
        "Copy {ThemeResource " + Key + "}";

    public string CopyStaticResourceLabel =>
        "Copy {StaticResource " + Key + "}";

    public string CopyValueLabel => $"Copy value ({Display})";
}
