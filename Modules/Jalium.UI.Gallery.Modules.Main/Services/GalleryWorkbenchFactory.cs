using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Hosting;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryWorkbenchFactory
{
    UIElement Create(GalleryComponentDescriptor descriptor);
}

/// <summary>
/// Composes the declarative workbench View, its ViewModel, and the runtime
/// component outside either View. This is the equivalent of resolving a WPF
/// Window/UserControl through the application's container.
/// </summary>
internal sealed class GalleryWorkbenchFactory : IGalleryWorkbenchFactory
{
    private readonly IViewFactory _viewFactory;

    public GalleryWorkbenchFactory(IViewFactory viewFactory)
    {
        _viewFactory = viewFactory;
    }

    public UIElement Create(GalleryComponentDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var component = _viewFactory.CreateView(descriptor.PageType) as UIElement
            ?? throw new InvalidOperationException(
                $"View '{descriptor.PageType.FullName}' could not be created.");
        var workbench =
            _viewFactory.CreateView<GalleryDemoWorkbench>()
            ?? throw new InvalidOperationException(
                "GalleryDemoWorkbench could not be created.");
        var viewModel =
            workbench.DataContext as GalleryDemoWorkbenchViewModel
            ?? throw new InvalidOperationException(
                "GalleryDemoWorkbench was resolved without its ViewModel.");

        var shaderExamples = (component as FrameworkElement)?.DataContext as ShaderEffectsViewModel;
        viewModel.Initialize(new GalleryWorkbenchDefinition(
            descriptor.PageTag,
            descriptor.Title,
            descriptor.CategoryLabel,
            descriptor.Description,
            shaderExamples?.XamlExample ?? descriptor.ExampleMarkup,
            shaderExamples?.CSharpExample ?? descriptor.ExampleCSharp));
        GalleryWorkbenchContentBehavior.SetDemoContent(
            workbench,
            component);
        return workbench;
    }
}
