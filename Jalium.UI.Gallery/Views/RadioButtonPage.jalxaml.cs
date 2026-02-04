using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for RadioButtonPage.jalxaml demonstrating RadioButton functionality.
/// </summary>
public partial class RadioButtonPage : Page
{
    public RadioButtonPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (RadioRed != null)
        {
            RadioRed.Checked += OnRadioChecked;
        }
        if (RadioGreen != null)
        {
            RadioGreen.Checked += OnRadioChecked;
        }
        if (RadioBlue != null)
        {
            RadioBlue.Checked += OnRadioChecked;
        }
    }

    private void OnRadioChecked(object sender, RoutedEventArgs e)
    {
        if (RadioStatus == null) return;

        if (sender == RadioRed)
        {
            RadioStatus.Text = "Selected: Red";
        }
        else if (sender == RadioGreen)
        {
            RadioStatus.Text = "Selected: Green";
        }
        else if (sender == RadioBlue)
        {
            RadioStatus.Text = "Selected: Blue";
        }
    }
}
