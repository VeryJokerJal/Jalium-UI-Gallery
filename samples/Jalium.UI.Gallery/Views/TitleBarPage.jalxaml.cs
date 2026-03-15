using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

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
}
