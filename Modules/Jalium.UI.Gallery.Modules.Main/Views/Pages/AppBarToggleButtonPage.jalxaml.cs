using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for AppBarToggleButtonPage.jalxaml demonstrating AppBarToggleButton functionality.
/// </summary>
public partial class AppBarToggleButtonPage : Page
{
    public AppBarToggleButtonPage()
    {
        InitializeComponent();
        SetupIcons();
        SetupEventHandlers();
        LoadCodeExamples();
    }

    private void SetupIcons()
    {
        // Text formatting toggle icons
        SetToggleContent(BoldToggle, "B", fontWeight: FontWeights.Bold);
        SetToggleContent(ItalicToggle, "I", isItalic: true);
        SetToggleContent(UnderlineToggle, "U", isUnderline: true);

        // Connectivity toggle icons
        SetToggleContent(WifiToggle, "\uD83D\uDCF6");   // 📶 signal
        SetToggleContent(BluetoothToggle, "\u0042");      // B for Bluetooth
        SetToggleContent(AirplaneModeToggle, "\u2708");    // ✈ airplane
    }

    private static void SetToggleContent(AppBarToggleButton? toggle, string text,
        FontWeight? fontWeight = null, bool isItalic = false, bool isUnderline = false)
    {
        if (toggle == null) return;

        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var iconText = new TextBlock
        {
            Text = text,
            FontSize = 16,
            FontWeight = fontWeight ?? FontWeights.Normal,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 2)
        };

        if (isItalic)
            iconText.FontStyle = FontStyles.Italic;

        stack.Children.Add(iconText);

        var labelText = new TextBlock
        {
            Text = toggle.Label,
            FontSize = 10,
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        stack.Children.Add(labelText);

        toggle.Content = stack;
    }

    private void SetupEventHandlers()
    {
        // Text formatting toggles
        if (BoldToggle != null)
            BoldToggle.Checked += (_, _) => UpdateFormattingPreview();
        if (BoldToggle != null)
            BoldToggle.Unchecked += (_, _) => UpdateFormattingPreview();

        if (ItalicToggle != null)
            ItalicToggle.Checked += (_, _) => UpdateFormattingPreview();
        if (ItalicToggle != null)
            ItalicToggle.Unchecked += (_, _) => UpdateFormattingPreview();

        if (UnderlineToggle != null)
            UnderlineToggle.Checked += (_, _) => UpdateFormattingPreview();
        if (UnderlineToggle != null)
            UnderlineToggle.Unchecked += (_, _) => UpdateFormattingPreview();

        // Checked state toggles
        if (WifiToggle != null)
        {
            WifiToggle.Checked += (_, _) => UpdateCheckedStateText();
            WifiToggle.Unchecked += (_, _) => UpdateCheckedStateText();
        }

        if (BluetoothToggle != null)
        {
            BluetoothToggle.Checked += (_, _) => UpdateCheckedStateText();
            BluetoothToggle.Unchecked += (_, _) => UpdateCheckedStateText();
        }

        if (AirplaneModeToggle != null)
        {
            AirplaneModeToggle.Checked += (_, _) => UpdateCheckedStateText();
            AirplaneModeToggle.Unchecked += (_, _) => UpdateCheckedStateText();
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

    private const string XamlExample = @"<!-- Text formatting toggle buttons -->
<StackPanel Orientation=""Horizontal"">
    <AppBarToggleButton x:Name=""BoldToggle""
                        Label=""Bold""
                        Width=""72""
                        Height=""48"" />
    <AppBarToggleButton x:Name=""ItalicToggle""
                        Label=""Italic""
                        Width=""72""
                        Height=""48"" />
    <AppBarToggleButton x:Name=""UnderlineToggle""
                        Label=""Underline""
                        Width=""84""
                        Height=""48"" />
</StackPanel>";

    private const string CSharpExample = @"// Create a toggle button
var toggle = new AppBarToggleButton();
toggle.Label = ""Bold"";
toggle.Icon = new SymbolIcon(Symbol.Bold);

// Handle checked/unchecked events
toggle.Checked += (s, e) =>
{
    Console.WriteLine(""Toggle is ON"");
};
toggle.Unchecked += (s, e) =>
{
    Console.WriteLine(""Toggle is OFF"");
};

// Read the toggle state
bool isOn = toggle.IsChecked == true;";

    private void UpdateFormattingPreview()
    {
        if (FormattingPreviewText == null) return;

        var isBold = BoldToggle?.IsChecked == true;
        var isItalic = ItalicToggle?.IsChecked == true;
        var isUnderline = UnderlineToggle?.IsChecked == true;

        FormattingPreviewText.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
        FormattingPreviewText.FontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;

        // Build a description of active formatting
        var activeFormats = new System.Collections.Generic.List<string>();
        if (isBold) activeFormats.Add("Bold");
        if (isItalic) activeFormats.Add("Italic");
        if (isUnderline) activeFormats.Add("Underline");

        FormattingPreviewText.Text = activeFormats.Count > 0
            ? $"Sample text with {string.Join(", ", activeFormats)} formatting"
            : "Sample text with no formatting";
    }

    private void UpdateCheckedStateText()
    {
        if (CheckedStateText == null) return;

        var wifiState = WifiToggle?.IsChecked == true ? "On" : "Off";
        var btState = BluetoothToggle?.IsChecked == true ? "On" : "Off";
        var airplaneState = AirplaneModeToggle?.IsChecked == true ? "On" : "Off";

        CheckedStateText.Text = $"Wi-Fi: {wifiState} | Bluetooth: {btState} | Airplane Mode: {airplaneState}";
    }
}
