using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Input;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for MediaElementPage.jalxaml demonstrating media element functionality.
/// </summary>
public partial class MediaElementPage : Page
{
    private DispatcherTimer? _audioProgressTimer;
    private bool _audioSliderDragging;

    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Video Player -->
    <Border Background=""#000000"" Width=""480"" Height=""270""
            CornerRadius=""4"" Margin=""0,0,0,16"">
        <MediaElement x:Name=""VideoPlayer""
                      Width=""480"" Height=""270""/>
    </Border>

    <!-- Playback Controls -->
    <StackPanel Orientation=""Horizontal"" HorizontalAlignment=""Center"">
        <Button x:Name=""PlayButton"" Content=""Play""
                Width=""70"" Height=""28"" Margin=""0,0,8,0""/>
        <Button x:Name=""PauseButton"" Content=""Pause""
                Width=""70"" Height=""28"" Margin=""0,0,8,0""/>
        <Button x:Name=""StopButton"" Content=""Stop""
                Width=""70"" Height=""28""/>
    </StackPanel>

    <!-- Audio Player (hidden MediaElement) -->
    <MediaElement x:Name=""AudioPlayer"" Width=""0"" Height=""0""/>

    <!-- Volume Control -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,16,0,0"">
        <TextBlock Text=""Volume:"" Width=""100""
                   Foreground=""#888888"" VerticalAlignment=""Center""/>
        <Slider x:Name=""VolumeSlider""
                Minimum=""0"" Maximum=""100""
                Value=""75"" Width=""200""/>
    </StackPanel>

    <!-- Playback Options -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,12,0,0"">
        <CheckBox Content=""Loop"" Margin=""0,0,16,0""/>
        <CheckBox Content=""Mute"" Margin=""0,0,16,0""/>
        <CheckBox Content=""Auto-play""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class MediaElementSample : Page
{
    public MediaElementSample()
    {
        InitializeComponent();

        // Wire up playback controls
        PlayButton.Click += (s, e) => VideoPlayer.Play();
        PauseButton.Click += (s, e) => VideoPlayer.Pause();
        StopButton.Click += (s, e) => VideoPlayer.Stop();

        // Volume control
        VolumeSlider.ValueChanged += (s, e) =>
        {
            VideoPlayer.Volume = e.NewValue / 100.0;
            AudioPlayer.Volume = e.NewValue / 100.0;
        };
    }

    private void LoadVideo(string path)
    {
        // Load from file path or URL
        if (path.StartsWith(""http""))
            VideoPlayer.Source = new Uri(path);
        else
            VideoPlayer.Source = new Uri(
                System.IO.Path.GetFullPath(path));

        VideoPlayer.Play();
    }

    private void LoadAudio(string path)
    {
        if (path.StartsWith(""http""))
            AudioPlayer.Source = new Uri(path);
        else
            AudioPlayer.Source = new Uri(
                System.IO.Path.GetFullPath(path));

        AudioPlayer.Play();
    }

    // Browse for video file using OpenFileDialog
    private void OnBrowseClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = ""Video|*.mp4;*.avi;*.wmv;*.mov|All|*.*"",
            Title = ""Select Video File""
        };

        if (dialog.ShowDialog() == true)
            LoadVideo(dialog.FileName);
    }
}";

    public MediaElementPage()
    {
        InitializeComponent();
        LoadCodeExamples();

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

        // Audio progress slider: 250ms tick 拉 AudioPlayer.Position 同步 slider 值;
        // 用户拖动时(_audioSliderDragging)暂停 tick 写入,避免 tick 把拖动覆盖回去。
        // 拖动结束(MouseUp / Lost capture)调 AudioPlayer.Position 反向 seek。
        if (AudioProgressSlider != null && AudioPlayer != null)
        {
            _audioProgressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250),
            };
            _audioProgressTimer.Tick += (_, _) => UpdateAudioProgressSlider();
            _audioProgressTimer.Start();

            AudioProgressSlider.PreviewMouseLeftButtonDown += (_, _) => _audioSliderDragging = true;
            AudioProgressSlider.PreviewMouseLeftButtonUp   += (_, _) =>
            {
                _audioSliderDragging = false;
                ApplyAudioSliderSeek();
            };
        }
    }

    private void UpdateAudioProgressSlider()
    {
        if (AudioPlayer == null || AudioProgressSlider == null || _audioSliderDragging) return;
        var nd = AudioPlayer.NaturalDuration;
        if (!nd.HasTimeSpan) return;
        var duration = nd.TimeSpan;
        if (duration <= TimeSpan.Zero) return;
        var position = AudioPlayer.Position;
        var pct = Math.Clamp(position.TotalSeconds / duration.TotalSeconds * 100.0, 0.0, 100.0);
        AudioProgressSlider.Value = pct;
    }

    private void ApplyAudioSliderSeek()
    {
        if (AudioPlayer == null || AudioProgressSlider == null) return;
        var nd = AudioPlayer.NaturalDuration;
        if (!nd.HasTimeSpan) return;
        var duration = nd.TimeSpan;
        if (duration <= TimeSpan.Zero) return;
        var target = TimeSpan.FromSeconds(duration.TotalSeconds * (AudioProgressSlider.Value / 100.0));
        AudioPlayer.Position = target;
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
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
