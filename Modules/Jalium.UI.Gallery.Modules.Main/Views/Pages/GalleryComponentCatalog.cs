using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class GalleryComponentDescriptor
{
    public GalleryComponentDescriptor(
        string pageTag,
        string title,
        string category,
        string componentTypeName,
        Func<UIElement> pageFactory,
        string? previewKind = null,
        string? description = null,
        string? exampleMarkup = null)
    {
        PageTag = pageTag;
        Title = title;
        Category = category;
        ComponentTypeName = componentTypeName;
        PageFactory = pageFactory;
        PreviewKind = previewKind ?? category.ToLowerInvariant();
        Description = description ?? $"Explore the {title} component, its states, behavior, and API.";
        ExampleMarkup = exampleMarkup ?? $"<{componentTypeName} />";
        ExampleCSharp =
            $"var component = new {componentTypeName}();\n" +
            "component.IsEnabled = true;\n" +
            "host.Children.Add(component);";
        SearchText = $"{title} {category} {pageTag}";
    }

    public string PageTag { get; }
    public string Title { get; }
    public string Category { get; }
    public string ComponentTypeName { get; }
    public Func<UIElement> PageFactory { get; }
    public string PreviewKind { get; }
    public string Description { get; }
    public string ExampleMarkup { get; }
    public string ExampleCSharp { get; }
    public string SearchText { get; }

    public UIElement CreateWorkbench() => new GalleryDemoWorkbench(this, PageFactory());

    public static string GetComponentTypeName(Type pageType)
    {
        const string pageSuffix = "Page";
        var name = pageType.Name;
        return name.EndsWith(pageSuffix, StringComparison.Ordinal)
            ? name[..^pageSuffix.Length]
            : name;
    }
}

