using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class CandlestickChartPage : Page
{
    private const string XamlExample = @"<!-- Basic Candlestick Chart -->
<CandlestickChart x:Name=""BasicChart""
                   Width=""700""
                   Height=""400""
                   Title=""ACME Corp (ACME)""/>

<!-- With Volume Sub-Chart -->
<CandlestickChart x:Name=""VolumeChart""
                   Width=""700""
                   Height=""450""
                   ShowVolume=""True""
                   VolumeHeight=""0.2""
                   Title=""Widget Inc (WGT)""/>

<!-- With Moving Average Overlays -->
<CandlestickChart x:Name=""MAChart""
                   Width=""700""
                   Height=""450""
                   ShowVolume=""True""
                   VolumeHeight=""0.15""
                   Title=""TechGlobal Ltd (TGL)""/>";

    private const string CSharpExample = @"using System.Collections.ObjectModel;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;

// Generate OHLC data
var data = new ObservableCollection<OhlcDataPoint>();
var date = new DateTime(2025, 1, 2);
double price = 150.0;
var rng = new Random(42);

for (int i = 0; i < 40; i++)
{
    double change = (rng.NextDouble() - 0.48) * 4.0;
    double open = price;
    double close = open + change;
    double high = Math.Max(open, close) + rng.NextDouble() * 2.0;
    double low = Math.Min(open, close) - rng.NextDouble() * 2.0;
    double volume = rng.NextDouble() * 5_000_000 + 1_000_000;

    data.Add(new OhlcDataPoint
    {
        Date = date,
        Open = Math.Round(open, 2),
        High = Math.Round(high, 2),
        Low = Math.Round(low, 2),
        Close = Math.Round(close, 2),
        Volume = Math.Round(volume, 0)
    });
    price = close;
    date = date.AddDays(date.DayOfWeek == DayOfWeek.Friday ? 3 : 1);
}

BasicChart.ItemsSource = data;

// Add moving averages
MAChart.MovingAverages.Add(new MovingAverageConfig
{
    Period = 10,
    Type = MovingAverageType.SMA,
    Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x9E, 0x22)),
    Thickness = 1.5
});

MAChart.MovingAverages.Add(new MovingAverageConfig
{
    Period = 20,
    Type = MovingAverageType.EMA,
    Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
    Thickness = 1.5
});";

    public CandlestickChartPage()
    {
        InitializeComponent();
        CreateBasicCandlestick();
        CreateVolumeChart();
        CreateMovingAverageChart();
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

    private static ObservableCollection<OhlcDataPoint> GenerateOhlcData(int count, int seed, double startPrice = 150.0)
    {
        var rng = new Random(seed);
        var data = new ObservableCollection<OhlcDataPoint>();
        var date = new DateTime(2025, 1, 2);
        double price = startPrice;

        for (int i = 0; i < count; i++)
        {
            double change = (rng.NextDouble() - 0.48) * 4.0;
            double open = price;
            double close = open + change;
            double high = Math.Max(open, close) + rng.NextDouble() * 2.0;
            double low = Math.Min(open, close) - rng.NextDouble() * 2.0;
            double volume = rng.NextDouble() * 5_000_000 + 1_000_000;

            data.Add(new OhlcDataPoint
            {
                Date = date,
                Open = Math.Round(open, 2),
                High = Math.Round(high, 2),
                Low = Math.Round(low, 2),
                Close = Math.Round(close, 2),
                Volume = Math.Round(volume, 0)
            });

            price = close;
            date = date.AddDays(date.DayOfWeek == DayOfWeek.Friday ? 3 : 1);
        }

        return data;
    }

    private void CreateBasicCandlestick()
    {
        var data = GenerateOhlcData(40, 42);

        var chart = new CandlestickChart
        {
            Width = 700,
            Height = 400,
            ItemsSource = data,
            Title = "ACME Corp (ACME)"
        };

        if (BasicCandlestickContainer != null)
            BasicCandlestickContainer.Child = chart;
    }

    private void CreateVolumeChart()
    {
        var data = GenerateOhlcData(40, 99);

        var chart = new CandlestickChart
        {
            Width = 700,
            Height = 450,
            ItemsSource = data,
            ShowVolume = true,
            VolumeHeight = 0.2,
            Title = "Widget Inc (WGT)"
        };

        if (VolumeContainer != null)
            VolumeContainer.Child = chart;
    }

    private void CreateMovingAverageChart()
    {
        var data = GenerateOhlcData(60, 77, 200.0);

        var chart = new CandlestickChart
        {
            Width = 700,
            Height = 450,
            ItemsSource = data,
            ShowVolume = true,
            VolumeHeight = 0.15,
            Title = "TechGlobal Ltd (TGL)"
        };

        chart.MovingAverages.Add(new MovingAverageConfig
        {
            Period = 10,
            Type = MovingAverageType.SMA,
            Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x9E, 0x22)),
            Thickness = 1.5
        });

        chart.MovingAverages.Add(new MovingAverageConfig
        {
            Period = 20,
            Type = MovingAverageType.EMA,
            Brush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
            Thickness = 1.5
        });

        if (MovingAverageContainer != null)
            MovingAverageContainer.Child = chart;
    }
}
