using System.ComponentModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for PropertyGridPage.jalxaml demonstrating the PropertyGrid control.
/// </summary>
public partial class PropertyGridPage : Page
{
    private const string XamlExample = """
        <!-- Basic Property Grid with categorized view -->
        <PropertyGrid x:Name="BasicPropertyGrid"
                      Height="350"
                      SortMode="Categorized"
                      ShowSearchBox="True"
                      ShowDescription="True"/>

        <!-- Side-by-side: Categorized vs Alphabetical -->
        <PropertyGrid x:Name="CategorizedGrid"
                      Height="300"
                      SortMode="Categorized"
                      ShowSearchBox="False"
                      ShowDescription="False"/>
        <PropertyGrid x:Name="AlphabeticalGrid"
                      Height="300"
                      SortMode="Alphabetical"
                      ShowSearchBox="False"
                      ShowDescription="False"/>

        <!-- Custom object with toolbar -->
        <PropertyGrid x:Name="CustomObjectGrid"
                      Height="400"
                      SortMode="Categorized"
                      ShowSearchBox="True"
                      ShowDescription="True"
                      ShowToolBar="True"/>
        """;

    private const string CSharpExample = """
        using System.ComponentModel;
        using Jalium.UI.Controls;

        // Bind an object to the PropertyGrid
        propertyGrid.SelectedObject = myObject;

        // Configure sort mode
        propertyGrid.SortMode = PropertySortMode.Categorized;
        // or
        propertyGrid.SortMode = PropertySortMode.Alphabetical;

        // Show/hide features
        propertyGrid.ShowSearchBox = true;
        propertyGrid.ShowDescription = true;
        propertyGrid.ShowToolBar = true;

        // Sample object with categories and descriptions
        public class SampleObject
        {
            [Category("Appearance")]
            [Description("The display text.")]
            public string Content { get; set; } = "Hello";

            [Category("Layout")]
            [Description("Width in pixels.")]
            public double Width { get; set; } = 120.0;

            [Category("Behavior")]
            [Description("Whether the control is enabled.")]
            public bool IsEnabled { get; set; } = true;
        }
        """;

    public PropertyGridPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        var basicSample = new ButtonSampleObject();

        if (BasicPropertyGrid != null)
        {
            BasicPropertyGrid.SelectedObject = basicSample;
        }

        if (CategorizedGrid != null)
        {
            CategorizedGrid.SelectedObject = basicSample;
        }

        if (AlphabeticalGrid != null)
        {
            AlphabeticalGrid.SelectedObject = basicSample;
        }

        if (CustomObjectGrid != null)
        {
            CustomObjectGrid.SelectedObject = new WindowSampleObject();
        }
    }

    /// <summary>
    /// A sample object representing button-like properties for the basic PropertyGrid demo.
    /// </summary>
    private class ButtonSampleObject
    {
        [Category("Appearance")]
        [Description("The text displayed on the button.")]
        public string Content { get; set; } = "Click Me";

        [Category("Appearance")]
        [Description("The font size of the button text.")]
        public double FontSize { get; set; } = 14.0;

        [Category("Appearance")]
        [Description("Whether the button text is bold.")]
        public bool IsBold { get; set; } = false;

        [Category("Layout")]
        [Description("The width of the button in pixels.")]
        public double Width { get; set; } = 120.0;

        [Category("Layout")]
        [Description("The height of the button in pixels.")]
        public double Height { get; set; } = 32.0;

        [Category("Layout")]
        [Description("The horizontal alignment of the button.")]
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;

        [Category("Behavior")]
        [Description("Whether the button is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Category("Behavior")]
        [Description("The tooltip text displayed on hover.")]
        public string ToolTip { get; set; } = "Press to perform action";
    }

    /// <summary>
    /// A richer sample object with more property types for the custom object demo.
    /// </summary>
    private class WindowSampleObject
    {
        [Category("Window")]
        [Description("The title displayed in the window title bar.")]
        public string Title { get; set; } = "My Application";

        [Category("Window")]
        [Description("The width of the window in pixels.")]
        public double Width { get; set; } = 1280.0;

        [Category("Window")]
        [Description("The height of the window in pixels.")]
        public double Height { get; set; } = 720.0;

        [Category("Window")]
        [Description("The minimum width the window can be resized to.")]
        public double MinWidth { get; set; } = 400.0;

        [Category("Window")]
        [Description("The minimum height the window can be resized to.")]
        public double MinHeight { get; set; } = 300.0;

        [Category("Window")]
        [Description("Whether the window starts maximized.")]
        public bool StartMaximized { get; set; } = false;

        [Category("Window")]
        [Description("The window resize mode.")]
        public ResizeMode ResizeMode { get; set; } = ResizeMode.CanResize;

        [Category("Appearance")]
        [Description("The background color of the window.")]
        public string BackgroundColor { get; set; } = "#1E1E1E";

        [Category("Appearance")]
        [Description("The opacity of the window (0.0 to 1.0).")]
        public double Opacity { get; set; } = 1.0;

        [Category("Appearance")]
        [Description("Whether to show the window icon.")]
        public bool ShowIcon { get; set; } = true;

        [Category("Behavior")]
        [Description("Whether to confirm before closing.")]
        public bool ConfirmOnClose { get; set; } = false;

        [Category("Behavior")]
        [Description("Whether the window is always on top.")]
        public bool TopMost { get; set; } = false;

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

    private enum ResizeMode
    {
        NoResize,
        CanMinimize,
        CanResize,
        CanResizeWithGrip
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }
}
