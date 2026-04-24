using Android.App;
using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main;
using Jalium.UI.Gallery.Services;
using Jalium.UI.Interop;

namespace Jalium.UI.Gallery.Android;

/// <summary>
/// Android launcher activity. <see cref="JaliumActivity"/> handles surface/lifecycle
/// wiring — we just build and return the Jalium <see cref="JaliumApp"/> via
/// <see cref="AppBuilder"/>. Theme + MainWindow wiring is delegated to the shared
/// <c>UseShared</c> extension on the built <see cref="JaliumApp"/> so this file
/// stays tiny.
/// </summary>
[Activity(
    Label = "Jalium.UI.Gallery.Android",
    MainLauncher = true,
    Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
    ConfigurationChanges = global::Android.Content.PM.ConfigChanges.Orientation
        | global::Android.Content.PM.ConfigChanges.ScreenSize
        | global::Android.Content.PM.ConfigChanges.KeyboardHidden)]
public class MainActivity : JaliumActivity
{
    protected override JaliumApp CreateHostedApp()
    {
        // Prefer Impeller for smooth GPU-accelerated frames on Android.
        var renderContext = RenderContext.GetOrCreateCurrent(RenderBackend.Auto);
        renderContext.DefaultRenderingEngine = RenderingEngine.Impeller;

        var builder = AppBuilder.CreateBuilder();

        // AddAppServices registers every concrete service implementation exposed by
        // the Services project. Additional Add{Transient,Scoped,Singleton} or
        // AddHostedService calls belong right here.
        builder.Services.AddAppServices();

        var app = builder.Build();

        // Post-Build: UseShared binds the App Application subclass and activates
        // DevTools. See AppBuilderExtensions in the Main module to customize.
        app.UseShared();

        return app;
    }
}
