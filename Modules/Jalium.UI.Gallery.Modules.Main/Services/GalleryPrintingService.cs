using System.Printing;
using System.Text;
using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryPrintingService : IGalleryPrintingService
{
    public bool? ShowPrintDialog()
    {
        var printDialog = new PrintDialog
        {
            MinPage = 1,
            MaxPage = 100,
            UserPageRangeEnabled = true
        };

        return printDialog.ShowDialog();
    }

    public string GetAvailablePrinters()
    {
        using var printServer = new PrintServer();
        var printers = printServer.GetPrintQueues().ToList();
        var text = new StringBuilder()
            .AppendLine("Available Printers:")
            .AppendLine("------------------");

        if (printers.Count == 0)
        {
            return text
                .AppendLine("(No printers found - this is a demo)")
                .AppendLine()
                .AppendLine("Demo Printer List:")
                .AppendLine("  - Microsoft Print to PDF")
                .AppendLine("  - Microsoft XPS Document Writer")
                .AppendLine("  - OneNote (Desktop)")
                .ToString();
        }

        foreach (var printer in printers)
        {
            text.AppendLine($"  - {printer.Name}");
            if (!string.IsNullOrEmpty(printer.Description))
            {
                text.AppendLine($"    Description: {printer.Description}");
            }
        }

        return text.ToString();
    }
}
