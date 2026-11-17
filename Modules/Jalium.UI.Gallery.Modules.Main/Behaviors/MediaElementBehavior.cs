using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class MediaElementBehavior
{
    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(MediaPlayerRequest),
            typeof(MediaElementBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static readonly DependencyProperty TrackProgressProperty =
        DependencyProperty.RegisterAttached(
            "TrackProgress",
            typeof(bool),
            typeof(MediaElementBehavior),
            new PropertyMetadata(false, OnTrackProgressChanged));

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.RegisterAttached(
            "Progress",
            typeof(double),
            typeof(MediaElementBehavior),
            new PropertyMetadata(0.0, OnProgressChanged));

    public static readonly DependencyProperty IsLoopingProperty =
        DependencyProperty.RegisterAttached(
            "IsLooping",
            typeof(bool),
            typeof(MediaElementBehavior),
            new PropertyMetadata(false, OnIsLoopingChanged));

    private static readonly DependencyProperty TimerProperty =
        DependencyProperty.RegisterAttached(
            "Timer",
            typeof(DispatcherTimer),
            typeof(MediaElementBehavior),
            new PropertyMetadata(null));

    private static readonly DependencyProperty PublishingProperty =
        DependencyProperty.RegisterAttached(
            "Publishing",
            typeof(bool),
            typeof(MediaElementBehavior),
            new PropertyMetadata(false));

    public static MediaPlayerRequest? GetRequest(DependencyObject element) =>
        element.GetValue(RequestProperty) as MediaPlayerRequest;

    public static void SetRequest(
        DependencyObject element,
        MediaPlayerRequest? value) =>
        element.SetValue(RequestProperty, value);

    public static bool GetTrackProgress(DependencyObject element) =>
        (bool)(element.GetValue(TrackProgressProperty) ?? false);

    public static void SetTrackProgress(
        DependencyObject element,
        bool value) =>
        element.SetValue(TrackProgressProperty, value);

    public static double GetProgress(DependencyObject element) =>
        (double)(element.GetValue(ProgressProperty) ?? 0.0);

    public static void SetProgress(
        DependencyObject element,
        double value) =>
        element.SetValue(ProgressProperty, value);

    public static bool GetIsLooping(DependencyObject element) =>
        (bool)(element.GetValue(IsLoopingProperty) ?? false);

    public static void SetIsLooping(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsLoopingProperty, value);

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not MediaElement player ||
            args.NewValue is not MediaPlayerRequest request)
        {
            return;
        }

        switch (request.Action)
        {
            case MediaPlayerAction.Play:
                player.Play();
                break;
            case MediaPlayerAction.Pause:
                player.Pause();
                break;
            case MediaPlayerAction.Stop:
                player.Stop();
                break;
        }
    }

    private static void OnTrackProgressChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not MediaElement player)
        {
            return;
        }

        if (player.GetValue(TimerProperty) is DispatcherTimer oldTimer)
        {
            oldTimer.Stop();
            player.ClearValue(TimerProperty);
        }

        if (args.NewValue is not true)
        {
            return;
        }

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        timer.Tick += (_, _) => PublishProgress(player);
        player.SetValue(TimerProperty, timer);
        player.Loaded += OnPlayerLoaded;
        player.Unloaded += OnPlayerUnloaded;
    }

    private static void OnPlayerLoaded(object? sender, RoutedEventArgs args)
    {
        if (sender is MediaElement player &&
            player.GetValue(TimerProperty) is DispatcherTimer timer)
        {
            timer.Start();
        }
    }

    private static void OnPlayerUnloaded(object? sender, RoutedEventArgs args)
    {
        if (sender is MediaElement player &&
            player.GetValue(TimerProperty) is DispatcherTimer timer)
        {
            timer.Stop();
        }
    }

    private static void PublishProgress(MediaElement player)
    {
        if (!player.NaturalDuration.HasTimeSpan ||
            player.NaturalDuration.TimeSpan <= TimeSpan.Zero)
        {
            return;
        }

        var value = Math.Clamp(
            player.Position.TotalSeconds /
            player.NaturalDuration.TimeSpan.TotalSeconds * 100,
            0,
            100);
        player.SetValue(PublishingProperty, true);
        player.SetCurrentValue(ProgressProperty, value);
        player.SetValue(PublishingProperty, false);
    }

    private static void OnProgressChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not MediaElement player ||
            player.GetValue(PublishingProperty) is true ||
            !player.NaturalDuration.HasTimeSpan ||
            player.NaturalDuration.TimeSpan <= TimeSpan.Zero)
        {
            return;
        }

        var percent = Math.Clamp((double)args.NewValue!, 0, 100);
        player.Position = TimeSpan.FromSeconds(
            player.NaturalDuration.TimeSpan.TotalSeconds *
            percent / 100);
    }

    private static void OnIsLoopingChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not MediaElement player)
        {
            return;
        }

        player.MediaEnded -= OnMediaEnded;
        if (args.NewValue is true)
        {
            player.MediaEnded += OnMediaEnded;
        }
    }

    private static void OnMediaEnded(object? sender, RoutedEventArgs args)
    {
        if (sender is MediaElement player)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }
    }
}
