using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Views;

public partial class QRCodePage : Page
{
    private QRCodeErrorCorrectionLevel _currentErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic QR Code with default settings -->
    <QRCode Text=""https://jalium.dev""
            Width=""220"" Height=""220""
            Margin=""0,0,0,16""/>

    <!-- QR Code with custom error correction -->
    <QRCode Text=""https://jalium.dev/docs""
            Width=""140"" Height=""140""
            QuietZoneModules=""2""
            ErrorCorrectionLevel=""H""
            Margin=""0,0,0,16""/>

    <!-- Wi-Fi QR Code -->
    <QRCode Text=""WIFI:T:WPA;S:MyNetwork;P:password123;;""
            Width=""140"" Height=""140""
            QuietZoneModules=""2""
            ErrorCorrectionLevel=""H""/>

    <!-- vCard Contact QR Code -->
    <QRCode Text=""BEGIN:VCARD;FN:John Doe;EMAIL:john@example.com;END:VCARD;""
            Width=""140"" Height=""140""
            QuietZoneModules=""2""/>

    <!-- Interactive Generator -->
    <StackPanel>
        <TextBox x:Name=""PayloadTextBox""
                 PlaceholderText=""Enter payload...""/>
        <Button Content=""Generate""
                Click=""OnGenerateClick""/>
        <QRCode x:Name=""DemoQRCode""
                Width=""220"" Height=""220""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class QRCodeSample : Page
{
    private QRCodeErrorCorrectionLevel _ecc = QRCodeErrorCorrectionLevel.Q;

    public QRCodeSample()
    {
        InitializeComponent();

        // Set initial payload
        if (PayloadTextBox != null)
            PayloadTextBox.Text = ""https://jalium.dev"";

        ApplyPayload();
    }

    private void OnGenerateClick(object sender, RoutedEventArgs e)
    {
        ApplyPayload();
    }

    private void OnErrorCorrectionClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
        {
            _ecc = tag switch
            {
                ""L"" => QRCodeErrorCorrectionLevel.L,
                ""M"" => QRCodeErrorCorrectionLevel.M,
                ""H"" => QRCodeErrorCorrectionLevel.H,
                _   => QRCodeErrorCorrectionLevel.Q
            };
            ApplyPayload();
        }
    }

    private void ApplyPayload()
    {
        var payload = PayloadTextBox?.Text?.Trim();
        if (string.IsNullOrWhiteSpace(payload))
            payload = ""https://jalium.dev"";

        if (DemoQRCode != null)
        {
            DemoQRCode.Text = payload;
            DemoQRCode.ErrorCorrectionLevel = _ecc;
        }
    }
}";

    public QRCodePage()
    {
        InitializeComponent();
        LoadCodeExamples();

        if (PayloadTextBox != null)
        {
            PayloadTextBox.Text = "https://jalium.dev";
            PayloadTextBox.KeyDown += OnPayloadTextBoxKeyDown;
        }

        ApplyPayload();
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

    private void OnGenerateClick(object sender, RoutedEventArgs e)
    {
        ApplyPayload();
    }

    private void OnErrorCorrectionClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
        {
            return;
        }

        _currentErrorCorrectionLevel = tag switch
        {
            "L" => QRCodeErrorCorrectionLevel.L,
            "M" => QRCodeErrorCorrectionLevel.M,
            "H" => QRCodeErrorCorrectionLevel.H,
            _ => QRCodeErrorCorrectionLevel.Q
        };

        ApplyPayload();
    }

    private void OnPayloadTextBoxKeyDown(object? sender, RoutedEventArgs e)
    {
        if (e is not KeyEventArgs keyArgs || keyArgs.Key != Key.Enter)
        {
            return;
        }

        ApplyPayload();
        keyArgs.Handled = true;
    }

    private void ApplyPayload()
    {
        var payload = PayloadTextBox?.Text?.Trim();
        if (string.IsNullOrWhiteSpace(payload))
        {
            payload = "https://jalium.dev";
            if (PayloadTextBox != null)
            {
                PayloadTextBox.Text = payload;
            }
        }

        if (DemoQRCode != null)
        {
            DemoQRCode.Text = payload;
            DemoQRCode.ErrorCorrectionLevel = _currentErrorCorrectionLevel;
        }

        if (GeneratorStatusText != null)
        {
            GeneratorStatusText.Text = $"ECC: {_currentErrorCorrectionLevel}";
        }

        if (PayloadSummaryText != null)
        {
            PayloadSummaryText.Text = $"Payload: {payload}";
        }
    }
}
