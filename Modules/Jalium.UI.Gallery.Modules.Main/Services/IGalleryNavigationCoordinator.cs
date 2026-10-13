namespace Jalium.UI.Gallery.Modules.Main.Services;

public sealed class GalleryNavigationRequestedEventArgs : EventArgs
{
    public GalleryNavigationRequestedEventArgs(string pageTag)
    {
        PageTag = pageTag;
    }

    public string PageTag { get; }
}

/// <summary>
/// ViewModel-to-ViewModel navigation channel used by catalog commands and the
/// application shell.
/// </summary>
public interface IGalleryNavigationCoordinator
{
    event EventHandler<GalleryNavigationRequestedEventArgs>? NavigationRequested;

    void RequestNavigation(string pageTag);
}
