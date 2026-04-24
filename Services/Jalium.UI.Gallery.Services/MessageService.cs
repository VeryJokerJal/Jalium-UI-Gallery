using Jalium.UI.Gallery.Services.Interfaces;

namespace Jalium.UI.Gallery.Services;

/// <summary>
/// Default in-memory <see cref="IMessageService"/>. Returns a greeting stamped with
/// the current local time so every refresh is observably different — handy as a smoke
/// test for commands and bindings.
/// </summary>
public class MessageService : IMessageService
{
    /// <inheritdoc/>
    public string GetMessage() => $"Jalium.UI Gallery · {DateTime.Now:HH:mm:ss}";
}
