using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryImageSourceService
    : IGalleryImageSourceService
{
    private const string VectorMarkup = """
        <svg xmlns="http://www.w3.org/2000/svg"
             width="640"
             height="360"
             viewBox="0 0 640 360">
          <defs>
            <linearGradient id="background" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0" stop-color="#071A30"/>
              <stop offset="1" stop-color="#123E68"/>
            </linearGradient>
            <linearGradient id="mark" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0" stop-color="#64D8FF"/>
              <stop offset="1" stop-color="#7C5CFF"/>
            </linearGradient>
          </defs>
          <rect width="640" height="360" rx="28" fill="url(#background)"/>
          <circle cx="320" cy="154" r="96" fill="none"
                  stroke="url(#mark)" stroke-width="20"/>
          <path d="M262 172 L306 216 L390 112"
                fill="none" stroke="#F7FBFF" stroke-width="22"
                stroke-linecap="round" stroke-linejoin="round"/>
          <text x="320" y="304" text-anchor="middle"
                font-family="Segoe UI, sans-serif" font-size="34"
                fill="#F7FBFF">Jalium SVG</text>
        </svg>
        """;

    public GalleryImageSources CreateDemoSources()
    {
        var raster = new BitmapImage(
            new Uri(
                "Assets/SaasCardBackgrounds/image.png",
                UriKind.Relative));
        var vector = SvgImage.FromSvgString(VectorMarkup);

        var rasterStatus = raster.Width > 0 && raster.Height > 0
            ? $"Image loaded: {raster.Width:F0} x {raster.Height:F0} pixels"
            : "Image source loaded from the application resources.";

        return new GalleryImageSources(
            raster,
            vector,
            rasterStatus,
            $"SVG loaded: {vector.Width:F0} x {vector.Height:F0} (vector)");
    }
}
