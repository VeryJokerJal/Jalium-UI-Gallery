using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for TreeDataGridPage.jalxaml demonstrating TreeDataGrid functionality.
/// </summary>
public partial class TreeDataGridPage : Page
{
    public TreeDataGridPage()
    {
        InitializeComponent();
        SetupDemoData();
        SetupButtons();
        LoadCodeExamples();
    }

    private void SetupButtons()
    {
        if (ExpandAllButton != null)
            ExpandAllButton.Click += OnExpandAllClick;
        if (CollapseAllButton != null)
            CollapseAllButton.Click += OnCollapseAllClick;
    }

    private void SetupDemoData()
    {
        if (DemoTreeDataGrid == null) return;

        var items = new List<FolderItem>
        {
            new("src", "Folder", 0, new List<FolderItem>
            {
                new("Controls", "Folder", 0, new List<FolderItem>
                {
                    new("Button.cs", "C# Source", 4_200),
                    new("TextBox.cs", "C# Source", 8_750),
                    new("TreeDataGrid.cs", "C# Source", 12_400),
                    new("DataGrid.cs", "C# Source", 15_600)
                }),
                new("Core", "Folder", 0, new List<FolderItem>
                {
                    new("FrameworkElement.cs", "C# Source", 22_100),
                    new("DependencyObject.cs", "C# Source", 9_800),
                    new("DependencyProperty.cs", "C# Source", 6_300)
                }),
                new("Media", "Folder", 0, new List<FolderItem>
                {
                    new("Brush.cs", "C# Source", 3_500),
                    new("Color.cs", "C# Source", 2_100)
                })
            }),
            new("tests", "Folder", 0, new List<FolderItem>
            {
                new("ButtonTests.cs", "C# Source", 3_400),
                new("TreeDataGridTests.cs", "C# Source", 5_600),
                new("DataGridTests.cs", "C# Source", 4_800)
            }),
            new("README.md", "Markdown", 2_048),
            new("Directory.Build.props", "MSBuild", 1_024),
            new(".gitignore", "Git Config", 512)
        };

        DemoTreeDataGrid.ChildrenPropertyPath = "Children";
        DemoTreeDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Jalium.UI.Data.Binding("Name"),
            Width = 250
        });
        DemoTreeDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Type",
            Binding = new Jalium.UI.Data.Binding("Type"),
            Width = 120
        });
        DemoTreeDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Size",
            Binding = new Jalium.UI.Data.Binding("SizeDisplay"),
            Width = 100
        });

        DemoTreeDataGrid.ItemsSource = items;
        DemoTreeDataGrid.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedItemText != null && DemoTreeDataGrid?.SelectedItem is FolderItem item)
        {
            SelectedItemText.Text = $"{item.Name} ({item.Type})";
        }
        else if (SelectedItemText != null)
        {
            SelectedItemText.Text = "(none)";
        }
    }

    private void OnExpandAllClick(object? sender, EventArgs e)
    {
        DemoTreeDataGrid?.ExpandAll();
    }

    private void OnCollapseAllClick(object? sender, EventArgs e)
    {
        DemoTreeDataGrid?.CollapseAll();
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

    private const string XamlExample = @"<TreeDataGrid x:Name=""DemoTreeDataGrid""
              Height=""280""
              ChildrenPropertyPath=""Children""
              SelectionMode=""Single"" />";

    private const string CSharpExample = @"// Configure TreeDataGrid with columns
var tree = new TreeDataGrid();
tree.ChildrenPropertyPath = ""Children"";

tree.Columns.Add(new DataGridTextColumn
{
    Header = ""Name"",
    Binding = new Binding(""Name""),
    Width = 250
});
tree.Columns.Add(new DataGridTextColumn
{
    Header = ""Size"",
    Binding = new Binding(""Size"")
});

tree.ItemsSource = rootItems;
tree.ExpandAll();";

    private class FolderItem
    {
        public string Name { get; }
        public string Type { get; }
        public long Size { get; }
        public List<FolderItem> Children { get; }
        public string SizeDisplay => Size > 0 ? $"{Size:N0} B" : "";

        public FolderItem(string name, string type, long size, List<FolderItem>? children = null)
        {
            Name = name;
            Type = type;
            Size = size;
            Children = children ?? new List<FolderItem>();
        }
    }
}
