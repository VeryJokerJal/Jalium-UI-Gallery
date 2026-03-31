using Jalium.UI.Controls;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for MediaElementPage.jalxaml demonstrating media element functionality.
/// </summary>
public partial class MediaElementPage : Page
{
    public MediaElementPage()
    {
        InitializeComponent();
        
        // Video controls
        if (PlayButton != null && VideoPlayer != null)
            PlayButton.Click += (s, e) => VideoPlayer.Play();

        if (PauseButton != null && VideoPlayer != null)
            PauseButton.Click += (s, e) => VideoPlayer.Pause();

        if (StopButton != null && VideoPlayer != null)
            StopButton.Click += (s, e) => VideoPlayer.Stop();

        // Video file loading
        if (VideoLoadButton != null)
            VideoLoadButton.Click += OnVideoLoadClick;

        if (VideoBrowseButton != null)
            VideoBrowseButton.Click += OnVideoBrowseClick;

        if (VideoPathTextBox != null)
            VideoPathTextBox.KeyDown += (s, e) =>
            {
                if (e is KeyEventArgs ke && ke.Key == Key.Enter)
                    LoadVideoFromPath();
            };

        // Audio file loading
        if (AudioLoadButton != null)
            AudioLoadButton.Click += OnAudioLoadClick;

        if (AudioBrowseButton != null)
            AudioBrowseButton.Click += OnAudioBrowseClick;

        if (AudioPathTextBox != null)
            AudioPathTextBox.KeyDown += (s, e) =>
            {
                if (e is KeyEventArgs ke && ke.Key == Key.Enter)
                    LoadAudioFromPath();
            };

        // Audio playback controls
        if (AudioPlayPauseButton != null && AudioPlayer != null)
            AudioPlayPauseButton.Click += (s, e) => AudioPlayer.Play();

        if (AudioPlayButton != null && AudioPlayer != null)
            AudioPlayButton.Click += (s, e) => AudioPlayer.Stop();

        if (AudioStopButton != null && AudioPlayer != null)
            AudioStopButton.Click += (s, e) => AudioPlayer.Stop();

        // Volume slider
        if (VolumeSlider != null)
            VolumeSlider.ValueChanged += OnVolumeChanged;
    }

    private void OnVideoLoadClick(object? sender, EventArgs e)
    {
        LoadVideoFromPath();
    }

    private void OnVideoBrowseClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.avi;*.wmv;*.mov;*.webm;*.mkv|All Files|*.*",
            Title = "Select Video File"
        };

        if (dialog.ShowDialog() == true)
        {
            if (VideoPathTextBox != null)
                VideoPathTextBox.Text = dialog.FileName;
            LoadVideoFromPath();
        }
    }

    private void LoadVideoFromPath()
    {
        if (VideoPlayer == null || VideoPathTextBox == null) return;

        var path = VideoPathTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            if (path.StartsWith("http://") || path.StartsWith("https://"))
                VideoPlayer.Source = new Uri(path);
            else
                VideoPlayer.Source = new Uri(System.IO.Path.GetFullPath(path));

            VideoPlayer.Play();
        }
        catch
        {
            // Invalid path
        }
    }

    private void OnAudioLoadClick(object? sender, EventArgs e)
    {
        LoadAudioFromPath();
    }

    private void OnAudioBrowseClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Audio Files|*.mp3;*.wav;*.aac;*.ogg;*.flac;*.wma|All Files|*.*",
            Title = "Select Audio File"
        };

        if (dialog.ShowDialog() == true)
        {
            if (AudioPathTextBox != null)
                AudioPathTextBox.Text = dialog.FileName;
            LoadAudioFromPath();
        }
    }

    private void LoadAudioFromPath()
    {
        if (AudioPlayer == null || AudioPathTextBox == null) return;

        var path = AudioPathTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            if (path.StartsWith("http://") || path.StartsWith("https://"))
                AudioPlayer.Source = new Uri(path);
            else
                AudioPlayer.Source = new Uri(System.IO.Path.GetFullPath(path));

            // Update display
            if (AudioTitleText != null)
                AudioTitleText.Text = System.IO.Path.GetFileNameWithoutExtension(path);
            if (AudioArtistText != null)
                AudioArtistText.Text = System.IO.Path.GetFileName(path);

            AudioPlayer.Play();
        }
        catch
        {
            // Invalid path
        }
    }

    private void OnVolumeChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (VolumeText != null)
            VolumeText.Text = $"{(int)e.NewValue}%";

        if (VideoPlayer != null)
            VideoPlayer.Volume = e.NewValue / 100.0;

        if (AudioPlayer != null)
            AudioPlayer.Volume = e.NewValue / 100.0;
    }
}
