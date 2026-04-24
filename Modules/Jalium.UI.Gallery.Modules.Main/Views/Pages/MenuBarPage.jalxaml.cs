using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for MenuBarPage.jalxaml demonstrating MenuBar (Menu) functionality.
/// </summary>
public partial class MenuBarPage : Page
{
    public MenuBarPage()
    {
        InitializeComponent();
        BuildMenuBar();
        LoadCodeExamples();
    }

    private void BuildMenuBar()
    {
        if (MenuBarContainer == null) return;

        var menuBar = new Menu();

        // File menu
        var fileMenu = new MenuItem { Header = "File" };
        var newItem = new MenuItem { Header = "New", InputGestureText = "Ctrl+N" };
        newItem.Click += (s, e) => UpdateStatus("File > New clicked");
        fileMenu.Items.Add(newItem);

        var openItem = new MenuItem { Header = "Open", InputGestureText = "Ctrl+O" };
        openItem.Click += (s, e) => UpdateStatus("File > Open clicked");
        fileMenu.Items.Add(openItem);

        var saveItem = new MenuItem { Header = "Save", InputGestureText = "Ctrl+S" };
        saveItem.Click += (s, e) => UpdateStatus("File > Save clicked");
        fileMenu.Items.Add(saveItem);

        var saveAsItem = new MenuItem { Header = "Save As...", InputGestureText = "Ctrl+Shift+S" };
        saveAsItem.Click += (s, e) => UpdateStatus("File > Save As clicked");
        fileMenu.Items.Add(saveAsItem);

        fileMenu.Items.Add(new Separator());

        var exitItem = new MenuItem { Header = "Exit", InputGestureText = "Alt+F4" };
        exitItem.Click += (s, e) => UpdateStatus("File > Exit clicked");
        fileMenu.Items.Add(exitItem);

        menuBar.Items.Add(fileMenu);

        // Edit menu
        var editMenu = new MenuItem { Header = "Edit" };
        var undoItem = new MenuItem { Header = "Undo", InputGestureText = "Ctrl+Z" };
        undoItem.Click += (s, e) => UpdateStatus("Edit > Undo clicked");
        editMenu.Items.Add(undoItem);

        var redoItem = new MenuItem { Header = "Redo", InputGestureText = "Ctrl+Y" };
        redoItem.Click += (s, e) => UpdateStatus("Edit > Redo clicked");
        editMenu.Items.Add(redoItem);

        editMenu.Items.Add(new Separator());

        var cutItem = new MenuItem { Header = "Cut", InputGestureText = "Ctrl+X" };
        cutItem.Click += (s, e) => UpdateStatus("Edit > Cut clicked");
        editMenu.Items.Add(cutItem);

        var copyItem = new MenuItem { Header = "Copy", InputGestureText = "Ctrl+C" };
        copyItem.Click += (s, e) => UpdateStatus("Edit > Copy clicked");
        editMenu.Items.Add(copyItem);

        var pasteItem = new MenuItem { Header = "Paste", InputGestureText = "Ctrl+V" };
        pasteItem.Click += (s, e) => UpdateStatus("Edit > Paste clicked");
        editMenu.Items.Add(pasteItem);

        editMenu.Items.Add(new Separator());

        var selectAllItem = new MenuItem { Header = "Select All", InputGestureText = "Ctrl+A" };
        selectAllItem.Click += (s, e) => UpdateStatus("Edit > Select All clicked");
        editMenu.Items.Add(selectAllItem);

        menuBar.Items.Add(editMenu);

        // View menu
        var viewMenu = new MenuItem { Header = "View" };
        var zoomInItem = new MenuItem { Header = "Zoom In", InputGestureText = "Ctrl++" };
        zoomInItem.Click += (s, e) => UpdateStatus("View > Zoom In clicked");
        viewMenu.Items.Add(zoomInItem);

        var zoomOutItem = new MenuItem { Header = "Zoom Out", InputGestureText = "Ctrl+-" };
        zoomOutItem.Click += (s, e) => UpdateStatus("View > Zoom Out clicked");
        viewMenu.Items.Add(zoomOutItem);

        viewMenu.Items.Add(new Separator());

        var statusBarItem = new MenuItem { Header = "Status Bar", IsCheckable = true, IsChecked = true };
        statusBarItem.Click += (s, e) => UpdateStatus($"View > Status Bar toggled: {statusBarItem.IsChecked}");
        viewMenu.Items.Add(statusBarItem);

        var wordWrapItem = new MenuItem { Header = "Word Wrap", IsCheckable = true };
        wordWrapItem.Click += (s, e) => UpdateStatus($"View > Word Wrap toggled: {wordWrapItem.IsChecked}");
        viewMenu.Items.Add(wordWrapItem);

        menuBar.Items.Add(viewMenu);

        // Help menu
        var helpMenu = new MenuItem { Header = "Help" };
        var docsItem = new MenuItem { Header = "Documentation" };
        docsItem.Click += (s, e) => UpdateStatus("Help > Documentation clicked");
        helpMenu.Items.Add(docsItem);

        var releaseNotesItem = new MenuItem { Header = "Release Notes" };
        releaseNotesItem.Click += (s, e) => UpdateStatus("Help > Release Notes clicked");
        helpMenu.Items.Add(releaseNotesItem);

        helpMenu.Items.Add(new Separator());

        var aboutItem = new MenuItem { Header = "About" };
        aboutItem.Click += (s, e) => UpdateStatus("Help > About clicked");
        helpMenu.Items.Add(aboutItem);

        menuBar.Items.Add(helpMenu);

        MenuBarContainer.Child = menuBar;
    }

