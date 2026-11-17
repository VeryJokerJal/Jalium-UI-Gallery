using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryClipboardService
    : IGalleryClipboardService
{
    public bool TrySetText(string text, out string? error)
    {
        try
        {
            Clipboard.SetText(text);
            error = null;
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
    }
}
