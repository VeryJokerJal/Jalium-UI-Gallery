namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface IWebViewStateSink
{
    void NavigationStarting(Uri uri);

    void NavigationCompleted(
        bool isSuccess,
        int httpStatusCode,
        bool canGoBack,
        bool canGoForward);

    void InitializationCompleted(bool succeeded, string? error);

    void SetDocumentTitle(string title);

    void SetScriptResult(string result, bool isError);
}
