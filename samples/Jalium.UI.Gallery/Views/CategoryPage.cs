using Jalium.UI.Controls;
using Jalium.UI.Gallery.Theme;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Represents a control item to display in a category page.
/// </summary>
public record ControlInfo(string Name, string Description, string PageTag);

/// <summary>
/// Base class for category overview pages that display control cards.
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

        // Category Title
        mainStack.Children.Add(new TextBlock
        {
            Text = CategoryTitle,
            FontSize = 32,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 8)
        });

        // Category Description
        mainStack.Children.Add(new TextBlock
        {
            Text = CategoryDescription,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
            Margin = new Thickness(0, 0, 0, 32)
        });

        // Controls Section
        mainStack.Children.Add(new TextBlock
        {
            Text = "Controls",
            FontSize = 20,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 16)
        });

        // Create rows of control cards (4 per row)
        var controls = Controls.ToList();
        var currentRow = new StackPanel { Orientation = Orientation.Horizontal };
        var cardCount = 0;

        foreach (var control in controls)
        {
            var isLastInRow = (cardCount + 1) % 4 == 0 || cardCount == controls.Count - 1;
            currentRow.Children.Add(CreateControlCard(control, !isLastInRow));
            cardCount++;

            if (cardCount % 4 == 0)
            {
                mainStack.Children.Add(currentRow);
                currentRow = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 12, 0, 0)
                };
            }
        }

        // Add remaining cards
        if (currentRow.Children.Count > 0)
        {
            mainStack.Children.Add(currentRow);
        }

        Content = mainStack;
    }

    private Border CreateControlCard(ControlInfo control, bool hasMargin)
    {
        var card = new Border
        {
            Width = 220,
            Height = 100,
            Background = new SolidColorBrush(Color.FromRgb(37, 37, 37)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(53, 53, 53)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16),
            Margin = hasMargin ? new Thickness(0, 0, 12, 0) : new Thickness(0)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Accent bar
        stack.Children.Add(new Border
        {
            Width = 24,
            Height = 3,
            Background = new SolidColorBrush(AccentColor),
            CornerRadius = new CornerRadius(1.5),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 10)
        });

        // Control name
        stack.Children.Add(new TextBlock
        {
            Text = control.Name,
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 6)
        });

        // Description
        stack.Children.Add(new TextBlock
        {
            Text = control.Description,
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            TextWrapping = TextWrapping.Wrap
        });

        card.Child = stack;

        // Hover effects
        card.MouseEnter += (s, e) =>
        {
            card.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            card.BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        };

        card.MouseLeave += (s, e) =>
        {
            card.Background = new SolidColorBrush(Color.FromRgb(37, 37, 37));
            card.BorderBrush = new SolidColorBrush(Color.FromRgb(53, 53, 53));
        };

        // Click handler
        card.MouseDown += (s, e) =>
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
    protected override string CategoryDescription => "Basic input controls for user interaction.";
    protected override Color AccentColor => Color.FromRgb(0, 120, 212);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Button", "A control that responds to user clicks", "button"),
        new ControlInfo("CheckBox", "A control for boolean selection", "checkbox"),
        new ControlInfo("RadioButton", "A control for single selection from a group", "radiobutton"),
        new ControlInfo("Slider", "A control for selecting a value from a range", "slider"),
        new ControlInfo("ProgressBar", "A control for displaying progress", "progressbar"),
        new ControlInfo("TextBox", "A control for text input", "textbox"),
        new ControlInfo("PasswordBox", "A control for secure text input", "passwordbox"),
        new ControlInfo("ComboBox", "A control for dropdown selection", "combobox"),
        new ControlInfo("AutoCompleteBox", "A text box with auto-complete suggestions", "autocompletebox"),
        new ControlInfo("ToggleSwitch", "A switch that toggles between two states", "toggleswitch"),
        new ControlInfo("NumberBox", "A control for numeric input with validation", "numberbox"),
        new ControlInfo("RepeatButton", "A button that fires repeatedly while pressed", "repeatbutton"),
        new ControlInfo("HyperlinkButton", "A button styled as a hyperlink", "hyperlinkbutton"),
        new ControlInfo("SplitButton", "A button with a secondary flyout action", "splitbutton"),
        new ControlInfo("Drag & Drop", "DragEnter, DragOver, DragLeave, and Drop events", "dragdrop")
    };
}

