namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryNavigationCoordinator
    : IGalleryNavigationCoordinator
{
    public event EventHandler<GalleryNavigationRequestedEventArgs>? NavigationRequested;

    public void RequestNavigation(string pageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);
        NavigationRequested?.Invoke(
            this,
            new GalleryNavigationRequestedEventArgs(pageTag));
    }
}
