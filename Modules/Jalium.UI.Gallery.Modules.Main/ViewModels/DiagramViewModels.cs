using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class GanttChartViewModel
{
    public GanttChartViewModel()
    {
        var april = new DateTime(2026, 4, 1);
        BasicTasks = Tasks(
            Task("planning", "Planning", april, 0, 5),
            Task("design", "UI Design", april, 3, 10),
            Task("backend", "Backend Dev", april, 6, 18),
            Task("frontend", "Frontend Dev", april, 10, 22),
            Task("testing", "Testing", april, 18, 28),
            Task("deployment", "Deployment", april, 26, 30));

        DependencyTasks = Tasks(
            Task("req", "Requirements", april, 0, 4),
            Task("arch", "Architecture", april, 4, 8, "req"),
            Task("proto", "Prototype", april, 8, 14, "arch"),
            Task(
                "review_ms",
                "Design Review",
                april,
                14,
                14,
                true,
                "proto"),
            Task(
                "impl",
                "Implementation",
                april,
                15,
                25,
                "review_ms"),
            Task("qa", "QA Testing", april, 22, 30, "impl"),
            Task(
                "release_ms",
                "Release",
                april,
                30,
                30,
                true,
                "qa"));

        var march = new DateTime(2026, 3, 15);
        ProgressTasks = Tasks(
            Task("research", "Research", march, 0, 7, 1),
            Task(
                "wireframes",
                "Wireframes",
                march,
                5,
                12,
                0.9,
                "research"),
            Task(
                "db_schema",
                "DB Schema",
                march,
                7,
                14,
                0.75,
                "research"),
            Task(
                "api_dev",
                "API Development",
                march,
                12,
                24,
                0.4,
                "db_schema"),
            Task(
                "ui_dev",
                "UI Development",
                march,
                12,
                26,
                0.3,
                "wireframes"),
            Task(
                "integration",
                "Integration",
                march,
                22,
                30,
                0,
                "api_dev",
                "ui_dev"),
            Task(
                "launch",
                "Launch",
                march,
                30,
                30,
                true,
                0,
                "integration"));
    }

    public ObservableCollection<GanttTask> BasicTasks { get; }

    public ObservableCollection<GanttTask> DependencyTasks { get; }

    public ObservableCollection<GanttTask> ProgressTasks { get; }

    private static ObservableCollection<GanttTask> Tasks(
        params GanttTask[] tasks) =>
        new(tasks);

    private static GanttTask Task(
        string id,
        string name,
        DateTime start,
        int startOffset,
        int endOffset,
        params string[] dependencies) =>
        Task(
            id,
            name,
            start,
            startOffset,
            endOffset,
            false,
            0,
            dependencies);

    private static GanttTask Task(
        string id,
        string name,
        DateTime start,
        int startOffset,
        int endOffset,
        bool milestone,
        params string[] dependencies) =>
        Task(
            id,
            name,
            start,
            startOffset,
            endOffset,
            milestone,
            0,
            dependencies);

    private static GanttTask Task(
        string id,
        string name,
        DateTime start,
        int startOffset,
        int endOffset,
        double progress,
        params string[] dependencies) =>
        Task(
            id,
            name,
            start,
            startOffset,
            endOffset,
            false,
            progress,
            dependencies);

    private static GanttTask Task(
        string id,
        string name,
        DateTime start,
        int startOffset,
        int endOffset,
        bool milestone,
        double progress,
        params string[] dependencies) =>
        new()
        {
            Id = id,
            Name = name,
            StartDate = start.AddDays(startOffset),
            EndDate = start.AddDays(endOffset),
            IsMilestone = milestone,
            Progress = progress,
            DependsOn = dependencies.ToList()
        };
}

