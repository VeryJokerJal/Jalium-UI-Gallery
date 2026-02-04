using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ProgressBarPage.jalxaml demonstrating ProgressBar functionality.
/// </summary>
public partial class ProgressBarPage : Page
{
    public ProgressBarPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (ProgressSlider != null)
        {
            ProgressSlider.ValueChanged += OnSliderValueChanged;
        }
    }

    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DemoProgressBar != null)
        {
            DemoProgressBar.Value = e.NewValue;
        }
        if (ProgressValue != null)
        {
            ProgressValue.Text = $"{(int)e.NewValue}%";
        }
    }
}
