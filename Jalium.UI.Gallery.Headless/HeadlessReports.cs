namespace Jalium.UI.Gallery.Headless;

internal sealed record HeadlessCatalogReport(
    int SchemaVersion,
    int PageCount,
    IReadOnlyList<HeadlessCatalogPage> Pages);

internal sealed record HeadlessCatalogPage(
    string PageTag,
    string Title,
    string Category,
    string PageType);

internal sealed record HeadlessBatchReport(
    int SchemaVersion,
    string Renderer,
    bool PlatformWindowCreated,
    string Surface,
    string Theme,
    string Language,
    double Dpi,
    DateTimeOffset CreatedUtc,
    string ManifestPath,
    IReadOnlyList<HeadlessPageReport> Pages,
    IReadOnlyList<HeadlessFailureReport> Failures);

internal sealed record HeadlessPageReport(
    string PageTag,
    string Title,
    string Category,
    string PageType,
    string PngPath,
    string? MetadataPath,
    int PixelWidth,
    int PixelHeight,
    double LogicalWidth,
    double LogicalHeight,
    double DesiredContentHeight,
    bool ContentClipped,
    long PaintedPixels,
    double PaintedFraction,
    double OpaqueFraction,
    string PixelSha256,
    int SemanticTextCount,
    long ElapsedMilliseconds);

internal sealed record HeadlessFailureReport(
    string PageTag,
    string ErrorType,
    string Message);

internal sealed record HeadlessPageMetadata(
    int SchemaVersion,
    string PageTag,
    string Title,
    string Category,
    string PageType,
    string PngPath,
    string Renderer,
    bool PlatformWindowCreated,
    string Surface,
    string Theme,
    string Language,
    double Dpi,
    int PixelWidth,
    int PixelHeight,
    double LogicalWidth,
    double LogicalHeight,
    double DesiredContentHeight,
    bool ContentClipped,
    string PixelSha256,
    HeadlessContentBounds? PaintedBounds,
    IReadOnlyList<HeadlessSemanticText> SemanticText);

internal sealed record HeadlessContentBounds(
    int X,
    int Y,
    int Width,
    int Height);

internal sealed record HeadlessSemanticText(
    string Text,
    string Source,
    string ElementType,
    string? ElementName,
    double X,
    double Y,
    double Width,
    double Height);
