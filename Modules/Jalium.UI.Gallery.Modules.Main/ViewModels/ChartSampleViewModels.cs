using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class BarChartViewModel
{
    public BarChartViewModel()
    {
        Grouped2024 = Points(
            ("North", 340),
            ("South", 280),
            ("East", 190),
            ("West", 420));
        Grouped2025 = Points(
            ("North", 410),
            ("South", 310),
            ("East", 250),
            ("West", 480));
        Products = Points(
            ("Q1", 120),
            ("Q2", 150),
            ("Q3", 130),
            ("Q4", 180));
        Services = Points(
            ("Q1", 80),
            ("Q2", 95),
            ("Q3", 110),
            ("Q4", 120));
        Licensing = Points(
            ("Q1", 40),
            ("Q2", 55),
            ("Q3", 50),
            ("Q4", 70));
        LanguagePopularity = Points(
            ("C#", 78),
            ("TypeScript", 65),
            ("Python", 62),
            ("Rust", 45),
            ("Go", 38),
            ("Java", 52));
    }

    public IReadOnlyList<CategoryValuePoint> Grouped2024 { get; }

    public IReadOnlyList<CategoryValuePoint> Grouped2025 { get; }

    public IReadOnlyList<CategoryValuePoint> Products { get; }

    public IReadOnlyList<CategoryValuePoint> Services { get; }

    public IReadOnlyList<CategoryValuePoint> Licensing { get; }

    public IReadOnlyList<CategoryValuePoint> LanguagePopularity { get; }

    private static IReadOnlyList<CategoryValuePoint> Points(
        params (string Category, double Value)[] values) =>
        values
            .Select(value =>
                new CategoryValuePoint(value.Category, value.Value))
            .ToArray();
}

public sealed partial class LineChartViewModel
{
    public LineChartViewModel()
    {
        MonthlyRevenue = Points(
            ("Jan", 42), ("Feb", 55), ("Mar", 48),
            ("Apr", 67), ("May", 73), ("Jun", 62),
            ("Jul", 85), ("Aug", 78), ("Sep", 91),
            ("Oct", 86), ("Nov", 94), ("Dec", 102));
        ProductA = Points(
            ("Q1", 150), ("Q2", 230),
            ("Q3", 180), ("Q4", 310));
        ProductB = Points(
            ("Q1", 200), ("Q2", 180),
            ("Q3", 260), ("Q4", 240));
        ProductC = Points(
            ("Q1", 80), ("Q2", 120),
            ("Q3", 140), ("Q4", 190));
        CpuUsage = Points(
            ("00:00", 12), ("04:00", 8), ("08:00", 45),
            ("10:00", 72), ("12:00", 65), ("14:00", 80),
            ("16:00", 58), ("18:00", 42), ("20:00", 30),
            ("22:00", 18));
    }

    public ObservableCollection<ChartDataPoint> MonthlyRevenue { get; }

    public ObservableCollection<ChartDataPoint> ProductA { get; }

    public ObservableCollection<ChartDataPoint> ProductB { get; }

    public ObservableCollection<ChartDataPoint> ProductC { get; }

    public ObservableCollection<ChartDataPoint> CpuUsage { get; }

    private static ObservableCollection<ChartDataPoint> Points(
        params (string Category, double Value)[] values) =>
        new(values.Select(value => new ChartDataPoint
        {
            XValue = value.Category,
            YValue = value.Value
        }));
}

public sealed partial class ScatterPlotViewModel
{
    public ScatterPlotViewModel()
    {
        var basicRandom = new Random(42);
        DatasetA = RandomPoints(basicRandom, 50);
        DatasetB = RandomPoints(basicRandom, 40);

        var bubbleRandom = new Random(123);
        BubbleData = new ObservableCollection<ChartDataPoint>(
            Enumerable.Range(0, 30).Select(_ =>
            {
                var size = bubbleRandom.NextDouble() * 80 + 10;
                return new ChartDataPoint
                {
                    XValue = bubbleRandom.NextDouble() * 100,
                    YValue = bubbleRandom.NextDouble() * 100,
                    Tag = size
                };
            }));

        var trendRandom = new Random(77);
        CorrelatedData = new ObservableCollection<ChartDataPoint>(
            Enumerable.Range(0, 80).Select(index =>
            {
                var x = index * 1.25;
                var noise = (trendRandom.NextDouble() - 0.5) * 16;
                return new ChartDataPoint
                {
                    XValue = x,
                    YValue = x * 0.75 + noise
                };
            }));
    }

