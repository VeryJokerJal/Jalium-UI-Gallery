using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class TreeViewPage : Page
{
    public TreeViewPage()
    {
        InitializeComponent();
        CreateContent();
        LoadCodeExamples();
    }

    private void CreateContent()
    {
        if (DemoHost == null) return;

        // Basic TreeView — hierarchical items.
        var basicTreeView = CreateFileSystemTreeView();
        basicTreeView.Width = 300;
        basicTreeView.Height = 250;
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Basic TreeView", "A simple TreeView with hierarchical items.", basicTreeView));

        // TreeView with selection.
        var eventStack = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        var resultText = GalleryUi.ValueLabel("Selected: (none)");
        resultText.Margin = new Thickness(0, 0, 0, 8);
        eventStack.Children.Add(resultText);

        var eventTreeView = CreateCategoryTreeView();
        eventTreeView.Width = 300;
        eventTreeView.Height = 200;

        eventTreeView.SelectedItemChanged += (s, e) =>
        {
            if (e.NewValue is TreeViewItem item)
            {
                resultText.Text = $"Selected: {item.Header}";
            }
        };

        eventStack.Children.Add(eventTreeView);
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Selection Event", "Displays the selected item when a node is clicked.", eventStack));

        // Pre-expanded TreeView.
        var expandedTreeView = CreateExpandedTreeView();
        expandedTreeView.Width = 300;
        expandedTreeView.Height = 200;
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Pre-expanded Nodes", "A TreeView with some nodes already expanded.", expandedTreeView));
    }

    private TreeView CreateFileSystemTreeView()
    {
        var treeView = new TreeView();

        var documents = new TreeViewItem { Header = "Documents" };

        var workFolder = new TreeViewItem { Header = "Work" };
        workFolder.Items.Add(new TreeViewItem { Header = "Report.docx" });
        workFolder.Items.Add(new TreeViewItem { Header = "Presentation.pptx" });
        workFolder.Items.Add(new TreeViewItem { Header = "Budget.xlsx" });
        documents.Items.Add(workFolder);

        var personalFolder = new TreeViewItem { Header = "Personal" };
        personalFolder.Items.Add(new TreeViewItem { Header = "Notes.txt" });
        personalFolder.Items.Add(new TreeViewItem { Header = "Photos" });
        documents.Items.Add(personalFolder);

        treeView.Items.Add(documents);

        var downloads = new TreeViewItem { Header = "Downloads" };
        downloads.Items.Add(new TreeViewItem { Header = "installer.exe" });
        downloads.Items.Add(new TreeViewItem { Header = "archive.zip" });
        treeView.Items.Add(downloads);

        var desktop = new TreeViewItem { Header = "Desktop" };
        desktop.Items.Add(new TreeViewItem { Header = "Shortcut.lnk" });
        treeView.Items.Add(desktop);

        return treeView;
    }

    private TreeView CreateCategoryTreeView()
    {
        var treeView = new TreeView();

        var animals = new TreeViewItem { Header = "Animals" };

        var mammals = new TreeViewItem { Header = "Mammals" };
        mammals.Items.Add(new TreeViewItem { Header = "Dog" });
        mammals.Items.Add(new TreeViewItem { Header = "Cat" });
        mammals.Items.Add(new TreeViewItem { Header = "Elephant" });
        animals.Items.Add(mammals);

        var birds = new TreeViewItem { Header = "Birds" };
        birds.Items.Add(new TreeViewItem { Header = "Eagle" });
        birds.Items.Add(new TreeViewItem { Header = "Sparrow" });
        animals.Items.Add(birds);

        treeView.Items.Add(animals);

        var plants = new TreeViewItem { Header = "Plants" };
        plants.Items.Add(new TreeViewItem { Header = "Trees" });
        plants.Items.Add(new TreeViewItem { Header = "Flowers" });
        plants.Items.Add(new TreeViewItem { Header = "Grass" });
        treeView.Items.Add(plants);

        return treeView;
    }

    private TreeView CreateExpandedTreeView()
    {
        var treeView = new TreeView();

        var root = new TreeViewItem { Header = "Project", IsExpanded = true };

        var src = new TreeViewItem { Header = "src", IsExpanded = true };
        src.Items.Add(new TreeViewItem { Header = "App.cs" });
        src.Items.Add(new TreeViewItem { Header = "MainWindow.cs" });

        var components = new TreeViewItem { Header = "Components" };
        components.Items.Add(new TreeViewItem { Header = "Button.cs" });
        components.Items.Add(new TreeViewItem { Header = "TextBox.cs" });
        src.Items.Add(components);

        root.Items.Add(src);

        var tests = new TreeViewItem { Header = "tests" };
        tests.Items.Add(new TreeViewItem { Header = "UnitTests.cs" });
        root.Items.Add(tests);

        root.Items.Add(new TreeViewItem { Header = "README.md" });

        treeView.Items.Add(root);

        return treeView;
    }

    private const string XamlExample =
@"<!-- Basic TreeView -->
<TreeView Width=""300"" Height=""250"">
    <TreeViewItem Header=""Documents"">
        <TreeViewItem Header=""Work"">
            <TreeViewItem Header=""Report.docx""/>
            <TreeViewItem Header=""Budget.xlsx""/>
        </TreeViewItem>
        <TreeViewItem Header=""Personal"">
            <TreeViewItem Header=""Notes.txt""/>
        </TreeViewItem>
    </TreeViewItem>
    <TreeViewItem Header=""Downloads"">
        <TreeViewItem Header=""installer.exe""/>
        <TreeViewItem Header=""archive.zip""/>
    </TreeViewItem>
</TreeView>

<!-- Pre-expanded nodes -->
<TreeViewItem Header=""Project"" IsExpanded=""True"">
    <TreeViewItem Header=""src"" IsExpanded=""True"">
        <TreeViewItem Header=""App.cs""/>
    </TreeViewItem>
</TreeViewItem>";

    private const string CSharpExample =
@"// Create a TreeView programmatically
var treeView = new TreeView();

var documents = new TreeViewItem { Header = ""Documents"" };

var workFolder = new TreeViewItem { Header = ""Work"" };
workFolder.Items.Add(new TreeViewItem
    { Header = ""Report.docx"" });
workFolder.Items.Add(new TreeViewItem
    { Header = ""Budget.xlsx"" });
documents.Items.Add(workFolder);

treeView.Items.Add(documents);

// Handle selection changes
treeView.SelectedItemChanged += (s, e) =>
{
    if (e.NewValue is TreeViewItem item)
    {
        statusText.Text = $""Selected: {item.Header}"";
    }
};

// Pre-expand nodes
var root = new TreeViewItem
{
    Header = ""Project"",
    IsExpanded = true
};";

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
