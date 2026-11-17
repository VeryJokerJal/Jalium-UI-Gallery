using System.Diagnostics;
using System.Globalization;
using Foundation;
using Jalium.UI.Controls;
using Jalium.UI.Diagnostics;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.MacOS;

/// <summary>
/// Opt-in whole-Gallery measurement. Effect mode requests full frames;
/// scroll mode only observes completed frames without changing render demand.
/// Normal Gallery runs do not construct the timer or write a report.
/// </summary>
internal sealed class EffectPerformanceProbe : IDisposable
{
    internal const string LogKey = "JALIUM_GALLERY_EFFECT_PERF_LOG";
    internal const string ScrollLogKey = "JALIUM_GALLERY_SCROLL_PERF_LOG";
    private readonly DispatcherTimer _timer;
    private readonly bool _forceFrames;
    private readonly StreamWriter _log;
    private readonly StreamWriter? _frameLog;
    private readonly FrameHistory.Sample[] _samples = new FrameHistory.Sample[FrameHistory.Capacity];
    private Window? _window;
    private long _reportStart;
    private long _previousFrames;
    private long _lastSampleTicks;
    private bool _disposed;

    private EffectPerformanceProbe(string path, bool forceFrames)
    {
        _forceFrames = forceFrames;
        _timer = new DispatcherTimer
        {
            Interval = forceFrames ? TimeSpan.FromMilliseconds(16) : TimeSpan.FromSeconds(1)
        };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        _log = new StreamWriter(path) { AutoFlush = true };
        _log.WriteLine("utc,pixels,frames,seconds,submitted_fps,frame_ms,gpu_ms,texture_mb,layout_ms,render_ms,present_ms,p95_ms,max_ms,samples");
        if (!forceFrames)
        {
            _frameLog = new StreamWriter(path + ".frames.csv");
            _frameLog.WriteLine("utc,pixels,layout_ms,render_ms,present_ms,total_ms,dirty_elements");
        }
        _timer.Tick += OnTick;
        _timer.Start();
    }

    public static EffectPerformanceProbe? StartIfRequested()
    {
        string? scrollPath = Environment.GetEnvironmentVariable(ScrollLogKey)
            ?? NSBundle.MainBundle.ObjectForInfoDictionary(ScrollLogKey)?.ToString();
        if (!string.IsNullOrWhiteSpace(scrollPath)) return new EffectPerformanceProbe(scrollPath, forceFrames: false);
        string? path = Environment.GetEnvironmentVariable(LogKey)
            ?? NSBundle.MainBundle.ObjectForInfoDictionary(LogKey)?.ToString();
        return string.IsNullOrWhiteSpace(path) ? null : new EffectPerformanceProbe(path, forceFrames: true);
    }

    private void OnTick(object? sender, EventArgs args)
    {
        var window = Application.Current?.MainWindow;
        if (window?.RenderTarget is not { IsValid: true } target) return;
        if (_window != window)
        {
            if (_window != null) _window.Closed -= OnWindowClosed;
            _window = window;
            _window.Closed += OnWindowClosed;
            _previousFrames = window.FrameHistory.TotalFrames;
            _lastSampleTicks = 0;
            _reportStart = Stopwatch.GetTimestamp();
        }

        if (_forceFrames) window.ForceRenderFrame();
        long now = Stopwatch.GetTimestamp();
        double seconds = (now - _reportStart) / (double)Stopwatch.Frequency;
        if (seconds < 1) return;
        long frames = window.FrameHistory.TotalFrames - _previousFrames;
        int count = window.FrameHistory.CopyTo(_samples), sampled = 0;
        DateTime utcNow = DateTime.UtcNow;
        double frameMs = 0, layoutMs = 0, renderMs = 0, presentMs = 0;
        Span<double> durations = stackalloc double[FrameHistory.Capacity];
        for (int i = 0; i < count; ++i)
        {
            var sample = _samples[i];
            if (_frameLog is not null && sample.TimestampTicks > _lastSampleTicks)
            {
                var completed = utcNow.AddSeconds((sample.TimestampTicks - now) / (double)Stopwatch.Frequency);
                _frameLog.WriteLine(string.Create(CultureInfo.InvariantCulture,
                    $"{completed:O},{target.Width}x{target.Height},{sample.LayoutMs:F3},{sample.RenderMs:F3},{sample.PresentMs:F3},{sample.TotalMs:F3},{sample.DirtyElements}"));
                _lastSampleTicks = sample.TimestampTicks;
            }
            if (_samples[i].TimestampTicks >= _reportStart)
            {
                frameMs += _samples[i].TotalMs;
                layoutMs += _samples[i].LayoutMs;
                renderMs += _samples[i].RenderMs;
                presentMs += _samples[i].PresentMs;
                durations[sampled] = _samples[i].TotalMs;
                ++sampled;
            }
        }
        _frameLog?.Flush();
        var recent = durations[..sampled];
        recent.Sort();
        double p95 = sampled > 0 ? recent[(int)Math.Ceiling(sampled * 0.95) - 1] : 0;
        double max = sampled > 0 ? recent[^1] : 0;
        int divisor = Math.Max(sampled, 1);
        target.TryQueryGpuTiming(out var timing);
        target.TryQueryGpuStats(out var resources);
        _log.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"{DateTime.UtcNow:O},{target.Width}x{target.Height},{frames},{seconds:F3},{frames / seconds:F2},{frameMs / divisor:F3},{timing.TotalGpuNs / 1e6:F3},{resources.TextureBytes / 1048576.0:F2},{layoutMs / divisor:F3},{renderMs / divisor:F3},{presentMs / divisor:F3},{p95:F3},{max:F3},{sampled}"));
        _previousFrames = window.FrameHistory.TotalFrames;
        _reportStart = now;
    }

    private void OnWindowClosed(object? sender, EventArgs args) => Dispose();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _timer.Stop();
        _timer.Tick -= OnTick;
        if (_window != null) _window.Closed -= OnWindowClosed;
        _log.Dispose();
        _frameLog?.Dispose();
    }
}