public sealed partial class NetworkGraphViewModel
{
    public NetworkGraphViewModel()
    {
        ForceNodes = Nodes(
            Node("alice", "Alice", "#417EE0"),
            Node("bob", "Bob", "#4CAF50"),
            Node("carol", "Carol", "#FFC107"),
            Node("dave", "Dave", "#E0593E"),
            Node("eve", "Eve", "#9C5FC4"),
            Node("frank", "Frank", "#009688"),
            Node("grace", "Grace", "#795548"),
            Node("heidi", "Heidi", "#607D8B"));
        ForceLinks = Links(
            ("alice", "bob"), ("alice", "carol"),
            ("bob", "dave"), ("carol", "dave"),
            ("carol", "eve"), ("dave", "frank"),
            ("eve", "frank"), ("frank", "grace"),
            ("grace", "heidi"), ("heidi", "alice"),
            ("bob", "eve"));

        var moduleNames = new[]
        {
            "Core", "UI", "Data", "Auth", "API",
            "Cache", "Logger", "Config", "Tests", "CLI"
        };
        CircularNodes = Nodes(moduleNames
            .Select((name, index) =>
                Node(
                    name.ToLowerInvariant(),
                    name,
                    Palette[index % Palette.Length]))
            .ToArray());
        CircularLinks = Links(
            ("ui", "core"), ("data", "core"),
            ("auth", "core"), ("api", "auth"),
            ("api", "data"), ("cache", "data"),
            ("logger", "core"), ("config", "core"),
            ("tests", "core"), ("tests", "api"),
            ("cli", "api"), ("cli", "config"),
            ("ui", "api"));

        InteractiveNodes = Nodes(
            Node("server", "Server", "#E0593E", 20),
            Node("db", "Database", "#417EE0", 18),
            Node("cache", "Cache", "#FFC107", 14),
            Node("lb", "Load Balancer", "#4CAF50", 18),
            Node("web1", "Web 1", "#9C5FC4"),
            Node("web2", "Web 2", "#9C5FC4"),
            Node("worker", "Worker", "#009688"),
            Node("queue", "Queue", "#FF7043", 14));
        InteractiveLinks = Links(
            ("lb", "web1"), ("lb", "web2"),
            ("web1", "server"), ("web2", "server"),
            ("server", "db"), ("server", "cache"),
            ("server", "queue"), ("queue", "worker"),
            ("worker", "db"));
    }

    private static readonly string[] Palette =
    {
        "#417EE0", "#4CAF50", "#FFC107", "#E0593E",
        "#9C5FC4", "#009688", "#795548", "#607D8B",
        "#AFB42B", "#FF7043"
    };

    public ObservableCollection<NetworkNode> ForceNodes { get; }

    public ObservableCollection<NetworkLink> ForceLinks { get; }

    public ObservableCollection<NetworkNode> CircularNodes { get; }

    public ObservableCollection<NetworkLink> CircularLinks { get; }

    public ObservableCollection<NetworkNode> InteractiveNodes { get; }

    public ObservableCollection<NetworkLink> InteractiveLinks { get; }

    private static ObservableCollection<NetworkNode> Nodes(
        params NetworkNode[] nodes) =>
        new(nodes);

    private static ObservableCollection<NetworkLink> Links(
        params (string Source, string Target)[] links) =>
        new(links.Select(link => new NetworkLink
        {
            SourceId = link.Source,
            TargetId = link.Target
        }));

    private static NetworkNode Node(
        string id,
        string label,
        string color,
        double radius = 0) =>
        new()
        {
            Id = id,
            Label = label,
            Brush = Brush(color),
            Radius = radius
        };

    private static SolidColorBrush Brush(string color) =>
        new(ParseColor(color));

    private static Color ParseColor(string color)
    {
        var value = Convert.ToUInt32(
            color.TrimStart('#'),
            16);
        return color.Length > 7
            ? Color.FromArgb(
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value)
            : Color.FromRgb(
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value);
    }
}

