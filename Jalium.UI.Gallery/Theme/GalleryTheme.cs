using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Theme;

/// <summary>
/// Defines the theme colors and styles for the Gallery application.
/// </summary>
public static class GalleryTheme
{
    #region Colors - Dark Theme (WinUI 3 Style)

    // Background colors
    public static Color BackgroundDark => Color.FromRgb(32, 32, 32);        // #202020
    public static Color BackgroundMedium => Color.FromRgb(40, 40, 40);      // #282828
    public static Color BackgroundLight => Color.FromRgb(48, 48, 48);       // #303030
    public static Color BackgroundCard => Color.FromRgb(44, 44, 44);        // #2C2C2C
    public static Color BackgroundHover => Color.FromRgb(55, 55, 55);       // #373737
    public static Color BackgroundPressed => Color.FromRgb(65, 65, 65);     // #414141

    // Accent colors (Blue)
    public static Color AccentPrimary => Color.FromRgb(0, 120, 212);        // #0078D4
    public static Color AccentSecondary => Color.FromRgb(0, 99, 177);       // #0063B1
    public static Color AccentLight => Color.FromRgb(76, 194, 255);         // #4CC2FF
    public static Color AccentDark => Color.FromRgb(0, 90, 158);            // #005A9E

    // Text colors
    public static Color TextPrimary => Color.FromRgb(255, 255, 255);        // White
    public static Color TextSecondary => Color.FromRgb(180, 180, 180);      // Light gray
    public static Color TextTertiary => Color.FromRgb(130, 130, 130);       // Gray
    public static Color TextDisabled => Color.FromRgb(90, 90, 90);          // Dark gray

    // Border colors
    public static Color BorderDefault => Color.FromRgb(60, 60, 60);         // #3C3C3C
    public static Color BorderFocused => Color.FromRgb(0, 120, 212);        // Accent
    public static Color BorderSubtle => Color.FromRgb(50, 50, 50);          // #323232

    // Status colors
    public static Color Success => Color.FromRgb(16, 124, 16);              // Green
    public static Color Warning => Color.FromRgb(252, 225, 0);              // Yellow
    public static Color Error => Color.FromRgb(196, 43, 28);                // Red
    public static Color Info => Color.FromRgb(0, 120, 212);                 // Blue

    #endregion

    #region Brushes

    public static SolidColorBrush BackgroundDarkBrush => new(BackgroundDark);
    public static SolidColorBrush BackgroundMediumBrush => new(BackgroundMedium);
    public static SolidColorBrush BackgroundLightBrush => new(BackgroundLight);
    public static SolidColorBrush BackgroundCardBrush => new(BackgroundCard);
    public static SolidColorBrush BackgroundHoverBrush => new(BackgroundHover);
    public static SolidColorBrush BackgroundPressedBrush => new(BackgroundPressed);

    public static SolidColorBrush AccentPrimaryBrush => new(AccentPrimary);
    public static SolidColorBrush AccentSecondaryBrush => new(AccentSecondary);
    public static SolidColorBrush AccentLightBrush => new(AccentLight);
    public static SolidColorBrush AccentDarkBrush => new(AccentDark);

    public static SolidColorBrush TextPrimaryBrush => new(TextPrimary);
    public static SolidColorBrush TextSecondaryBrush => new(TextSecondary);
    public static SolidColorBrush TextTertiaryBrush => new(TextTertiary);
    public static SolidColorBrush TextDisabledBrush => new(TextDisabled);

    public static SolidColorBrush BorderDefaultBrush => new(BorderDefault);
    public static SolidColorBrush BorderFocusedBrush => new(BorderFocused);
    public static SolidColorBrush BorderSubtleBrush => new(BorderSubtle);

    public static SolidColorBrush TransparentBrush => new(Color.Transparent);

    #endregion

    #region Dimensions

    public static double CornerRadiusSmall => 4;
    public static double CornerRadiusMedium => 8;
    public static double CornerRadiusLarge => 12;

    public static double SpacingTiny => 4;
    public static double SpacingSmall => 8;
    public static double SpacingMedium => 12;
    public static double SpacingLarge => 16;
    public static double SpacingXLarge => 24;

    public static double NavigationWidth => 280;
    public static double NavigationCollapsedWidth => 48;

    public static double CardPadding => 16;
    public static double ContentPadding => 24;

    #endregion

    #region Typography

    public static double FontSizeCaption => 12;
    public static double FontSizeBody => 14;
    public static double FontSizeSubtitle => 16;
    public static double FontSizeTitle => 20;
    public static double FontSizeHeader => 28;
    public static double FontSizeDisplay => 36;

    // Aliases for convenience
    public static double FontSizeSmall => FontSizeCaption;
    public static double FontSizeNormal => FontSizeBody;
    public static double FontSizeLarge => FontSizeSubtitle;

    #endregion
}
