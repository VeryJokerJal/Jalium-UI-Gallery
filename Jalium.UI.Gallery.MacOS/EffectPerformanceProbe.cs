using System.Diagnostics;
using System.Globalization;
using Foundation;
using Jalium.UI.Controls;
using Jalium.UI.Diagnostics;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.MacOS;

/// <summary>
/// Opt-in whole-Gallery measurement. The 16 ms timer requests full frames so
/// a static effect page can be measured without including idle time in FPS.
/// Normal Gallery runs do not construct the timer or write a report.
/// </summary>
internal sealed class EffectPerformanceProbe : IDisposable
{
    internal const string LogKey = "JALIUM_GALLERY_EFFECT_PERF_LOG";
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(16) };
    private readonly StreamWriter _log;
    private readonly FrameHistory.Sample[] _samples = new FrameHistory.Sample[FrameHistory.Capacity];
    private Window? _window;
    private long _reportStart;
    private long _previousFrames;
    private bool _disposed;

    private EffectPerformanceProbe(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        _log = new StreamWriter(path) { AutoFlush = true };
        _log.WriteLine("utc,pixels,frames,seconds,submitted_fps,frame_ms,gpu_ms,texture_mb");
        _timer.Tick += OnTick;
        _timer.Start();
    }

    public static EffectPerformanceProbe? StartIfRequested()
    {
        string? path = Environment.GetEnvironmentVariable(LogKey)
            ?? NSBundle.MainBundle.ObjectForInfoDictionary(LogKey)?.ToString();
        return string.IsNullOrWhiteSpace(path) ? null : new EffectPerformanceProbe(path);
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
            _reportStart = Stopwatch.GetTimestamp();
        }

        window.ForceRenderFrame();
        long now = Stopwatch.GetTimestamp();
        double seconds = (now - _reportStart) / (double)Stopwatch.Frequency;
        if (seconds < 1) return;
        long frames = window.FrameHistory.TotalFrames - _previousFrames;
        int count = window.FrameHistory.CopyTo(_samples), sampled = 0;
        double frameMs = 0;
        for (int i = 0; i < count; ++i)
            if (_samples[i].TimestampTicks >= _reportStart)
            {
                frameMs += _samples[i].TotalMs;
                ++sampled;
            }
        target.TryQueryGpuTiming(out var timing);
        target.TryQueryGpuStats(out var resources);
        _log.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"{DateTime.UtcNow:O},{target.Width}x{target.Height},{frames},{seconds:F3},{frames / seconds:F2},{frameMs / Math.Max(sampled, 1):F3},{timing.TotalGpuNs / 1e6:F3},{resources.TextureBytes / 1048576.0:F2}"));
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
    }
}
