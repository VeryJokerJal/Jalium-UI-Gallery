using Jalium.UI;
using Jalium.UI.Threading;
using System.Runtime.CompilerServices;

namespace Jalium.UI.Gallery.Modules.Main.Support;

/// <summary>
/// Lightweight opacity / numeric tweening helper driven by a single
/// <see cref="DispatcherTimer"/>. The Gallery uses it for the Overview → Main
/// shell transition and the startup fade-in. Ease-out cubic is the default
/// easing because it matches the perceptual "settle" motion the WinUI
/// NavigationView transitions ship with.
/// </summary>
internal static class FadeAnimator
{
    private const double FrameIntervalMs = 16.0;
    private static readonly ConditionalWeakTable<UIElement, FadeState>
        ActiveFades = new();

    private sealed class FadeState(DispatcherTimer timer)
    {
        public DispatcherTimer Timer { get; } = timer;
    }

    /// <summary>
    /// Linearly tweens <see cref="UIElement.Opacity"/> from <paramref name="from"/>
    /// to <paramref name="to"/> over <paramref name="duration"/> using an
    /// ease-out cubic curve. Invokes <paramref name="completed"/> on the
    /// dispatcher thread once the final frame is rendered. Zero or negative
    /// durations snap to the final value synchronously.
    /// </summary>
    public static void Fade(
        UIElement element,
        double from,
        double to,
        TimeSpan duration,
        Action? completed = null)
    {
        ArgumentNullException.ThrowIfNull(element);

        var effectiveFrom = from;
        if (ActiveFades.TryGetValue(element, out var activeFade))
        {
            effectiveFrom = element.Opacity;
            activeFade.Timer.Stop();
            ActiveFades.Remove(element);
        }

        if (duration.TotalMilliseconds <= 0)
        {
            element.Opacity = to;
            completed?.Invoke();
            return;
        }

        element.Opacity = effectiveFrom;

        var totalMs = duration.TotalMilliseconds;
        var start = DateTime.UtcNow;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(FrameIntervalMs) };
        ActiveFades.Add(element, new FadeState(timer));

        timer.Tick += (_, _) =>
        {
            var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
            var t = Math.Clamp(elapsed / totalMs, 0.0, 1.0);
            var eased = EaseOutCubic(t);
            element.Opacity = effectiveFrom + ((to - effectiveFrom) * eased);

            if (t >= 1.0)
            {
                element.Opacity = to;
                timer.Stop();
                ActiveFades.Remove(element);
                completed?.Invoke();
            }
        };

        timer.Start();
    }

    /// <summary>
    /// Schedules <paramref name="action"/> on the dispatcher after
    /// <paramref name="delay"/> elapses. Used to chain animation stages
    /// without blocking the UI thread.
    /// </summary>
    public static void After(TimeSpan delay, Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (delay.TotalMilliseconds <= 0)
        {
            action();
            return;
        }

        var timer = new DispatcherTimer { Interval = delay };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            action();
        };
        timer.Start();
    }

    private static double EaseOutCubic(double t)
    {
        var inverted = 1.0 - t;
        return 1.0 - (inverted * inverted * inverted);
    }
}
