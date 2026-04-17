using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for JsonTreeViewerPage.jalxaml demonstrating the JsonTreeViewer control.
/// </summary>
public partial class JsonTreeViewerPage : Page
{
    private const string XamlExample = """
        <!-- Basic JSON Tree with expand depth -->
        <JsonTreeViewer x:Name="BasicJsonTree"
                        Height="350"
                        ExpandDepth="2"/>

        <!-- Search & Filter -->
        <TextBox x:Name="SearchInput"
                 PlaceholderText="Search keys or values..."/>
        <JsonTreeViewer x:Name="SearchJsonTree"
                        Height="300"
                        ExpandDepth="3"/>

        <!-- Interactive: type JSON and see tree update -->
        <TextBox x:Name="JsonInput"
                 AcceptsReturn="True"
                 Height="350"
                 FontFamily="Consolas"/>
        <JsonTreeViewer x:Name="InteractiveJsonTree"
                        Height="350"
                        ExpandDepth="3"/>
        """;

    private const string CSharpExample = """"
        using Jalium.UI.Controls;

        // Load JSON into the tree viewer
        jsonTree.JsonText = """
        {
          "name": "Jalium.UI",
          "version": "1.0.0",
          "features": ["syntax-highlight", "tree-view"]
        }
        """;
        jsonTree.ExpandDepth = 2;

        // Search/filter the tree
        searchInput.TextChanged += (s, e) =>
        {
            jsonTree.SearchText = searchInput.Text ?? "";
        };

        // Interactive: update tree on JSON input change
        jsonInput.TextChanged += (s, e) =>
        {
            interactiveTree.JsonText = jsonInput.Text ?? "";
        };
        """";

    private const string SampleJson =
        """
        {
          "application": {
            "name": "Jalium.UI Gallery",
            "version": "1.0.0",
            "framework": "Jalium.UI"
          },
          "settings": {
            "theme": "dark",
            "fontSize": 14,
            "showLineNumbers": true,
            "recentFiles": [
              "document1.txt",
              "image.png",
              "config.json"
            ],
            "window": {
              "width": 1280,
              "height": 720,
              "isMaximized": false
            }
          },
          "plugins": [
            {
              "id": "syntax-highlight",
              "enabled": true,
              "version": "2.1.0"
            },
            {
              "id": "auto-complete",
              "enabled": true,
              "version": "1.5.3"
            },
            {
              "id": "git-integration",
              "enabled": false,
              "version": null
            }
          ],
          "metadata": {
            "createdAt": "2026-01-15T10:30:00Z",
            "modifiedAt": "2026-03-31T14:00:00Z",
            "tags": ["ui", "desktop", "windows"]
          }
        }
        """;

    private const string SearchSampleJson =
        """
        {
          "employees": [
            { "name": "Alice Johnson", "department": "Engineering", "salary": 95000 },
            { "name": "Bob Smith", "department": "Design", "salary": 82000 },
            { "name": "Charlie Brown", "department": "Engineering", "salary": 105000 },
            { "name": "Diana Prince", "department": "Marketing", "salary": 78000 },
            { "name": "Eve Adams", "department": "Engineering", "salary": 92000 }
          ],
          "departments": ["Engineering", "Design", "Marketing", "Sales"],
          "totalBudget": 1500000
        }
        """;

    public JsonTreeViewerPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        // Set up basic JSON tree
        if (BasicJsonTree != null)
        {
            BasicJsonTree.JsonText = SampleJson;
        }

        // Set up search JSON tree
        if (SearchJsonTree != null)
        {
            SearchJsonTree.JsonText = SearchSampleJson;
        }

        // Wire search input
        if (SearchInput != null)
        {
            SearchInput.TextChanged += OnSearchTextChanged;
        }

        // Set up interactive demo
        if (JsonInput != null)
        {
            JsonInput.Text = "{\n  \"greeting\": \"Hello World\",\n  \"count\": 42,\n  \"active\": true,\n  \"items\": [1, 2, 3]\n}";
            JsonInput.TextChanged += OnJsonInputChanged;
        }

        UpdateInteractiveTree();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (SearchJsonTree != null && SearchInput != null)
        {
            SearchJsonTree.SearchText = SearchInput.Text ?? "";
        }
    }

    private void OnJsonInputChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateInteractiveTree();
    }

    private void UpdateInteractiveTree()
    {
        if (InteractiveJsonTree != null && JsonInput != null)
        {
            InteractiveJsonTree.JsonText = JsonInput.Text ?? "";
        }
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
