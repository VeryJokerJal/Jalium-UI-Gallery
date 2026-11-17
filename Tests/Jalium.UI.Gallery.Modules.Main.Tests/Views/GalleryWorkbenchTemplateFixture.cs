using System.ComponentModel;
using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryWorkbenchTemplateFixture
{
    [Fact]
    public void InspectorStatesRespondToWidthAndKeepTheirTemplateBinding()
    {
        WithWorkbench((workbench, window) =>
        {
            Layout(window, 1500);
            var column = Assert.IsType<ColumnDefinition>(workbench.FindName("InspectorColumn"));
            var panel = Assert.IsType<Border>(workbench.FindName("InspectorPanel"));
            Assert.Equal(316, column.ActualWidth);
            Assert.Equal(300, panel.Width);
            Assert.Equal(1, Grid.GetColumn(panel));
            Assert.Equal(1, Grid.GetRow(panel));
            Assert.Equal(1, Grid.GetColumnSpan(panel));
            workbench.InspectorWidth = 344;
            Layout(window, 1500);
            Assert.Equal(360, column.ActualWidth);
            Layout(window, 390);
            Assert.Equal(0, column.ActualWidth);
            Assert.Equal(0, Grid.GetColumn(panel));
            Assert.Equal(2, Grid.GetRow(panel));
            Assert.Equal(2, Grid.GetColumnSpan(panel));
            Assert.Equal(new Thickness(0, 16, 0, 0), panel.Margin);
            Layout(window, 1200);
            Assert.Equal(360, column.ActualWidth);
            Assert.Equal(new Thickness(16, 0, 0, 0), panel.Margin);
            workbench.CompactWidth = 1300;
            Layout(window, 1200);
            Assert.Equal(0, column.ActualWidth);
        });
    }

    [Fact]
    public void ContentReplacementAndTemplateRetirementReleaseVisualsWithoutOverridingTheirStyle()
    {
        WithWorkbench((workbench, window) =>
        {
            var first = new Button { Content = "First" };
            var second = new Button { Content = "Second" };
            var context = new object();
            first.DataContext = context;
            workbench.DemoContent = first;
            Layout(window, 390);
            var presenter = Assert.IsType<ContentPresenter>(workbench.FindName("PART_DemoContentPresenter"));
            Assert.Same(first, presenter.Content);
            Assert.Same(context, first.DataContext);
            Assert.Same(DependencyProperty.UnsetValue, first.ReadLocalValue(Control.ForegroundProperty));
            workbench.DemoContent = second;
            Layout(window, 390);
            Assert.Null(first.VisualParent);
            Assert.Same(second, presenter.Content);
            var template = workbench.Template;
            var minimal = new ControlTemplate(typeof(GalleryDemoWorkbench));
            minimal.SetVisualTree(() => new Grid());
            workbench.Template = minimal;
            Layout(window, 390);
            Assert.Null(second.VisualParent);
            workbench.DemoContent = first;
            workbench.Template = template;
            Layout(window, 390);
            var replacement = Assert.IsType<ContentPresenter>(workbench.FindName("PART_DemoContentPresenter"));
            Assert.NotSame(presenter, replacement);
            Assert.Same(first, replacement.Content);
            Assert.Same(context, first.DataContext);
            Assert.Same(DependencyProperty.UnsetValue, first.ReadLocalValue(Control.ForegroundProperty));
        });
    }

    [Fact]
    public void ADataTemplateCanPresentLiveDataInsteadOfRequiringAVisualInstance()
    {
        WithWorkbench((workbench, window) =>
        {
            var data = new PreviewData { Label = "Preview data" };
            var template = new DataTemplate();
            template.SetVisualTree(() =>
            {
                var text = new TextBlock();
                BindingOperations.SetBinding(text, TextBlock.TextProperty, new Binding(nameof(PreviewData.Label)));
                return text;
            });
            workbench.DemoContentTemplate = template;
            workbench.DemoContent = data;
            Layout(window, 390);
            var presenter = Assert.IsType<ContentPresenter>(workbench.FindName("PART_DemoContentPresenter"));
            var text = Assert.IsType<TextBlock>(Jalium.UI.Media.VisualTreeHelper.GetChild(presenter, 0));
            Assert.Equal("Preview data", text.Text);
            data.Label = "Updated data";
            Assert.Equal("Updated data", text.Text);
            Assert.Same(data, presenter.Content);
        });
    }

    private static void WithWorkbench(Action<GalleryDemoWorkbench, Window> action)
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var model = new GalleryDemoWorkbenchViewModel(new GalleryThemeService());
        var workbench = new GalleryDemoWorkbench { DataContext = model };
        var window = new Window { Width = 1500, Height = 1000, Content = workbench };
        try
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(window, new object[] { Visibility.Visible });
            action(workbench, window);
        }
        finally { window.Content = null; model.Destroy(); LocalizationManager.SetLanguage(language); GC.KeepAlive(app); }
    }
    private static void Layout(Window window, double width)
    {
        window.Width = width;
        window.Measure(new Size(width, 1000)); window.Arrange(new Rect(0, 0, width, 1000)); window.UpdateLayout();
    }
    private sealed class PreviewData : INotifyPropertyChanged
    {
        private string _label = "";
        public string Label { get => _label; set { _label = value; PropertyChanged?.Invoke(this, new(nameof(Label))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
