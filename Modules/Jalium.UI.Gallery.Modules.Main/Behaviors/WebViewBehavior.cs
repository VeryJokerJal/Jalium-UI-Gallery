using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class WebViewBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(WebViewBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(WebViewRequest),
            typeof(WebViewBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    public static void SetIsEnabled(DependencyObject element, bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static WebViewRequest? GetRequest(DependencyObject element) =>
        element.GetValue(RequestProperty) as WebViewRequest;

    public static void SetRequest(
        DependencyObject element,
        WebViewRequest? value) =>
        element.SetValue(RequestProperty, value);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not WebView webView)
        {
            return;
        }

        Detach(webView);
        if (args.NewValue is true)
        {
            webView.NavigationStarting += OnNavigationStarting;
            webView.NavigationCompleted += OnNavigationCompleted;
            webView.DocumentTitleChanged += OnDocumentTitleChanged;
            webView.CoreWebView2InitializationCompleted +=
                OnInitializationCompleted;
            webView.Loaded += OnLoaded;
            webView.Unloaded += OnUnloaded;
        }
    }

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is WebView webView &&
            args.NewValue is WebViewRequest request)
        {
            _ = ExecuteRequestAsync(webView, request);
        }
    }

    private static async Task ExecuteRequestAsync(
        WebView webView,
        WebViewRequest request)
    {
        try
        {
            switch (request.Action)
            {
                case WebViewAction.Navigate:
                    if (!string.IsNullOrWhiteSpace(request.Value))
                    {
                        webView.Navigate(NormalizeUrl(request.Value));
                    }

                    break;
                case WebViewAction.Back:
                    webView.GoBack();
                    break;
                case WebViewAction.Forward:
                    webView.GoForward();
                    break;
                case WebViewAction.Refresh:
                    webView.Refresh();
                    break;
                case WebViewAction.ExecuteScript:
                    if (!string.IsNullOrWhiteSpace(request.Value))
                    {
                        var result = await webView.ExecuteScriptAsync(
                            request.Value);
                        Sink(webView)?.SetScriptResult(result, false);
                    }

                    break;
            }
        }
        catch (Exception exception)
        {
            Sink(webView)?.SetScriptResult(exception.Message, true);
        }
    }

    private static async void OnLoaded(object? sender, RoutedEventArgs args)
    {
        if (sender is WebView webView)
        {
            try
            {
                await webView.EnsureCoreWebView2Async();
            }
            catch (Exception exception)
            {
                Sink(webView)?.InitializationCompleted(
                    false,
                    exception.Message);
            }
        }
    }

    private static void OnUnloaded(object? sender, RoutedEventArgs args)
    {
        if (sender is not WebView webView)
        {
            return;
        }

        Detach(webView);
        webView.Dispose();
    }

    private static void OnNavigationStarting(
        object? sender,
        WebViewNavigationStartingEventArgs args)
    {
        if (sender is WebView webView)
        {
            Sink(webView)?.NavigationStarting(args.Uri);
        }
    }

    private static void OnNavigationCompleted(
        object? sender,
        WebViewNavigationCompletedEventArgs args)
    {
        if (sender is WebView webView)
        {
            Sink(webView)?.NavigationCompleted(
                args.IsSuccess,
                args.HttpStatusCode,
                webView.CanGoBack,
                webView.CanGoForward);
        }
    }

    private static void OnDocumentTitleChanged(
        object? sender,
        WebViewDocumentTitleChangedEventArgs args)
    {
        if (sender is WebView webView)
        {
            Sink(webView)?.SetDocumentTitle(args.Title);
        }
    }

    private static void OnInitializationCompleted(
        object? sender,
        EventArgs args)
    {
        if (sender is WebView webView)
        {
            Sink(webView)?.InitializationCompleted(
                webView.IsWebViewInitialized,
                webView.InitializationError);
        }
    }

    private static void Detach(WebView webView)
    {
        webView.NavigationStarting -= OnNavigationStarting;
        webView.NavigationCompleted -= OnNavigationCompleted;
        webView.DocumentTitleChanged -= OnDocumentTitleChanged;
        webView.CoreWebView2InitializationCompleted -=
            OnInitializationCompleted;
        webView.Loaded -= OnLoaded;
        webView.Unloaded -= OnUnloaded;
    }

    private static string NormalizeUrl(string value)
    {
        var url = value.Trim();
        return url.Contains("://", StringComparison.Ordinal)
            ? url
            : $"https://{url}";
    }

    private static IWebViewStateSink? Sink(WebView webView) =>
        webView.DataContext as IWebViewStateSink;
}
