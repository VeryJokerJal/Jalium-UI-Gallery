using System.Globalization;
using Jalium.UI.Data;

namespace Jalium.UI.Gallery.Modules.Main.Converters;

/// <summary>
/// Keeps segmented View choices synchronized with a string property in a
/// ViewModel without introducing selection logic into code-behind.
/// </summary>
public sealed class StringEqualsConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) =>
        string.Equals(
            value as string,
            parameter as string,
            StringComparison.Ordinal);

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) =>
        value is true
            ? parameter?.ToString() ?? string.Empty
            : Binding.DoNothing;
}
