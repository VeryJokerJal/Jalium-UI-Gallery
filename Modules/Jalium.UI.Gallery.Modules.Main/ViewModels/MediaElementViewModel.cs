using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class MediaElementViewModel
{
    private readonly IGalleryMediaFilePicker _filePicker;
    private long _requestSequence;
    private string _videoPath = string.Empty;
    private string _audioPath = string.Empty;
    private Uri? _videoSource;
    private Uri? _audioSource;
    private string _audioTitle = "Audio Track";
    private string _audioArtist = "No file loaded";
    private double _volumePercent = 75;
    private double _speedRatio = 1;
    private double _audioProgress;
    private bool _isMuted;
    private bool _isLooping;
    private bool _isAutoPlayEnabled;
    private MediaPlayerRequest? _videoRequest;
    private MediaPlayerRequest? _audioRequest;

    public MediaElementViewModel(IGalleryMediaFilePicker filePicker)
    {
        _filePicker = filePicker;
        SpeedOptions = [0.5, 0.75, 1.0, 1.25, 1.5, 2.0];
        LoadVideoCommand = new RelayCommand(LoadVideo);
        BrowseVideoCommand = new RelayCommand(BrowseVideo);
        PlayVideoCommand = Video(MediaPlayerAction.Play);
        PauseVideoCommand = Video(MediaPlayerAction.Pause);
        StopVideoCommand = Video(MediaPlayerAction.Stop);
        LoadAudioCommand = new RelayCommand(LoadAudio);
        BrowseAudioCommand = new RelayCommand(BrowseAudio);
        PlayAudioCommand = Audio(MediaPlayerAction.Play);
        PauseAudioCommand = Audio(MediaPlayerAction.Pause);
        StopAudioCommand = Audio(MediaPlayerAction.Stop);
    }

    public IReadOnlyList<double> SpeedOptions { get; }

    public ICommand LoadVideoCommand { get; }

    public ICommand BrowseVideoCommand { get; }

    public ICommand PlayVideoCommand { get; }

    public ICommand PauseVideoCommand { get; }

    public ICommand StopVideoCommand { get; }

    public ICommand LoadAudioCommand { get; }

    public ICommand BrowseAudioCommand { get; }

    public ICommand PlayAudioCommand { get; }

    public ICommand PauseAudioCommand { get; }

    public ICommand StopAudioCommand { get; }

    public string VideoPath
    {
        get => _videoPath;
        set => SetProperty(ref _videoPath, value);
    }

    public string AudioPath
    {
        get => _audioPath;
        set => SetProperty(ref _audioPath, value);
    }

    public Uri? VideoSource
    {
        get => _videoSource;
        private set => SetProperty(ref _videoSource, value);
    }

    public Uri? AudioSource
    {
        get => _audioSource;
        private set => SetProperty(ref _audioSource, value);
    }

    public string AudioTitle
    {
        get => _audioTitle;
        private set => SetProperty(ref _audioTitle, value);
    }

    public string AudioArtist
    {
        get => _audioArtist;
        private set => SetProperty(ref _audioArtist, value);
    }

    public double VolumePercent
    {
        get => _volumePercent;
        set
        {
            if (SetProperty(ref _volumePercent, value))
            {
                OnPropertyChanged(nameof(Volume));
            }
        }
    }

    public double Volume => VolumePercent / 100;

    public double SpeedRatio
    {
        get => _speedRatio;
        set => SetProperty(ref _speedRatio, value);
    }

    public double AudioProgress
    {
        get => _audioProgress;
        set => SetProperty(ref _audioProgress, value);
    }

    public bool IsMuted
    {
        get => _isMuted;
        set => SetProperty(ref _isMuted, value);
    }

    public bool IsLooping
    {
        get => _isLooping;
        set => SetProperty(ref _isLooping, value);
    }

    public bool IsAutoPlayEnabled
    {
        get => _isAutoPlayEnabled;
        set => SetProperty(ref _isAutoPlayEnabled, value);
    }

    public MediaPlayerRequest? VideoRequest
    {
        get => _videoRequest;
        private set => SetProperty(ref _videoRequest, value);
    }

    public MediaPlayerRequest? AudioRequest
    {
        get => _audioRequest;
        private set => SetProperty(ref _audioRequest, value);
    }

    private ICommand Video(MediaPlayerAction action) =>
        new RelayCommand(() =>
            VideoRequest = NewRequest(action));

    private ICommand Audio(MediaPlayerAction action) =>
        new RelayCommand(() =>
            AudioRequest = NewRequest(action));

    private MediaPlayerRequest NewRequest(MediaPlayerAction action) =>
        new(++_requestSequence, action);

    private void BrowseVideo()
    {
        if (_filePicker.PickVideo() is not { } path)
        {
            return;
        }

        VideoPath = path;
        LoadVideo();
    }

    private void BrowseAudio()
    {
        if (_filePicker.PickAudio() is not { } path)
        {
            return;
        }

        AudioPath = path;
        LoadAudio();
    }

    private void LoadVideo()
    {
        if (TryResolveSource(VideoPath, out var source))
        {
            VideoSource = source;
            if (IsAutoPlayEnabled)
            {
                VideoRequest = NewRequest(MediaPlayerAction.Play);
            }
        }
    }

    private void LoadAudio()
    {
        if (!TryResolveSource(AudioPath, out var source))
        {
            return;
        }

        AudioSource = source;
        AudioTitle = Path.GetFileNameWithoutExtension(AudioPath);
        AudioArtist = Path.GetFileName(AudioPath);
        if (IsAutoPlayEnabled)
        {
            AudioRequest = NewRequest(MediaPlayerAction.Play);
        }
    }

    private static bool TryResolveSource(
        string path,
        out Uri? source)
    {
        source = null;
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        try
        {
            source = Uri.TryCreate(
                path.Trim(),
                UriKind.Absolute,
                out var absolute)
                ? absolute
                : new Uri(Path.GetFullPath(path.Trim()));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