/// <summary>
/// Text controls category page.
/// </summary>
public class TextCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Text";
    protected override string CategoryDescription => "Controls for displaying and formatting text.";
    protected override Color AccentColor => Color.FromRgb(16, 124, 16);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("TextBlock", "A control for displaying text", "textblock"),
        new ControlInfo("Markdown", "Render markdown documents with rich formatting", "markdown"),
        new ControlInfo("Label", "A text label with access key support", "label"),
        new ControlInfo("RichTextBox", "A rich text editing control", "richtextbox"),
        new ControlInfo("EditControl", "A code editor control with syntax highlighting", "editcontrol")
    };
}

/// <summary>
/// Layout controls category page.
/// </summary>
public class LayoutCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Layout";
    protected override string CategoryDescription => "Controls for arranging and positioning content.";
    protected override Color AccentColor => Color.FromRgb(136, 23, 152);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("StackPanel", "Arranges children in a single line", "stackpanel"),
        new ControlInfo("Grid", "Arranges children in rows and columns", "grid"),
        new ControlInfo("Canvas", "Positions children at absolute coordinates", "canvas"),
        new ControlInfo("Border", "Draws a border around content", "border"),
        new ControlInfo("DockPanel", "Docks children to edges", "dockpanel"),
        new ControlInfo("WrapPanel", "Wraps children to multiple lines", "wrappanel"),
        new ControlInfo("ScrollViewer", "Enables scrolling of content", "scrollviewer"),
        new ControlInfo("Expander", "A collapsible content container", "expander"),
        new ControlInfo("GroupBox", "Groups related controls with a header", "groupbox"),
        new ControlInfo("Separator", "A visual separator between elements", "separator"),
        new ControlInfo("Viewbox", "Scales content to fit available space", "viewbox"),
        new ControlInfo("Splitter", "A resizable split container", "splitter"),
        new ControlInfo("DockLayout", "An advanced docking layout manager", "docklayout")
    };
}

/// <summary>
/// Navigation controls category page.
/// </summary>
public class NavigationCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Navigation";
    protected override string CategoryDescription => "Controls for navigating between content.";
    protected override Color AccentColor => Color.FromRgb(202, 80, 16);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("TabControl", "A control for tabbed content", "tabcontrol"),
        new ControlInfo("Frame", "A control for hosting navigable content", "frame"),
        new ControlInfo("Menu", "A standard menu control", "menu"),
        new ControlInfo("ContextMenu", "A right-click context menu", "contextmenu"),
        new ControlInfo("ToolBar", "A toolbar for hosting command buttons", "toolbar")
    };
}

/// <summary>
/// Media controls category page.
/// </summary>
public class MediaCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Media";
    protected override string CategoryDescription => "Controls for displaying media content.";
    protected override Color AccentColor => Color.FromRgb(0, 99, 177);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Image", "A control for displaying images", "image"),
        new ControlInfo("QRCode", "Generate QR codes from text payloads", "qrcode"),
        new ControlInfo("MediaElement", "A control for playing audio and video", "mediaelement"),
        new ControlInfo("Shapes", "Drawing shapes like rectangles and ellipses", "shapes"),
        new ControlInfo("InkCanvas", "A drawing surface for freehand ink input", "inkcanvas"),
        new ControlInfo("WebView", "An embedded web browser control", "webview")
    };
}

/// <summary>
/// Collections controls category page.
/// </summary>
public class CollectionsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Collections";
    protected override string CategoryDescription => "Controls for displaying collections of items.";
    protected override Color AccentColor => Color.FromRgb(177, 70, 194);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("ListBox", "A control for displaying a list of items", "listbox"),
        new ControlInfo("ListView", "A control for displaying items in a list view", "listview"),
        new ControlInfo("TreeView", "A control for displaying hierarchical data", "treeview"),
        new ControlInfo("DataGrid", "A tabular data display control", "datagrid"),
        new ControlInfo("Calendar", "A control for date selection", "calendar")
    };
}

/// <summary>
/// Data binding category page.
/// </summary>
public class DataCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Data";
    protected override string CategoryDescription => "Data binding and data management features.";
    protected override Color AccentColor => Color.FromRgb(0, 153, 188);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Binding", "Data binding expressions and converters", "binding")
    };
}

