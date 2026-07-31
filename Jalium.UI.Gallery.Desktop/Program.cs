using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main;
using Jalium.UI.Gallery.Services;
using Jalium.UI.Hosting;
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
        var renderContext = RenderContext.GetOrCreateCurrent(RenderBackend.D3D12);
        renderContext.DefaultRenderingEngine = RenderingEngine.Impeller;

        // DisableDefaults skips the Microsoft.Extensions.Hosting default config /
        // logging chain (appsettings.json, env-var, command-line, user-secrets,
        // Console/Debug/EventSource/EventLog logger providers, ConsoleLifetime).
        // Gallery is a GUI demo and uses none of them — opting out drops ~27
        // managed DLL loads from the startup cost without losing functionality.
        var builder = AppBuilder.CreateBuilder(new AppBuilderSettings
        {
            Args = args,
            DisableDefaults = true,
        });

        // AddAppServices registers every concrete service implementation exposed by
        // the Services project. Additional Add{Transient,Scoped,Singleton} or
        // AddHostedService calls belong right here.
        builder.Services.AddAppServices();

        using var app = builder.Build();

        // Post-Build: UseShared binds the App Application subclass and activates
        // DevTools. See AppBuilderExtensions in the Main module to customize.
        app.UseShared();
        app.UseIdleResourceReclamation();
        app.UseRenderingMode(RenderingMode.Performance);
        app.UsePathAntiAliasing(PathAntiAliasing.Msaa4x);

        return app.Run();
    }
}
