using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for GeographicHeatmapPage.jalxaml demonstrating GeographicHeatmap functionality.
/// </summary>
public partial class GeographicHeatmapPage : Page
{
    private const string XamlExample = @"<!-- Basic Geographic Heatmap -->
<GeographicHeatmap x:Name=""BasicHeatmap""
                    Height=""300""
                    Radius=""25""
                    Intensity=""1.0""
                    HeatmapOpacity=""0.7""
                    ShowLegend=""True""
                    LegendPosition=""BottomRight""/>

<!-- Small Radius, High Intensity -->
<GeographicHeatmap Width=""250""
                    Height=""200""
                    Radius=""10""
                    Intensity=""1.0""
                    HeatmapOpacity=""0.7""
                    ShowLegend=""False""/>

<!-- Custom Gradient -->
<GeographicHeatmap Width=""250""
                    Height=""200""
                    Radius=""25""
                    Intensity=""1.0""
                    HeatmapOpacity=""0.7""
                    ShowLegend=""True""
                    LegendPosition=""BottomRight""/>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

// Create heat points
var points = new HeatPointCollection
{
    new HeatPoint(37.7749, -122.4194, 1.0),
    new HeatPoint(37.7849, -122.4094, 0.9),
    new HeatPoint(37.7649, -122.4294, 0.7),
    new HeatPoint(37.8044, -122.2712, 0.7)
};

// Basic heatmap
var heatmap = new GeographicHeatmap
{
    Height = 300,
    Points = points,
    Radius = 25,
    Intensity = 1.0,
    HeatmapOpacity = 0.7,
    ShowLegend = true,
    LegendPosition = HeatmapLegendPosition.BottomRight
};

// Custom warm gradient
var warmStops = new GradientStopCollection();
warmStops.Add(new GradientStop
{
    Color = Color.FromArgb(0, 255, 200, 0),
    Offset = 0.0
});
warmStops.Add(new GradientStop
{
    Color = Color.FromRgb(255, 165, 0),
    Offset = 0.5
});
warmStops.Add(new GradientStop
{
    Color = Color.FromRgb(200, 0, 0),
    Offset = 1.0
});

heatmap.Gradient = new HeatmapGradient(warmStops);";

