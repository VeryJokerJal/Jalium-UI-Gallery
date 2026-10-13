using System.ComponentModel;
using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Models;

/// <summary>
/// Editable sample data presented by the PropertyGrid gallery page.
/// </summary>
public sealed class PropertyGridButtonSample
{
    [Category("Appearance")]
    [Description("The text displayed on the button.")]
    public string Content { get; set; } = "Click Me";

    [Category("Appearance")]
    [Description("The font size of the button text.")]
    public double FontSize { get; set; } = 14;

    [Category("Appearance")]
    [Description("Whether the button text is bold.")]
    public bool IsBold { get; set; }

    [Category("Layout")]
    [Description("The width of the button in pixels.")]
    public double Width { get; set; } = 120;

    [Category("Layout")]
    [Description("The height of the button in pixels.")]
    public double Height { get; set; } = 32;

    [Category("Layout")]
    [Description("The horizontal alignment of the button.")]
    public HorizontalAlignment HorizontalAlignment { get; set; } =
        HorizontalAlignment.Left;

    [Category("Behavior")]
    [Description("Whether the button is enabled.")]
    public bool IsEnabled { get; set; } = true;

    [Category("Behavior")]
    [Description("The tooltip text displayed on hover.")]
    public string ToolTip { get; set; } =
        "Press to perform action";
}

/// <summary>
/// Richer editable sample data for the custom-object PropertyGrid.
/// </summary>
public sealed class PropertyGridWindowSample
{
    [Category("Window")]
    [Description("The title displayed in the window title bar.")]
    public string Title { get; set; } = "My Application";

    [Category("Window")]
    [Description("The width of the window in pixels.")]
    public double Width { get; set; } = 1280;

    [Category("Window")]
    [Description("The height of the window in pixels.")]
    public double Height { get; set; } = 720;

    [Category("Window")]
    [Description("The minimum width the window can be resized to.")]
    public double MinWidth { get; set; } = 400;

    [Category("Window")]
    [Description("The minimum height the window can be resized to.")]
    public double MinHeight { get; set; } = 300;

    [Category("Window")]
    [Description("Whether the window starts maximized.")]
    public bool StartMaximized { get; set; }

    [Category("Window")]
    [Description("The window resize mode.")]
    public GalleryWindowResizeMode ResizeMode { get; set; } =
        GalleryWindowResizeMode.CanResize;

    [Category("Appearance")]
    [Description("The background color of the window.")]
    public string BackgroundColor { get; set; } = "#1E1E1E";

    [Category("Appearance")]
    [Description("The opacity of the window (0.0 to 1.0).")]
    public double Opacity { get; set; } = 1;

    [Category("Appearance")]
    [Description("Whether to show the window icon.")]
    public bool ShowIcon { get; set; } = true;

    [Category("Behavior")]
    [Description("Whether to confirm before closing.")]
    public bool ConfirmOnClose { get; set; }

    [Category("Behavior")]
    [Description("Whether the window is always on top.")]
    public bool TopMost { get; set; }

    [Category("Behavior")]
    [Description("Whether to show in the taskbar.")]
    public bool ShowInTaskbar { get; set; } = true;

    [Category("Content")]
    [Description("The status bar text.")]
    public string StatusText { get; set; } = "Ready";

    [Category("Content")]
    [Description("The number of open tabs.")]
    public int TabCount { get; set; } = 3;
}

public enum GalleryWindowResizeMode
{
    NoResize,
    CanMinimize,
    CanResize,
    CanResizeWithGrip
}
