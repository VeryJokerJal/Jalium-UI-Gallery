using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>
/// Catalog selection, filtering, search, and navigation state for
/// <c>HomePage</c>.
/// </summary>
public sealed class HomeViewModel
    : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private readonly IGalleryNavigationCoordinator _navigationCoordinator;
    private string _selectedItemKey;
    private HomeCatalogItem _selectedItem;
    private string _activeFilter = "All";
    private string _activeDevice = "Desktop";
    private string _searchQuery = string.Empty;
    private IReadOnlyList<HomeCatalogItem> _visibleItems;

    public HomeViewModel(IGalleryNavigationCoordinator navigationCoordinator)
    {
        ArgumentNullException.ThrowIfNull(navigationCoordinator);
        _navigationCoordinator = navigationCoordinator;

        _visibleItems = HomeCatalog.Items;
        _selectedItem = HomeCatalog.Items[0];
        _selectedItemKey = _selectedItem.Key;

        LocalizationManager.LanguageChanged += OnLanguageChanged;

        SelectItemCommand = new RelayCommand<HomeCatalogItem>(SelectItem);
        SetFilterCommand = new RelayCommand<string>(SetFilter);
        SetDeviceCommand = new RelayCommand<string>(SetDevice);
        OpenItemCommand = new RelayCommand<HomeCatalogItem>(OpenItem);
        OpenSelectedCommand = new RelayCommand(OpenSelected, () => HasVisibleItems);
        OpenBestMatchCommand = new RelayCommand(OpenBestMatch, () => HasVisibleItems);
    }

    public ICommand SelectItemCommand { get; }

    public ICommand SetFilterCommand { get; }

    public ICommand SetDeviceCommand { get; }

    public ICommand OpenItemCommand { get; }

    public ICommand OpenSelectedCommand { get; }

    public ICommand OpenBestMatchCommand { get; }

    public IReadOnlyList<HomeCatalogItem> VisibleItems
    {
        get => _visibleItems;
        private set
        {
            if (SetProperty(ref _visibleItems, value))
            {
                OnPropertyChanged(nameof(HasVisibleItems));
                OnPropertyChanged(nameof(ResultCountText));
                NotifyNavigationCommands();
            }
        }
    }

    public string SelectedItemKey
    {
        get => _selectedItemKey;
        set
        {
            if (SetProperty(ref _selectedItemKey, value))
            {
                var item = HomeCatalog.Items.FirstOrDefault(
                    candidate => candidate.Key == value);
                if (item is not null &&
                    !Equals(_selectedItem, item))
                {
                    _selectedItem = item;
                    OnPropertyChanged(nameof(SelectedItem));
                    NotifySelectedItemDetailsChanged();
                }
            }
        }
    }

    public HomeCatalogItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            var normalized = value ?? HomeCatalog.Items[0];
            if (SetProperty(ref _selectedItem, normalized))
            {
                _selectedItemKey = normalized.Key;
                OnPropertyChanged(nameof(SelectedItemKey));
                NotifySelectedItemDetailsChanged();
            }
        }
    }

    public IReadOnlyList<string> FilterOptions { get; } =
        new[] { "All" }
            .Concat(
                HomeCatalog.Items
                    .Select(item => item.Category)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(
                        category => category,
                        StringComparer.Ordinal))
            .ToArray();

    public IReadOnlyList<string> DeviceOptions { get; } =
    [
        "Desktop",
        "Tablet",
        "Mobile"
    ];

    public string ActiveFilter
    {
        get => _activeFilter;
        set
        {
            var normalized = string.IsNullOrWhiteSpace(value) ? "All" : value;
            if (SetProperty(ref _activeFilter, normalized))
            {
                RefreshVisibleItems();
            }
        }
    }

    public string ActiveDevice
    {
        get => _activeDevice;
        set
        {
            if (SetProperty(
                    ref _activeDevice,
                    string.IsNullOrWhiteSpace(value)
                        ? "Desktop"
                        : value))
            {
                OnPropertyChanged(nameof(PreviewWidth));
            }
        }
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            var normalized = value?.Trim() ?? string.Empty;
            if (SetProperty(ref _searchQuery, normalized))
            {
                RefreshVisibleItems();
            }
        }
    }

    public bool HasVisibleItems => VisibleItems.Count > 0;

    public string ResultCountText =>
        LocalizationManager.Format(
            VisibleItems.Count == 1
                ? "Loc.Home.ResultCountOneFormat"
                : "Loc.Home.ResultCountFormat",
            VisibleItems.Count);

    public string SelectedCategoryLabel =>
        SelectedItem.CategoryLabel.ToUpperInvariant();

    public string OpenSelectedLabel =>
        LocalizationManager.Format("Loc.Home.OpenDemoFormat", SelectedItem.Title);

    public double PreviewWidth => ActiveDevice switch
    {
        "Mobile" => 210,
        "Tablet" => 260,
        _ => 310
    };

    private void SelectItem(HomeCatalogItem? item)
    {
        if (item != null)
        {
            SelectedItem = item;
        }
    }

    private void SetFilter(string? filter)
    {
        if (filter != null)
        {
            ActiveFilter = filter;
        }
    }

    private void SetDevice(string? device)
    {
        if (device != null)
        {
            ActiveDevice = device;
        }
    }

    private void OpenSelected()
    {
        if (HasVisibleItems)
        {
            _navigationCoordinator.RequestNavigation(
                SelectedItem.PageTag);
        }
    }

    private void OpenItem(HomeCatalogItem? item)
    {
        if (item == null)
        {
            return;
        }

        SelectedItem = item;
        _navigationCoordinator.RequestNavigation(item.PageTag);
    }

    private void OpenBestMatch()
    {
        if (!HasVisibleItems)
        {
            return;
        }

        SelectedItem = VisibleItems[0];
        _navigationCoordinator.RequestNavigation(VisibleItems[0].PageTag);
    }

    private void RefreshVisibleItems()
    {
        var visibleItems = HomeCatalog.Items
            .Where(item =>
                ActiveFilter == "All" ||
                item.Category == ActiveFilter)
            .Where(item =>
                string.IsNullOrEmpty(SearchQuery) ||
                item.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                item.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                item.PageTag.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        VisibleItems = visibleItems;

        if (visibleItems.Length > 0 &&
            !visibleItems.Any(item => item.Key == SelectedItemKey))
        {
            SelectedItem = visibleItems[0];
        }
    }

    private void NotifyNavigationCommands()
    {
        ((RelayCommand)OpenSelectedCommand).NotifyCanExecuteChanged();
        ((RelayCommand)OpenBestMatchCommand).NotifyCanExecuteChanged();
    }

    private void NotifySelectedItemDetailsChanged()
    {
        OnPropertyChanged(nameof(SelectedCategoryLabel));
        OnPropertyChanged(nameof(OpenSelectedLabel));
    }

    /// <summary>
    /// The catalog rows show a localized category and the inspector shows a
    /// localized description, but both live on the item records rather than on
    /// this view model — so a language switch has to hand the list a fresh
    /// array to make the item containers re-read them.
    /// </summary>
    private void OnLanguageChanged(object? sender, EventArgs args)
    {
        RefreshVisibleItems();
        OnPropertyChanged(nameof(ResultCountText));
        OnPropertyChanged(nameof(SelectedItem));
        NotifySelectedItemDetailsChanged();
    }

    public override void Destroy()
    {
        LocalizationManager.LanguageChanged -= OnLanguageChanged;
        base.Destroy();
    }
}
