using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for DialogsPage.jalxaml demonstrating dialog functionality.
/// </summary>
public partial class DialogsPage : Page
{
    public DialogsPage()
    {
        InitializeComponent();

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

        if (BrowseFolderButton != null)
        {
            BrowseFolderButton.Click += OnBrowseFolderClick;
        }

        // Set up font dialog button
        if (ChooseFontButton != null)
        {
            ChooseFontButton.Click += OnChooseFontClick;
        }

        // Set up message dialog buttons
        if (InfoDialogButton != null)
        {
            InfoDialogButton.Click += (s, e) => ShowMessageDialog("Information", "This is an informational message.", "info");
        }

        if (WarningDialogButton != null)
        {
            WarningDialogButton.Click += (s, e) => ShowMessageDialog("Warning", "This is a warning message.", "warning");
        }

        if (ErrorDialogButton != null)
        {
            ErrorDialogButton.Click += (s, e) => ShowMessageDialog("Error", "This is an error message.", "error");
        }

        if (ConfirmDialogButton != null)
        {
            ConfirmDialogButton.Click += (s, e) => ShowConfirmDialog();
        }
    }

    private void OnOpenFileClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open File",
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

    private void OnOpenMultipleFilesClick(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open Files",
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
            Title = "Save File",
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
        var dialog = new FolderBrowserDialog
        {
            Title = "Select Folder"
        };

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

    private void ShowMessageDialog(string title, string message, string type)
    {
        // In a real implementation, this would show a message box
        if (DialogResultText != null)
        {
            DialogResultText.Text = $"{type.ToUpper()}: {message}";
        }
    }

    private void ShowConfirmDialog()
    {
        // In a real implementation, this would show a confirmation dialog
        if (DialogResultText != null)
        {
            DialogResultText.Text = "Confirmation dialog would appear here";
        }
    }
}
