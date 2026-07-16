using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class WrapPanelPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Horizontal WrapPanel with variable-width items -->
    <WrapPanel Orientation=""Horizontal""
               Width=""400"">
        <Border Background=""#0078D4"" Width=""80"" Height=""40"" Margin=""4"" CornerRadius=""4"">
            <TextBlock Text=""Item 1"" Foreground=""White"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#4CAF50"" Width=""100"" Height=""40"" Margin=""4"" CornerRadius=""4"">
            <TextBlock Text=""Item 2"" Foreground=""White"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#FF9800"" Width=""120"" Height=""40"" Margin=""4"" CornerRadius=""4"">
            <TextBlock Text=""Item 3"" Foreground=""White"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
    </WrapPanel>

    <!-- Vertical WrapPanel -->
    <WrapPanel Orientation=""Vertical""
               Height=""150"">
        <Button Content=""A"" Width=""60"" Height=""30"" Margin=""4""/>
        <Button Content=""B"" Width=""60"" Height=""40"" Margin=""4""/>
        <Button Content=""C"" Width=""60"" Height=""50"" Margin=""4""/>
    </WrapPanel>

    <!-- Fixed Item Size WrapPanel -->
    <WrapPanel Orientation=""Horizontal""
               ItemWidth=""80""
               ItemHeight=""80"">
        <Border Background=""#0078D4"" CornerRadius=""4"" Margin=""4"">
            <TextBlock Text=""1"" Foreground=""White"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#4CAF50"" CornerRadius=""4"" Margin=""4"">
            <TextBlock Text=""2"" Foreground=""White"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
    </WrapPanel>

    <!-- Tag Cloud using WrapPanel -->
    <WrapPanel Orientation=""Horizontal"">
        <Border Background=""#0078D4"" CornerRadius=""12"" Padding=""12,6"" Margin=""4"">
            <TextBlock Text=""C#"" Foreground=""White"" FontSize=""12""/>
        </Border>
        <Border Background=""#0078D4"" CornerRadius=""12"" Padding=""12,6"" Margin=""4"">
            <TextBlock Text=""XAML"" Foreground=""White"" FontSize=""12""/>
        </Border>
        <Border Background=""#0078D4"" CornerRadius=""12"" Padding=""12,6"" Margin=""4"">
            <TextBlock Text=""UI Framework"" Foreground=""White"" FontSize=""12""/>
        </Border>
    </WrapPanel>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace MyApp;

public partial class WrapPanelDemo : Page
{
    public WrapPanelDemo()
    {
        InitializeComponent();
        CreateTagCloud();
        CreatePhotoGallery();
    }

    private void CreateTagCloud()
    {
        var wrapPanel = new WrapPanel
        {
            Orientation = Orientation.Horizontal
        };

        var tags = new[] { ""C#"", ""WPF"", ""XAML"", ""UI Framework"",
                           "".NET"", ""Windows"", ""Desktop"", ""Controls"" };

        foreach (var tag in tags)
        {
            var tagBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12, 6, 12, 6),
                Margin = new Thickness(4)
            };

            tagBorder.Child = new TextBlock
            {
                Text = tag,
                Foreground = new SolidColorBrush(Color.White),
                FontSize = 12
            };

            wrapPanel.Children.Add(tagBorder);
        }

        ContentPanel.Children.Add(wrapPanel);
    }

    private void CreatePhotoGallery()
    {
        var gallery = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            ItemWidth = 120,
            ItemHeight = 120
        };

        var colors = new[]
        {
            Color.FromRgb(0, 120, 212),
            Color.FromRgb(76, 175, 80),
            Color.FromRgb(255, 152, 0),
            Color.FromRgb(156, 39, 176)
        };

        for (int i = 0; i < 8; i++)
        {
            var thumb = new Border
            {
                Background = new SolidColorBrush(colors[i % colors.Length]),
                Margin = new Thickness(4),
                CornerRadius = new CornerRadius(8)
            };

            thumb.Child = new TextBlock
            {
                Text = $""Photo {i + 1}"",
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            gallery.Children.Add(thumb);
        }

        ContentPanel.Children.Add(gallery);
    }
}";

    public WrapPanelPage()
    {
        InitializeComponent();
        CreateContent();
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

    private void CreateContent()
    {
        if (DemoHost == null) return;

        var colors = new[]
        {
            Color.FromRgb(0, 120, 212),
            Color.FromRgb(76, 175, 80),
            Color.FromRgb(255, 152, 0),
            Color.FromRgb(156, 39, 176),
            Color.FromRgb(33, 150, 243),
            Color.FromRgb(244, 67, 54),
            Color.FromRgb(0, 150, 136),
            Color.FromRgb(255, 87, 34)
        };

        // Horizontal WrapPanel — items flow left to right and wrap to the next row.
        var horizontalWrapPanel = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            Width = 400
        };
        for (int i = 0; i < 12; i++)
        {
            var color = colors[i % colors.Length];
            horizontalWrapPanel.Children.Add(CreateWrapItem($"Item {i + 1}", color, 80 + (i % 3) * 20, 40));
        }
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Horizontal WrapPanel", "Items flow from left to right, wrapping to the next row.", horizontalWrapPanel));

        // Vertical WrapPanel — items flow top to bottom and wrap to the next column.
        var verticalWrapPanel = new WrapPanel
        {
            Orientation = Orientation.Vertical,
            Width = 400,
            Height = 150
        };
        for (int i = 0; i < 10; i++)
        {
            var color = colors[i % colors.Length];
            verticalWrapPanel.Children.Add(CreateWrapItem($"V{i + 1}", color, 60, 30 + (i % 3) * 10));
        }
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Vertical WrapPanel", "Items flow from top to bottom, wrapping to the next column.", verticalWrapPanel));

        // Fixed item size — ItemWidth/ItemHeight give every child a uniform cell.
        var fixedWrapPanel = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            ItemWidth = 80,
            ItemHeight = 80,
            Width = 400
        };
        for (int i = 0; i < 8; i++)
        {
            var color = colors[i % colors.Length];
            fixedWrapPanel.Children.Add(CreateWrapItem($"{i + 1}", color, 60, 60));
        }
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Fixed Item Size", "Using ItemWidth and ItemHeight for uniform sizing.", fixedWrapPanel));

        // Tag cloud — a practical WrapPanel use case.
        var tagWrapPanel = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            Width = 400
        };
        var tags = new[] { "C#", "WPF", "XAML", "UI Framework", ".NET", "Windows", "Desktop", "Controls", "Layout", "Styling" };
        foreach (var tag in tags)
        {
            var tagBorder = new Border
            {
                Background = GalleryTheme.AccentPrimaryBrush,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12, 6, 12, 6),
                Margin = new Thickness(4)
            };

            var tagText = new TextBlock
            {
                Text = tag,
                Foreground = new SolidColorBrush(Color.White),
                FontSize = 12
            };

            tagBorder.Child = tagText;
            tagWrapPanel.Children.Add(tagBorder);
        }
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Tag Cloud Example", "A practical use case for WrapPanel.", tagWrapPanel));
    }

    private Border CreateWrapItem(string text, Color color, double width, double height)
    {
        var border = new Border
        {
            Width = width,
            Height = height,
            Background = new SolidColorBrush(color),
            Margin = new Thickness(4),
            CornerRadius = new CornerRadius(4)
        };

        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Color.White),
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        border.Child = textBlock;
        return border;
    }
}
