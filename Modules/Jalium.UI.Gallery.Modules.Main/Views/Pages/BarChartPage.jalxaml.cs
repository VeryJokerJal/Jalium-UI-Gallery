using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for BarChartPage.jalxaml demonstrating the BarChart control.
/// </summary>
public partial class BarChartPage : Page
{
    private const string XamlExample =
@"<BarChart Height=""350""
          Title=""Sales by Region""
          IsLegendVisible=""True""
          BarMode=""Grouped""
          ShowValueLabels=""True""
          IsGridLinesVisible=""True"">
    <BarChart.Series>
        <BarSeries Title=""2024"" Brush=""#3B82F6"">
            <BarSeries.DataPoints>
                <ChartDataPoint XValue=""North"" YValue=""340""/>
                <ChartDataPoint XValue=""South"" YValue=""280""/>
                <ChartDataPoint XValue=""East""  YValue=""190""/>
                <ChartDataPoint XValue=""West""  YValue=""420""/>
            </BarSeries.DataPoints>
        </BarSeries>
        <BarSeries Title=""2025"" Brush=""#10B981"">
            <BarSeries.DataPoints>
                <ChartDataPoint XValue=""North"" YValue=""410""/>
                <ChartDataPoint XValue=""South"" YValue=""310""/>
                <ChartDataPoint XValue=""East""  YValue=""250""/>
                <ChartDataPoint XValue=""West""  YValue=""480""/>
            </BarSeries.DataPoints>
        </BarSeries>
    </BarChart.Series>
</BarChart>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;
using System.Collections.ObjectModel;

// Create a grouped bar chart
var chart = new BarChart
{
    Height = 350,
    Title = ""Sales by Region"",
    IsLegendVisible = true,
    BarMode = BarMode.Grouped,
    ShowValueLabels = true,
    IsGridLinesVisible = true
};

var series2024 = new BarSeries
{
    Title = ""2024"",
    Brush = new SolidColorBrush(Color.FromRgb(59, 130, 246))
};
series2024.DataPoints = new ObservableCollection<ChartDataPoint>
{
    new() { XValue = ""North"", YValue = 340 },
    new() { XValue = ""South"", YValue = 280 },
    new() { XValue = ""East"",  YValue = 190 },
    new() { XValue = ""West"",  YValue = 420 }
};

chart.Series.Add(series2024);

// Switch to stacked or horizontal mode
chart.BarMode = BarMode.Stacked;
chart.Orientation = Orientation.Horizontal;";

    public BarChartPage()
    {
        InitializeComponent();
        SetupGroupedChart();
        SetupStackedChart();
        SetupHorizontalChart();
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

    private void SetupGroupedChart()
    {
        if (GroupedBarContainer == null) return;

        var chart = new BarChart
        {
            Height = 350,
            Title = "Sales by Region",
            IsLegendVisible = true,
            BarMode = BarMode.Grouped,
            ShowValueLabels = true,
            IsGridLinesVisible = true
        };

        var series2024 = new BarSeries
        {
            Title = "2024",
            Brush = new SolidColorBrush(Color.FromRgb(59, 130, 246))
        };
        series2024.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "North", YValue = 340 },
            new() { XValue = "South", YValue = 280 },
            new() { XValue = "East", YValue = 190 },
            new() { XValue = "West", YValue = 420 }
        };

        var series2025 = new BarSeries
        {
            Title = "2025",
            Brush = new SolidColorBrush(Color.FromRgb(16, 185, 129))
        };
        series2025.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "North", YValue = 410 },
            new() { XValue = "South", YValue = 310 },
            new() { XValue = "East", YValue = 250 },
            new() { XValue = "West", YValue = 480 }
        };

        chart.Series.Add(series2024);
        chart.Series.Add(series2025);
        GroupedBarContainer.Children.Add(chart);
    }

    private void SetupStackedChart()
    {
        if (StackedBarContainer == null) return;

        var chart = new BarChart
        {
            Height = 350,
            Title = "Revenue Breakdown",
            IsLegendVisible = true,
            BarMode = BarMode.Stacked,
            IsGridLinesVisible = true
        };

        var productSeries = new BarSeries
        {
            Title = "Products",
            Brush = new SolidColorBrush(Color.FromRgb(99, 102, 241))
        };
        productSeries.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Q1", YValue = 120 },
            new() { XValue = "Q2", YValue = 150 },
            new() { XValue = "Q3", YValue = 130 },
            new() { XValue = "Q4", YValue = 180 }
        };

        var servicesSeries = new BarSeries
        {
            Title = "Services",
            Brush = new SolidColorBrush(Color.FromRgb(245, 158, 11))
        };
        servicesSeries.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Q1", YValue = 80 },
            new() { XValue = "Q2", YValue = 95 },
            new() { XValue = "Q3", YValue = 110 },
            new() { XValue = "Q4", YValue = 120 }
        };

        var licensingSeries = new BarSeries
        {
            Title = "Licensing",
            Brush = new SolidColorBrush(Color.FromRgb(236, 72, 153))
        };
        licensingSeries.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "Q1", YValue = 40 },
            new() { XValue = "Q2", YValue = 55 },
            new() { XValue = "Q3", YValue = 50 },
            new() { XValue = "Q4", YValue = 70 }
        };

        chart.Series.Add(productSeries);
        chart.Series.Add(servicesSeries);
        chart.Series.Add(licensingSeries);
        StackedBarContainer.Children.Add(chart);
    }

    private void SetupHorizontalChart()
    {
        if (HorizontalBarContainer == null) return;

        var chart = new BarChart
        {
            Height = 350,
            Title = "Programming Language Popularity",
            IsLegendVisible = true,
            Orientation = Orientation.Horizontal,
            BarMode = BarMode.Grouped,
            ShowValueLabels = true,
            IsGridLinesVisible = true
        };

        var series = new BarSeries
        {
            Title = "Developers (%)",
            Brush = new SolidColorBrush(Color.FromRgb(14, 165, 233))
        };
        series.DataPoints = new ObservableCollection<ChartDataPoint>
        {
            new() { XValue = "C#", YValue = 78 },
            new() { XValue = "TypeScript", YValue = 65 },
            new() { XValue = "Python", YValue = 62 },
            new() { XValue = "Rust", YValue = 45 },
            new() { XValue = "Go", YValue = 38 },
            new() { XValue = "Java", YValue = 52 }
        };

        chart.Series.Add(series);
        HorizontalBarContainer.Children.Add(chart);
    }
}
