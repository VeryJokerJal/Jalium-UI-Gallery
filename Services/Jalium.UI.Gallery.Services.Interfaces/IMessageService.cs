namespace Jalium.UI.Gallery.Services.Interfaces;

/// <summary>
/// Supplies a short human-readable message for the sample view. Implementations can
/// return anything — a static string, a tip of the day, a snippet fetched from a
/// backend, etc. Add additional members here as the module grows.
/// </summary>
public interface IMessageService
{
    /// <summary>Returns the greeting shown on the main view.</summary>
    string GetMessage();
}
