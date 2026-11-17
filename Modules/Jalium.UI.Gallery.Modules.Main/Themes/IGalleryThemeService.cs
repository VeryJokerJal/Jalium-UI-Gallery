using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>Application-facing theme operations consumed by ViewModels.</summary>
public interface IGalleryThemeService
{
    event EventHandler? ThemeChanged;

    bool IsDark { get; }

    void Toggle();

    void SetDark(bool isDark);

    void SetAccent(Color accent);

    void SetSurface(Color surface);

    void ResetTuning();
}
