using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ProgressBarPage.jalxaml demonstrating ProgressBar functionality.
/// </summary>
public partial class ProgressBarPage : Page
{
    private const string XamlExample = @"<!-- Basic ProgressBar -->
<ProgressBar Value=""45""
             Minimum=""0""
             Maximum=""100""
             Height=""8""/>

<!-- ProgressBar with different values -->
<StackPanel Orientation=""Vertical"" Spacing=""12"">
    <ProgressBar Value=""25"" Maximum=""100"" Height=""8""/>
    <ProgressBar Value=""50"" Maximum=""100"" Height=""8""/>
    <ProgressBar Value=""75"" Maximum=""100"" Height=""8""/>
    <ProgressBar Value=""100"" Maximum=""100"" Height=""8""/>
</StackPanel>

<!-- Indeterminate ProgressBar (loading animation) -->
<ProgressBar IsIndeterminate=""True""
             Height=""8""/>

<!-- ProgressBar controlled by Slider -->
<StackPanel Orientation=""Vertical"">
    <ProgressBar x:Name=""DemoProgressBar""
                 Value=""45""
                 Minimum=""0""
                 Maximum=""100""
                 Height=""8""/>
    <StackPanel Orientation=""Horizontal"" Margin=""0,8,0,0"">
        <TextBlock Text=""Progress: ""/>
        <TextBlock x:Name=""ProgressLabel"" Text=""45%""/>
    </StackPanel>
    <Slider x:Name=""ProgressSlider""
            Minimum=""0""
            Maximum=""100""
            Value=""45""
            ValueChanged=""OnSliderValueChanged""/>
</StackPanel>

<!-- Custom height ProgressBars -->
<StackPanel Orientation=""Vertical"" Spacing=""8"">
    <ProgressBar Value=""60"" Maximum=""100"" Height=""4""/>
    <ProgressBar Value=""60"" Maximum=""100"" Height=""8""/>
    <ProgressBar Value=""60"" Maximum=""100"" Height=""16""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class DownloadPage : Page
{
    private DispatcherTimer? _timer;
    private double _progress;

    public DownloadPage()
    {
        InitializeComponent();

        ProgressSlider.ValueChanged += OnSliderValueChanged;

        StartButton.Click += OnStartDownload;
        CancelButton.Click += OnCancelDownload;
    }

    private void OnSliderValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        DemoProgressBar.Value = e.NewValue;
        ProgressLabel.Text = $""{(int)e.NewValue}%"";
    }

    private void OnStartDownload(object? sender, EventArgs e)
    {
        _progress = 0;
        DownloadProgress.IsIndeterminate = false;
        DownloadProgress.Value = 0;
        StatusText.Text = ""Downloading..."";

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();

        StartButton.IsEnabled = false;
        CancelButton.IsEnabled = true;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _progress += 0.5;
        DownloadProgress.Value = _progress;
        ProgressText.Text = $""{(int)_progress}%"";

        if (_progress >= 100)
        {
            _timer?.Stop();
            StatusText.Text = ""Download complete!"";
            StartButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
        }
    }

    private void OnCancelDownload(object? sender, EventArgs e)
    {
        _timer?.Stop();
        DownloadProgress.Value = 0;
        StatusText.Text = ""Download cancelled."";
        StartButton.IsEnabled = true;
        CancelButton.IsEnabled = false;
    }
}";

    public ProgressBarPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (ProgressSlider != null)
        {
            ProgressSlider.ValueChanged += OnSliderValueChanged;
        }

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