    public ObservableCollection<ChartDataPoint> DatasetA { get; }

    public ObservableCollection<ChartDataPoint> DatasetB { get; }

    public ObservableCollection<ChartDataPoint> BubbleData { get; }

    public ObservableCollection<ChartDataPoint> CorrelatedData { get; }

    private static ObservableCollection<ChartDataPoint> RandomPoints(
        Random random,
        int count) =>
        new(Enumerable.Range(0, count).Select(_ =>
            new ChartDataPoint
            {
                XValue = random.NextDouble() * 100,
                YValue = random.NextDouble() * 100
            }));
}

public sealed partial class PieChartViewModel
{
    public PieChartViewModel()
    {
        MarketShare = PiePoints(
            ("Company A", 35, false),
            ("Company B", 25, false),
            ("Company C", 20, false),
            ("Company D", 12, false),
            ("Others", 8, false));
        StorageUsage = PiePoints(
            ("Documents", 45, false),
            ("Photos", 30, false),
            ("Videos", 15, false),
            ("Free Space", 10, false));
        BrowserUsage = PiePoints(
            ("Chrome", 65, true),
            ("Firefox", 12, false),
            ("Safari", 10, true),
            ("Edge", 8, false),
            ("Others", 5, false));
    }

    public ObservableCollection<PieDataPoint> MarketShare { get; }

    public ObservableCollection<PieDataPoint> StorageUsage { get; }

    public ObservableCollection<PieDataPoint> BrowserUsage { get; }

    private static ObservableCollection<PieDataPoint> PiePoints(
        params (string Label, double Value, bool IsExploded)[] values) =>
        new(values.Select(value => new PieDataPoint
        {
            Label = value.Label,
            Value = value.Value,
            IsExploded = value.IsExploded
        }));
}

public sealed partial class CandlestickChartViewModel
{
    public CandlestickChartViewModel()
    {
        BasicData = GenerateOhlcData(40, 42);
        VolumeData = GenerateOhlcData(40, 99);
        MovingAverageData = GenerateOhlcData(60, 77, 200);
    }

    public ObservableCollection<OhlcDataPoint> BasicData { get; }

    public ObservableCollection<OhlcDataPoint> VolumeData { get; }

    public ObservableCollection<OhlcDataPoint> MovingAverageData { get; }

    private static ObservableCollection<OhlcDataPoint> GenerateOhlcData(
        int count,
        int seed,
        double startPrice = 150)
    {
        var random = new Random(seed);
        var data = new ObservableCollection<OhlcDataPoint>();
        var date = new DateTime(2025, 1, 2);
        var price = startPrice;

        for (var index = 0; index < count; index++)
        {
            var change = (random.NextDouble() - 0.48) * 4;
            var open = price;
            var close = open + change;
            var high = Math.Max(open, close) +
                random.NextDouble() * 2;
            var low = Math.Min(open, close) -
                random.NextDouble() * 2;

            data.Add(new OhlcDataPoint
            {
                Date = date,
                Open = Math.Round(open, 2),
                High = Math.Round(high, 2),
                Low = Math.Round(low, 2),
                Close = Math.Round(close, 2),
                Volume = Math.Round(
                    random.NextDouble() * 5_000_000 + 1_000_000,
                    0)
            });

            price = close;
            date = date.AddDays(
                date.DayOfWeek == DayOfWeek.Friday ? 3 : 1);
        }

        return data;
    }
}

public sealed partial class HeatmapViewModel
{
    public HeatmapViewModel()
    {
        BasicData = GenerateSampleData(10, 10, 42);
        ScaleData = GenerateSampleData(8, 8, 99);
        CellValueData = GenerateCellValues(6, 6, 55);
        BasicXLabels = Enumerable.Range(0, 10)
            .Select(index => $"C{index}")
            .ToArray();
        BasicYLabels = Enumerable.Range(0, 10)
            .Select(index => $"R{index}")
            .ToArray();
        WeekdayLabels =
            new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        WeekLabels = Enumerable.Range(1, 6)
            .Select(index => $"Week {index}")
            .ToArray();
    }

    public double[,] BasicData { get; }

    public double[,] ScaleData { get; }

    public double[,] CellValueData { get; }

    public IReadOnlyList<string> BasicXLabels { get; }

    public IReadOnlyList<string> BasicYLabels { get; }

    public IReadOnlyList<string> WeekdayLabels { get; }

    public IReadOnlyList<string> WeekLabels { get; }

