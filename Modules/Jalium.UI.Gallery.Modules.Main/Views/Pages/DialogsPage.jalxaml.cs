using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;
using System.Runtime.InteropServices;
using System.Text;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for DialogsPage.jalxaml demonstrating dialog functionality.
/// </summary>
public partial class DialogsPage : Page
{
    private const string XamlExample =
@"<!-- Open File Dialog button -->
<Button x:Name=""OpenFileButton""
        Content=""Open File...""
        Width=""100"" Height=""28"" />

<Button x:Name=""OpenFolderPickerButton""
        Content=""Pick Folder...""
        Width=""118"" Height=""28"" />

<!-- Save File Dialog button -->
<Button x:Name=""SaveFileButton""
        Content=""Save As...""
        Width=""100"" Height=""28"" />

<!-- Folder Browser Dialog button -->
<Button x:Name=""BrowseFolderButton""
        Content=""Browse Folder...""
        Width=""120"" Height=""28"" />

<!-- Font Dialog button -->
<Button x:Name=""ChooseFontButton""
        Content=""Choose Font...""
        Width=""110"" Height=""28"" />

<Button x:Name=""PickIconButton""
        Content=""Pick Icon...""
        Width=""100"" Height=""28"" />
<Button x:Name=""OpenWithButton""
        Content=""Open With...""
        Width=""105"" Height=""28"" />
<Button x:Name=""FilePropertiesButton""
        Content=""File Properties...""
        Width=""126"" Height=""28"" />
<Button x:Name=""PrinterPropertiesButton""
        Content=""Printer Properties...""
        Width=""140"" Height=""28"" />
<Button x:Name=""VolumePropertiesButton""
        Content=""Volume Properties...""
        Width=""140"" Height=""28"" />
<Button x:Name=""MultiFilePropertiesButton""
        Content=""Multi-file Properties...""
        Width=""156"" Height=""28"" />

<!-- Message Dialog buttons -->
<Button x:Name=""InfoDialogButton""
        Content=""Show info""
        Width=""108"" Height=""32"" />
<Button x:Name=""WarningDialogButton""
        Content=""Retry flow""
        Width=""108"" Height=""32"" />
<Button x:Name=""ErrorDialogButton""
        Content=""Show error""
        Width=""108"" Height=""32"" />";

    private const string CSharpExample =
@"using Jalium.UI.Controls;

public partial class DialogsPage : Page
{
    private void OnOpenFileClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = ""Text files (*.txt)|*.txt|All files (*.*)|*.*""
        };

