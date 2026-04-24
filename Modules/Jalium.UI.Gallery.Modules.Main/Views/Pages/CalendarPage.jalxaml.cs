using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for CalendarPage.jalxaml demonstrating Calendar functionality.
/// </summary>
public partial class CalendarPage : Page
{
    public CalendarPage()
    {
        InitializeComponent();
        SetupDemo();
        LoadCodeExamples();
    }

    private void SetupDemo()
    {
        if (DemoCalendar != null)
        {
            DemoCalendar.SelectedDateChanged += OnSelectedDateChanged;
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

    private const string XamlExample = @"<Calendar x:Name=""DemoCalendar""
          SelectionMode=""SingleDate""
          DisplayDateStart=""2024-01-01""
          DisplayDateEnd=""2024-12-31"" />";

    private const string CSharpExample = @"// Create and configure a Calendar
var calendar = new Calendar();
calendar.SelectionMode = CalendarSelectionMode.SingleDate;
calendar.DisplayDateStart = new DateTime(2024, 1, 1);
calendar.DisplayDateEnd = new DateTime(2024, 12, 31);

// Handle date selection
calendar.SelectedDateChanged += (s, e) =>
{
    if (calendar.SelectedDate != null)
    {
        var date = calendar.SelectedDate.Value;
        Console.WriteLine($""Selected: {date:yyyy-MM-dd}"");
    }
};";

    private void OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedDateText != null && DemoCalendar?.SelectedDate != null)
        {
            SelectedDateText.Text = DemoCalendar.SelectedDate.Value.ToString("yyyy-MM-dd");
        }
        else if (SelectedDateText != null)
        {
            SelectedDateText.Text = "None";
        }
    }
}
