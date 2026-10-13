using System.Collections.ObjectModel;
using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class DragDropViewModel
    : IDragDropStateSink
{
    private const int MaximumLogLines = 50;
    private const int RetainedLogLines = 30;

    private readonly IGalleryDragFileService _dragFileService;
    private readonly Queue<string> _eventLines = new();
    private string _dropTargetHint = "Drop items here";
    private string _customImageDropHint = "Drop here";
    private string _shellDropHint =
        "Drag files here from Explorer";
    private string _dragOutStatus =
        "A temp file is created on drag; drop it into a folder to copy it out.";
    private string _eventLog = "(drag items to see events)";

    public DragDropViewModel(
        IGalleryDragFileService dragFileService)
    {
        _dragFileService = dragFileService;
        ClearLogCommand = new RelayCommand(ClearLog);
    }

    public ObservableCollection<string> DroppedItems { get; } = [];

    public ObservableCollection<string> ShellDroppedFiles { get; } = [];

    public string DropTargetHint
    {
        get => _dropTargetHint;
        private set => SetProperty(ref _dropTargetHint, value);
    }

    public string CustomImageDropHint
    {
        get => _customImageDropHint;
        private set => SetProperty(ref _customImageDropHint, value);
    }

    public string ShellDropHint
    {
        get => _shellDropHint;
        private set => SetProperty(ref _shellDropHint, value);
    }

    public string DragOutStatus
    {
        get => _dragOutStatus;
        private set => SetProperty(ref _dragOutStatus, value);
    }

    public string EventLog
    {
        get => _eventLog;
        private set => SetProperty(ref _eventLog, value);
    }

    public ICommand ClearLogCommand { get; }

    public void OnDragEntered(
        DragDropTargetRole role,
        string targetName)
    {
        if (role == DragDropTargetRole.ItemList)
        {
            DropTargetHint = "Release to drop";
        }

        AddEvent("DragEnter", targetName);
    }

    public void OnDragLeft(
        DragDropTargetRole role,
        string targetName)
    {
        if (role == DragDropTargetRole.ItemList)
        {
            DropTargetHint = "Drop items here";
        }

        AddEvent("DragLeave", targetName);
    }

    public void OnTextDropped(
        DragDropTargetRole role,
        string targetName,
        string text)
    {
        switch (role)
        {
            case DragDropTargetRole.ItemList:
                DroppedItems.Add(text);
                DropTargetHint = "Drop items here";
                break;
            case DragDropTargetRole.CustomImage:
                CustomImageDropHint = $"Dropped: {text}";
                break;
        }

        AddEvent("Drop", $"{targetName} (received: {text})");
    }

    public void OnFilesDropped(
        string targetName,
        IReadOnlyList<string> paths)
    {
        ShellDroppedFiles.Clear();
        foreach (var path in paths)
        {
            ShellDroppedFiles.Add(Path.GetFileName(path));
        }

        ShellDropHint = paths.Count > 0
            ? $"Dropped {paths.Count} file(s):"
            : "Drag files here from Explorer";
        AddEvent(
            "Drop",
            $"{targetName} (received {paths.Count} file(s))");
    }

    public string? CreateDragOutFile()
    {
        var path = _dragFileService.CreateDemoFile();
        if (path is null)
        {
            DragOutStatus = "Could not create the temp file.";
        }

        return path;
    }

    public void OnDragOutCompleted(string effect)
    {
        DragOutStatus = effect == "None"
            ? "Drag cancelled (dropped nowhere)."
            : $"Dropped out with effect: {effect}.";
        AddEvent("DoShellDragDrop", $"DragOutSource → {effect}");
    }

    private void ClearLog()
    {
        _eventLines.Clear();
        EventLog = "(drag items to see events)";
    }

    private void AddEvent(string eventName, string target)
    {
        _eventLines.Enqueue($"[{eventName}] {target}");
        if (_eventLines.Count > MaximumLogLines)
        {
            while (_eventLines.Count > RetainedLogLines)
            {
                _eventLines.Dequeue();
            }
        }

        EventLog = string.Join(Environment.NewLine, _eventLines);
    }
}
