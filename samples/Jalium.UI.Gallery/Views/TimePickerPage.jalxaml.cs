using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for TimePickerPage.jalxaml demonstrating time picker functionality.
/// </summary>
public partial class TimePickerPage : Page
{
    public TimePickerPage()
    {
        InitializeComponent();

        // Set up event handler for the interactive demo
        if (DemoTimePicker != null)
        {
            DemoTimePicker.SelectedTimeChanged += OnDemoTimeChanged;
        }
    }

    private void OnDemoTimeChanged(object? sender, TimePickerSelectedValueChangedEventArgs e)
    {
        if (SelectedTimeText != null)
        {
            if (e.NewTime.HasValue)
            {
                SelectedTimeText.Text = $"Selected: {e.NewTime.Value:hh\\:mm}";
            }
            else
            {
                SelectedTimeText.Text = "No time selected";
            }
        }
    }
}
