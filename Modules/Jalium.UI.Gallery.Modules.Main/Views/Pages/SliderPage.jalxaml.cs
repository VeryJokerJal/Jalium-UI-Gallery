using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for SliderPage.jalxaml demonstrating Slider functionality.
/// </summary>
public partial class SliderPage : Page
{
    private const string XamlExample = @"<!-- Basic Slider -->
<Slider Minimum=""0""
        Maximum=""100""
        Value=""50""
        Height=""24""/>

<!-- Slider with tick marks -->
<Slider Minimum=""0""
        Maximum=""100""
        Value=""50""
        TickFrequency=""10""
        Height=""24""/>

<!-- Snap-to-tick Slider -->
<Slider Minimum=""0""
        Maximum=""100""
        Value=""25""
        TickFrequency=""25""
        IsSnapToTickEnabled=""True""
        Height=""24""/>

<!-- Slider with value display -->
<StackPanel Orientation=""Vertical"">
    <Slider x:Name=""VolumeSlider""
            Minimum=""0""
            Maximum=""100""
            Value=""75""
            Height=""24""
            ValueChanged=""OnVolumeChanged""/>
    <TextBlock x:Name=""VolumeLabel""
               Text=""Volume: 75%""
               Foreground=""#AAAAAA""/>
</StackPanel>

<!-- Custom range Slider -->
<StackPanel Orientation=""Vertical"">
    <TextBlock Text=""Temperature (-10 to 40 C)""/>
    <Slider Minimum=""-10""
            Maximum=""40""
            Value=""22""
            TickFrequency=""5""
            Height=""24""/>
</StackPanel>

<!-- Disabled Slider -->
<Slider Minimum=""0""
        Maximum=""100""
        Value=""30""
        IsEnabled=""False""
        Height=""24""/>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class AudioSettingsPage : Page
{
    public AudioSettingsPage()
    {
        InitializeComponent();

        VolumeSlider.ValueChanged += OnVolumeChanged;
        BassSlider.ValueChanged += OnBassChanged;
        TrebleSlider.ValueChanged += OnTrebleChanged;

        // Set initial values
        VolumeSlider.Value = 75;
        BassSlider.Value = 50;
        TrebleSlider.Value = 50;
    }

    private void OnVolumeChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        VolumeLabel.Text = $""Volume: {(int)e.NewValue}%"";

        // Mute icon when volume is 0
        MuteIcon.Opacity = e.NewValue > 0 ? 0.0 : 1.0;
    }

    private void OnBassChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        BassLabel.Text = $""Bass: {(int)e.NewValue}"";
    }

    private void OnTrebleChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        TrebleLabel.Text = $""Treble: {(int)e.NewValue}"";
    }

    // Create a slider programmatically
    private Slider CreateSlider(
        double min, double max, double value,
        double tickFrequency = 1,
        bool snapToTicks = false)
    {
        return new Slider
        {
            Minimum = min,
            Maximum = max,
            Value = value,
            TickFrequency = tickFrequency,
            IsSnapToTickEnabled = snapToTicks,
            Height = 24
        };
    }
}";

    public SliderPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (DemoSlider != null)
        {
            DemoSlider.ValueChanged += OnSliderValueChanged;
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
        if (SliderValue != null)
        {
            SliderValue.Text = $"{(int)e.NewValue}";
        }
    }
}