        if (dialog.ShowDialog() == true)
            SelectedFileText.Text = $""Selected: {dialog.FileName}"";
    }

    private void OnOpenFolderPickerClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            IsFolderPicker = true,
            InitialDirectory = AppContext.BaseDirectory
        };

        if (dialog.ShowDialog() == true)
            SelectedFolderViaOpenFileText.Text = $""Selected: {dialog.FileName}"";
    }

    private void OnPickIconClick(object? sender, EventArgs e)
    {
        var dialog = new PickIconDialog();
        if (dialog.ShowDialog())
            SelectedIconText.Text = $""Icon: {dialog.IconPath} (index {dialog.IconIndex})"";
    }

    private void OnFilePropertiesClick(object? sender, EventArgs e)
    {
        var dialog = new OpenPropertiesDialog
        {
            ObjectName = GetSampleDialogDocumentPaths(1).FirstOrDefault()
        };

        dialog.ShowDialog();
    }
}";

    private static readonly MessageDialogScenario s_infoScenario = new(
        "Information",
        "Sync complete",
        "Your theme preset was applied to the preview workspace.\n\nEverything is already up to date.",
        "A lightweight acknowledgement pattern for completed work that only needs an OK button.",
        MessageBoxButton.OK,
        MessageBoxImage.Information,
        MessageBoxResult.OK,
        Color.FromRgb(0x4C, 0x93, 0xFF));

    private static readonly MessageDialogScenario s_warningScenario = new(
        "Warning",
        "Unsaved changes",
        "The preview still has staged edits.\n\nRetry saving before you leave this page?",
        "A recovery-oriented warning that prefers Retry while still allowing the user to cancel safely.",
        MessageBoxButton.RetryCancel,
        MessageBoxImage.Warning,
        MessageBoxResult.Retry,
        Color.FromRgb(0xD6, 0xA4, 0x1F));

    private static readonly MessageDialogScenario s_errorScenario = new(
        "Error",
        "Import failed",
        "Jalium.UI.Gallery couldn't parse one of the selected assets.\n\nCheck the file path and try again.",
        "A blocking error with a system icon and a single acknowledgement action.",
        MessageBoxButton.OK,
        MessageBoxImage.Error,
        MessageBoxResult.OK,
        Color.FromRgb(0xE0, 0x64, 0x64));

    private static readonly MessageDialogScenario s_confirmScenario = new(
        "Confirm",
        "Replace accent palette?",
        "This action will update every preview card in the current sample.\n\nDo you want to continue with the replacement?",
        "A three-way confirmation flow where No is the safer default for destructive changes.",
        MessageBoxButton.YesNoCancel,
        MessageBoxImage.Question,
        MessageBoxResult.No,
        Color.FromRgb(0x58, 0xB7, 0x84));

    public DialogsPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        // Set up file dialog buttons
        if (OpenFileButton != null)
        {
            OpenFileButton.Click += OnOpenFileClick;
        }

        if (OpenMultipleFilesButton != null)
        {
            OpenMultipleFilesButton.Click += OnOpenMultipleFilesClick;
        }

        if (SaveFileButton != null)
        {
            SaveFileButton.Click += OnSaveFileClick;
        }

        if (OpenFolderPickerButton != null)
        {
            OpenFolderPickerButton.Click += OnOpenFolderPickerClick;
        }

        if (BrowseFolderButton != null)
        {
            BrowseFolderButton.Click += OnBrowseFolderClick;
        }

        // Set up font dialog button
        if (ChooseFontButton != null)
        {
            ChooseFontButton.Click += OnChooseFontClick;
        }

        if (PickIconButton != null)
        {
            PickIconButton.Click += OnPickIconClick;
        }

        if (OpenWithButton != null)
        {
            OpenWithButton.Click += OnOpenWithClick;
        }

        if (FilePropertiesButton != null)
        {
            FilePropertiesButton.Click += OnFilePropertiesClick;
        }

        if (PrinterPropertiesButton != null)
        {
            PrinterPropertiesButton.Click += OnPrinterPropertiesClick;
        }

        if (VolumePropertiesButton != null)
        {
            VolumePropertiesButton.Click += OnVolumePropertiesClick;
        }

        if (MultiFilePropertiesButton != null)
        {
            MultiFilePropertiesButton.Click += OnMultiFilePropertiesClick;
        }

        // Set up message dialog buttons
        if (InfoDialogButton != null)
        {
            InfoDialogButton.Click += (s, e) => ShowMessageDialog(s_infoScenario);
        }

        if (WarningDialogButton != null)
        {
            WarningDialogButton.Click += (s, e) => ShowMessageDialog(s_warningScenario);
        }

        if (ErrorDialogButton != null)
        {
            ErrorDialogButton.Click += (s, e) => ShowMessageDialog(s_errorScenario);
        }

        if (ConfirmDialogButton != null)
        {
            ConfirmDialogButton.Click += (s, e) => ShowMessageDialog(s_confirmScenario);
        }

        InitializeMessageDialogPreview();
    }

    private void OnOpenFileClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            if (SelectedFileText != null)
            {
                SelectedFileText.Text = $"Selected: {dialog.FileName}";
            }
        }
    }

    private void OnOpenFolderPickerClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            IsFolderPicker = true,
            InitialDirectory = AppContext.BaseDirectory
        };

        if (dialog.ShowDialog() == true && SelectedFolderViaOpenFileText != null)
        {
            SelectedFolderViaOpenFileText.Text = $"Selected: {dialog.FileName}";
        }
    }

    private void OnOpenMultipleFilesClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "All files (*.*)|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            if (SelectedFileText != null)
            {
                SelectedFileText.Text = $"Selected {dialog.FileNames?.Length ?? 0} file(s)";
            }
        }
    }

    private void OnSaveFileClick(object? sender, EventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            if (SaveFilePathText != null)
            {
                SaveFilePathText.Text = $"Save to: {dialog.FileName}";
            }
        }
    }

    private void OnBrowseFolderClick(object? sender, EventArgs e)
    {
        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == true)
        {
            if (SelectedFolderText != null)
            {
                SelectedFolderText.Text = $"Selected: {dialog.SelectedPath}";
            }
        }
    }

    private void OnChooseFontClick(object? sender, EventArgs e)
    {
        var dialog = new FontDialog();

        if (dialog.ShowDialog())
        {
            if (FontPreviewText != null && dialog.FontFamily != null)
            {
                FontPreviewText.FontFamily = dialog.FontFamily;
                FontPreviewText.FontSize = dialog.FontSize;
            }

            if (SelectedFontText != null && dialog.FontFamily != null)
            {
                SelectedFontText.Text = $"Current: {dialog.FontFamily.Source}, {dialog.FontSize}pt";
            }
        }
    }

    private void OnPickIconClick(object? sender, EventArgs e)
    {
        var dialog = new PickIconDialog();

        if (dialog.ShowDialog() && SelectedIconText != null)
        {
            SelectedIconText.Text = $"Icon: {dialog.IconPath} (index {dialog.IconIndex})";
        }
    }

    private void OnOpenWithClick(object? sender, EventArgs e)
    {
        var target = GetSampleDialogDocumentPaths(1).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(target))
        {
            if (OpenWithTargetText != null)
            {
                OpenWithTargetText.Text = "No sample document was available for Open With.";
            }

            return;
        }

        var dialog = new OpenWithDialog
        {
            FileName = target,
        };

        var result = dialog.ShowDialog();
        if (OpenWithTargetText != null)
        {
            OpenWithTargetText.Text = result
                ? $"Opened selector for: {target}"
                : $"Cancelled for: {target}";
        }
    }

    private void OnFilePropertiesClick(object? sender, EventArgs e)
    {
        var target = GetSampleDialogDocumentPaths(1).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(target))
        {
            if (PropertiesTargetText != null)
            {
                PropertiesTargetText.Text = "No sample document was available for the properties dialog.";
            }

            return;
        }

        var dialog = new OpenPropertiesDialog
        {
            ObjectName = target,
            PropertyPage = "General"
        };

        var result = dialog.ShowDialog();
        if (PropertiesTargetText != null)
        {
            PropertiesTargetText.Text = result
                ? $"Opened properties for: {target}"
                : $"Cancelled for: {target}";
        }
    }

    private void OnMultiFilePropertiesClick(object? sender, EventArgs e)
    {
        var targets = GetSampleDialogDocumentPaths(2);
        if (targets.Length < 2)
        {
            if (PropertiesTargetText != null)
            {
                PropertiesTargetText.Text = "At least two sample documents are required for the multi-file properties sample.";
            }

            return;
        }

        var dialog = new OpenPropertiesDialog
        {
            ObjectNames = targets
        };

        var result = dialog.ShowDialog();
        if (PropertiesTargetText != null)
        {
            PropertiesTargetText.Text = result
                ? $"Opened multi-file properties for: {string.Join(", ", targets.Select(Path.GetFileName))}"
                : $"Cancelled for: {string.Join(", ", targets.Select(Path.GetFileName))}";
        }
    }

    private void OnPrinterPropertiesClick(object? sender, EventArgs e)
    {
        var printerName = GetDefaultPrinterName();
        if (string.IsNullOrWhiteSpace(printerName))
        {
            if (PropertiesTargetText != null)
            {
                PropertiesTargetText.Text = "No default printer was available for the printer properties sample.";
            }

            return;
        }

        var dialog = new OpenPropertiesDialog
        {
            ObjectType = OpenPropertiesObjectType.PrinterName,
            ObjectName = printerName
        };

        var result = dialog.ShowDialog();
        if (PropertiesTargetText != null)
        {
            PropertiesTargetText.Text = result
                ? $"Opened printer properties for: {printerName}"
                : $"Cancelled for printer: {printerName}";
        }
    }

    private void OnVolumePropertiesClick(object? sender, EventArgs e)
    {
        var volumePath = GetSampleVolumePath();
        if (string.IsNullOrWhiteSpace(volumePath))
        {
            if (PropertiesTargetText != null)
            {
                PropertiesTargetText.Text = "No volume path was available for the volume properties sample.";
            }

            return;
        }

        var dialog = new OpenPropertiesDialog
        {
            ObjectType = OpenPropertiesObjectType.VolumeGuid,
            ObjectName = volumePath
        };

        var result = dialog.ShowDialog();

        Window.GetWindow(this)?.Activate();

        if (PropertiesTargetText != null)
        {
            PropertiesTargetText.Text = result
                ? $"Opened volume properties for: {volumePath}"
                : $"Cancelled for volume: {volumePath}";
        }
    }

    private void ShowMessageDialog(MessageDialogScenario scenario)
    {
        var owner = FindOwnerWindow();
        var result = owner != null
            ? MessageBox.Show(owner, scenario.Message, scenario.Caption, scenario.Buttons, scenario.Icon, scenario.DefaultResult)
            : MessageBox.Show(scenario.Message, scenario.Caption, scenario.Buttons, scenario.Icon, scenario.DefaultResult);

        UpdateMessageDialogPreview(scenario, result);
    }

    private void InitializeMessageDialogPreview()
    {
        ApplyMessageDialogPreview(
            caption: "Native message boxes ready",
            description: "Run one of the four presets to inspect owner-aware message boxes with different icons, button layouts, and default answers.",
            message: "Message: The sample records the exact prompt after the native dialog closes.",
            details: "Buttons: varies by sample  |  Icon: system-provided  |  Default: configured per scenario",
            result: "Result: -",
            accentColor: s_infoScenario.AccentColor);
    }

    private void UpdateMessageDialogPreview(MessageDialogScenario scenario, MessageBoxResult result)
    {
        var outcome = GetOutcomeSummary(scenario, result);
        ApplyMessageDialogPreview(
            caption: scenario.Caption,
            description: $"{scenario.Description} {outcome}",
            message: $"Message: {scenario.Message.Replace(Environment.NewLine + Environment.NewLine, " ").Replace('\n', ' ')}",
            details: $"Buttons: {GetButtonsSummary(scenario.Buttons)}  |  Icon: {GetIconSummary(scenario.Icon)}  |  Default: {scenario.DefaultResult}",
            result: $"Result: {result}",
            accentColor: scenario.AccentColor);
    }

    private void ApplyMessageDialogPreview(
        string caption,
        string description,
        string message,
        string details,
        string result,
        Color accentColor)
    {
        var accentBrush = new SolidColorBrush(accentColor);

        if (DialogResultAccent != null)
        {
            DialogResultAccent.Background = accentBrush;
        }

        if (DialogResultTitleText != null)
        {
            DialogResultTitleText.Text = caption;
        }

        if (DialogScenarioText != null)
        {
            DialogScenarioText.Text = description;
        }

        if (DialogMessageText != null)
        {
            DialogMessageText.Text = message;
        }

        if (DialogDetailsText != null)
        {
            DialogDetailsText.Text = details;
        }

        if (DialogResultText != null)
        {
            DialogResultText.Text = result;
            DialogResultText.Foreground = accentBrush;
        }
    }

    private Window? FindOwnerWindow()
    {
        UIElement? current = this;
        while (current != null)
        {
            if (current is Window window)
            {
                return window;
            }

            current = current.VisualParent as UIElement;
        }

        return Jalium.UI.Application.Current?.MainWindow;
    }

    private static string GetButtonsSummary(MessageBoxButton buttons)
    {
        return buttons switch
        {
            MessageBoxButton.OK => "OK",
            MessageBoxButton.OKCancel => "OK / Cancel",
            MessageBoxButton.AbortRetryIgnore => "Abort / Retry / Ignore",
            MessageBoxButton.YesNoCancel => "Yes / No / Cancel",
            MessageBoxButton.YesNo => "Yes / No",
            MessageBoxButton.RetryCancel => "Retry / Cancel",
            MessageBoxButton.CancelTryContinue => "Cancel / Try Again / Continue",
            _ => buttons.ToString()
        };
    }

    private static string GetIconSummary(MessageBoxImage icon)
    {
        return icon switch
        {
            MessageBoxImage.None => "None",
            MessageBoxImage.Information => "Information",
            MessageBoxImage.Warning => "Warning",
            MessageBoxImage.Error => "Error",
            MessageBoxImage.Question => "Question",
            _ => icon.ToString()
        };
    }

    private static string GetOutcomeSummary(MessageDialogScenario scenario, MessageBoxResult result)
    {
        return scenario.Kind switch
        {
            "Information" => "The user acknowledged the success message and returned to the sample.",
            "Warning" => result == MessageBoxResult.Retry
                ? "The user chose to retry the save flow before leaving the current page."
                : "The user canceled the retry flow and kept the page unchanged.",
            "Error" => "The user dismissed the blocking error after reviewing the import failure details.",
            "Confirm" => result switch
            {
                MessageBoxResult.Yes => "The user approved replacing the current accent palette.",
                MessageBoxResult.No => "The user kept the existing palette and declined the replacement.",
                _ => "The user backed out without committing to the replacement."
            },
            _ => $"The dialog closed with {result}."
        };
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

    private static string[] GetSampleDialogDocumentPaths(int count)
    {
        var directory = Path.Combine(Path.GetTempPath(), "Jalium.UI.Gallery", "Dialogs");
        Directory.CreateDirectory(directory);

        var paths = new List<string>(count);
        for (var index = 1; index <= count; index++)
        {
            var path = Path.Combine(directory, $"sample-document-{index}.txt");
            if (!File.Exists(path))
            {
                File.WriteAllText(
                    path,
                    $"Jalium.UI dialog sample document {index}{Environment.NewLine}{Environment.NewLine}Created for native shell dialog demos.");
            }

            paths.Add(path);
        }

        return paths.ToArray();
    }

    private static string? GetDefaultPrinterName()
    {
        var requiredLength = 0;
        _ = GetDefaultPrinter(null, ref requiredLength);
        if (requiredLength <= 0)
        {
            return null;
        }

        var buffer = new StringBuilder(requiredLength);
        return GetDefaultPrinter(buffer, ref requiredLength) ? buffer.ToString() : null;
    }

    private static string? GetSampleVolumePath()
    {
        return Path.GetPathRoot(Environment.SystemDirectory);
    }

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetDefaultPrinter(StringBuilder? pszBuffer, ref int pcchBuffer);

    private sealed record MessageDialogScenario(
        string Kind,
        string Caption,
        string Message,
        string Description,
        MessageBoxButton Buttons,
        MessageBoxImage Icon,
        MessageBoxResult DefaultResult,
        Color AccentColor);
}
