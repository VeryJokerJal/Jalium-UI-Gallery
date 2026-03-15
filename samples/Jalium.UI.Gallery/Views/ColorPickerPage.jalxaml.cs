using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ColorPickerPage.jalxaml demonstrating color picker functionality.
/// </summary>
public partial class ColorPickerPage : Page
{
    public ColorPickerPage()
    {
        InitializeComponent();

        // Set up event handler for the interactive demo
        if (DemoColorPicker != null)
        {
            DemoColorPicker.ColorChanged += OnDemoColorChanged;
        }
    }

    private void OnDemoColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (ColorPreview != null)
        {
            ColorPreview.Background = new SolidColorBrush(e.NewColor);
        }

        if (ColorHexText != null)
        {
            ColorHexText.Text = $"#{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}";
        }
    }
}