    private static double[,] GenerateSampleData(
        int rows,
        int columns,
        int seed)
    {
        var random = new Random(seed);
        var data = new double[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var x = (double)column / columns - 0.5;
                var y = (double)row / rows - 0.5;
                data[row, column] =
                    Math.Sin(x * 6) *
                    Math.Cos(y * 6) *
                    50 +
                    random.NextDouble() * 20;
            }
        }

        return data;
    }

    private static double[,] GenerateCellValues(
        int rows,
        int columns,
        int seed)
    {
        var random = new Random(seed);
        var data = new double[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                data[row, column] =
                    Math.Round(random.NextDouble() * 100, 1);
            }
        }

        return data;
    }
}

public sealed partial class GeographicHeatmapViewModel
{
    public GeographicHeatmapViewModel()
    {
        Points = new HeatPointCollection
        {
            new(37.7749, -122.4194, 1.0),
            new(37.7849, -122.4094, 0.9),
            new(37.7649, -122.4294, 0.7),
            new(37.7950, -122.3934, 0.8),
            new(37.7550, -122.4444, 0.5),
            new(37.7849, -122.4394, 0.6),
            new(37.7699, -122.4694, 0.4),
            new(37.8044, -122.2712, 0.7),
            new(37.8144, -122.2612, 0.5),
            new(37.8244, -122.2812, 0.3),
            new(37.7599, -122.3894, 0.6),
            new(37.7399, -122.3994, 0.4),
            new(37.7949, -122.4494, 0.5),
            new(37.7699, -122.5094, 0.3),
            new(37.8749, -122.2594, 0.6),
            new(37.8649, -122.2694, 0.4),
            new(37.7299, -122.4794, 0.3),
            new(37.7449, -122.4194, 0.8),
            new(37.7549, -122.4094, 0.7),
            new(37.7849, -122.4194, 1.0)
        };
        WarmGradient = Gradient(
            (Color.FromArgb(0, 255, 200, 0), 0),
            (Color.FromRgb(255, 230, 0), 0.25),
            (Color.FromRgb(255, 165, 0), 0.5),
            (Color.FromRgb(255, 69, 0), 0.75),
            (Color.FromRgb(200, 0, 0), 1));
        CoolGradient = Gradient(
            (Color.FromArgb(0, 0, 200, 255), 0),
            (Color.FromRgb(0, 200, 255), 0.25),
            (Color.FromRgb(0, 100, 255), 0.5),
            (Color.FromRgb(80, 0, 200), 0.75),
            (Color.FromRgb(150, 0, 180), 1));
    }

    public HeatPointCollection Points { get; }

    public HeatmapGradient WarmGradient { get; }

    public HeatmapGradient CoolGradient { get; }

    private static HeatmapGradient Gradient(
        params (Color Color, double Offset)[] stops)
    {
        var collection = new GradientStopCollection();
        foreach (var (color, offset) in stops)
        {
            collection.Add(new GradientStop
            {
                Color = color,
                Offset = offset
            });
        }

        return new HeatmapGradient(collection);
    }
}

public sealed partial class SparklineViewModel
{
    public SparklineViewModel()
    {
        Revenue = GenerateData(30, 10);
        Users = GenerateData(30, 20);
        Latency = GenerateData(30, 30);
        Sales = GenerateData(20, 44);
        Traffic = GenerateData(30, 55);
        TeamAlpha = GenerateWinLoss(100);
        TeamBeta = GenerateWinLoss(200);
        TeamGamma = GenerateWinLoss(300);
    }

    public IList<double> Revenue { get; }

    public IList<double> Users { get; }

    public IList<double> Latency { get; }

    public IList<double> Sales { get; }

    public IList<double> Traffic { get; }

    public IList<double> TeamAlpha { get; }

    public IList<double> TeamBeta { get; }

    public IList<double> TeamGamma { get; }

    private static IList<double> GenerateData(
        int count,
        int seed,
        double amplitude = 50,
        double offset = 50)
    {
        var random = new Random(seed);
        var data = new List<double>(count);
        var value = offset;
        for (var index = 0; index < count; index++)
        {
            value +=
                (random.NextDouble() - 0.5) * amplitude * 0.3;
            value = Math.Clamp(value, 0, 100);
            data.Add(value);
        }

        return data;
    }

    private static IList<double> GenerateWinLoss(int seed)
    {
        var random = new Random(seed);
        return Enumerable.Range(0, 20)
            .Select(_ => random.NextDouble() > 0.45 ? 1d : -1d)
            .ToArray();
    }
}
