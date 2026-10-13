using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class WebViewViewModel : IWebViewStateSink
{
    private long _requestSequence;
    private string _address = "https://www.bing.com";
    private string _script = "document.title";
    private string _documentTitle = "Title: (not loaded)";
    private string _scriptResult = "Result:";
    private bool _canGoBack;
    private bool _canGoForward;
    private WebViewRequest? _request;

    public WebViewViewModel()
    {
        Status = "Status: Initializing WebView...";
        NavigateCommand = new RelayCommand(() =>
            Request(WebViewAction.Navigate, Address));
        BackCommand = new RelayCommand(
            () => Request(WebViewAction.Back),
            () => CanGoBack);
        ForwardCommand = new RelayCommand(
            () => Request(WebViewAction.Forward),
            () => CanGoForward);
        RefreshCommand = new RelayCommand(() =>
            Request(WebViewAction.Refresh));
        ExecuteScriptCommand = new RelayCommand(() =>
            Request(WebViewAction.ExecuteScript, Script));
    }

    public Uri InitialSource { get; } =
        new("https://www.bing.com");

    public ICommand NavigateCommand { get; }

    public ICommand BackCommand { get; }

    public ICommand ForwardCommand { get; }

    public ICommand RefreshCommand { get; }

    public ICommand ExecuteScriptCommand { get; }

    public WebViewRequest? RequestValue
    {
        get => _request;
        private set => SetProperty(ref _request, value);
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public string Script
    {
        get => _script;
        set => SetProperty(ref _script, value);
    }

    public string DocumentTitle
    {
        get => _documentTitle;
        private set => SetProperty(ref _documentTitle, value);
    }

    public string ScriptResult
    {
        get => _scriptResult;
        private set => SetProperty(ref _scriptResult, value);
    }

    public bool CanGoBack
    {
        get => _canGoBack;
        private set
        {
            if (SetProperty(ref _canGoBack, value))
            {
                (BackCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public bool CanGoForward
    {
        get => _canGoForward;
        private set
        {
            if (SetProperty(ref _canGoForward, value))
            {
                (ForwardCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public void NavigationStarting(Uri uri)
    {
        Address = uri.AbsoluteUri;
        Status = "Status: Loading...";
    }

    public void NavigationCompleted(
        bool isSuccess,
        int httpStatusCode,
        bool canGoBack,
        bool canGoForward)
    {
        Status = isSuccess
            ? $"Status: Loaded (HTTP {httpStatusCode})"
            : $"Status: Navigation failed (HTTP {httpStatusCode})";
        CanGoBack = canGoBack;
        CanGoForward = canGoForward;
    }

    public void InitializationCompleted(bool succeeded, string? error) =>
        Status = succeeded
            ? "Status: WebView initialized."
            : $"Status: WebView initialization failed - " +
              $"{(string.IsNullOrWhiteSpace(error) ? "Unknown initialization error." : error)}";

    public void SetDocumentTitle(string title) =>
        DocumentTitle = $"Title: {title}";

    public void SetScriptResult(string result, bool isError) =>
        ScriptResult = $"{(isError ? "Error" : "Result")}: {result}";

    private void Request(WebViewAction action, string value = "") =>
        RequestValue = new WebViewRequest(
            ++_requestSequence,
            action,
            value);
}
