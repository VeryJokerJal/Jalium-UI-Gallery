using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public interface IGallerySystemDialogService
{
    string? OpenFile();

    int? OpenMultipleFiles();

    string? PickFolder(bool useInitialDirectory);

    string? SaveFile();

    FontDialogSelection? ChooseFont();

    string? PickIcon();

    string ShowOpenWith();

    string ShowFileProperties(bool multiple);

    string ShowPrinterProperties();

    string ShowVolumeProperties();

    MessageDialogPreview ShowMessage(MessageDialogKind kind);
}
