using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for FlowchartPage.jalxaml demonstrating the FlowchartDiagram control and authoring
/// flowcharts from mermaid text via MermaidDiagram.
/// </summary>
public partial class FlowchartPage : Page
{
    private const string FlowchartSource = """
        flowchart TD
            A[开始] --> B{条件判断}
            B -->|是| C[执行操作 A]
            B -->|否| D[执行操作 B]
            C --> E{子条件}
            D --> F[记录日志]
            E -->|通过| G[提交结果]
            E -->|未通过| H[回滚操作]
            H --> D
            F --> G
            G --> I([结束])
        """;

    private const string ShapesSource = """
        flowchart LR
            A[Rectangle] --> B(Rounded)
            B --> C([Stadium])
            C --> D{Decision}
            D -->|yes| E((Circle))
            D -->|no| F{{Hexagon}}
            E --> G[(Database)]
            F -.-> G
        """;

    private const string LiveDefaultSource = """
        flowchart TD
            Start([Start]) --> Input[/Read input/]
            Input --> Check{Valid?}
            Check -->|yes| Save[(Save)]
            Check -->|no| Input
            Save --> Done([Done])
        """;

    private const string XamlExample =
@"<!-- Author a flowchart as mermaid text -->
<MermaidDiagram x:Name=""Diagram"" Height=""460"" />

<!-- Markdown also renders fenced ```mermaid flowcharts automatically -->
<Markdown x:Name=""Doc"" Height=""400"" />";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;

// Easiest: render a flowchart from mermaid text
var diagram = new MermaidDiagram
{
    Height = 460,
    Source = """"""
        flowchart TD
            A[Start] --> B{OK?}
            B -->|yes| C[Done]
            B -->|no| A
        """"""
};
container.Children.Add(diagram);

// Or drive the FlowchartDiagram control directly
var flow = new FlowchartDiagram { Direction = FlowchartDirection.LeftToRight };
flow.Nodes.Add(new FlowchartNode(""A"", ""Start"", FlowchartNodeShape.Stadium));
flow.Nodes.Add(new FlowchartNode(""B"", ""Work"", FlowchartNodeShape.Rectangle));
flow.Nodes.Add(new FlowchartNode(""C"", ""Done"", FlowchartNodeShape.Stadium));
flow.Edges.Add(new FlowchartEdge(""A"", ""B""));
flow.Edges.Add(new FlowchartEdge(""B"", ""C"", label: ""ok""));";

    private MermaidDiagram? _liveDiagram;

    public FlowchartPage()
    {
        InitializeComponent();

        AddDiagram(FlowchartContainer, FlowchartSource, 460);
        AddDiagram(ShapesContainer, ShapesSource, 240);

        SetupLiveEditor();
        LoadCodeExamples();
    }

    private static void AddDiagram(StackPanel? container, string source, double height)
    {
        if (container == null)
        {
            return;
        }

        container.Children.Add(new MermaidDiagram
        {
            Source = source,
            Height = height,
            HorizontalAlignment = HorizontalAlignment.Stretch
        });
    }

    private void SetupLiveEditor()
    {
        if (MermaidInput != null)
        {
            MermaidInput.Text = LiveDefaultSource;
            MermaidInput.TextChanged += OnMermaidInputChanged;
        }

        if (LiveDiagramHost != null)
        {
            _liveDiagram = new MermaidDiagram
            {
                Source = MermaidInput?.Text ?? LiveDefaultSource,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            LiveDiagramHost.Child = _liveDiagram;
        }
    }

    private void OnMermaidInputChanged(object? sender, TextChangedEventArgs e)
    {
        if (_liveDiagram != null && MermaidInput != null)
        {
            _liveDiagram.Source = MermaidInput.Text ?? string.Empty;
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
