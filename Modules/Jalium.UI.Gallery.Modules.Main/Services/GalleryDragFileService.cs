namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryDragFileService
    : IGalleryDragFileService
{
    public string? CreateDemoFile()
    {
        try
        {
            var path = Path.Combine(
                Path.GetTempPath(),
                "JaliumGalleryDragDrop.txt");
            File.WriteAllText(
                path,
                "Hello from the Jalium.UI Gallery — dragged out through the Windows Shell.");
            return path;
        }
        catch
        {
            return null;
        }
    }
}
