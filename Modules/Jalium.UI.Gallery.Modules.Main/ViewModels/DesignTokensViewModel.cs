using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class DesignTokensViewModel
{
    private readonly IGalleryThemeService _themeService;
    private IReadOnlyList<ColorTokenSample> _colorTokens = [];

    public DesignTokensViewModel(
        IGalleryThemeService themeService)
    {
        _themeService = themeService;
        _themeService.ThemeChanged += OnThemeChanged;
        UseLightThemeCommand =
            new RelayCommand(() => SetTheme(false));
        UseDarkThemeCommand =
            new RelayCommand(() => SetTheme(true));
        RefreshThemeState();
    }

    public IReadOnlyList<ColorTokenSample> ColorTokens
    {
        get => _colorTokens;
        private set => SetProperty(ref _colorTokens, value);
    }

    public IReadOnlyList<TypeTokenSample> TypeTokens { get; } =
    [
        new("Display", 36, "Bold", "Compose interfaces"),
        new("Title", 28, "Bold", "Design tokens"),
        new("Heading", 18, "SemiBold", "Semantic roles"),
        new("Body", 14, "Regular", "Readable application copy"),
        new("Caption", 11, "Regular", "METADATA / LABEL")
    ];

    public IReadOnlyList<SpacingTokenSample> SpacingTokens { get; } =
    [
        new("Space 1", 4, "Micro gap"),
        new("Space 2", 8, "Related content"),
        new("Space 3", 12, "Compact control"),
        new("Space 4", 16, "Default inset"),
        new("Space 6", 24, "Section rhythm"),
        new("Space 8", 32, "Large separation")
    ];

    public IReadOnlyList<RadiusTokenSample> RadiusTokens { get; } =
    [
        new("Small", 4, "Tags and compact controls"),
        new("Medium", 8, "Buttons and fields"),
        new("Large", 12, "Cards"),
        new("XL", 16, "Major surfaces")
    ];

    public IReadOnlyList<MetricTokenSample> MotionTokens { get; } =
    [
        new("Fast", "120 ms", "Hover and press"),
        new("Standard", "200 ms", "State transitions"),
        new("Emphasis", "320 ms", "Panels and navigation")
    ];

    public string ActiveThemeLabel =>
        _themeService.IsDark
            ? "Previewing dark tokens"
            : "Previewing light tokens";

    public bool IsDarkTheme => _themeService.IsDark;

    public bool IsLightTheme => !IsDarkTheme;

    public ICommand UseLightThemeCommand { get; }

    public ICommand UseDarkThemeCommand { get; }

    public string TokenExample => """
        <ResourceDictionary>
            <SolidColorBrush x:Key="GalleryAccent" Color="#08948A" />
            <SolidColorBrush x:Key="GalleryCardBackground" Color="#FFFFFF" />
            <SolidColorBrush x:Key="GalleryTextPrimary" Color="#101827" />

            <CornerRadius x:Key="RadiusSmall">4</CornerRadius>
            <CornerRadius x:Key="RadiusMedium">8</CornerRadius>
            <Thickness x:Key="SpaceMedium">16</Thickness>
        </ResourceDictionary>
        """;

    public override void Destroy()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
        base.Destroy();
    }

    private void SetTheme(bool dark)
    {
        _themeService.SetDark(dark);
        RefreshThemeState();
    }

    private void OnThemeChanged(
        object? sender,
        EventArgs args) =>
        RefreshThemeState();

    private void RefreshThemeState()
    {
        ColorTokens =
        [
            Token("Accent", "Primary actions", GalleryTheme.AccentPrimary),
            Token("Accent Light", "Hover emphasis", GalleryTheme.AccentLight),
            Token("Surface", "Application canvas", GalleryTheme.BackgroundLight),
            Token("Card", "Raised content", GalleryTheme.BackgroundCard),
            Token("Card Inner", "Nested surface", GalleryTheme.BackgroundCardInner),
            Token("Text Primary", "High emphasis", GalleryTheme.TextPrimary),
            Token("Text Secondary", "Supporting copy", GalleryTheme.TextSecondary),
            Token("Border", "Default separation", GalleryTheme.BorderDefault)
        ];
        OnPropertyChanged(nameof(ActiveThemeLabel));
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(IsLightTheme));
    }

    private static ColorTokenSample Token(
        string name,
        string role,
        Color color) =>
        new(
            name,
            role,
            new SolidColorBrush(color),
            GalleryThemeTuner.ToHex(color));
}
