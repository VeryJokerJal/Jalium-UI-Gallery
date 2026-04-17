using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;
using RibbonControl = Jalium.UI.Controls.Ribbon.Ribbon;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for RibbonPage.jalxaml demonstrating the Office-style Ribbon control.
/// </summary>
public partial class RibbonPage : Page
{
    public RibbonPage()
    {
        InitializeComponent();

        // Ribbon button interactions
        if (PasteButton != null)
            PasteButton.Click += (s, e) => SetStatus("Paste clicked");
        if (CutButton != null)
            CutButton.Click += (s, e) => SetStatus("Cut clicked");
        if (CopyButton != null)
            CopyButton.Click += (s, e) => SetStatus("Copy clicked");
        if (InsertTableButton != null)
            InsertTableButton.Click += (s, e) => SetStatus("Insert Table clicked");

        if (BoldButton != null)
            BoldButton.Click += (s, e) => SetStatus("Bold clicked");
        if (ItalicButton != null)
            ItalicButton.Click += (s, e) => SetStatus("Italic clicked");
        if (UnderlineButton != null)
            UnderlineButton.Click += (s, e) => SetStatus("Underline clicked");

        // Minimize toggle
        if (MinimizeCheckBox != null)
        {
            MinimizeCheckBox.Checked += (s, e) =>
            {
                if (DemoRibbon != null) DemoRibbon.IsMinimized = true;
            };
            MinimizeCheckBox.Unchecked += (s, e) =>
            {
                if (DemoRibbon != null) DemoRibbon.IsMinimized = false;
            };
        }

        // Tab selection
        if (TabSelectComboBox != null)
        {
            TabSelectComboBox.SelectionChanged += (s, e) =>
            {
                if (DemoRibbon != null)
                    DemoRibbon.SelectedIndex = TabSelectComboBox.SelectedIndex;
            };
        }

        LoadCodeExamples();
    }

    private void SetStatus(string text)
    {
        if (StatusText != null)
            StatusText.Text = text;
    }

    private const string XamlExample = @"<Ribbon Title=""My Application"">
    <Ribbon.QuickAccessToolBar>
        <RibbonQuickAccessToolBar>
            <Button Content=""Save""/>
            <Button Content=""Undo""/>
        </RibbonQuickAccessToolBar>
    </Ribbon.QuickAccessToolBar>

    <RibbonTab Header=""Home"">
        <RibbonGroup Header=""Clipboard"">
            <Button Content=""Paste""/>
            <Button Content=""Cut""/>
            <Button Content=""Copy""/>
        </RibbonGroup>
        <RibbonGroup Header=""Font"">
            <ToggleButton Content=""B"" FontWeight=""Bold""/>
            <ToggleButton Content=""I"" FontStyle=""Italic""/>
            <ToggleButton Content=""U""/>
        </RibbonGroup>
    </RibbonTab>

    <RibbonTab Header=""Insert"">
        <RibbonGroup Header=""Tables"">
            <Button Content=""Table""/>
        </RibbonGroup>
        <RibbonGroup Header=""Illustrations"">
            <Button Content=""Picture""/>
            <Button Content=""Shapes""/>
            <Button Content=""Chart""/>
        </RibbonGroup>
    </RibbonTab>
</Ribbon>";

    private const string CSharpExample = @"// Access ribbon properties
ribbon.Title = ""My Document"";
ribbon.IsMinimized = false;
ribbon.SelectedIndex = 0;

// Toggle minimize at runtime
ribbon.IsMinimized = !ribbon.IsMinimized;

// Programmatically select a tab
ribbon.SelectedIndex = 1; // Switch to Insert tab

// Quick access toolbar
var qat = new RibbonQuickAccessToolBar();
qat.Items.Add(new Button { Content = ""Save"" });
ribbon.QuickAccessToolBar = qat;

// RibbonGallery with categories
var gallery = new RibbonGallery
{
    MinColumnCount = 3,
    MaxColumnCount = 6
};
gallery.SelectionChanged += (s, e) =>
    Debug.WriteLine($""Selected: {gallery.SelectedItem}"");";

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
