using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main;
using Jalium.UI.Gallery.Modules.Main.Headless;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Hosting;
using Jalium.UI.Interop;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Jalium.UI.Gallery.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Jalium.UI.Gallery.Headless;

internal sealed class HeadlessGallerySession : IDisposable
{
    private const int BytesPerPixel = 4;

    // The current public RenderContext surface factory rejects handle zero in
    // jalium.native.core before dispatching to SoftwareBackend, although that
    // backend is explicitly able to render without a presenter. A non-zero
    // sentinel crosses that generic validation without creating a native
    // window. SoftwareRenderTarget's optional Windows present attempt simply
    // observes that this is not an HWND, while readback remains the authoritative
    // output. No CreateWindow/ShowWindow/Application.Run path is involved.
    private static readonly nint OffscreenSurfaceSentinel = new(1);

    private readonly HeadlessOptions _options;
    private readonly RenderContext _renderContext;
    private readonly JaliumApp _app;
    private readonly IViewFactory _viewFactory;
    private readonly IGalleryPageResolver _pageResolver;
    private bool _disposed;

    private HeadlessGallerySession(
        HeadlessOptions options,
        RenderContext renderContext,
        JaliumApp app)
    {
        _options = options;
        _renderContext = renderContext;
        _app = app;
        _viewFactory = app.Services.GetRequiredService<IViewFactory>();
        _pageResolver = app.Services.GetRequiredService<IGalleryPageResolver>();
    }

    public string RendererName =>
        $"Jalium.UI native {_renderContext.Backend}/{_options.RenderingEngine} offscreen";

    public static HeadlessGallerySession Create(HeadlessOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        RenderContext? renderContext = null;
        JaliumApp? app = null;
        try
        {
            // Unlike RenderTargetBitmap's intentionally small managed rasterizer,
            // the native Software path includes glyphs, decoded images, paths,
            // clips and effects while remaining independent of a platform window.
            renderContext = new RenderContext(
                RenderBackend.Software,
                renderingEngine: options.RenderingEngine);

            var builder = AppBuilder.CreateBuilder(new AppBuilderSettings
            {
                Args = [],
                DisableDefaults = true,
            });
            builder.Services.AddAppServices();
            builder.Services.AddMainModule();

            app = builder.Build();
            app.UseApplication<App>();

            // Construct and attach the Application/resource tree, but deliberately
            // do not call StartHosted(), Run(), Show() or OnStartup().
            _ = app.Application;
            LocalizationManager.SetLanguage(options.Language);
            app.Services
                .GetRequiredService<IGalleryThemeService>()
                .SetDark(options.DarkTheme);

            return new HeadlessGallerySession(options, renderContext, app);
        }
        catch
        {
            app?.Dispose();
            renderContext?.Dispose();
            throw;
        }
    }

    public HeadlessPageReport Render(
        HeadlessPageDescriptor descriptor,
        string pngPath,
        string? metadataPath)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(descriptor);

        if (string.Equals(descriptor.PageTag, "home", StringComparison.OrdinalIgnoreCase))
        {
            GalleryHeadlessSupport.WaitForStartupAssets();
        }

