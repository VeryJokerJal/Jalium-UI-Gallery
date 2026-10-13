using System.Globalization;

namespace Jalium.UI.Gallery.Modules.Main.Localization;

/// <summary>
/// Display languages shipped with the Gallery. Each member has exactly one
/// <c>Strings.*.jalxaml</c> resource dictionary behind it.
/// </summary>
public enum GalleryLanguage
{
    ChineseSimplified,
    English,
    Japanese,
    Korean
}

/// <summary>
/// Static metadata for one shipped language: the culture it maps to and the
/// endonym shown in the language switcher (a picker that spells every option in
/// its own script needs no translation of its own), plus a one-glyph label for
/// the mobile header where a full endonym does not fit.
/// </summary>
public sealed record GalleryLanguageInfo(
    GalleryLanguage Language,
    string CultureName,
    string NativeName,
    string ShortLabel,
    string EnglishName);

/// <summary>
/// The shipped language table plus the lookups the shell needs: cycle to the
/// next language, resolve a persisted culture name, and pick a sensible default
/// from the operating system's UI culture.
/// </summary>
public static class GalleryLanguages
{
    public static IReadOnlyList<GalleryLanguageInfo> All { get; } =
    [
        new(GalleryLanguage.ChineseSimplified, "zh-CN", "简体中文", "中", "Chinese (Simplified)"),
        new(GalleryLanguage.English, "en-US", "English", "EN", "English"),
        new(GalleryLanguage.Japanese, "ja-JP", "日本語", "日", "Japanese"),
        new(GalleryLanguage.Korean, "ko-KR", "한국어", "한", "Korean")
    ];

    /// <summary>Language used when nothing else resolves.</summary>
    public const GalleryLanguage Fallback = GalleryLanguage.English;

    public static GalleryLanguageInfo Get(GalleryLanguage language) =>
        All.FirstOrDefault(info => info.Language == language)
        ?? All.First(info => info.Language == Fallback);

    public static CultureInfo GetCulture(GalleryLanguage language) =>
        CultureInfo.GetCultureInfo(Get(language).CultureName);

    /// <summary>Next language in <see cref="All"/> order, wrapping at the end.</summary>
    public static GalleryLanguage Next(GalleryLanguage language)
    {
        var index = 0;
        for (var i = 0; i < All.Count; i++)
        {
            if (All[i].Language == language)
            {
                index = i;
                break;
            }
        }

        return All[(index + 1) % All.Count].Language;
    }

    /// <summary>
    /// Resolves a culture name ("zh", "zh-CN", "zh-Hans-CN", …) to a shipped
    /// language. Matching is done on the two-letter language part so any
    /// regional variant lands on the right dictionary.
    /// </summary>
    public static bool TryParse(string? cultureName, out GalleryLanguage language)
    {
        language = Fallback;
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return false;
        }

        var candidate = cultureName.Trim();
        foreach (var info in All)
        {
            if (string.Equals(info.CultureName, candidate, StringComparison.OrdinalIgnoreCase))
            {
                language = info.Language;
                return true;
            }
        }

        var separator = candidate.IndexOfAny(['-', '_']);
        var prefix = separator > 0 ? candidate[..separator] : candidate;
        foreach (var info in All)
        {
            if (info.CultureName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                language = info.Language;
                return true;
            }
        }

        return false;
    }

    /// <summary>Best shipped language for the current operating system UI culture.</summary>
    public static GalleryLanguage FromCurrentCulture()
    {
        return TryParse(CultureInfo.CurrentUICulture.Name, out var language)
            ? language
            : Fallback;
    }
}
