using System.ComponentModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GallerySampleLayoutFixture
{
    [Fact]
    public void EveryBarChartSeriesHasTheDataShownInItsCodeExample()
    {
        var app = new App();
        var model = new BarChartViewModel();
        var page = new BarChartPage { DataContext = model };
        Layout(page, 354);

        var charts = Descendants<BarChart>(page).ToArray();
        Assert.Equal(3, charts.Length);
        Assert.Equal(new[] { 2, 3, 1 }, charts.Select(chart => chart.Series.Count));
        foreach (var series in charts.SelectMany(chart => chart.Series))
        {
            Assert.NotEmpty(series.DataPoints);
            Assert.All(series.DataPoints, point => Assert.True(point.YValue > 0));
        }
        Assert.Same(model.Grouped2024, charts[0].Series[0].DataPoints);
        Assert.Same(model.LanguagePopularity, charts[2].Series[0].DataPoints);
        GC.KeepAlive(app);
    }

    [Theory]
    [InlineData(typeof(PasswordBoxPage), typeof(PasswordBoxViewModel), typeof(PasswordBox))]
    [InlineData(typeof(TextBoxPage), typeof(TextBoxViewModel), typeof(TextBox))]
    [InlineData(typeof(LabelPage), typeof(LabelViewModel), typeof(TextBox))]
    [InlineData(typeof(BindingPage), typeof(BindingViewModel), typeof(TextBox))]
    [InlineData(typeof(CandlestickChartPage), typeof(CandlestickChartViewModel), typeof(CandlestickChart))]
    [InlineData(typeof(HeatmapPage), typeof(HeatmapViewModel), typeof(Heatmap))]
    [InlineData(typeof(TreeMapPage), typeof(TreeMapViewModel), typeof(TreeMap))]
    [InlineData(typeof(NetworkGraphPage), typeof(NetworkGraphViewModel), typeof(NetworkGraph))]
    [InlineData(typeof(ScatterPlotPage), typeof(ScatterPlotViewModel), typeof(ScatterPlot))]
    public void PhoneSamplesStayInsideTheirDemoSurface(Type pageType, Type modelType, Type controlType)
    {
        var app = new App();
        var page = (FrameworkElement)Activator.CreateInstance(pageType)!;
        page.DataContext = Activator.CreateInstance(modelType);
        // The shell/workbench padding leaves this width for a phone's page.
        Layout(page, 284);
        var samples = Descendants<FrameworkElement>(page)
            .Where(controlType.IsInstanceOfType)
            .Select(element => (Element: element, Surface: FindSurface(element)))
            .Where(item => item.Surface != null)
            .ToArray();
        Assert.NotEmpty(samples);
        foreach (var (element, surface) in samples)
        {
            var available = surface!.ActualWidth - surface.Padding.Left - surface.Padding.Right;
            Assert.InRange(element.ActualWidth, 1, available + 1);
        }
        GC.KeepAlive(app);
    }

    private static void Layout(FrameworkElement page, double width)
    {
        var viewport = new ScrollViewer
        {
            Width = width, Height = 1000, Content = page,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        for (var pass = 0; pass < 4; pass++)
        {
            viewport.Measure(new Size(width, 1000));
            viewport.Arrange(new Rect(0, 0, width, 1000));
            viewport.UpdateLayout();
        }
    }

    private static GalleryDemoSurface? FindSurface(Visual visual)
    {
        for (var parent = visual.VisualParent; parent != null; parent = parent.VisualParent)
            if (parent is GalleryDemoSurface surface) return surface;
        return null;
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject root)
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            var child = VisualTreeHelper.GetChild(root, index);
            if (child is T match) yield return match;
            if (child != null)
                foreach (var descendant in Descendants<T>(child)) yield return descendant;
        }
    }
}
