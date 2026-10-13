using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class ComboBoxViewModel
{
    private string _selectedColor = "Blue";
    private string? _selectedDay;

    public IReadOnlyList<string> Fruits { get; } =
        ["Apple", "Banana", "Cherry", "Orange", "Grape"];

    public IReadOnlyList<string> Colors { get; } =
        ["Red", "Green", "Blue", "Yellow"];

    public IReadOnlyList<string> Days { get; } =
    [
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday"
    ];

    public string SelectedColor
    {
        get => _selectedColor;
        set => SetProperty(ref _selectedColor, value);
    }

    public string? SelectedDay
    {
        get => _selectedDay;
        set
        {
            if (SetProperty(ref _selectedDay, value))
            {
                OnPropertyChanged(nameof(SelectedDayText));
            }
        }
    }

    public string SelectedDayText => $"Selected: {SelectedDay ?? "(none)"}";
}

public sealed partial class PrintingViewModel
{
    private readonly IGalleryPrintingService _printingService;
    private string _printStatus = "Print Status: Ready";
    private string _printerList = "Click to list printers...";

    public PrintingViewModel(IGalleryPrintingService printingService)
    {
        ArgumentNullException.ThrowIfNull(printingService);
        _printingService = printingService;

        ShowPrintDialogCommand = new RelayCommand(ShowPrintDialog);
        PrintVisualCommand = new RelayCommand(() =>
            PrintStatus =
                "Print Status: Print Visual demo (not connected to actual printing)");
        ListPrintersCommand = new RelayCommand(ListPrinters);
    }

    public ICommand ShowPrintDialogCommand { get; }

    public ICommand PrintVisualCommand { get; }

    public ICommand ListPrintersCommand { get; }

    public string PrintStatus
    {
        get => _printStatus;
        private set => SetProperty(ref _printStatus, value);
    }

    public string PrinterList
    {
        get => _printerList;
        private set => SetProperty(ref _printerList, value);
    }

    private void ShowPrintDialog()
    {
        var accepted = _printingService.ShowPrintDialog();
        PrintStatus = accepted == true
            ? "Print Status: Print dialog accepted"
            : "Print Status: Print dialog cancelled";
    }

    private void ListPrinters()
    {
        try
        {
            PrinterList = _printingService.GetAvailablePrinters();
        }
        catch (Exception exception)
        {
            PrinterList =
                $"Unable to enumerate printers: {exception.Message}";
        }
    }
}
