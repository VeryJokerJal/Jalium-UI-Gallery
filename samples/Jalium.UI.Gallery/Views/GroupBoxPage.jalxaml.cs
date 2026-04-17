using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for GroupBoxPage.jalxaml demonstrating GroupBox functionality.
/// </summary>
public partial class GroupBoxPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Basic GroupBox with form fields -->
    <GroupBox Header=""Personal Information"" Width=""300"">
        <StackPanel Orientation=""Vertical"" Margin=""8"">
            <TextBlock Text=""Name:"" Margin=""0,0,0,4""/>
            <TextBox Margin=""0,0,0,8""/>
            <TextBlock Text=""Email:"" Margin=""0,0,0,4""/>
            <TextBox Margin=""0,0,0,8""/>
            <TextBlock Text=""Phone:"" Margin=""0,0,0,4""/>
            <TextBox/>
        </StackPanel>
    </GroupBox>

    <!-- GroupBox with checkboxes -->
    <GroupBox Header=""Preferences"" Width=""300"" Margin=""0,16,0,0"">
        <StackPanel Orientation=""Vertical"" Margin=""8"">
            <CheckBox Content=""Receive email updates"" Margin=""0,0,0,8""/>
            <CheckBox Content=""Enable dark mode"" Margin=""0,0,0,8""/>
            <CheckBox Content=""Show advanced options"" Margin=""0,0,0,8""/>
            <CheckBox Content=""Auto-save on exit""/>
        </StackPanel>
    </GroupBox>

    <!-- Side-by-side GroupBoxes -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,16,0,0"">
        <GroupBox Header=""Connection Settings"" Width=""250"" Margin=""0,0,16,0"">
            <StackPanel Orientation=""Vertical"" Margin=""8"">
                <TextBlock Text=""Host:"" Margin=""0,0,0,4""/>
                <TextBox Text=""localhost"" Margin=""0,0,0,8""/>
                <TextBlock Text=""Port:"" Margin=""0,0,0,4""/>
                <TextBox Text=""5432""/>
            </StackPanel>
        </GroupBox>
        <GroupBox Header=""Authentication"" Width=""250"">
            <StackPanel Orientation=""Vertical"" Margin=""8"">
                <TextBlock Text=""Username:"" Margin=""0,0,0,4""/>
                <TextBox Margin=""0,0,0,8""/>
                <TextBlock Text=""Password:"" Margin=""0,0,0,4""/>
                <PasswordBox/>
            </StackPanel>
        </GroupBox>
    </StackPanel>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;

namespace MyApp;

public partial class GroupBoxDemo : Page
{
    public GroupBoxDemo()
    {
        InitializeComponent();
        CreateSettingsForm();
    }

    private void CreateSettingsForm()
    {
        // Create a GroupBox for application settings
        var settingsGroup = new GroupBox
        {
            Header = ""Application Settings"",
            Width = 350,
            Margin = new Thickness(0, 0, 0, 16)
        };

        var settingsPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(8)
        };

        // Add language selector
        settingsPanel.Children.Add(new TextBlock
        {
            Text = ""Language:"",
            Margin = new Thickness(0, 0, 0, 4)
        });

        var languageCombo = new ComboBox { Width = 200, Height = 28 };
        languageCombo.Items.Add(new ComboBoxItem { Content = ""English"" });
        languageCombo.Items.Add(new ComboBoxItem { Content = ""Japanese"" });
        languageCombo.Items.Add(new ComboBoxItem { Content = ""Chinese"" });
        languageCombo.SelectedIndex = 0;
        settingsPanel.Children.Add(languageCombo);

        // Add theme toggle
        var darkModeCheck = new CheckBox
        {
            Content = ""Enable dark mode"",
            IsChecked = true,
            Margin = new Thickness(0, 12, 0, 0)
        };
        settingsPanel.Children.Add(darkModeCheck);

        var autoSaveCheck = new CheckBox
        {
            Content = ""Auto-save on exit"",
            IsChecked = true,
            Margin = new Thickness(0, 8, 0, 0)
        };
        settingsPanel.Children.Add(autoSaveCheck);

        // Add save button
        var saveButton = new Button
        {
            Content = ""Save Settings"",
            Width = 120,
            Height = 32,
            Margin = new Thickness(0, 16, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        saveButton.Click += (s, e) => SaveSettings();
        settingsPanel.Children.Add(saveButton);

        settingsGroup.Content = settingsPanel;
        ContentPanel.Children.Add(settingsGroup);
    }

    private void SaveSettings()
    {
        // Persist settings to storage
    }
}";

    public GroupBoxPage()
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
