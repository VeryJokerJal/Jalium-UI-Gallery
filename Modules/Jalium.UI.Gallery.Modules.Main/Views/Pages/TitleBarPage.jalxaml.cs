using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for TitleBarPage.jalxaml demonstrating TitleBar functionality.
/// </summary>
public partial class TitleBarPage : Page
{
    public TitleBarPage()
    {
        InitializeComponent();
        HookEvents();
        ApplyDemoState();
        LoadCodeExamples();
    }

    private void HookEvents()
    {
        if (DemoTitleBar != null)
        {
            DemoTitleBar.MinimizeClicked += (_, _) => UpdateStatus("Status: Minimize clicked (demo only)");
            DemoTitleBar.MaximizeRestoreClicked += (_, _) =>
            {
                var nextState = !(IsMaximizedCheckBox?.IsChecked == true);
                if (IsMaximizedCheckBox != null)
                {
                    IsMaximizedCheckBox.IsChecked = nextState;
                }

                ApplyDemoState();
                UpdateStatus($"Status: Maximize/Restore clicked (IsMaximized={nextState})");
            };
            DemoTitleBar.CloseClicked += (_, _) => UpdateStatus("Status: Close clicked (demo only)");
        }

        HookCheckBox(MinimizeCheckBox);
        HookCheckBox(MaximizeCheckBox);
        HookCheckBox(CloseCheckBox);
        HookCheckBox(IsMaximizedCheckBox);
    }

    private void HookCheckBox(CheckBox? checkBox)
    {
        if (checkBox == null)
        {
            return;
        }

        checkBox.Checked += OnDemoOptionChanged;
        checkBox.Unchecked += OnDemoOptionChanged;
    }

    private void OnDemoOptionChanged(object sender, RoutedEventArgs e)
    {
        ApplyDemoState();
        UpdateStatus("Status: Demo options updated");
    }

    private void ApplyDemoState()
    {
        if (DemoTitleBar == null)
        {
            return;
        }

        DemoTitleBar.ShowMinimizeButton = MinimizeCheckBox?.IsChecked == true;
        DemoTitleBar.ShowMaximizeButton = MaximizeCheckBox?.IsChecked == true;
        DemoTitleBar.ShowCloseButton = CloseCheckBox?.IsChecked == true;
        DemoTitleBar.IsMaximized = IsMaximizedCheckBox?.IsChecked == true;
    }

    private void UpdateStatus(string message)
    {
        if (StatusText != null)
        {
            StatusText.Text = message;
        }
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

    private const string XamlExample =
@"<!-- Basic TitleBar with all caption buttons -->
<TitleBar Title=""My Application""
          Height=""36""
          ShowMinimizeButton=""True""
          ShowMaximizeButton=""True""
          ShowCloseButton=""True""/>

<!-- TitleBar inside a custom window chrome -->
<Window Title=""Custom Window"">
    <WindowChrome.WindowChrome>
        <WindowChrome CaptionHeight=""36""
                      UseAeroCaptionButtons=""False""/>
    </WindowChrome.WindowChrome>

    <StackPanel Orientation=""Vertical"">
        <TitleBar Title=""Custom Chrome Window""
                  Height=""36""/>
        <Border Padding=""16"">
            <!-- Window content -->
        </Border>
    </StackPanel>
</Window>

<!-- TitleBar with minimize-only (tool window style) -->
<TitleBar Title=""Tool Window""
          Height=""28""
          ShowMinimizeButton=""False""
          ShowMaximizeButton=""False""
          ShowCloseButton=""True""/>";

    private const string CSharpExample =
@"// Create and configure a TitleBar programmatically
var titleBar = new TitleBar
{
    Title = ""My Application"",
    Height = 36,
    ShowMinimizeButton = true,
    ShowMaximizeButton = true,
    ShowCloseButton = true
};

// Handle caption button events
titleBar.MinimizeClicked += (sender, e) =>
{
    window.WindowState = WindowState.Minimized;
};

titleBar.MaximizeRestoreClicked += (sender, e) =>
{
    window.WindowState = window.WindowState == WindowState.Maximized
        ? WindowState.Normal
        : WindowState.Maximized;
    titleBar.IsMaximized = window.WindowState == WindowState.Maximized;
};

titleBar.CloseClicked += (sender, e) =>
{
    window.Close();
};

// Toggle button visibility dynamically
titleBar.ShowMaximizeButton = false; // Hide maximize
titleBar.IsMaximized = true; // Show restore icon

// Use with Window.TitleBarStyle for built-in integration
window.TitleBarStyle = TitleBarStyle.Custom;";
}
