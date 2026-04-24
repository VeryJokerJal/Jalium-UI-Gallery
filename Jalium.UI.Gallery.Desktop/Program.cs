using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main;
using Jalium.UI.Gallery.Services;
using Jalium.UI.Interop;
using Jalium.UI.Markup;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Desktop;

/// <summary>
/// Desktop entry point. Boots Jalium.UI via <see cref="AppBuilder"/>, wires the
/// Impeller rendering backend, and delegates theme + MainWindow wiring to the
/// shared <c>UseShared</c> extension on the built <see cref="JaliumApp"/> so
/// the per-platform entry stays thin. The full Microsoft.Extensions.Hosting
/// surface (<c>builder.Services</c>, <c>builder.Configuration</c>,
/// <c>builder.Logging</c>, hosted services) remains available for extra
/// customization before <c>Build</c>.
/// </summary>
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // Prefer Impeller for smooth GPU-accelerated frames. Swap to RenderBackend.D3D
        // or .OpenGL if you need to pin the backend for debugging.
        var renderContext = RenderContext.GetOrCreateCurrent(RenderBackend.Auto);
        renderContext.DefaultRenderingEngine = RenderingEngine.Impeller;

        var builder = AppBuilder.CreateBuilder(args);

        // AddAppServices registers every concrete service implementation exposed by
        // the Services project. Additional Add{Transient,Scoped,Singleton} or
        // AddHostedService calls belong right here.
        builder.Services.AddAppServices();

        using var app = builder.Build();

        // Post-Build: UseShared binds the App Application subclass and activates
        // DevTools. See AppBuilderExtensions in the Main module to customize.
        app.UseShared();

        return app.Run();
    }
}
