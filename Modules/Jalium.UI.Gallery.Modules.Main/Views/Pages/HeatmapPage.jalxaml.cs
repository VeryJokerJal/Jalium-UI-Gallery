using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class HeatmapPage : Page
{
    private const string XamlExample =
@"<Heatmap Width=""600"" Height=""400""
         Title=""10x10 Matrix""
         ColorScale=""BlueToRed""
         ShowCellValues=""False"">
    <Heatmap.XLabels>
        <x:String>C0</x:String>
        <x:String>C1</x:String>
        <x:String>C2</x:String>
    </Heatmap.XLabels>
    <Heatmap.YLabels>
        <x:String>R0</x:String>
        <x:String>R1</x:String>
        <x:String>R2</x:String>
    </Heatmap.YLabels>
</Heatmap>

<!-- With cell values displayed -->
<Heatmap ShowCellValues=""True""
         CellValueFormat=""F1""
         ColorScale=""Viridis""/>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;

// Generate sample data
var data = new double[10, 10];
var rng = new Random(42);
for (int r = 0; r < 10; r++)
    for (int c = 0; c < 10; c++)
        data[r, c] = Math.Sin((double)c / 10 * 6)
                   * Math.Cos((double)r / 10 * 6) * 50
                   + rng.NextDouble() * 20;

var xLabels = new List<string>();
var yLabels = new List<string>();
for (int i = 0; i < 10; i++)
{
    xLabels.Add($""C{i}"");
    yLabels.Add($""R{i}"");
}

var heatmap = new Heatmap
{
    Width = 600,
    Height = 400,
    Data = data,
    XLabels = xLabels,
    YLabels = yLabels,
    ColorScale = HeatmapColorScale.BlueToRed,
    Title = ""10x10 Matrix""
};

// Show numeric values inside cells
heatmap.ShowCellValues = true;
heatmap.CellValueFormat = ""F1"";

// Switch color scale
heatmap.ColorScale = HeatmapColorScale.Viridis;";

    public HeatmapPage()
    {
        InitializeComponent();
        CreateBasicHeatmap();
        CreateColorScales();
        CreateCellValuesHeatmap();
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

    private static double[,] GenerateSampleData(int rows, int cols, int seed)
    {
        var rng = new Random(seed);
        var data = new double[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                double cx = (double)c / cols - 0.5;
                double cy = (double)r / rows - 0.5;
                data[r, c] = Math.Sin(cx * 6) * Math.Cos(cy * 6) * 50 + rng.NextDouble() * 20;
            }
        }
        return data;
    }

    private void CreateBasicHeatmap()
    {
        var data = GenerateSampleData(10, 10, 42);

        var xLabels = new List<string>();
        var yLabels = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            xLabels.Add($"C{i}");
            yLabels.Add($"R{i}");
        }

        var heatmap = new Heatmap
        {
            Width = 600,
            Height = 400,
            Data = data,
            XLabels = xLabels,
            YLabels = yLabels,
            ColorScale = HeatmapColorScale.BlueToRed,
            Title = "10x10 Matrix"
        };

        if (BasicHeatmapContainer != null)
            BasicHeatmapContainer.Child = heatmap;
    }

    private void CreateColorScales()
    {
        var data = GenerateSampleData(8, 8, 99);

        var panel = new StackPanel { Orientation = Orientation.Horizontal };

        var viridis = new Heatmap
        {
            Width = 290,
            Height = 300,
            Data = data,
            ColorScale = HeatmapColorScale.Viridis,
            Title = "Viridis"
        };

        var grayscale = new Heatmap
        {
            Width = 290,
            Height = 300,
            Data = data,
            ColorScale = HeatmapColorScale.Grayscale,
            Title = "Grayscale",
            Margin = new Thickness(16, 0, 0, 0)
        };

        panel.Children.Add(viridis);
        panel.Children.Add(grayscale);

        if (ColorScalesContainer != null)
            ColorScalesContainer.Child = panel;
    }

    private void CreateCellValuesHeatmap()
    {
        var rng = new Random(55);
        var data = new double[6, 6];
        for (int r = 0; r < 6; r++)
        {
            for (int c = 0; c < 6; c++)
            {
                data[r, c] = Math.Round(rng.NextDouble() * 100, 1);
            }
        }

        var xLabels = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        var yLabels = new List<string> { "Week 1", "Week 2", "Week 3", "Week 4", "Week 5", "Week 6" };

        var heatmap = new Heatmap
        {
            Width = 600,
            Height = 400,
            Data = data,
            XLabels = xLabels,
            YLabels = yLabels,
            ShowCellValues = true,
            CellValueFormat = "F1",
            ColorScale = HeatmapColorScale.BlueToRed,
            Title = "Weekly Activity"
        };

        if (CellValuesContainer != null)
            CellValuesContainer.Child = heatmap;
    }
}
