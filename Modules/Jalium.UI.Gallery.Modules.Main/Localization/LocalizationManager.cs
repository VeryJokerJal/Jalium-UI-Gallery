using System.Globalization;
using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Localization;

/// <summary>
/// WPF-style string localization. Each language is a jalxaml
/// <c>ResourceDictionary</c> of <c>&lt;x:String x:Key="Loc.…"&gt;</c> entries; one
/// merged dictionary is re-filled from the selected language, so markup reads
/// strings the same way it reads theme brushes:
/// <c>Text="{DynamicResource Loc.Home.Title}"</c>.
///
/// Re-filling rather than swapping the merged dictionary is deliberate: a keyed
/// write becomes a targeted DynamicResource refresh of just the changed keys,
/// while replacing a merged dictionary forces a whole-tree implicit-style
/// re-evaluation. Callers that cannot bind use <see cref="Get"/> /
/// <see cref="Format"/> and re-read on <see cref="LanguageChanged"/>.
/// </summary>
public static class LocalizationManager
{
    private const string CategoryKeyPrefix = "Loc.Category.";
    private const string DescriptionKeyPrefix = "Loc.Catalog.Desc.";
    private const string DefaultDescriptionKey = "Loc.Catalog.DefaultDescriptionFormat";

    private static readonly Dictionary<GalleryLanguage, ResourceDictionary> LoadedDictionaries = new();

    /// <summary>
    /// The live table merged into <c>Application.Resources</c>. Keys are never
    /// removed — a switch overwrites in place — so a DynamicResource can never
    /// transiently resolve to nothing while a language is applied.
    /// </summary>
    private static readonly ResourceDictionary ActiveStrings = new();

    private static ResourceDictionary? _fallbackStrings;
    private static GalleryLanguage _currentLanguage = GalleryLanguages.Fallback;
    private static bool _initialized;

    /// <summary>Raised after the active language has been applied.</summary>
    public static event EventHandler? LanguageChanged;

    public static GalleryLanguage CurrentLanguage => _currentLanguage;

    public static GalleryLanguageInfo CurrentLanguageInfo =>
        GalleryLanguages.Get(_currentLanguage);

    /// <summary>
    /// Seeds the string table and merges it into the application resources.
    /// Call once, before the first window is created, so the very first frame
    /// already renders in the right language.
    /// </summary>
    public static void Initialize(
        ResourceDictionary applicationResources,
        GalleryLanguage language)
    {
        ArgumentNullException.ThrowIfNull(applicationResources);

        EnsureFallback();
        _currentLanguage = language;
        Populate(language);
        ApplyCulture(language);

        if (!applicationResources.MergedDictionaries.Contains(ActiveStrings))
        {
            applicationResources.MergedDictionaries.Add(ActiveStrings);
        }

        _initialized = true;
    }

