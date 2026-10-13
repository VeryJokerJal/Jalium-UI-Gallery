namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryClipboardService
{
    bool TrySetText(string text, out string? error);
}
