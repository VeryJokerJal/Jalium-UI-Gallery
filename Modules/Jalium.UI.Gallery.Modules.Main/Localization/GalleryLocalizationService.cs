namespace Jalium.UI.Gallery.Modules.Main.Localization;

/// <summary>
/// Applies a language change and persists the choice as one operation.
/// ViewModels depend on this instead of the static
/// <see cref="LocalizationManager"/> so they stay testable.
/// </summary>
internal sealed class GalleryLocalizationService : IGalleryLocalizationService
{
    public GalleryLocalizationService()
    {
        LocalizationManager.LanguageChanged += OnLanguageChanged;
    }

    public event EventHandler? LanguageChanged;

    public GalleryLanguage Current => LocalizationManager.CurrentLanguage;

    public GalleryLanguageInfo CurrentInfo => LocalizationManager.CurrentLanguageInfo;

    public IReadOnlyList<GalleryLanguageInfo> Available => GalleryLanguages.All;

    public void SetLanguage(GalleryLanguage language)
    {
        if (LocalizationManager.CurrentLanguage == language)
        {
            return;
        }

        LocalizationManager.SetLanguage(language);
        LanguageSettingsStore.Save(language);
    }

    public void SwitchToNext() =>
        SetLanguage(GalleryLanguages.Next(LocalizationManager.CurrentLanguage));

    public string Get(string key) => LocalizationManager.Get(key);

    public string Format(string key, params object?[] arguments) =>
        LocalizationManager.Format(key, arguments);

    private void OnLanguageChanged(object? sender, EventArgs args) =>
        LanguageChanged?.Invoke(this, EventArgs.Empty);
}
