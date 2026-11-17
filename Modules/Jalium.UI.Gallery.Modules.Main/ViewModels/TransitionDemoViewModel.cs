using System.Windows.Input;
using Jalium.UI;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Media;
using Jalium.UI.Media.Animation;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Playback state and commands for the content-transition demo.</summary>
public sealed class TransitionDemoViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private static readonly SolidColorBrush[] ContentBrushes =
    [
        new(Color.FromArgb(255, 0, 120, 212)),
        new(Color.FromArgb(255, 16, 137, 62)),
        new(Color.FromArgb(255, 200, 50, 50))
    ];

    private static readonly CornerRadius[] ContentRadii =
    [
        new(16),
        new(60),
        new(8)
    ];

    private const string XamlCode = """
        <ComboBox ItemsSource="{Binding Modes}"
                  SelectedItem="{Binding SelectedMode, Mode=TwoWay}"/>
        <Button Content="Switch Content"
                Command="{Binding SwitchContentCommand}"/>
        <TransitioningContentControl TransitionMode="{Binding SelectedMode}">
            <Border Background="{Binding ContentBackground}">
                <TextBlock Text="{Binding ContentLetter}"/>
            </Border>
        </TransitioningContentControl>
        """;

    private const string CSharpCode = """
        // TransitionDemoViewModel.cs
        public sealed class TransitionDemoViewModel : ViewModelBase
        {
            public ICommand SwitchContentCommand { get; }
            public ICommand ToggleAutoPlayCommand { get; }

            // Mode, content index and playback are all bindable state.
        }
        """;

    private readonly DispatcherTimer _autoPlayTimer;
    private int _contentIndex;
    private int _autoPlayIndex;
    private bool _isAutoPlaying;
    private TransitionMode _selectedMode = TransitionMode.Crossfade;

    public TransitionDemoViewModel()
    {
        Modes = Enum.GetValues<TransitionMode>();
        SwitchContentCommand = new RelayCommand(SwitchContent);
        ToggleAutoPlayCommand = new RelayCommand(ToggleAutoPlay);
        _autoPlayTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1500)
        };
        _autoPlayTimer.Tick += OnAutoPlayTick;
    }

    public IReadOnlyList<TransitionMode> Modes { get; }

    public TransitionMode SelectedMode
    {
        get => _selectedMode;
        set
        {
            if (SetProperty(ref _selectedMode, value))
            {
                OnPropertyChanged(nameof(CurrentTransitionText));
            }
        }
    }

    public int ContentIndex
    {
        get => _contentIndex;
        private set
        {
            var normalized = Math.Abs(value % ContentBrushes.Length);
            if (!SetProperty(ref _contentIndex, normalized))
            {
                return;
            }

            OnPropertyChanged(nameof(ContentLetter));
            OnPropertyChanged(nameof(ContentTitle));
            OnPropertyChanged(nameof(ContentBackground));
            OnPropertyChanged(nameof(ContentCornerRadius));
        }
    }

    public bool IsAutoPlaying
    {
        get => _isAutoPlaying;
        private set
        {
            if (SetProperty(ref _isAutoPlaying, value))
            {
                OnPropertyChanged(nameof(AutoPlayButtonText));
            }
        }
    }

    public string ContentLetter => ((char)('A' + ContentIndex)).ToString();

    public string ContentTitle => $"Content Panel {ContentLetter}";

    public SolidColorBrush ContentBackground => ContentBrushes[ContentIndex];

    public CornerRadius ContentCornerRadius => ContentRadii[ContentIndex];

    public string AutoPlayButtonText =>
        IsAutoPlaying ? "Stop" : "Auto Play All";

    public string CurrentTransitionText =>
        IsAutoPlaying
            ? $"Current: {SelectedMode} ({_autoPlayIndex + 1}/{Modes.Count})"
            : $"Current: {SelectedMode}";

    public ICommand SwitchContentCommand { get; }

    public ICommand ToggleAutoPlayCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    public override void Destroy()
    {
        _autoPlayTimer.Stop();
        _autoPlayTimer.Tick -= OnAutoPlayTick;
        base.Destroy();
    }

    private void SwitchContent()
    {
        ContentIndex = (ContentIndex + 1) % ContentBrushes.Length;
    }

    private void ToggleAutoPlay()
    {
        if (IsAutoPlaying)
        {
            StopAutoPlay();
            return;
        }

        _autoPlayIndex = 0;
        IsAutoPlaying = true;
        _autoPlayTimer.Start();
        PlayNextTransition();
    }

    private void OnAutoPlayTick(object? sender, EventArgs e)
    {
        PlayNextTransition();
    }

    private void PlayNextTransition()
    {
        if (_autoPlayIndex >= Modes.Count)
        {
            StopAutoPlay();
            return;
        }

        SelectedMode = Modes[_autoPlayIndex];
        SwitchContent();
        OnPropertyChanged(nameof(CurrentTransitionText));
        _autoPlayIndex++;
    }

    private void StopAutoPlay()
    {
        _autoPlayTimer.Stop();
        IsAutoPlaying = false;
        OnPropertyChanged(nameof(CurrentTransitionText));
    }
}
