using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class TreeMapPage : Page
{
    private const string XamlExample =
@"<TreeMap Width=""600"" Height=""400""
         Algorithm=""Squarified""
         ShowLabels=""True""
         Title=""Market Sectors"">
    <TreeMap.Items>
        <TreeMapItem Label=""Technology"" Value=""320"" Brush=""#417EE0""/>
        <TreeMapItem Label=""Healthcare"" Value=""210"" Brush=""#4CAF50""/>
        <TreeMapItem Label=""Finance""    Value=""180"" Brush=""#FFC107""/>
        <TreeMapItem Label=""Energy""     Value=""140"" Brush=""#E0593E""/>
        <TreeMapItem Label=""Consumer""   Value=""120"" Brush=""#9C5FC4""/>
    </TreeMap.Items>
</TreeMap>

<!-- Nested hierarchical tree map -->
<TreeMap Algorithm=""Squarified"" Title=""Disk Usage"">
    <TreeMap.Items>
        <TreeMapItem Label=""Documents"" Brush=""#417EE0"">
            <TreeMapItem.Children>
                <TreeMapItem Label=""Reports"" Value=""45""/>
                <TreeMapItem Label=""Spreadsheets"" Value=""32""/>
            </TreeMapItem.Children>
        </TreeMapItem>
    </TreeMap.Items>
</TreeMap>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;
using System.Collections.ObjectModel;

// Create a flat tree map
var treeMap = new TreeMap
{
    Width = 600,
    Height = 400,
    Algorithm = TreeMapAlgorithm.Squarified,
    ShowLabels = true,
    Title = ""Market Sectors""
};

treeMap.Items = new ObservableCollection<TreeMapItem>
{
    new TreeMapItem { Label = ""Technology"", Value = 320,
        Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)) },
    new TreeMapItem { Label = ""Healthcare"", Value = 210,
        Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)) },
    new TreeMapItem { Label = ""Finance"",    Value = 180,
        Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)) }
};

// Nested hierarchy
var docs = new TreeMapItem
{
    Label = ""Documents"",
    Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0))
};
docs.Children.Add(new TreeMapItem { Label = ""Reports"", Value = 45 });
docs.Children.Add(new TreeMapItem { Label = ""Spreadsheets"", Value = 32 });

// Switch layout algorithm
treeMap.Algorithm = TreeMapAlgorithm.SliceAndDice;";

    public TreeMapPage()
    {
        InitializeComponent();
        CreateBasicTreeMap();
        CreateNestedTreeMap();
        CreateAlgorithmComparison();
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

    private void CreateBasicTreeMap()
    {
        var treeMap = new TreeMap
        {
            Width = 600,
            Height = 400,
            Algorithm = TreeMapAlgorithm.Squarified,
            ShowLabels = true,
            Title = "Market Sectors"
        };

        var items = new ObservableCollection<TreeMapItem>
        {
            new TreeMapItem { Label = "Technology", Value = 320, Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)) },
            new TreeMapItem { Label = "Healthcare", Value = 210, Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)) },
            new TreeMapItem { Label = "Finance", Value = 180, Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)) },
            new TreeMapItem { Label = "Energy", Value = 140, Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E)) },
            new TreeMapItem { Label = "Consumer", Value = 120, Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4)) },
            new TreeMapItem { Label = "Industrial", Value = 95, Brush = new SolidColorBrush(Color.FromRgb(0x00, 0x96, 0x88)) },
            new TreeMapItem { Label = "Telecom", Value = 70, Brush = new SolidColorBrush(Color.FromRgb(0x79, 0x55, 0x48)) },
            new TreeMapItem { Label = "Utilities", Value = 50, Brush = new SolidColorBrush(Color.FromRgb(0x60, 0x7D, 0x8B)) }
        };

        treeMap.Items = items;

        if (BasicTreeMapContainer != null)
            BasicTreeMapContainer.Child = treeMap;
    }

    private void CreateNestedTreeMap()
    {
        var treeMap = new TreeMap
        {
            Width = 600,
            Height = 400,
            Algorithm = TreeMapAlgorithm.Squarified,
            ShowLabels = true,
            Title = "Disk Usage"
        };

        var documents = new TreeMapItem
        {
            Label = "Documents",
            Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0))
        };
        documents.Children.Add(new TreeMapItem { Label = "Reports", Value = 45 });
        documents.Children.Add(new TreeMapItem { Label = "Spreadsheets", Value = 32 });
        documents.Children.Add(new TreeMapItem { Label = "Presentations", Value = 28 });

        var media = new TreeMapItem
        {
            Label = "Media",
            Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50))
        };
        media.Children.Add(new TreeMapItem { Label = "Videos", Value = 120 });
        media.Children.Add(new TreeMapItem { Label = "Music", Value = 60 });
        media.Children.Add(new TreeMapItem { Label = "Photos", Value = 80 });

        var code = new TreeMapItem
        {
            Label = "Code",
            Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07))
        };
        code.Children.Add(new TreeMapItem { Label = "Source", Value = 55 });
        code.Children.Add(new TreeMapItem { Label = "Dependencies", Value = 90 });
        code.Children.Add(new TreeMapItem { Label = "Build Output", Value = 35 });

        var system = new TreeMapItem
        {
            Label = "System",
            Value = 70,
            Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4))
        };

        treeMap.Items = new ObservableCollection<TreeMapItem> { documents, media, code, system };

        if (NestedTreeMapContainer != null)
            NestedTreeMapContainer.Child = treeMap;
    }

    private void CreateAlgorithmComparison()
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal };

        var items = CreateComparisonItems();

        var squarified = new TreeMap
        {
            Width = 290,
            Height = 300,
            Algorithm = TreeMapAlgorithm.Squarified,
            ShowLabels = true,
            Title = "Squarified",
            Items = items
        };

        var sliceAndDice = new TreeMap
        {
            Width = 290,
            Height = 300,
            Algorithm = TreeMapAlgorithm.SliceAndDice,
            ShowLabels = true,
            Title = "Slice & Dice",
            Items = CreateComparisonItems(),
            Margin = new Thickness(16, 0, 0, 0)
        };

        panel.Children.Add(squarified);
        panel.Children.Add(sliceAndDice);

        if (AlgorithmContainer != null)
            AlgorithmContainer.Child = panel;
    }

    private static ObservableCollection<TreeMapItem> CreateComparisonItems()
    {
        return new ObservableCollection<TreeMapItem>
        {
            new TreeMapItem { Label = "A", Value = 100, Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)) },
            new TreeMapItem { Label = "B", Value = 80, Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)) },
            new TreeMapItem { Label = "C", Value = 60, Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)) },
            new TreeMapItem { Label = "D", Value = 40, Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E)) },
            new TreeMapItem { Label = "E", Value = 30, Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4)) },
            new TreeMapItem { Label = "F", Value = 20, Brush = new SolidColorBrush(Color.FromRgb(0x00, 0x96, 0x88)) },
            new TreeMapItem { Label = "G", Value = 15, Brush = new SolidColorBrush(Color.FromRgb(0x79, 0x55, 0x48)) },
            new TreeMapItem { Label = "H", Value = 10, Brush = new SolidColorBrush(Color.FromRgb(0x60, 0x7D, 0x8B)) }
        };
    }
}
