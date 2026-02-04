using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for MediaElementPage.jalxaml demonstrating media element functionality.
/// </summary>
public partial class MediaElementPage : Page
{
    public MediaElementPage()
    {
        InitializeComponent();

        // Set up event handlers for video controls
        if (PlayButton != null && VideoPlayer != null)
        {
            PlayButton.Click += (s, e) => VideoPlayer.Play();
        }

        if (PauseButton != null && VideoPlayer != null)
        {
            PauseButton.Click += (s, e) => VideoPlayer.Pause();
        }

        if (StopButton != null && VideoPlayer != null)
        {
            StopButton.Click += (s, e) => VideoPlayer.Stop();
        }

        // Set up volume slider
        if (VolumeSlider != null)
        {
            VolumeSlider.ValueChanged += OnVolumeChanged;
        }
    }

    private void OnVolumeChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (VolumeText != null)
        {
            VolumeText.Text = $"{(int)e.NewValue}%";
        }

        if (VideoPlayer != null)
        {
            VideoPlayer.Volume = e.NewValue / 100.0;
        }
    }
}
