using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for RepeatButtonPage.jalxaml demonstrating RepeatButton functionality.
/// </summary>
public partial class RepeatButtonPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Counter with RepeatButtons -->
    <TextBlock Text=""Counter Demo"" FontSize=""16"" Margin=""0,0,0,12""/>
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,16"">
        <RepeatButton Content=""-""
                      Width=""40"" Height=""40""
                      Delay=""500""
                      Interval=""100""
                      Click=""OnDecrementClick""
                      Margin=""0,0,8,0""/>
        <TextBlock x:Name=""CounterText""
                   Text=""0""
                   Width=""60""
                   TextAlignment=""Center""
                   FontSize=""24""
                   VerticalAlignment=""Center""/>
        <RepeatButton Content=""+""
                      Width=""40"" Height=""40""
                      Delay=""500""
                      Interval=""100""
                      Click=""OnIncrementClick""
                      Margin=""8,0,0,0""/>
    </StackPanel>

    <!-- Volume control with custom intervals -->
    <TextBlock Text=""Volume Control"" FontSize=""16"" Margin=""0,0,0,12""/>
    <StackPanel Orientation=""Horizontal"">
        <RepeatButton Content=""&#xE738;""
                      FontFamily=""Segoe MDL2 Assets""
                      Width=""36"" Height=""36""
                      Delay=""300""
                      Interval=""50""
                      Click=""OnVolumeDown""/>
        <Slider x:Name=""VolumeSlider""
                Width=""200""
                Minimum=""0"" Maximum=""100""
                Value=""50""
                VerticalAlignment=""Center""
                Margin=""8,0,8,0""/>
        <RepeatButton Content=""&#xE995;""
                      FontFamily=""Segoe MDL2 Assets""
                      Width=""36"" Height=""36""
                      Delay=""300""
                      Interval=""50""
                      Click=""OnVolumeUp""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class RepeatButtonSample : Page
{
    private int _counter = 0;

    public RepeatButtonSample()
    {
        InitializeComponent();
        SetupRepeatButtons();
    }

    private void SetupRepeatButtons()
    {
        // Create RepeatButton programmatically
        var incrementBtn = new RepeatButton
        {
            Content = ""+"",
            Width = 40,
            Height = 40,
            Delay = 500,     // ms before repeating starts
            Interval = 100   // ms between repeat clicks
        };
        incrementBtn.Click += OnIncrementClick;

        // Fast repeat button (shorter delay & interval)
        var fastBtn = new RepeatButton
        {
            Content = "">>"",
            Delay = 200,
            Interval = 50
        };
        fastBtn.Click += OnFastForward;

        // Slow repeat button (longer intervals)
        var slowBtn = new RepeatButton
        {
            Content = ""Step"",
            Delay = 1000,
            Interval = 500
        };
        slowBtn.Click += OnStepClick;
    }

    private void OnIncrementClick(object? sender, RoutedEventArgs e)
    {
        _counter++;
        CounterText.Text = _counter.ToString();
    }

    private void OnFastForward(object? sender, RoutedEventArgs e)
    {
        _counter += 10;
        CounterText.Text = _counter.ToString();
    }

    private void OnStepClick(object? sender, RoutedEventArgs e)
    {
        _counter++;
        CounterText.Text = _counter.ToString();
    }
}";

    private int _counter = 0;

    public RepeatButtonPage()
    {
        InitializeComponent();
        SetupDemo();
        LoadCodeExamples();
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

    private void SetupDemo()
    {
        if (IncrementButton != null)
        {
            IncrementButton.Click += OnIncrementClick;
        }

        if (DecrementButton != null)
        {
            DecrementButton.Click += OnDecrementClick;
        }
    }

    private void OnIncrementClick(object? sender, RoutedEventArgs e)
    {
        _counter++;
        UpdateCounterDisplay();
    }

    private void OnDecrementClick(object? sender, RoutedEventArgs e)
    {
        _counter--;
        UpdateCounterDisplay();
    }

    private void UpdateCounterDisplay()
    {
        if (CounterText != null)
        {
            CounterText.Text = _counter.ToString();
        }
    }
}
