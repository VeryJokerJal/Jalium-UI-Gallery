namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGalleryContentDialogService
{
    Task<string> ShowRenameAsync();

    Task<string> ShowFullSizeAsync();

    Task<string> ShowDeferredValidationAsync(
        IProgress<string> progress);
}
