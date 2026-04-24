using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for SeparatorPage.jalxaml demonstrating Separator functionality.
/// </summary>
public partial class SeparatorPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Horizontal Separator between content sections -->
    <StackPanel Orientation=""Vertical"" Width=""300"">
        <TextBlock Text=""Section 1: General Settings""
                   FontSize=""14"" FontWeight=""SemiBold"" Margin=""0,0,0,8""/>
        <CheckBox Content=""Enable auto-updates"" Margin=""0,0,0,4""/>
        <CheckBox Content=""Show notifications"" Margin=""0,0,0,8""/>

        <Separator Margin=""0,8""/>

        <TextBlock Text=""Section 2: Display Settings""
                   FontSize=""14"" FontWeight=""SemiBold"" Margin=""0,0,0,8""/>
        <CheckBox Content=""Dark mode"" Margin=""0,0,0,4""/>
        <CheckBox Content=""Compact view"" Margin=""0,0,0,8""/>

        <Separator Margin=""0,8""/>

        <TextBlock Text=""Section 3: Advanced""
                   FontSize=""14"" FontWeight=""SemiBold"" Margin=""0,0,0,8""/>
        <CheckBox Content=""Developer mode""/>
    </StackPanel>

    <!-- Separator in a menu-like context -->
    <Border Background=""#3D3D40"" Padding=""8"" CornerRadius=""4""
            Width=""200"" HorizontalAlignment=""Left"" Margin=""0,16,0,0"">
        <StackPanel Orientation=""Vertical"">
            <TextBlock Text=""New File"" Margin=""4""/>
            <TextBlock Text=""Open File"" Margin=""4""/>
            <TextBlock Text=""Open Recent"" Margin=""4""/>
            <Separator Margin=""0,4""/>
            <TextBlock Text=""Save"" Margin=""4""/>
            <TextBlock Text=""Save As..."" Margin=""4""/>
            <TextBlock Text=""Save All"" Margin=""4""/>
            <Separator Margin=""0,4""/>
            <TextBlock Text=""Print..."" Margin=""4""/>
            <Separator Margin=""0,4""/>
            <TextBlock Text=""Exit"" Margin=""4""/>
        </StackPanel>
    </Border>

    <!-- Separator in a toolbar -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,16,0,0"">
        <Button Content=""Cut"" Width=""60"" Height=""28"" Margin=""2""/>
        <Button Content=""Copy"" Width=""60"" Height=""28"" Margin=""2""/>
        <Button Content=""Paste"" Width=""60"" Height=""28"" Margin=""2""/>
        <Separator Width=""1"" Margin=""4,0""/>
        <Button Content=""Undo"" Width=""60"" Height=""28"" Margin=""2""/>
        <Button Content=""Redo"" Width=""60"" Height=""28"" Margin=""2""/>
    </StackPanel>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace MyApp;

public partial class SeparatorDemo : Page
{
    public SeparatorDemo()
    {
        InitializeComponent();
        BuildSettingsPanel();
    }

    private void BuildSettingsPanel()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Width = 350
        };

        // General section
        AddSectionHeader(panel, ""General"");
        panel.Children.Add(new CheckBox
        {
            Content = ""Start on system startup"",
            Margin = new Thickness(0, 0, 0, 4)
        });
        panel.Children.Add(new CheckBox
        {
            Content = ""Check for updates automatically"",
            Margin = new Thickness(0, 0, 0, 4)
        });

        // Add separator
        panel.Children.Add(new Separator
        {
            Margin = new Thickness(0, 12, 0, 12)
        });

        // Appearance section
        AddSectionHeader(panel, ""Appearance"");
        panel.Children.Add(new CheckBox
        {
            Content = ""Use dark theme"",
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 4)
        });
        panel.Children.Add(new CheckBox
        {
            Content = ""Show status bar"",
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 4)
        });

        // Add separator
        panel.Children.Add(new Separator
        {
            Margin = new Thickness(0, 12, 0, 12)
        });

        // Privacy section
        AddSectionHeader(panel, ""Privacy"");
        panel.Children.Add(new CheckBox
        {
            Content = ""Send anonymous usage data"",
            Margin = new Thickness(0, 0, 0, 4)
        });
        panel.Children.Add(new CheckBox
        {
            Content = ""Remember recent files"",
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 4)
        });

        ContentPanel.Children.Add(panel);
    }

    private void AddSectionHeader(StackPanel parent, string title)
    {
        parent.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            Margin = new Thickness(0, 0, 0, 8)
        });
    }
}";

    public SeparatorPage()
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
