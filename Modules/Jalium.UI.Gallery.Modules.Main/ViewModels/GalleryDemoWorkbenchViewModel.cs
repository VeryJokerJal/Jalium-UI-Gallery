using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>
/// Inspector, code-tab, theme, and viewport state for the component workbench.
/// The live UIElement remains in the View layer.
/// </summary>
public sealed class GalleryDemoWorkbenchViewModel
    : GalleryPageViewModelBase
{
    private readonly IGalleryThemeService _themeService;
    private string _pageTag = string.Empty;
    private string _title = "Component";
    private string _category = "Gallery";
    private string _description = string.Empty;
    private string _exampleMarkup = string.Empty;
    private string _exampleCSharp = string.Empty;
    private string _viewportMode = "Desktop";
    private bool _showingCode;
    private double _previewPadding = 18;
    private double _cornerRadius = 8;
    private double _previewOpacity = 1;
    private bool _isPreviewEnabled = true;
    private Color _accent;
    private Color _surface;
    private bool _hasCustomAccent;
    private bool _hasCustomSurface;
    private bool _useAccentPreviewBorder;
    private bool _syncingTheme;

    public GalleryDemoWorkbenchViewModel(
        IGalleryThemeService themeService)
    {
        _themeService = themeService;
        _themeService.ThemeChanged += OnThemeChanged;
        _accent = GalleryTheme.AccentPrimary;
        _surface = GalleryTheme.BackgroundCard;

        SetViewportCommand =
            new RelayCommand<string>(SetViewport);
        ShowPreviewCommand =
            new RelayCommand(() => ShowingCode = false);
        ShowCodeCommand =
            new RelayCommand(() => ShowingCode = true);
        UseLightThemeCommand =
            new RelayCommand(() => _themeService.SetDark(false));
        UseDarkThemeCommand =
            new RelayCommand(() => _themeService.SetDark(true));
        ResetCommand = new RelayCommand(Reset);
    }

    public string PageTag
    {
        get => _pageTag;
        private set => SetProperty(ref _pageTag, value);
    }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string Category
    {
        get => _category;
        private set
        {
            if (SetProperty(ref _category, value))
            {
                OnPropertyChanged(nameof(CategoryLabel));
            }
        }
    }

    public string CategoryLabel =>
        Category.ToUpperInvariant();

    public string Description
    {
        get => _description;
        private set => SetProperty(ref _description, value);
    }

    public string ExampleMarkup
    {
        get => _exampleMarkup;
        private set => SetProperty(ref _exampleMarkup, value);
    }

    public string ExampleCSharp
    {
        get => _exampleCSharp;
        private set => SetProperty(ref _exampleCSharp, value);
    }

    public string ViewportMode
    {
        get => _viewportMode;
        private set
        {
            if (SetProperty(ref _viewportMode, value))
            {
                OnPropertyChanged(nameof(PreviewMaxWidth));
                OnPropertyChanged(nameof(ViewportStatus));
            }
        }
    }

    public bool ShowingCode
    {
        get => _showingCode;
        private set
        {
            if (SetProperty(ref _showingCode, value))
            {
                OnPropertyChanged(nameof(ShowingPreview));
            }
        }
    }

    public bool ShowingPreview => !ShowingCode;

    public double PreviewMaxWidth => ViewportMode switch
    {
        "Mobile" => 420,
        "Tablet" => 760,
        _ => 1600
    };

    public string ViewportStatus =>
        $"{ViewportMode} · max {PreviewMaxWidth:0} px";

    public double PreviewPadding
    {
        get => _previewPadding;
        set
        {
            if (SetProperty(ref _previewPadding, value))
            {
                OnPropertyChanged(nameof(PreviewPaddingThickness));
            }
        }
    }

    public double CornerRadius
    {
        get => _cornerRadius;
        set
        {
            if (SetProperty(ref _cornerRadius, value))
            {
                OnPropertyChanged(nameof(PreviewCornerRadius));
            }
        }
    }

    public double PreviewOpacity
    {
        get => _previewOpacity;
        set
        {
            if (SetProperty(ref _previewOpacity, value))
            {
                OnPropertyChanged(nameof(PreviewOpacityPercent));
            }
        }
    }

    public double PreviewOpacityPercent
    {
        get => PreviewOpacity * 100;
        set => PreviewOpacity = Math.Clamp(value, 0, 100) / 100;
    }

    public bool IsPreviewEnabled
    {
        get => _isPreviewEnabled;
        set => SetProperty(ref _isPreviewEnabled, value);
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
                    _hasCustomAccent = true;
                    _useAccentPreviewBorder = true;
                    _themeService.SetAccent(value);
                }

                OnPropertyChanged(nameof(AccentBrush));
                OnPropertyChanged(nameof(AccentHex));
                OnPropertyChanged(nameof(PreviewBorderBrush));
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
                    _hasCustomSurface = true;
                }

                OnPropertyChanged(nameof(SurfaceBrush));
                OnPropertyChanged(nameof(SurfaceHex));
            }
        }
    }

    public Brush AccentBrush => new SolidColorBrush(Accent);

    public Brush PreviewBorderBrush =>
        _useAccentPreviewBorder
            ? AccentBrush
            : GalleryTheme.BorderDefaultBrush;

    public Brush SurfaceBrush => new SolidColorBrush(Surface);

    public string AccentHex =>
        GalleryThemeTuner.ToHex(Accent);

    public string SurfaceHex =>
        GalleryThemeTuner.ToHex(Surface);

    public Thickness PreviewPaddingThickness =>
        new(PreviewPadding);

    public CornerRadius PreviewCornerRadius =>
        new(CornerRadius);

    public string ActiveThemeLabel =>
        _themeService.IsDark ? "Dark theme" : "Light theme";

    public string ThemeMode =>
        _themeService.IsDark ? "Dark" : "Light";

    public ICommand SetViewportCommand { get; }

    public ICommand ShowPreviewCommand { get; }

    public ICommand ShowCodeCommand { get; }

    public ICommand UseLightThemeCommand { get; }

    public ICommand UseDarkThemeCommand { get; }

    public ICommand ResetCommand { get; }

    public void Initialize(GalleryWorkbenchDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        PageTag = definition.PageTag;
        Title = definition.Title;
        Category = definition.Category;
        Description = definition.Description;
        ExampleMarkup = definition.ExampleMarkup;
        ExampleCSharp = definition.ExampleCSharp;
    }

    public override void Destroy()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
        base.Destroy();
    }

    private void SetViewport(string? viewport)
    {
        if (viewport is "Desktop" or "Tablet" or "Mobile")
        {
            ViewportMode = viewport;
        }
    }

    private void Reset()
    {
        _themeService.ResetTuning();
        _hasCustomAccent = false;
        _hasCustomSurface = false;
        _useAccentPreviewBorder = true;
        SyncThemeDefaults(force: true);
        OnPropertyChanged(nameof(PreviewBorderBrush));
        PreviewPadding = 18;
        CornerRadius = 8;
        PreviewOpacity = 1;
        IsPreviewEnabled = true;
        ViewportMode = "Desktop";
        ShowingCode = false;
        Status = "Workbench settings reset.";
    }

    private void OnThemeChanged(
        object? sender,
        EventArgs args)
    {
        SyncThemeDefaults(force: false);
        OnPropertyChanged(nameof(ActiveThemeLabel));
        OnPropertyChanged(nameof(ThemeMode));
    }

    private void SyncThemeDefaults(bool force)
    {
        _syncingTheme = true;
        try
        {
            if (force || !_hasCustomAccent)
            {
                Accent = GalleryTheme.AccentPrimary;
            }

            if (force || !_hasCustomSurface)
            {
                Surface = GalleryTheme.BackgroundCard;
            }
        }
        finally
        {
            _syncingTheme = false;
        }
    }
}
