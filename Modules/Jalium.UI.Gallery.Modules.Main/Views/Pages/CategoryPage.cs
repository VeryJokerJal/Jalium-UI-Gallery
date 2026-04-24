using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Represents a control item to display in a category page.
/// </summary>
public record ControlInfo(string Name, string Description, string PageTag);

/// <summary>
/// Base class for category overview pages that display control cards.
/// Uses the Aureate-inspired gallery theme (dark purple + warm amber accent).
/// </summary>
public abstract class CategoryPage : Page
{
    /// <summary>
    /// Occurs when a control card is clicked and navigation is requested.
    /// </summary>
    public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

    protected abstract string CategoryTitle { get; }
    protected abstract string CategoryDescription { get; }
    protected abstract Color AccentColor { get; }
    protected abstract IEnumerable<ControlInfo> Controls { get; }

    /// <summary>
    /// Optional category glyph shown in the hero badge. Subclasses may override.
    /// </summary>
    protected virtual string CategoryGlyph => "\u25C7";

    protected CategoryPage()
    {
        BuildContent();
    }

    private void BuildContent()
    {
        var mainStack = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        mainStack.Children.Add(BuildHero());

        mainStack.Children.Add(new TextBlock
        {
            Text = "Controls",
            FontSize = GalleryTheme.FontSizeTitle,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(4, 4, 0, 4)
        });

        mainStack.Children.Add(new TextBlock
        {
            Text = "Select any card to jump into the live demo.",
            FontSize = 13,
            Foreground = GalleryTheme.TextTertiaryBrush,
            Margin = new Thickness(4, 0, 0, 16)
        });

        var grid = new UniformGrid
        {
            Columns = 3
        };

        foreach (var control in Controls)
        {
            grid.Children.Add(CreateControlCard(control));
        }

        mainStack.Children.Add(grid);

        Content = mainStack;
    }

    private Border BuildHero()
    {
        var hero = new Border
        {
            Background = GalleryTheme.HeroGradientBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusXLarge),
            Padding = new Thickness(32, 28, 32, 28),
            Margin = new Thickness(0, 0, 0, 28)
        };

        var row = new StackPanel { Orientation = Orientation.Horizontal };

        var badge = new Border
        {
            Width = 64,
            Height = 64,
            Background = new SolidColorBrush(Color.FromArgb(0x33, AccentColor.R, AccentColor.G, AccentColor.B)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x55, AccentColor.R, AccentColor.G, AccentColor.B)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Margin = new Thickness(0, 0, 20, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                Text = CategoryGlyph,
                FontSize = 28,
                Foreground = new SolidColorBrush(AccentColor),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        row.Children.Add(badge);

        var texts = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };

        texts.Children.Add(new TextBlock
        {
            Text = "CATEGORY",
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 6)
        });

        texts.Children.Add(new TextBlock
        {
            Text = CategoryTitle,
            FontSize = GalleryTheme.FontSizeHeader,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4)
        });

        texts.Children.Add(new TextBlock
        {
            Text = CategoryDescription,
            FontSize = 14,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 680
        });

        row.Children.Add(texts);
        hero.Child = row;
        return hero;
    }

    private Border CreateControlCard(ControlInfo control)
    {
        var card = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(18, 16, 18, 16),
            Margin = new Thickness(6)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Title row with accent dot
        var titleRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 6)
        };
        titleRow.Children.Add(new Border
        {
            Width = 10,
            Height = 10,
            Background = new SolidColorBrush(AccentColor),
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(0, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Center
        });
        titleRow.Children.Add(new TextBlock
        {
            Text = control.Name,
            FontSize = 15,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(titleRow);

        stack.Children.Add(new TextBlock
        {
            Text = control.Description,
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 10)
        });

        stack.Children.Add(new TextBlock
        {
            Text = "Open  \u2192",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.AccentPrimaryBrush
        });

        card.Child = stack;

        card.MouseEnter += (_, _) =>
        {
            card.Background = GalleryTheme.BackgroundHoverBrush;
            card.BorderBrush = new SolidColorBrush(Color.FromArgb(0x88, AccentColor.R, AccentColor.G, AccentColor.B));
        };
        card.MouseLeave += (_, _) =>
        {
            card.Background = GalleryTheme.BackgroundCardBrush;
            card.BorderBrush = GalleryTheme.BorderSubtleBrush;
        };

        card.MouseDown += (_, e) =>
        {
            if (e is MouseButtonEventArgs mouseArgs && mouseArgs.ChangedButton == MouseButton.Left)
            {
                NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(control.PageTag));
            }
        };

        return card;
    }
}

