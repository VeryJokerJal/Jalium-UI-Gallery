using System.Text;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Printing;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class PrintingPage : Page
{
    public PrintingPage()
    {
        InitializeComponent();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (ShowPrintDialogButton != null)
        {
            ShowPrintDialogButton.Click += (s, e) =>
            {
                var printDialog = new PrintDialog
                {
                    MinPage = 1,
                    MaxPage = 100,
                    UserPageRangeEnabled = true
                };

                var result = printDialog.ShowDialog();
                UpdateStatus(result ? "Print dialog accepted" : "Print dialog cancelled");
            };
        }

        if (PrintVisualButton != null)
        {
            PrintVisualButton.Click += (s, e) =>
            {
                var printDialog = new PrintDialog();
                // In a real app, we would print the actual visual
                // printDialog.PrintVisual(this, "Gallery Page");
                UpdateStatus("Print Visual demo (not connected to actual printing)");
            };
        }

        if (ListPrintersButton != null)
        {
            ListPrintersButton.Click += (s, e) =>
            {
                var printers = PrintQueue.GetPrintQueues();
                var sb = new StringBuilder();
                sb.AppendLine("Available Printers:");
                sb.AppendLine("------------------");

                var printerList = printers.ToList();
                if (printerList.Count == 0)
                {
                    sb.AppendLine("(No printers found - this is a demo)");
                    sb.AppendLine();
                    sb.AppendLine("Demo Printer List:");
                    sb.AppendLine("  - Microsoft Print to PDF");
                    sb.AppendLine("  - Microsoft XPS Document Writer");
                    sb.AppendLine("  - OneNote (Desktop)");
                }
                else
                {
                    foreach (var printer in printerList)
                    {
                        var defaultMark = printer.IsDefault ? " [Default]" : "";
                        sb.AppendLine($"  - {printer.Name}{defaultMark}");
                        if (!string.IsNullOrEmpty(printer.Description))
                        {
                            sb.AppendLine($"    Description: {printer.Description}");
                        }
                    }
                }

                if (PrinterList != null)
                {
                    PrinterList.Text = sb.ToString();
                }
            };
        }
    }

    private void UpdateStatus(string message)
    {
        if (PrintStatus != null)
        {
            PrintStatus.Text = $"Print Status: {message}";
        }
    }
}
