using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for MiniMapPage.jalxaml demonstrating MiniMap functionality.
/// </summary>
public partial class MiniMapPage : Page
{
    private const string XamlExample = @"<!-- MiniMap targeting a ScrollViewer -->
<ScrollViewer x:Name=""MyScrollViewer""
              Height=""250""
              HorizontalScrollBarVisibility=""Auto""
              VerticalScrollBarVisibility=""Auto"">
    <!-- Large content here -->
</ScrollViewer>

<MiniMap Target=""{Binding ElementName=MyScrollViewer}""
         Width=""200""
         Height=""150""
         ContentRenderMode=""Outline""
         HorizontalAlignment=""Right""/>

<!-- MiniMap targeting a MapView -->
<MapView x:Name=""MyMap""
         Height=""300""
         ZoomLevel=""4""
         ShowZoomControls=""True""/>

<MiniMap MapViewTarget=""{Binding ElementName=MyMap}""
         Width=""180""
         Height=""120""
         ContentRenderMode=""Simplified""
         HorizontalAlignment=""Right""/>";

    private const string CSharpExample = @"using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Media;

// Create a ScrollViewer with content
var scrollViewer = new ScrollViewer
{
    Height = 250,
    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
    Content = largeContent
};

// Create a MiniMap for the ScrollViewer
var miniMap = new MiniMap
{
    Target = scrollViewer,
    Width = 200,
    Height = 150,
    ContentRenderMode = MiniMapRenderMode.Outline,
    HorizontalAlignment = HorizontalAlignment.Right
};

// MiniMap for a MapView with highlight markers
var mapMiniMap = new MiniMap
{
    MapViewTarget = mapView,
    Width = 180,
    Height = 120,
    ContentRenderMode = MiniMapRenderMode.Simplified,
    HighlightMarkers = new ObservableCollection<MiniMapMarker>
    {
        new MiniMapMarker
        {
            X = 0.7, Y = 0.4,
            Color = new SolidColorBrush(
                Color.FromRgb(0xE0, 0x3E, 0x3E)),
            Size = 6
        }
    }
};

// Render modes: None, Outline, Simplified
var outlineMap = new MiniMap
{
    Target = scrollViewer,
    Width = 150,
    Height = 100,
    ContentRenderMode = MiniMapRenderMode.Outline
};";

