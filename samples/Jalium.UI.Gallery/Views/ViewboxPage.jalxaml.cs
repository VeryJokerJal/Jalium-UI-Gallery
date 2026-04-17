using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ViewboxPage.jalxaml demonstrating viewbox functionality.
/// </summary>
public partial class ViewboxPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Uniform Stretch: scales preserving aspect ratio -->
    <Border BorderBrush=""#3D3D3D"" BorderThickness=""1"" Width=""150"" Height=""100"">
        <Viewbox Stretch=""Uniform"">
            <Button Content=""Scale Me"" Width=""200"" Height=""50""/>
        </Viewbox>
    </Border>

    <!-- Stretch Modes Comparison -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,16,0,0"">
        <!-- None: no scaling applied -->
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""100"" Height=""80"" Margin=""0,0,16,0"">
            <Viewbox Stretch=""None"">
                <Ellipse Width=""60"" Height=""40"" Fill=""#0078D4""/>
            </Viewbox>
        </Border>

        <!-- Fill: stretches to fill, ignoring aspect ratio -->
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""100"" Height=""80"" Margin=""0,0,16,0"">
            <Viewbox Stretch=""Fill"">
                <Ellipse Width=""60"" Height=""40"" Fill=""#00CC6A""/>
            </Viewbox>
        </Border>

        <!-- Uniform: fits inside while preserving ratio -->
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""100"" Height=""80"" Margin=""0,0,16,0"">
            <Viewbox Stretch=""Uniform"">
                <Ellipse Width=""60"" Height=""40"" Fill=""#F7630C""/>
            </Viewbox>
        </Border>

        <!-- UniformToFill: fills while preserving ratio (may clip) -->
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""100"" Height=""80"" ClipToBounds=""True"">
            <Viewbox Stretch=""UniformToFill"">
                <Ellipse Width=""60"" Height=""40"" Fill=""#E81123""/>
            </Viewbox>
        </Border>
    </StackPanel>

    <!-- StretchDirection: control scale up/down behavior -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,16,0,0"">
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""120"" Height=""80"" Margin=""0,0,16,0"">
            <Viewbox Stretch=""Uniform"" StretchDirection=""UpOnly"">
                <TextBlock Text=""Small"" FontSize=""8"" Foreground=""#0078D4""/>
            </Viewbox>
        </Border>
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""60"" Height=""40"" Margin=""0,0,16,0"">
            <Viewbox Stretch=""Uniform"" StretchDirection=""DownOnly"">
                <TextBlock Text=""Large"" FontSize=""24"" Foreground=""#00CC6A""/>
            </Viewbox>
        </Border>
        <Border BorderBrush=""#3D3D3D"" BorderThickness=""1""
                Width=""100"" Height=""60"">
            <Viewbox Stretch=""Uniform"" StretchDirection=""Both"">
                <TextBlock Text=""Both"" FontSize=""16"" Foreground=""#F7630C""/>
            </Viewbox>
        </Border>
    </StackPanel>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;
using Jalium.UI.Shapes;

namespace MyApp;

public partial class ViewboxDemo : Page
{
    private Border _demoContainer;

    public ViewboxDemo()
    {
        InitializeComponent();
        SetupInteractiveDemo();
        CreateIconScalingExample();
    }

    private void SetupInteractiveDemo()
    {
        // Create a resizable container with Viewbox
        _demoContainer = new Border
        {
            Width = 200,
            Height = 150,
            BorderBrush = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
            BorderThickness = new Thickness(1)
        };

        var viewbox = new Viewbox
        {
            Stretch = Stretch.Uniform
        };

        var content = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Width = 200
        };
        content.Children.Add(new TextBlock
        {
            Text = ""Viewbox Demo"",
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Center
        });
        content.Children.Add(new Button
        {
            Content = ""Scaled Button"",
            Width = 120,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        });

        viewbox.Child = content;
        _demoContainer.Child = viewbox;

        // Use sliders to resize the container
        WidthSlider.ValueChanged += (s, e) =>
        {
            _demoContainer.Width = e.NewValue;
            WidthText.Text = ((int)e.NewValue).ToString();
        };

        HeightSlider.ValueChanged += (s, e) =>
        {
            _demoContainer.Height = e.NewValue;
            HeightText.Text = ((int)e.NewValue).ToString();
        };
    }

    private void CreateIconScalingExample()
    {
        // Scale the same icon to different sizes
        var sizes = new[] { 16, 24, 32, 48, 64 };
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 16, 0, 0)
        };

        foreach (var size in sizes)
        {
            var viewbox = new Viewbox
            {
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(8)
            };

            // Original icon at fixed design size
            var icon = new Ellipse
            {
                Width = 24,
                Height = 24,
                Fill = new SolidColorBrush(Color.FromRgb(0, 120, 212))
            };

            viewbox.Child = icon;
            panel.Children.Add(viewbox);
        }

        ContentPanel.Children.Add(panel);
    }
}";

    public ViewboxPage()
    {
        InitializeComponent();

        // Set up slider event handlers
        if (WidthSlider != null)
        {
            WidthSlider.ValueChanged += OnWidthSliderChanged;
        }

        if (HeightSlider != null)
        {
            HeightSlider.ValueChanged += OnHeightSliderChanged;
        }

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

    private void OnWidthSliderChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DemoContainer != null && WidthText != null)
        {
            DemoContainer.Width = e.NewValue;
            WidthText.Text = ((int)e.NewValue).ToString();
        }
    }

    private void OnHeightSliderChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DemoContainer != null && HeightText != null)
        {
            DemoContainer.Height = e.NewValue;
            HeightText.Text = ((int)e.NewValue).ToString();
        }
    }
}
