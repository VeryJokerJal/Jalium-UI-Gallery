using System.Runtime.InteropServices;
using System.Text;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using OpenFolderDialog = Microsoft.Win32.OpenFolderDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public sealed class GallerySystemDialogService
    : IGallerySystemDialogService
{
    public string? OpenFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = TextFilter
        };
        return dialog.ShowDialog() == true
            ? dialog.FileName
            : null;
    }

    public int? OpenMultipleFiles()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "All files (*.*)|*.*",
            Multiselect = true
        };
        return dialog.ShowDialog() == true
            ? dialog.FileNames?.Length ?? 0
            : null;
    }

    public string? PickFolder(bool useInitialDirectory)
    {
        var dialog = new OpenFolderDialog();
        if (useInitialDirectory)
        {
            dialog.InitialDirectory = AppContext.BaseDirectory;
        }

        return dialog.ShowDialog() == true
            ? dialog.FolderName
            : null;
    }

    public string? SaveFile()
    {
        var dialog = new SaveFileDialog
        {
            Filter = TextFilter
        };
        return dialog.ShowDialog() == true
            ? dialog.FileName
            : null;
    }

    public FontDialogSelection? ChooseFont()
    {
        var dialog = new FontDialog();
        return dialog.ShowDialog() && dialog.FontFamily != null
            ? new FontDialogSelection(
                dialog.FontFamily.Source,
                dialog.FontSize)
            : null;
    }

    public string? PickIcon()
    {
        var dialog = new PickIconDialog();
        return dialog.ShowDialog()
            ? $"Icon: {dialog.IconPath} (index {dialog.IconIndex})"
            : null;
    }

    public string ShowOpenWith()
    {
        var target = SampleDocuments(1).First();
        var result = new OpenWithDialog
        {
            FileName = target
        }.ShowDialog();
        return result
            ? $"Opened selector for: {target}"
            : $"Cancelled for: {target}";
    }

    public string ShowFileProperties(bool multiple)
    {
        var targets = SampleDocuments(multiple ? 2 : 1);
        var dialog = multiple
            ? new OpenPropertiesDialog { ObjectNames = targets }
            : new OpenPropertiesDialog
            {
                ObjectName = targets[0],
                PropertyPage = "General"
            };
        var result = dialog.ShowDialog();
        var display = multiple
            ? string.Join(", ", targets.Select(Path.GetFileName))
            : targets[0];
        return result
            ? $"Opened properties for: {display}"
            : $"Cancelled for: {display}";
    }

    public string ShowPrinterProperties()
    {
        var printer = DefaultPrinterName();
        if (string.IsNullOrWhiteSpace(printer))
        {
            return "No default printer was available.";
        }

        var result = new OpenPropertiesDialog
        {
            ObjectType = OpenPropertiesObjectType.PrinterName,
            ObjectName = printer
        }.ShowDialog();
        return result
            ? $"Opened printer properties for: {printer}"
            : $"Cancelled for printer: {printer}";
    }

    public string ShowVolumeProperties()
    {
        var volume = Path.GetPathRoot(Environment.SystemDirectory);
        if (string.IsNullOrWhiteSpace(volume))
        {
            return "No volume path was available.";
        }

        var result = new OpenPropertiesDialog
        {
            ObjectType = OpenPropertiesObjectType.VolumeGuid,
            ObjectName = volume
        }.ShowDialog();
        Jalium.UI.Application.Current?.MainWindow?.Activate();
        return result
            ? $"Opened volume properties for: {volume}"
            : $"Cancelled for volume: {volume}";
    }

    public MessageDialogPreview ShowMessage(MessageDialogKind kind)
    {
        var scenario = Scenario(kind);
        var owner = Jalium.UI.Application.Current?.MainWindow;
        var result = owner != null
            ? MessageBox.Show(
                owner,
                scenario.Message,
                scenario.Caption,
                scenario.Buttons,
                scenario.Icon,
                scenario.DefaultResult)
            : MessageBox.Show(
                scenario.Message,
                scenario.Caption,
                scenario.Buttons,
                scenario.Icon,
                scenario.DefaultResult);

        return new MessageDialogPreview(
            scenario.Caption,
            $"{scenario.Description} {Outcome(kind, result)}",
            "Message: " + scenario.Message
                .Replace(
                    Environment.NewLine + Environment.NewLine,
                    " ")
                .Replace('\n', ' '),
            $"Buttons: {Buttons(scenario.Buttons)}  |  " +
            $"Icon: {scenario.Icon}  |  Default: {scenario.DefaultResult}",
            $"Result: {result}");
    }

    private static ScenarioData Scenario(MessageDialogKind kind) =>
        kind switch
        {
            MessageDialogKind.Warning => new(
                "Unsaved changes",
                "The preview still has staged edits.\n\nTry saving again before you leave this page?",
                "A recovery-oriented warning that allows a safe cancel.",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.OK),
            MessageDialogKind.Error => new(
                "Import failed",
                "Jalium.UI.Gallery couldn't parse one of the selected assets.\n\nCheck the file path and try again.",
                "A blocking error with a system icon.",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK),
            MessageDialogKind.Confirm => new(
                "Replace accent palette?",
                "This action will update every preview card.\n\nDo you want to continue?",
                "A three-way confirmation with a safer default.",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.No),
            _ => new(
                "Sync complete",
                "Your theme preset was applied.\n\nEverything is already up to date.",
                "A lightweight acknowledgement for completed work.",
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.OK)
        };

    private static string Outcome(
        MessageDialogKind kind,
        MessageBoxResult result) =>
        kind switch
        {
            MessageDialogKind.Warning when result == MessageBoxResult.OK =>
                "The user chose to retry.",
            MessageDialogKind.Warning =>
                "The user canceled the retry.",
            MessageDialogKind.Confirm when result == MessageBoxResult.Yes =>
                "The replacement was approved.",
            MessageDialogKind.Confirm when result == MessageBoxResult.No =>
                "The existing palette was kept.",
            MessageDialogKind.Confirm =>
                "The user backed out.",
            _ => "The dialog was acknowledged."
        };

    private static string Buttons(MessageBoxButton buttons) =>
        buttons switch
        {
            MessageBoxButton.OK => "OK",
            MessageBoxButton.OKCancel => "OK / Cancel",
            MessageBoxButton.YesNoCancel => "Yes / No / Cancel",
            MessageBoxButton.YesNo => "Yes / No",
            _ => buttons.ToString()
        };

    private static string[] SampleDocuments(int count)
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "Jalium.UI.Gallery",
            "Dialogs");
        Directory.CreateDirectory(directory);
        return Enumerable.Range(1, count)
            .Select(index =>
            {
                var path = Path.Combine(
                    directory,
                    $"sample-document-{index}.txt");
                if (!File.Exists(path))
                {
                    File.WriteAllText(
                        path,
                        $"Jalium.UI dialog sample document {index}");
                }

                return path;
            })
            .ToArray();
    }

    private static string? DefaultPrinterName()
    {
        var length = 0;
        _ = GetDefaultPrinter(null, ref length);
        if (length <= 0)
        {
            return null;
        }

        var buffer = new StringBuilder(length);
        return GetDefaultPrinter(buffer, ref length)
            ? buffer.ToString()
            : null;
    }

    [DllImport(
        "winspool.drv",
        CharSet = CharSet.Unicode,
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetDefaultPrinter(
        StringBuilder? buffer,
        ref int length);

    private const string TextFilter =
        "Text files (*.txt)|*.txt|All files (*.*)|*.*";

    private sealed record ScenarioData(
        string Caption,
        string Message,
        string Description,
        MessageBoxButton Buttons,
        MessageBoxImage Icon,
        MessageBoxResult DefaultResult);
}
