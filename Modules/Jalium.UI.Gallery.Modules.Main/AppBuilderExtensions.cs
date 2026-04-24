using Jalium.UI;
using Jalium.UI.Hosting;

namespace Jalium.UI.Gallery.Modules.Main;

/// <summary>
/// Shared setup for the Gallery Main module. Runs on the built
/// <see cref="JaliumApp"/> because every <c>Use*</c> — including
/// <c>UseApplication</c> — lives on the app (ASP.NET Core-style): type
/// registration and feature activation happen in the same post-Build phase so
/// the per-platform entry points stay thin.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    /// Runs the shared application setup on the built <see cref="JaliumApp"/>:
    /// <list type="number">
    ///   <item>Binds the strongly-typed <see cref="App"/> Application subclass
    ///     (whose <c>Application.jalxaml</c> declares
    ///     <c>StartupUri="Views/MainWindow.jalxaml"</c>).</item>
    ///   <item>Opts the module into the Jalium DevTools window (F12 inspector,
    ///     Ctrl+Shift+C picker).</item>
    /// </list>
    /// </summary>
    public static JaliumApp UseShared(this JaliumApp app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Register the strongly-typed Application subclass. The Resources tree
        // declared in Application.jalxaml (AppTheme merge + accent overrides) is
        // applied automatically when the framework instantiates App, and the
        // StartupUri attribute instructs the framework to load MainWindow.jalxaml
        // as the application's MainWindow via its parameterless constructor.
        app.UseApplication<App>();

        app.UseDevTools();

        return app;
    }
}