/// <summary>
/// Basic Input controls category page.
/// </summary>
public class BasicCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Basic Input";
    protected override string CategoryDescription => "Primary interactive controls your users touch first — buttons, text inputs, toggles, and selectors.";
    protected override Color AccentColor => GalleryTheme.AccentPrimary;
    protected override string CategoryGlyph => "\u25A3";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Button", "Click-driven commands with focus + pressed states.", "button"),
        new ControlInfo("CheckBox", "Boolean and three-state selection.", "checkbox"),
        new ControlInfo("RadioButton", "Exclusive selection within a group.", "radiobutton"),
        new ControlInfo("Slider", "Pick a value from a continuous range.", "slider"),
        new ControlInfo("ProgressBar", "Determinate and indeterminate progress.", "progressbar"),
        new ControlInfo("TextBox", "Single and multi-line text input.", "textbox"),
        new ControlInfo("PasswordBox", "Masked input with reveal support.", "passwordbox"),
        new ControlInfo("ComboBox", "Drop-down selection with filtering.", "combobox"),
        new ControlInfo("AutoCompleteBox", "Suggestion-driven text input.", "autocompletebox"),
        new ControlInfo("ToggleSwitch", "Fluent-style on/off switch.", "toggleswitch"),
        new ControlInfo("NumberBox", "Numeric input with spin buttons and validation.", "numberbox"),
        new ControlInfo("RepeatButton", "Fires repeatedly while held down.", "repeatbutton"),
        new ControlInfo("HyperlinkButton", "Button styled as a hyperlink.", "hyperlinkbutton"),
        new ControlInfo("SplitButton", "Primary action plus a secondary flyout.", "splitbutton"),
        new ControlInfo("Drag & Drop", "DragEnter, DragOver, DragLeave, and Drop events.", "dragdrop")
    };
}

/// <summary>
/// Text controls category page.
/// </summary>
public class TextCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Text";
    protected override string CategoryDescription => "Everything for presenting and authoring text — from display blocks to rich editors.";
    protected override Color AccentColor => GalleryTheme.Info;
    protected override string CategoryGlyph => "\u00B6";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("TextBlock", "Formatted, inline-rich text display.", "textblock"),
        new ControlInfo("Markdown", "Render markdown documents natively.", "markdown"),
        new ControlInfo("Label", "Text label with access-key support.", "label"),
        new ControlInfo("RichTextBox", "Full rich-text editing experience.", "richtextbox"),
        new ControlInfo("EditControl", "Syntax-highlighted code editor.", "editcontrol")
    };
}

/// <summary>
/// Layout controls category page.
/// </summary>
public class LayoutCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Layout";
    protected override string CategoryDescription => "Panels and containers that arrange child content responsively and declaratively.";
    protected override Color AccentColor => GalleryTheme.HaloPurple;
    protected override string CategoryGlyph => "\u25A6";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("StackPanel", "Linear stacking in a single direction.", "stackpanel"),
        new ControlInfo("Grid", "Rows and columns with flexible sizing.", "grid"),
        new ControlInfo("Canvas", "Absolute positioning of children.", "canvas"),
        new ControlInfo("Border", "Background, border, and corner radius.", "border"),
        new ControlInfo("DockPanel", "Dock children to the edges.", "dockpanel"),
        new ControlInfo("WrapPanel", "Wrap children onto multiple lines.", "wrappanel"),
        new ControlInfo("UniformGrid", "Evenly divided rows and columns.", "uniformgrid"),
        new ControlInfo("ScrollViewer", "Scrollable content with pan + wheel.", "scrollviewer"),
        new ControlInfo("Expander", "Collapsible content region.", "expander"),
        new ControlInfo("GroupBox", "Titled content grouping.", "groupbox"),
        new ControlInfo("Separator", "Visual divider between elements.", "separator"),
        new ControlInfo("Viewbox", "Scale content to fit its container.", "viewbox"),
        new ControlInfo("Splitter", "Resizable split between regions.", "splitter"),
        new ControlInfo("DockLayout", "Advanced docking layout manager.", "docklayout")
    };
}

