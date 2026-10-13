using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed class ThemeColorsViewModel
    : GalleryPageViewModelBase
{
    private readonly IGalleryThemeResourceService _resourceService;
    private readonly IGalleryClipboardService _clipboardService;
    private IReadOnlyList<ThemeResourceEntry> _allEntries = [];
    private IReadOnlyList<ThemeResourceRowViewModel> _resources = [];
    private string _previewThemeKey;
    private string _filterText = string.Empty;
    private bool _showColors;
    private bool _showBrushes;
    private bool _showGradients;
    private string _countText = "0 keys";
    private string _emptyMessage =
        "No theme color resources found.";
    private bool _isCopyBannerVisible;
    private string _copyBannerText = string.Empty;

    public ThemeColorsViewModel(
        IGalleryThemeResourceService resourceService,
        IGalleryClipboardService clipboardService)
    {
        _resourceService = resourceService;
        _clipboardService = clipboardService;
        _previewThemeKey = resourceService.CurrentThemeKey;
        Reload();
    }

    public IReadOnlyList<string> ThemeOptions { get; } =
    [
        "Dark",
        "Light",
        "HighContrast"
    ];

    public IReadOnlyList<ThemeResourceRowViewModel> Resources
    {
        get => _resources;
        private set
        {
            if (SetProperty(ref _resources, value))
            {
                OnPropertyChanged(nameof(HasResources));
                OnPropertyChanged(nameof(HasNoResources));
            }
        }
    }

    public string PreviewThemeKey
    {
        get => _previewThemeKey;
        set
        {
            if (SetProperty(ref _previewThemeKey, value))
            {
                Reload();
            }
        }
    }

    public string FilterText
    {
        get => _filterText;
        set
        {
            if (SetProperty(ref _filterText, value ?? string.Empty))
            {
                ApplyFilter();
            }
        }
    }

    public bool ShowColors
    {
        get => _showColors;
        set
        {
            if (SetProperty(ref _showColors, value))
            {
                ApplyFilter();
            }
        }
    }

    public bool ShowBrushes
    {
        get => _showBrushes;
        set
        {
            if (SetProperty(ref _showBrushes, value))
            {
                ApplyFilter();
            }
        }
    }

    public bool ShowGradients
    {
        get => _showGradients;
        set
        {
            if (SetProperty(ref _showGradients, value))
            {
                ApplyFilter();
            }
        }
    }

    public string CountText
    {
        get => _countText;
        private set => SetProperty(ref _countText, value);
    }

    public string EmptyMessage
    {
        get => _emptyMessage;
        private set => SetProperty(ref _emptyMessage, value);
    }

    public bool HasResources => Resources.Count > 0;

    public bool HasNoResources => !HasResources;

    public bool IsCopyBannerVisible
    {
        get => _isCopyBannerVisible;
        private set =>
            SetProperty(ref _isCopyBannerVisible, value);
    }

    public string CopyBannerText
    {
        get => _copyBannerText;
        private set => SetProperty(ref _copyBannerText, value);
    }

    private void Reload()
    {
        _allEntries =
            _resourceService.GetResources(PreviewThemeKey);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filter = FilterText.Trim();
        var applyKindFilter =
            ShowColors || ShowBrushes || ShowGradients;
        var rows = new List<ThemeResourceRowViewModel>();

        foreach (var entry in _allEntries)
        {
            if (applyKindFilter &&
                !MatchesKind(entry.Kind))
            {
                continue;
            }

            if (filter.Length > 0 &&
                entry.Key.IndexOf(
                    filter,
                    StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            rows.Add(new ThemeResourceRowViewModel(
                entry,
                rows.Count,
                Copy));
        }

        Resources = rows;
        CountText = rows.Count == _allEntries.Count
            ? $"{_allEntries.Count} keys"
            : $"{rows.Count} / {_allEntries.Count} keys";
        EmptyMessage = _allEntries.Count == 0
            ? "No theme color resources found."
            : "No matching keys.";
    }

    private bool MatchesKind(ThemeResourceKind kind) =>
        kind switch
        {
            ThemeResourceKind.Color => ShowColors,
            ThemeResourceKind.SolidBrush => ShowBrushes,
            ThemeResourceKind.GradientBrush => ShowGradients,
            _ => true
        };

    private void Copy(string text, string banner)
    {
        if (_clipboardService.TrySetText(text, out var error))
        {
            CopyBannerText = banner;
            IsCopyBannerVisible = true;
            Status = banner;
            return;
        }

        CopyBannerText = $"Copy failed: {error}";
        IsCopyBannerVisible = true;
        Status = CopyBannerText;
    }
}

public sealed class ThemeResourceRowViewModel
{
    private static readonly Brush TransparentRow =
        new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
    private static readonly Brush AlternateRow =
        new SolidColorBrush(Color.FromArgb(24, 128, 128, 128));

    public ThemeResourceRowViewModel(
        ThemeResourceEntry entry,
        int rowIndex,
        Action<string, string> copy)
    {
        Entry = entry;
        RowBackground =
            (rowIndex & 1) == 0 ? TransparentRow : AlternateRow;
        CopyKeyCommand = new RelayCommand(() =>
            copy(entry.Key, $"Copied key: {entry.Key}"));
        CopyThemeResourceCommand = new RelayCommand(() =>
            copy(
                entry.ThemeResourceExpression,
                $"Copied: {entry.ThemeResourceExpression}"));
        CopyStaticResourceCommand = new RelayCommand(() =>
            copy(
                entry.StaticResourceExpression,
                $"Copied: {entry.StaticResourceExpression}"));
        CopyValueCommand = new RelayCommand(() =>
            copy(
                entry.Display,
                $"Copied value: {entry.Display}"));
    }

    public ThemeResourceEntry Entry { get; }

    public Brush RowBackground { get; }

    public ICommand CopyKeyCommand { get; }

    public ICommand CopyThemeResourceCommand { get; }

    public ICommand CopyStaticResourceCommand { get; }

    public ICommand CopyValueCommand { get; }
}
