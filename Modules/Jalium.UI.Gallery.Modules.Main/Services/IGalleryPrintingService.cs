namespace Jalium.UI.Gallery.Modules.Main.Services;

/// <summary>
/// Isolates the operating-system printing APIs from the ViewModel layer.
/// </summary>
public interface IGalleryPrintingService
{
    bool? ShowPrintDialog();

    string GetAvailablePrinters();
}
