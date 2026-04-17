using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for SankeyDiagramPage.jalxaml demonstrating SankeyDiagram functionality.
/// </summary>
public partial class SankeyDiagramPage : Page
{
    private const string XamlExample = @"<!-- Basic Sankey Diagram -->
<SankeyDiagram x:Name=""BasicSankey""
               Height=""300""
               ShowLabels=""True""
               ShowValues=""False""/>

<!-- Multi-Layer with Values -->
<SankeyDiagram x:Name=""MultiLayerSankey""
               Height=""320""
               ShowLabels=""True""
               ShowValues=""True""/>

<!-- Customized Appearance -->
<SankeyDiagram x:Name=""CustomSankey""
               Height=""280""
               ShowLabels=""True""
               ShowValues=""True""
               NodeWidth=""24""
               NodeSpacing=""14""
               LinkOpacity=""0.5""
               LabelPosition=""Right""/>";

    private const string CSharpExample = @"using System.Collections.ObjectModel;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;

var diagram = new SankeyDiagram
{
    Height = 300,
    ShowLabels = true,
    ShowValues = true,
    Nodes = new ObservableCollection<SankeyNode>
    {
        new SankeyNode { Id = ""coal"", Label = ""Coal"" },
        new SankeyNode { Id = ""gas"", Label = ""Natural Gas"" },
        new SankeyNode { Id = ""electricity"", Label = ""Electricity"" },
        new SankeyNode { Id = ""residential"", Label = ""Residential"" }
    },
    Links = new ObservableCollection<SankeyLink>
    {
        new SankeyLink
        {
            SourceId = ""coal"",
            TargetId = ""electricity"",
            Value = 40
        },
        new SankeyLink
        {
            SourceId = ""gas"",
            TargetId = ""electricity"",
            Value = 30
        },
        new SankeyLink
        {
            SourceId = ""electricity"",
            TargetId = ""residential"",
            Value = 45
        }
    }
};

// Custom node colors
var customNode = new SankeyNode
{
    Id = ""budget"",
    Label = ""Total Budget"",
    Brush = new SolidColorBrush(
        Color.FromRgb(0x4C, 0xAF, 0x50))
};";

