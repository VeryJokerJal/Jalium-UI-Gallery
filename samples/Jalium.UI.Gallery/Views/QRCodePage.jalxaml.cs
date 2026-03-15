using Jalium.UI.Controls;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Views;

public partial class QRCodePage : Page
{
    private QRCodeErrorCorrectionLevel _currentErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

    public QRCodePage()
    {
        InitializeComponent();

        if (PayloadTextBox != null)
        {
            PayloadTextBox.Text = "https://jalium.dev";
            PayloadTextBox.KeyDown += OnPayloadTextBoxKeyDown;
        }

        ApplyPayload();
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
