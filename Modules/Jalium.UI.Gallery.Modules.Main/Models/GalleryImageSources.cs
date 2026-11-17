using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed record GalleryImageSources(
    ImageSource Raster,
    ImageSource Vector,
    string RasterStatus,
    string VectorStatus);
