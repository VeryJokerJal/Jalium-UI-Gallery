using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for RepeatButtonPage.jalxaml demonstrating RepeatButton functionality.
/// </summary>
public partial class RepeatButtonPage : Page
{
    private int _counter = 0;

    public RepeatButtonPage()
    {
        InitializeComponent();
        SetupDemo();
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
