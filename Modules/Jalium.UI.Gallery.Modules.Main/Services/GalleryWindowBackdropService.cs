using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryWindowBackdropService
    : IGalleryWindowBackdropService
{
    public bool TrySetBackdrop(WindowBackdropMode mode)
    {
        var window = Jalium.UI.Application.Current?.MainWindow;
        if (window is null)
        {
            return false;
        }

        window.SystemBackdrop = mode switch
        {
            WindowBackdropMode.Mica => WindowBackdropType.Mica,
            WindowBackdropMode.MicaAlt => WindowBackdropType.MicaAlt,
            WindowBackdropMode.Acrylic => WindowBackdropType.Acrylic,
            _ => WindowBackdropType.None
        };

        return true;
    }
}