    public MiniMapPage()
    {
        InitializeComponent();
        SetupBasicMiniMap();
        SetupMapMiniMap();
        SetupRenderModesMiniMap();
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

    private void SetupBasicMiniMap()
    {
        if (BasicMiniMapContainer == null) return;

        // Create a ScrollViewer with large content
        var largeContent = new StackPanel { Orientation = Orientation.Vertical };

        for (int i = 0; i < 30; i++)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal };
            for (int j = 0; j < 5; j++)
            {
                var block = new Border
                {
                    Width = 150,
                    Height = 60,
                    Background = new SolidColorBrush(Color.FromRgb(
                        (byte)(40 + (i * 7) % 60),
                        (byte)(40 + (j * 13) % 60),
                        (byte)(60 + ((i + j) * 11) % 80))),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x3D, 0x3D, 0x3D)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(4),
                    Padding = new Thickness(8)
                };

                var text = new TextBlock
                {
                    Text = $"Item {i * 5 + j + 1}",
                    Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
                    FontSize = 11
                };

                block.Child = text;
                row.Children.Add(block);
            }
            largeContent.Children.Add(row);
        }

        var scrollViewer = new ScrollViewer
        {
            Height = 250,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = largeContent
        };

        var miniMap = new MiniMap
        {
            Target = scrollViewer,
            Width = 200,
            Height = 150,
            ContentRenderMode = MiniMapRenderMode.Outline,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 8, 0, 0)
        };

        BasicMiniMapContainer.Children.Add(scrollViewer);
        BasicMiniMapContainer.Children.Add(miniMap);
    }

    private void SetupMapMiniMap()
    {
        if (MapMiniMapContainer == null) return;

        var markers = new MapMarkerCollection
        {
            new MapMarker
            {
                Location = new GeoPoint(40.7128, -74.0060),
                Label = "New York",
                Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x3E, 0x3E)),
                MarkerSize = 12
            },
            new MapMarker
            {
                Location = new GeoPoint(34.0522, -118.2437),
                Label = "Los Angeles",
                Fill = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3)),
                MarkerSize = 12
            },
            new MapMarker
            {
                Location = new GeoPoint(41.8781, -87.6298),
                Label = "Chicago",
                Fill = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                MarkerSize = 12
            }
        };

        var mapView = new MapView
        {
            Height = 300,
            Center = new GeoPoint(39.8283, -98.5795), // Center of US
            ZoomLevel = 4,
            ShowZoomControls = true,
            Markers = markers
        };

        var miniMap = new MiniMap
        {
            MapViewTarget = mapView,
            Width = 180,
            Height = 120,
            ContentRenderMode = MiniMapRenderMode.Simplified,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 8, 0, 0),
            HighlightMarkers = new ObservableCollection<MiniMapMarker>
            {
                new MiniMapMarker { X = 0.7, Y = 0.4, Color = new SolidColorBrush(Color.FromRgb(0xE0, 0x3E, 0x3E)), Size = 6 },
                new MiniMapMarker { X = 0.2, Y = 0.5, Color = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3)), Size = 6 },
                new MiniMapMarker { X = 0.55, Y = 0.35, Color = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)), Size = 6 }
            }
        };

        MapMiniMapContainer.Children.Add(mapView);
        MapMiniMapContainer.Children.Add(miniMap);
    }

    private void SetupRenderModesMiniMap()
    {
        if (RenderModesMiniMapContainer == null) return;

        // Create a shared target with some content
        var targetContent = new StackPanel { Orientation = Orientation.Vertical };
        for (int i = 0; i < 8; i++)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal };
            for (int j = 0; j < 4; j++)
            {
                var block = new Border
                {
                    Width = 120,
                    Height = 40,
                    Background = new SolidColorBrush(Color.FromRgb(
                        (byte)(50 + (i * 17) % 80),
                        (byte)(50 + (j * 23) % 80),
                        (byte)(70 + ((i + j) * 19) % 90))),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3),
                    Margin = new Thickness(3)
                };
                row.Children.Add(block);
            }
            targetContent.Children.Add(row);
        }

        var scrollViewer = new ScrollViewer
        {
            Height = 180,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = targetContent
        };

        var miniMapRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };

        // None mode
        var nonePanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 16, 0) };
        nonePanel.Children.Add(new TextBlock
        {
            Text = "None",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 4)
        });
        nonePanel.Children.Add(new MiniMap
        {
            Target = scrollViewer,
            Width = 150,
            Height = 100,
            ContentRenderMode = MiniMapRenderMode.None
        });
        miniMapRow.Children.Add(nonePanel);

        // Outline mode
        var outlinePanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 16, 0) };
        outlinePanel.Children.Add(new TextBlock
        {
            Text = "Outline",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 4)
        });
        outlinePanel.Children.Add(new MiniMap
        {
            Target = scrollViewer,
            Width = 150,
            Height = 100,
            ContentRenderMode = MiniMapRenderMode.Outline
        });
        miniMapRow.Children.Add(outlinePanel);

        // Simplified mode
        var simplifiedPanel = new StackPanel { Orientation = Orientation.Vertical };
        simplifiedPanel.Children.Add(new TextBlock
        {
            Text = "Simplified",
            Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 4)
        });
        simplifiedPanel.Children.Add(new MiniMap
        {
            Target = scrollViewer,
            Width = 150,
            Height = 100,
            ContentRenderMode = MiniMapRenderMode.Simplified
        });
        miniMapRow.Children.Add(simplifiedPanel);

        RenderModesMiniMapContainer.Children.Add(scrollViewer);
        RenderModesMiniMapContainer.Children.Add(miniMapRow);
    }
}
