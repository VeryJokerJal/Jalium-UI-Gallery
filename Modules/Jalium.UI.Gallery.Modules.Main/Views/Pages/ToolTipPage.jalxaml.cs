using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for ToolTipPage.jalxaml demonstrating tooltip functionality.
/// </summary>
public partial class ToolTipPage : Page
{
    public ToolTipPage()
    {
        InitializeComponent();

        // Keep one example configured from code-behind.
        if (SaveButton != null)
        {
            SaveButton.ToolTip = "Save the current document";
        }

        LoadCodeExamples();
    }

    private const string XamlExample = @"<!-- Simple string ToolTip -->
<Button Content=""Open""
        ToolTip=""Open an existing document""/>

<!-- ToolTip on various controls -->
<TextBox PlaceholderText=""Enter username""
         ToolTip=""Enter your username (3-20 characters)""/>

<Slider Minimum=""0"" Maximum=""100""
        ToolTip=""Adjust the volume level""/>

<CheckBox Content=""Enable notifications""
          ToolTip=""Turn on/off notification alerts""/>

<!-- Rich content ToolTip -->
<Button Content=""Hover for rich tooltip"">
    <Button.ToolTip>
        <ToolTip>
            <StackPanel Orientation=""Vertical"">
                <TextBlock Text=""Rich ToolTip"" FontWeight=""Bold""/>
                <TextBlock Text=""Multi-line tooltip content""/>
                <Separator Margin=""0,8""/>
                <TextBlock Text=""Shortcut: Ctrl+R"" FontSize=""11""/>
            </StackPanel>
        </ToolTip>
    </Button.ToolTip>
</Button>";

    private const string CSharpExample = @"// Set ToolTip from code-behind
SaveButton.ToolTip = ""Save the current document"";

// ToolTip with complex content
var richToolTip = new ToolTip();
var panel = new StackPanel { Orientation = Orientation.Vertical };
panel.Children.Add(new TextBlock
{
    Text = ""Rich ToolTip"",
    FontWeight = FontWeights.Bold
});
panel.Children.Add(new TextBlock
{
    Text = ""This tooltip has multiple lines""
});
richToolTip.Content = panel;
myButton.ToolTip = richToolTip;";

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
