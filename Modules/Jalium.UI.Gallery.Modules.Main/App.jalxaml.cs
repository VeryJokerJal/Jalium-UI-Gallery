using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main;

/// <summary>
/// Application subclass for the Gallery main module. The Resources dictionary
/// declared in <c>Application.jalxaml</c> pulls in <c>Themes/AppTheme.jalxaml</c>
/// plus the static accent overrides that reskin the framework control templates.
///
/// Dynamic palette work — the live dark/light toggle driven by
/// <see cref="GalleryTheme"/> — stays in code-behind because it mutates brushes
/// after XAML load and emits a <see cref="GalleryTheme.ModeChanged"/> event.
/// Per-platform entry points (Desktop / Android) register this type on the
/// built <see cref="JaliumApp"/> via <c>app.UseApplication&lt;App&gt;()</c>;
/// see <c>AppBuilderExtensions.UseShared</c>.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Seed the gallery-scoped palette so every {DynamicResource Gallery...}
        // reference in the pages resolves before the first window renders. The
        // GalleryTheme static brushes also flip with the live mode toggle.
        GalleryTheme.RegisterResources(Resources);
    }
}
