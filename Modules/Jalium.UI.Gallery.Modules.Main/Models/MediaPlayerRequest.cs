namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum MediaPlayerAction
{
    Play,
    Pause,
    Stop
}

public sealed record MediaPlayerRequest(
    long Sequence,
    MediaPlayerAction Action);
