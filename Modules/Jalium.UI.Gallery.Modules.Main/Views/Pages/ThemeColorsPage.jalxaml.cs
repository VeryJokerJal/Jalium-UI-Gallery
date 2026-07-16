using System.Globalization;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Theme color browser. Walks the active <see cref="Application.Resources"/> tree
/// (including merged + theme dictionaries), surfaces every Color/Brush key, and
/// lets the user copy either the raw key, a ready-to-paste <c>{ThemeResource Key}</c>
/// /<c>{StaticResource Key}</c> expression, or the literal value via a per-row
/// <see cref="SplitButton"/> (primary click = copy key, flyout = alternates).
///
/// The list is a <em>virtualizing</em> <see cref="ListBox"/>: the previewed theme can
/// expose 500+ keys, so instead of materializing one row (Grid + SplitButton +
/// MenuFlyout) per key into a StackPanel, the rows are driven by an
/// <see cref="ItemsControl.ItemsSource"/> of lightweight <see cref="ResourceRow"/>
/// models and an <see cref="ItemsControl.ItemTemplate"/> that builds a row's visuals
/// only when the container is realized. With recycling, scrolling re-points an existing
/// container's <c>DataContext</c> at a new row, so <see cref="PopulateRow"/> rebuilds in
/// place rather than allocating a fresh container.
/// </summary>
public partial class ThemeColorsPage : Page
{
    private const string ThemeOptionDark = "Dark";
    private const string ThemeOptionLight = "Light";
    private const string ThemeOptionHighContrast = "HighContrast";

    /// <summary>
    /// One row of the resource browser.
    /// </summary>
    private sealed class ResourceEntry
    {
        public required string Key { get; init; }
        public required object Value { get; init; }
        public required ResourceKind Kind { get; init; }
        public required string Display { get; init; }
        public required Brush Preview { get; init; }
    }

    /// <summary>
    /// View model for one realized list row. Carries the row's position in the
    /// <em>currently filtered</em> list so alternating-row striping stays correct under
    /// virtualization/recycling (the data item itself has no inherent index).
    /// </summary>
    private sealed class ResourceRow
    {
        public required ResourceEntry Entry { get; init; }
        public required int RowIndex { get; init; }
    }

    private enum ResourceKind
    {
        Color,
        SolidBrush,
        GradientBrush
    }

    /// <summary>
    /// Shared, fully transparent brush used as the even-row background. A non-null
    /// (transparent) brush is required so the whole row stays hit-testable for the
    /// click-to-copy gesture — a null background would let clicks fall through.
    /// </summary>
    private static readonly SolidColorBrush s_rowHitBrush = new(Color.FromArgb(0x00, 0, 0, 0));

    /// <summary>
    /// Solid accent fill + caption color for the per-row Copy button. A <em>solid</em> brush
    /// is deliberate: a <see cref="GradientBrush"/> <c>Background</c> (e.g. the theme
    /// <c>AccentBrush</c>) suppresses Button/SplitButton primary-content rendering in this
    /// framework, which is what made the "Copy" caption invisible.
    /// </summary>
    private static readonly SolidColorBrush s_copyAccentBrush = new(Color.FromArgb(0xFF, 0xF2, 0xB8, 0x00));
    private static readonly SolidColorBrush s_copyAccentTextBrush = new(Color.FromArgb(0xFF, 0x1A, 0x14, 0x05));

    private readonly List<ResourceEntry> _allEntries = new();

    /// <summary>
    /// The per-row <see cref="DataTemplate"/>, built once and reused for every container.
    /// Reusing a single template instance is also what arms the ContentPresenter's
    /// recycling fast path (rebind DataContext instead of rebuilding the subtree).
    /// </summary>
    private DataTemplate? _rowTemplate;

    /// <summary>
    /// The gallery hosts every page inside an auto-scrolling <see cref="ScrollViewer"/>,
    /// which hands the page infinite height. A virtualizing list needs a bounded viewport,
    /// so the list lives in a star row whose host <see cref="RootGrid"/> is pinned to this
    /// host's visible viewport height (see <see cref="UpdateListViewportHeight"/>). Cached
    /// once the page is loaded so the page fills the viewport and scrolls internally
    /// instead of inflating the outer scroller with 500+ realized rows.
    /// </summary>
    private ScrollViewer? _hostScrollViewer;

