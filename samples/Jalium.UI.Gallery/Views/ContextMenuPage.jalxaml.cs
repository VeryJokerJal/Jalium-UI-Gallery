using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ContextMenuPage.jalxaml demonstrating context menu functionality.
/// </summary>
public partial class ContextMenuPage : Page
{
    public ContextMenuPage()
    {
        InitializeComponent();
        // Context menus are demonstrated statically in the XAML.
        // The interactive demo shows the currently selected action.
        LoadCodeExamples();
    }

    private const string XamlExample =
@"<!-- Basic ContextMenu -->
<Border Background=""#1E1E1E"" Padding=""40"">
    <TextBlock Text=""Right-click here""/>
    <Border.ContextMenu>
        <ContextMenu>
            <MenuItem Header=""Cut"" InputGestureText=""Ctrl+X""/>
            <MenuItem Header=""Copy"" InputGestureText=""Ctrl+C""/>
            <MenuItem Header=""Paste"" InputGestureText=""Ctrl+V""/>
            <Separator/>
            <MenuItem Header=""Select All"" InputGestureText=""Ctrl+A""/>
        </ContextMenu>
    </Border.ContextMenu>
</Border>

<!-- ContextMenu with Submenus -->
<Border.ContextMenu>
    <ContextMenu>
        <MenuItem Header=""New"">
            <MenuItem Header=""File""/>
            <MenuItem Header=""Folder""/>
        </MenuItem>
        <MenuItem Header=""Sort By"">
            <MenuItem Header=""Name""/>
            <MenuItem Header=""Date""/>
            <MenuItem Header=""Size""/>
        </MenuItem>
        <Separator/>
        <MenuItem Header=""Properties""/>
    </ContextMenu>
</Border.ContextMenu>";

    private const string CSharpExample =
@"// Create a ContextMenu programmatically
var contextMenu = new ContextMenu();

contextMenu.Items.Add(new MenuItem
{
    Header = ""Cut"",
    InputGestureText = ""Ctrl+X""
});
contextMenu.Items.Add(new MenuItem
{
    Header = ""Copy"",
    InputGestureText = ""Ctrl+C""
});
contextMenu.Items.Add(new MenuItem
{
    Header = ""Paste"",
    InputGestureText = ""Ctrl+V""
});

// Assign to an element
myBorder.ContextMenu = contextMenu;

// Handle menu item click
var actionItem = new MenuItem { Header = ""Action 1"" };
actionItem.Click += (s, e) =>
{
    statusText.Text = ""Action 1 selected"";
};
contextMenu.Items.Add(actionItem);

// Nested submenus
var sortMenu = new MenuItem { Header = ""Sort By"" };
sortMenu.Items.Add(new MenuItem { Header = ""Name"" });
sortMenu.Items.Add(new MenuItem { Header = ""Date"" });
contextMenu.Items.Add(sortMenu);";

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
