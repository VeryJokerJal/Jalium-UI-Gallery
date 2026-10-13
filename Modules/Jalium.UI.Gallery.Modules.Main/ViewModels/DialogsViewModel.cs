using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class DialogsViewModel
{
    private readonly IGallerySystemDialogService _dialogs;
    private string _selectedFile = "No file selected";
    private string _selectedPickerFolder =
        "No folder selected via OpenFolderDialog";
    private string _savePath = "No save location selected";
    private string _selectedFolder = "No folder selected";
    private string _selectedIcon = "No icon selected";
    private string _openWithTarget = "Open With target not launched";
    private string _propertiesTarget = "Properties dialog not launched";
    private string _selectedFont = "Current: Segoe UI, 14pt";
    private string _messageCaption = "Native message boxes ready";
    private string _messageDescription =
        "Run one of the four presets to inspect owner-aware message boxes.";
    private string _messageText =
        "Message: The sample records the exact prompt after the dialog closes.";
    private string _messageDetails =
        "Buttons: varies by sample  |  Icon: system-provided";
    private string _messageResult = "Result: -";

    public DialogsViewModel(IGallerySystemDialogService dialogs)
    {
        _dialogs = dialogs;
        ShowDialogCommand = new RelayCommand<string>(Execute);
    }

    public ICommand ShowDialogCommand { get; }

    public string SelectedFile
    {
        get => _selectedFile;
        private set => SetProperty(ref _selectedFile, value);
    }

    public string SelectedPickerFolder
    {
        get => _selectedPickerFolder;
        private set => SetProperty(ref _selectedPickerFolder, value);
    }

    public string SavePath
    {
        get => _savePath;
        private set => SetProperty(ref _savePath, value);
    }

    public string SelectedFolder
    {
        get => _selectedFolder;
        private set => SetProperty(ref _selectedFolder, value);
    }

    public string SelectedIcon
    {
        get => _selectedIcon;
        private set => SetProperty(ref _selectedIcon, value);
    }

    public string OpenWithTarget
    {
        get => _openWithTarget;
        private set => SetProperty(ref _openWithTarget, value);
    }

    public string PropertiesTarget
    {
        get => _propertiesTarget;
        private set => SetProperty(ref _propertiesTarget, value);
    }

    public string SelectedFont
    {
        get => _selectedFont;
        private set => SetProperty(ref _selectedFont, value);
    }

    public string MessageCaption
    {
        get => _messageCaption;
        private set => SetProperty(ref _messageCaption, value);
    }

    public string MessageDescription
    {
        get => _messageDescription;
        private set => SetProperty(ref _messageDescription, value);
    }

    public string MessageText
    {
        get => _messageText;
        private set => SetProperty(ref _messageText, value);
    }

    public string MessageDetails
    {
        get => _messageDetails;
        private set => SetProperty(ref _messageDetails, value);
    }

    public string MessageResult
    {
        get => _messageResult;
        private set => SetProperty(ref _messageResult, value);
    }

    private void Execute(string? action)
    {
        switch (action)
        {
            case "OpenFile":
                if (_dialogs.OpenFile() is { } file)
                {
                    SelectedFile = $"Selected: {file}";
                }

                break;
            case "OpenMultiple":
                if (_dialogs.OpenMultipleFiles() is { } count)
                {
                    SelectedFile = $"Selected {count} file(s)";
                }

                break;
            case "PickFolder":
                if (_dialogs.PickFolder(true) is { } pickerFolder)
                {
                    SelectedPickerFolder =
                        $"Selected: {pickerFolder}";
                }

                break;
            case "SaveFile":
                if (_dialogs.SaveFile() is { } savePath)
                {
                    SavePath = $"Save to: {savePath}";
                }

                break;
            case "BrowseFolder":
                if (_dialogs.PickFolder(false) is { } folder)
                {
                    SelectedFolder = $"Selected: {folder}";
                }

                break;
            case "ChooseFont":
                if (_dialogs.ChooseFont() is { } font)
                {
                    SelectedFont =
                        $"Current: {font.FamilyName}, {font.Size:F0}pt";
                }

                break;
            case "PickIcon":
                if (_dialogs.PickIcon() is { } icon)
                {
                    SelectedIcon = icon;
                }

                break;
            case "OpenWith":
                OpenWithTarget = _dialogs.ShowOpenWith();
                break;
            case "FileProperties":
                PropertiesTarget =
                    _dialogs.ShowFileProperties(false);
                break;
            case "MultiFileProperties":
                PropertiesTarget =
                    _dialogs.ShowFileProperties(true);
                break;
            case "PrinterProperties":
                PropertiesTarget =
                    _dialogs.ShowPrinterProperties();
                break;
            case "VolumeProperties":
                PropertiesTarget =
                    _dialogs.ShowVolumeProperties();
                break;
            case "Information":
                Apply(_dialogs.ShowMessage(
                    MessageDialogKind.Information));
                break;
            case "Warning":
                Apply(_dialogs.ShowMessage(
                    MessageDialogKind.Warning));
                break;
            case "Error":
                Apply(_dialogs.ShowMessage(
                    MessageDialogKind.Error));
                break;
            case "Confirm":
                Apply(_dialogs.ShowMessage(
                    MessageDialogKind.Confirm));
                break;
        }
    }

    private void Apply(MessageDialogPreview preview)
    {
        MessageCaption = preview.Caption;
        MessageDescription = preview.Description;
        MessageText = preview.Message;
        MessageDetails = preview.Details;
        MessageResult = preview.Result;
    }
}
