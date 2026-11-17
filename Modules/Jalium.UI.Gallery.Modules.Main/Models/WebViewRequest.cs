namespace Jalium.UI.Gallery.Modules.Main.Models;

public enum WebViewAction
{
    Navigate,
    Back,
    Forward,
    Refresh,
    ExecuteScript
}

public sealed record WebViewRequest(
    long Sequence,
    WebViewAction Action,
    string Value = "");
