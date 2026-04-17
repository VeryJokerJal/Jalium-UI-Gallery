using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Views;

public partial class WebViewPage : Page
{
    private WebView? _webView;

    private const string XamlExample = """
        <!-- WebView with address bar -->
        <StackPanel Orientation="Horizontal">
            <Button x:Name="BackButton" Content="&lt;" Width="32"/>
            <Button x:Name="ForwardButton" Content="&gt;" Width="32"/>
            <Button x:Name="RefreshButton" Content="R" Width="32"/>
            <TextBox x:Name="AddressBar" Width="600"/>
            <Button x:Name="GoButton" Content="Go" Width="48"/>
        </StackPanel>

        <!-- WebView container -->
        <Border x:Name="WebViewContainer"
                Height="500"
                Background="#0D0D0D"
                CornerRadius="4"
                ClipToBounds="True"/>

        <!-- JavaScript execution -->
        <TextBox x:Name="ScriptInput" Height="32"/>
        <Button x:Name="ExecuteScriptButton" Content="Execute"/>
        <TextBlock x:Name="ScriptResultText" Text="Result: "/>
        """;

    private const string CSharpExample = """
        using Jalium.UI.Controls;

        // Create and configure WebView
        var webView = new WebView
        {
            Source = new Uri("https://www.bing.com")
        };
        container.Child = webView;

        // Initialize WebView2
        await webView.EnsureCoreWebView2Async();

        // Navigation
        webView.Navigate("https://example.com");
        webView.GoBack();
        webView.GoForward();
        webView.Refresh();

        // Handle events
        webView.NavigationStarting += (s, e) =>
        {
            addressBar.Text = e.Uri.AbsoluteUri;
        };
        webView.NavigationCompleted += (s, e) =>
        {
            status.Text = e.IsSuccess
                ? $"Loaded (HTTP {e.HttpStatusCode})"
                : "Navigation failed";
        };
        webView.DocumentTitleChanged += (s, e) =>
        {
            titleText.Text = e.Title;
        };

        // Execute JavaScript
        var result = await webView.ExecuteScriptAsync("document.title");

        // Cleanup
        webView.Dispose();
        """;

    public WebViewPage()
    {
        InitializeComponent();
        SetupWebView();
        SetupEventHandlers();
        LoadCodeExamples();
        Unloaded += OnPageUnloaded;
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }

    private void SetupWebView()
    {
        if (WebViewContainer == null) return;

        _webView = new WebView
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            // Set Source so WebView2 navigates automatically after initialization
            Source = new Uri("https://www.bing.com")
        };

        WebViewContainer.Child = _webView;

        if (AddressBar != null)
            AddressBar.Text = "https://www.bing.com";

        // Wire up WebView events
        _webView.NavigationStarting += OnWebViewNavigationStarting;
        _webView.NavigationCompleted += OnWebViewNavigationCompleted;
        _webView.DocumentTitleChanged += OnWebViewDocumentTitleChanged;
        _webView.CoreWebView2InitializationCompleted += OnWebViewInitializationCompleted;

        SetStatus("Status: Initializing WebView...");
        _ = _webView.EnsureCoreWebView2Async();
    }

    private void SetupEventHandlers()
    {
        if (BackButton != null)
            BackButton.Click += (s, e) => _webView?.GoBack();

        if (ForwardButton != null)
            ForwardButton.Click += (s, e) => _webView?.GoForward();

        if (RefreshButton != null)
            RefreshButton.Click += (s, e) => _webView?.Refresh();

        if (GoButton != null)
            GoButton.Click += (s, e) => NavigateToAddressBar();

        if (AddressBar != null)
        {
            AddressBar.KeyDown += (s, e) =>
            {
                if (e is KeyEventArgs ke && ke.Key == Key.Enter)
                    NavigateToAddressBar();
            };
        }

        if (ExecuteScriptButton != null)
            ExecuteScriptButton.Click += OnExecuteScriptClick;

        if (ScriptInput != null)
            ScriptInput.Text = "document.title";
    }

    private void NavigateToAddressBar()
    {
        if (_webView == null || AddressBar == null) return;

        var url = AddressBar.Text?.Trim();
        if (string.IsNullOrEmpty(url)) return;

        // Add https:// if no protocol specified
        if (!url.Contains("://"))
            url = "https://" + url;

        _webView.Navigate(url);
    }

    private void OnWebViewNavigationStarting(object? sender, WebViewNavigationStartingEventArgs e)
    {
        SetStatus("Status: Loading...");
        if (AddressBar != null)
            AddressBar.Text = e.Uri.AbsoluteUri;
    }

    private void OnWebViewNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        SetStatus(e.IsSuccess
            ? $"Status: Loaded (HTTP {e.HttpStatusCode})"
            : $"Status: Navigation failed (HTTP {e.HttpStatusCode})");
    }

    private void OnWebViewDocumentTitleChanged(object? sender, WebViewDocumentTitleChangedEventArgs e)
    {
        if (TitleText != null)
            TitleText.Text = $"Title: {e.Title}";
    }

    private void OnWebViewInitializationCompleted(object? sender, EventArgs e)
    {
        if (_webView == null) return;

        if (_webView.IsWebViewInitialized)
        {
            SetStatus("Status: WebView initialized.");
            return;
        }

        var error = string.IsNullOrWhiteSpace(_webView.InitializationError)
            ? "Unknown initialization error."
            : _webView.InitializationError;
        SetStatus($"Status: WebView initialization failed - {error}");
    }

    private async void OnExecuteScriptClick(object? sender, RoutedEventArgs e)
    {
        if (_webView == null || ScriptInput == null || ScriptResultText == null) return;

        var script = ScriptInput.Text?.Trim();
        if (string.IsNullOrEmpty(script)) return;

        try
        {
            var result = await _webView.ExecuteScriptAsync(script);
            ScriptResultText.Text = $"Result: {result}";
        }
        catch (Exception ex)
        {
            ScriptResultText.Text = $"Error: {ex.Message}";
        }
    }

    private void SetStatus(string text)
    {
        if (StatusText != null)
            StatusText.Text = text;
        if (InlineStatusText != null)
            InlineStatusText.Text = text;
    }

    private void OnPageUnloaded(object? sender, RoutedEventArgs e)
    {
        if (_webView == null)
            return;

        _webView.NavigationStarting -= OnWebViewNavigationStarting;
        _webView.NavigationCompleted -= OnWebViewNavigationCompleted;
        _webView.DocumentTitleChanged -= OnWebViewDocumentTitleChanged;
        _webView.CoreWebView2InitializationCompleted -= OnWebViewInitializationCompleted;
        _webView.Dispose();

        if (WebViewContainer != null && ReferenceEquals(WebViewContainer.Child, _webView))
            WebViewContainer.Child = null;

        _webView = null;
    }
}
