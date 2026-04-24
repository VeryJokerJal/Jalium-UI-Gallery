using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for LabelPage.jalxaml demonstrating Label functionality.
/// </summary>
public partial class LabelPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Label associated with a TextBox via Target binding -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,12"">
        <Label Content=""_Username:""
               Target=""{Binding ElementName=UsernameBox}""
               Width=""120""
               VerticalAlignment=""Center""/>
        <TextBox x:Name=""UsernameBox""
                 Width=""250""
                 Watermark=""Enter your username""/>
    </StackPanel>

    <!-- Label with access key for password field -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,12"">
        <Label Content=""_Password:""
               Target=""{Binding ElementName=PasswordBox}""
               Width=""120""
               VerticalAlignment=""Center""/>
        <PasswordBox x:Name=""PasswordBox""
                     Width=""250""/>
    </StackPanel>

    <!-- Label with custom styling -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,12"">
        <Label Content=""_Email:""
               Target=""{Binding ElementName=EmailBox}""
               Width=""120""
               FontWeight=""Bold""
               Foreground=""#0078D4""
               VerticalAlignment=""Center""/>
        <TextBox x:Name=""EmailBox""
                 Width=""250""
                 Watermark=""user@example.com""/>
    </StackPanel>

    <!-- Label with content other than text -->
    <Label Margin=""0,16,0,0"">
        <StackPanel Orientation=""Horizontal"">
            <TextBlock Text=""&#xE8FA;"" FontFamily=""Segoe MDL2 Assets"" Margin=""0,0,8,0""/>
            <TextBlock Text=""Settings""/>
        </StackPanel>
    </Label>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class LoginForm : Page
{
    public LoginForm()
    {
        InitializeComponent();
        SetupLabels();
    }

    private void SetupLabels()
    {
        // Create a label programmatically
        var emailLabel = new Label
        {
            Content = ""_Email:"",
            Width = 120
        };

        // Associate the label with a target control
        // When the user presses Alt+E, focus moves to EmailBox
        emailLabel.Target = EmailBox;

        // Labels support any content, not just text
        var iconLabel = new Label();
        var panel = new StackPanel { Orientation = Orientation.Horizontal };
        panel.Children.Add(new TextBlock { Text = ""Settings"" });
        iconLabel.Content = panel;

        // Add labels to the form
        FormPanel.Children.Add(emailLabel);
        FormPanel.Children.Add(EmailBox);

        // Access key underline appears when Alt is held
        // The underscore prefix (_Email) defines the access key
        var nameLabel = new Label
        {
            Content = ""_Name:"",
            Target = NameBox,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212))
        };
        FormPanel.Children.Add(nameLabel);
    }
}";

    public LabelPage()
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