/// <summary>
/// Navigation controls category page.
/// </summary>
public class NavigationCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Navigation";
    protected override string CategoryDescription => "Tabs, menus, and toolbars for moving between views and invoking commands.";
    protected override Color AccentColor => GalleryTheme.Warning;
    protected override string CategoryGlyph => "\u2630";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("TabControl", "Tabbed content switcher.", "tabcontrol"),
        new ControlInfo("Frame", "Host navigable page content.", "frame"),
        new ControlInfo("Menu", "Standard menu with submenus.", "menu"),
        new ControlInfo("ContextMenu", "Right-click context menu.", "contextmenu"),
        new ControlInfo("ToolBar", "Command-button toolbar.", "toolbar")
    };
}

/// <summary>
/// Media controls category page.
/// </summary>
public class MediaCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Media";
    protected override string CategoryDescription => "Display images, vectors, video, and web content with GPU acceleration.";
    protected override Color AccentColor => GalleryTheme.Success;
    protected override string CategoryGlyph => "\u25B6";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Image", "Raster image display with stretch modes.", "image"),
        new ControlInfo("QRCode", "Generate QR codes from text payloads.", "qrcode"),
        new ControlInfo("MediaElement", "Audio and video playback.", "mediaelement"),
        new ControlInfo("Shapes", "Ellipse, rectangle, path, and more.", "shapes"),
        new ControlInfo("InkCanvas", "Freehand ink drawing surface.", "inkcanvas"),
        new ControlInfo("WebView", "Embedded browser control.", "webview")
    };
}

/// <summary>
/// Collections controls category page.
/// </summary>
public class CollectionsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Collections";
    protected override string CategoryDescription => "List, tree, and grid controls for rendering collections of data.";
    protected override Color AccentColor => GalleryTheme.Error;
    protected override string CategoryGlyph => "\u25A4";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("ListBox", "Single-column selectable list.", "listbox"),
        new ControlInfo("ListView", "Flexible list with templates.", "listview"),
        new ControlInfo("TreeView", "Hierarchical data display.", "treeview"),
        new ControlInfo("DataGrid", "Editable tabular data view.", "datagrid"),
        new ControlInfo("TreeDataGrid", "Hierarchical grid with expand/collapse.", "treedatagrid"),
        new ControlInfo("Calendar", "Month/date selection control.", "calendar")
    };
}

/// <summary>
/// Data binding category page.
/// </summary>
public class DataCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Data";
    protected override string CategoryDescription => "Binding expressions, converters, Razor syntax, and template sections.";
    protected override Color AccentColor => GalleryTheme.AccentSecondary;
    protected override string CategoryGlyph => "\u2261";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Binding", "Data binding expressions and converters.", "binding"),
        new ControlInfo("Razor Syntax", "Razor template syntax for JALXAML.", "razor"),
        new ControlInfo("Section", "Named sections for content composition.", "section")
    };
}

/// <summary>
/// Menus &amp; Toolbars category page.
/// </summary>
public class MenusToolbarsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Menus & Toolbars";
    protected override string CategoryDescription => "AppBar, CommandBar, and Ribbon surfaces for exposing commands.";
    protected override Color AccentColor => GalleryTheme.Warning;
    protected override string CategoryGlyph => "\u2637";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("AppBarButton", "App-bar command button.", "appbarbutton"),
        new ControlInfo("AppBarSeparator", "App-bar item separator.", "appbarseparator"),
        new ControlInfo("AppBarToggleButton", "App-bar toggleable command button.", "appbartogglebutton"),
        new ControlInfo("CommandBar", "Bar hosting app commands.", "commandbar"),
        new ControlInfo("CommandBarFlyout", "Flyout-hosted command bar.", "commandbarflyout"),
        new ControlInfo("MenuBar", "Top-level menu bar with drop-downs.", "menubar"),
        new ControlInfo("MenuFlyout", "Flyout-hosted menu for context actions.", "menuflyout"),
        new ControlInfo("Ribbon", "Office-style ribbon with tabs + galleries.", "ribbon"),
        new ControlInfo("SwipeControl", "Swipe gesture command surface.", "swipecontrol"),
        new ControlInfo("StandardUICommand", "Pre-defined standard UI commands.", "standarduicommand"),
        new ControlInfo("XamlUICommand", "Custom XAML-authored UI commands.", "xamluicommand")
    };
}

