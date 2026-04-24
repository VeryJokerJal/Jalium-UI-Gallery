using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

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

        LoadCodeExamples();
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

    private const string XamlExample =
@"<!-- Basic DatePicker -->
<DatePicker Width=""200""/>

<!-- Disabled DatePicker -->
<DatePicker Width=""200"" IsEnabled=""False""/>

<!-- DatePicker with short date format -->
<DatePicker Width=""200"" SelectedDateFormat=""Short""/>

<!-- DatePicker with long date format -->
<DatePicker Width=""250"" SelectedDateFormat=""Long""/>

<!-- DatePicker with date range constraints -->
<StackPanel Orientation=""Vertical"">
    <TextBlock Text=""Start Date:"" Foreground=""#888888""/>
    <DatePicker x:Name=""StartDatePicker"" Width=""200""/>

    <TextBlock Text=""End Date:"" Foreground=""#888888""/>
    <DatePicker x:Name=""EndDatePicker"" Width=""200""/>
</StackPanel>

<!-- Interactive DatePicker with event binding -->
<DatePicker x:Name=""DemoDatePicker"" Width=""200""/>
<TextBlock x:Name=""SelectedDateText""
           Text=""No date selected""
           Foreground=""#0078D4""/>";

    private const string CSharpExample =
@"// Create a DatePicker programmatically
var datePicker = new DatePicker
{
    Width = 200,
    SelectedDateFormat = DatePickerFormat.Short
};

// Set a default selected date
datePicker.SelectedDate = DateTime.Today;

// Handle date selection changes
datePicker.SelectedDateChanged += (sender, e) =>
{
    if (datePicker.SelectedDate.HasValue)
    {
        var date = datePicker.SelectedDate.Value;
        Console.WriteLine($""Selected: {date:D}"");
    }
};

// Set date range constraints
datePicker.DisplayDateStart = DateTime.Today;
datePicker.DisplayDateEnd = DateTime.Today.AddMonths(6);

// Blackout specific dates (e.g., holidays)
datePicker.BlackoutDates.Add(
    new CalendarDateRange(
        new DateTime(2026, 12, 25),
        new DateTime(2026, 12, 26)));

// Use long date format
datePicker.SelectedDateFormat = DatePickerFormat.Long;

// Coordinate start/end date pickers
startDatePicker.SelectedDateChanged += (s, e) =>
{
    endDatePicker.DisplayDateStart =
        startDatePicker.SelectedDate ?? DateTime.Today;
};";
}
