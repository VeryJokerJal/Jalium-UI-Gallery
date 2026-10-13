using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryPageResolver
{
    UIElement Resolve(string pageTag);
}
