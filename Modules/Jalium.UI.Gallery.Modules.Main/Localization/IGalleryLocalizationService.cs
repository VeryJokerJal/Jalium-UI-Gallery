namespace Jalium.UI.Gallery.Modules.Main.Localization;

/// <summary>Application-facing language operations consumed by ViewModels.</summary>
public interface IGalleryLocalizationService
{
    event EventHandler? LanguageChanged;

    GalleryLanguage Current { get; }

    GalleryLanguageInfo CurrentInfo { get; }

    IReadOnlyList<GalleryLanguageInfo> Available { get; }

    void SetLanguage(GalleryLanguage language);

    /// <summary>Advances to the next shipped language, wrapping at the end.</summary>
    void SwitchToNext();

    string Get(string key);

    string Format(string key, params object?[] arguments);
}
