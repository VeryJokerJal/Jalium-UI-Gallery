using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public class NavigationRequestEventArgs : EventArgs
{
    public NavigationRequestEventArgs(string pageTag)
    {
        PageTag = pageTag;
    }

    public string PageTag { get; }
}

/// <summary>
/// Component workbench shown when the Gallery opens. It mirrors the generated
/// reference with a filterable component grid, a live preview inspector, code,
/// and design-token swatches while preserving the existing page routes.
/// </summary>
public partial class HomePage : Page
{
    private sealed class ShowcaseItem
    {
        public ShowcaseItem(
            string key,
            string title,
            string category,
            string pageTag,
            string description,
            string snippet,
            string previewKind)
        {
            Key = key;
            Title = title;
            Category = category;
            PageTag = pageTag;
            Description = description;
            Snippet = snippet;
            PreviewKind = previewKind;
        }

        public string Key { get; }
        public string Title { get; }
        public string Category { get; }
        public string PageTag { get; }
        public string Description { get; }
        public string Snippet { get; }
        public string PreviewKind { get; }
    }

    private sealed class CardVisual
    {
        public CardVisual(Border container, Border selectedBadge)
        {
            Container = container;
            SelectedBadge = selectedBadge;
        }

        public Border Container { get; }
        public Border SelectedBadge { get; }
    }

    private static readonly IReadOnlyList<ShowcaseItem> ShowcaseItems =
        GalleryComponentCatalog.Items
            .Select(item => new ShowcaseItem(
                item.PageTag,
                item.Title,
                item.Category,
                item.PageTag,
                item.Description,
                item.ExampleMarkup,
                item.PreviewKind))
            .ToList();

    private readonly Dictionary<string, CardVisual> _cardVisuals = new();
    private readonly Dictionary<string, Button> _filterButtons = new();
    private readonly Dictionary<string, Button> _deviceButtons = new();
    private readonly List<ShowcaseItem> _visibleItems = new();

    private ShowcaseItem _selectedItem = ShowcaseItems[0];
    private string _activeFilter = "All";
    private string _activeDevice = "Desktop";
    private string _searchQuery = string.Empty;
    private bool _hasVisibleItems = true;

    private Grid? _workspaceGrid;
    private StackPanel? _catalogPanel;
    private ColumnDefinition? _detailColumn;
    private Border? _detailPanel;
    private UniformGrid? _componentGrid;
    private TextBlock? _resultCount;
    private TextBlock? _detailTitle;
    private TextBlock? _detailCategory;
    private TextBlock? _detailDescription;
    private StackPanel? _detailPreviewHost;
    private Border? _detailPreviewFrame;
    private TextBlock? _codeText;
    private Button? _openDemoButton;

    public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

    public HomePage()
    {
        InitializeComponent();
        BuildContent();
    }

