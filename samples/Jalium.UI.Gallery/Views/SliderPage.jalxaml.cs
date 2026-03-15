using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for SliderPage.jalxaml demonstrating Slider functionality.
/// </summary>
public partial class SliderPage : Page
{
    public SliderPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (DemoSlider != null)
        {
            DemoSlider.ValueChanged += OnSliderValueChanged;
        }
    }

    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (SliderValue != null)
        {
            SliderValue.Text = $"{(int)e.NewValue}";
        }
    }
}
