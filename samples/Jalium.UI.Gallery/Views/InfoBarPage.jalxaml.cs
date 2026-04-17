using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for InfoBarPage.jalxaml demonstrating InfoBar functionality.
/// </summary>
public partial class InfoBarPage : Page
{
    private const string XamlExample =
@"<!-- Informational severity -->
<InfoBar Title=""Update Available""
         Message=""A new version (2.1.0) is ready to install.""
         Severity=""Informational""
         IsOpen=""True""
         IsClosable=""True"" />

<!-- Success severity -->
<InfoBar Title=""Saved""
         Message=""All changes have been saved successfully.""
         Severity=""Success""
         IsOpen=""{Binding ShowSaveSuccess}""
         IsClosable=""True"" />

<!-- Warning with action button -->
<InfoBar Title=""Low Disk Space""
         Message=""Less than 500 MB remaining on drive C:.""
         Severity=""Warning""
         IsOpen=""True"">
    <InfoBar.ActionButton>
        <Button Content=""Free up space"" Click=""OnFreeSpaceClick"" />
    </InfoBar.ActionButton>
</InfoBar>

<!-- Error severity -->
<InfoBar Title=""Connection Failed""
         Message=""Unable to reach the server. Check your network.""
         Severity=""Error""
         IsOpen=""True""
         IsClosable=""False"" />";

    private const string CSharpExample =
@"using Jalium.UI.Controls;

public partial class NotificationPage : Page
{
    public NotificationPage()
    {
        InitializeComponent();
    }

    private void ShowNotification(string title, string message,
        InfoBarSeverity severity)
    {
        var infoBar = new InfoBar
        {
            Title = title,
            Message = message,
            Severity = severity,
            IsOpen = true,
            IsClosable = true
        };

        infoBar.Closed += (s, e) =>
        {
            NotificationPanel.Children.Remove(infoBar);
        };

        NotificationPanel.Children.Insert(0, infoBar);
    }

    private void OnOperationCompleted()
    {
        ShowNotification(""Success"",
            ""The operation completed without errors."",
            InfoBarSeverity.Success);
    }

    private void OnOperationFailed(Exception ex)
    {
        ShowNotification(""Error"",
            $""Operation failed: {ex.Message}"",
            InfoBarSeverity.Error);
    }
}";

    public InfoBarPage()
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