    private void BuildContent()
    {
        var root = new Grid
        {
            Background = GalleryTheme.TransparentBrush
        };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        var detailColumn = new ColumnDefinition { Width = new GridLength(0) };
        root.ColumnDefinitions.Add(detailColumn);
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        _workspaceGrid = root;
        _detailColumn = detailColumn;

        var catalog = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0)
        };
        _catalogPanel = catalog;
        catalog.Children.Add(CreateCatalogHeader());
        catalog.Children.Add(CreateFilterBar());

        _componentGrid = new UniformGrid
        {
            Columns = 1,
            RowSpacing = 12,
            ColumnSpacing = 12
        };
        catalog.Children.Add(_componentGrid);

        Grid.SetColumn(catalog, 0);
        Grid.SetRow(catalog, 0);
        root.Children.Add(catalog);

        _detailPanel = CreateDetailPanel();
        _detailPanel.Visibility = Visibility.Collapsed;
        Grid.SetColumn(_detailPanel, 1);
        Grid.SetRow(_detailPanel, 0);
        root.Children.Add(_detailPanel);

        Content = root;

        RenderCards();
        UpdateDetailPanel();
        SetDeviceMode("Desktop");

        SizeChanged += (_, _) => UpdateResponsiveLayout();
        Loaded += (_, _) => UpdateResponsiveLayout();
    }

    private UIElement CreateCatalogHeader()
    {
        var header = new Grid
        {
            Margin = new Thickness(0, 0, 0, 16)
        };
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var copy = new StackPanel { Orientation = Orientation.Vertical };
        copy.Children.Add(new TextBlock
        {
            Text = "Components",
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "Browse every Gallery component, inspect it, then open the full demo.",
            FontSize = 13,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        header.Children.Add(copy);

        _resultCount = new TextBlock
        {
            Text = $"{ShowcaseItems.Count} components",
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.AccentDarkBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        var countBadge = new Border
        {
            Background = GalleryTheme.AccentSoftBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(10, 6, 10, 6),
            VerticalAlignment = VerticalAlignment.Top,
            Child = _resultCount
        };
        Grid.SetColumn(countBadge, 1);
        header.Children.Add(countBadge);

        return header;
    }

    private UIElement CreateFilterBar()
    {
        var row = new WrapPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 18)
        };

        foreach (var filter in new[]
                 {
                     "All", "Controls", "Text", "Layout", "Navigation",
                     "Data", "Media", "Visuals", "System"
                 })
        {
            var button = new Button
            {
                Content = filter,
                Height = 34,
                Padding = new Thickness(14, 0, 14, 0),
                Margin = new Thickness(0, 0, 8, 8),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };
            button.Click += (_, _) => SetFilter(filter);
            _filterButtons[filter] = button;
            row.Children.Add(button);
        }

        UpdateFilterButtons();
        return row;
    }

    private Border CreateDetailPanel()
    {
        var panel = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1, 0, 0, 0),
            Padding = new Thickness(22, 2, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var titleRow = new Grid { Margin = new Thickness(0, 0, 0, 6) };
        titleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        titleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        _detailTitle = new TextBlock
        {
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        titleRow.Children.Add(_detailTitle);

        _detailCategory = new TextBlock
        {
            FontSize = 10,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.AccentDarkBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        var categoryBadge = new Border
        {
            Background = GalleryTheme.AccentSoftBrush,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 4, 8, 4),
            Child = _detailCategory
        };
        Grid.SetColumn(categoryBadge, 1);
        titleRow.Children.Add(categoryBadge);
        stack.Children.Add(titleRow);

        _detailDescription = new TextBlock
        {
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 16)
        };
        stack.Children.Add(_detailDescription);

        stack.Children.Add(CreateDeviceSelector());

        _detailPreviewHost = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        _detailPreviewFrame = new Border
        {
            Width = 310,
            MinHeight = 220,
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(18),
            HorizontalAlignment = HorizontalAlignment.Center,
            Child = _detailPreviewHost
        };
        stack.Children.Add(_detailPreviewFrame);

        _openDemoButton = new Button
        {
            Height = 38,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 12, 0, 20),
            Background = GalleryTheme.AccentPrimaryBrush,
            BorderBrush = GalleryTheme.AccentPrimaryBrush,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            FontSize = 12,
            FontWeight = FontWeights.Bold
        };
        _openDemoButton.Click += (_, _) =>
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(_selectedItem.PageTag));
        stack.Children.Add(_openDemoButton);

        stack.Children.Add(new TextBlock
        {
            Text = "JALXAML",
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(2, 0, 0, 8)
        });

        _codeText = new TextBlock
        {
            FontFamily = new FontFamily("Cascadia Code"),
            FontSize = 11,
            Foreground = new SolidColorBrush(Color.FromRgb(0xD7, 0xE0, 0xE5)),
            TextWrapping = TextWrapping.Wrap
        };
        stack.Children.Add(new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x18, 0x1E, 0x24)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x2D, 0x36, 0x40)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 0, 0, 20),
            Child = _codeText
        });

        stack.Children.Add(new TextBlock
        {
            Text = "Design tokens",
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 10)
        });
        stack.Children.Add(CreateTokenSwatches());

        panel.Child = stack;
        return panel;
    }

    private UIElement CreateDeviceSelector()
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 0, 10)
        };

        foreach (var mode in new[] { "Desktop", "Tablet", "Mobile" })
        {
            var button = new Button
            {
                Content = mode,
                Height = 30,
                Padding = new Thickness(9, 0, 9, 0),
                Margin = new Thickness(4, 0, 0, 0),
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                ToolTip = $"Preview at {mode.ToLowerInvariant()} width"
            };
            button.Click += (_, _) => SetDeviceMode(mode);
            _deviceButtons[mode] = button;
            row.Children.Add(button);
        }

        return row;
    }

    private UIElement CreateTokenSwatches()
    {
        var grid = new UniformGrid
        {
            Columns = 6,
            ColumnSpacing = 7
        };

        grid.Children.Add(CreateTokenSwatch("Teal", Color.FromRgb(0x08, 0x94, 0x8A)));
        grid.Children.Add(CreateTokenSwatch("Indigo", Color.FromRgb(0x4F, 0x46, 0xE5)));
        grid.Children.Add(CreateTokenSwatch("Sky", Color.FromRgb(0x0E, 0xA5, 0xE9)));
        grid.Children.Add(CreateTokenSwatch("Green", Color.FromRgb(0x10, 0xB9, 0x81)));
        grid.Children.Add(CreateTokenSwatch("Amber", Color.FromRgb(0xF5, 0x9E, 0x0B)));
        grid.Children.Add(CreateTokenSwatch("Rose", Color.FromRgb(0xF4, 0x3F, 0x5E)));

        return grid;
    }

    private static UIElement CreateTokenSwatch(string name, Color color)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        stack.Children.Add(new Border
        {
            Width = 34,
            Height = 34,
            Background = new SolidColorBrush(color),
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Margin = new Thickness(0, 0, 0, 5)
        });
        stack.Children.Add(new TextBlock
        {
            Text = name,
            FontSize = 8,
            Foreground = GalleryTheme.TextMutedBrush,
            HorizontalAlignment = HorizontalAlignment.Center
        });
        return stack;
    }

    private void SetFilter(string filter)
    {
        if (_activeFilter == filter)
        {
            return;
        }

        _activeFilter = filter;
        UpdateFilterButtons();
        RenderCards();
    }

    public void SetSearchQuery(string query)
    {
        var normalized = query?.Trim() ?? string.Empty;
        if (string.Equals(_searchQuery, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _searchQuery = normalized;
        RenderCards();
    }

    public bool TryOpenBestMatch()
    {
        if (_visibleItems.Count == 0)
        {
            return false;
        }

        SelectComponent(_visibleItems[0]);
        NavigationRequested?.Invoke(
            this,
            new NavigationRequestEventArgs(_visibleItems[0].PageTag));
        return true;
    }

    private void UpdateFilterButtons()
    {
        foreach (var pair in _filterButtons)
        {
            var selected = pair.Key == _activeFilter;
            pair.Value.Background = selected
                ? GalleryTheme.AccentPrimaryBrush
                : GalleryTheme.BackgroundCardBrush;
            pair.Value.BorderBrush = selected
                ? GalleryTheme.AccentPrimaryBrush
                : GalleryTheme.BorderDefaultBrush;
            pair.Value.Foreground = selected
                ? new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF))
                : GalleryTheme.TextSecondaryBrush;
        }
    }

    private void RenderCards()
    {
        if (_componentGrid == null)
        {
            return;
        }

        var visibleItems = ShowcaseItems
            .Where(item => _activeFilter == "All" || item.Category == _activeFilter)
            .Where(item =>
                string.IsNullOrEmpty(_searchQuery) ||
                item.Title.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                item.Category.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                item.PageTag.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _visibleItems.Clear();
        _visibleItems.AddRange(visibleItems);
        _hasVisibleItems = visibleItems.Count > 0;

        if (!visibleItems.Contains(_selectedItem) && visibleItems.Count > 0)
        {
            _selectedItem = visibleItems[0];
        }

        ClearPanelChildren(_componentGrid);
        _cardVisuals.Clear();

        foreach (var item in visibleItems)
        {
            _componentGrid.Children.Add(CreateComponentCard(item));
        }

        if (visibleItems.Count == 0)
        {
            _componentGrid.Children.Add(CreateEmptyState());
        }

        if (_resultCount != null)
        {
            _resultCount.Text = $"{visibleItems.Count} component{(visibleItems.Count == 1 ? string.Empty : "s")}";
        }

        UpdateCardSelection();
        if (_hasVisibleItems)
        {
            UpdateDetailPanel();
        }

        UpdateResponsiveLayout();
    }

    private static UIElement CreateEmptyState()
    {
        return new Border
        {
            Height = 150,
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Child = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        Text = "No components found",
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = GalleryTheme.TextPrimaryBrush,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new TextBlock
                    {
                        Text = "Try another name or category.",
                        FontSize = 11,
                        Foreground = GalleryTheme.TextTertiaryBrush,
                        Margin = new Thickness(0, 5, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                }
            }
        };
    }

    private UIElement CreateComponentCard(ShowcaseItem item)
    {
        var card = new Border
        {
            Height = 198,
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Cursor = Cursors.Hand,
            ClipToBounds = true
        };

        var layout = new Grid();
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(148) });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

        var previewSurface = new Border
        {
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderSubtleBrush,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(14),
            IsHitTestVisible = false,
            Child = CreateComponentPreview(item, false)
        };
        layout.Children.Add(previewSurface);

        var selectedBadge = new Border
        {
            Width = 22,
            Height = 22,
            Background = GalleryTheme.AccentPrimaryBrush,
            CornerRadius = new CornerRadius(6),
            Margin = new Thickness(0, 9, 9, 0),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Visibility = Visibility.Collapsed,
            IsHitTestVisible = false,
            Child = new TextBlock
            {
                Text = "✓",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        layout.Children.Add(selectedBadge);

        var footer = new Grid
        {
            Background = GalleryTheme.BackgroundCardBrush,
            Margin = new Thickness(12, 0, 12, 0)
        };
        footer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        footer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var footerCopy = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };
        footerCopy.Children.Add(new TextBlock
        {
            Text = item.Title,
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush
        });
        footerCopy.Children.Add(new TextBlock
        {
            Text = item.Category,
            FontSize = 9,
            Foreground = GalleryTheme.TextMutedBrush
        });
        footer.Children.Add(footerCopy);

        var arrow = new TextBlock
        {
            Text = ">",
            FontSize = 16,
            Foreground = GalleryTheme.TextTertiaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(arrow, 1);
        footer.Children.Add(arrow);
        Grid.SetRow(footer, 1);
        layout.Children.Add(footer);

        card.Child = layout;
        card.MouseEnter += (_, _) =>
        {
            if (_selectedItem.Key != item.Key)
            {
                card.BorderBrush = GalleryTheme.AccentLightBrush;
            }
        };
        card.MouseLeave += (_, _) =>
        {
            if (_selectedItem.Key != item.Key)
            {
                card.BorderBrush = GalleryTheme.BorderDefaultBrush;
            }
        };
        card.MouseDown += (_, e) =>
        {
            if (e is MouseButtonEventArgs mouse && mouse.ChangedButton == MouseButton.Left)
            {
                SelectComponent(item);
            }
        };

        _cardVisuals[item.Key] = new CardVisual(card, selectedBadge);
        return card;
    }

    private void SelectComponent(ShowcaseItem item)
    {
        _selectedItem = item;
        UpdateCardSelection();
        UpdateDetailPanel();
    }

    private void UpdateCardSelection()
    {
        foreach (var pair in _cardVisuals)
        {
            var selected = pair.Key == _selectedItem.Key;
            pair.Value.Container.BorderBrush = selected
                ? GalleryTheme.AccentPrimaryBrush
                : GalleryTheme.BorderDefaultBrush;
            pair.Value.Container.BorderThickness = new Thickness(selected ? 2 : 1);
            pair.Value.SelectedBadge.Visibility = selected
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void UpdateDetailPanel()
    {
        if (_detailTitle == null ||
            _detailCategory == null ||
            _detailDescription == null ||
            _detailPreviewHost == null ||
            _codeText == null ||
            _openDemoButton == null)
        {
            return;
        }

        _detailTitle.Text = _selectedItem.Title;
        _detailCategory.Text = _selectedItem.Category.ToUpperInvariant();
        _detailDescription.Text = _selectedItem.Description;
        _codeText.Text = _selectedItem.Snippet;
        _openDemoButton.Content = $"Open {_selectedItem.Title} demo  >";

        ClearPanelChildren(_detailPreviewHost);
        _detailPreviewHost.Children.Add(CreateComponentPreview(_selectedItem, true));
    }

    private static void ClearPanelChildren(Panel panel)
    {
        for (var index = 0; index < panel.Children.Count; index++)
        {
            panel.Children[index].Visibility = Visibility.Collapsed;
        }

        panel.Children.Clear();
    }

    private void SetDeviceMode(string mode)
    {
        _activeDevice = mode;

        if (_detailPreviewFrame != null)
        {
            _detailPreviewFrame.Width = mode switch
            {
                "Mobile" => 210,
                "Tablet" => 260,
                _ => 310
            };
        }

        foreach (var pair in _deviceButtons)
        {
            var selected = pair.Key == _activeDevice;
            pair.Value.Background = selected
                ? GalleryTheme.AccentSoftBrush
                : GalleryTheme.TransparentBrush;
            pair.Value.BorderBrush = selected
                ? GalleryTheme.AccentPrimaryBrush
                : GalleryTheme.BorderDefaultBrush;
            pair.Value.Foreground = selected
                ? GalleryTheme.AccentDarkBrush
                : GalleryTheme.TextTertiaryBrush;
        }
    }

    private void UpdateResponsiveLayout()
    {
        if (_workspaceGrid == null ||
            _catalogPanel == null ||
            _detailColumn == null ||
            _detailPanel == null ||
            _componentGrid == null)
        {
            return;
        }

        var width = ActualWidth;
        if (width <= 0)
        {
            return;
        }

        var splitView = width >= 1040;
        _detailPanel.Visibility = _hasVisibleItems ? Visibility.Visible : Visibility.Collapsed;
        if (splitView)
        {
            _detailColumn.Width = new GridLength(_hasVisibleItems ? 360 : 0);
            Grid.SetColumn(_detailPanel, 1);
            Grid.SetRow(_detailPanel, 0);
            _detailPanel.Margin = new Thickness(0);
            _detailPanel.BorderThickness = new Thickness(1, 0, 0, 0);
            _detailPanel.Padding = new Thickness(22, 2, 0, 0);
            _catalogPanel.Margin = new Thickness(0, 0, _hasVisibleItems ? 20 : 0, 0);
            _componentGrid.Columns = _hasVisibleItems
                ? width >= 1180 ? 3 : 2
                : 1;
        }
        else
        {
            _detailColumn.Width = new GridLength(0);
            Grid.SetColumn(_detailPanel, 0);
            Grid.SetRow(_detailPanel, 1);
            _detailPanel.Margin = new Thickness(0, 24, 0, 0);
            _detailPanel.BorderThickness = new Thickness(0, 1, 0, 0);
            _detailPanel.Padding = new Thickness(0, 22, 0, 0);
            _catalogPanel.Margin = new Thickness(0);
            _componentGrid.Columns = _hasVisibleItems && width >= 680 ? 2 : 1;
        }
    }

    private static UIElement CreateComponentPreview(ShowcaseItem item, bool detailed)
    {
        return item.PreviewKind switch
        {
            "buttons" => CreateButtonsPreview(detailed),
            "inputs" => CreateInputsPreview(detailed),
            "cards" => CreateCardPreview(),
            "checkboxes" => CreateCheckboxPreview(detailed),
            "switches" => CreateSwitchPreview(detailed),
            "badges" => CreateBadgePreview(),
            "select" => CreateSelectPreview(),
            "progress" => CreateProgressPreview(),
            "alerts" => CreateAlertPreview(),
            _ => CreateGenericPreview(item, detailed)
        };
    }

    private static UIElement CreateGenericPreview(ShowcaseItem item, bool detailed)
    {
        var accent = GetCategoryColor(item.Category);
        var layout = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(54) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        layout.Children.Add(new Border
        {
            Width = 44,
            Height = 44,
            Background = new SolidColorBrush(Color.FromArgb(0x2F, accent.R, accent.G, accent.B)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x66, accent.R, accent.G, accent.B)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(7),
            Child = new TextBlock
            {
                Text = GetInitials(item.Title),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(accent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        var copy = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };
        copy.Children.Add(new TextBlock
        {
            Text = item.Title,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 3)
        });
        copy.Children.Add(new TextBlock
        {
            Text = item.Category,
            FontSize = 9,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(accent)
        });
        if (detailed)
        {
            copy.Children.Add(new TextBlock
            {
                Text = item.Description,
                FontSize = 9,
                Foreground = GalleryTheme.TextTertiaryBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 0)
            });
        }

        Grid.SetColumn(copy, 1);
        layout.Children.Add(copy);
        return layout;
    }

    private static string GetInitials(string title)
    {
        var initials = new string(title.Where(char.IsUpper).Take(2).ToArray());
        if (!string.IsNullOrEmpty(initials))
        {
            return initials;
        }

        return title.Length == 0 ? "UI" : title[..1].ToUpperInvariant();
    }

    private static Color GetCategoryColor(string category)
    {
        return category switch
        {
            "Controls" => GalleryTheme.AccentPrimary,
            "Text" => Color.FromRgb(0x4F, 0x46, 0xE5),
            "Layout" => Color.FromRgb(0x0E, 0xA5, 0xE9),
            "Navigation" => Color.FromRgb(0x10, 0xB9, 0x81),
            "Data" => Color.FromRgb(0xF5, 0x9E, 0x0B),
            "Media" => Color.FromRgb(0xF4, 0x3F, 0x5E),
            "Visuals" => Color.FromRgb(0x8B, 0x5C, 0xF6),
            _ => Color.FromRgb(0x64, 0x70, 0x85)
        };
    }

    private static UIElement CreateButtonsPreview(bool detailed)
    {
        var row = new WrapPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        row.Children.Add(CreatePreviewButton("Primary", true));
        row.Children.Add(CreatePreviewButton("Secondary", false));
        row.Children.Add(CreateGhostButton());

        if (detailed)
        {
            row.Children.Add(new Button
            {
                Content = "Disabled",
                IsEnabled = false,
                Height = 36,
                Padding = new Thickness(12, 0, 12, 0),
                Margin = new Thickness(4)
            });
        }

        return row;
    }

    private static Button CreatePreviewButton(string label, bool primary)
    {
        return new Button
        {
            Content = label,
            Height = 36,
            Padding = new Thickness(12, 0, 12, 0),
            Margin = new Thickness(4),
            Background = primary ? GalleryTheme.AccentPrimaryBrush : GalleryTheme.BackgroundCardBrush,
            BorderBrush = primary ? GalleryTheme.AccentPrimaryBrush : GalleryTheme.BorderStrongBrush,
            Foreground = primary
                ? new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF))
                : GalleryTheme.TextSecondaryBrush,
            FontSize = 11,
            FontWeight = FontWeights.SemiBold
        };
    }

    private static Button CreateGhostButton()
    {
        return new Button
        {
            Content = "Ghost",
            Height = 36,
            Padding = new Thickness(10, 0, 10, 0),
            Margin = new Thickness(4),
            Background = GalleryTheme.TransparentBrush,
            BorderThickness = new Thickness(0),
            Foreground = GalleryTheme.AccentPrimaryBrush,
            FontSize = 11,
            FontWeight = FontWeights.SemiBold
        };
    }

    private static UIElement CreateInputsPreview(bool detailed)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(new TextBox
        {
            Height = 34,
            Text = "Placeholder",
            Foreground = GalleryTheme.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 7)
        });
        stack.Children.Add(new TextBox
        {
            Height = 34,
            Text = "Focused input",
            Margin = new Thickness(0, 0, 0, detailed ? 7 : 0)
        });
        if (detailed)
        {
            stack.Children.Add(new TextBox
            {
                Height = 34,
                Text = "Disabled input",
                IsEnabled = false
            });
        }
        return stack;
    }

    private static UIElement CreateCardPreview()
    {
        var content = new Grid();
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        content.Children.Add(new Border
        {
            Width = 34,
            Height = 34,
            Background = GalleryTheme.AccentSoftBrush,
            CornerRadius = new CornerRadius(6),
            Child = new TextBlock
            {
                Text = "▣",
                FontSize = 15,
                Foreground = GalleryTheme.AccentPrimaryBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        var copy = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };
        copy.Children.Add(new TextBlock
        {
            Text = "Card title",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "Supporting content for this component.",
            FontSize = 10,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        Grid.SetColumn(copy, 1);
        content.Children.Add(copy);

        return new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            Child = content
        };
    }

    private static UIElement CreateCheckboxPreview(bool detailed)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(new CheckBox
        {
            Content = "Checked",
            IsChecked = true,
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, 6)
        });
        stack.Children.Add(new CheckBox
        {
            Content = "Unchecked",
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, detailed ? 6 : 0)
        });
        if (detailed)
        {
            stack.Children.Add(new CheckBox
            {
                Content = "Disabled",
                IsChecked = true,
                Foreground = GalleryTheme.TextDisabledBrush,
                IsEnabled = false
            });
        }
        return stack;
    }

    private static UIElement CreateSwitchPreview(bool detailed)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(new ToggleSwitch
        {
            Header = "Notifications",
            IsOn = true,
            Foreground = GalleryTheme.TextSecondaryBrush,
            OnBackground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 8)
        });
        stack.Children.Add(new ToggleSwitch
        {
            Header = "Auto update",
            IsOn = false,
            Foreground = GalleryTheme.TextSecondaryBrush,
            OnBackground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, detailed ? 8 : 0)
        });
        if (detailed)
        {
            stack.Children.Add(new ToggleSwitch
            {
                Header = "Unavailable",
                Foreground = GalleryTheme.TextDisabledBrush,
                IsEnabled = false
            });
        }
        return stack;
    }

    private static UIElement CreateBadgePreview()
    {
        var row = new WrapPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        row.Children.Add(CreateBadge("Default", GalleryTheme.TextTertiary));
        row.Children.Add(CreateBadge("Success", GalleryTheme.Success));
        row.Children.Add(CreateBadge("Info", GalleryTheme.Info));
        row.Children.Add(CreateBadge("Warning", GalleryTheme.Warning));
        row.Children.Add(CreateBadge("Error", GalleryTheme.Error));
        return row;
    }

    private static UIElement CreateBadge(string label, Color color)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(0x2F, color.R, color.G, color.B)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x66, color.R, color.G, color.B)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(7, 4, 7, 4),
            Margin = new Thickness(3),
            Child = new TextBlock
            {
                Text = label,
                FontSize = 9,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(color)
            }
        };
    }

    private static UIElement CreateSelectPreview()
    {
        var comboBox = new ComboBox
        {
            Width = 220,
            Height = 38,
            PlaceholderText = "Select an option",
            Foreground = GalleryTheme.TextSecondaryBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        comboBox.Items.Add("Option one");
        comboBox.Items.Add("Option two");
        comboBox.Items.Add("Option three");
        return comboBox;
    }

    private static UIElement CreateProgressPreview()
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
        var valueRow = new Grid { Margin = new Thickness(0, 0, 0, 7) };
        valueRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        valueRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        valueRow.Children.Add(new TextBlock
        {
            Text = "Uploading",
            FontSize = 10,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextSecondaryBrush
        });
        var value = new TextBlock
        {
            Text = "60%",
            FontSize = 10,
            Foreground = GalleryTheme.TextTertiaryBrush
        };
        Grid.SetColumn(value, 1);
        valueRow.Children.Add(value);
        stack.Children.Add(valueRow);
        stack.Children.Add(new ProgressBar
        {
            Value = 60,
            Height = 8,
            ProgressBrush = GalleryTheme.AccentPrimaryBrush
        });
        return stack;
    }

    private static UIElement CreateAlertPreview()
    {
        var content = new Grid();
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        content.Children.Add(new TextBlock
        {
            Text = "i",
            Width = 20,
            Height = 20,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.InfoBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });
        var copy = new StackPanel { Orientation = Orientation.Vertical };
        copy.Children.Add(new TextBlock
        {
            Text = "Update available",
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 2)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "A new version is ready to install.",
            FontSize = 9,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        Grid.SetColumn(copy, 1);
        content.Children.Add(copy);

        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(0x22, 0x38, 0xBD, 0xF8)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x77, 0x38, 0xBD, 0xF8)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            Child = content
        };
    }
}