    public SankeyDiagramPage()
    {
        InitializeComponent();
        SetupBasicSankey();
        SetupMultiLayerSankey();
        SetupCustomSankey();
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

    private void SetupBasicSankey()
    {
        if (BasicSankeyContainer == null) return;

        var diagram = new SankeyDiagram
        {
            Height = 300,
            ShowLabels = true,
            ShowValues = false,
            Nodes = new ObservableCollection<SankeyNode>
            {
                new SankeyNode { Id = "coal", Label = "Coal" },
                new SankeyNode { Id = "gas", Label = "Natural Gas" },
                new SankeyNode { Id = "solar", Label = "Solar" },
                new SankeyNode { Id = "wind", Label = "Wind" },
                new SankeyNode { Id = "electricity", Label = "Electricity" },
                new SankeyNode { Id = "heat", Label = "Heat" },
                new SankeyNode { Id = "residential", Label = "Residential" },
                new SankeyNode { Id = "industrial", Label = "Industrial" },
                new SankeyNode { Id = "transport", Label = "Transport" }
            },
            Links = new ObservableCollection<SankeyLink>
            {
                new SankeyLink { SourceId = "coal", TargetId = "electricity", Value = 40 },
                new SankeyLink { SourceId = "coal", TargetId = "heat", Value = 15 },
                new SankeyLink { SourceId = "gas", TargetId = "electricity", Value = 30 },
                new SankeyLink { SourceId = "gas", TargetId = "heat", Value = 25 },
                new SankeyLink { SourceId = "solar", TargetId = "electricity", Value = 20 },
                new SankeyLink { SourceId = "wind", TargetId = "electricity", Value = 15 },
                new SankeyLink { SourceId = "electricity", TargetId = "residential", Value = 45 },
                new SankeyLink { SourceId = "electricity", TargetId = "industrial", Value = 40 },
                new SankeyLink { SourceId = "electricity", TargetId = "transport", Value = 20 },
                new SankeyLink { SourceId = "heat", TargetId = "residential", Value = 25 },
                new SankeyLink { SourceId = "heat", TargetId = "industrial", Value = 15 }
            }
        };

        BasicSankeyContainer.Children.Add(diagram);
    }

    private void SetupMultiLayerSankey()
    {
        if (MultiLayerSankeyContainer == null) return;

        var diagram = new SankeyDiagram
        {
            Height = 320,
            ShowLabels = true,
            ShowValues = true,
            Nodes = new ObservableCollection<SankeyNode>
            {
                // Layer 0: Traffic sources
                new SankeyNode { Id = "organic", Label = "Organic Search" },
                new SankeyNode { Id = "paid", Label = "Paid Ads" },
                new SankeyNode { Id = "social", Label = "Social Media" },
                new SankeyNode { Id = "direct", Label = "Direct" },
                // Layer 1: Landing pages
                new SankeyNode { Id = "home", Label = "Home Page" },
                new SankeyNode { Id = "product", Label = "Product Page" },
                new SankeyNode { Id = "blog", Label = "Blog" },
                // Layer 2: Actions
                new SankeyNode { Id = "signup", Label = "Sign Up" },
                new SankeyNode { Id = "cart", Label = "Add to Cart" },
                new SankeyNode { Id = "bounce", Label = "Bounce" },
                // Layer 3: Outcomes
                new SankeyNode { Id = "purchase", Label = "Purchase" },
                new SankeyNode { Id = "abandon", Label = "Abandon" }
            },
            Links = new ObservableCollection<SankeyLink>
            {
                // Sources -> Landing pages
                new SankeyLink { SourceId = "organic", TargetId = "home", Value = 300 },
                new SankeyLink { SourceId = "organic", TargetId = "blog", Value = 200 },
                new SankeyLink { SourceId = "paid", TargetId = "product", Value = 400 },
                new SankeyLink { SourceId = "paid", TargetId = "home", Value = 100 },
                new SankeyLink { SourceId = "social", TargetId = "blog", Value = 150 },
                new SankeyLink { SourceId = "social", TargetId = "product", Value = 100 },
                new SankeyLink { SourceId = "direct", TargetId = "home", Value = 200 },
                // Landing pages -> Actions
                new SankeyLink { SourceId = "home", TargetId = "signup", Value = 250 },
                new SankeyLink { SourceId = "home", TargetId = "cart", Value = 150 },
                new SankeyLink { SourceId = "home", TargetId = "bounce", Value = 200 },
                new SankeyLink { SourceId = "product", TargetId = "cart", Value = 350 },
                new SankeyLink { SourceId = "product", TargetId = "bounce", Value = 150 },
                new SankeyLink { SourceId = "blog", TargetId = "signup", Value = 200 },
                new SankeyLink { SourceId = "blog", TargetId = "bounce", Value = 150 },
                // Actions -> Outcomes
                new SankeyLink { SourceId = "signup", TargetId = "purchase", Value = 180 },
                new SankeyLink { SourceId = "signup", TargetId = "abandon", Value = 270 },
                new SankeyLink { SourceId = "cart", TargetId = "purchase", Value = 320 },
                new SankeyLink { SourceId = "cart", TargetId = "abandon", Value = 180 }
            }
        };

        MultiLayerSankeyContainer.Children.Add(diagram);
    }

    private void SetupCustomSankey()
    {
        if (CustomSankeyContainer == null) return;

        var diagram = new SankeyDiagram
        {
            Height = 280,
            ShowLabels = true,
            ShowValues = true,
            NodeWidth = 24,
            NodeSpacing = 14,
            LinkOpacity = 0.5,
            LabelPosition = SankeyLabelPosition.Right,
            Nodes = new ObservableCollection<SankeyNode>
            {
                new SankeyNode { Id = "budget", Label = "Total Budget", Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)) },
                new SankeyNode { Id = "eng", Label = "Engineering", Brush = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3)) },
                new SankeyNode { Id = "mkt", Label = "Marketing", Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)) },
                new SankeyNode { Id = "ops", Label = "Operations", Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x27, 0xB0)) },
                new SankeyNode { Id = "salary", Label = "Salaries", Brush = new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36)) },
                new SankeyNode { Id = "tools", Label = "Tools & Infra", Brush = new SolidColorBrush(Color.FromRgb(0x00, 0xBC, 0xD4)) },
                new SankeyNode { Id = "ads", Label = "Advertising", Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x57, 0x22)) },
                new SankeyNode { Id = "office", Label = "Office & Facilities", Brush = new SolidColorBrush(Color.FromRgb(0x79, 0x55, 0x48)) }
            },
            Links = new ObservableCollection<SankeyLink>
            {
                new SankeyLink { SourceId = "budget", TargetId = "eng", Value = 500 },
                new SankeyLink { SourceId = "budget", TargetId = "mkt", Value = 300 },
                new SankeyLink { SourceId = "budget", TargetId = "ops", Value = 200 },
                new SankeyLink { SourceId = "eng", TargetId = "salary", Value = 350 },
                new SankeyLink { SourceId = "eng", TargetId = "tools", Value = 150 },
                new SankeyLink { SourceId = "mkt", TargetId = "salary", Value = 120 },
                new SankeyLink { SourceId = "mkt", TargetId = "ads", Value = 180 },
                new SankeyLink { SourceId = "ops", TargetId = "salary", Value = 100 },
                new SankeyLink { SourceId = "ops", TargetId = "office", Value = 100 }
            }
        };

        CustomSankeyContainer.Children.Add(diagram);
    }
}
