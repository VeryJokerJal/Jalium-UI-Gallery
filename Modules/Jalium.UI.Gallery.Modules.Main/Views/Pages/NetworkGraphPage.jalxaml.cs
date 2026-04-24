using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class NetworkGraphPage : Page
{
    private const string XamlExample = @"<!-- Force-Directed Layout -->
<NetworkGraph x:Name=""ForceGraph""
              Width=""600""
              Height=""450""
              LayoutAlgorithm=""ForceDirected""
              ShowLabels=""True""
              NodeRadius=""12""
              IsNodeDraggable=""False""
              Title=""Social Network""/>

<!-- Circular Layout -->
<NetworkGraph x:Name=""CircularGraph""
              Width=""600""
              Height=""450""
              LayoutAlgorithm=""Circular""
              ShowLabels=""True""
              NodeRadius=""14""
              IsNodeDraggable=""False""
              Title=""Module Dependencies""/>

<!-- Interactive (Draggable) -->
<NetworkGraph x:Name=""InteractiveGraph""
              Width=""600""
              Height=""450""
              LayoutAlgorithm=""ForceDirected""
              ShowLabels=""True""
              NodeRadius=""16""
              IsNodeDraggable=""True""
              Title=""Drag Nodes to Rearrange""/>";

    private const string CSharpExample = @"using System.Collections.ObjectModel;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;

// Create nodes
var nodes = new ObservableCollection<NetworkNode>
{
    new NetworkNode
    {
        Id = ""alice"",
        Label = ""Alice"",
        Brush = new SolidColorBrush(
            Color.FromRgb(0x41, 0x7E, 0xE0))
    },
    new NetworkNode
    {
        Id = ""bob"",
        Label = ""Bob"",
        Brush = new SolidColorBrush(
            Color.FromRgb(0x4C, 0xAF, 0x50))
    }
};

// Create links
var links = new ObservableCollection<NetworkLink>
{
    new NetworkLink
    {
        SourceId = ""alice"",
        TargetId = ""bob""
    }
};

// Assign to graph
ForceGraph.Nodes = nodes;
ForceGraph.Links = links;

// Interactive nodes can have custom radius
var serverNode = new NetworkNode
{
    Id = ""server"",
    Label = ""Server"",
    Radius = 20,
    Brush = new SolidColorBrush(
        Color.FromRgb(0xE0, 0x59, 0x3E))
};";

    public NetworkGraphPage()
    {
        InitializeComponent();
        CreateForceDirectedGraph();
        CreateCircularGraph();
        CreateInteractiveGraph();
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

    private void CreateForceDirectedGraph()
    {
        var graph = new NetworkGraph
        {
            Width = 600,
            Height = 450,
            LayoutAlgorithm = NetworkLayoutAlgorithm.ForceDirected,
            ShowLabels = true,
            NodeRadius = 12,
            IsNodeDraggable = false,
            Title = "Social Network"
        };

        var nodes = new ObservableCollection<NetworkNode>
        {
            new NetworkNode { Id = "alice", Label = "Alice", Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)) },
            new NetworkNode { Id = "bob", Label = "Bob", Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)) },
            new NetworkNode { Id = "carol", Label = "Carol", Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)) },
            new NetworkNode { Id = "dave", Label = "Dave", Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E)) },
            new NetworkNode { Id = "eve", Label = "Eve", Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4)) },
            new NetworkNode { Id = "frank", Label = "Frank", Brush = new SolidColorBrush(Color.FromRgb(0x00, 0x96, 0x88)) },
            new NetworkNode { Id = "grace", Label = "Grace", Brush = new SolidColorBrush(Color.FromRgb(0x79, 0x55, 0x48)) },
            new NetworkNode { Id = "heidi", Label = "Heidi", Brush = new SolidColorBrush(Color.FromRgb(0x60, 0x7D, 0x8B)) }
        };

        var links = new ObservableCollection<NetworkLink>
        {
            new NetworkLink { SourceId = "alice", TargetId = "bob" },
            new NetworkLink { SourceId = "alice", TargetId = "carol" },
            new NetworkLink { SourceId = "bob", TargetId = "dave" },
            new NetworkLink { SourceId = "carol", TargetId = "dave" },
            new NetworkLink { SourceId = "carol", TargetId = "eve" },
            new NetworkLink { SourceId = "dave", TargetId = "frank" },
            new NetworkLink { SourceId = "eve", TargetId = "frank" },
            new NetworkLink { SourceId = "frank", TargetId = "grace" },
            new NetworkLink { SourceId = "grace", TargetId = "heidi" },
            new NetworkLink { SourceId = "heidi", TargetId = "alice" },
            new NetworkLink { SourceId = "bob", TargetId = "eve" }
        };

        graph.Nodes = nodes;
        graph.Links = links;

        if (ForceDirectedContainer != null)
            ForceDirectedContainer.Child = graph;
    }

    private void CreateCircularGraph()
    {
        var graph = new NetworkGraph
        {
            Width = 600,
            Height = 450,
            LayoutAlgorithm = NetworkLayoutAlgorithm.Circular,
            ShowLabels = true,
            NodeRadius = 14,
            IsNodeDraggable = false,
            Title = "Module Dependencies"
        };

        var moduleNames = new[] { "Core", "UI", "Data", "Auth", "API", "Cache", "Logger", "Config", "Tests", "CLI" };
        var nodes = new ObservableCollection<NetworkNode>();

        var colors = new[]
        {
            Color.FromRgb(0x41, 0x7E, 0xE0),
            Color.FromRgb(0x4C, 0xAF, 0x50),
            Color.FromRgb(0xFF, 0xC1, 0x07),
            Color.FromRgb(0xE0, 0x59, 0x3E),
            Color.FromRgb(0x9C, 0x5F, 0xC4),
            Color.FromRgb(0x00, 0x96, 0x88),
            Color.FromRgb(0x79, 0x55, 0x48),
            Color.FromRgb(0x60, 0x7D, 0x8B),
            Color.FromRgb(0xAF, 0xB4, 0x2B),
            Color.FromRgb(0xFF, 0x70, 0x43)
        };

        for (int i = 0; i < moduleNames.Length; i++)
        {
            nodes.Add(new NetworkNode
            {
                Id = moduleNames[i].ToLowerInvariant(),
                Label = moduleNames[i],
                Brush = new SolidColorBrush(colors[i])
            });
        }

        var links = new ObservableCollection<NetworkLink>
        {
            new NetworkLink { SourceId = "ui", TargetId = "core" },
            new NetworkLink { SourceId = "data", TargetId = "core" },
            new NetworkLink { SourceId = "auth", TargetId = "core" },
            new NetworkLink { SourceId = "api", TargetId = "auth" },
            new NetworkLink { SourceId = "api", TargetId = "data" },
            new NetworkLink { SourceId = "cache", TargetId = "data" },
            new NetworkLink { SourceId = "logger", TargetId = "core" },
            new NetworkLink { SourceId = "config", TargetId = "core" },
            new NetworkLink { SourceId = "tests", TargetId = "core" },
            new NetworkLink { SourceId = "tests", TargetId = "api" },
            new NetworkLink { SourceId = "cli", TargetId = "api" },
            new NetworkLink { SourceId = "cli", TargetId = "config" },
            new NetworkLink { SourceId = "ui", TargetId = "api" }
        };

        graph.Nodes = nodes;
        graph.Links = links;

        if (CircularContainer != null)
            CircularContainer.Child = graph;
    }

    private void CreateInteractiveGraph()
    {
        var graph = new NetworkGraph
        {
            Width = 600,
            Height = 450,
            LayoutAlgorithm = NetworkLayoutAlgorithm.ForceDirected,
            ShowLabels = true,
            NodeRadius = 16,
            IsNodeDraggable = true,
            Title = "Drag Nodes to Rearrange"
        };

        var nodes = new ObservableCollection<NetworkNode>
        {
            new NetworkNode { Id = "server", Label = "Server", Radius = 20, Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E)) },
            new NetworkNode { Id = "db", Label = "Database", Radius = 18, Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)) },
            new NetworkNode { Id = "cache", Label = "Cache", Radius = 14, Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)) },
            new NetworkNode { Id = "lb", Label = "Load Balancer", Radius = 18, Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)) },
            new NetworkNode { Id = "web1", Label = "Web 1", Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4)) },
            new NetworkNode { Id = "web2", Label = "Web 2", Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4)) },
            new NetworkNode { Id = "worker", Label = "Worker", Brush = new SolidColorBrush(Color.FromRgb(0x00, 0x96, 0x88)) },
            new NetworkNode { Id = "queue", Label = "Queue", Radius = 14, Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x70, 0x43)) }
        };

        var links = new ObservableCollection<NetworkLink>
        {
            new NetworkLink { SourceId = "lb", TargetId = "web1" },
            new NetworkLink { SourceId = "lb", TargetId = "web2" },
            new NetworkLink { SourceId = "web1", TargetId = "server" },
            new NetworkLink { SourceId = "web2", TargetId = "server" },
            new NetworkLink { SourceId = "server", TargetId = "db" },
            new NetworkLink { SourceId = "server", TargetId = "cache" },
            new NetworkLink { SourceId = "server", TargetId = "queue" },
            new NetworkLink { SourceId = "queue", TargetId = "worker" },
            new NetworkLink { SourceId = "worker", TargetId = "db" }
        };

        graph.Nodes = nodes;
        graph.Links = links;

        if (InteractiveContainer != null)
            InteractiveContainer.Child = graph;
    }
}
