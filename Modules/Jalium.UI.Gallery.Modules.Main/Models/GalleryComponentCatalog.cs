using Jalium.UI;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;

namespace Jalium.UI.Gallery.Modules.Main.Models;

public sealed class GalleryComponentDescriptor
{
    public GalleryComponentDescriptor(
        string pageTag,
        string title,
        string category,
        string componentTypeName,
        Type pageType,
        string? previewKind = null,
        string? exampleMarkup = null)
    {
        PageTag = pageTag;
        Title = title;
        Category = category;
        ComponentTypeName = componentTypeName;
        PageType = pageType;
        PreviewKind = previewKind ?? category.ToLowerInvariant();
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
    public Type PageType { get; }
    public string PreviewKind { get; }
    public string ExampleMarkup { get; }
    public string ExampleCSharp { get; }
    public string SearchText { get; }

    /// <summary>
    /// Localized category label. <see cref="Category"/> stays invariant because
    /// filtering, search, and the card artwork lookup all key off it.
    /// </summary>
    public string CategoryLabel => LocalizationManager.GetCategory(Category);

    /// <summary>
    /// Localized description. Pages with bespoke copy declare it in the string
    /// dictionaries as <c>Loc.Catalog.Desc.&lt;pageTag&gt;</c>; every other page falls
    /// back to the localized default sentence. Resolved on read so a language
    /// switch is picked up without rebuilding the catalog.
    /// </summary>
    public string Description =>
        LocalizationManager.GetDescription(
            LocalizationManager.FindDescriptionKey(PageTag),
            Title);

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
                "<StackPanel Orientation=\"Horizontal\">\n    <Button Content=\"Primary\" />\n    <Button Content=\"Secondary\" />\n    <Button Content=\"Ghost\" />\n</StackPanel>"),
            Item<TextBoxPage>(
                "textbox", "Inputs", "Controls", "inputs",
                "<StackPanel>\n    <TextBox Text=\"Placeholder\" />\n    <TextBox Text=\"Focused input\" />\n    <TextBox IsEnabled=\"False\" />\n</StackPanel>"),
            Item<BorderPage>(
                "border", "Cards", "Layout", "cards",
                "<Border Style=\"{StaticResource GalleryCard}\">\n    <StackPanel>\n        <TextBlock Text=\"Card title\" />\n        <TextBlock Text=\"Supporting description\" />\n    </StackPanel>\n</Border>"),
            Item<CheckBoxPage>(
                "checkbox", "Checkboxes", "Controls", "checkboxes",
                "<StackPanel>\n    <CheckBox Content=\"Checked\" IsChecked=\"True\" />\n    <CheckBox Content=\"Unchecked\" />\n    <CheckBox Content=\"Disabled\" IsEnabled=\"False\" />\n</StackPanel>"),
            Item<ToggleSwitchPage>(
                "toggleswitch", "Switches", "Controls", "switches",
                "<StackPanel>\n    <ToggleSwitch Header=\"Notifications\" IsOn=\"True\" />\n    <ToggleSwitch Header=\"Auto update\" />\n</StackPanel>"),
            Item<LabelPage>(
                "label", "Badges", "Text", "badges",
                "<Border Background=\"#DDF5F2\">\n    <TextBlock Text=\"Success\" />\n</Border>"),
            Item<ComboBoxPage>(
                "combobox", "Select", "Controls", "select",
                "<ComboBox PlaceholderText=\"Select an option\">\n    <ComboBoxItem Content=\"Option one\" />\n    <ComboBoxItem Content=\"Option two\" />\n</ComboBox>"),
            Item<ProgressBarPage>(
                "progressbar", "Progress", "Controls", "progress",
                "<ProgressBar Value=\"60\" />"),
            Item<InfoBarPage>(
                "infobar", "Alerts", "Controls", "alerts",
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
            Item<RazorVirtualizePage>("razorvirtualize", "Razor @virtualize", "Text"),
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

            // Styling — the CSS subsystem
            Item<CssStylingPage>(
                "css", "CSS Styling", "Styling", "styling",
                "<Border Css.Style=\"background-color: #0078D4; border-radius: 8px; padding: 12px 16px\" />"),
            Item<CssSelectorsPage>(
                "cssselectors", "Selectors & Cascade", "Styling", "styling",
                "<StackPanel Css.StyleSheet=\"Button:hover { background-color: #2563eb }\" />"),
            Item<CssLayoutPage>(
                "csslayout", "CSS Layout", "Styling", "styling",
                "<Border Css.Style=\"width: 50%; aspect-ratio: 16/9\" />"),
            Item<CssOutlinePage>(
                "cssoutline", "Outline", "Styling", "styling",
                "<Border Css.Style=\"outline: 2px solid #38bdf8; outline-offset: 3px\" />"),
            Item<CssMappingsPage>(
                "cssmappings", "Custom Properties", "Styling", "styling",
                "CssMappings.RegisterAlias(\"brand-corner\", \"border-radius\");"),
            Item<FlexPanelPage>(
                "flexpanel", "FlexPanel", "Layout", "layout",
                "<FlexPanel JustifyContent=\"SpaceBetween\" ColumnSpacing=\"10\" />"),

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
            Item<ShaderEffectsPage>(
                "shadereffects", "Shader Effects", "Media",
                exampleMarkup: "<Border>\n    <Border.Effect>\n        <effects:BlurEffect Radius=\"10\" />\n    </Border.Effect>\n</Border>"),
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

    public static IReadOnlyDictionary<string, GalleryComponentDescriptor> PagesByTag { get; } =
        Items.ToDictionary(
            item => item.PageTag,
            item => item,
            StringComparer.OrdinalIgnoreCase);

    private static GalleryComponentDescriptor Item<TPage>(
        string pageTag,
        string title,
        string category,
        string? previewKind = null,
        string? exampleMarkup = null)
        where TPage : UIElement
    {
        return new GalleryComponentDescriptor(
            pageTag,
            title,
            category,
            GalleryComponentDescriptor.GetComponentTypeName(typeof(TPage)),
            typeof(TPage),
            previewKind,
            exampleMarkup);
    }
}