    private const string XamlExample = @"<Menu>
    <MenuItem Header=""File"">
        <MenuItem Header=""New"" InputGestureText=""Ctrl+N""/>
        <MenuItem Header=""Open"" InputGestureText=""Ctrl+O""/>
        <MenuItem Header=""Save"" InputGestureText=""Ctrl+S""/>
        <Separator/>
        <MenuItem Header=""Exit"" InputGestureText=""Alt+F4""/>
    </MenuItem>
    <MenuItem Header=""Edit"">
        <MenuItem Header=""Undo"" InputGestureText=""Ctrl+Z""/>
        <MenuItem Header=""Redo"" InputGestureText=""Ctrl+Y""/>
        <Separator/>
        <MenuItem Header=""Cut"" InputGestureText=""Ctrl+X""/>
        <MenuItem Header=""Copy"" InputGestureText=""Ctrl+C""/>
        <MenuItem Header=""Paste"" InputGestureText=""Ctrl+V""/>
    </MenuItem>
    <MenuItem Header=""View"">
        <MenuItem Header=""Status Bar"" IsCheckable=""True"" IsChecked=""True""/>
        <MenuItem Header=""Word Wrap"" IsCheckable=""True""/>
    </MenuItem>
    <MenuItem Header=""Help"">
        <MenuItem Header=""Documentation""/>
        <MenuItem Header=""About""/>
    </MenuItem>
</Menu>";

    private const string CSharpExample = @"// Create a MenuBar programmatically
var menuBar = new Menu();

// File menu
var fileMenu = new MenuItem { Header = ""File"" };
var newItem = new MenuItem { Header = ""New"", InputGestureText = ""Ctrl+N"" };
newItem.Click += (s, e) => Debug.WriteLine(""New clicked"");
fileMenu.Items.Add(newItem);
menuBar.Items.Add(fileMenu);

// Edit menu with checkable items
var viewMenu = new MenuItem { Header = ""View"" };
var statusBar = new MenuItem
{
    Header = ""Status Bar"",
    IsCheckable = true,
    IsChecked = true
};
viewMenu.Items.Add(statusBar);
menuBar.Items.Add(viewMenu);";

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

    private void UpdateStatus(string message)
    {
        if (MenuStatusText != null)
        {
            MenuStatusText.Text = message;
        }
        System.Diagnostics.Debug.WriteLine($"[MenuBarPage] {message}");
    }
}
