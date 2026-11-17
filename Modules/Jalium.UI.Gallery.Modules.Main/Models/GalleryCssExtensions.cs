using System.Globalization;
using Jalium.UI;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;
using Jalium.UI.Styling;

namespace Jalium.UI.Gallery.Modules.Main.Models;

/// <summary>
/// The custom CSS vocabulary demonstrated on the "Custom Properties" page, registered through
/// the public <see cref="CssMappings"/> surface. Registrations are global and permanent, so this
/// runs once; the names are prefixed to stay clear of the built-in table.
/// </summary>
public static class GalleryCssExtensions
{
    private static readonly Lock Gate = new();
    private static bool s_registered;

    /// <summary>Registers the demo vocabulary. Safe to call repeatedly.</summary>
    public static void EnsureRegistered()
    {
        lock (Gate)
        {
            if (s_registered)
            {
                return;
            }

            s_registered = true;
            RegisterAll();
        }
    }

    private static void RegisterAll()
    {
        // 1. An alias: a design-token name that redirects to a built-in property.
        CssMappings.RegisterAlias("brand-corner", "border-radius");

        // 2. A custom property bound to a fixed dependency property, with a custom converter.
        CssMappings.RegisterProperty(
            "gallery-elevation", UIElement.EffectProperty, new ElevationShadowConverter());

        // 3. Target resolved by dependency-property name against each element's runtime type —
        //    the right shape for a property that several owners declare separately. The alpha
        //    comes from the registration's converter parameter, which is why one converter
        //    class can back two css names at different strengths.
        var tint = new TintBrushConverter();
        CssMappings.RegisterProperty("gallery-tint", "Background", tint, converterParameter: 0.35);
        CssMappings.RegisterProperty("gallery-tint-strong", "Background", tint, converterParameter: 0.85);

        // 4. A multi-property converter that produces several assignments from one declaration.
        CssMappings.RegisterProperty("gallery-square", new SquareConverter());
    }

    /// <summary>Levels 1–5 become a matching drop shadow; anything else drops the declaration.</summary>
    private sealed class ElevationShadowConverter : ICssValueConverter
    {
        public object? Convert(string rawValue, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!int.TryParse(rawValue, NumberStyles.Integer, culture, out var level) ||
                level is < 1 or > 5)
            {
                return null;
            }

            var shadow = new DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 270,
                ShadowDepth = level * 2,
                BlurRadius = level * 6,
                Opacity = 0.18 + (level * 0.06),
            };

            // cacheByValue is on by default: the result is shared by every element that
            // declares the same text, so it must be frozen.
            shadow.Freeze();
            return shadow;
        }
    }

    /// <summary>Any CSS color, painted back at the alpha given as the converter parameter.</summary>
    private sealed class TintBrushConverter : ICssValueConverter
    {
        public object? Convert(string rawValue, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!CssValueParsing.TryParseColor(rawValue, out var color))
            {
                return null;
            }

            var alpha = parameter is double a ? a : 1.0;
            var brush = new SolidColorBrush(
                Color.FromArgb((byte)Math.Round(alpha * 255), color.R, color.G, color.B));
            brush.Freeze();
            return brush;
        }
    }

    /// <summary>One length in, both size properties out.</summary>
    private sealed class SquareConverter : ICssMultiPropertyConverter
    {
        public IReadOnlyList<CssPropertyAssignment>? Convert(
            string rawValue, FrameworkElement element, object? parameter, CultureInfo culture)
        {
            if (!CssValueParsing.TryParseLength(rawValue, out var side) || side <= 0)
            {
                return null;
            }

            return new[]
            {
                new CssPropertyAssignment(FrameworkElement.WidthProperty, side),
                new CssPropertyAssignment(FrameworkElement.HeightProperty, side),
            };
        }
    }
}
