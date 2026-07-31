using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class GalleryColorEditor
{
    private readonly Action<Color> _apply;
    private readonly TextBlock _hexValue;
    private readonly Border _colorPreview;
    private readonly GalleryNumericStepper _red;
    private readonly GalleryNumericStepper _green;
    private readonly GalleryNumericStepper _blue;
    private bool _isUpdating;

    public GalleryColorEditor(
        string label,
        Color initialColor,
        IReadOnlyList<Color> palette,
        Action<Color> apply)
    {
        _apply = apply;
        Root = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 16)
        };

        var heading = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        heading.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        heading.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        heading.Children.Add(new TextBlock
        {
            Text = label,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextSecondaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        });

        var valuePanel = new StackPanel { Orientation = Orientation.Horizontal };
        _colorPreview = new Border
        {
            Width = 16,
            Height = 16,
            CornerRadius = new CornerRadius(3),
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 6, 0)
        };
        valuePanel.Children.Add(_colorPreview);
        _hexValue = new TextBlock
        {
            FontSize = 10,
            Foreground = GalleryTheme.TextMutedBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        valuePanel.Children.Add(_hexValue);
        Grid.SetColumn(valuePanel, 1);
        heading.Children.Add(valuePanel);
        Root.Children.Add(heading);

        var swatches = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 10)
        };
        foreach (var color in palette)
        {
            var captured = color;
            var swatch = new Button
            {
                Width = 28,
                Height = 28,
                MinWidth = 28,
                MinHeight = 28,
                Margin = new Thickness(0, 0, 7, 0),
                Background = new SolidColorBrush(color),
                BorderBrush = GalleryTheme.BorderDefaultBrush,
                BorderThickness = new Thickness(1),
                ToolTip = GalleryThemeTuner.ToHex(color)
            };
            swatch.Click += (_, _) => SetColor(captured);
            swatches.Children.Add(swatch);
        }
        Root.Children.Add(swatches);

        _red = CreateChannel("R", initialColor.R);
        _green = CreateChannel("G", initialColor.G);
        _blue = CreateChannel("B", initialColor.B);
        Root.Children.Add(_red.Root);
        Root.Children.Add(_green.Root);
        Root.Children.Add(_blue.Root);
        SetColor(initialColor, apply: false);
    }

    public StackPanel Root { get; }

    public Color CurrentColor { get; private set; }

    public void SetColor(Color color) => SetColor(color, apply: true);

    private void SetColor(Color color, bool apply)
    {
        _isUpdating = true;
        _red.SetValue(color.R, apply: false);
        _green.SetValue(color.G, apply: false);
        _blue.SetValue(color.B, apply: false);
        _isUpdating = false;

        CurrentColor = color;
        UpdateReadout(color);
        if (apply)
        {
            _apply(color);
        }
    }

    private void ApplyChannels()
    {
        if (_isUpdating)
        {
            return;
        }

        var color = Color.FromRgb(
            (byte)Math.Round(_red.Value),
            (byte)Math.Round(_green.Value),
            (byte)Math.Round(_blue.Value));
        CurrentColor = color;
        UpdateReadout(color);
        _apply(color);
    }

    private void UpdateReadout(Color color)
    {
        _hexValue.Text = GalleryThemeTuner.ToHex(color);
        _colorPreview.Background = new SolidColorBrush(color);
    }

    private GalleryNumericStepper CreateChannel(string label, byte value)
    {
        return new GalleryNumericStepper(
            label,
            0,
            255,
            value,
            5,
            channel => $"{channel:0}",
            _ => ApplyChannels());
    }
}