    /// <summary>
    /// The theme dictionary key this page is *previewing*.
    /// Initialized from the live application theme but driven independently
    /// after the user picks something from the Theme combo box, so toggling
    /// the combo never calls ThemeManager.ApplyTheme — it just walks a
    /// different ThemeDictionaries slice.
    /// </summary>
    private string _previewThemeKey = ThemeManager.CurrentTheme.ToString();

    public ThemeColorsPage()
    {
        InitializeComponent();
        SetupThemeSelector();
        WireFilterEvents();
        ConfigureList();
        ReloadAll();

        // Pin the list to the host viewport once we're in the live tree, and keep it in
        // sync as the window/pane resizes.
        Loaded += OnPageLoaded;
        Unloaded += OnPageUnloaded;
    }

    private void OnPageLoaded(object? sender, RoutedEventArgs e)
    {
        if (_hostScrollViewer == null)
        {
            _hostScrollViewer = FindAncestorScrollViewer(this);
            if (_hostScrollViewer != null)
            {
                _hostScrollViewer.SizeChanged += OnHostSizeChanged;
            }
        }

        UpdateListViewportHeight();
    }

    private void OnPageUnloaded(object? sender, RoutedEventArgs e)
    {
        if (_hostScrollViewer != null)
        {
            _hostScrollViewer.SizeChanged -= OnHostSizeChanged;
            _hostScrollViewer = null;
        }
    }

    private void OnHostSizeChanged(object? sender, SizeChangedEventArgs e) => UpdateListViewportHeight();

    /// <summary>
    /// Sizes <see cref="RootGrid"/> to the host scroll viewer's visible viewport so the
    /// page fills the available area exactly (no outer scroll) and the list's star row
    /// gets a bounded height to virtualize against. Idempotent and loop-safe: pinning the
    /// page to the viewport height does not change the viewport, so the host doesn't
    /// re-fire size changes.
    /// </summary>
    private void UpdateListViewportHeight()
    {
        if (_hostScrollViewer == null || RootGrid == null)
            return;

        var viewport = _hostScrollViewer.ViewportHeight;
        if (viewport <= 0 || double.IsNaN(viewport) || double.IsInfinity(viewport))
        {
            // Viewport not measured yet (or unavailable) — fall back to the host's
            // arranged height minus its padding.
            viewport = _hostScrollViewer.ActualHeight
                       - _hostScrollViewer.Padding.Top
                       - _hostScrollViewer.Padding.Bottom;
        }

        if (viewport <= 0 || double.IsNaN(viewport) || double.IsInfinity(viewport))
            return;

        var target = viewport - RootGrid.Margin.Top - RootGrid.Margin.Bottom;
        if (target < 240)
            target = 240;

        if (double.IsNaN(RootGrid.Height) || Math.Abs(RootGrid.Height - target) > 0.5)
            RootGrid.Height = target;
    }

    /// <summary>
    /// Walks up the visual tree to the nearest ancestor <see cref="ScrollViewer"/> — the
    /// gallery's per-page content host.
    /// </summary>
    private static ScrollViewer? FindAncestorScrollViewer(Visual start)
    {
        Visual? current = start.VisualParent;
        while (current != null)
        {
            if (current is ScrollViewer scrollViewer)
                return scrollViewer;
            current = current.VisualParent;
        }

        return null;
    }

    /// <summary>
    /// Walks up the visual tree to the generated <see cref="ListBoxItem"/> container that
    /// hosts a realized row, so its content alignment can be corrected.
    /// </summary>
    private static ListBoxItem? FindAncestorListBoxItem(Visual start)
    {
        Visual? current = start.VisualParent;
        while (current != null)
        {
            if (current is ListBoxItem item)
                return item;
            current = current.VisualParent;
        }

        return null;
    }

    /// <summary>
    /// Makes a realized row fill the list width. The row host's parent is the container's
    /// <see cref="ContentPresenter"/>, whose <see cref="FrameworkElement.HorizontalAlignment"/>
    /// is template-bound to the <see cref="ListBoxItem"/>'s <c>HorizontalContentAlignment</c>
    /// (default <see cref="HorizontalAlignment.Left"/>). Left alignment arranges the host at
    /// its shrink-wrapped width, collapsing the row's star column. Setting both the presenter's
    /// alignment (authoritative for arrange) and the container's content alignment stretches
    /// the row to the full width.
    /// </summary>
    private static void StretchRowContainer(Border host)
    {
        if (host.VisualParent is FrameworkElement presenter)
            presenter.HorizontalAlignment = HorizontalAlignment.Stretch;

        if (FindAncestorListBoxItem(host) is { } container)
            container.HorizontalContentAlignment = HorizontalAlignment.Stretch;
    }

