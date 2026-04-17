using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class ScatterPlotPage : Page
{
    private const string XamlExample =
@"<ScatterPlot Width=""600"" Height=""400""
             Title=""Random Data Points"">
    <ScatterPlot.Series>
        <ScatterSeries Title=""Dataset A""
                       Brush=""#417EE0""
                       PointShape=""Circle"">
            <ScatterSeries.DataPoints>
                <ChartDataPoint XValue=""23.5"" YValue=""67.2""/>
                <ChartDataPoint XValue=""45.1"" YValue=""34.8""/>
                <ChartDataPoint XValue=""78.9"" YValue=""89.1""/>
            </ScatterSeries.DataPoints>
        </ScatterSeries>
        <ScatterSeries Title=""Dataset B""
                       Brush=""#4CAF50""
                       PointShape=""Triangle""/>
    </ScatterPlot.Series>
</ScatterPlot>

<!-- Bubble chart with size binding -->
<ScatterPlot MinPointSize=""6""
             MaxPointSize=""30""
             SizeBindingPath=""Size""/>

<!-- Trend line overlay -->
<ScatterPlot ShowTrendLine=""True""
             TrendLineType=""Linear""/>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;

// Create a basic scatter plot
var chart = new ScatterPlot { Width = 600, Height = 400 };
var rng = new Random(42);

var series = new ScatterSeries
{
    Title = ""Dataset A"",
    Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
    PointShape = PointShape.Circle
};

for (int i = 0; i < 50; i++)
{
    series.DataPoints.Add(new ChartDataPoint
    {
        XValue = rng.NextDouble() * 100,
        YValue = rng.NextDouble() * 100
    });
}
chart.Series.Add(series);

// Enable bubble sizing
chart.MinPointSize = 6;
chart.MaxPointSize = 30;
chart.SizeBindingPath = ""Size"";

// Add a linear trend line
chart.ShowTrendLine = true;
chart.TrendLineType = TrendLineType.Linear;";

    public ScatterPlotPage()
    {
        InitializeComponent();
        CreateBasicScatterPlot();
        CreateBubbleChart();
        CreateTrendLinePlot();
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

    private void CreateBasicScatterPlot()
    {
        var chart = new ScatterPlot { Width = 600, Height = 400 };
        var rng = new Random(42);

        var series = new ScatterSeries
        {
            Title = "Dataset A",
            Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
            PointShape = PointShape.Circle
        };

        for (int i = 0; i < 50; i++)
        {
            series.DataPoints.Add(new ChartDataPoint
            {
                XValue = rng.NextDouble() * 100,
                YValue = rng.NextDouble() * 100
            });
        }

        var series2 = new ScatterSeries
        {
            Title = "Dataset B",
            Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
            PointShape = PointShape.Triangle
        };

        for (int i = 0; i < 40; i++)
        {
            series2.DataPoints.Add(new ChartDataPoint
            {
                XValue = rng.NextDouble() * 100,
                YValue = rng.NextDouble() * 100
            });
        }

        chart.Series.Add(series);
        chart.Series.Add(series2);
        chart.Title = "Random Data Points";

        if (BasicScatterContainer != null)
            BasicScatterContainer.Child = chart;
    }

    private void CreateBubbleChart()
    {
        var chart = new ScatterPlot
        {
            Width = 600,
            Height = 400,
            MinPointSize = 6,
            MaxPointSize = 30,
            SizeBindingPath = "Size"
        };

        var rng = new Random(123);
        var series = new ScatterSeries
        {
            Title = "Bubble Data",
            Brush = new SolidColorBrush(Color.FromRgb(0x9C, 0x5F, 0xC4)),
            PointShape = PointShape.Circle
        };

        for (int i = 0; i < 30; i++)
        {
            double size = rng.NextDouble() * 80 + 10;
            series.DataPoints.Add(new ChartDataPoint
            {
                XValue = rng.NextDouble() * 100,
                YValue = rng.NextDouble() * 100,
                Tag = size
            });
        }

        chart.Series.Add(series);
        chart.Title = "Bubble Sizing";

        if (BubbleChartContainer != null)
            BubbleChartContainer.Child = chart;
    }

    private void CreateTrendLinePlot()
    {
        var chart = new ScatterPlot
        {
            Width = 600,
            Height = 400,
            ShowTrendLine = true,
            TrendLineType = TrendLineType.Linear
        };

        var rng = new Random(77);
        var series = new ScatterSeries
        {
            Title = "Correlated Data",
            Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x9E, 0x22)),
            PointShape = PointShape.Diamond
        };

        for (int i = 0; i < 80; i++)
        {
            double x = i * 1.25;  // 0 to 100 evenly
            double noise = (rng.NextDouble() - 0.5) * 16;
            double y = x * 0.75 + noise;
            series.DataPoints.Add(new ChartDataPoint
            {
                XValue = (double)x,
                YValue = (double)y
            });
        }

        chart.Series.Add(series);
        chart.Title = "Linear Trend Line";

        if (TrendLineContainer != null)
            TrendLineContainer.Child = chart;
    }
}
