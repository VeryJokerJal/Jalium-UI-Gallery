using System.ComponentModel;
using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Models;

/// <summary>
/// Editable sample data presented by the PropertyGrid gallery page.
/// </summary>
public sealed class PropertyGridButtonSample : ViewModelBase
{
    private string _content = "Click Me";
    private double _fontSize = 14;
    private bool _isBold;
    private double _width = 120;
    private double _height = 32;
    private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
    private bool _isEnabled = true;
    private string _toolTip = "Press to perform action";

    [Category("Appearance")]
    [Description("The text displayed on the button.")]
    public string Content { get => _content; set => SetProperty(ref _content, value); }

    [Category("Appearance")]
    [Description("The font size of the button text.")]
    public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

    [Category("Appearance")]
    [Description("Whether the button text is bold.")]
    public bool IsBold { get => _isBold; set => SetProperty(ref _isBold, value); }

    [Category("Layout")]
    [Description("The width of the button in pixels.")]
    public double Width { get => _width; set => SetProperty(ref _width, value); }

    [Category("Layout")]
    [Description("The height of the button in pixels.")]
    public double Height { get => _height; set => SetProperty(ref _height, value); }

    [Category("Layout")]
    [Description("The horizontal alignment of the button.")]
    public HorizontalAlignment HorizontalAlignment { get => _horizontalAlignment; set => SetProperty(ref _horizontalAlignment, value); }

    [Category("Behavior")]
    [Description("Whether the button is enabled.")]
    public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

    [Category("Behavior")]
    [Description("The tooltip text displayed on hover.")]
    public string ToolTip { get => _toolTip; set => SetProperty(ref _toolTip, value); }
}

/// <summary>
/// Richer editable sample data for the custom-object PropertyGrid.
/// </summary>
public sealed class PropertyGridWindowSample : ViewModelBase
{
    private string _title = "My Application";
    private double _width = 1280;
    private double _height = 720;
    private double _minWidth = 400;
    private double _minHeight = 300;
    private bool _startMaximized;
    private GalleryWindowResizeMode _resizeMode = GalleryWindowResizeMode.CanResize;
    private string _backgroundColor = "#1E1E1E";
    private double _opacity = 1;
    private bool _showIcon = true;
    private bool _confirmOnClose;
    private bool _topMost;
    private bool _showInTaskbar = true;
    private string _statusText = "Ready";
    private int _tabCount = 3;

    [Category("Window")]
    [Description("The title displayed in the window title bar.")]
    public string Title { get => _title; set => SetProperty(ref _title, value); }

    [Category("Window")]
    [Description("The width of the window in pixels.")]
    public double Width { get => _width; set => SetProperty(ref _width, value); }

    [Category("Window")]
    [Description("The height of the window in pixels.")]
    public double Height { get => _height; set => SetProperty(ref _height, value); }

    [Category("Window")]
    [Description("The minimum width the window can be resized to.")]
    public double MinWidth { get => _minWidth; set => SetProperty(ref _minWidth, value); }

    [Category("Window")]
    [Description("The minimum height the window can be resized to.")]
    public double MinHeight { get => _minHeight; set => SetProperty(ref _minHeight, value); }

    [Category("Window")]
    [Description("Whether the window starts maximized.")]
    public bool StartMaximized { get => _startMaximized; set => SetProperty(ref _startMaximized, value); }

    [Category("Window")]
    [Description("The window resize mode.")]
    public GalleryWindowResizeMode ResizeMode { get => _resizeMode; set => SetProperty(ref _resizeMode, value); }

    [Category("Appearance")]
    [Description("The background color of the window.")]
    public string BackgroundColor { get => _backgroundColor; set => SetProperty(ref _backgroundColor, value); }

    [Category("Appearance")]
    [Description("The opacity of the window (0.0 to 1.0).")]
    public double Opacity { get => _opacity; set => SetProperty(ref _opacity, value); }

    [Category("Appearance")]
    [Description("Whether to show the window icon.")]
    public bool ShowIcon { get => _showIcon; set => SetProperty(ref _showIcon, value); }

    [Category("Behavior")]
    [Description("Whether to confirm before closing.")]
    public bool ConfirmOnClose { get => _confirmOnClose; set => SetProperty(ref _confirmOnClose, value); }

    [Category("Behavior")]
    [Description("Whether the window is always on top.")]
    public bool TopMost { get => _topMost; set => SetProperty(ref _topMost, value); }

    [Category("Behavior")]
    [Description("Whether to show in the taskbar.")]
    public bool ShowInTaskbar { get => _showInTaskbar; set => SetProperty(ref _showInTaskbar, value); }

    [Category("Content")]
    [Description("The status bar text.")]
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

    [Category("Content")]
    [Description("The number of open tabs.")]
    public int TabCount { get => _tabCount; set => SetProperty(ref _tabCount, value); }
}

public enum GalleryWindowResizeMode
{
    NoResize,
    CanMinimize,
    CanResize,
    CanResizeWithGrip
}