public sealed partial class SankeyDiagramViewModel
{
    public SankeyDiagramViewModel()
    {
        BasicNodes = Nodes(
            ("coal", "Coal"), ("gas", "Natural Gas"),
            ("solar", "Solar"), ("wind", "Wind"),
            ("electricity", "Electricity"), ("heat", "Heat"),
            ("residential", "Residential"),
            ("industrial", "Industrial"),
            ("transport", "Transport"));
        BasicLinks = Links(
            ("coal", "electricity", 40), ("coal", "heat", 15),
            ("gas", "electricity", 30), ("gas", "heat", 25),
            ("solar", "electricity", 20), ("wind", "electricity", 15),
            ("electricity", "residential", 45),
            ("electricity", "industrial", 40),
            ("electricity", "transport", 20),
            ("heat", "residential", 25),
            ("heat", "industrial", 15));

        TrafficNodes = Nodes(
            ("organic", "Organic Search"), ("paid", "Paid Ads"),
            ("social", "Social Media"), ("direct", "Direct"),
            ("home", "Home Page"), ("product", "Product Page"),
            ("blog", "Blog"), ("signup", "Sign Up"),
            ("cart", "Add to Cart"), ("bounce", "Bounce"),
            ("purchase", "Purchase"), ("abandon", "Abandon"));
        TrafficLinks = Links(
            ("organic", "home", 300), ("organic", "blog", 200),
            ("paid", "product", 400), ("paid", "home", 100),
            ("social", "blog", 150), ("social", "product", 100),
            ("direct", "home", 200), ("home", "signup", 250),
            ("home", "cart", 150), ("home", "bounce", 200),
            ("product", "cart", 350), ("product", "bounce", 150),
            ("blog", "signup", 200), ("blog", "bounce", 150),
            ("signup", "purchase", 180),
            ("signup", "abandon", 270),
            ("cart", "purchase", 320),
            ("cart", "abandon", 180));

        BudgetNodes = Nodes(
            ("budget", "Total Budget"), ("eng", "Engineering"),
            ("mkt", "Marketing"), ("ops", "Operations"),
            ("salary", "Salaries"), ("tools", "Tools & Infra"),
            ("ads", "Advertising"),
            ("office", "Office & Facilities"));
        BudgetLinks = Links(
            ("budget", "eng", 500), ("budget", "mkt", 300),
            ("budget", "ops", 200), ("eng", "salary", 350),
            ("eng", "tools", 150), ("mkt", "salary", 120),
            ("mkt", "ads", 180), ("ops", "salary", 100),
            ("ops", "office", 100));
    }

    public ObservableCollection<SankeyNode> BasicNodes { get; }

    public ObservableCollection<SankeyLink> BasicLinks { get; }

    public ObservableCollection<SankeyNode> TrafficNodes { get; }

    public ObservableCollection<SankeyLink> TrafficLinks { get; }

    public ObservableCollection<SankeyNode> BudgetNodes { get; }

    public ObservableCollection<SankeyLink> BudgetLinks { get; }

    private static ObservableCollection<SankeyNode> Nodes(
        params (string Id, string Label)[] nodes) =>
        new(nodes.Select(node => new SankeyNode
        {
            Id = node.Id,
            Label = node.Label
        }));

    private static ObservableCollection<SankeyLink> Links(
        params (string Source, string Target, double Value)[] links) =>
        new(links.Select(link => new SankeyLink
        {
            SourceId = link.Source,
            TargetId = link.Target,
            Value = link.Value
        }));
}

public sealed partial class TreeMapViewModel
{
    public TreeMapViewModel()
    {
        MarketItems = Items(
            Item("Technology", 320, "#417EE0"),
            Item("Healthcare", 210, "#4CAF50"),
            Item("Finance", 180, "#FFC107"),
            Item("Energy", 140, "#E0593E"),
            Item("Consumer", 120, "#9C5FC4"),
            Item("Industrial", 95, "#009688"),
            Item("Telecom", 70, "#795548"),
            Item("Utilities", 50, "#607D8B"));

        NestedItems = Items(
            Parent(
                "Documents",
                "#417EE0",
                ("Reports", 45),
                ("Spreadsheets", 32),
                ("Presentations", 28)),
            Parent(
                "Media",
                "#4CAF50",
                ("Videos", 120),
                ("Music", 60),
                ("Photos", 80)),
            Parent(
                "Code",
                "#FFC107",
                ("Source", 55),
                ("Dependencies", 90),
                ("Build Output", 35)),
            Item("System", 70, "#9C5FC4"));

        SquarifiedItems = ComparisonItems();
        SliceAndDiceItems = ComparisonItems();
    }

