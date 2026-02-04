using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for DatePickerPage.jalxaml demonstrating date picker functionality.
/// </summary>
public partial class DatePickerPage : Page
{
    public DatePickerPage()
    {
        InitializeComponent();

        // Set up event handler for the interactive demo
        if (DemoDatePicker != null)
        {
            DemoDatePicker.SelectedDateChanged += OnDemoDateChanged;
        }
    }

    private void OnDemoDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedDateText != null && DemoDatePicker != null)
        {
            if (DemoDatePicker.SelectedDate.HasValue)
            {
                SelectedDateText.Text = $"Selected: {DemoDatePicker.SelectedDate.Value:D}";
            }
            else
            {
                SelectedDateText.Text = "No date selected";
            }
        }
    }
}
