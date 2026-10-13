using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jalium.UI.Hosting;
using Jalium.UI.Markup;

namespace Jalium.UI.Gallery.Modules.Main.Infrastructure;

/// <summary>
/// Bridges the type-forwarded hosting locator into the JALXAML simple-name
/// registry. The runtime resolves in-module behavior owners by CLR namespace,
/// while this forwarded type needs an explicit AOT-safe alias.
/// </summary>
internal static class GalleryXamlTypeRegistration
{
    [ModuleInitializer]
    [SuppressMessage(
        "Usage",
        "CA2255:The ModuleInitializer attribute should not be used in libraries")]
    internal static void Register() =>
        XamlTypeRegistry.RegisterType<
            GalleryViewModelLocatorOwner>("ViewModelLocator");
}

/// <summary>
/// Non-static registration token whose attached accessors delegate to the
/// framework's static ViewModelLocator.
/// </summary>
internal sealed class GalleryViewModelLocatorOwner
{
    private GalleryViewModelLocatorOwner()
    {
    }

    public static bool GetAutoWireViewModel(
        DependencyObject element) =>
        ViewModelLocator.GetAutoWireViewModel(element);

    public static void SetAutoWireViewModel(
        DependencyObject element,
        bool value) =>
        ViewModelLocator.SetAutoWireViewModel(element, value);
}