    public ObservableCollection<TreeMapItem> MarketItems { get; }

    public ObservableCollection<TreeMapItem> NestedItems { get; }

    public ObservableCollection<TreeMapItem> SquarifiedItems { get; }

    public ObservableCollection<TreeMapItem> SliceAndDiceItems { get; }

    private static ObservableCollection<TreeMapItem> ComparisonItems() =>
        Items(
            Item("A", 100, "#417EE0"),
            Item("B", 80, "#4CAF50"),
            Item("C", 60, "#FFC107"),
            Item("D", 40, "#E0593E"),
            Item("E", 30, "#9C5FC4"),
            Item("F", 20, "#009688"),
            Item("G", 15, "#795548"),
            Item("H", 10, "#607D8B"));

    private static ObservableCollection<TreeMapItem> Items(
        params TreeMapItem[] items) =>
        new(items);

    private static TreeMapItem Item(
        string label,
        double value,
        string color) =>
        new()
        {
            Label = label,
            Value = value,
            Brush = new SolidColorBrush(ParseColor(color))
        };

    private static TreeMapItem Parent(
        string label,
        string color,
        params (string Label, double Value)[] children)
    {
        var item = Item(label, 0, color);
        foreach (var child in children)
        {
            item.Children.Add(new TreeMapItem
            {
                Label = child.Label,
                Value = child.Value
            });
        }

        return item;
    }

    private static Color ParseColor(string color)
    {
        var value = Convert.ToUInt32(
            color.TrimStart('#'),
            16);
        return Color.FromRgb(
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value);
    }
}

public sealed partial class MiniMapViewModel
{
    public MiniMapViewModel()
    {
        BasicCells = CreateCells(150, 5);
        RenderModeCells = CreateCells(32, 4);
        Markers = new MapMarkerCollection
        {
            new()
            {
                Location = new GeoPoint(39.9042, 116.4074),
                Label = "Beijing",
                Fill = Brush("#E03E3E"),
                MarkerSize = 12
            },
            new()
            {
                Location = new GeoPoint(31.2304, 121.4737),
                Label = "Shanghai",
                Fill = Brush("#2196F3"),
                MarkerSize = 12
            },
            new()
            {
                Location = new GeoPoint(23.1291, 113.2644),
                Label = "Guangzhou",
                Fill = Brush("#4CAF50"),
                MarkerSize = 12
            }
        };
        HighlightMarkers = new ObservableCollection<MiniMapMarker>
        {
            new() { X = 0.7, Y = 0.4, Color = Brush("#E03E3E"), Size = 6 },
            new() { X = 0.2, Y = 0.5, Color = Brush("#2196F3"), Size = 6 },
            new() { X = 0.55, Y = 0.35, Color = Brush("#4CAF50"), Size = 6 }
        };
    }

    public IReadOnlyList<MiniMapCell> BasicCells { get; }

    public IReadOnlyList<MiniMapCell> RenderModeCells { get; }

    public MapMarkerCollection Markers { get; }

    public ObservableCollection<MiniMapMarker> HighlightMarkers { get; }

    private static IReadOnlyList<MiniMapCell> CreateCells(
        int count,
        int columns) =>
        Enumerable.Range(0, count)
            .Select(index =>
            {
                var row = index / columns;
                var column = index % columns;
                return new MiniMapCell(
                    $"Item {index + 1}",
                    new SolidColorBrush(Color.FromRgb(
                        (byte)(40 + row * 7 % 60),
                        (byte)(40 + column * 13 % 60),
                        (byte)(60 + (row + column) * 11 % 80))));
            })
            .ToArray();

    private static SolidColorBrush Brush(string color) =>
        new(ParseColor(color));

    private static Color ParseColor(string color)
    {
        var value = Convert.ToUInt32(
            color.TrimStart('#'),
            16);
        return Color.FromRgb(
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value);
    }
}
