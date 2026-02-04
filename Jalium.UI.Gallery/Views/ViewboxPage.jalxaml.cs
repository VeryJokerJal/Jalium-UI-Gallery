using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ViewboxPage.jalxaml demonstrating viewbox functionality.
/// </summary>
public partial class ViewboxPage : Page
{
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
