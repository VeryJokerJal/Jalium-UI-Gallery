using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed record SymbolSample(
    string Name,
    Symbol Symbol);

public sealed record SymbolCategory(
    string Name,
    IReadOnlyList<SymbolSample> Symbols);

public sealed record FontIconSample(
    string Glyph,
    string Label,
    double Size,
    Brush Foreground);