        var timer = Stopwatch.StartNew();
        var content = CreateContent(descriptor);
        var shell = content as MainWindow;
        try
        {
            var layout = PrepareLayout(content, descriptor.PageTag);
            var background = _options.DarkTheme
                ? GalleryTheme.BackgroundDark
                : GalleryTheme.BackgroundLight;
            var pixels = RenderPixels(layout.Root, layout.PixelWidth, layout.PixelHeight, background);
            var statistics = AnalyzePixels(pixels, layout.PixelWidth, layout.PixelHeight, background);
            if (statistics.PaintedPixels < 64)
            {
                throw new InvalidOperationException(
                    $"The offscreen renderer produced an effectively blank image " +
                    $"({statistics.PaintedPixels} painted pixels).");
            }

            WritePng(pngPath, pixels, layout.PixelWidth, layout.PixelHeight);
            var semantics = _options.EmitSemantics
                ? HeadlessSemanticSnapshot.Capture(
                    layout.Root,
                    _options.Dpi / 96.0,
                    layout.PixelWidth,
                    layout.PixelHeight)
                : [];
            var pixelHash = Convert.ToHexString(SHA256.HashData(pixels)).ToLowerInvariant();

            if (metadataPath is not null)
            {
                var metadata = new HeadlessPageMetadata(
                    1,
                    descriptor.PageTag,
                    descriptor.Title,
                    descriptor.Category,
                    descriptor.PageType.FullName ?? descriptor.PageType.Name,
                    Path.GetFullPath(pngPath),
                    RendererName,
                    PlatformWindowCreated: false,
                    _options.Surface.ToString(),
                    _options.DarkTheme ? "dark" : "light",
                    _options.LanguageName,
                    _options.Dpi,
                    layout.PixelWidth,
                    layout.PixelHeight,
                    layout.LogicalWidth,
                    layout.LogicalHeight,
                    layout.DesiredContentHeight,
                    layout.ContentClipped,
                    pixelHash,
                    statistics.PaintedBounds,
                    semantics);
                WriteJson(metadataPath, metadata);
            }

            timer.Stop();
            return new HeadlessPageReport(
                descriptor.PageTag,
                descriptor.Title,
                descriptor.Category,
                descriptor.PageType.FullName ?? descriptor.PageType.Name,
                Path.GetFullPath(pngPath),
                metadataPath is null ? null : Path.GetFullPath(metadataPath),
                layout.PixelWidth,
                layout.PixelHeight,
                layout.LogicalWidth,
                layout.LogicalHeight,
                layout.DesiredContentHeight,
                layout.ContentClipped,
                statistics.PaintedPixels,
                Math.Round(statistics.PaintedPixels / (double)(layout.PixelWidth * layout.PixelHeight), 6),
                Math.Round(statistics.OpaquePixels / (double)(layout.PixelWidth * layout.PixelHeight), 6),
                pixelHash,
                semantics.Count,
                timer.ElapsedMilliseconds);
        }
        finally
        {
            if (shell is not null)
            {
                GalleryHeadlessSupport.ReleaseShell(shell);
            }
        }
    }

    private UIElement CreateContent(HeadlessPageDescriptor descriptor)
    {
        var logicalWidth = _options.PixelWidth * 96.0 / _options.Dpi;
        switch (_options.Surface)
        {
            case HeadlessSurface.Shell:
            {
                var window = _viewFactory.CreateView<MainWindow>()
                    ?? throw new InvalidOperationException("MainWindow could not be resolved.");
                window.Width = logicalWidth;
                window.Height = _options.PixelHeight * 96.0 / _options.Dpi;
                GalleryHeadlessSupport.PrepareShell(window, descriptor.PageTag, logicalWidth);
                return window;
            }

            case HeadlessSurface.RawPage:
                return _viewFactory.CreateView(descriptor.PageType) as UIElement
                    ?? throw new InvalidOperationException(
                        $"View '{descriptor.PageType.FullName}' could not be resolved.");

            default:
                return _pageResolver.Resolve(descriptor.PageTag);
        }
    }

    private LayoutResult PrepareLayout(UIElement content, string pageTag)
    {
        var dpiScale = _options.Dpi / 96.0;
        var logicalWidth = _options.PixelWidth / dpiScale;
        var logicalHeight = _options.PixelHeight / dpiScale;

        if (_options.Surface == HeadlessSurface.Shell)
        {
            SettleLayout(content, pageTag, logicalWidth, logicalHeight);
            var scrollState = ReadScrollState(content, logicalHeight);
            return new LayoutResult(
                content,
                _options.PixelWidth,
                _options.PixelHeight,
                logicalWidth,
                logicalHeight,
                scrollState.DesiredContentHeight,
                scrollState.ContentClipped);
        }

        var viewport = new ScrollViewer
        {
            Content = content,
            Width = logicalWidth,
            Height = logicalHeight,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            ClipToBounds = true,
            Background = new SolidColorBrush(
                _options.DarkTheme
                    ? GalleryTheme.BackgroundDark
                    : GalleryTheme.BackgroundLight),
        };

        SettleLayout(viewport, pageTag, logicalWidth, logicalHeight);
        var desiredContentHeight = Math.Max(
            content.DesiredSize.Height,
            viewport.ExtentHeight);
        if (!double.IsFinite(desiredContentHeight) || desiredContentHeight <= 0)
        {
            desiredContentHeight = logicalHeight;
        }

        var pixelHeight = _options.PixelHeight;
        if (_options.FullPage)
        {
            pixelHeight = Math.Clamp(
                (int)Math.Ceiling((desiredContentHeight + 1) * dpiScale),
                1,
                _options.MaxPixelHeight);
            logicalHeight = pixelHeight / dpiScale;
            viewport.Height = logicalHeight;
            SettleLayout(viewport, pageTag, logicalWidth, logicalHeight);
            desiredContentHeight = Math.Max(desiredContentHeight, viewport.ExtentHeight);
        }

        var contentClipped = desiredContentHeight > logicalHeight + 0.5 ||
            viewport.ScrollableHeight > 0.5;
        return new LayoutResult(
            viewport,
            _options.PixelWidth,
            pixelHeight,
            logicalWidth,
            logicalHeight,
            desiredContentHeight,
            contentClipped);
    }

    private void SettleLayout(
        UIElement root,
        string pageTag,
        double logicalWidth,
        double logicalHeight)
    {
        var size = new Size(logicalWidth, logicalHeight);
        for (var pass = 0; pass < _options.SettlePasses; pass++)
        {
            ApplyTemplatesAndInvalidate(root);
            GalleryHeadlessSupport.PrepareDeterministicContent(root, pageTag);
            root.Measure(size);
            root.Arrange(new Rect(0, 0, logicalWidth, logicalHeight));
            root.UpdateLayout();
        }

        GalleryHeadlessSupport.PrepareDeterministicContent(root, pageTag);
    }

    private static void ApplyTemplatesAndInvalidate(DependencyObject root)
    {
        var pending = new Stack<DependencyObject>();
        var visited = new HashSet<DependencyObject>(ReferenceEqualityComparer.Instance);
        pending.Push(root);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            if (!visited.Add(current))
            {
                continue;
            }

            if (current is Control control)
            {
                control.ApplyTemplate();
            }

            if (current is UIElement element)
            {
                element.InvalidateMeasure();
                element.InvalidateArrange();
            }

            var childCount = VisualTreeHelper.GetChildrenCount(current);
            for (var index = childCount - 1; index >= 0; index--)
            {
                if (VisualTreeHelper.GetChild(current, index) is { } child)
                {
                    pending.Push(child);
                }
            }
        }
    }

    private static (double DesiredContentHeight, bool ContentClipped) ReadScrollState(
        DependencyObject root,
        double viewportHeight)
    {
        var desiredContentHeight = viewportHeight;
        var contentClipped = false;
        var pending = new Stack<DependencyObject>();
        var visited = new HashSet<DependencyObject>(ReferenceEqualityComparer.Instance);
        pending.Push(root);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            if (!visited.Add(current))
            {
                continue;
            }

            if (current is ScrollViewer viewer)
            {
                if (double.IsFinite(viewer.ExtentHeight))
                {
                    desiredContentHeight = Math.Max(desiredContentHeight, viewer.ExtentHeight);
                }

                contentClipped |= viewer.ScrollableHeight > 0.5;
            }

            var childCount = VisualTreeHelper.GetChildrenCount(current);
            for (var index = childCount - 1; index >= 0; index--)
            {
                if (VisualTreeHelper.GetChild(current, index) is { } child)
                {
                    pending.Push(child);
                }
            }
        }

        return (desiredContentHeight, contentClipped);
    }

    private byte[] RenderPixels(
        UIElement root,
        int pixelWidth,
        int pixelHeight,
        Color background)
    {
        using var target = _renderContext.CreateRenderTarget(
            OffscreenSurfaceSentinel,
            pixelWidth,
            pixelHeight);
        target.SetRenderingEngine(_options.RenderingEngine);
        target.SetVSyncEnabled(false);
        target.SetDpi((float)_options.Dpi, (float)_options.Dpi);
        target.SetFullInvalidation();

        var drawingContext = new RenderTargetDrawingContext(target, _renderContext);
        var ended = false;
        try
        {
            target.BeginDraw();
            target.Clear(
                background.R / 255f,
                background.G / 255f,
                background.B / 255f,
                background.A / 255f);
            drawingContext.Offset = Point.Zero;
            root.Render(drawingContext);

            var requestResult = target.RequestReadback();
            if (requestResult != JaliumResult.Ok)
            {
                throw new InvalidOperationException(
                    $"The {_renderContext.Backend} backend rejected readback: {requestResult}.");
            }

            target.EndDraw();
            ended = true;
        }
        finally
        {
            if (!ended && target.IsDrawing)
            {
                _ = target.TryEndDraw();
            }

            drawingContext.ClearCache();
            drawingContext.Close();
        }

        var pixels = new byte[checked(pixelWidth * pixelHeight * BytesPerPixel)];
        var fetchResult = target.FetchReadback(
            pixels,
            checked((uint)(pixelWidth * BytesPerPixel)),
            out var capturedWidth,
            out var capturedHeight);
        if (fetchResult != JaliumResult.Ok)
        {
            throw new InvalidOperationException(
                $"The {_renderContext.Backend} backend could not fetch readback: {fetchResult}.");
        }

        if (capturedWidth != pixelWidth || capturedHeight != pixelHeight)
        {
            throw new InvalidOperationException(
                $"Readback size {capturedWidth}x{capturedHeight} does not match " +
                $"the requested {pixelWidth}x{pixelHeight} bitmap.");
        }

        return pixels;
    }

    private static PixelStatistics AnalyzePixels(
        byte[] pixels,
        int width,
        int height,
        Color background)
    {
        long painted = 0;
        long opaque = 0;
        var left = width;
        var top = height;
        var right = -1;
        var bottom = -1;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var offset = ((y * width) + x) * BytesPerPixel;
                var b = pixels[offset];
                var g = pixels[offset + 1];
                var r = pixels[offset + 2];
                var a = pixels[offset + 3];
                if (a >= 250)
                {
                    opaque++;
                }

                if (Math.Abs(b - background.B) <= 2 &&
                    Math.Abs(g - background.G) <= 2 &&
                    Math.Abs(r - background.R) <= 2 &&
                    Math.Abs(a - background.A) <= 2)
                {
                    continue;
                }

                painted++;
                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }

        HeadlessContentBounds? paintedBounds = right < left || bottom < top
            ? null
            : new HeadlessContentBounds(
                left,
                top,
                right - left + 1,
                bottom - top + 1);
        return new PixelStatistics(painted, opaque, paintedBounds);
    }

    private static void WritePng(string path, byte[] pixels, int width, int height)
    {
        using var bitmap = BitmapImage.FromPixels(pixels, width, height);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        WriteAtomically(path, encoder.Save);
    }

    public static void WriteJson<T>(string path, T value)
    {
        WriteAtomically(path, stream =>
        {
            JsonSerializer.Serialize(stream, value, HeadlessJson.Options);
            stream.WriteByte((byte)'\n');
        });
    }

    private static void WriteAtomically(string path, Action<Stream> write)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(write);

        var fullPath = Path.GetFullPath(path);
        var parent = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidOperationException($"Output path '{fullPath}' has no parent directory.");
        Directory.CreateDirectory(parent);
        var temporaryPath = Path.Combine(
            parent,
            $".{Path.GetFileName(fullPath)}.{Environment.ProcessId}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(
                       temporaryPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None))
            {
                write(stream);
                stream.Flush(flushToDisk: true);
            }

            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _app.Dispose();
        _renderContext.Dispose();
    }

    private sealed record LayoutResult(
        UIElement Root,
        int PixelWidth,
        int PixelHeight,
        double LogicalWidth,
        double LogicalHeight,
        double DesiredContentHeight,
        bool ContentClipped);

    private sealed record PixelStatistics(
        long PaintedPixels,
        long OpaquePixels,
        HeadlessContentBounds? PaintedBounds);
}

internal static class HeadlessJson
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
}
