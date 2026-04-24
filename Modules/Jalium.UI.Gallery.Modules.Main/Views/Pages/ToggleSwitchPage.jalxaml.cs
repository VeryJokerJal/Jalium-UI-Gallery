using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for ToggleSwitchPage.jalxaml demonstrating ToggleSwitch functionality.
/// </summary>
public partial class ToggleSwitchPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic ToggleSwitch with header -->
    <ToggleSwitch Header=""Wi-Fi""
                  IsOn=""True""
                  OnContent=""On""
                  OffContent=""Off""
                  Margin=""0,0,0,16""/>

    <!-- ToggleSwitch with custom on/off content -->
    <ToggleSwitch Header=""Bluetooth""
                  OnContent=""Enabled""
                  OffContent=""Disabled""
                  Margin=""0,0,0,16""/>

    <!-- ToggleSwitch with symbol content -->
    <ToggleSwitch Header=""Airplane Mode""
                  OnContent=""&#x2713;""
                  OffContent=""&#x2717;""
                  Margin=""0,0,0,16""/>

    <!-- Disabled ToggleSwitch -->
    <ToggleSwitch Header=""Dark Mode (Locked)""
                  IsOn=""True""
                  IsEnabled=""False""
                  Margin=""0,0,0,16""/>

    <!-- Data-bound ToggleSwitch -->
    <ToggleSwitch Header=""Notifications""
                  IsOn=""{Binding EnableNotifications, Mode=TwoWay}""
                  OnContent=""Active""
                  OffContent=""Muted""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class ToggleSwitchSample : Page
{
    public ToggleSwitchSample()
    {
        InitializeComponent();
        SetupToggleSwitches();
    }

    private void SetupToggleSwitches()
    {
        // Create a ToggleSwitch programmatically
        var wifiSwitch = new ToggleSwitch
        {
            Header = ""Wi-Fi"",
            IsOn = true,
            OnContent = ""Connected"",
            OffContent = ""Disconnected""
        };

        // Handle toggled event
        wifiSwitch.Toggled += OnWifiToggled;

        // Query current state
        bool isWifiOn = wifiSwitch.IsOn;

        // Programmatically toggle
        wifiSwitch.IsOn = !wifiSwitch.IsOn;

        // Disable the switch
        wifiSwitch.IsEnabled = false;
    }

    private void OnWifiToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggle)
        {
            string state = toggle.IsOn ? ""ON"" : ""OFF"";
            StatusText.Text = $""{toggle.Header} is now {state}"";
        }
    }
}";

    public ToggleSwitchPage()
    {
        InitializeComponent();
        LoadCodeExamples();
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
}
