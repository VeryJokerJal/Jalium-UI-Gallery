using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class GaugeChartPage : Page
{
    private const string XamlExample =
@"<!-- Basic gauge -->
<GaugeChart Width=""300"" Height=""300""
            Value=""72""
            Minimum=""0""
            Maximum=""100""
            ShowValueText=""True""
            ValueFormat=""F0""
            Title=""Speed""/>

<!-- Gauge with colored ranges -->
<GaugeChart Width=""300"" Height=""300""
            Value=""65""
            Minimum=""0""
            Maximum=""100""
            ShowValueText=""True""
            Title=""Temperature"">
    <GaugeChart.Ranges>
        <GaugeRange Minimum=""0""  Maximum=""40""
                    Brush=""#4CAF50"" InnerRadiusRatio=""0.8""/>
        <GaugeRange Minimum=""40"" Maximum=""70""
                    Brush=""#FFC107"" InnerRadiusRatio=""0.8""/>
        <GaugeRange Minimum=""70"" Maximum=""100""
                    Brush=""#E03E3E"" InnerRadiusRatio=""0.8""/>
    </GaugeChart.Ranges>
</GaugeChart>";

    private const string CSharpExample =
@"using Jalium.UI.Controls.Charts;
using Jalium.UI.Media;

// Create a basic gauge
var gauge = new GaugeChart
{
    Width = 300,
    Height = 300,
    Value = 72,
    Minimum = 0,
    Maximum = 100,
    ShowValueText = true,
    ValueFormat = ""F0"",
    Title = ""Speed""
};

// Add colored range segments
gauge.Ranges.Add(new GaugeRange
{
    Minimum = 0,
    Maximum = 40,
    Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
    InnerRadiusRatio = 0.8
});
gauge.Ranges.Add(new GaugeRange
{
    Minimum = 40,
    Maximum = 70,
    Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)),
    InnerRadiusRatio = 0.8
});
gauge.Ranges.Add(new GaugeRange
{
    Minimum = 70,
    Maximum = 100,
    Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x3E, 0x3E)),
    InnerRadiusRatio = 0.8
});

// Customize tick marks
gauge.MajorTickInterval = 25;
gauge.MinorTickInterval = 5;
gauge.TrackThickness = 14;
gauge.ValueFontSize = 16;";

    public GaugeChartPage()
    {
        InitializeComponent();
        CreateBasicGauge();
        CreateColoredRangesGauge();
        CreateMultipleGauges();
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

    private void CreateBasicGauge()
    {
        var gauge = new GaugeChart
        {
            Width = 300,
            Height = 300,
            Value = 72,
            Minimum = 0,
            Maximum = 100,
            ShowValueText = true,
            ValueFormat = "F0",
            Title = "Speed"
        };

        if (BasicGaugeContainer != null)
            BasicGaugeContainer.Child = gauge;
    }

    private void CreateColoredRangesGauge()
    {
        var gauge = new GaugeChart
        {
            Width = 300,
            Height = 300,
            Value = 65,
            Minimum = 0,
            Maximum = 100,
            ShowValueText = true,
            ValueFormat = "F0",
            Title = "Temperature"
        };

        gauge.Ranges.Add(new GaugeRange
        {
            Minimum = 0,
            Maximum = 40,
            Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
            InnerRadiusRatio = 0.8
        });

        gauge.Ranges.Add(new GaugeRange
        {
            Minimum = 40,
            Maximum = 70,
            Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)),
            InnerRadiusRatio = 0.8
        });

        gauge.Ranges.Add(new GaugeRange
        {
            Minimum = 70,
            Maximum = 100,
            Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x3E, 0x3E)),
            InnerRadiusRatio = 0.8
        });

        if (ColoredRangesContainer != null)
            ColoredRangesContainer.Child = gauge;
    }

    private void CreateMultipleGauges()
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal };

        var configs = new[]
        {
            ("CPU", 45.0),
            ("Memory", 78.0),
            ("Disk", 92.0)
        };

        foreach (var (label, value) in configs)
        {
            var gaugePanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 16, 0) };

            var gauge = new GaugeChart
            {
                Width = 180,
                Height = 180,
                Value = value,
                Minimum = 0,
                Maximum = 100,
                ShowValueText = true,
                ValueFormat = "F0",
                ValueFontSize = 16,
                TrackThickness = 14,
                MajorTickInterval = 25,
                MinorTickInterval = 5
            };

            gauge.Ranges.Add(new GaugeRange
            {
                Minimum = 0,
                Maximum = 60,
                Brush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                InnerRadiusRatio = 0.82
            });
            gauge.Ranges.Add(new GaugeRange
            {
                Minimum = 60,
                Maximum = 85,
                Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)),
                InnerRadiusRatio = 0.82
            });
            gauge.Ranges.Add(new GaugeRange
            {
                Minimum = 85,
                Maximum = 100,
                Brush = new SolidColorBrush(Color.FromRgb(0xE0, 0x3E, 0x3E)),
                InnerRadiusRatio = 0.82
            });

            var labelText = new TextBlock
            {
                Text = label,
                Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 4, 0, 0)
            };

            gaugePanel.Children.Add(gauge);
            gaugePanel.Children.Add(labelText);
            panel.Children.Add(gaugePanel);
        }

        if (MultiGaugeContainer != null)
            MultiGaugeContainer.Child = panel;
    }
}
