using System.Text;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for HexEditorPage.jalxaml demonstrating the HexEditor control.
/// </summary>
public partial class HexEditorPage : Page
{
    private const string XamlExample = """
        <!-- Basic Hex Editor -->
        <HexEditor x:Name="BasicHexEditor"
                   Height="300"
                   BytesPerRow="16"
                   ShowOffsetColumn="True"
                   ShowAsciiColumn="True"/>

        <!-- Configurable Hex Editor -->
        <CheckBox x:Name="ShowAsciiCheckBox"
                  Content="Show ASCII Column" IsChecked="True"/>
        <CheckBox x:Name="ShowOffsetCheckBox"
                  Content="Show Offset Column" IsChecked="True"/>
        <CheckBox x:Name="ShowDataInterpretCheckBox"
                  Content="Show Data Interpretation"/>
        <Slider x:Name="BytesPerRowSlider"
                Minimum="8" Maximum="32" Value="16"/>
        <HexEditor x:Name="ConfigHexEditor"
                   Height="250" BytesPerRow="16"/>

        <!-- Read-Only Hex Viewer -->
        <HexEditor x:Name="ReadOnlyHexEditor"
                   Height="200"
                   IsReadOnly="True"
                   BytesPerRow="16"
                   ShowOffsetColumn="True"
                   ShowAsciiColumn="True"/>
        """;

    private const string CSharpExample = """
        using System.Text;
        using Jalium.UI.Controls;

        // Load binary data into the hex editor
        var data = new byte[256];
        data[0] = 0x4D; // 'M'
        data[1] = 0x5A; // 'Z'

        // Copy ASCII string into data
        var msg = Encoding.ASCII.GetBytes("Hello HexEditor!");
        Array.Copy(msg, 0, data, 64, msg.Length);

        hexEditor.Data = data;

        // Configure display options
        hexEditor.BytesPerRow = 16;
        hexEditor.ShowOffsetColumn = true;
        hexEditor.ShowAsciiColumn = true;
        hexEditor.ShowDataInterpretation = false;
        hexEditor.IsReadOnly = false;

        // React to configuration changes
        showAsciiCheckBox.Checked += (s, e) =>
            hexEditor.ShowAsciiColumn = true;
        showAsciiCheckBox.Unchecked += (s, e) =>
            hexEditor.ShowAsciiColumn = false;

        bytesPerRowSlider.ValueChanged += (s, e) =>
            hexEditor.BytesPerRow = (int)e.NewValue;
        """;

    public HexEditorPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        // Generate sample byte data: a mix of printable ASCII and binary values
        var sampleData = GenerateSampleData();

        if (BasicHexEditor != null)
        {
            BasicHexEditor.Data = sampleData;
        }

        if (ConfigHexEditor != null)
        {
            ConfigHexEditor.Data = sampleData;
        }

        if (ReadOnlyHexEditor != null)
        {
            ReadOnlyHexEditor.Data = GenerateReadOnlyData();
        }

        // Wire configuration controls
        if (ShowAsciiCheckBox != null)
        {
            ShowAsciiCheckBox.Checked += (s, e) => UpdateConfigHexEditor();
            ShowAsciiCheckBox.Unchecked += (s, e) => UpdateConfigHexEditor();
        }

        if (ShowOffsetCheckBox != null)
        {
            ShowOffsetCheckBox.Checked += (s, e) => UpdateConfigHexEditor();
            ShowOffsetCheckBox.Unchecked += (s, e) => UpdateConfigHexEditor();
        }

        if (ShowDataInterpretCheckBox != null)
        {
            ShowDataInterpretCheckBox.Checked += (s, e) => UpdateConfigHexEditor();
            ShowDataInterpretCheckBox.Unchecked += (s, e) => UpdateConfigHexEditor();
        }

        if (BytesPerRowSlider != null)
        {
            BytesPerRowSlider.ValueChanged += OnBytesPerRowChanged;
        }
    }

    private void OnBytesPerRowChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var value = (int)e.NewValue;
        if (BytesPerRowLabel != null)
        {
            BytesPerRowLabel.Text = value.ToString();
        }

        if (ConfigHexEditor != null)
        {
            ConfigHexEditor.BytesPerRow = value;
        }
    }

    private void UpdateConfigHexEditor()
    {
        if (ConfigHexEditor == null) return;

        ConfigHexEditor.ShowAsciiColumn = ShowAsciiCheckBox?.IsChecked ?? true;
        ConfigHexEditor.ShowOffsetColumn = ShowOffsetCheckBox?.IsChecked ?? true;
        ConfigHexEditor.ShowDataInterpretation = ShowDataInterpretCheckBox?.IsChecked ?? false;
    }

    private static byte[] GenerateSampleData()
    {
        // Create data that includes a PE-like header pattern and readable ASCII
        var data = new byte[256];

        // "MZ" header signature
        data[0] = 0x4D; // M
        data[1] = 0x5A; // Z
        data[2] = 0x90;
        data[3] = 0x00;

        // Fill with a pattern
        for (int i = 4; i < 64; i++)
        {
            data[i] = (byte)(i * 3 + 0x10);
        }

        // ASCII string section
        var message = Encoding.ASCII.GetBytes("Hello from Jalium.UI HexEditor!");
        Array.Copy(message, 0, data, 64, message.Length);

        // Fill remaining with incrementing bytes
        for (int i = 64 + message.Length; i < data.Length; i++)
        {
            data[i] = (byte)(i & 0xFF);
        }

        return data;
    }

    private static byte[] GenerateReadOnlyData()
    {
        // PNG-like file header for demo
        var data = new byte[128];
        // PNG signature
        data[0] = 0x89;
        data[1] = 0x50; // P
        data[2] = 0x4E; // N
        data[3] = 0x47; // G
        data[4] = 0x0D;
        data[5] = 0x0A;
        data[6] = 0x1A;
        data[7] = 0x0A;

        // IHDR chunk
        data[8] = 0x00;
        data[9] = 0x00;
        data[10] = 0x00;
        data[11] = 0x0D;
        data[12] = 0x49; // I
        data[13] = 0x48; // H
        data[14] = 0x44; // D
        data[15] = 0x52; // R

        // Fill rest with sample image data pattern
        for (int i = 16; i < data.Length; i++)
        {
            data[i] = (byte)((i * 7 + 0x33) & 0xFF);
        }

        return data;
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
