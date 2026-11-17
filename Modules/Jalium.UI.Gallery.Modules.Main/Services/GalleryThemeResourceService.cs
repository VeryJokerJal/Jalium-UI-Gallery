using System.Globalization;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryThemeResourceService
    : IGalleryThemeResourceService
{
    public string CurrentThemeKey =>
        ThemeManager.CurrentTheme.ToString();

    public IReadOnlyList<ThemeResourceEntry> GetResources(
        string previewThemeKey)
    {
        if (Jalium.UI.Application.Current?.Resources is not
            { } resources)
        {
            return [];
        }

        var entries = new List<ThemeResourceEntry>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        Collect(
            resources,
            seen,
            previewThemeKey,
            entries);
        entries.Sort((left, right) =>
            StringComparer.OrdinalIgnoreCase.Compare(
                left.Key,
                right.Key));
        return entries;
    }

    private static void Collect(
        ResourceDictionary dictionary,
        HashSet<string> seen,
        string previewThemeKey,
        ICollection<ThemeResourceEntry> entries)
    {
        if (TryGetThemedSlice(
                dictionary,
                previewThemeKey,
                out var themed) &&
            themed is not null)
        {
            Collect(
                themed,
                seen,
                previewThemeKey,
                entries);
        }

        foreach (var key in dictionary.Keys)
        {
            if (key is not string name ||
                !seen.Add(name) ||
                dictionary[key] is not { } value)
            {
                continue;
            }

            if (TryCreateEntry(name, value, out var entry))
            {
                entries.Add(entry);
            }
        }

        for (var index =
                dictionary.MergedDictionaries.Count - 1;
             index >= 0;
             index--)
        {
            Collect(
                dictionary.MergedDictionaries[index],
                seen,
                previewThemeKey,
                entries);
        }
    }

    private static bool TryGetThemedSlice(
        ResourceDictionary dictionary,
        string themeKey,
        out ResourceDictionary? themed)
    {
        if (dictionary.ThemeDictionaries.TryGetValue(
                themeKey,
                out themed))
        {
            return themed is not null;
        }

        foreach (var pair in dictionary.ThemeDictionaries)
        {
            if (pair.Key is string key &&
                string.Equals(
                    key,
                    themeKey,
                    StringComparison.OrdinalIgnoreCase))
            {
                themed = pair.Value;
                return themed is not null;
            }
        }

        themed = null;
        return false;
    }

    private static bool TryCreateEntry(
        string key,
        object value,
        out ThemeResourceEntry entry)
    {
        switch (value)
        {
            case Color color:
                entry = new ThemeResourceEntry(
                    key,
                    ThemeResourceKind.Color,
                    FormatColor(color),
                    new SolidColorBrush(color));
                return true;

            case SolidColorBrush solid:
                entry = new ThemeResourceEntry(
                    key,
                    ThemeResourceKind.SolidBrush,
                    FormatColor(solid.Color) +
                    (solid.Opacity < 1
                        ? $"  α={solid.Opacity:0.##}"
                        : string.Empty),
                    solid);
                return true;

            case GradientBrush gradient:
                entry = new ThemeResourceEntry(
                    key,
                    ThemeResourceKind.GradientBrush,
                    FormatGradient(gradient),
                    gradient);
                return true;

            default:
                entry = null!;
                return false;
        }
    }

    private static string FormatColor(Color color) =>
        string.Format(
            CultureInfo.InvariantCulture,
            "#{0:X2}{1:X2}{2:X2}{3:X2}",
            color.A,
            color.R,
            color.G,
            color.B);

    private static string FormatGradient(
        GradientBrush gradient)
    {
        if (gradient.GradientStops.Count == 0)
        {
            return "(empty gradient)";
        }

        var first = gradient.GradientStops[0].Color;
        var last = gradient.GradientStops[^1].Color;
        return gradient.GradientStops.Count == 2
            ? $"{FormatColor(first)} → {FormatColor(last)}"
            : $"{FormatColor(first)} → {FormatColor(last)}  " +
              $"({gradient.GradientStops.Count} stops)";
    }
}
