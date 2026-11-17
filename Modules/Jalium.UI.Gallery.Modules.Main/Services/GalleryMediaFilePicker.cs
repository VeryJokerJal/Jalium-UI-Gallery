using Microsoft.Win32;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public sealed class GalleryMediaFilePicker : IGalleryMediaFilePicker
{
    public string? PickVideo() =>
        Pick(
            "Select Video File",
            "Video Files|*.mp4;*.avi;*.wmv;*.mov;*.webm;*.mkv|All Files|*.*");

    public string? PickAudio() =>
        Pick(
            "Select Audio File",
            "Audio Files|*.mp3;*.wav;*.aac;*.ogg;*.flac;*.wma|All Files|*.*");

    private static string? Pick(string title, string filter)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            Filter = filter
        };
        return dialog.ShowDialog() == true
            ? dialog.FileName
            : null;
    }
}