internal static class GalleryComponentCatalog
{
    public static IReadOnlyList<GalleryComponentDescriptor> Items { get; } =
        new List<GalleryComponentDescriptor>
        {
            // Featured controls keep bespoke live previews at the top of All.
            Item<ButtonPage>(
                "button", "Buttons", "Controls", "buttons",
                "Primary, secondary, ghost, and disabled command states.",
                "<StackPanel Orientation=\"Horizontal\">\n    <Button Content=\"Primary\" />\n    <Button Content=\"Secondary\" />\n    <Button Content=\"Ghost\" />\n</StackPanel>"),
            Item<TextBoxPage>(
                "textbox", "Inputs", "Controls", "inputs",
                "Text entry states with clear focus and disabled feedback.",
                "<StackPanel>\n    <TextBox Text=\"Placeholder\" />\n    <TextBox Text=\"Focused input\" />\n    <TextBox IsEnabled=\"False\" />\n</StackPanel>"),
            Item<BorderPage>(
                "border", "Cards", "Layout", "cards",
                "A quiet surface for grouping content and supporting actions.",
                "<Border Style=\"{StaticResource GalleryCard}\">\n    <StackPanel>\n        <TextBlock Text=\"Card title\" />\n        <TextBlock Text=\"Supporting description\" />\n    </StackPanel>\n</Border>"),
            Item<CheckBoxPage>(
                "checkbox", "Checkboxes", "Controls", "checkboxes",
                "Checked, unchecked, and unavailable selection states.",
                "<StackPanel>\n    <CheckBox Content=\"Checked\" IsChecked=\"True\" />\n    <CheckBox Content=\"Unchecked\" />\n    <CheckBox Content=\"Disabled\" IsEnabled=\"False\" />\n</StackPanel>"),
            Item<ToggleSwitchPage>(
                "toggleswitch", "Switches", "Controls", "switches",
                "Immediate on/off settings with an explicit state label.",
                "<StackPanel>\n    <ToggleSwitch Header=\"Notifications\" IsOn=\"True\" />\n    <ToggleSwitch Header=\"Auto update\" />\n</StackPanel>"),
            Item<LabelPage>(
                "label", "Badges", "Text", "badges",
                "Compact semantic labels for status and metadata.",
                "<Border Background=\"#DDF5F2\">\n    <TextBlock Text=\"Success\" />\n</Border>"),
            Item<ComboBoxPage>(
                "combobox", "Select", "Controls", "select",
                "A concise single-choice control with a clear prompt.",
                "<ComboBox PlaceholderText=\"Select an option\">\n    <ComboBoxItem Content=\"Option one\" />\n    <ComboBoxItem Content=\"Option two\" />\n</ComboBox>"),
            Item<ProgressBarPage>(
                "progressbar", "Progress", "Controls", "progress",
                "Determinate progress with a readable percentage value.",
                "<ProgressBar Value=\"60\" />"),
            Item<InfoBarPage>(
                "infobar", "Alerts", "Controls", "alerts",
                "Contextual messages that remain visible near related work.",
                "<InfoBar Severity=\"Informational\"\n         Title=\"Update available\" />"),

            // Controls
            Item<RadioButtonPage>("radiobutton", "RadioButton", "Controls"),
            Item<SliderPage>("slider", "Slider", "Controls"),
            Item<RangeSliderPage>("rangeslider", "RangeSlider", "Controls"),
            Item<PasswordBoxPage>("passwordbox", "PasswordBox", "Controls"),
            Item<AutoCompleteBoxPage>("autocompletebox", "AutoCompleteBox", "Controls"),
            Item<NumberBoxPage>("numberbox", "NumberBox", "Controls"),
            Item<RepeatButtonPage>("repeatbutton", "RepeatButton", "Controls"),
            Item<HyperlinkButtonPage>("hyperlinkbutton", "HyperlinkButton", "Controls"),
            Item<SplitButtonPage>("splitbutton", "SplitButton", "Controls"),
            Item<DragDropPage>("dragdrop", "Drag & Drop", "Controls"),
            Item<CalendarPage>("calendar", "Calendar", "Controls"),
            Item<DatePickerPage>("datepicker", "DatePicker", "Controls"),
            Item<TimePickerPage>("timepicker", "TimePicker", "Controls"),
            Item<ColorPickerPage>("colorpicker", "ColorPicker", "Controls"),
            Item<StatusBarPage>("statusbar", "StatusBar", "Controls"),
            Item<ThumbPage>("thumb", "Thumb", "Controls"),

            // Text
            Item<TextBlockPage>("textblock", "TextBlock", "Text"),
            Item<MarkdownPage>("markdown", "Markdown", "Text"),
            Item<RichTextBoxPage>("richtextbox", "RichTextBox", "Text"),
            Item<EditControlPage>("editcontrol", "EditControl", "Text"),
            Item<TextEffectPresenterPage>("texteffectpresenter", "TextEffectPresenter", "Text"),
            Item<RazorDemoPage>("razor", "Razor Syntax", "Text"),
            Item<SectionDemoPage>("section", "Section", "Text"),

            // Layout
            Item<StackPanelPage>("stackpanel", "StackPanel", "Layout"),
            Item<GridPage>("grid", "Grid", "Layout"),
            Item<CanvasPage>("canvas", "Canvas", "Layout"),
            Item<DockPanelPage>("dockpanel", "DockPanel", "Layout"),
            Item<WrapPanelPage>("wrappanel", "WrapPanel", "Layout"),
            Item<ScrollViewerPage>("scrollviewer", "ScrollViewer", "Layout"),
            Item<UniformGridPage>("uniformgrid", "UniformGrid", "Layout"),
            Item<GroupBoxPage>("groupbox", "GroupBox", "Layout"),
            Item<SeparatorPage>("separator", "Separator", "Layout"),
            Item<ExpanderPage>("expander", "Expander", "Layout"),
            Item<ViewboxPage>("viewbox", "Viewbox", "Layout"),
            Item<SplitterPage>("splitter", "Splitter", "Layout"),
            Item<DockLayoutPage>("docklayout", "DockLayout", "Layout"),

            // Navigation
            Item<TabControlPage>("tabcontrol", "TabControl", "Navigation"),
            Item<FramePage>("frame", "Frame", "Navigation"),
            Item<MenuPage>("menu", "Menu", "Navigation"),
            Item<ContextMenuPage>("contextmenu", "ContextMenu", "Navigation"),
            Item<ToolBarPage>("toolbar", "ToolBar", "Navigation"),
            Item<AppBarButtonPage>("appbarbutton", "AppBarButton", "Navigation"),
            Item<AppBarSeparatorPage>("appbarseparator", "AppBarSeparator", "Navigation"),
            Item<AppBarToggleButtonPage>("appbartogglebutton", "AppBarToggleButton", "Navigation"),
            Item<CommandBarPage>("commandbar", "CommandBar", "Navigation"),
            Item<CommandBarFlyoutPage>("commandbarflyout", "CommandBarFlyout", "Navigation"),
            Item<MenuBarPage>("menubar", "MenuBar", "Navigation"),
            Item<MenuFlyoutPage>("menuflyout", "MenuFlyout", "Navigation"),
            Item<SwipeControlPage>("swipecontrol", "SwipeControl", "Navigation"),
            Item<StandardUICommandPage>("standarduicommand", "StandardUICommand", "Navigation"),
            Item<XamlUICommandPage>("xamluicommand", "XamlUICommand", "Navigation"),
            Item<RibbonPage>("ribbon", "Ribbon", "Navigation"),
            Item<NavigationDemoPage>("navigationdemo", "Navigation", "Navigation"),

            // Data
            Item<BindingPage>("binding", "Binding", "Data"),
            Item<ListBoxPage>("listbox", "ListBox", "Data"),
            Item<ListViewPage>("listview", "ListView", "Data"),
            Item<TreeViewPage>("treeview", "TreeView", "Data"),
            Item<TreeSelectorPage>("treeselector", "TreeSelector", "Data"),
            Item<DataGridPage>("datagrid", "DataGrid", "Data"),
            Item<TreeDataGridPage>("treedatagrid", "TreeDataGrid", "Data"),
            Item<DocumentViewerPage>("documentviewer", "DocumentViewer", "Data"),
            Item<DiffViewerPage>("diffviewer", "DiffViewer", "Data"),
            Item<HexEditorPage>("hexeditor", "HexEditor", "Data"),
            Item<JsonTreeViewerPage>("jsontreeviewer", "JsonTreeViewer", "Data"),
            Item<PropertyGridPage>("propertygrid", "PropertyGrid", "Data"),

            // Media and effects
            Item<ImagePage>("image", "Image", "Media"),
            Item<QRCodePage>("qrcode", "QRCode", "Media"),
            Item<MediaElementPage>("mediaelement", "MediaElement", "Media"),
            Item<ShapesPage>("shapes", "Shapes", "Media"),
            Item<InkCanvasPage>("inkcanvas", "InkCanvas", "Media"),
            Item<WebViewPage>("webview", "WebView", "Media"),
            Item<IconElementPage>("iconelement", "IconElement", "Media"),
            Item<BackdropEffectsPage>("backdropeffects", "Backdrop Effects", "Media"),
            Item<ShaderEffectsPage>("shadereffects", "Shader Effects", "Media"),
            Item<ElementEffectsPage>("elementeffects", "Element Effects", "Media"),
            Item<LiquidGlassPage>("liquidglass", "Liquid Glass", "Media"),
            Item<TransitionDemoPage>("transitions", "Content Transitions", "Media"),

            // Charts and maps
            Item<LineChartPage>("linechart", "LineChart", "Visuals"),
            Item<BarChartPage>("barchart", "BarChart", "Visuals"),
            Item<PieChartPage>("piechart", "PieChart", "Visuals"),
            Item<ScatterPlotPage>("scatterplot", "ScatterPlot", "Visuals"),
            Item<HeatmapPage>("heatmap", "Heatmap", "Visuals"),
            Item<SparklinePage>("sparkline", "Sparkline", "Visuals"),
            Item<GaugeChartPage>("gaugechart", "GaugeChart", "Visuals"),
            Item<TreeMapPage>("treemap", "TreeMap", "Visuals"),
            Item<CandlestickChartPage>("candlestickchart", "CandlestickChart", "Visuals"),
            Item<NetworkGraphPage>("networkgraph", "NetworkGraph", "Visuals"),
            Item<GanttChartPage>("ganttchart", "GanttChart", "Visuals"),
            Item<SankeyDiagramPage>("sankeydiagram", "SankeyDiagram", "Visuals"),
            Item<FlowchartPage>("flowchart", "Flowchart", "Visuals"),
            Item<MapViewPage>("mapview", "MapView", "Visuals"),
            Item<MiniMapPage>("minimap", "MiniMap", "Visuals"),
            Item<GeographicHeatmapPage>("geographicheatmap", "GeographicHeatmap", "Visuals"),

            // System and overlays
            Item<DialogsPage>("dialogs", "File Dialogs", "System"),
            Item<ContentDialogPage>("contentdialog", "ContentDialog", "System"),
            Item<ToolTipPage>("tooltip", "ToolTip", "System"),
            Item<PopupPage>("popup", "Popup", "System"),
            Item<ToastNotificationPage>("toastnotification", "ToastNotification", "System"),
            Item<SystemNotificationPage>("systemnotification", "SystemNotification", "System"),
            Item<NotifyIconPage>("notifyicon", "NotifyIcon", "System"),
            Item<PrintingPage>("printing", "Printing", "System"),
            Item<ShellIntegrationPage>("shellintegration", "Shell Integration", "System"),
            Item<TitleBarPage>("titlebar", "TitleBar", "System"),
            Item<TerminalPage>("terminal", "Terminal", "System"),
            Item<ThemeColorsPage>("themecolors", "Theme Colors", "System")
        };

    public static IReadOnlyDictionary<string, Func<UIElement>> PageFactories { get; } =
        Items.ToDictionary(
            item => item.PageTag,
            item => new Func<UIElement>(item.CreateWorkbench),
            StringComparer.OrdinalIgnoreCase);

    private static GalleryComponentDescriptor Item<TPage>(
        string pageTag,
        string title,
        string category,
        string? previewKind = null,
        string? description = null,
        string? exampleMarkup = null)
        where TPage : UIElement, new()
    {
        return new GalleryComponentDescriptor(
            pageTag,
            title,
            category,
            GalleryComponentDescriptor.GetComponentTypeName(typeof(TPage)),
            static () => new TPage(),
            previewKind,
            description,
            exampleMarkup);
    }
}
