using Jalium.UI;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>Applies Gallery and framework theme state as one atomic operation.</summary>
internal sealed class GalleryThemeService : IGalleryThemeService
{
    public event EventHandler? ThemeChanged;

    public bool IsDark =>
        GalleryTheme.CurrentMode == GalleryThemeMode.Dark;

    public void Toggle() => SetDark(!IsDark);

    public void SetAccent(Color accent)
    {
        GalleryThemeTuner.ApplyAccent(accent);
        ThemeManager.ApplyAccent(accent);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSurface(Color surface)
    {
        GalleryThemeTuner.ApplySurface(surface);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ResetTuning()
    {
        GalleryThemeTuner.Reset();
        ThemeManager.ApplyAccent(GalleryTheme.AccentPrimary);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetDark(bool isDark)
    {
        var nextFrameworkTheme = isDark
            ? ThemeVariant.Dark
            : ThemeVariant.Light;
        var nextGalleryTheme = isDark
            ? GalleryThemeMode.Dark
            : GalleryThemeMode.Light;
#pragma warning disable WPF0001
        var nextApplicationTheme = isDark
            ? ThemeMode.Dark
            : ThemeMode.Light;
        var applicationModeMatches =
            Application.Current == null ||
            Application.Current.ThemeMode == nextApplicationTheme;
#pragma warning restore WPF0001

        if (ThemeManager.CurrentTheme == nextFrameworkTheme &&
            GalleryTheme.CurrentMode == nextGalleryTheme &&
            applicationModeMatches)
        {
            return;
        }

#pragma warning disable WPF0001
        if (Application.Current is { } application)
        {
            // ThemeMode pins the user's choice against later platform preference
            // callbacks. ApplyTheme below also repairs a pre-existing split state
            // when ThemeMode already has the requested value.
            application.ThemeMode = nextApplicationTheme;
        }
#pragma warning restore WPF0001
        ThemeManager.ApplyTheme(nextFrameworkTheme);
        GalleryTheme.CurrentMode = nextGalleryTheme;
        GalleryThemeTuner.RefreshForCurrentMode();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }
}