/// <summary>
/// Icons category page.
/// </summary>
public class IconsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Icons";
    protected override string CategoryDescription => "Icon element and glyph rendering helpers.";
    protected override Color AccentColor => GalleryTheme.TextTertiary;
    protected override string CategoryGlyph => "\u2605";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("IconElement", "Unified icon/glyph element.", "iconelement")
    };
}

/// <summary>
/// Date &amp; Time category page.
/// </summary>
public class DateTimeCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Date & Time";
    protected override string CategoryDescription => "Calendar, date, and time pickers for precise temporal input.";
    protected override Color AccentColor => GalleryTheme.Info;
    protected override string CategoryGlyph => "\u25F4";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("DatePicker", "Pick a calendar date.", "datepicker"),
        new ControlInfo("TimePicker", "Pick a time of day.", "timepicker"),
        new ControlInfo("Calendar", "Month/year calendar.", "calendar")
    };
}

/// <summary>
/// Pickers category page.
/// </summary>
public class PickersCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Pickers";
    protected override string CategoryDescription => "Specialized value pickers for colors and more.";
    protected override Color AccentColor => GalleryTheme.Error;
    protected override string CategoryGlyph => "\u25F0";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("ColorPicker", "Pick a color with sliders and palette.", "colorpicker")
    };
}

/// <summary>
/// Overlays category page.
/// </summary>
public class OverlaysCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Overlays";
    protected override string CategoryDescription => "Popups, tooltips, and notifications layered on top of content.";
    protected override Color AccentColor => GalleryTheme.Success;
    protected override string CategoryGlyph => "\u25A0";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Popup", "Floating popup overlay.", "popup"),
        new ControlInfo("ToolTip", "Hover/focus-triggered tooltip.", "tooltip"),
        new ControlInfo("ToastNotification", "In-app toast notification.", "toastnotification"),
        new ControlInfo("SystemNotification", "OS-level notifications.", "systemnotification"),
        new ControlInfo("NotifyIcon", "System tray icon with balloon tips.", "notifyicon")
    };
}

/// <summary>
/// Status &amp; Info category page.
/// </summary>
public class StatusInfoCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Status & Info";
    protected override string CategoryDescription => "Status bars, info bars, and feedback surfaces.";
    protected override Color AccentColor => GalleryTheme.Success;
    protected override string CategoryGlyph => "\u2139";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("StatusBar", "App-level status information bar.", "statusbar"),
        new ControlInfo("InfoBar", "In-page contextual notifications.", "infobar"),
        new ControlInfo("Thumb", "Low-level draggable thumb primitive.", "thumb")
    };
}

/// <summary>
/// Dialogs category page.
/// </summary>
public class DialogsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Dialogs";
    protected override string CategoryDescription => "Modal and non-modal dialog surfaces for confirmations and workflows.";
    protected override Color AccentColor => GalleryTheme.TextTertiary;
    protected override string CategoryGlyph => "\u25A2";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("File Dialogs", "Open, save, and folder dialogs.", "dialogs"),
        new ControlInfo("ContentDialog", "Modal content dialogs with inline hosting.", "contentdialog")
    };
}

