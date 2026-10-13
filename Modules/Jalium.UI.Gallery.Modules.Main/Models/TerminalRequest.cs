namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum TerminalAction
{
    Clear,
    Copy,
    Paste,
    Restart,
    Write,
    ClearAndWrite
}

public sealed record TerminalRequest(
    long Sequence,
    TerminalAction Action,
    string Output = "");
