using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for MapViewPage.jalxaml demonstrating MapView functionality.
/// </summary>
public partial class MapViewPage : Page
{
    private MapView? _zoomMap;
    private TextBlock? _zoomLevelText;

    private const string XamlExample = @"<!-- Basic Map -->
<MapView x:Name=""BasicMap""
         Height=""300""
         ZoomLevel=""12""
         ShowZoomControls=""True""
         ShowScaleBar=""True""
         ShowAttribution=""True""
         IsPanEnabled=""True""
         IsZoomEnabled=""True""/>

<!-- Map with Markers -->
<MapView x:Name=""MarkersMap""
         Height=""350""
         ZoomLevel=""5""
         ShowZoomControls=""True""
         ShowScaleBar=""True""/>";

    private const string CSharpExample = @"using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Media;

// Create a basic map centered on San Francisco
var map = new MapView
{
    Height = 300,
    Center = new GeoPoint(37.7749, -122.4194),
    ZoomLevel = 12,
    ShowZoomControls = true,
    ShowScaleBar = true,
    IsPanEnabled = true,
    IsZoomEnabled = true
};

// Add markers
var markers = new MapMarkerCollection
{
    new MapMarker
    {
        Location = new GeoPoint(48.8566, 2.3522),
        Label = ""Paris"",
        Fill = new SolidColorBrush(
            Color.FromRgb(0xE0, 0x3E, 0x3E)),
        MarkerSize = 14
    },
    new MapMarker
    {
        Location = new GeoPoint(51.5074, -0.1278),
        Label = ""London"",
        Fill = new SolidColorBrush(
            Color.FromRgb(0x21, 0x96, 0xF3)),
        MarkerSize = 14
    }
};
map.Markers = markers;

// Add a polyline route
var polyline = new MapPolyline
{
    Points = new ObservableCollection<GeoPoint>
    {
        new GeoPoint(51.5074, -0.1278),
        new GeoPoint(48.8566, 2.3522)
    },
    Stroke = new SolidColorBrush(
        Color.FromRgb(0x41, 0x7E, 0xE0)),
    StrokeThickness = 3
};";

    public MapViewPage()
    {
        InitializeComponent();
        SetupBasicMap();
        SetupMarkersMap();
        SetupZoomMap();
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

    private void SetupBasicMap()
    {
        if (BasicMapContainer == null) return;

        var map = new MapView
        {
            Height = 300,
            Center = new GeoPoint(37.7749, -122.4194), // San Francisco
            ZoomLevel = 12,
            ShowZoomControls = true,
            ShowScaleBar = true,
            ShowAttribution = true,
            IsPanEnabled = true,
            IsZoomEnabled = true
        };

        BasicMapContainer.Children.Add(map);
    }

    private void SetupMarkersMap()
    {
        if (MarkersMapContainer == null) return;

        var markers = new MapMarkerCollection
        {
            new MapMarker
            {
                Location = new GeoPoint(48.8566, 2.3522),
                Label = "Paris",
                Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x3E, 0x3E)),
                MarkerSize = 14
            },
            new MapMarker
            {
                Location = new GeoPoint(51.5074, -0.1278),
                Label = "London",
                Fill = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3)),
                MarkerSize = 14
            },
            new MapMarker
            {
                Location = new GeoPoint(52.5200, 13.4050),
                Label = "Berlin",
                Fill = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                MarkerSize = 14
            },
            new MapMarker
            {
                Location = new GeoPoint(41.9028, 12.4964),
                Label = "Rome",
                Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)),
                MarkerSize = 14
            },
            new MapMarker
            {
                Location = new GeoPoint(40.4168, -3.7038),
                Label = "Madrid",
                Fill = new SolidColorBrush(Color.FromRgb(0x9C, 0x27, 0xB0)),
                MarkerSize = 14
            }
        };

        var routePoints = new ObservableCollection<GeoPoint>
        {
            new GeoPoint(51.5074, -0.1278),  // London
            new GeoPoint(48.8566, 2.3522),   // Paris
            new GeoPoint(52.5200, 13.4050),  // Berlin
            new GeoPoint(41.9028, 12.4964),  // Rome
            new GeoPoint(40.4168, -3.7038)   // Madrid
        };

        var polyline = new MapPolyline
        {
            Points = routePoints,
            Stroke = new SolidColorBrush(Color.FromRgb(0x41, 0x7E, 0xE0)),
            StrokeThickness = 3
        };

        var polylines = new MapPolylineCollection();
        polylines.Add(polyline);

        var map = new MapView
        {
            Height = 350,
            Center = new GeoPoint(48.0, 5.0), // Center of Europe
            ZoomLevel = 5,
            ShowZoomControls = true,
            ShowScaleBar = true,
            Markers = markers,
            Polylines = polylines
        };

        MarkersMapContainer.Children.Add(map);
    }

    private void SetupZoomMap()
    {
        if (ZoomMapContainer == null) return;

        _zoomMap = new MapView
        {
            Height = 300,
            Center = new GeoPoint(35.6762, 139.6503), // Tokyo
            ZoomLevel = 10,
            ShowZoomControls = true,
            ShowScaleBar = true,
            IsPanEnabled = true,
            IsZoomEnabled = true
        };

        _zoomLevelText = new TextBlock
        {
            Text = "Zoom Level: 10",
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            FontSize = 12,
            Margin = new Thickness(0, 8, 0, 8)
        };

        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };

        var zoomInButton = new Button { Content = "Zoom In", Margin = new Thickness(0, 0, 8, 0) };
        zoomInButton.Click += OnZoomInClick;

        var zoomOutButton = new Button { Content = "Zoom Out", Margin = new Thickness(0, 0, 8, 0) };
        zoomOutButton.Click += OnZoomOutClick;

        var resetButton = new Button { Content = "Reset View" };
        resetButton.Click += OnResetViewClick;

        buttonPanel.Children.Add(zoomInButton);
        buttonPanel.Children.Add(zoomOutButton);
        buttonPanel.Children.Add(resetButton);

        ZoomMapContainer.Children.Add(_zoomMap);
        ZoomMapContainer.Children.Add(_zoomLevelText);
        ZoomMapContainer.Children.Add(buttonPanel);
    }

    private void OnZoomInClick(object sender, RoutedEventArgs e)
    {
        if (_zoomMap == null) return;
        _zoomMap.ZoomLevel = Math.Min(_zoomMap.ZoomLevel + 1, _zoomMap.MaxZoomLevel);
        UpdateZoomText();
    }

    private void OnZoomOutClick(object sender, RoutedEventArgs e)
    {
        if (_zoomMap == null) return;
        _zoomMap.ZoomLevel = Math.Max(_zoomMap.ZoomLevel - 1, _zoomMap.MinZoomLevel);
        UpdateZoomText();
    }

    private void OnResetViewClick(object sender, RoutedEventArgs e)
    {
        if (_zoomMap == null) return;
        _zoomMap.Center = new GeoPoint(35.6762, 139.6503);
        _zoomMap.ZoomLevel = 10;
        UpdateZoomText();
    }

    private void UpdateZoomText()
    {
        if (_zoomLevelText != null && _zoomMap != null)
        {
            _zoomLevelText.Text = $"Zoom Level: {_zoomMap.ZoomLevel:F1}";
        }
    }
}
