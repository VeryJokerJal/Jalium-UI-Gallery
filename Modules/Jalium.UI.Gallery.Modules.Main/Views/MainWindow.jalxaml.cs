using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Gallery.Services.Interfaces;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Jalium.UI.Gallery.Modules.Main.Views;

/// <summary>
/// Primary shell window for the Jalium.UI Gallery. The layout root lives in the
/// companion <c>MainWindow.jalxaml</c>; code-behind drives the navigation tree,
/// title-bar search, theme toggle, and page instantiation.
/// </summary>
public partial class MainWindow : Window
{
    private readonly Dictionary<NavigationViewItem, string> _itemTags = new();
    private readonly Dictionary<NavigationViewItem, List<NavigationViewItem>> _childrenByGroup = new();
    private readonly Dictionary<NavigationViewItem, NavigationViewItem> _parentByChild = new();
    private readonly List<SearchEntry> _searchEntries = new();
    private readonly Dictionary<NavigationViewItem, bool> _groupExpandedSnapshot = new();
    private readonly ScrollViewer _contentHost = CreateContentHost();
    private TextBox? _titleBarSearchBox;
    private Border? _sectionBadge;
    private string _currentPageTag = "home";
    private bool _isSearchFilterActive;
    private static readonly Dictionary<string, BitmapImage> _embeddedBitmapCache = new();

    private sealed class SearchEntry
    {
        public SearchEntry(
            NavigationViewItem item,
            string displayText,
            string displayPath,
            string tag,
            bool isLeaf,
            int order)
        {
            Item = item;
            DisplayText = displayText;
            DisplayPath = displayPath;
            Tag = tag;
            IsLeaf = isLeaf;
            Order = order;
        }

        public NavigationViewItem Item { get; }
        public string DisplayText { get; }
        public string DisplayPath { get; }
        public string Tag { get; }
        public bool IsLeaf { get; }
        public int Order { get; }
    }

    // All pages use code-behind (x:Class + .jalxaml.cs)
    // Category pages and HomePage are handled specially to wire up navigation events
    private static readonly Dictionary<string, Func<UIElement>> Pages = new()
    {
        // These are placeholders - created in NavigateToPage with event wiring
        { "home", () => null! },
        { "basic", () => null! },
        { "text", () => null! },
        { "layout", () => null! },
        { "navigation", () => null! },
        { "media", () => null! },
        { "collections", () => null! },
        { "data", () => null! },
        { "menustoolbars", () => null! },
        { "icons", () => null! },
        { "datetime", () => null! },
        { "pickers", () => null! },
        { "overlays", () => null! },
        { "statusinfo", () => null! },
        { "dialogscategory", () => null! },
        { "effects", () => null! },
        { "system", () => null! },
        // Static pages
        { "getting-started", () => new GettingStartedPage() },
        // Individual control pages
        { "button", () => new ButtonPage() },
        { "checkbox", () => new CheckBoxPage() },
        { "radiobutton", () => new RadioButtonPage() },
        { "slider", () => new SliderPage() },
        { "progressbar", () => new ProgressBarPage() },
        { "textbox", () => new TextBoxPage() },
        { "passwordbox", () => new PasswordBoxPage() },
        { "combobox", () => new ComboBoxPage() },
        { "textblock", () => new TextBlockPage() },
        { "markdown", () => new MarkdownPage() },
        { "binding", () => new BindingPage() },
        { "razor", () => new RazorDemoPage() },
        { "section", () => new SectionDemoPage() },
        { "stackpanel", () => new StackPanelPage() },
        { "grid", () => new GridPage() },
        { "canvas", () => new CanvasPage() },
        { "border", () => new BorderPage() },
        { "dockpanel", () => new DockPanelPage() },
        { "wrappanel", () => new WrapPanelPage() },
        { "scrollviewer", () => new ScrollViewerPage() },
        { "tabcontrol", () => new TabControlPage() },
        { "treeview", () => new TreeViewPage() },
        { "image", () => new ImagePage() },
        { "qrcode", () => new QRCodePage() },
        { "listbox", () => new ListBoxPage() },
        { "backdropeffects", () => new BackdropEffectsPage() },
        { "datagrid", () => new DataGridPage() },
        { "treedatagrid", () => new TreeDataGridPage() },
        { "autocompletebox", () => new AutoCompleteBoxPage() },
        { "groupbox", () => new GroupBoxPage() },
        { "statusbar", () => new StatusBarPage() },
        { "label", () => new LabelPage() },
        { "separator", () => new SeparatorPage() },
        { "hyperlinkbutton", () => new HyperlinkButtonPage() },
        { "splitbutton", () => new SplitButtonPage() },
        { "toggleswitch", () => new ToggleSwitchPage() },
        { "numberbox", () => new NumberBoxPage() },
        { "repeatbutton", () => new RepeatButtonPage() },
        { "thumb", () => new ThumbPage() },
        { "calendar", () => new CalendarPage() },
        { "infobar", () => new InfoBarPage() },
        // New WPF parity features
        { "shadereffects", () => new ShaderEffectsPage() },
        { "elementeffects", () => new ElementEffectsPage() },
        { "navigationdemo", () => new NavigationDemoPage() },
        { "printing", () => new PrintingPage() },
        { "shellintegration", () => new ShellIntegrationPage() },
        { "titlebar", () => new TitleBarPage() },
        // New control pages
        { "menu", () => new MenuPage() },
        { "contextmenu", () => new ContextMenuPage() },
        { "toolbar", () => new ToolBarPage() },
        { "tooltip", () => new ToolTipPage() },
        { "popup", () => new PopupPage() },
        { "expander", () => new ExpanderPage() },
        { "datepicker", () => new DatePickerPage() },
        { "timepicker", () => new TimePickerPage() },
        { "colorpicker", () => new ColorPickerPage() },
        { "richtextbox", () => new RichTextBoxPage() },
        { "shapes", () => new ShapesPage() },
        { "frame", () => new FramePage() },
        { "viewbox", () => new ViewboxPage() },
        { "listview", () => new ListViewPage() },
        { "mediaelement", () => new MediaElementPage() },
        { "dialogs", () => new DialogsPage() },
        { "contentdialog", () => new ContentDialogPage() },
        { "toastnotification", () => new ToastNotificationPage() },
        { "splitter", () => new SplitterPage() },
        { "inkcanvas", () => new InkCanvasPage() },
        { "editcontrol", () => new EditControlPage() },
        { "texteffectpresenter", () => new TextEffectPresenterPage() },
        { "liquidglass", () => new LiquidGlassPage() },
        { "docklayout", () => new DockLayoutPage() },
        { "webview", () => new WebViewPage() },
        { "transitions", () => new TransitionDemoPage() },
        // Menus & Toolbars pages
        { "appbarbutton", () => new AppBarButtonPage() },
        { "appbarseparator", () => new AppBarSeparatorPage() },
        { "appbartogglebutton", () => new AppBarToggleButtonPage() },
        { "commandbar", () => new CommandBarPage() },
        { "commandbarflyout", () => new CommandBarFlyoutPage() },
        { "menubar", () => new MenuBarPage() },
        { "menuflyout", () => new MenuFlyoutPage() },
        { "swipecontrol", () => new SwipeControlPage() },
        { "standarduicommand", () => new StandardUICommandPage() },
        { "xamluicommand", () => new XamlUICommandPage() },
        // Icons
        { "iconelement", () => new IconElementPage() },
        // Drag & Drop
        { "dragdrop", () => new DragDropPage() },
        // Terminal
        { "terminal", () => new TerminalPage() },
        // Data Viewers
        { "diffviewer", () => new DiffViewerPage() },
        { "hexeditor", () => new HexEditorPage() },
        { "jsontreeviewer", () => new JsonTreeViewerPage() },
        { "propertygrid", () => new PropertyGridPage() },
        // Charts
        { "linechart", () => new LineChartPage() },
        { "barchart", () => new BarChartPage() },
        { "piechart", () => new PieChartPage() },
        { "scatterplot", () => new ScatterPlotPage() },
        { "heatmap", () => new HeatmapPage() },
        { "sparkline", () => new SparklinePage() },
        { "gaugechart", () => new GaugeChartPage() },
        { "treemap", () => new TreeMapPage() },
        { "candlestickchart", () => new CandlestickChartPage() },
        { "networkgraph", () => new NetworkGraphPage() },
        { "ganttchart", () => new GanttChartPage() },
        { "sankeydiagram", () => new SankeyDiagramPage() },
        // Maps
        { "mapview", () => new MapViewPage() },
        { "minimap", () => new MiniMapPage() },
        { "geographicheatmap", () => new GeographicHeatmapPage() },
        // System Notifications
        { "systemnotification", () => new SystemNotificationPage() },
        { "notifyicon", () => new NotifyIconPage() },
        // Ribbon
        { "ribbon", () => new RibbonPage() },
        // Document Viewer
        { "documentviewer", () => new DocumentViewerPage() },
        // UniformGrid
        { "uniformgrid", () => new UniformGridPage() },
        // Category placeholders
        { "dataviewers", () => null! },
        { "charts", () => null! },
        { "maps", () => null! }
    };

