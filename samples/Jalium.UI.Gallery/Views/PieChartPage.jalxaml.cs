using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for PieChartPage.jalxaml demonstrating the PieChart control.
/// </summary>
public partial class PieChartPage : Page
{
    private const string XamlExample =
@"<PieChart Height=""350""
          Title=""Market Share""
          IsLegendVisible=""True""
          ShowLabels=""True""
          LabelPosition=""Outside"">
    <PieChart.Series>
        <PieSeries Title=""Market Share"">
            <PieSeries.DataPoints>
                <PieDataPoint Label=""Company A"" Value=""35"" Brush=""#3B82F6""/>
                <PieDataPoint Label=""Company B"" Value=""25"" Brush=""#10B981""/>
                <PieDataPoint Label=""Company C"" Value=""20"" Brush=""#F59E0B""/>
                <PieDataPoint Label=""Company D"" Value=""12"" Brush=""#EF4444""/>
                <PieDataPoint Label=""Others""    Value=""8""  Brush=""#9CA3AF""/>
            </PieSeries.DataPoints>
        </PieSeries>
    </PieChart.Series>
</PieChart>

<!-- Donut chart variant -->
<PieChart InnerRadiusRatio=""0.5""
          Title=""Storage Usage""/>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;
using System.Collections.ObjectModel;

// Create a basic pie chart
var chart = new PieChart
{
    Height = 350,
    Title = ""Market Share"",
    IsLegendVisible = true,
    ShowLabels = true,
    LabelPosition = PieLabelPosition.Outside
};

var series = new PieSeries { Title = ""Market Share"" };
series.DataPoints = new ObservableCollection<PieDataPoint>
{
    new() { Label = ""Company A"", Value = 35,
            Brush = new SolidColorBrush(Color.FromRgb(59, 130, 246)) },
    new() { Label = ""Company B"", Value = 25,
            Brush = new SolidColorBrush(Color.FromRgb(16, 185, 129)) },
    new() { Label = ""Others"",    Value = 8,
            Brush = new SolidColorBrush(Color.FromRgb(156, 163, 175)) }
};

chart.Series = series;

// Donut mode: set InnerRadiusRatio
chart.InnerRadiusRatio = 0.5;

// Exploded slices
chart.ExplodeOffset = 12;
series.DataPoints[0].IsExploded = true;";

    public PieChartPage()
    {
        InitializeComponent();
        SetupBasicPie();
        SetupDonutChart();
        SetupExplodedPie();
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

    private void SetupBasicPie()
    {
        if (BasicPieContainer == null) return;

        var chart = new PieChart
        {
            Height = 350,
            Title = "Market Share",
            IsLegendVisible = true,
            ShowLabels = true,
            LabelPosition = PieLabelPosition.Outside
        };

        var series = new PieSeries { Title = "Market Share" };
        series.DataPoints = new ObservableCollection<PieDataPoint>
        {
            new() { Label = "Company A", Value = 35, Brush = new SolidColorBrush(Color.FromRgb(59, 130, 246)) },
            new() { Label = "Company B", Value = 25, Brush = new SolidColorBrush(Color.FromRgb(16, 185, 129)) },
            new() { Label = "Company C", Value = 20, Brush = new SolidColorBrush(Color.FromRgb(245, 158, 11)) },
            new() { Label = "Company D", Value = 12, Brush = new SolidColorBrush(Color.FromRgb(239, 68, 68)) },
            new() { Label = "Others", Value = 8, Brush = new SolidColorBrush(Color.FromRgb(156, 163, 175)) }
        };

        chart.Series = series;
        BasicPieContainer.Children.Add(chart);
    }

    private void SetupDonutChart()
    {
        if (DonutContainer == null) return;

        var chart = new PieChart
        {
            Height = 350,
            Title = "Storage Usage",
            IsLegendVisible = true,
            ShowLabels = true,
            InnerRadiusRatio = 0.5
        };

        var series = new PieSeries { Title = "Storage" };
        series.DataPoints = new ObservableCollection<PieDataPoint>
        {
            new() { Label = "Documents", Value = 45, Brush = new SolidColorBrush(Color.FromRgb(99, 102, 241)) },
            new() { Label = "Photos", Value = 30, Brush = new SolidColorBrush(Color.FromRgb(14, 165, 233)) },
            new() { Label = "Videos", Value = 15, Brush = new SolidColorBrush(Color.FromRgb(168, 85, 247)) },
            new() { Label = "Free Space", Value = 10, Brush = new SolidColorBrush(Color.FromRgb(75, 85, 99)) }
        };

        chart.Series = series;
        DonutContainer.Children.Add(chart);
    }

    private void SetupExplodedPie()
    {
        if (ExplodedPieContainer == null) return;

        var chart = new PieChart
        {
            Height = 350,
            Title = "Browser Usage",
            IsLegendVisible = true,
            ShowLabels = true,
            ExplodeOffset = 12
        };

        var series = new PieSeries { Title = "Browsers" };
        series.DataPoints = new ObservableCollection<PieDataPoint>
        {
            new() { Label = "Chrome", Value = 65, IsExploded = true, Brush = new SolidColorBrush(Color.FromRgb(52, 168, 83)) },
            new() { Label = "Firefox", Value = 12, Brush = new SolidColorBrush(Color.FromRgb(255, 122, 0)) },
            new() { Label = "Safari", Value = 10, IsExploded = true, Brush = new SolidColorBrush(Color.FromRgb(0, 122, 255)) },
            new() { Label = "Edge", Value = 8, Brush = new SolidColorBrush(Color.FromRgb(0, 120, 212)) },
            new() { Label = "Others", Value = 5, Brush = new SolidColorBrush(Color.FromRgb(128, 128, 128)) }
        };

        chart.Series = series;
        ExplodedPieContainer.Children.Add(chart);
    }
}