/// <summary>
/// Effects category page.
/// </summary>
public class EffectsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Effects";
    protected override string CategoryDescription => "Backdrop materials, shader effects, and transitions for polished visuals.";
    protected override Color AccentColor => GalleryTheme.HaloPurple;
    protected override string CategoryGlyph => "\u2728";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Backdrop Effects", "Acrylic and Mica backdrop effects.", "backdropeffects"),
        new ControlInfo("Liquid Glass", "Glassy liquid material effects.", "liquidglass"),
        new ControlInfo("Shader Effects", "Custom shader-based visual effects.", "shadereffects"),
        new ControlInfo("Element Effects", "OuterGlow, InnerShadow, Emboss, ColorMatrix.", "elementeffects"),
        new ControlInfo("Content Transitions", "Animated content transition effects.", "transitions")
    };
}

/// <summary>
/// System features category page.
/// </summary>
public class SystemCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "System";
    protected override string CategoryDescription => "System-level integration — printing, shell, title bar, and platform navigation.";
    protected override Color AccentColor => GalleryTheme.Warning;
    protected override string CategoryGlyph => "\u2699";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Navigation", "Page navigation and journal support.", "navigationdemo"),
        new ControlInfo("Printing", "Document printing support.", "printing"),
        new ControlInfo("Shell Integration", "Taskbar, jump lists, and shell features.", "shellintegration"),
        new ControlInfo("TitleBar", "Custom window caption bar.", "titlebar")
    };
}

/// <summary>
/// Data Viewers category page.
/// </summary>
public class DataViewersCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Data Viewers";
    protected override string CategoryDescription => "Specialized viewers for documents, diffs, hex, JSON, and properties.";
    protected override Color AccentColor => GalleryTheme.Info;
    protected override string CategoryGlyph => "\u25A7";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("DocumentViewer", "Paginated document viewer with zoom.", "documentviewer"),
        new ControlInfo("DiffViewer", "Side-by-side or unified text diffs.", "diffviewer"),
        new ControlInfo("HexEditor", "Binary hex editor with ASCII view.", "hexeditor"),
        new ControlInfo("JsonTreeViewer", "JSON document tree viewer.", "jsontreeviewer"),
        new ControlInfo("PropertyGrid", "Object property editor with editors.", "propertygrid")
    };
}

/// <summary>
/// Charts category page.
/// </summary>
public class ChartsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Charts";
    protected override string CategoryDescription => "Data visualization controls — series, gauges, trees, and flow diagrams.";
    protected override Color AccentColor => GalleryTheme.AccentPrimary;
    protected override string CategoryGlyph => "\u25A9";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("LineChart", "Line and area charts with multiple series.", "linechart"),
        new ControlInfo("BarChart", "Grouped, stacked, and horizontal bars.", "barchart"),
        new ControlInfo("PieChart", "Pie and donut charts with labels.", "piechart"),
        new ControlInfo("ScatterPlot", "Scatter plots with trend lines.", "scatterplot"),
        new ControlInfo("Heatmap", "2D color grid for matrix data.", "heatmap"),
        new ControlInfo("Sparkline", "Inline mini chart for dashboards.", "sparkline"),
        new ControlInfo("GaugeChart", "Circular gauge with ranges and needle.", "gaugechart"),
        new ControlInfo("TreeMap", "Space-filling hierarchical rectangles.", "treemap"),
        new ControlInfo("CandlestickChart", "OHLC financial candlestick charts.", "candlestickchart"),
        new ControlInfo("NetworkGraph", "Force-directed node-link diagrams.", "networkgraph"),
        new ControlInfo("GanttChart", "Timeline task bars with dependencies.", "ganttchart"),
        new ControlInfo("SankeyDiagram", "Flow diagrams with weighted links.", "sankeydiagram")
    };
}

/// <summary>
/// Maps category page.
/// </summary>
public class MapsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Maps";
    protected override string CategoryDescription => "Geographic and spatial visualization controls.";
    protected override Color AccentColor => GalleryTheme.Success;
    protected override string CategoryGlyph => "\u25D4";
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("MapView", "Tile-based map viewer with markers.", "mapview"),
        new ControlInfo("MiniMap", "Miniature overview of a larger area.", "minimap"),
        new ControlInfo("GeographicHeatmap", "Geographic heatmap with gradient coloring.", "geographicheatmap")
    };
}
