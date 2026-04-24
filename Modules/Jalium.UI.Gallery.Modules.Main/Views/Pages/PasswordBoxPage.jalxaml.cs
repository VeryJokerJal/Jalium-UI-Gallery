using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class PasswordBoxPage : Page
{
    private const string XamlExample = @"<!-- Basic PasswordBox -->
<PasswordBox Width=""300""
             Height=""32""/>

<!-- PasswordBox with pre-filled password -->
<PasswordBox Width=""300""
             Height=""32""
             Password=""secret123""/>

<!-- Disabled PasswordBox -->
<PasswordBox Width=""300""
             Height=""32""
             IsEnabled=""False""/>

<!-- Custom password character -->
<PasswordBox Width=""300""
             Height=""32""
             PasswordChar=""*""/>

<!-- PasswordBox with max length -->
<PasswordBox Width=""300""
             Height=""32""
             MaxLength=""16""/>

<!-- Login form layout -->
<StackPanel Orientation=""Vertical"" Spacing=""12"">
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""Username"" FontSize=""12""/>
        <TextBox x:Name=""UsernameBox""
                 Width=""300""
                 Height=""32""/>
    </StackPanel>
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""Password"" FontSize=""12""/>
        <PasswordBox x:Name=""PasswordInput""
                     Width=""300""
                     Height=""32""
                     PasswordChanged=""OnPasswordChanged""/>
    </StackPanel>
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""Confirm Password"" FontSize=""12""/>
        <PasswordBox x:Name=""ConfirmPasswordInput""
                     Width=""300""
                     Height=""32""
                     PasswordChanged=""OnConfirmPasswordChanged""/>
    </StackPanel>
    <TextBlock x:Name=""PasswordMatchStatus""
               Foreground=""#888888""
               FontSize=""12""/>
    <Button Content=""Sign In""
            Width=""300""
            Height=""36""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();

        PasswordInput.PasswordChanged += OnPasswordChanged;
        ConfirmPasswordInput.PasswordChanged += OnConfirmChanged;
        SignInButton.Click += OnSignInClick;
    }

    private void OnPasswordChanged(object sender, EventArgs e)
    {
        ValidatePasswords();
        UpdatePasswordStrength(PasswordInput.Password);
    }

    private void OnConfirmChanged(object sender, EventArgs e)
    {
        ValidatePasswords();
    }

    private void ValidatePasswords()
    {
        string pass = PasswordInput.Password ?? """";
        string confirm = ConfirmPasswordInput.Password ?? """";

        if (string.IsNullOrEmpty(confirm))
        {
            MatchStatus.Text = """";
        }
        else if (pass == confirm)
        {
            MatchStatus.Text = ""Passwords match"";
            MatchStatus.Foreground = new SolidColorBrush(
                Color.FromArgb(255, 0, 200, 83));
        }
        else
        {
            MatchStatus.Text = ""Passwords do not match"";
            MatchStatus.Foreground = new SolidColorBrush(
                Color.FromArgb(255, 255, 69, 58));
        }
    }

    private void UpdatePasswordStrength(string password)
    {
        int strength = 0;
        if (password.Length >= 8) strength++;
        if (password.Any(char.IsUpper)) strength++;
        if (password.Any(char.IsDigit)) strength++;
        if (password.Any(c => !char.IsLetterOrDigit(c)))
            strength++;

        StrengthText.Text = strength switch
        {
            0 => ""Strength: Very Weak"",
            1 => ""Strength: Weak"",
            2 => ""Strength: Fair"",
            3 => ""Strength: Strong"",
            _ => ""Strength: Very Strong""
        };
    }

    private void OnSignInClick(object? sender, EventArgs e)
    {
        string user = UsernameBox.Text ?? """";
        string pass = PasswordInput.Password ?? """";

        if (string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(pass))
        {
            StatusText.Text = ""Please fill in all fields."";
            return;
        }
        // Proceed with authentication...
    }
}";

    public PasswordBoxPage()
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
