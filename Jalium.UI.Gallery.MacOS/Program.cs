using AppKit;
using Foundation;
using Jalium.UI.Gallery.Modules.Main;
using Jalium.UI.Gallery.Services;
using Jalium.UI.Hosting;
using Jalium.UI.Interop;
using Jalium.UI.MacOS;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.MacOS;

internal static class Program
{
    private static void Main(string[] args)
    {
        JaliumMacApplication.Initialize();
        NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;
        using var appDelegate = new GalleryApplicationDelegate(args);
        NSApplication.SharedApplication.Delegate = appDelegate;
        NSApplication.Main(args);
    }
}

[Register("GalleryApplicationDelegate")]
internal sealed class GalleryApplicationDelegate(string[] args) : JaliumMacApplicationDelegate
{
    private EffectPerformanceProbe? _effectPerformanceProbe;
    protected override JaliumApp CreateHostedApp()
    {
        var renderContext = RenderContext.GetOrCreateCurrent(RenderBackend.Metal);
        renderContext.DefaultRenderingEngine = RenderingEngine.Impeller;

        var builder = AppBuilder.CreateBuilder(new AppBuilderSettings
        {
            Args = args,
            DisableDefaults = true,
        });
        builder.Services.AddAppServices();
        builder.Services.AddMainModule();

        var app = builder.Build();
        app.UseShared();
        app.UseIdleResourceReclamation();
        app.UseRenderingMode(RenderingMode.Performance);
        app.UsePathAntiAliasing(PathAntiAliasing.Msaa4x);
        return app;
    }

    public override void DidFinishLaunching(NSNotification notification)
    {
        var menu = new NSMenu();
        var applicationItem = new NSMenuItem();
        var applicationMenu = new NSMenu("Jalium Gallery");
        applicationMenu.AddItem(new NSMenuItem("Quit Jalium Gallery",
            new ObjCRuntime.Selector("terminate:"), "q"));
        applicationItem.Submenu = applicationMenu;
        menu.AddItem(applicationItem);

        var editItem = new NSMenuItem();
        var editMenu = new NSMenu("Edit");
        editMenu.AddItem(new NSMenuItem("Undo", new ObjCRuntime.Selector("undo:"), "z"));
        editMenu.AddItem(new NSMenuItem("Redo", new ObjCRuntime.Selector("redo:"), "Z"));
        editMenu.AddItem(NSMenuItem.SeparatorItem);
        editMenu.AddItem(new NSMenuItem("Cut", new ObjCRuntime.Selector("cut:"), "x"));
        editMenu.AddItem(new NSMenuItem("Copy", new ObjCRuntime.Selector("copy:"), "c"));
        editMenu.AddItem(new NSMenuItem("Paste", new ObjCRuntime.Selector("paste:"), "v"));
        editMenu.AddItem(new NSMenuItem("Select All", new ObjCRuntime.Selector("selectAll:"), "a"));
        editItem.Submenu = editMenu;
        menu.AddItem(editItem);
        NSApplication.SharedApplication.MainMenu = menu;

        base.DidFinishLaunching(notification);
        _effectPerformanceProbe = EffectPerformanceProbe.StartIfRequested();
        NSApplication.SharedApplication.Activate();
    }

    public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) => true;
}
