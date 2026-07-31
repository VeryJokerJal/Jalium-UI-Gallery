using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main;
using Jalium.UI.Gallery.Services;
using Jalium.UI.Hosting;
using Jalium.UI.Interop;
using Jalium.UI.Markup;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Linux;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        bool diagnostics = LinuxRuntimeDiagnostics.IsEnabled(args);
        if (diagnostics)
        {
            LinuxRuntimeDiagnostics.WritePreflight(Console.Out);
            if (args.Contains("--diagnostics-only", StringComparer.OrdinalIgnoreCase))
                return LinuxRuntimeDiagnostics.HasCompleteNativePayload ? 0 : 2;
        }

        try
        {
            // Auto selects Vulkan on Linux when it is available and preserves the
            // software fallback for machines without a usable Vulkan device/driver.
            var renderContext = RenderContext.GetOrCreateCurrent(RenderBackend.Auto);
            renderContext.DefaultRenderingEngine = RenderingEngine.Impeller;
            if (diagnostics)
                LinuxRuntimeDiagnostics.WriteRenderContext(Console.Out, renderContext);

            var builder = AppBuilder.CreateBuilder(new AppBuilderSettings
            {
                Args = args,
                DisableDefaults = true,
            });

            builder.Services.AddAppServices();

            using var app = builder.Build();
            app.UseShared();
            app.UseIdleResourceReclamation();
            app.UseRenderingMode(RenderingMode.Performance);
            app.UsePathAntiAliasing(PathAntiAliasing.Msaa4x);

            return app.Run();
        }
        catch (Exception exception) when (diagnostics)
        {
            Console.Error.WriteLine($"Jalium.UI Gallery Linux startup failed: {exception}");
            return 1;
        }
    }
}