    private readonly ViewAViewModel _viewModel;

    public MainWindow(IMessageService messageService)
    {
        InitializeComponent();

        // StartupUri creates the window via a parameterless constructor, so we
        // pull IMessageService off the application-scoped service provider.
        // MainModule stays the single composition entry for code-behind DI wiring.
        _viewModel = MainModule.CreateViewA(messageService);
        DataContext = _viewModel;

        // Setting the window Foreground seeds the Foreground inheritance chain
        // so every descendant Control (Button, ToggleSwitch, ListBox, ...) that
        // doesn't explicitly override Foreground picks up the active gallery
        // text brush instead of the stale Dark-mode default on Control.Foreground.
        Foreground = GalleryTheme.TextPrimaryBrush;

        BuildTitleBarCommands();
        AddNavigationItems();
        BuildSearchEntries();
        InitializeTitleBarSearch();

        if (NavigationView != null)
        {
            NavigationView.SelectionChanged += OnSelectionChanged;
            NavigationView.PaneHeader = BuildPaneHeader();
            NavigationView.PaneFooter = BuildPaneFooter();
            NavigationView.PaneBackground = GalleryTheme.BackgroundMediumBrush;
            NavigationView.ContentBackground = GalleryTheme.TransparentBrush;
        }

        GalleryTheme.ModeChanged += OnGalleryModeChanged;
        Closed += (_, _) => GalleryTheme.ModeChanged -= OnGalleryModeChanged;

        NavigateToPage("home");

        // Embedded WebView2 (child HWND) requires redirected parent HWND.
        // Keep backdrop disabled in Gallery default config.
        SystemBackdrop = WindowBackdropType.None;
    }

    private void OnGalleryModeChanged(object? sender, EventArgs e)
    {
        // Push the new gallery palette into Application.Resources so every
        // {DynamicResource Gallery...} reference in jalxaml re-binds itself.
        if (Application.Current is { } app)
        {
            GalleryTheme.RegisterResources(app.Resources);
        }

        // Re-skin everything that sampled a brush eagerly.
        Background = GalleryTheme.BackgroundDarkBrush;
        Foreground = GalleryTheme.TextPrimaryBrush;
        _contentHost.Background = GalleryTheme.ShellBackgroundBrush;

        if (NavigationView != null)
        {
            NavigationView.PaneBackground = GalleryTheme.BackgroundMediumBrush;
            NavigationView.ContentBackground = GalleryTheme.TransparentBrush;
            NavigationView.PaneHeader = BuildPaneHeader();
            NavigationView.PaneFooter = BuildPaneFooter();

            // The framework theme switch re-applies NavigationView's template,
            // which leaves _menuItemsPanel / _footerItemsPanel holding brand-new
            // (empty) panels. Without this forced refresh the items still live in
            // the previous template parts and the sidebar appears unresponsive.
            NavigationView.UpdateMenuItems();

            // Re-sync the expanded state for the selected group so its children
            // are reattached under the newly-refreshed template parts.
            RestoreGroupExpansionAfterRetemplate();
        }

        // Rebuild the chrome (title bar) so the yellow badge / theme chip
        // pick up the new palette.
        BuildTitleBarCommands();
        InitializeTitleBarSearch();

        // Refresh section-badge visibility (it was just recreated).
        if (_sectionBadge != null)
        {
            _sectionBadge.Visibility = _currentPageTag == "section"
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Rebuild the currently visible page so its brushes flip too.
        NavigateToPage(_currentPageTag);
    }

    /// <summary>
    /// After the framework re-applies a control template (e.g. during a theme
    /// switch) a NavigationViewItem's children panel can end up empty. Toggling
    /// <c>IsExpanded</c> round-trips through Jalium's AddItemWithChildren path
    /// which rewires the children into the fresh template parts.
    /// </summary>
    private void RestoreGroupExpansionAfterRetemplate()
    {
        foreach (var group in _childrenByGroup.Keys)
        {
            if (!group.IsExpanded) continue;

            // Flip off/on to force the hierarchy to rebuild.
            group.IsExpanded = false;
            group.IsExpanded = true;
        }
    }

    private void BuildTitleBarCommands()
    {
        var leftStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0, 0, 0)
        };