    public GeographicHeatmapPage()
    {
        InitializeComponent();
        SetupBasicHeatmap();
        SetupRadiusIntensity();
        SetupGradients();
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

    private static HeatPointCollection CreateSamplePoints()
    {
        // Scattered heat points around the San Francisco Bay Area
        var points = new HeatPointCollection
        {
            new HeatPoint(37.7749, -122.4194, 1.0),   // Downtown SF
            new HeatPoint(37.7849, -122.4094, 0.9),
            new HeatPoint(37.7649, -122.4294, 0.7),
            new HeatPoint(37.7950, -122.3934, 0.8),   // Near Embarcadero
            new HeatPoint(37.7550, -122.4444, 0.5),
            new HeatPoint(37.7849, -122.4394, 0.6),
            new HeatPoint(37.7699, -122.4694, 0.4),   // Sunset district
            new HeatPoint(37.8044, -122.2712, 0.7),   // Oakland
            new HeatPoint(37.8144, -122.2612, 0.5),
            new HeatPoint(37.8244, -122.2812, 0.3),
            new HeatPoint(37.7599, -122.3894, 0.6),   // Mission Bay
            new HeatPoint(37.7399, -122.3994, 0.4),
            new HeatPoint(37.7949, -122.4494, 0.5),   // Presidio area
            new HeatPoint(37.7699, -122.5094, 0.3),   // Ocean Beach
            new HeatPoint(37.8749, -122.2594, 0.6),   // Berkeley
            new HeatPoint(37.8649, -122.2694, 0.4),
            new HeatPoint(37.7299, -122.4794, 0.3),   // Daly City border
            new HeatPoint(37.7449, -122.4194, 0.8),   // Glen Park
            new HeatPoint(37.7549, -122.4094, 0.7),   // Castro
            new HeatPoint(37.7849, -122.4194, 1.0)    // Union Square
        };
        return points;
    }

    private void SetupBasicHeatmap()
    {
        if (BasicHeatmapContainer == null) return;

        var heatmap = new GeographicHeatmap
        {
            Height = 300,
            Points = CreateSamplePoints(),
            Radius = 25,
            Intensity = 1.0,
            HeatmapOpacity = 0.7,
            ShowLegend = true,
            LegendPosition = HeatmapLegendPosition.BottomRight
        };

        BasicHeatmapContainer.Children.Add(heatmap);
    }

    private void SetupRadiusIntensity()
    {
        if (RadiusIntensityContainer == null) return;

        var points = CreateSamplePoints();

        var row = new StackPanel { Orientation = Orientation.Horizontal };

        // Small radius, normal intensity
        var smallRadiusPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 12, 0) };
        smallRadiusPanel.Children.Add(new TextBlock
        {
            Text = "Radius: 10, Intensity: 1.0",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 4)
        });
        smallRadiusPanel.Children.Add(new GeographicHeatmap
        {
            Width = 250,
            Height = 200,
            Points = points,
            Radius = 10,
            Intensity = 1.0,
            HeatmapOpacity = 0.7,
            ShowLegend = false
        });
        row.Children.Add(smallRadiusPanel);

        // Large radius, low intensity
        var largeRadiusPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 12, 0) };
        largeRadiusPanel.Children.Add(new TextBlock
        {
            Text = "Radius: 40, Intensity: 0.5",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 4)
        });
        largeRadiusPanel.Children.Add(new GeographicHeatmap
        {
            Width = 250,
            Height = 200,
            Points = points,
            Radius = 40,
            Intensity = 0.5,
            HeatmapOpacity = 0.7,
            ShowLegend = false
        });
        row.Children.Add(largeRadiusPanel);

        // Medium radius, high intensity
        var highIntensityPanel = new StackPanel { Orientation = Orientation.Vertical };
        highIntensityPanel.Children.Add(new TextBlock
        {
            Text = "Radius: 25, Intensity: 2.0",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 4)
        });
        highIntensityPanel.Children.Add(new GeographicHeatmap
        {
            Width = 250,
            Height = 200,
            Points = points,
            Radius = 25,
            Intensity = 2.0,
            HeatmapOpacity = 0.7,
            ShowLegend = false
        });
        row.Children.Add(highIntensityPanel);

        RadiusIntensityContainer.Children.Add(row);
    }

    private void SetupGradients()
    {
        if (GradientContainer == null) return;

        var points = CreateSamplePoints();

        var row = new StackPanel { Orientation = Orientation.Horizontal };

        // Default gradient (blue -> cyan -> green -> yellow -> red)
        var defaultPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 12, 0) };
        defaultPanel.Children.Add(new TextBlock
        {
            Text = "Default (Spectrum)",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 4)
        });
        defaultPanel.Children.Add(new GeographicHeatmap
        {
            Width = 250,
            Height = 200,
            Points = points,
            Radius = 25,
            Intensity = 1.0,
            HeatmapOpacity = 0.7,
            Gradient = HeatmapGradient.Default,
            ShowLegend = true,
            LegendPosition = HeatmapLegendPosition.BottomRight
        });
        row.Children.Add(defaultPanel);

        // Warm gradient (transparent -> yellow -> orange -> red)
        var warmStops = new GradientStopCollection();
        warmStops.Add(new GradientStop { Color = Color.FromArgb(0, 255, 200, 0), Offset = 0.0 });
        warmStops.Add(new GradientStop { Color = Color.FromRgb(255, 230, 0), Offset = 0.25 });
        warmStops.Add(new GradientStop { Color = Color.FromRgb(255, 165, 0), Offset = 0.5 });
        warmStops.Add(new GradientStop { Color = Color.FromRgb(255, 69, 0), Offset = 0.75 });
        warmStops.Add(new GradientStop { Color = Color.FromRgb(200, 0, 0), Offset = 1.0 });

        var warmPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 12, 0) };
        warmPanel.Children.Add(new TextBlock
        {
            Text = "Warm (Yellow-Red)",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 4)
        });
        warmPanel.Children.Add(new GeographicHeatmap
        {
            Width = 250,
            Height = 200,
            Points = points,
            Radius = 25,
            Intensity = 1.0,
            HeatmapOpacity = 0.7,
            Gradient = new HeatmapGradient(warmStops),
            ShowLegend = true,
            LegendPosition = HeatmapLegendPosition.BottomRight
        });
        row.Children.Add(warmPanel);

        // Cool gradient (transparent -> cyan -> blue -> purple)
        var coolStops = new GradientStopCollection();
        coolStops.Add(new GradientStop { Color = Color.FromArgb(0, 0, 200, 255), Offset = 0.0 });
        coolStops.Add(new GradientStop { Color = Color.FromRgb(0, 200, 255), Offset = 0.25 });
        coolStops.Add(new GradientStop { Color = Color.FromRgb(0, 100, 255), Offset = 0.5 });
        coolStops.Add(new GradientStop { Color = Color.FromRgb(80, 0, 200), Offset = 0.75 });
        coolStops.Add(new GradientStop { Color = Color.FromRgb(150, 0, 180), Offset = 1.0 });

        var coolPanel = new StackPanel { Orientation = Orientation.Vertical };
        coolPanel.Children.Add(new TextBlock
        {
            Text = "Cool (Cyan-Purple)",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 4)
        });
        coolPanel.Children.Add(new GeographicHeatmap
        {
            Width = 250,
            Height = 200,
            Points = points,
            Radius = 25,
            Intensity = 1.0,
            HeatmapOpacity = 0.7,
            Gradient = new HeatmapGradient(coolStops),
            ShowLegend = true,
            LegendPosition = HeatmapLegendPosition.BottomRight
        });
        row.Children.Add(coolPanel);

        GradientContainer.Children.Add(row);
    }
}
