using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ColorPickerPage.jalxaml demonstrating color picker functionality.
/// </summary>
public partial class ColorPickerPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic ColorPicker -->
    <ColorPicker x:Name=""BasicColorPicker""
                 Margin=""0,0,0,16""/>

    <!-- Compact ColorPicker -->
    <ColorPicker IsCompact=""True""
                 Margin=""0,0,0,16""/>

    <!-- ColorPicker with Alpha Channel -->
    <ColorPicker IsAlphaEnabled=""True""
                 Margin=""0,0,0,16""/>

    <!-- ColorPicker without Alpha Channel -->
    <ColorPicker IsAlphaEnabled=""False""
                 Margin=""0,0,0,16""/>

    <!-- Interactive Demo with preview -->
    <StackPanel Orientation=""Horizontal"">
        <ColorPicker x:Name=""DemoColorPicker"" Width=""300""/>
        <Border x:Name=""ColorPreview""
                Width=""100"" Height=""100""
                Background=""#0078D4""
                CornerRadius=""8""
                Margin=""24,0,0,0""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class ColorPickerSample : Page
{
    public ColorPickerSample()
    {
        InitializeComponent();

        // Subscribe to color changes
        if (DemoColorPicker != null)
            DemoColorPicker.ColorChanged += OnColorChanged;
    }

    private void OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        // Update preview with the selected color
        if (ColorPreview != null)
            ColorPreview.Background = new SolidColorBrush(e.NewColor);

        // Get hex string representation
        string hex = $""#{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}"";

        // Access individual color components
        byte r = e.NewColor.R;
        byte g = e.NewColor.G;
        byte b = e.NewColor.B;
        byte a = e.NewColor.A;
    }
}";

    public ColorPickerPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        // Set up event handler for the interactive demo
        if (DemoColorPicker != null)
        {
            DemoColorPicker.ColorChanged += OnDemoColorChanged;
        }
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

    private void OnDemoColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (ColorPreview != null)
        {
            ColorPreview.Background = new SolidColorBrush(e.NewColor);
        }

        if (ColorHexText != null)
        {
            ColorHexText.Text = $"#{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}";
        }
    }
}