        _sectionBadge = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x1B, 0x2F, 0x1F)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x2F, 0x4F, 0x34)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(999),
            Padding = new Thickness(12, 6, 12, 6),
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Center,
            // Shown only while the Section demo page is active.
            Visibility = Visibility.Collapsed
        };
        var sectionStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };
        sectionStack.Children.Add(new Border
        {
            Width = 8,
            Height = 8,
            Background = GalleryTheme.SuccessBrush,
            CornerRadius = new CornerRadius(999),
            Margin = new Thickness(0, 0, 8, 0),
            VerticalAlignment = VerticalAlignment.Center
        });
        sectionStack.Children.Add(new TextBlock
        {
            Text = "Section",
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x86, 0xEF, 0xAC)),
            VerticalAlignment = VerticalAlignment.Center
        });
        _sectionBadge.Child = sectionStack;
        leftStack.Children.Add(_sectionBadge);

        _titleBarSearchBox = new TextBox
        {
            Width = 320,
            Height = 36,
            PlaceholderText = "Search controls and categories...",
            VerticalAlignment = VerticalAlignment.Center
        };
        leftStack.Children.Add(_titleBarSearchBox);

        LeftWindowCommands = leftStack;

        var rightStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0)
        };

        rightStack.Children.Add(BuildThemeToggle());

        RightWindowCommands = rightStack;
    }

    /// <summary>
    /// Builds a pill-shaped theme toggle chip: an amber glyph disk + label,
    /// with hover/press feedback. Clicking flips both the framework
    /// <see cref="ThemeManager"/> and the gallery <see cref="GalleryTheme"/>
    /// palettes so the entire shell re-skins in one step.
    /// </summary>
    private Border BuildThemeToggle()
    {
        var chip = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(999),
            Padding = new Thickness(5, 4, 14, 4),
            VerticalAlignment = VerticalAlignment.Center,
            Cursor = Cursors.Hand
        };

        var layout = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };

        var iconDisc = new Border
        {
            Width = 28,
            Height = 28,
            Background = GalleryTheme.AccentPrimaryBrush,
            CornerRadius = new CornerRadius(999),
            Margin = new Thickness(0, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var iconGlyph = new TextBlock
        {
            FontSize = 15,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(0x0B, 0x08, 0x14)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        iconDisc.Child = iconGlyph;
        layout.Children.Add(iconDisc);

        var label = new TextBlock
        {
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextSecondaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        layout.Children.Add(label);

        chip.Child = layout;

        ApplyThemeToggleVisual(iconGlyph, label);

        chip.MouseEnter += (_, _) =>
        {
            chip.Background = GalleryTheme.BackgroundHoverBrush;
            chip.BorderBrush = GalleryTheme.BorderStrongBrush;
        };
        chip.MouseLeave += (_, _) =>
        {
            chip.Background = GalleryTheme.BackgroundCardBrush;
            chip.BorderBrush = GalleryTheme.BorderDefaultBrush;
        };
        chip.MouseDown += (_, e) =>
        {
            if (e is not MouseButtonEventArgs m || m.ChangedButton != MouseButton.Left) return;

            chip.Background = GalleryTheme.BackgroundPressedBrush;
            ToggleTheme();
        };
        chip.MouseUp += (_, e) =>
        {
            if (e is not MouseButtonEventArgs m || m.ChangedButton != MouseButton.Left) return;
            chip.Background = chip.IsMouseOver
                ? GalleryTheme.BackgroundHoverBrush
                : GalleryTheme.BackgroundCardBrush;
        };

        return chip;
    }

    private void ToggleTheme()
    {
        var nextFramework = ThemeManager.CurrentTheme == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;

        // Detach every NavigationViewItem from its current panel *before* the
        // framework re-applies NavigationView's control template. Without this
        // step, the old template root is removed wholesale but its child items
        // keep their _parent pointer, which causes the subsequent RefreshMenuItems
        // to throw on the first Add ("Visual already has a parent") — the exact
        // symptom we saw as "sidebar loses most items after theme switch".
        DetachAllNavigationItemsFromVisualTree();

        ThemeManager.ApplyTheme(nextFramework);

        GalleryTheme.CurrentMode = nextFramework == ThemeVariant.Dark
            ? GalleryThemeMode.Dark
            : GalleryThemeMode.Light;
    }

    /// <summary>
    /// Walk every NavigationViewItem registered in our tag map and, if it's
    /// currently a child of a Panel, remove it from that panel so its
    /// <c>_parent</c> backing field goes back to null. <see cref="RestoreGroupExpansionAfterRetemplate"/>
    /// later re-runs the Jalium hierarchy builder which will re-attach them
    /// under the freshly templated parts.
    /// </summary>
    private void DetachAllNavigationItemsFromVisualTree()
    {
        foreach (var item in _itemTags.Keys)
        {
            if (item.VisualParent is Panel panel)
            {
                panel.Children.Remove(item);
            }
        }
    }

    private static void ApplyThemeToggleVisual(TextBlock iconGlyph, TextBlock label)
    {
        if (GalleryTheme.CurrentMode == GalleryThemeMode.Dark)
        {
            iconGlyph.Text = "\u263D"; // waxing crescent
            label.Text = "Light mode";
        }
        else
        {
            iconGlyph.Text = "\u2600"; // sun
            label.Text = "Dark mode";
        }
    }

    private static BitmapImage LoadEmbeddedBitmap(string resourceName)
    {
        if (_embeddedBitmapCache.TryGetValue(resourceName, out var cached))
        {
            return cached;
        }

        var assembly = typeof(MainWindow).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' was not found in {assembly.FullName}.");

        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var rawBytes = memory.ToArray();

        var bitmap = resourceName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
            ? IcoImageLoader.Load(rawBytes)
            : BitmapImage.FromBytes(rawBytes);

        _embeddedBitmapCache[resourceName] = bitmap;
        return bitmap;
    }

    private static void OpenExternalUrl(string url)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Gallery] Failed to open URL '{url}': {ex}");
        }
    }

    private Border BuildPaneHeader()
    {
        var header = new Border
        {
            Margin = new Thickness(16, 20, 16, 16),
            Padding = new Thickness(14, 14, 14, 14),
            Background = GalleryTheme.BackgroundMediumBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge)
        };

        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };

        row.Children.Add(new Image
        {
            Source = LoadEmbeddedBitmap("Jalium.UI.Gallery.Modules.Main.Assets.logo.png"),
            Stretch = Stretch.Uniform,
            Width = 38,
            Height = 38,
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Center
        });

        var textStack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };
        textStack.Children.Add(new TextBlock
        {
            Text = "JALIUM",
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 2)
        });
        textStack.Children.Add(new TextBlock
        {
            Text = "Control Gallery",
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextTertiaryBrush
        });
        row.Children.Add(textStack);

        header.Child = row;
        return header;
    }

    private Border BuildPaneFooter()
    {
        var footer = new Border
        {
            Margin = new Thickness(16, 20, 16, 20),
            Padding = new Thickness(18, 20, 18, 20),
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        stack.Children.Add(new Border
        {
            Width = 54,
            Height = 54,
            Background = GalleryTheme.BackgroundHoverBrush,
            CornerRadius = new CornerRadius(14),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 14),
            Padding = new Thickness(8),
            Child = new Image
            {
                Source = LoadEmbeddedBitmap("Jalium.UI.Gallery.Modules.Main.Assets.Jalium_IDE_logo.ico"),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        stack.Children.Add(new TextBlock
        {
            Text = "Build Fast",
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextTertiaryBrush
        });
        stack.Children.Add(new TextBlock
        {
            Text = "Native UI Platform",
            FontSize = 15,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 2, 0, 14)
        });

        var startButton = new Button
        {
            Content = "Start your project",
            Background = GalleryTheme.AccentPrimaryBrush,
            Foreground = new SolidColorBrush(Color.FromRgb(0x0B, 0x08, 0x14)),
            BorderThickness = new Thickness(0),
            FontWeight = FontWeights.Bold,
            FontSize = 13,
            Height = 38,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        startButton.Click += (_, _) => OpenExternalUrl("http://jaliumui.top/downloads");
        stack.Children.Add(startButton);

        footer.Child = stack;
        return footer;
    }

    private void AddNavigationItems()
    {
        if (NavigationView == null) return;

        // Home
        AddItem("Home", "home");
        AddItem("Getting Started", "getting-started");

        // Controls (expandable group) - clicking group navigates to category overview
        var controlsGroup = AddGroupItem("Basic", "basic");
        AddChildItem(controlsGroup, "Button", "button");
        AddChildItem(controlsGroup, "CheckBox", "checkbox");
        AddChildItem(controlsGroup, "RadioButton", "radiobutton");
        AddChildItem(controlsGroup, "Slider", "slider");
        AddChildItem(controlsGroup, "ProgressBar", "progressbar");
        AddChildItem(controlsGroup, "TextBox", "textbox");
        AddChildItem(controlsGroup, "PasswordBox", "passwordbox");
        AddChildItem(controlsGroup, "ComboBox", "combobox");
        AddChildItem(controlsGroup, "AutoCompleteBox", "autocompletebox");
        AddChildItem(controlsGroup, "ToggleSwitch", "toggleswitch");
        AddChildItem(controlsGroup, "NumberBox", "numberbox");
        AddChildItem(controlsGroup, "RepeatButton", "repeatbutton");
        AddChildItem(controlsGroup, "HyperlinkButton", "hyperlinkbutton");
        AddChildItem(controlsGroup, "SplitButton", "splitbutton");
        AddChildItem(controlsGroup, "Drag & Drop", "dragdrop");

        // Text (expandable group) - clicking group navigates to category overview
        var textGroup = AddGroupItem("Text", "text");
        AddChildItem(textGroup, "TextBlock", "textblock");
        AddChildItem(textGroup, "Markdown", "markdown");
        AddChildItem(textGroup, "Label", "label");
        AddChildItem(textGroup, "RichTextBox", "richtextbox");
        AddChildItem(textGroup, "EditControl", "editcontrol");
        AddChildItem(textGroup, "TextEffectPresenter", "texteffectpresenter");

        // Data (expandable group) - data binding and Razor features
        var dataGroup = AddGroupItem("Data", "data");
        AddChildItem(dataGroup, "Binding", "binding");
        AddChildItem(dataGroup, "Razor Syntax", "razor");
        AddChildItem(dataGroup, "Section", "section");

        // Layout (expandable group) - clicking group navigates to category overview
        var layoutGroup = AddGroupItem("Layout", "layout");
        AddChildItem(layoutGroup, "StackPanel", "stackpanel");
        AddChildItem(layoutGroup, "Grid", "grid");
        AddChildItem(layoutGroup, "Canvas", "canvas");
        AddChildItem(layoutGroup, "Border", "border");
        AddChildItem(layoutGroup, "DockPanel", "dockpanel");
        AddChildItem(layoutGroup, "WrapPanel", "wrappanel");
        AddChildItem(layoutGroup, "UniformGrid", "uniformgrid");
        AddChildItem(layoutGroup, "ScrollViewer", "scrollviewer");
        AddChildItem(layoutGroup, "Expander", "expander");
        AddChildItem(layoutGroup, "GroupBox", "groupbox");
        AddChildItem(layoutGroup, "Separator", "separator");
        AddChildItem(layoutGroup, "Viewbox", "viewbox");
        AddChildItem(layoutGroup, "Splitter", "splitter");
        AddChildItem(layoutGroup, "DockLayout", "docklayout");

        // Navigation (expandable group) - clicking group navigates to category overview
        var navigationGroup = AddGroupItem("Navigation", "navigation");
        AddChildItem(navigationGroup, "TabControl", "tabcontrol");
        AddChildItem(navigationGroup, "Frame", "frame");
        AddChildItem(navigationGroup, "Menu", "menu");
        AddChildItem(navigationGroup, "ContextMenu", "contextmenu");
        AddChildItem(navigationGroup, "ToolBar", "toolbar");

        // Menus & Toolbars (expandable group)
        var menusToolbarsGroup = AddGroupItem("Menus & Toolbars", "menustoolbars");
        AddChildItem(menusToolbarsGroup, "AppBarButton", "appbarbutton");
        AddChildItem(menusToolbarsGroup, "AppBarSeparator", "appbarseparator");
        AddChildItem(menusToolbarsGroup, "AppBarToggleButton", "appbartogglebutton");
        AddChildItem(menusToolbarsGroup, "CommandBar", "commandbar");
        AddChildItem(menusToolbarsGroup, "CommandBarFlyout", "commandbarflyout");
        AddChildItem(menusToolbarsGroup, "MenuBar", "menubar");
        AddChildItem(menusToolbarsGroup, "MenuFlyout", "menuflyout");
        AddChildItem(menusToolbarsGroup, "Ribbon", "ribbon");
        AddChildItem(menusToolbarsGroup, "SwipeControl", "swipecontrol");
        AddChildItem(menusToolbarsGroup, "StandardUICommand", "standarduicommand");
        AddChildItem(menusToolbarsGroup, "XamlUICommand", "xamluicommand");

        // Icons (expandable group)
        var iconsGroup = AddGroupItem("Icons", "icons");
        AddChildItem(iconsGroup, "IconElement", "iconelement");

        // Media (expandable group) - clicking group navigates to category overview
        var mediaGroup = AddGroupItem("Media", "media");
        AddChildItem(mediaGroup, "Image", "image");
        AddChildItem(mediaGroup, "QRCode", "qrcode");
        AddChildItem(mediaGroup, "MediaElement", "mediaelement");
        AddChildItem(mediaGroup, "Shapes", "shapes");
        AddChildItem(mediaGroup, "InkCanvas", "inkcanvas");
        AddChildItem(mediaGroup, "WebView", "webview");

        // Collections (expandable group) - clicking group navigates to category overview
        var collectionsGroup = AddGroupItem("Collections", "collections");
        AddChildItem(collectionsGroup, "ListBox", "listbox");
        AddChildItem(collectionsGroup, "ListView", "listview");
        AddChildItem(collectionsGroup, "TreeView", "treeview");
        AddChildItem(collectionsGroup, "DataGrid", "datagrid");
        AddChildItem(collectionsGroup, "TreeDataGrid", "treedatagrid");
        AddChildItem(collectionsGroup, "Calendar", "calendar");

        // Date & Time (expandable group)
        var dateTimeGroup = AddGroupItem("Date & Time", "datetime");
        AddChildItem(dateTimeGroup, "DatePicker", "datepicker");
        AddChildItem(dateTimeGroup, "TimePicker", "timepicker");
        AddChildItem(dateTimeGroup, "Calendar", "calendar");

        // Pickers (expandable group)
        var pickersGroup = AddGroupItem("Pickers", "pickers");
        AddChildItem(pickersGroup, "ColorPicker", "colorpicker");

        // Overlays (expandable group)
        var overlaysGroup = AddGroupItem("Overlays", "overlays");
        AddChildItem(overlaysGroup, "Popup", "popup");
        AddChildItem(overlaysGroup, "ToolTip", "tooltip");
        AddChildItem(overlaysGroup, "ToastNotification", "toastnotification");
        AddChildItem(overlaysGroup, "SystemNotification", "systemnotification");
        AddChildItem(overlaysGroup, "NotifyIcon", "notifyicon");

        // Status & Info (expandable group)
        var statusGroup = AddGroupItem("Status & Info", "statusinfo");
        AddChildItem(statusGroup, "StatusBar", "statusbar");
        AddChildItem(statusGroup, "InfoBar", "infobar");
        AddChildItem(statusGroup, "Thumb", "thumb");

        // Dialogs (expandable group)
        var dialogsGroup = AddGroupItem("Dialogs", "dialogscategory");
        AddChildItem(dialogsGroup, "File Dialogs", "dialogs");
        AddChildItem(dialogsGroup, "ContentDialog", "contentdialog");

        // Effects (expandable group)
        var effectsGroup = AddGroupItem("Effects", "effects");
        AddChildItem(effectsGroup, "Backdrop Effects", "backdropeffects");
        AddChildItem(effectsGroup, "Liquid Glass", "liquidglass");
        AddChildItem(effectsGroup, "Shader Effects", "shadereffects");
        AddChildItem(effectsGroup, "Element Effects", "elementeffects");
        AddChildItem(effectsGroup, "Content Transitions", "transitions");

        // System (expandable group) - new WPF parity features
        var systemGroup = AddGroupItem("System", "system");
        AddChildItem(systemGroup, "Navigation", "navigationdemo");
        AddChildItem(systemGroup, "Printing", "printing");
        AddChildItem(systemGroup, "Shell Integration", "shellintegration");
        AddChildItem(systemGroup, "TitleBar", "titlebar");
        AddChildItem(systemGroup, "Terminal", "terminal");

        // Data Viewers (expandable group)
        var dataViewersGroup = AddGroupItem("Data Viewers", "dataviewers");
        AddChildItem(dataViewersGroup, "DocumentViewer", "documentviewer");
        AddChildItem(dataViewersGroup, "DiffViewer", "diffviewer");
        AddChildItem(dataViewersGroup, "HexEditor", "hexeditor");
        AddChildItem(dataViewersGroup, "JsonTreeViewer", "jsontreeviewer");
        AddChildItem(dataViewersGroup, "PropertyGrid", "propertygrid");

        // Charts (expandable group)
        var chartsGroup = AddGroupItem("Charts", "charts");
        AddChildItem(chartsGroup, "LineChart", "linechart");
        AddChildItem(chartsGroup, "BarChart", "barchart");
        AddChildItem(chartsGroup, "PieChart", "piechart");
        AddChildItem(chartsGroup, "ScatterPlot", "scatterplot");
        AddChildItem(chartsGroup, "Heatmap", "heatmap");
        AddChildItem(chartsGroup, "Sparkline", "sparkline");
        AddChildItem(chartsGroup, "GaugeChart", "gaugechart");
        AddChildItem(chartsGroup, "TreeMap", "treemap");
        AddChildItem(chartsGroup, "CandlestickChart", "candlestickchart");
        AddChildItem(chartsGroup, "NetworkGraph", "networkgraph");
        AddChildItem(chartsGroup, "GanttChart", "ganttchart");
        AddChildItem(chartsGroup, "SankeyDiagram", "sankeydiagram");

        // Maps (expandable group)
        var mapsGroup = AddGroupItem("Maps", "maps");
        AddChildItem(mapsGroup, "MapView", "mapview");
        AddChildItem(mapsGroup, "MiniMap", "minimap");
        AddChildItem(mapsGroup, "GeographicHeatmap", "geographicheatmap");

        // Update the visual tree
        NavigationView.UpdateMenuItems();
    }

    private void InitializeTitleBarSearch()
    {
        // The search box was created in BuildTitleBarCommands.
        if (_titleBarSearchBox == null) return;

        _titleBarSearchBox.TextChanged += OnTitleBarSearchTextChanged;
        _titleBarSearchBox.KeyDown += OnTitleBarSearchKeyDown;
    }

    private void OnTitleBarSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyNavigationFilter(_titleBarSearchBox?.Text ?? string.Empty);
    }

    private void OnTitleBarSearchKeyDown(object? sender, RoutedEventArgs e)
    {
        if (_titleBarSearchBox == null) return;
        if (e is not KeyEventArgs keyArgs) return;

        if (keyArgs.Key == Key.Enter)
        {
            if (TryNavigateToBestMatch(_titleBarSearchBox.Text ?? string.Empty))
            {
                keyArgs.Handled = true;
            }

            return;
        }

        if (keyArgs.Key == Key.Escape && (!string.IsNullOrWhiteSpace(_titleBarSearchBox.Text) || _isSearchFilterActive))
        {
            _titleBarSearchBox.Text = string.Empty;
            ApplyNavigationFilter(string.Empty);
            keyArgs.Handled = true;
        }
    }

    private void BuildSearchEntries()
    {
        _searchEntries.Clear();

        if (NavigationView == null) return;

        var order = 0;
        foreach (var menuItem in NavigationView.MenuItems)
        {
            if (menuItem is NavigationViewItem navItem)
            {
                BuildSearchEntriesCore(navItem, string.Empty, ref order);
            }
        }
    }

    private void BuildSearchEntriesCore(NavigationViewItem item, string parentPath, ref int order)
    {
        var displayText = GetNavigationItemText(item);
        var displayPath = string.IsNullOrEmpty(parentPath) ? displayText : $"{parentPath} / {displayText}";

        if (_itemTags.TryGetValue(item, out var tag))
        {
            _searchEntries.Add(new SearchEntry(
                item,
                displayText,
                displayPath,
                tag,
                item.MenuItems.Count == 0,
                order));

            order++;
        }

        foreach (var child in item.MenuItems)
        {
            BuildSearchEntriesCore(child, displayPath, ref order);
        }
    }

    private static string GetNavigationItemText(NavigationViewItem item)
    {
        return item.Content?.ToString() ?? string.Empty;
    }

    private void ApplyNavigationFilter(string query)
    {
        if (NavigationView == null) return;

        var normalizedQuery = query.Trim();
        if (normalizedQuery.Length == 0)
        {
            foreach (var item in _itemTags.Keys)
            {
                item.Visibility = Visibility.Visible;
            }

            if (_isSearchFilterActive)
            {
                foreach (var snapshot in _groupExpandedSnapshot)
                {
                    snapshot.Key.IsExpanded = snapshot.Value;
                }
            }

            _groupExpandedSnapshot.Clear();
            _isSearchFilterActive = false;

            NavigationView.UpdateMenuItems();
            return;
        }

        if (!_isSearchFilterActive)
        {
            _groupExpandedSnapshot.Clear();
            foreach (var group in _childrenByGroup.Keys)
            {
                _groupExpandedSnapshot[group] = group.IsExpanded;
            }

            _isSearchFilterActive = true;
        }

        var matchedItems = new HashSet<NavigationViewItem>();
        foreach (var entry in _searchEntries)
        {
            if (IsSearchMatch(entry, normalizedQuery))
            {
                matchedItems.Add(entry.Item);
            }
        }

        foreach (var item in _itemTags.Keys)
        {
            item.Visibility = Visibility.Collapsed;
        }

        foreach (var groupPair in _childrenByGroup)
        {
            var group = groupPair.Key;
            var children = groupPair.Value;

            var groupMatched = matchedItems.Contains(group);
            var hasMatchedChild = false;

            foreach (var child in children)
            {
                var childMatched = matchedItems.Contains(child);
                child.Visibility = childMatched ? Visibility.Visible : Visibility.Collapsed;
                hasMatchedChild |= childMatched;
            }

            var shouldShowGroup = groupMatched || hasMatchedChild;
            group.Visibility = shouldShowGroup ? Visibility.Visible : Visibility.Collapsed;
            group.IsExpanded = shouldShowGroup;
        }

        foreach (var item in _itemTags.Keys)
        {
            if (_childrenByGroup.ContainsKey(item) || _parentByChild.ContainsKey(item))
            {
                continue;
            }

            item.Visibility = matchedItems.Contains(item) ? Visibility.Visible : Visibility.Collapsed;
        }

        NavigationView.UpdateMenuItems();
    }

    private bool TryNavigateToBestMatch(string query)
    {
        var normalizedQuery = query.Trim();
        if (normalizedQuery.Length == 0)
        {
            return false;
        }

        SearchEntry? bestEntry = null;
        var bestScore = int.MinValue;

        foreach (var entry in _searchEntries)
        {
            var score = GetSearchScore(entry, normalizedQuery);
            if (score <= 0)
            {
                continue;
            }

            if (bestEntry == null
                || score > bestScore
                || (score == bestScore && entry.IsLeaf && !bestEntry.IsLeaf)
                || (score == bestScore && entry.IsLeaf == bestEntry.IsLeaf && entry.Order < bestEntry.Order))
            {
                bestEntry = entry;
                bestScore = score;
            }
        }

        if (bestEntry == null)
        {
            return false;
        }

        if (_parentByChild.TryGetValue(bestEntry.Item, out var parent))
        {
            parent.IsExpanded = true;
        }

        NavigateToPage(bestEntry.Tag);
        SelectNavigationItem(bestEntry.Tag);
        return true;
    }

    private static bool IsSearchMatch(SearchEntry entry, string query)
    {
        return entry.DisplayText.Contains(query, StringComparison.OrdinalIgnoreCase)
               || entry.DisplayPath.Contains(query, StringComparison.OrdinalIgnoreCase)
               || entry.Tag.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    private static int GetSearchScore(SearchEntry entry, string query)
    {
        if (entry.DisplayText.Equals(query, StringComparison.OrdinalIgnoreCase))
        {
            return 400;
        }

        if (entry.DisplayText.StartsWith(query, StringComparison.OrdinalIgnoreCase))
        {
            return 300;
        }

        if (entry.DisplayText.Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            return 200;
        }

        if (entry.Tag.Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        if (entry.DisplayPath.Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            return 50;
        }

        return 0;
    }

    private void AddItem(string name, string tag)
    {
        if (NavigationView == null) return;

        var item = new NavigationViewItem
        {
            Content = name
        };

        _itemTags[item] = tag;
        NavigationView.MenuItems.Add(item);
    }

    private NavigationViewItem AddGroupItem(string name, string? defaultPageTag = null)
    {
        var item = new NavigationViewItem
        {
            Content = name,
            SelectsOnInvoked = defaultPageTag != null // Allow selection if we have a default page
        };

        if (defaultPageTag != null)
        {
            _itemTags[item] = defaultPageTag;
        }

        _childrenByGroup[item] = new List<NavigationViewItem>();

        NavigationView?.MenuItems.Add(item);
        return item;
    }

    private void AddChildItem(NavigationViewItem parent, string name, string tag)
    {
        var item = new NavigationViewItem
        {
            Content = name
        };

        _itemTags[item] = tag;
        parent.MenuItems.Add(item);

        _parentByChild[item] = parent;
        if (!_childrenByGroup.TryGetValue(parent, out var children))
        {
            children = new List<NavigationViewItem>();
            _childrenByGroup[parent] = children;
        }

        children.Add(item);
    }

    private void OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem != null && _itemTags.TryGetValue(e.SelectedItem, out var tag))
        {
            NavigateToPage(tag);
        }
    }

    private void OnPageNavigationRequested(object? sender, NavigationRequestEventArgs e)
    {
        NavigateToPage(e.PageTag);

        // Also select the corresponding item in the navigation view
        SelectNavigationItem(e.PageTag);
    }

    private void SelectNavigationItem(string pageTag)
    {
        if (NavigationView == null) return;

        // Find the navigation item with the matching tag
        foreach (var kvp in _itemTags)
        {
            if (kvp.Value == pageTag)
            {
                NavigationView.SelectedItem = kvp.Key;
                break;
            }
        }
    }

    private void NavigateToPage(string pageTag)
    {
        if (NavigationView == null) return;

        _currentPageTag = pageTag;

        if (_sectionBadge != null)
        {
            _sectionBadge.Visibility = pageTag == "section" ? Visibility.Visible : Visibility.Collapsed;
        }

        UIElement? pageContent = null;

        // Special handling for pages that need navigation event wiring
        if (pageTag == "home")
        {
            var homePage = new HomePage();
            homePage.NavigationRequested += OnPageNavigationRequested;
            pageContent = homePage;
        }
        else if (pageTag == "basic")
        {
            var categoryPage = new BasicCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "text")
        {
            var categoryPage = new TextCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "layout")
        {
            var categoryPage = new LayoutCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "navigation")
        {
            var categoryPage = new NavigationCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "media")
        {
            var categoryPage = new MediaCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "collections")
        {
            var categoryPage = new CollectionsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "data")
        {
            var categoryPage = new DataCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "menustoolbars")
        {
            var categoryPage = new MenusToolbarsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "icons")
        {
            var categoryPage = new IconsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "datetime")
        {
            var categoryPage = new DateTimeCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "pickers")
        {
            var categoryPage = new PickersCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "overlays")
        {
            var categoryPage = new OverlaysCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "statusinfo")
        {
            var categoryPage = new StatusInfoCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "dialogscategory")
        {
            var categoryPage = new DialogsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "effects")
        {
            var categoryPage = new EffectsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "system")
        {
            var categoryPage = new SystemCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "dataviewers")
        {
            var categoryPage = new DataViewersCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "charts")
        {
            var categoryPage = new ChartsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (pageTag == "maps")
        {
            var categoryPage = new MapsCategoryPage();
            categoryPage.NavigationRequested += OnPageNavigationRequested;
            pageContent = categoryPage;
        }
        else if (Pages.TryGetValue(pageTag, out var factory))
        {
            try
            {
                pageContent = factory();
            }
            catch (Exception ex)
            {
                // Page creation failed - show error details
                System.Diagnostics.Debug.WriteLine($"[Gallery] Failed to create page '{pageTag}': {ex}");
                SetNavigationContent(CreateErrorPage(pageTag, ex));
                return;
            }
        }

        SetNavigationContent(pageContent ?? CreatePlaceholderPage(pageTag));
    }

    private void SetNavigationContent(UIElement content)
    {
        if (NavigationView == null) return;

        CleanupContentInputState();

        _contentHost.Content = content;
        if (!ReferenceEquals(NavigationView.Content, _contentHost))
        {
            NavigationView.SetContent(_contentHost);
        }

        RelaxConstrainedTextControlWidths(content);
    }

    private void CleanupContentInputState()
    {
        if (_contentHost.Content is not UIElement currentContent)
        {
            return;
        }

        if (Keyboard.FocusedElement is UIElement focused &&
            IsDescendantOf(focused, currentContent))
        {
            Keyboard.ClearFocus();
        }

        var captured = UIElement.MouseCapturedElement;
        if (captured != null && IsDescendantOf(captured, currentContent))
        {
            captured.ReleaseMouseCapture();
        }
    }

    private static bool IsDescendantOf(UIElement element, UIElement root)
    {
        Visual? current = element;
        while (current != null)
        {
            if (ReferenceEquals(current, root))
            {
                return true;
            }

            current = current.VisualParent;
        }

        return false;
    }

    private static ScrollViewer CreateContentHost()
    {
        return new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Padding = new Thickness(32, 24, 32, 32),
            Background = GalleryTheme.ShellBackgroundBrush,
            ClipToBounds = true
        };
    }

    private static void RelaxConstrainedTextControlWidths(Visual? root)
    {
        if (root == null)
        {
            return;
        }

        if (root is FrameworkElement element)
        {
            RelaxConstrainedTextControlWidth(element);
        }

        for (var i = 0; i < root.VisualChildrenCount; i++)
        {
            RelaxConstrainedTextControlWidths(root.GetVisualChild(i));
        }
    }

    private static void RelaxConstrainedTextControlWidth(FrameworkElement element)
    {
        var widthLimit = GetLocalWidthLimit(element);
        if (double.IsNaN(widthLimit) || widthLimit <= 0)
        {
            return;
        }

        var representativeText = GetRepresentativeText(element);
        if (string.IsNullOrWhiteSpace(representativeText))
        {
            return;
        }

        var requiredWidth = EstimateRequiredWidth(element, representativeText);
        if (requiredWidth <= 0 || widthLimit + 4 >= requiredWidth)
        {
            return;
        }

        ClearLocalWidthConstraint(element, FrameworkElement.WidthProperty);
        ClearLocalWidthConstraint(element, FrameworkElement.MaxWidthProperty);
    }

    private static double GetLocalWidthLimit(FrameworkElement element)
    {
        var hasLimit = false;
        var widthLimit = double.PositiveInfinity;

        if (TryGetLocalFiniteDouble(element, FrameworkElement.WidthProperty, out var width))
        {
            widthLimit = width;
            hasLimit = true;
        }

        if (TryGetLocalFiniteDouble(element, FrameworkElement.MaxWidthProperty, out var maxWidth))
        {
            widthLimit = hasLimit ? Math.Min(widthLimit, maxWidth) : maxWidth;
            hasLimit = true;
        }

        return hasLimit ? widthLimit : double.NaN;
    }

    private static bool TryGetLocalFiniteDouble(DependencyObject target, DependencyProperty property, out double value)
    {
        value = 0;
        var localValue = target.ReadLocalValue(property);
        if (ReferenceEquals(localValue, DependencyProperty.UnsetValue) ||
            localValue is not double numericValue ||
            double.IsNaN(numericValue) ||
            double.IsInfinity(numericValue) ||
            numericValue <= 0)
        {
            return false;
        }

        value = numericValue;
        return true;
    }

    private static void ClearLocalWidthConstraint(DependencyObject target, DependencyProperty property)
    {
        if (!ReferenceEquals(target.ReadLocalValue(property), DependencyProperty.UnsetValue))
        {
            target.ClearValue(property);
        }
    }

    private static string? GetRepresentativeText(FrameworkElement element)
    {
        return element switch
        {
            AppBarButton appBarButton => NormalizeText(appBarButton.Label),
            AppBarToggleButton appBarToggleButton => NormalizeText(appBarToggleButton.Label),
            ComboBox comboBox => GetComboBoxRepresentativeText(comboBox),
            SplitButton splitButton => ExtractDisplayText(splitButton.Content),
            ButtonBase buttonBase => ExtractDisplayText(buttonBase.Content),
            _ => null
        };
    }

    private static string? GetComboBoxRepresentativeText(ComboBox comboBox)
    {
        var fontSize = comboBox.FontSize > 0 ? comboBox.FontSize : 14;
        string? representativeText = null;

        representativeText = TakeWiderText(representativeText, NormalizeText(comboBox.PlaceholderText), fontSize);
        representativeText = TakeWiderText(representativeText, NormalizeText(comboBox.Text), fontSize);
        representativeText = TakeWiderText(representativeText, ExtractDisplayText(comboBox.SelectedItem), fontSize);

        foreach (var item in comboBox.Items)
        {
            representativeText = TakeWiderText(representativeText, ExtractDisplayText(item), fontSize);
        }

        return representativeText;
    }

    private static string? TakeWiderText(string? current, string? candidate, double fontSize)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return current;
        }

        if (string.IsNullOrWhiteSpace(current))
        {
            return candidate;
        }

        return EstimateTextWidth(candidate, fontSize) > EstimateTextWidth(current, fontSize)
            ? candidate
            : current;
    }

    private static string? ExtractDisplayText(object? content)
    {
        return content switch
        {
            null => null,
            string text => NormalizeText(text),
            TextBlock textBlock => NormalizeText(textBlock.Text),
            AccessText accessText => NormalizeText(accessText.Text),
            ComboBoxItem comboBoxItem => ExtractDisplayText(comboBoxItem.Content),
            UIElement => null,
            _ => NormalizeText(content.ToString())
        };
    }

    private static string? NormalizeText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text.Trim();
    }

    private static double EstimateRequiredWidth(FrameworkElement element, string text)
    {
        var fontSize = element switch
        {
            AppBarButton or AppBarToggleButton => 10,
            Control { FontSize: > 0 } sizedControl => sizedControl.FontSize,
            _ => 14
        };

        var paddingWidth = element is Control control
            ? control.Padding.Left + control.Padding.Right
            : 0;

        var extraWidth = 14.0;
        if (element is ComboBox)
        {
            extraWidth += 30;
        }
        else if (element is SplitButton)
        {
            extraWidth += 34;
        }
        else if (element is AppBarButton or AppBarToggleButton)
        {
            extraWidth += 12;
        }

        return EstimateTextWidth(text, fontSize) + paddingWidth + extraWidth;
    }

    private static double EstimateTextWidth(string text, double fontSize)
    {
        var widthUnits = 0.0;

        foreach (var ch in text)
        {
            if (char.IsWhiteSpace(ch))
            {
                widthUnits += 0.35;
            }
            else if (IsWideCharacter(ch))
            {
                widthUnits += 1.0;
            }
            else if (char.IsUpper(ch))
            {
                widthUnits += 0.72;
            }
            else if (char.IsDigit(ch))
            {
                widthUnits += 0.62;
            }
            else if (char.IsPunctuation(ch))
            {
                widthUnits += 0.45;
            }
            else
            {
                widthUnits += 0.58;
            }
        }

        return Math.Max(widthUnits, 1) * fontSize;
    }

    private static bool IsWideCharacter(char ch)
    {
        return (ch >= '\u1100' && ch <= '\u11FF') ||
               (ch >= '\u2E80' && ch <= '\uA4CF') ||
               (ch >= '\uAC00' && ch <= '\uD7A3') ||
               (ch >= '\uF900' && ch <= '\uFAFF') ||
               (ch >= '\uFE10' && ch <= '\uFE6F') ||
               (ch >= '\uFF01' && ch <= '\uFF60') ||
               (ch >= '\uFFE0' && ch <= '\uFFE6');
    }

    private UIElement CreateErrorPage(string pageTag, Exception ex)
    {
        var card = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(32),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 640
        };

        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var iconBorder = new Border
        {
            Width = 72,
            Height = 72,
            Background = new SolidColorBrush(Color.FromArgb(0x33, 0xEF, 0x44, 0x44)),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Margin = new Thickness(0, 0, 0, 20),
            HorizontalAlignment = HorizontalAlignment.Center,
            Child = new TextBlock
            {
                Text = "!",
                FontSize = 34,
                FontWeight = FontWeights.Bold,
                Foreground = GalleryTheme.ErrorBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        stack.Children.Add(iconBorder);

        stack.Children.Add(new TextBlock
        {
            Text = $"{pageTag} – Load failed",
            FontSize = GalleryTheme.FontSizeTitle,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 8)
        });

        stack.Children.Add(new TextBlock
        {
            Text = $"{ex.GetType().Name}: {ex.Message}",
            FontSize = GalleryTheme.FontSizeNormal,
            Foreground = GalleryTheme.TextTertiaryBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 560
        });

        card.Child = stack;
        return card;
    }

    private UIElement CreatePlaceholderPage(string pageTag)
    {
        var card = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(32),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 640
        };

        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var iconBorder = new Border
        {
            Width = 72,
            Height = 72,
            Background = GalleryTheme.AccentSoftBrush,
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Margin = new Thickness(0, 0, 0, 20),
            HorizontalAlignment = HorizontalAlignment.Center,
            Child = new TextBlock
            {
                Text = "\u2728",
                FontSize = 30,
                Foreground = GalleryTheme.AccentPrimaryBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        stack.Children.Add(iconBorder);

        stack.Children.Add(new TextBlock
        {
            Text = pageTag,
            FontSize = GalleryTheme.FontSizeTitle,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 8)
        });

        stack.Children.Add(new TextBlock
        {
            Text = "This page is coming soon.",
            FontSize = GalleryTheme.FontSizeNormal,
            Foreground = GalleryTheme.TextTertiaryBrush,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        card.Child = stack;
        return card;
    }
}
