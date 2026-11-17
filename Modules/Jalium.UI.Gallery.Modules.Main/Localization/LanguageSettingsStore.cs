using System.Diagnostics;

namespace Jalium.UI.Gallery.Modules.Main.Localization;

/// <summary>
/// Remembers the language the user picked, so the choice survives a restart.
/// The store is a single culture name in a text file under the per-user
/// application-data folder; every failure mode (sandboxed storage, read-only
/// profile, corrupt file) degrades to "no preference" rather than throwing on a
/// startup path.
/// </summary>
public static class LanguageSettingsStore
{
    private const string FolderName = "JaliumUIGallery";
    private const string FileName = "language.txt";

    /// <summary>
    /// The persisted language, or the operating-system UI language when nothing
    /// has been saved yet.
    /// </summary>
    public static GalleryLanguage Load()
    {
        try
        {
            var path = GetFilePath();
            if (path is not null && File.Exists(path))
            {
                var saved = File.ReadAllText(path).Trim();
                if (GalleryLanguages.TryParse(saved, out var language))
                {
                    return language;
                }
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Gallery language preference could not be read: {exception.Message}");
        }

        return GalleryLanguages.FromCurrentCulture();
    }

    public static void Save(GalleryLanguage language)
    {
        try
        {
            var path = GetFilePath();
            if (path is null)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, GalleryLanguages.Get(language).CultureName);
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Gallery language preference could not be saved: {exception.Message}");
        }
    }

    private static string? GetFilePath()
    {
        var root = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create);

        return string.IsNullOrEmpty(root)
            ? null
            : Path.Combine(root, FolderName, FileName);
    }
}
