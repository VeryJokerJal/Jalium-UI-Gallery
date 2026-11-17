namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum MessageDialogKind
{
    Information,
    Warning,
    Error,
    Confirm
}

public sealed record FontDialogSelection(
    string FamilyName,
    double Size);

public sealed record MessageDialogPreview(
    string Caption,
    string Description,
    string Message,
    string Details,
    string Result);
