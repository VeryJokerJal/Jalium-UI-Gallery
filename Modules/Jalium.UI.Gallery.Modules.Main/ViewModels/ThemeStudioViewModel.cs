using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Editable theme state for the declarative Theme Studio view.</summary>
public sealed class ThemeStudioViewModel : GalleryPageViewModelBase
{
    private readonly IGalleryThemeService _themeService;
    private Color _accent;
    private Color _surface;
    private bool _hasCustomAccent;
    private bool _hasCustomSurface;
    private double _radius = 14;
    private double _spacing = 18;
    private double _previewOpacity = 1;
    private bool _isPreviewEnabled = true;
    private bool _syncingTheme;

    public ThemeStudioViewModel(
        IGalleryThemeService themeService)
    {
        _themeService = themeService;
        _themeService.ThemeChanged += OnThemeChanged;
        _accent = GalleryTheme.AccentPrimary;
        _surface = GalleryTheme.BackgroundCard;

        UseLightThemeCommand =
            new RelayCommand(() => _themeService.SetDark(false));
        UseDarkThemeCommand =
            new RelayCommand(() => _themeService.SetDark(true));
        ResetCommand = new RelayCommand(ResetPalette);
    }

    public Color Accent
    {
        get => _accent;
        set
        {
            if (SetProperty(ref _accent, value))
            {
                if (!_syncingTheme)
                {
                    HasCustomAccent = true;
                    _themeService.SetAccent(value);
                }

                NotifyPreviewProperties();
            }
        }
    }

    public Color Surface
    {
        get => _surface;
        set
        {
            if (SetProperty(ref _surface, value))
            {
                if (!_syncingTheme)
                {
                    HasCustomSurface = true;
                    _themeService.SetSurface(value);
                }

                NotifyPreviewProperties();
            }
        }
    }

    public bool HasCustomAccent
    {
        get => _hasCustomAccent;
        private set => SetProperty(ref _hasCustomAccent, value);
    }

    public bool HasCustomSurface
    {
        get => _hasCustomSurface;
        private set => SetProperty(ref _hasCustomSurface, value);
    }

    public double Radius
    {
        get => _radius;
        set
        {
            if (SetProperty(ref _radius, value))
            {
                OnPropertyChanged(nameof(PreviewCornerRadius));
                OnPropertyChanged(nameof(TokenCode));
            }
        }
    }

    public double Spacing
    {
        get => _spacing;
        set
        {
            if (SetProperty(ref _spacing, value))
            {
                OnPropertyChanged(nameof(PreviewPadding));
                OnPropertyChanged(nameof(TokenCode));
            }
        }
    }

    public double PreviewOpacity
    {
        get => _previewOpacity;
        set => SetProperty(ref _previewOpacity, value);
    }

    public bool IsPreviewEnabled
    {
        get => _isPreviewEnabled;
        set => SetProperty(ref _isPreviewEnabled, value);
    }

    public Brush AccentBrush => new SolidColorBrush(Accent);

    public Brush SurfaceBrush => new SolidColorBrush(Surface);

    public CornerRadius PreviewCornerRadius => new(Radius);

    public Thickness PreviewPadding => new(Spacing);

    public string AccentHex =>
        GalleryThemeTuner.ToHex(Accent);

    public string SurfaceHex =>
        GalleryThemeTuner.ToHex(Surface);

    public string ActiveThemeLabel =>
        _themeService.IsDark ? "Dark" : "Light";

    public bool IsDarkTheme => _themeService.IsDark;

    public bool IsLightTheme => !IsDarkTheme;

    public string TokenCode => $$"""
        <ResourceDictionary>
            <SolidColorBrush x:Key="GalleryAccent"
                             Color="{{AccentHex}}" />
            <SolidColorBrush x:Key="GalleryCardBackground"
                             Color="{{SurfaceHex}}" />
            <CornerRadius x:Key="GalleryRadius">{{Radius:0}}</CornerRadius>
            <Thickness x:Key="GallerySpacing">{{Spacing:0}}</Thickness>
        </ResourceDictionary>
        """;

    public ICommand UseLightThemeCommand { get; }

    public ICommand UseDarkThemeCommand { get; }

    public ICommand ResetCommand { get; }

    public void ResetPalette()
    {
        HasCustomAccent = false;
        HasCustomSurface = false;
        SyncThemeDefaults(force: true);
        Radius = 14;
        Spacing = 18;
        PreviewOpacity = 1;
        IsPreviewEnabled = true;
        Status = "Theme Studio values reset.";
    }

    public override void Destroy()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
        base.Destroy();
    }

    private void OnThemeChanged(
        object? sender,
        EventArgs args)
    {
        SyncThemeDefaults(force: false);
        OnPropertyChanged(nameof(ActiveThemeLabel));
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(IsLightTheme));
    }

    private void SyncThemeDefaults(bool force)
    {
        _syncingTheme = true;
        try
        {
            if (force || !HasCustomAccent)
            {
                Accent = GalleryTheme.AccentPrimary;
            }

            if (force || !HasCustomSurface)
            {
                Surface = GalleryTheme.BackgroundCard;
            }
        }
        finally
        {
            _syncingTheme = false;
        }
    }

    private void NotifyPreviewProperties()
    {
        OnPropertyChanged(nameof(AccentBrush));
        OnPropertyChanged(nameof(SurfaceBrush));
        OnPropertyChanged(nameof(AccentHex));
        OnPropertyChanged(nameof(SurfaceHex));
        OnPropertyChanged(nameof(TokenCode));
    }

}
