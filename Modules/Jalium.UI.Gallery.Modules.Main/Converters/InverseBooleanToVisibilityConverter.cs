using System.Globalization;
using Jalium.UI.Data;

namespace Jalium.UI.Gallery.Modules.Main.Converters;

public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) =>
        value is Visibility visibility && visibility != Visibility.Visible;
}
