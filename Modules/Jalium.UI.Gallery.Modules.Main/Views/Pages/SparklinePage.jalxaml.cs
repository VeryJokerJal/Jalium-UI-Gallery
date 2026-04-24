using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class SparklinePage : Page
{
    private const string XamlExample =
@"<!-- Line sparkline with high/low markers -->
<Sparkline Width=""200"" Height=""32""
           SparklineType=""Line""
           LineBrush=""#417EE0""
           ShowHighLowPoints=""True""/>

<!-- Bar sparkline with negative baseline -->
<Sparkline Width=""200"" Height=""40""
           SparklineType=""Bar""
           LineBrush=""#417EE0""
           NegativeBarBrush=""#E0593E""
           BaselineValue=""50""/>

<!-- Area sparkline -->
<Sparkline Width=""200"" Height=""40""
           SparklineType=""Area""
           LineBrush=""#4CAF50""
           FillBrush=""#3C4CAF50""/>

<!-- Win/Loss sparkline -->
<Sparkline Width=""200"" Height=""32""
           SparklineType=""WinLoss""
           WinColor=""#4CAF50""
           LossColor=""#E0593E""/>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;

// Generate random walk data
var data = new List<double>();
double value = 50;
var rng = new Random(10);
for (int i = 0; i < 30; i++)
{
    value += (rng.NextDouble() - 0.5) * 15;
    value = Math.Clamp(value, 0, 100);
    data.Add(value);
}

// Line sparkline
var lineSparkline = new Sparkline
{
    Width = 200,
    Height = 32,
    SparklineType = SparklineType.Line,
    Values = data,
    LineBrush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
    ShowHighLowPoints = true
};

// Bar sparkline with baseline
var barSparkline = new Sparkline
{
    Width = 200,
    Height = 40,
    SparklineType = SparklineType.Bar,
    Values = data,
    BaselineValue = 50,
    NegativeBarBrush = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E))
};

// Win/Loss sparkline
var winLossData = new List<double>();
for (int i = 0; i < 20; i++)
    winLossData.Add(rng.NextDouble() > 0.45 ? 1.0 : -1.0);

var wlSparkline = new Sparkline
{
    SparklineType = SparklineType.WinLoss,
    Values = winLossData,
    WinColor = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
    LossColor = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E))
};";

    public SparklinePage()
    {
        InitializeComponent();
        CreateLineSparklines();
        CreateBarAreaSparklines();
        CreateWinLossSparklines();
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

    private static IList<double> GenerateData(int count, int seed, double amplitude = 50, double offset = 50)
    {
        var rng = new Random(seed);
        var data = new List<double>();
        double value = offset;
        for (int i = 0; i < count; i++)
        {
            value += (rng.NextDouble() - 0.5) * amplitude * 0.3;
            value = Math.Clamp(value, 0, 100);
            data.Add(value);
        }
        return data;
    }

    private void CreateLineSparklines()
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical };

        var labels = new[] { "Revenue", "Users", "Latency" };
        var seeds = new[] { 10, 20, 30 };
        var colors = new[]
        {
            Color.FromRgb(0x41, 0x7E, 0xE0),
            Color.FromRgb(0x4C, 0xAF, 0x50),
            Color.FromRgb(0xFF, 0x9E, 0x22)
        };

        for (int i = 0; i < 3; i++)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };

            var label = new TextBlock
            {
                Text = labels[i],
                Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
                FontSize = 13,
                Width = 80,
                VerticalAlignment = VerticalAlignment.Center
            };

            var sparkline = new Sparkline
            {
                Width = 200,
                Height = 32,
                SparklineType = SparklineType.Line,
                Values = GenerateData(30, seeds[i]),
                LineBrush = new SolidColorBrush(colors[i]),
                ShowHighLowPoints = true
            };

            row.Children.Add(label);
            row.Children.Add(sparkline);
            panel.Children.Add(row);
        }

        if (LineSparklineContainer != null)
            LineSparklineContainer.Child = panel;
    }

    private void CreateBarAreaSparklines()
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical };

        // Bar sparkline
        var barRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
        var barLabel = new TextBlock
        {
            Text = "Sales (Bar)",
            Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
            FontSize = 13,
            Width = 100,
            VerticalAlignment = VerticalAlignment.Center
        };

        var barSparkline = new Sparkline
        {
            Width = 200,
            Height = 40,
            SparklineType = SparklineType.Bar,
            Values = GenerateData(20, 44),
            LineBrush = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
            NegativeBarBrush = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E)),
            BaselineValue = 50
        };

        barRow.Children.Add(barLabel);
        barRow.Children.Add(barSparkline);
        panel.Children.Add(barRow);

        // Area sparkline
        var areaRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
        var areaLabel = new TextBlock
        {
            Text = "Traffic (Area)",
            Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
            FontSize = 13,
            Width = 100,
            VerticalAlignment = VerticalAlignment.Center
        };

        var areaSparkline = new Sparkline
        {
            Width = 200,
            Height = 40,
            SparklineType = SparklineType.Area,
            Values = GenerateData(30, 55),
            LineBrush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
            FillBrush = new SolidColorBrush(Color.FromArgb(60, 0x4C, 0xAF, 0x50))
        };

        areaRow.Children.Add(areaLabel);
        areaRow.Children.Add(areaSparkline);
        panel.Children.Add(areaRow);

        if (BarAreaContainer != null)
            BarAreaContainer.Child = panel;
    }

    private void CreateWinLossSparklines()
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical };
        var rng = new Random(66);

        var titles = new[] { "Team Alpha", "Team Beta", "Team Gamma" };
        var seeds = new[] { 100, 200, 300 };

        for (int t = 0; t < 3; t++)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
            var label = new TextBlock
            {
                Text = titles[t],
                Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
                FontSize = 13,
                Width = 100,
                VerticalAlignment = VerticalAlignment.Center
            };

            var winLossRng = new Random(seeds[t]);
            var data = new List<double>();
            for (int i = 0; i < 20; i++)
            {
                data.Add(winLossRng.NextDouble() > 0.45 ? 1.0 : -1.0);
            }

            var sparkline = new Sparkline
            {
                Width = 200,
                Height = 32,
                SparklineType = SparklineType.WinLoss,
                Values = data,
                WinColor = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                LossColor = new SolidColorBrush(Color.FromRgb(0xE0, 0x59, 0x3E))
            };

            row.Children.Add(label);
            row.Children.Add(sparkline);
            panel.Children.Add(row);
        }

        if (WinLossContainer != null)
            WinLossContainer.Child = panel;
    }
}