    private void SetupThemeSelector()
    {
        if (ThemeSelector == null) return;

        ThemeSelector.Items.Add(ThemeOptionDark);
        ThemeSelector.Items.Add(ThemeOptionLight);
        ThemeSelector.Items.Add(ThemeOptionHighContrast);
        ThemeSelector.SelectedItem = ThemeManager.CurrentTheme.ToString();
        ThemeSelector.SelectionChanged += OnThemeSelectorChanged;
    }

    private void WireFilterEvents()
    {
        if (FilterBox != null)
            FilterBox.TextChanged += (_, _) => RenderRows();

        if (ShowColorsCheck != null)
        {
            ShowColorsCheck.Checked += (_, _) => RenderRows();
            ShowColorsCheck.Unchecked += (_, _) => RenderRows();
        }
        if (ShowBrushesCheck != null)
        {
            ShowBrushesCheck.Checked += (_, _) => RenderRows();
            ShowBrushesCheck.Unchecked += (_, _) => RenderRows();
        }
        if (ShowGradientsCheck != null)
        {
            ShowGradientsCheck.Checked += (_, _) => RenderRows();
            ShowGradientsCheck.Unchecked += (_, _) => RenderRows();
        }
    }

    /// <summary>
    /// Wires the virtualizing list: installs the row template and neutralizes the
    /// ListBox hover/selection chrome. This page copies on click and never uses list
    /// selection, so overriding <c>SelectionBackground</c>/<c>HighlightBackground</c>
    /// (scoped to the list) keeps the realized rows looking exactly like the old flat,
    /// striped rows instead of painting an accent highlight on the clicked row.
    /// </summary>
    private void ConfigureList()
    {
        if (ResourceList == null) return;

        _rowTemplate ??= BuildRowTemplate();
        ResourceList.ItemTemplate = _rowTemplate;

        ResourceList.Resources["SelectionBackground"] = s_rowHitBrush;
        ResourceList.Resources["HighlightBackground"] = s_rowHitBrush;
    }

    private void OnThemeSelectorChanged(object? sender, EventArgs e)
    {
        if (ThemeSelector?.SelectedItem is not string variantName)
            return;

        // Preview-only switch: just remember which theme dictionary to walk
        // next time, and rebuild rows. Do NOT call ThemeManager.ApplyTheme.
        //
        // Why: ApplyTheme reparses Generic.jalxaml + every control style
        // dictionary AND sweeps every DynamicResource subscription. Reading values
        // straight out of ThemeDictionaries[<key>] gives the user the exact same
        // data without any global side-effect. (Virtualization further shrinks the
        // blast radius here: only the rows in view are live SplitButton/MenuFlyout
        // controls, not all 500+.)
        _previewThemeKey = variantName;
        ReloadAll();
    }

