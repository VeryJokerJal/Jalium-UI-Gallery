using System.Collections.ObjectModel;
using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class MapViewViewModel
{
    private static readonly GeoPoint Shanghai =
        new(31.2304, 121.4737);

    private GeoPoint _zoomCenter = Shanghai;
    private double _zoomLevel = 10;

    public MapViewViewModel()
    {
        Markers =
        [
            Marker(39.9042, 116.4074, "Beijing", 0xE0, 0x3E, 0x3E),
            Marker(31.2304, 121.4737, "Shanghai", 0x21, 0x96, 0xF3),
            Marker(23.1291, 113.2644, "Guangzhou", 0x4C, 0xAF, 0x50),
            Marker(30.5728, 104.0668, "Chengdu", 0xFF, 0x98, 0x00),
            Marker(34.3416, 108.9398, "Xi'an", 0x9C, 0x27, 0xB0)
        ];

        var route = new ObservableCollection<GeoPoint>
        {
            new(39.9042, 116.4074),
            new(34.3416, 108.9398),
            new(30.5728, 104.0668),
            new(23.1291, 113.2644),
            new(31.2304, 121.4737)
        };
        Polylines =
        [
            new MapPolyline
            {
                Points = route,
                Stroke = new SolidColorBrush(
                    Color.FromRgb(0x41, 0x7E, 0xE0)),
                StrokeThickness = 3
            }
        ];

        ZoomInCommand = new RelayCommand(() =>
            ZoomLevel = Math.Min(ZoomLevel + 1, 20));
        ZoomOutCommand = new RelayCommand(() =>
            ZoomLevel = Math.Max(ZoomLevel - 1, 1));
        ResetViewCommand = new RelayCommand(() =>
        {
            ZoomCenter = Shanghai;
            ZoomLevel = 10;
        });
    }

    public GeoPoint BeijingCenter { get; } =
        new(39.9042, 116.4074);

    public GeoPoint ChinaCenter { get; } =
        new(33.0, 113.0);

    public MapTileSource TileSource { get; } =
        GalleryTileSources.Amap;

    public MapMarkerCollection Markers { get; }

    public MapPolylineCollection Polylines { get; }

    public ICommand ZoomInCommand { get; }

    public ICommand ZoomOutCommand { get; }

    public ICommand ResetViewCommand { get; }

    public GeoPoint ZoomCenter
    {
        get => _zoomCenter;
        set => SetProperty(ref _zoomCenter, value);
    }

    public double ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (SetProperty(ref _zoomLevel, value))
            {
                OnPropertyChanged(nameof(ZoomLevelText));
            }
        }
    }

    public string ZoomLevelText => $"Zoom Level: {ZoomLevel:F1}";

    private static MapMarker Marker(
        double latitude,
        double longitude,
        string label,
        byte red,
        byte green,
        byte blue) =>
        new()
        {
            Location = new GeoPoint(latitude, longitude),
            Label = label,
            Fill = new SolidColorBrush(Color.FromRgb(red, green, blue)),
            MarkerSize = 14
        };
}
