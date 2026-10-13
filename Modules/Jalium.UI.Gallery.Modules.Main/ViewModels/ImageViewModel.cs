using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class ImageViewModel
{
    public ImageViewModel(
        IGalleryImageSourceService imageSourceService)
    {
        var sources = imageSourceService.CreateDemoSources();
        RasterSource = sources.Raster;
        SvgSource = sources.Vector;
        RasterStatus = sources.RasterStatus;
        SvgStatus = sources.VectorStatus;
    }

    public ImageSource RasterSource { get; }

    public ImageSource SvgSource { get; }

    public string RasterStatus { get; }

    public string SvgStatus { get; }
}
