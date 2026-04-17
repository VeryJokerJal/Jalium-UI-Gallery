using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for MenuPage.jalxaml demonstrating menu control functionality.
/// </summary>
public partial class MenuPage : Page
{
    public MenuPage()
    {
        InitializeComponent();
        LoadCodeExamples();
    }

    private const string XamlExample =
@"<Menu Background=""Transparent"">
    <MenuItem Header=""File"">
        <MenuItem Header=""New"" InputGestureText=""Ctrl+N""/>
        <MenuItem Header=""Open"" InputGestureText=""Ctrl+O""/>
        <MenuItem Header=""Save"" InputGestureText=""Ctrl+S""/>
        <Separator/>
        <MenuItem Header=""Exit""/>
    </MenuItem>
    <MenuItem Header=""Edit"">
        <MenuItem Header=""Undo"" InputGestureText=""Ctrl+Z""/>
        <MenuItem Header=""Redo"" InputGestureText=""Ctrl+Y""/>
        <Separator/>
        <MenuItem Header=""Cut"" InputGestureText=""Ctrl+X""/>
        <MenuItem Header=""Copy"" InputGestureText=""Ctrl+C""/>
        <MenuItem Header=""Paste"" InputGestureText=""Ctrl+V""/>
    </MenuItem>
</Menu>

<!-- Nested Submenus -->
<Menu>
    <MenuItem Header=""Settings"">
        <MenuItem Header=""Appearance"">
            <MenuItem Header=""Theme"">
                <MenuItem Header=""Light""/>
                <MenuItem Header=""Dark""/>
            </MenuItem>
        </MenuItem>
    </MenuItem>
</Menu>";

    private const string CSharpExample =
@"// Create a Menu programmatically
var menu = new Menu();

var fileMenu = new MenuItem { Header = ""File"" };
fileMenu.Items.Add(new MenuItem
{
    Header = ""New"",
    InputGestureText = ""Ctrl+N""
});
fileMenu.Items.Add(new MenuItem
{
    Header = ""Open"",
    InputGestureText = ""Ctrl+O""
});
fileMenu.Items.Add(new Separator());
fileMenu.Items.Add(new MenuItem { Header = ""Exit"" });

menu.Items.Add(fileMenu);

// Handle menu item click
var saveItem = new MenuItem
{
    Header = ""Save"",
    InputGestureText = ""Ctrl+S""
};
saveItem.Click += (s, e) =>
{
    // Save logic here
};
fileMenu.Items.Add(saveItem);";

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
