using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Services.Interfaces;

namespace Jalium.UI.Gallery.Modules.Main;

/// <summary>
/// Composition entry for the main module. The shell calls into here to build the
/// primary view model; additional factory methods can live alongside as the module
/// grows.
/// </summary>
public sealed class MainModule
{
    /// <summary>
    /// Creates the module's primary view model with its dependencies already resolved.
    /// </summary>
    /// <param name="messageService">Service providing the welcome-message payload.</param>
    public static ViewAViewModel CreateViewA(IMessageService messageService)
    {
        ArgumentNullException.ThrowIfNull(messageService);
        return new ViewAViewModel(messageService);
    }
}
