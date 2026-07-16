using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class GalleryNumericStepper
{
    private readonly double _minimum;
    private readonly double _maximum;
    private readonly double _step;
    private readonly Func<double, string> _formatter;
    private readonly Action<double> _apply;
    private readonly TextBlock _valueText;
    private double _value;

    public GalleryNumericStepper(
        string label,
        double minimum,
        double maximum,
        double initialValue,
        double step,
        Func<double, string> formatter,
        Action<double> apply)
    {
        _minimum = minimum;
        _maximum = maximum;
        _step = step;
        _formatter = formatter;
        _apply = apply;

        Root = new Grid { Margin = new Thickness(0, 0, 0, 10) };
        Root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Root.Children.Add(new TextBlock
        {
            Text = label,
            FontSize = 11,
            Foreground = GalleryTheme.TextSecondaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        });

        var controls = new StackPanel { Orientation = Orientation.Horizontal };
        controls.Children.Add(CreateButton("-", $"Decrease {label}", () => SetValue(_value - _step)));
        _valueText = new TextBlock
        {
            Width = 54,
            FontSize = 10,
            Foreground = GalleryTheme.TextMutedBrush,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        controls.Children.Add(_valueText);
        controls.Children.Add(CreateButton("+", $"Increase {label}", () => SetValue(_value + _step)));
        Grid.SetColumn(controls, 1);
        Root.Children.Add(controls);

        SetValue(initialValue, apply: false);
    }

    public Grid Root { get; }

    public double Value => _value;

    public void SetValue(double value) => SetValue(value, apply: true);

    public void SetValue(double value, bool apply)
    {
        var next = Math.Clamp(value, _minimum, _maximum);
        _value = next;
        _valueText.Text = _formatter(next);
        if (apply)
        {
            _apply(next);
        }
    }

    private static Button CreateButton(string content, string toolTip, Action action)
    {
        var button = new Button
        {
            Content = content,
            Width = 28,
            Height = 28,
            MinWidth = 28,
            MinHeight = 28,
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            Foreground = GalleryTheme.TextSecondaryBrush,
            ToolTip = toolTip
        };
        button.Click += (_, _) => action();
        return button;
    }
}
