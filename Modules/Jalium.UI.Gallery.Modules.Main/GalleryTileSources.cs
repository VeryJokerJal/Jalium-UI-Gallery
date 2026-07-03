using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main;

/// <summary>
/// Shared <see cref="MapTileSource"/> definitions used by the gallery's map demos.
/// The framework default (OpenStreetMap) blocks the gallery's tile requests with an
/// "Access blocked" / HTTP 403 tile, so the map pages use a China-accessible
/// provider instead.
/// </summary>
internal static class GalleryTileSources
{
    /// <summary>
    /// AutoNavi / AMap (高德地图) raster tiles. AMap uses the standard Web Mercator
    /// (EPSG:3857) XYZ tile scheme, so it is a drop-in replacement for the
    /// OpenStreetMap default used by <see cref="MapView"/> — no projection changes
    /// are required and overlays still align (GCJ-02 has no offset outside China).
    /// </summary>
    public static MapTileSource Amap { get; } = new()
    {
        // style=7 → road map with labels. wprd01..wprd04 are interchangeable tile
        // subdomains; a single host is plenty for the gallery's modest tile volume.
        UrlTemplate = "https://wprd01.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=1&style=7",
        // Rendered on the map surface — kept ASCII so it never depends on a CJK-capable font.
        Attribution = "© AutoNavi",
        MinZoom = 3,
        MaxZoom = 18,
        TileSize = 256
    };
}
