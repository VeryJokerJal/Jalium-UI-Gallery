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
/// automatic rendering backend, and delegates theme + MainWindow wiring to the
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
        var renderContext = RenderContext.GetOrCreateCurrent(RenderBackend.Auto);
        renderContext.DefaultRenderingEngine = RenderingEngine.Vello;
        // Match the catalog's bounded image hot set at the native-texture
        // layer too. Current-frame images are protected by the renderer.
        renderContext.BitmapCacheCapacity = 32;
        renderContext.BitmapCacheByteBudget = 16L * 1024 * 1024;

        var builder = AppBuilder.CreateBuilder(new AppBuilderSettings
        {
            Args = args,
            DisableDefaults = true,
        });

        // AddHostedService calls belong right here.
        builder.Services.AddAppServices();
        builder.Services.AddMainModule();

        using var app = builder.Build();

        app.UseShared();
        app.UseIdleResourceReclamation();
        app.UseRenderingMode(RenderingMode.Performance);
        app.UsePathAntiAliasing(PathAntiAliasing.Msaa4x);

        return app.Run();
    }
}
