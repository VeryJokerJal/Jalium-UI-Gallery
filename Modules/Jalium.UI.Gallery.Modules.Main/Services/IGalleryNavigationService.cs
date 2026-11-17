namespace Jalium.UI.Gallery.Modules.Main.Services;

/// <summary>
/// Provides navigation-shell operations without exposing controls or windows to
/// ViewModels.
/// </summary>
public interface IGalleryNavigationService
{
    bool TryGoBack();

    bool TryGoForward();

    void Refresh();

    void ShowNavigationWindowDemo();
}