    /// <summary>
    /// Re-walks the resource tree for the currently active theme and rebuilds the rows.
    /// </summary>
    private void ReloadAll()
    {
        _allEntries.Clear();

        var app = Application.Current;
        if (app?.Resources == null)
        {
            RenderRows();
            return;
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        CollectFromDictionary(app.Resources, seen);

        _allEntries.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Key, b.Key));
        RenderRows();
    }

    /// <summary>
    /// Walks a ResourceDictionary, its MergedDictionaries, and the slice of
    /// ThemeDictionaries that matches the *previewed* theme key. Theme-dictionary
    /// values win over merged-dictionary values to mirror the lookup order
    /// the framework uses at runtime. The preview key is independent of the
    /// global <see cref="ResourceDictionary.CurrentThemeKey"/> so users can
    /// inspect any theme without triggering a real theme switch.
    /// </summary>
    private void CollectFromDictionary(ResourceDictionary dict, HashSet<string> seen)
    {
        // Theme dictionary slice for the previewed theme has priority.
        // Try exact match first, then case-insensitive fallback so combo-box
        // strings like "Dark" still resolve when the dictionary uses different casing.
        if (TryGetThemedSlice(dict, _previewThemeKey, out var themed) && themed != null)
        {
            CollectFromDictionary(themed, seen);
        }

        foreach (var key in dict.Keys)
        {
            if (key is not string keyName)
                continue;

            if (!seen.Add(keyName))
                continue;

            var raw = dict[key];
            if (raw == null)
                continue;

            if (TryBuildEntry(keyName, raw, out var entry))
            {
                _allEntries.Add(entry);
            }
        }

        // Recurse into merged dictionaries (in reverse so earlier ones win,
        // matching ResourceDictionary lookup order).
        for (var i = dict.MergedDictionaries.Count - 1; i >= 0; i--)
        {
            CollectFromDictionary(dict.MergedDictionaries[i], seen);
        }
    }

    /// <summary>
    /// Look up a ThemeDictionaries entry by name, falling back to a
    /// case-insensitive match so the combo box string "Dark" resolves
    /// even if the dictionary's key was registered as "DARK", etc.
    /// </summary>
    private static bool TryGetThemedSlice(ResourceDictionary dict, string themeKey, out ResourceDictionary? themed)
    {
        if (dict.ThemeDictionaries.TryGetValue(themeKey, out themed))
            return themed != null;

        foreach (var kvp in dict.ThemeDictionaries)
        {
            if (kvp.Key is string s && string.Equals(s, themeKey, StringComparison.OrdinalIgnoreCase))
            {
                themed = kvp.Value;
                return themed != null;
            }
        }

        themed = null;
        return false;
    }

    private static bool TryBuildEntry(string key, object value, out ResourceEntry entry)
    {
        switch (value)
        {
            case Color color:
                entry = new ResourceEntry
                {
                    Key = key,
                    Value = color,
                    Kind = ResourceKind.Color,
                    Display = FormatColor(color),
                    Preview = new SolidColorBrush(color)
                };
                return true;

            case SolidColorBrush solid:
                entry = new ResourceEntry
                {
                    Key = key,
                    Value = solid,
                    Kind = ResourceKind.SolidBrush,
                    Display = FormatColor(solid.Color) + (solid.Opacity < 1.0 ? $"  α={solid.Opacity:0.##}" : string.Empty),
                    Preview = solid
                };
                return true;

            case LinearGradientBrush gradient:
                entry = new ResourceEntry
                {
                    Key = key,
                    Value = gradient,
                    Kind = ResourceKind.GradientBrush,
                    Display = FormatGradient(gradient),
                    Preview = gradient
                };
                return true;

            case GradientBrush other:
                entry = new ResourceEntry
                {
                    Key = key,
                    Value = other,
                    Kind = ResourceKind.GradientBrush,
                    Display = FormatGradient(other),
                    Preview = other
                };
                return true;

            default:
                entry = null!;
                return false;
        }
    }

    private static string FormatColor(Color c)
    {
        return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
    }

    private static string FormatGradient(GradientBrush gradient)
    {
        if (gradient.GradientStops.Count == 0)
            return "(empty gradient)";

        var first = gradient.GradientStops[0].Color;
        var last = gradient.GradientStops[^1].Color;
        return gradient.GradientStops.Count == 2
            ? $"{FormatColor(first)} → {FormatColor(last)}"
            : $"{FormatColor(first)} → {FormatColor(last)}  ({gradient.GradientStops.Count} stops)";
    }

    /// <summary>
    /// Rebuilds the visible row list based on the search/filter state and hands it to
    /// the virtualizing <see cref="ListBox"/> as a fresh <see cref="ItemsControl.ItemsSource"/>.
    /// Only the rows scrolled into view are realized; this method never touches visuals.
    /// </summary>
    private void RenderRows()
    {
        if (ResourceList == null)
            return;

        var filter = (FilterBox?.Text ?? string.Empty).Trim();
        var showColors = ShowColorsCheck?.IsChecked ?? true;
        var showBrushes = ShowBrushesCheck?.IsChecked ?? true;
        var showGradients = ShowGradientsCheck?.IsChecked ?? true;

        var rows = new List<ResourceRow>();
        foreach (var entry in _allEntries)
        {
            if (!PassesKindFilter(entry, showColors, showBrushes, showGradients))
                continue;
            if (filter.Length > 0 && entry.Key.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            rows.Add(new ResourceRow { Entry = entry, RowIndex = rows.Count });
        }

        // Assigning a fresh list resets the ItemsSource → the panel re-virtualizes
        // against the new item set and only re-realizes what fits the viewport.
        ResourceList.ItemsSource = rows;

        var visible = rows.Count;
        ResourceList.Visibility = visible == 0 ? Visibility.Collapsed : Visibility.Visible;

        if (EmptyText != null)
        {
            EmptyText.Text = _allEntries.Count == 0
                ? "No theme color resources found."
                : "No matching keys.";
            EmptyText.Visibility = visible == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        if (CountText != null)
        {
            CountText.Text = visible == _allEntries.Count
                ? $"{_allEntries.Count} keys"
                : $"{visible} / {_allEntries.Count} keys";
        }
    }

    private static bool PassesKindFilter(ResourceEntry entry, bool showColors, bool showBrushes, bool showGradients)
    {
        // No checkbox selected → no kind filter applied, show every kind.
        if (!showColors && !showBrushes && !showGradients)
            return true;

        return entry.Kind switch
        {
            ResourceKind.Color => showColors,
            ResourceKind.SolidBrush => showBrushes,
            ResourceKind.GradientBrush => showGradients,
            _ => true
        };
    }

    /// <summary>
    /// Builds the per-row <see cref="DataTemplate"/>. The factory creates the row's
    /// outer host <see cref="Border"/> and attaches the row-level handlers exactly once
    /// (the host is reused as containers recycle); <see cref="PopulateRow"/> then fills
    /// the host from whatever <see cref="ResourceRow"/> is the current DataContext, and
    /// re-runs whenever recycling re-points that DataContext.
    /// </summary>
    private DataTemplate BuildRowTemplate()
    {
        var template = new DataTemplate();
        template.SetVisualTree(() =>
        {
            var host = new Border
            {
                Padding = new Thickness(0),
                Cursor = Cursors.Hand
            };

            // Whole-row click copies the key. Attached once on the reused host and
            // reads the CURRENT DataContext, so recycling can never bind this to a
            // stale entry.
            host.MouseLeftButtonUp += (_, _) =>
            {
                if (host.DataContext is ResourceRow row)
                    CopyToClipboard(row.Entry.Key, $"Copied key: {row.Entry.Key}");
            };

            // First realization sets DataContext right after LoadContent(); recycling
            // re-points it to a new row. Either way, (re)build the row contents.
            host.DataContextChanged += (_, _) => PopulateRow(host);

            // The generated ListBoxItem container defaults HorizontalContentAlignment to
            // Left, and its template's ContentPresenter is bound to that — so without this
            // the ContentPresenter shrink-wraps + left-aligns the row (collapsing the star
            // column, squashing the Copy button) instead of filling the list width. Once
            // we're in the tree, stretch the content host so each row spans the full width.
            // Done on Loaded (container present) and persists across recycling (the same
            // host/container/presenter are reused, only DataContext changes).
            host.Loaded += (_, _) => StretchRowContainer(host);

            return host;
        });
        template.Seal();
        return template;
    }

    /// <summary>
    /// Fills (or clears) a realized row host from its current <see cref="ResourceRow"/>
    /// DataContext: [swatch] [key + value] [Copy SplitButton], with alternating-row
    /// striping. The per-row child controls (SplitButton/MenuFlyout) are rebuilt fresh
    /// here; the old subtree is dropped with the previous <see cref="Border.Child"/>, so
    /// recycling never accumulates stale handlers.
    /// </summary>
    private void PopulateRow(Border host)
    {
        if (host.DataContext is not ResourceRow rowModel)
        {
            host.Child = null;
            host.Background = s_rowHitBrush;
            return;
        }

        var entry = rowModel.Entry;

        var grid = new Grid
        {
            Margin = new Thickness(0)
        };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(64) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Swatch
        var swatch = new Border
        {
            Background = entry.Preview,
            Width = 40,
            Height = 32,
            CornerRadius = new CornerRadius(6),
            Margin = new Thickness(12, 8, 12, 8),
            BorderThickness = new Thickness(1),
            BorderBrush = TryFindResource("ControlStrokeColorDefaultBrush") as Brush
                          ?? new SolidColorBrush(Color.FromArgb(0x33, 0x80, 0x80, 0x80))
        };
        Grid.SetColumn(swatch, 0);
        grid.Children.Add(swatch);

        // Key + value text stack
        var textStack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 8, 12, 8)
        };
        textStack.Children.Add(new TextBlock
        {
            Text = entry.Key,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            TextTrimming = TextTrimming.CharacterEllipsis
        });
        textStack.Children.Add(new TextBlock
        {
            Text = $"{KindLabel(entry.Kind)}  ·  {entry.Display}",
            FontSize = 11,
            Opacity = 0.65,
            FontFamily = "Cascadia Code,Consolas,Menlo,monospace",
            Margin = new Thickness(0, 2, 0, 0),
            TextTrimming = TextTrimming.CharacterEllipsis
        });
        Grid.SetColumn(textStack, 1);
        grid.Children.Add(textStack);

        // Copy SplitButton: primary action copies the key, flyout exposes alternates.
        var copySplit = BuildCopySplitButton(entry);
        Grid.SetColumn(copySplit, 2);
        grid.Children.Add(copySplit);

        // Alternating row background uses SubtleFillColorTertiaryBrush for striping
        // without hardcoding a non-themed color; even rows get a transparent (but
        // hit-testable) background so the whole row still responds to clicks.
        var rowBackground = (rowModel.RowIndex & 1) == 1
            ? TryFindResource("SubtleFillColorTertiaryBrush") as Brush
            : null;

        host.Background = rowBackground ?? s_rowHitBrush;
        host.Child = grid;
    }

    /// <summary>
    /// Builds the per-row SplitButton.
    /// Primary action = copy raw key.
    /// Flyout items = {ThemeResource} expression, hex/value string, and
    /// when applicable a StaticResource expression for completeness.
    /// </summary>
    private SplitButton BuildCopySplitButton(ResourceEntry entry)
    {
        var themeResourceExpr = "{ThemeResource " + entry.Key + "}";
        var staticResourceExpr = "{StaticResource " + entry.Key + "}";

        var flyout = new MenuFlyout();

        var copyExprItem = new MenuFlyoutItem { Text = "Copy {ThemeResource " + entry.Key + "}" };
        copyExprItem.Click += (_, _) => CopyToClipboard(themeResourceExpr, $"Copied: {themeResourceExpr}");
        flyout.Items.Add(copyExprItem);

        var copyStaticItem = new MenuFlyoutItem { Text = "Copy {StaticResource " + entry.Key + "}" };
        copyStaticItem.Click += (_, _) => CopyToClipboard(staticResourceExpr, $"Copied: {staticResourceExpr}");
        flyout.Items.Add(copyStaticItem);

        flyout.Items.Add(new Separator());

        var copyValueItem = new MenuFlyoutItem { Text = $"Copy value ({entry.Display})" };
        copyValueItem.Click += (_, _) => CopyToClipboard(entry.Display, $"Copied value: {entry.Display}");
        flyout.Items.Add(copyValueItem);

        var copyKeyItem = new MenuFlyoutItem { Text = "Copy key only" };
        copyKeyItem.Click += (_, _) => CopyToClipboard(entry.Key, $"Copied key: {entry.Key}");
        flyout.Items.Add(copyKeyItem);

        // NOTE: the row background uses a solid accent brush, NOT the {AccentBrush}
        // GradientBrush. A gradient Background on a Button/SplitButton suppresses the
        // primary button's content rendering in this framework (the "Copy" caption goes
        // invisible), so this control is given a solid accent fill instead. A fixed Width
        // is used because the SplitButton sits in an Auto grid column (which measures with
        // infinite width); without it the template's star-sized primary column collapses.
        var splitButton = new SplitButton
        {
            Content = "Copy",
            Width = 104,
            Margin = new Thickness(0, 8, 12, 8),
            FontSize = 12,
            Background = s_copyAccentBrush,
            BorderBrush = s_copyAccentBrush,
            Foreground = s_copyAccentTextBrush,
            Flyout = flyout
        };

        // Primary action: copy the raw key (matches the row click for muscle memory).
        splitButton.Click += (_, _) => CopyToClipboard(entry.Key, $"Copied key: {entry.Key}");

        return splitButton;
    }

    private static string KindLabel(ResourceKind kind)
    {
        return kind switch
        {
            ResourceKind.Color => "Color",
            ResourceKind.SolidBrush => "SolidColorBrush",
            ResourceKind.GradientBrush => "GradientBrush",
            _ => kind.ToString()
        };
    }

    private void CopyToClipboard(string text, string banner)
    {
        try
        {
            Clipboard.SetText(text);
            ShowCopyBanner(banner);
        }
        catch (Exception ex)
        {
            ShowCopyBanner($"Copy failed: {ex.Message}");
        }
    }

    private void ShowCopyBanner(string message)
    {
        if (CopyBanner == null || CopyBannerText == null)
            return;

        CopyBannerText.Text = message;
        CopyBanner.Visibility = Visibility.Visible;
    }
}
