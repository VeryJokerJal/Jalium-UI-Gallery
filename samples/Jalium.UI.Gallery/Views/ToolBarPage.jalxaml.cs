using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ToolBarPage.jalxaml demonstrating toolbar functionality.
/// </summary>
public partial class ToolBarPage : Page
{
    public ToolBarPage()
    {
        InitializeComponent();

        // Set up event handlers
        if (NewButton != null) NewButton.Click += (s, e) => UpdateAction("New clicked");
        if (OpenButton != null) OpenButton.Click += (s, e) => UpdateAction("Open clicked");
        if (SaveButton != null) SaveButton.Click += (s, e) => UpdateAction("Save clicked");
        if (UndoButton != null) UndoButton.Click += (s, e) => UpdateAction("Undo clicked");
        if (RedoButton != null) RedoButton.Click += (s, e) => UpdateAction("Redo clicked");

        LoadCodeExamples();
    }

    private void UpdateAction(string action)
    {
        if (ActionText != null)
        {
            ActionText.Text = action;
        }
    }

    private const string XamlExample =
@"<!-- Basic ToolBar -->
<ToolBar Background=""Transparent"">
    <Button Content=""New"" Width=""60"" Height=""28""/>
    <Button Content=""Open"" Width=""60"" Height=""28""/>
    <Button Content=""Save"" Width=""60"" Height=""28""/>
    <Separator/>
    <Button Content=""Cut"" Width=""50"" Height=""28""/>
    <Button Content=""Copy"" Width=""50"" Height=""28""/>
    <Button Content=""Paste"" Width=""50"" Height=""28""/>
</ToolBar>

<!-- ToolBar with Mixed Controls -->
<ToolBar Background=""Transparent"">
    <Button Content=""File"" Width=""50"" Height=""28""/>
    <Separator/>
    <TextBlock Text=""Zoom:"" VerticalAlignment=""Center""/>
    <ComboBox Width=""80"" Height=""28"">
        <ComboBoxItem Content=""100%"" IsSelected=""True""/>
        <ComboBoxItem Content=""150%""/>
    </ComboBox>
    <Separator/>
    <CheckBox Content=""Bold""/>
    <CheckBox Content=""Italic""/>
    <Separator/>
    <TextBox Width=""150"" Height=""28""
             PlaceholderText=""Search...""/>
</ToolBar>";

    private const string CSharpExample =
@"// Create a ToolBar programmatically
var toolBar = new ToolBar
{
    Background = Brushes.Transparent
};

// Add buttons
var newBtn = new Button { Content = ""New"", Width = 60 };
newBtn.Click += (s, e) => UpdateAction(""New clicked"");
toolBar.Items.Add(newBtn);

var openBtn = new Button { Content = ""Open"", Width = 60 };
openBtn.Click += (s, e) => UpdateAction(""Open clicked"");
toolBar.Items.Add(openBtn);

// Add separator
toolBar.Items.Add(new Separator());

// Add other control types
toolBar.Items.Add(new CheckBox { Content = ""Bold"" });
toolBar.Items.Add(new TextBox
{
    Width = 150,
    PlaceholderText = ""Search...""
});

// Multiple toolbars in a stack
var toolBarTray = new StackPanel
{
    Orientation = Orientation.Vertical
};
toolBarTray.Children.Add(toolBar);
toolBarTray.Children.Add(secondToolBar);";

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
