using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Controls;
using Jalium.UI.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Jalium.UI.Gallery.Modules.Main;

/// <summary>
/// Composition entry for the main module. Every platform host calls
/// <see cref="AddMainModule"/> before building the application.
/// </summary>
public static class MainModule
{
    /// <summary>
    /// Cards decoded before the first catalog frame is allowed to render. Three
    /// columns fill ~15 cards in a typical viewport and the virtualization cache
    /// band realizes about a row beyond that.
    /// </summary>
    private const int FirstViewportPrefetchCount = 24;

    /// <summary>
    /// Registers the module's Views and their convention-named ViewModels. There
    /// is deliberately no shared/fallback DataContext: an unpaired View is an
    /// architecture error caught by the module tests.
    /// </summary>
    public static IServiceCollection AddMainModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // The first viewport blocks the initial navigation
        // (GalleryNavigationHost waits on it), so it stays small; three columns
        // plus the cache band need more than a dozen cards, hence 24.
        var pageTags = HomeCatalog.Items.Select(item => item.PageTag).ToArray();
        SaasCardBackgrounds.BeginStartupPrefetch(pageTags.Take(FirstViewportPrefetchCount));

        // The ListBox's half-page virtualization cache realizes nearby cards
        // ahead of scrolling and starts their asynchronous image loads. Keep
        // the rest demand-loaded instead of retaining all catalog bitmaps.

        var assembly = typeof(MainModule).Assembly;
        services.AddViewsAndViewModels(assembly);

        services.AddView<MainWindow, MainWindowViewModel>();
        services.AddView<HomePage, HomeViewModel>();
        services.AddView<DesignTokensPage, DesignTokensViewModel>();
        services.AddView<GettingStartedPage, GettingStartedViewModel>();
        services.AddView<PlatformUnavailablePage, PlatformUnavailableViewModel>();
        services.AddView<ThemeStudioPage, ThemeStudioViewModel>();
        GetOrCreateRegistry(services).Register(
            typeof(GalleryDemoWorkbench),
            typeof(GalleryDemoWorkbenchViewModel));

        services.TryAddSingleton<IGalleryThemeService, GalleryThemeService>();
        services.TryAddSingleton<
            IGalleryLocalizationService,
            GalleryLocalizationService>();
        services.TryAddSingleton<IGalleryNotifyIconService, GalleryNotifyIconService>();
        services.TryAddSingleton<IGalleryPrintingService, GalleryPrintingService>();
        services.TryAddSingleton<
            IGalleryMediaFilePicker,
            GalleryMediaFilePicker>();
        services.TryAddTransient<
            IGalleryContentDialogService,
            GalleryContentDialogService>();
        services.TryAddTransient<
            IGallerySystemDialogService,
            GallerySystemDialogService>();
        services.TryAddSingleton<
            IGallerySystemNotificationService,
            GallerySystemNotificationService>();
        services.TryAddTransient<IGalleryNavigationService, GalleryNavigationService>();
        services.TryAddSingleton<
            IGalleryNavigationCoordinator,
            GalleryNavigationCoordinator>();
        services.TryAddSingleton<IExternalUriService, ExternalUriService>();
        services.TryAddSingleton<
            IGalleryWindowBackdropService,
            GalleryWindowBackdropService>();
        services.TryAddSingleton<
            IGalleryShellIntegrationService,
            GalleryShellIntegrationService>();
        services.TryAddSingleton<
            IGalleryImageSourceService,
            GalleryImageSourceService>();
        services.TryAddSingleton<
            IGalleryDragFileService,
            GalleryDragFileService>();
        services.TryAddSingleton<
            IGalleryThemeResourceService,
            GalleryThemeResourceService>();
        services.TryAddSingleton<
            IGalleryClipboardService,
            GalleryClipboardService>();
        services.TryAddTransient<IGalleryPageResolver, GalleryPageResolver>();
        services.RemoveAll<HomeViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.TryAddTransient<GalleryDemoWorkbenchViewModel>();
        services.TryAddSingleton<IGalleryWorkbenchFactory, GalleryWorkbenchFactory>();

        return services;
    }

    private static ViewRegistry GetOrCreateRegistry(IServiceCollection services)
    {
        var existing = services
            .LastOrDefault(descriptor => descriptor.ServiceType == typeof(ViewRegistry))
            ?.ImplementationInstance as ViewRegistry;
        if (existing != null)
        {
            return existing;
        }

        var registry = new ViewRegistry();
        services.RemoveAll<ViewRegistry>();
        services.AddSingleton(registry);
        return registry;
    }

    internal static IReadOnlyList<Type> GetConcreteViewTypes(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false } &&
                typeof(FrameworkElement).IsAssignableFrom(type) &&
                type.Namespace?.StartsWith(
                    "Jalium.UI.Gallery.Modules.Main.Views",
                    StringComparison.Ordinal) == true)
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
    }

}
