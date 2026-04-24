using Microsoft.Extensions.DependencyInjection;
using Jalium.UI.Gallery.Services.Interfaces;

namespace Jalium.UI.Gallery.Services;

/// <summary>
/// Composition root for the Services layer. Keeps DI registration in one place so
/// per-platform entry points (Desktop / Android) just call
/// <c>builder.Services.AddAppServices()</c> without having to know which concrete
/// types live here.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers every concrete service implementation shipped by this assembly.
    /// Add new <c>AddSingleton</c> / <c>AddScoped</c> / <c>AddTransient</c> lines
    /// here as the service layer grows.
    /// </summary>
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IMessageService, MessageService>();
        return services;
    }
}
