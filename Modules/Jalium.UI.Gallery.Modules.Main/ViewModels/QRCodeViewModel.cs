using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Payload, options and commands for the interactive QR code generator.</summary>
public sealed class QRCodeViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string DefaultPayload = "https://jalium.dev";

    private const string XamlCode = """
        <TextBox Text="{Binding PayloadInput, Mode=TwoWay}"/>
        <Button Content="Generate" Command="{Binding GenerateCommand}"/>
        <QRCode Text="{Binding Payload}"
                ErrorCorrectionLevel="{Binding ErrorCorrectionLevel}"/>
        """;

    private const string CSharpCode = """
        // QRCodeViewModel.cs
        public sealed class QRCodeViewModel : ViewModelBase
        {
            public ICommand GenerateCommand { get; }
            public ICommand SetErrorCorrectionCommand { get; }

            // The QRCode control renders bound payload and option properties.
        }
        """;

    private string _payloadInput = DefaultPayload;
    private string _payload = DefaultPayload;
    private QRCodeErrorCorrectionLevel _errorCorrectionLevel =
        QRCodeErrorCorrectionLevel.Q;

    public QRCodeViewModel()
    {
        GenerateCommand = new RelayCommand(Generate);
        SetErrorCorrectionCommand = new RelayCommand<string>(SetErrorCorrection);
    }

    public string PayloadInput
    {
        get => _payloadInput;
        set => SetProperty(ref _payloadInput, value);
    }

    public string Payload
    {
        get => _payload;
        private set
        {
            if (SetProperty(ref _payload, value))
            {
                OnPropertyChanged(nameof(PayloadSummaryText));
            }
        }
    }

    public QRCodeErrorCorrectionLevel ErrorCorrectionLevel
    {
        get => _errorCorrectionLevel;
        private set
        {
            if (SetProperty(ref _errorCorrectionLevel, value))
            {
                OnPropertyChanged(nameof(GeneratorStatusText));
            }
        }
    }

    public string GeneratorStatusText => $"ECC: {ErrorCorrectionLevel}";

    public string PayloadSummaryText => $"Payload: {Payload}";

    public ICommand GenerateCommand { get; }

    public ICommand SetErrorCorrectionCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    private void Generate()
    {
        var payload = PayloadInput.Trim();
        if (payload.Length == 0)
        {
            payload = DefaultPayload;
            PayloadInput = payload;
        }

        Payload = payload;
    }

    private void SetErrorCorrection(string? level)
    {
        ErrorCorrectionLevel = level switch
        {
            "L" => QRCodeErrorCorrectionLevel.L,
            "M" => QRCodeErrorCorrectionLevel.M,
            "H" => QRCodeErrorCorrectionLevel.H,
            _ => QRCodeErrorCorrectionLevel.Q
        };
    }
}
