using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class IconElementViewModel
{
    public IconElementViewModel()
    {
        FeaturedSymbols = CreateFeaturedSymbols();
        FontIcons = CreateFontIcons();
        SymbolCategories = CreateSymbolCategories();
    }

    public IReadOnlyList<SymbolSample> FeaturedSymbols { get; }

    public IReadOnlyList<FontIconSample> FontIcons { get; }

    public IReadOnlyList<SymbolCategory> SymbolCategories { get; }

    private static IReadOnlyList<SymbolSample> CreateFeaturedSymbols()
    {
        Symbol[] values =
        [
            Symbol.Save,
            Symbol.OpenFile,
            Symbol.Document,
            Symbol.Delete,
            Symbol.Undo,
            Symbol.Redo,
            Symbol.Copy,
            Symbol.Cut,
            Symbol.Paste,
            Symbol.Play,
            Symbol.Pause,
            Symbol.Stop,
            Symbol.Refresh,
            Symbol.Find,
            Symbol.Search,
            Symbol.Settings,
            Symbol.Help,
            Symbol.Home,
            Symbol.Back,
            Symbol.Forward,
            Symbol.Add,
            Symbol.Edit,
            Symbol.Pin,
            Symbol.FavoriteStar,
            Symbol.Share,
            Symbol.Download,
            Symbol.Upload,
            Symbol.Print,
            Symbol.Lock
        ];

        return values
            .Select(value => new SymbolSample(
                value.ToString(),
                value))
            .ToArray();
    }

    private static IReadOnlyList<FontIconSample> CreateFontIcons()
    {
        // Theme-sourced brushes, not literals: these are GalleryTheme's shared
        // mutable instances, so a Dark/Light flip recolors the existing icons in
        // place. The *OnSurface variants keep each sample's named hue while
        // clearing 4.5:1 in both modes.
        return
        [
            new("\uE713", "Settings 16px", 16, GalleryTheme.TextPrimaryBrush),
            new("\uE713", "Settings 24px", 24, GalleryTheme.TextPrimaryBrush),
            new("\uE713", "Settings 32px", 32, GalleryTheme.TextPrimaryBrush),
            new("\uE768", "Play (Green)", 24, GalleryTheme.SuccessOnSurfaceBrush),
            new("\uE71A", "Stop (Red)", 24, GalleryTheme.ErrorOnSurfaceBrush),
            new("\uE7BA", "Warning (Yellow)", 24, GalleryTheme.WarningOnSurfaceBrush),
            new("\uE946", "Info (Blue)", 24, GalleryTheme.InfoOnSurfaceBrush)
        ];
    }

    private static IReadOnlyList<SymbolCategory>
        CreateSymbolCategories()
    {
        var samples = Enum.GetNames<Symbol>()
            .Select(name => new SymbolSample(
                name,
                Enum.Parse<Symbol>(name)))
            .OrderBy(sample => sample.Name, StringComparer.Ordinal)
            .ToArray();

        return
        [
            CreateCategory("A - F", samples, 'A', 'F'),
            CreateCategory("G - L", samples, 'G', 'L'),
            CreateCategory("M - R", samples, 'M', 'R'),
            CreateCategory("S - Z", samples, 'S', 'Z')
        ];
    }

    private static SymbolCategory CreateCategory(
        string name,
        IEnumerable<SymbolSample> symbols,
        char start,
        char end) =>
        new(
            name,
            symbols
                .Where(sample =>
                    sample.Name.Length > 0 &&
                    sample.Name[0] >= start &&
                    sample.Name[0] <= end)
                .ToArray());
}
