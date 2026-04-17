using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for LineChartPage.jalxaml demonstrating the LineChart control.
/// </summary>
public partial class LineChartPage : Page
{
    private const string XamlExample =
@"<LineChart Height=""300""
           Title=""Monthly Sales""
           IsLegendVisible=""True""
           ShowDataPoints=""True""
           IsGridLinesVisible=""True"">
    <LineChart.Series>
        <LineSeries Title=""Revenue""
                    Brush=""#0078D4"">
            <LineSeries.DataPoints>
                <ChartDataPoint XValue=""Jan"" YValue=""42""/>
                <ChartDataPoint XValue=""Feb"" YValue=""55""/>
                <ChartDataPoint XValue=""Mar"" YValue=""48""/>
                <ChartDataPoint XValue=""Apr"" YValue=""67""/>
                <ChartDataPoint XValue=""May"" YValue=""73""/>
                <ChartDataPoint XValue=""Jun"" YValue=""62""/>
            </LineSeries.DataPoints>
        </LineSeries>
    </LineChart.Series>
</LineChart>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;
using System.Collections.ObjectModel;

// Create a line chart with multiple series
var chart = new LineChart
{
    Height = 350,
    Title = ""Quarterly Performance"",
    IsLegendVisible = true,
    ShowDataPoints = true,
    IsGridLinesVisible = true
};

var series = new LineSeries
{
    Title = ""Product A"",
    Brush = new SolidColorBrush(Color.FromRgb(0, 120, 212))
};

series.DataPoints = new ObservableCollection<ChartDataPoint>
{
    new() { XValue = ""Q1"", YValue = 150 },
    new() { XValue = ""Q2"", YValue = 230 },
    new() { XValue = ""Q3"", YValue = 180 },
    new() { XValue = ""Q4"", YValue = 310 }
};

chart.Series.Add(series);

// Enable smoothing and area fill
chart.LineSmoothing = true;
chart.ShowArea = true;
chart.AreaOpacity = 0.3;";

    public LineChartPage()
    {
        InitializeComponent();
        SetupBasicChart();
        SetupMultiSeriesChart();
        SetupSmoothedChart();
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

    private void SetupBasicChart()
    {
        if (BasicChartContainer == null) return;

        var chart = new LineChart
        {
            Height = 300,
            Title = "Monthly Sales",
            IsLegendVisible = true,
            ShowDataPoints = true,
            IsGridLinesVisible = true
        };

        var series = new LineSeries
        {
            Title = "Revenue",
            Brush = new SolidColorBrush(Color.FromRgb(0, 120, 212))
        };

        series.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Jan", YValue = 42 },
            new() { XValue = "Feb", YValue = 55 },
            new() { XValue = "Mar", YValue = 48 },
            new() { XValue = "Apr", YValue = 67 },
            new() { XValue = "May", YValue = 73 },
            new() { XValue = "Jun", YValue = 62 },
            new() { XValue = "Jul", YValue = 85 },
            new() { XValue = "Aug", YValue = 78 },
            new() { XValue = "Sep", YValue = 91 },
            new() { XValue = "Oct", YValue = 86 },
            new() { XValue = "Nov", YValue = 94 },
            new() { XValue = "Dec", YValue = 102 }
        };

        chart.Series.Add(series);
        BasicChartContainer.Children.Add(chart);
    }

    private void SetupMultiSeriesChart()
    {
        if (MultiSeriesContainer == null) return;

        var chart = new LineChart
        {
            Height = 350,
            Title = "Quarterly Performance",
            IsLegendVisible = true,
            ShowDataPoints = true,
            IsGridLinesVisible = true
        };

        // Series 1: Product A
        var seriesA = new LineSeries
        {
            Title = "Product A",
            Brush = new SolidColorBrush(Color.FromRgb(0, 120, 212))
        };
        seriesA.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Q1", YValue = 150 },
            new() { XValue = "Q2", YValue = 230 },
            new() { XValue = "Q3", YValue = 180 },
            new() { XValue = "Q4", YValue = 310 }
        };

        // Series 2: Product B
        var seriesB = new LineSeries
        {
            Title = "Product B",
            Brush = new SolidColorBrush(Color.FromRgb(16, 185, 129))
        };
        seriesB.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Q1", YValue = 200 },
            new() { XValue = "Q2", YValue = 180 },
            new() { XValue = "Q3", YValue = 260 },
            new() { XValue = "Q4", YValue = 240 }
        };

        // Series 3: Product C
        var seriesC = new LineSeries
        {
            Title = "Product C",
            Brush = new SolidColorBrush(Color.FromRgb(239, 68, 68))
        };
        seriesC.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Q1", YValue = 80 },
            new() { XValue = "Q2", YValue = 120 },
            new() { XValue = "Q3", YValue = 140 },
            new() { XValue = "Q4", YValue = 190 }
        };

        chart.Series.Add(seriesA);
        chart.Series.Add(seriesB);
        chart.Series.Add(seriesC);
        MultiSeriesContainer.Children.Add(chart);
    }

    private void SetupSmoothedChart()
    {
        if (SmoothedChartContainer == null) return;

        var chart = new LineChart
        {
            Height = 300,
            Title = "CPU Usage Over Time",
            IsLegendVisible = true,
            ShowDataPoints = true,
            LineSmoothing = true,
            ShowArea = true,
            AreaOpacity = 0.3,
            IsGridLinesVisible = true
        };

        var series = new LineSeries
        {
            Title = "CPU %",
            Brush = new SolidColorBrush(Color.FromRgb(139, 92, 246))
        };

        series.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "00:00", YValue = 12 },
            new() { XValue = "04:00", YValue = 8 },
            new() { XValue = "08:00", YValue = 45 },
            new() { XValue = "10:00", YValue = 72 },
            new() { XValue = "12:00", YValue = 65 },
            new() { XValue = "14:00", YValue = 80 },
            new() { XValue = "16:00", YValue = 58 },
            new() { XValue = "18:00", YValue = 42 },
            new() { XValue = "20:00", YValue = 30 },
            new() { XValue = "22:00", YValue = 18 }
        };

        chart.Series.Add(series);
        SmoothedChartContainer.Children.Add(chart);
    }
}
