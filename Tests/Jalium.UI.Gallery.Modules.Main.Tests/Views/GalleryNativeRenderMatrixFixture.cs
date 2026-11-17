using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Diagnostics;
using Jalium.UI.Interop;
using Jalium.UI.Media;
using Jalium.UI.Media.Rendering;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

/// <summary>
/// Opt-in native renderer matrix. Set JALIUM_GALLERY_RENDER_MATRIX=1 and
/// JALIUM_GALLERY_RENDER_BACKEND=Software|D3D12 to run every catalog page.
/// Set JALIUM_GALLERY_RENDER_SCENARIO=All (or a comma-separated subset of
/// Full,PartialDamage,AnimatedTransform) for the complete workload matrix.
/// The normal unit-test pass returns immediately, so the 121-page/120-frame
/// performance qualification never makes CI timing-dependent by accident.
/// </summary>
[Collection("Application")]
public sealed class GalleryNativeRenderMatrixFixture
{
    private const int Width = 1500;
    private const int Height = 920;

    [Fact]
    public void AllCatalogPages_RecordNativeCompletedFramePercentiles()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("JALIUM_GALLERY_RENDER_MATRIX"),
                "1",
                StringComparison.Ordinal))
        {
            return;
        }

        int warmupFrames = ReadPositiveInt("JALIUM_GALLERY_RENDER_WARMUP", 30);
        int sampleFrames = ReadPositiveInt("JALIUM_GALLERY_RENDER_SAMPLES", 120);
        int idleBeforeSampleMs = ReadPositiveInt("JALIUM_GALLERY_RENDER_IDLE_MS", 0);
        IReadOnlyList<RenderScenario> scenarios = ReadScenarios();
        RenderBackend backend = Enum.TryParse(
            Environment.GetEnvironmentVariable("JALIUM_GALLERY_RENDER_BACKEND"),
            ignoreCase: true,
            out RenderBackend parsedBackend)
            ? parsedBackend
            : RenderBackend.Software;
        Assert.True(
            backend is RenderBackend.Software or RenderBackend.D3D12,
            "The render matrix supports Software and D3D12 reference runs.");

        ResetApplicationState();
        _ = CreateStyledApplication();
        GalleryCssExtensions.EnsureRegistered();

        var results = new List<PageResult>();
        var failures = new List<string>();
        using var window = new MatrixNativeWindow(Width, Height);
        using var context = new RenderContext(backend);
        RenderingEngine engine = Enum.TryParse(
            Environment.GetEnvironmentVariable("JALIUM_GALLERY_RENDER_ENGINE"),
            ignoreCase: true, out RenderingEngine requestedEngine)
            ? requestedEngine : RenderingEngine.Impeller;
        context.DefaultRenderingEngine = engine;
        using var target = context.CreateRenderTarget(window.Hwnd, Width, Height);
        target.SetRenderingEngine(engine);
        target.SetVSyncEnabled(false);
        target.SetDpi(96f, 96f);
        RenderDiagnostics.ApiStatsEnabled = true;

        var pages = new List<(string Tag, Type Type)> { ("home", typeof(HomePage)) };
        pages.AddRange(GalleryComponentCatalog.Items.Select(item => (item.PageTag, item.PageType)));
        string? pageFilter = Environment.GetEnvironmentVariable("JALIUM_GALLERY_RENDER_PAGE");
        if (!string.IsNullOrWhiteSpace(pageFilter))
        {
            var selected = pageFilter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            pages = pages.Where(page => selected.Contains(page.Tag)).ToList();
        }

        try
        {
            foreach (var (tag, pageType) in pages)
            {
                try
                {
                    FrameworkElement page = CreatePage(pageType);
                    PrepareDeterministicPage(tag, page);
                    var viewport = new ScrollViewer
                    {
                        Content = page,
                        Width = Width,
                        Height = Height,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        ClipToBounds = true,
                    };
                    viewport.Measure(new Size(Width, Height));
                    viewport.Arrange(new Rect(0, 0, Width, Height));
                    viewport.UpdateLayout();

                    var drawingContext = new RenderTargetDrawingContext(target, context);
                    try
                    {
                        // Capture once, then replay the exact immutable command
                        // stream for every warmup/sample frame. This keeps the
                        // comparison on renderer work: no page code, layout,
                        // animation tick or per-frame object allocation can
                        // randomly tax one backend's P95.
                        var cacheHost = new MediaRenderCacheHost();
                        object baseDrawing = CaptureFrozenDrawing(cacheHost, viewport, tag);
                        foreach (RenderScenario scenario in scenarios)
                        {
                            object scenarioDrawing = baseDrawing;
                            nint retainedLayer = nint.Zero;
                            try
                            {
                            if (scenario.Kind == RenderScenarioKind.PartialDamage)
                            {
                                if (viewport.ScrollableHeight > 0)
                                {
                                    viewport.ScrollToVerticalOffset(
                                        Math.Min(64.0, viewport.ScrollableHeight));
                                    viewport.UpdateLayout();
                                    scenarioDrawing = CaptureFrozenDrawing(
                                        cacheHost, viewport, tag);
                                }

                                // Establish a complete backing image for the
                                // scrolled/hover state before timing damage-only
                                // updates. Subsequent frames touch one exposed
                                // horizontal band and present that damage only.
                                RenderCompletedFrame(
                                    target, drawingContext, cacheHost,
                                    scenarioDrawing, scenario, 0,
                                    forceFullInvalidation: true);
                            }
                            else if (scenario.Kind == RenderScenarioKind.AnimatedTransform)
                            {
                                retainedLayer = CaptureRetainedLayer(
                                    target, drawingContext, cacheHost,
                                    scenarioDrawing);
                            }

                            for (int frame = 0; frame < warmupFrames; frame++)
                            {
                                RenderCompletedFrame(
                                    target, drawingContext, cacheHost,
                                    scenarioDrawing, scenario, frame,
                                    retainedLayer: retainedLayer);
                            }

                            var samples = new double[sampleFrames];
                            for (int frame = 0; frame < sampleFrames; frame++)
                            {
                                if (idleBeforeSampleMs > 0)
                                {
                                    Thread.Sleep(idleBeforeSampleMs);
                                    target.ReclaimIdleResources();
                                }
                                var timer = Stopwatch.StartNew();
                                RenderCompletedFrame(
                                    target, drawingContext, cacheHost,
                                    scenarioDrawing, scenario,
                                    warmupFrames + frame,
                                    retainedLayer: retainedLayer);
                                timer.Stop();
                                samples[frame] = timer.Elapsed.TotalMilliseconds;
                            }
                            Array.Sort(samples);
                            var api = RenderDiagnostics.LatestDrawApiStats;
                            var hottest = api?.Entries.FirstOrDefault();
                            _ = target.TryQueryGpuStats(out GpuResourceStats resources);
                            results.Add(new PageResult(
                                tag,
                                pageType.FullName ?? pageType.Name,
                                backend.ToString(),
                                scenario.Name,
                                sampleFrames,
                                Percentile(samples, 0.50),
                                Percentile(samples, 0.95),
                                Percentile(samples, 0.99),
                                samples[0],
                                samples[^1],
                                api?.TotalMs ?? 0.0,
                                hottest?.Name ?? string.Empty,
                                hottest?.Count ?? 0,
                                hottest?.TotalMs ?? 0.0,
                                resources.GlyphBytes,
                                resources.PathBytes,
                                resources.TextureBytes,
                                resources.GlyphBytes + resources.PathBytes +
                                    resources.TextureBytes,
                                resources.GlyphSlotsUsed,
                                resources.PathEntries,
                                resources.TextureCount,
                                resources.SoftwareRasterNs,
                                resources.SoftwarePixelsVisited,
                                resources.SoftwarePixelsBlended,
                                resources.SoftwareAaSamples,
                                resources.SoftwareClipRejectedPixels,
                                resources.SoftwareParallelNs,
                                resources.SoftwareCacheBytes,
                                resources.SoftwareEffectCacheHits,
                                resources.SoftwareEffectCacheMisses,
                                resources.SoftwareWorkerCount,
                                resources.SoftwareWorkerUtilizationPermille,
                                resources.SoftwareEffectCacheEntries,
                                resources.SoftwareGradientCacheEntries,
                                api?.Entries.Select(entry => new ApiResult(
                                    entry.Name,
                                    entry.Count,
                                    entry.TotalMs,
                                    entry.AvgUs)).ToArray() ?? []));
                            }
                            finally
                            {
                                if (retainedLayer != nint.Zero)
                                {
                                    NativeMethods.jalium_render_target_destroy_retained_layer(
                                        target.Handle, retainedLayer);
                                }
                            }
                        }
                    }
                    finally
                    {
                        drawingContext.ClearCache();
                        target.ReclaimIdleResources();
                        drawingContext.Close();
                    }
                }
                catch (Exception exception)
                {
                    failures.Add($"{tag}: {exception.GetType().Name}: {exception.Message}");
                }
            }
        }
        finally
        {
            RenderDiagnostics.ApiStatsEnabled = false;
            WriteResults(results, failures, backend);
            ResetApplicationState();
        }

        Assert.True(
            failures.Count == 0,
            "Gallery native render matrix failures:" + Environment.NewLine +
            string.Join(Environment.NewLine, failures));
        Assert.Equal(pages.Count * scenarios.Count, results.Count);
    }

    private static object CaptureFrozenDrawing(
        MediaRenderCacheHost cacheHost,
        FrameworkElement viewport,
        string pageTag)
    {
        DrawingContext recorder = cacheHost.CreateFrameRecorder();
        object frozenDrawing;
        try
        {
            viewport.Render(recorder);
        }
        finally
        {
            frozenDrawing = cacheHost.FinishRecord(recorder);
        }
        var recorded = Assert.IsType<RecordedDrawing>(frozenDrawing);
        Assert.True(
            recorded.IsFullyRecordable,
            $"{pageTag} contains a draw command that cannot be frozen for fair replay.");
        return frozenDrawing;
    }

    private static nint CaptureRetainedLayer(
        RenderTarget target,
        RenderTargetDrawingContext drawingContext,
        MediaRenderCacheHost cacheHost,
        object frozenDrawing)
    {
        var bounds = new Rect(0, 0, Width, Height);
        target.SetFullInvalidation();
        Assert.True(target.TryBeginDraw());
        target.Clear(0.04f, 0.055f, 0.07f, 1f);
        Point savedOffset = drawingContext.Offset;
        drawingContext.Offset = Point.Zero;
        nint layer = nint.Zero;
        try
        {
            layer = drawingContext.BeginLayerCapture(nint.Zero, bounds);
            Assert.NotEqual(nint.Zero, layer);
            try
            {
                cacheHost.Replay(frozenDrawing, drawingContext);
            }
            finally
            {
                drawingContext.EndLayerCapture(layer);
            }
            drawingContext.CompositeLayer(
                layer, bounds, 1.0, transform: null, originX: 0, originY: 0);
        }
        catch
        {
            if (layer != nint.Zero)
            {
                NativeMethods.jalium_render_target_destroy_retained_layer(
                    target.Handle, layer);
            }
            throw;
        }
        finally
        {
            drawingContext.Offset = savedOffset;
        }
        Assert.Equal(JaliumResult.Ok, target.TryEndDraw());
        Assert.Equal(
            0,
            NativeMethods.jalium_render_target_wait_for_completion(target.Handle));
        RenderDiagnostics.PublishAndResetApiStats();
        return layer;
    }

    private static void RenderCompletedFrame(
        RenderTarget target,
        RenderTargetDrawingContext drawingContext,
        MediaRenderCacheHost cacheHost,
        object frozenDrawing,
        RenderScenario scenario,
        int frameIndex,
        bool forceFullInvalidation = false,
        nint retainedLayer = default)
    {
        if (forceFullInvalidation ||
            scenario.Kind != RenderScenarioKind.PartialDamage)
        {
            target.SetFullInvalidation();
        }
        else
        {
            target.AddDirtyRect(0f, Height / 2f - 64f, Width, 128f);
        }
        Assert.True(target.TryBeginDraw());
        target.Clear(0.04f, 0.055f, 0.07f, 1f);
        Point savedOffset = drawingContext.Offset;
        drawingContext.Offset = Point.Zero;
        try
        {
            if (scenario.Kind == RenderScenarioKind.AnimatedTransform)
            {
                Assert.NotEqual(nint.Zero, retainedLayer);
                drawingContext.CompositeLayer(
                    retainedLayer,
                    new Rect(0, 0, Width, Height),
                    1.0,
                    s_animationTransforms[frameIndex % s_animationTransforms.Length],
                    originX: 0,
                    originY: 0);
            }
            else
            {
                cacheHost.Replay(frozenDrawing, drawingContext);
            }
        }
        finally
        {
            drawingContext.Offset = savedOffset;
        }
        Assert.Equal(JaliumResult.Ok, target.TryEndDraw());
        Assert.Equal(
            0,
            NativeMethods.jalium_render_target_wait_for_completion(target.Handle));
        RenderDiagnostics.PublishAndResetApiStats();
    }

    private static IReadOnlyList<RenderScenario> ReadScenarios()
    {
        string configured = Environment.GetEnvironmentVariable(
            "JALIUM_GALLERY_RENDER_SCENARIO") ?? "Full";
        if (string.Equals(configured, "All", StringComparison.OrdinalIgnoreCase))
        {
            return s_allScenarios;
        }

        var requested = configured.Split(
            ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new List<RenderScenario>();
        foreach (string name in requested)
        {
            RenderScenario scenario = s_allScenarios.FirstOrDefault(
                candidate => string.Equals(
                    candidate.Name, name, StringComparison.OrdinalIgnoreCase));
            Assert.False(
                string.IsNullOrEmpty(scenario.Name),
                $"Unknown render scenario '{name}'.");
            if (!result.Contains(scenario)) result.Add(scenario);
        }
        Assert.NotEmpty(result);
        return result;
    }

    private enum RenderScenarioKind
    {
        Full,
        PartialDamage,
        AnimatedTransform,
    }

    private readonly record struct RenderScenario(
        string Name,
        RenderScenarioKind Kind);

    private static readonly RenderScenario[] s_allScenarios =
    [
        new("Full", RenderScenarioKind.Full),
        new("PartialDamage", RenderScenarioKind.PartialDamage),
        new("AnimatedTransform", RenderScenarioKind.AnimatedTransform),
    ];

    private static readonly MatrixTransform[] s_animationTransforms =
    [
        new(1, 0, 0, 1, 0.00, 0.00),
        new(1, 0, 0, 1, 0.25, 0.15),
        new(1, 0, 0, 1, 0.50, 0.30),
        new(1, 0, 0, 1, 0.75, 0.45),
    ];

    private static readonly MapTileSource s_deterministicTileSource = new()
    {
        // MapTileCache creates its coordinate-derived placeholder before it
        // attempts the URL. An empty URL makes that attempt fail immediately,
        // so no network completion can race the frozen command capture.
        UrlTemplate = string.Empty,
        Attribution = "Local deterministic tiles",
        MinZoom = 0,
        MaxZoom = 20,
        TileSize = 256,
    };

    private static FrameworkElement CreatePage(Type pageType)
    {
        var page = Assert.IsAssignableFrom<FrameworkElement>(
            Activator.CreateInstance(pageType));
        if (page is HomePage)
        {
            SaasCardBackgrounds.BeginStartupPrefetch(
                GalleryComponentCatalog.Items.Take(24).Select(item => item.PageTag));
            SaasCardBackgrounds.WaitForStartupPrefetch();
            page.DataContext = new HomeViewModel(new GalleryNavigationCoordinator());
            return page;
        }
        string viewModelName =
            "Jalium.UI.Gallery.Modules.Main.ViewModels." +
            (pageType.Name.EndsWith("Page", StringComparison.Ordinal)
                ? pageType.Name[..^"Page".Length]
                : pageType.Name) +
            "ViewModel";
        Type? viewModelType = pageType.Assembly.GetType(viewModelName);
        if (viewModelType?.GetConstructor(Type.EmptyTypes) is not null)
        {
            page.DataContext = Activator.CreateInstance(viewModelType);
        }
        return page;
    }

    private static void PrepareDeterministicPage(string tag, FrameworkElement page)
    {
        // A windowed WebView emits an OS child-window punch command, which is
        // intentionally non-recordable and would also make the benchmark
        // depend on an installed runtime/network response. Keep its reserved
        // layout slot but hide the native child; all surrounding Gallery UI is
        // still frozen and replayed like every other page.
        bool prepareWebView = string.Equals(
            tag, "webview", StringComparison.OrdinalIgnoreCase);
        bool prepareMap = string.Equals(
            tag, "mapview", StringComparison.OrdinalIgnoreCase);
        if (!prepareWebView && !prepareMap)
        {
            return;
        }

        var pending = new Stack<DependencyObject>();
        pending.Push(page);
        while (pending.Count > 0)
        {
            DependencyObject current = pending.Pop();
            if (prepareWebView && current is WebView webView)
            {
                webView.Source = null;
                webView.Visibility = Visibility.Hidden;
                continue;
            }
            if (prepareMap && current is MapView mapView)
            {
                mapView.TileSource = s_deterministicTileSource;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(current);
            for (int index = 0; index < childCount; index++)
            {
                if (VisualTreeHelper.GetChild(current, index) is { } child)
                {
                    pending.Push(child);
                }
            }
        }
    }

    private static Application CreateStyledApplication()
    {
        var application = new Application();
        GalleryTheme.CurrentMode = GalleryThemeMode.Dark;
        GalleryTheme.RegisterResources(application.Resources);
        application.Resources.MergedDictionaries.Add(new GalleryStyles());
        return application;
    }

    private static void ResetApplicationState()
    {
        typeof(Application)
            .GetField("_current", BindingFlags.NonPublic | BindingFlags.Static)
            ?.SetValue(null, null);
    }

    private static int ReadPositiveInt(string name, int fallback)
    {
        return int.TryParse(Environment.GetEnvironmentVariable(name), out int value) && value > 0
            ? value
            : fallback;
    }

    private static double Percentile(double[] sorted, double percentile)
    {
        double position = percentile * (sorted.Length - 1);
        int lower = (int)Math.Floor(position);
        int upper = (int)Math.Ceiling(position);
        if (lower == upper) return sorted[lower];
        double weight = position - lower;
        return sorted[lower] * (1.0 - weight) + sorted[upper] * weight;
    }

    private static void WriteResults(
        IReadOnlyList<PageResult> results,
        IReadOnlyList<string> failures,
        RenderBackend backend)
    {
        string? output = Environment.GetEnvironmentVariable("JALIUM_GALLERY_RENDER_MATRIX_OUT");
        if (string.IsNullOrWhiteSpace(output)) return;
        string path = Path.GetFullPath(output);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(
                new
                {
                    Backend = backend.ToString(),
                    Engine = Environment.GetEnvironmentVariable("JALIUM_GALLERY_RENDER_ENGINE") ?? "Impeller",
                    IdleBeforeSampleMs = ReadPositiveInt("JALIUM_GALLERY_RENDER_IDLE_MS", 0),
                    Width,
                    Height,
                    Dpi = 96,
                    TimestampUtc = DateTime.UtcNow,
                    Results = results,
                    Failures = failures,
                },
                new JsonSerializerOptions { WriteIndented = true }));
    }

    private sealed record PageResult(
        string PageTag,
        string PageType,
        string Backend,
        string Scenario,
        int Samples,
        double P50Ms,
        double P95Ms,
        double P99Ms,
        double MinMs,
        double MaxMs,
        double ApiTotalMs,
        string HottestApi,
        long HottestApiCalls,
        double HottestApiMs,
        long GlyphCacheBytes,
        long PathCacheBytes,
        long TextureAndWorkBytes,
        long RendererTrackedBytes,
        int GlyphCacheEntries,
        int PathCacheEntries,
        int TextureAndWorkEntries,
        long SoftwareRasterNs,
        long SoftwarePixelsVisited,
        long SoftwarePixelsBlended,
        long SoftwareAaSamples,
        long SoftwareClipRejectedPixels,
        long SoftwareParallelNs,
        long SoftwareCacheBytes,
        long SoftwareEffectCacheHits,
        long SoftwareEffectCacheMisses,
        int SoftwareWorkerCount,
        int SoftwareWorkerUtilizationPermille,
        int SoftwareEffectCacheEntries,
        int SoftwareGradientCacheEntries,
        IReadOnlyList<ApiResult> ApiEntries);

    private sealed record ApiResult(
        string Name,
        long Calls,
        double TotalMs,
        double AverageUs);

    private static class NativeMethods
    {
        [DllImport("jalium.native.core.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int jalium_render_target_wait_for_completion(nint target);

        [DllImport("jalium.native.core.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void jalium_render_target_destroy_retained_layer(
            nint target,
            nint layer);
    }

    private sealed class MatrixNativeWindow : IDisposable
    {
        public MatrixNativeWindow(int width, int height)
        {
            Hwnd = CreateWindowExW(
                0, "STATIC", "Jalium Gallery render matrix", 0,
                0, 0, width, height,
                nint.Zero, nint.Zero, nint.Zero, nint.Zero);
            if (Hwnd == nint.Zero)
            {
                throw new InvalidOperationException("Failed to create native matrix window.");
            }
        }

        public nint Hwnd { get; }

        public void Dispose()
        {
            if (Hwnd != nint.Zero) DestroyWindow(Hwnd);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern nint CreateWindowExW(
            uint exStyle,
            string className,
            string title,
            uint style,
            int x,
            int y,
            int width,
            int height,
            nint parent,
            nint menu,
            nint instance,
            nint parameter);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(nint hwnd);
    }
}
