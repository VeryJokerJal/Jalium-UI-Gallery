using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

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

        LoadCodeExamples();
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
@"<!-- Basic TimePicker -->
<TimePicker Width=""150""/>

<!-- Disabled TimePicker -->
<TimePicker Width=""150"" IsEnabled=""False""/>

<!-- 12-hour clock format -->
<TimePicker Width=""150"" ClockIdentifier=""12HourClock""/>

<!-- 24-hour clock format -->
<TimePicker Width=""150"" ClockIdentifier=""24HourClock""/>

<!-- TimePicker with different minute increments -->
<TimePicker Width=""150"" MinuteIncrement=""1""/>
<TimePicker Width=""150"" MinuteIncrement=""5""/>
<TimePicker Width=""150"" MinuteIncrement=""15""/>
<TimePicker Width=""150"" MinuteIncrement=""30""/>

<!-- Interactive TimePicker with event binding -->
<TimePicker x:Name=""DemoTimePicker"" Width=""150""/>
<TextBlock x:Name=""SelectedTimeText""
           Text=""No time selected""
           Foreground=""#0078D4""/>";

    private const string CSharpExample =
@"// Create a TimePicker programmatically
var timePicker = new TimePicker
{
    Width = 150,
    ClockIdentifier = ""24HourClock"",
    MinuteIncrement = 15
};

// Set a default time
timePicker.SelectedTime = new TimeSpan(9, 30, 0);

// Handle time selection changes
timePicker.SelectedTimeChanged += (sender, e) =>
{
    if (e.NewTime.HasValue)
    {
        var time = e.NewTime.Value;
        Console.WriteLine($""Selected: {time:hh\\:mm}"");
    }
};

// Use 12-hour clock format
timePicker.ClockIdentifier = ""12HourClock"";

// Set minute increment for appointment scheduling
timePicker.MinuteIncrement = 15; // 15-minute slots

// Common use cases
var alarmPicker = new TimePicker
{
    MinuteIncrement = 1,
    ClockIdentifier = ""12HourClock""
};

var meetingPicker = new TimePicker
{
    MinuteIncrement = 15,
    ClockIdentifier = ""24HourClock""
};

var reminderPicker = new TimePicker
{
    MinuteIncrement = 5
};";
}