/// <summary>
/// Menus &amp; Toolbars category page.
/// </summary>
public class MenusToolbarsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Menus & Toolbars";
    protected override string CategoryDescription => "Controls for creating menus, toolbars, and command surfaces.";
    protected override Color AccentColor => Color.FromRgb(218, 59, 1);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("AppBarButton", "A button for app bar commands", "appbarbutton"),
        new ControlInfo("AppBarSeparator", "A separator for app bar items", "appbarseparator"),
        new ControlInfo("AppBarToggleButton", "A toggle button for app bar commands", "appbartogglebutton"),
        new ControlInfo("CommandBar", "A bar for hosting app commands", "commandbar"),
        new ControlInfo("CommandBarFlyout", "A flyout command bar", "commandbarflyout"),
        new ControlInfo("MenuBar", "A menu bar with dropdown menus", "menubar"),
        new ControlInfo("MenuFlyout", "A flyout menu for context actions", "menuflyout"),
        new ControlInfo("SwipeControl", "A control for swipe-based interactions", "swipecontrol"),
        new ControlInfo("StandardUICommand", "Pre-defined standard UI commands", "standarduicommand"),
        new ControlInfo("XamlUICommand", "Custom XAML-based UI commands", "xamluicommand")
    };
}

/// <summary>
/// Icons category page.
/// </summary>
public class IconsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Icons";
    protected override string CategoryDescription => "Icon controls and glyph rendering.";
    protected override Color AccentColor => Color.FromRgb(76, 74, 72);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("IconElement", "Display icons and glyphs", "iconelement")
    };
}

/// <summary>
/// Date &amp; Time category page.
/// </summary>
public class DateTimeCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Date & Time";
    protected override string CategoryDescription => "Controls for selecting dates and times.";
    protected override Color AccentColor => Color.FromRgb(0, 120, 212);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("DatePicker", "A control for selecting a date", "datepicker"),
        new ControlInfo("TimePicker", "A control for selecting a time", "timepicker"),
        new ControlInfo("Calendar", "A calendar for date selection", "calendar")
    };
}

/// <summary>
/// Pickers category page.
/// </summary>
public class PickersCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Pickers";
    protected override string CategoryDescription => "Controls for selecting values from specialized pickers.";
    protected override Color AccentColor => Color.FromRgb(232, 17, 35);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("ColorPicker", "A control for selecting colors", "colorpicker")
    };
}

/// <summary>
/// Overlays category page.
/// </summary>
public class OverlaysCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Overlays";
    protected override string CategoryDescription => "Popup, tooltip, and notification overlay controls.";
    protected override Color AccentColor => Color.FromRgb(0, 178, 148);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Popup", "A floating popup overlay", "popup"),
        new ControlInfo("ToolTip", "A tooltip shown on hover", "tooltip"),
        new ControlInfo("ToastNotification", "A non-blocking notification message", "toastnotification")
    };
}

/// <summary>
/// Status &amp; Info category page.
/// </summary>
public class StatusInfoCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Status & Info";
    protected override string CategoryDescription => "Controls for displaying status, information, and feedback.";
    protected override Color AccentColor => Color.FromRgb(16, 124, 16);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("StatusBar", "A bar for displaying status information", "statusbar"),
        new ControlInfo("InfoBar", "An informational notification bar", "infobar"),
        new ControlInfo("Thumb", "A draggable thumb control", "thumb")
    };
}

/// <summary>
/// Dialogs category page.
/// </summary>
public class DialogsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Dialogs";
    protected override string CategoryDescription => "Modal and non-modal dialog controls.";
    protected override Color AccentColor => Color.FromRgb(104, 118, 138);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("File Dialogs", "Open, save, and browse file dialogs", "dialogs"),
        new ControlInfo("ContentDialog", "Modal content dialogs with popup and inline hosting", "contentdialog")
    };
}

/// <summary>
/// Effects category page.
/// </summary>
public class EffectsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Effects";
    protected override string CategoryDescription => "Visual effects, backdrop materials, and transitions.";
    protected override Color AccentColor => Color.FromRgb(136, 23, 152);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Backdrop Effects", "Acrylic and Mica backdrop effects", "backdropeffects"),
        new ControlInfo("Liquid Glass", "Liquid glass material effects", "liquidglass"),
        new ControlInfo("Shader Effects", "Custom shader-based visual effects", "shadereffects"),
        new ControlInfo("Content Transitions", "Animated content transition effects", "transitions")
    };
}

/// <summary>
/// System features category page.
/// </summary>
public class SystemCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "System";
    protected override string CategoryDescription => "System integration and platform features.";
    protected override Color AccentColor => Color.FromRgb(202, 80, 16);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Navigation", "Page navigation and journal support", "navigationdemo"),
        new ControlInfo("Printing", "Document printing support", "printing"),
        new ControlInfo("Shell Integration", "Taskbar, jump lists, and shell features", "shellintegration"),
        new ControlInfo("TitleBar", "Custom window caption bar and system buttons", "titlebar")
    };
}
