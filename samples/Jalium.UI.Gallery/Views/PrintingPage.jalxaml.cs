using System.Text;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Printing;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class PrintingPage : Page
{
    public PrintingPage()
    {
        InitializeComponent();
        SetupButtons();
        LoadCodeExamples();
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

    private const string XamlExample =
@"<!-- Print button in a toolbar -->
<StackPanel Orientation=""Horizontal"">
    <Button x:Name=""ShowPrintDialogButton""
            Content=""Show Print Dialog""
            Width=""150""/>
    <Button x:Name=""PrintVisualButton""
            Content=""Print Visual""
            Width=""120""/>
</StackPanel>

<!-- Printer list display -->
<Border Background=""#1E1E1E"" CornerRadius=""4"" Padding=""12"">
    <ScrollViewer VerticalScrollBarVisibility=""Auto"">
        <TextBlock x:Name=""PrinterList""
                   FontFamily=""Consolas""
                   FontSize=""12""
                   Foreground=""#A0A0A0""/>
    </ScrollViewer>
</Border>";

    private const string CSharpExample =
@"// Show a print dialog
var printDialog = new PrintDialog
{
    MinPage = 1,
    MaxPage = 100,
    UserPageRangeEnabled = true
};

bool? result = printDialog.ShowDialog();
if (result == true)
{
    // User accepted - proceed with printing
}

// Print a visual element directly
printDialog.PrintVisual(myElement, ""Document Title"");

// Print a paginated document
var paginator = document.DocumentPaginator;
paginator.PageSize = new Size(816, 1056); // Letter size
printDialog.PrintDocument(paginator, ""My Document"");

// List available printers
var printers = PrintQueue.GetPrintQueues();
foreach (var printer in printers)
{
    Console.WriteLine($""{printer.Name}"");
    if (printer.IsDefault)
        Console.WriteLine(""  [Default Printer]"");

    // Query printer capabilities
    var caps = printer.GetPrintCapabilities();
    // caps.PageSizes, caps.Orientations, etc.
}";
}
