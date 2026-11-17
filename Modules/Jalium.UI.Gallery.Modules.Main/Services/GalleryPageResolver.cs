using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Gallery.Modules.Main.Support;
using Jalium.UI.Hosting;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryPageResolver : IGalleryPageResolver
{
    private readonly IViewFactory _viewFactory;
    private readonly IGalleryWorkbenchFactory _workbenchFactory;

    public GalleryPageResolver(
        IViewFactory viewFactory,
        IGalleryWorkbenchFactory workbenchFactory)
    {
        _viewFactory = viewFactory;
        _workbenchFactory = workbenchFactory;
    }

    public UIElement Resolve(string pageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);

        if (GalleryFeatureAvailability.TryGetUnavailableReason(
                pageTag,
                out var feature,
                out var reason,
                out var guidance))
        {
            return CreatePlatformUnavailable(feature, reason, guidance);
        }

        try
        {
            if (pageTag == "home")
            {
                return RequireView<HomePage>();
            }

            if (pageTag == "design-tokens")
            {
                return RequireView<DesignTokensPage>();
            }

            if (pageTag == "theme-studio")
            {
                return RequireView<ThemeStudioPage>();
            }

            if (pageTag == "overview")
            {
                return RequireView<OverviewPage>();
            }

            if (GalleryComponentCatalog.PagesByTag.TryGetValue(
                    pageTag,
                    out var descriptor))
            {
                return _workbenchFactory.Create(descriptor);
            }

            return CreateNavigationState(pageTag, exception: null);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Gallery] Failed to create page '{pageTag}': {exception}");
            return CreateNavigationState(pageTag, exception);
        }
    }

    private TView RequireView<TView>()
        where TView : FrameworkElement
    {
        return _viewFactory.CreateView<TView>()
            ?? throw new InvalidOperationException(
                $"View '{typeof(TView).FullName}' could not be resolved.");
    }

    private UIElement CreateNavigationState(
        string pageTag,
        Exception? exception)
    {
        var view = RequireView<NavigationStatePage>();
        var viewModel = (NavigationStateViewModel?)view.DataContext
            ?? throw new InvalidOperationException(
                "NavigationStatePage was resolved without its ViewModel.");

        if (exception == null)
        {
            viewModel.ShowMissing(pageTag);
        }
        else
        {
            viewModel.ShowError(pageTag, exception);
        }

        return view;
    }

    private UIElement CreatePlatformUnavailable(
        string feature,
        string reason,
        string guidance)
    {
        var view = RequireView<PlatformUnavailablePage>();
        var viewModel = (PlatformUnavailableViewModel?)view.DataContext
            ?? throw new InvalidOperationException(
                "PlatformUnavailablePage was resolved without its ViewModel.");
        viewModel.Show(feature, reason, guidance);
        return view;
    }
}
