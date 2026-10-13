namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed record CategoryValuePoint(
    string Category,
    double Value);

public sealed record NumericValuePoint(
    double X,
    double Y,
    double Size = 0);