    /// <summary>
    /// Applies <paramref name="language"/>. Markup bound with
    /// <c>{DynamicResource Loc.*}</c> updates itself; everything else should
    /// listen to <see cref="LanguageChanged"/>.
    /// </summary>
    public static void SetLanguage(GalleryLanguage language)
    {
        if (_initialized && _currentLanguage == language)
        {
            return;
        }

        _currentLanguage = language;
        Populate(language);
        ApplyCulture(language);
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>Advances to the next shipped language, wrapping at the end.</summary>
    public static void SwitchToNext() =>
        SetLanguage(GalleryLanguages.Next(_currentLanguage));

    /// <summary>
    /// Missing keys fall back to English and finally to the key itself, so a
    /// half-translated dictionary degrades to readable text, not blank UI.
    /// </summary>
    public static string Get(string key)
    {
        return TryGet(key, out var value) ? value : key;
    }

    public static bool TryGet(string key, out string value)
    {
        if (!string.IsNullOrEmpty(key))
        {
            if (ActiveStrings.TryGetValue(key, out var active) && active is string activeText)
            {
                value = activeText;
                return true;
            }

            var fallbackStrings = EnsureFallback();
            if (fallbackStrings is not null &&
                fallbackStrings.TryGetValue(key, out var fallback) &&
                fallback is string fallbackText)
            {
                value = fallbackText;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }

    /// <summary>Looks up a composite-format string and fills in its arguments.</summary>
    public static string Format(string key, params object?[] arguments)
    {
        var format = Get(key);
        if (arguments.Length == 0)
        {
            return format;
        }

        try
        {
            return string.Format(
                GalleryLanguages.GetCulture(_currentLanguage),
                format,
                arguments);
        }
        catch (FormatException)
        {
            // A translation that dropped or mangled its placeholders must not
            // take the UI down — show the unformatted text instead.
            return format;
        }
    }

    /// <summary>Display name for an invariant catalog category such as "Controls".</summary>
    public static string GetCategory(string category)
    {
        return string.IsNullOrEmpty(category)
            ? string.Empty
            : Get(CategoryKeyPrefix + category);
    }

    /// <summary>
    /// Resource key holding the bespoke description for a catalog page, or
    /// <c>null</c> when the page uses the generated default sentence.
    /// </summary>
    public static string? FindDescriptionKey(string pageTag)
    {
        if (string.IsNullOrEmpty(pageTag))
        {
            return null;
        }

        var key = DescriptionKeyPrefix + pageTag;
        return TryGet(key, out _) ? key : null;
    }

    /// <summary>
    /// Description for a catalog entry: the bespoke sentence when the page has
    /// one, otherwise the localized default built from the component title.
    /// </summary>
    public static string GetDescription(string? descriptionKey, string title)
    {
        return descriptionKey is not null && TryGet(descriptionKey, out var text)
            ? text
            : Format(DefaultDescriptionKey, title);
    }

    private static void Populate(GalleryLanguage language)
    {
        var source = Load(language);
        var fallbackStrings = EnsureFallback();

        using (ActiveStrings.DeferNotifications())
        {
            // Seed with English first so a key the translation has not covered
            // yet still resolves; the language's own entries then win.
            if (fallbackStrings is not null && !ReferenceEquals(fallbackStrings, source))
            {
                CopyEntries(fallbackStrings, ActiveStrings);
            }

            CopyEntries(source, ActiveStrings);
        }
    }

    private static void CopyEntries(ResourceDictionary source, ResourceDictionary target)
    {
        foreach (var key in source.Keys)
        {
            if (key is null || !source.TryGetValue(key, out var value) || value is not string text)
            {
                continue;
            }

            text = GalleryShortcutText.Expand(text);

            if (target.TryGetValue(key, out var existing) &&
                existing is string existingText &&
                string.Equals(existingText, text, StringComparison.Ordinal))
            {
                // Unchanged between languages (brand names, shortcut hints, …).
                // Skipping the write keeps it out of the refresh key set.
                continue;
            }

            target[key] = text;
        }
    }

    /// <summary>
    /// Loads the English table on first use. Catalog metadata is read before
    /// <see cref="Initialize"/> runs (the DI module builds it) and unit fixtures
    /// touch it with no Application at all, so lookups must still resolve — and
    /// must never take a startup path down if the dictionary cannot be built.
    /// </summary>
    private static ResourceDictionary? EnsureFallback()
    {
        if (_fallbackStrings is not null)
        {
            return _fallbackStrings;
        }

        try
        {
            _fallbackStrings = Load(GalleryLanguages.Fallback);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Gallery fallback string table could not be loaded: {exception.Message}");
        }

        return _fallbackStrings;
    }

    /// <summary>
    /// Builds one flat table per language out of its jalxaml parts (shell copy
    /// and page copy live in separate files so neither grows unmanageable).
    /// Flattening matters: <see cref="CopyEntries"/> walks direct entries only.
    /// </summary>
    private static ResourceDictionary Load(GalleryLanguage language)
    {
        if (LoadedDictionaries.TryGetValue(language, out var cached))
        {
            return cached;
        }

        ResourceDictionary[] parts = language switch
        {
            GalleryLanguage.ChineseSimplified => [new StringsZhCN(), new PagesZhCN()],
            GalleryLanguage.Japanese => [new StringsJaJP(), new PagesJaJP()],
            GalleryLanguage.Korean => [new StringsKoKR(), new PagesKoKR()],
            _ => [new StringsEnUS(), new PagesEnUS()]
        };

        var dictionary = new ResourceDictionary();
        foreach (var part in parts)
        {
            CopyEntries(part, dictionary);
        }

        GalleryShortcutText.AddResources(dictionary);

        LoadedDictionaries[language] = dictionary;
        return dictionary;
    }

    private static void ApplyCulture(GalleryLanguage language)
    {
        var culture = GalleryLanguages.GetCulture(language);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
}
