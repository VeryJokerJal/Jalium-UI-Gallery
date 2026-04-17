using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;
using Jalium.UI.Media.Animation;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Demo page for ContentTransition animations.
/// Shows all 32+ transition modes with interactive switching.
/// </summary>
public partial class TransitionDemoPage : Page
{
    private int _contentIndex;
    private DispatcherTimer? _autoPlayTimer;
    private int _autoPlayIndex;
    private bool _isAutoPlaying;

    private static readonly TransitionMode[] AllModes = Enum.GetValues<TransitionMode>();

    // Content panels to cycle through
    private static readonly Func<UIElement>[] ContentFactories =
    [
        CreateContentA,
        CreateContentB,
        CreateContentC,
    ];

    public TransitionDemoPage()
    {
        InitializeComponent();
        Unloaded += OnPageUnloaded;

        // Populate ComboBox with all TransitionMode values
        if (TransitionComboBox != null)
        {
            foreach (var mode in AllModes)
            {
                TransitionComboBox.Items.Add(new ComboBoxItem { Content = mode.ToString() });
            }
            TransitionComboBox.SelectedIndex = 0;
            TransitionComboBox.SelectionChanged += OnTransitionModeChanged;
        }

        // Set initial content
        if (TransitionControl != null)
        {
            TransitionControl.TransitionMode = Media.Animation.TransitionMode.Crossfade;
            TransitionControl.Content = CreateContentA();
        }

        if (SwitchButton != null)
            SwitchButton.Click += OnSwitchClick;

        if (AutoPlayButton != null)
            AutoPlayButton.Click += OnAutoPlayClick;

        LoadCodeExamples();
    }

    private void OnTransitionModeChanged(object? sender, EventArgs e)
    {
        if (TransitionComboBox == null || TransitionControl == null) return;

        var index = TransitionComboBox.SelectedIndex;
        if (index >= 0 && index < AllModes.Length)
        {
            var mode = AllModes[index];
            TransitionControl.TransitionMode = mode;

            if (CurrentTransitionText != null)
                CurrentTransitionText.Text = $"Current: {mode}";
        }
    }

    private void OnSwitchClick(object? sender, EventArgs e)
    {
        SwitchContent();
    }

    private void SwitchContent()
    {
        if (TransitionControl == null) return;

        _contentIndex = (_contentIndex + 1) % ContentFactories.Length;
        TransitionControl.Content = ContentFactories[_contentIndex]();
    }

    private void OnAutoPlayClick(object? sender, EventArgs e)
    {
        if (_isAutoPlaying)
        {
            StopAutoPlay();
            return;
        }

        _isAutoPlaying = true;
        _autoPlayIndex = 0;

        if (AutoPlayButton != null)
            AutoPlayButton.Content = "Stop";

        _autoPlayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1500) };
        _autoPlayTimer.Tick += OnAutoPlayTick;
        _autoPlayTimer.Start();

        // Start with first mode immediately
        PlayNextTransition();
    }

    private void OnAutoPlayTick(object? sender, EventArgs e)
    {
        PlayNextTransition();
    }

    private void PlayNextTransition()
    {
        if (_autoPlayIndex >= AllModes.Length)
        {
            StopAutoPlay();
            return;
        }

        var mode = AllModes[_autoPlayIndex];

        if (TransitionComboBox != null)
            TransitionComboBox.SelectedIndex = _autoPlayIndex;

        if (TransitionControl != null)
            TransitionControl.TransitionMode = mode;

        if (CurrentTransitionText != null)
            CurrentTransitionText.Text = $"Current: {mode} ({_autoPlayIndex + 1}/{AllModes.Length})";

        SwitchContent();
        _autoPlayIndex++;
    }

    private void StopAutoPlay()
    {
        _isAutoPlaying = false;
        if (_autoPlayTimer != null)
        {
            _autoPlayTimer.Tick -= OnAutoPlayTick;
            _autoPlayTimer.Stop();
            _autoPlayTimer = null;
        }

        if (AutoPlayButton != null)
            AutoPlayButton.Content = "Auto Play All";
    }

    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        StopAutoPlay();
    }

    private static UIElement CreateContentA()
    {
        var panel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        panel.Children.Add(new Border
        {
            Width = 120, Height = 120,
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)),
            CornerRadius = new CornerRadius(16),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 16),
            Child = new TextBlock
            {
                Text = "A",
                FontSize = 48,
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        panel.Children.Add(new TextBlock
        {
            Text = "Content Panel A",
            FontSize = 18,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
            HorizontalAlignment = HorizontalAlignment.Center
        });

        return panel;
    }

    private static UIElement CreateContentB()
    {
        var panel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        panel.Children.Add(new Border
        {
            Width = 120, Height = 120,
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 137, 62)),
            CornerRadius = new CornerRadius(60),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 16),
            Child = new TextBlock
            {
                Text = "B",
                FontSize = 48,
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        panel.Children.Add(new TextBlock
        {
            Text = "Content Panel B",
            FontSize = 18,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
            HorizontalAlignment = HorizontalAlignment.Center
        });

        return panel;
    }

    private static UIElement CreateContentC()
    {
        var panel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        panel.Children.Add(new Border
        {
            Width = 120, Height = 120,
            Background = new SolidColorBrush(Color.FromArgb(255, 200, 50, 50)),
            CornerRadius = new CornerRadius(8),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 16),
            Child = new TextBlock
            {
                Text = "C",
                FontSize = 48,
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        panel.Children.Add(new TextBlock
        {
            Text = "Content Panel C",
            FontSize = 18,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
            HorizontalAlignment = HorizontalAlignment.Center
        });

        return panel;
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
@"<!-- TransitioningContentControl with crossfade -->
<TransitioningContentControl x:Name=""TransitionControl""
                              TransitionMode=""Crossfade""
                              HorizontalAlignment=""Stretch""
                              VerticalAlignment=""Stretch""/>

<!-- Available TransitionMode values include:
     Crossfade, SlideLeft, SlideRight, SlideUp, SlideDown,
     ZoomIn, ZoomOut, FlipHorizontal, FlipVertical,
     RotateClockwise, RotateCounterClockwise, and more -->

<!-- Wrapping in a clipped border for smooth transitions -->
<Border Background=""#1A1A1A""
        CornerRadius=""8""
        Height=""300""
        ClipToBounds=""True"">
    <TransitioningContentControl x:Name=""MyTransition""
                                  TransitionMode=""SlideLeft""/>
</Border>";

    private const string CSharpExample =
@"// Set up a TransitioningContentControl
var transitionControl = new TransitioningContentControl();
transitionControl.TransitionMode = TransitionMode.Crossfade;

// Switch content to trigger the transition animation
transitionControl.Content = CreateNewContentPanel();

// Change transition mode dynamically
transitionControl.TransitionMode = TransitionMode.SlideLeft;
transitionControl.Content = CreateAnotherPanel();

// Auto-play through all transition modes
var allModes = Enum.GetValues<TransitionMode>();
var timer = new DispatcherTimer
{
    Interval = TimeSpan.FromMilliseconds(1500)
};

int index = 0;
timer.Tick += (s, e) =>
{
    if (index >= allModes.Length)
    {
        timer.Stop();
        return;
    }
    transitionControl.TransitionMode = allModes[index];
    transitionControl.Content = contentFactories[index % 3]();
    index++;
};
timer.Start();";
}
