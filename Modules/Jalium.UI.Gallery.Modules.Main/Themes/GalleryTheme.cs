using Jalium.UI;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Selected visual mode for the Gallery palette.
/// </summary>
public enum GalleryThemeMode
{
    Dark,
    Light
}

/// <summary>
/// Aureate-inspired palette with two modes — deep purple "Dark" and a warm
/// pearl-white "Light" — both anchored by the same amber accent. All gallery
/// pages read tokens from this class instead of hard-coding colors, so the
/// shell can be re-skinned live when <see cref="CurrentMode"/> changes.
/// </summary>
public static class GalleryTheme
{
    /// <summary>
    /// Fires whenever <see cref="CurrentMode"/> changes. Listeners should
    /// rebuild any parts of the UI that sampled the brushes eagerly.
    /// </summary>
    public static event EventHandler? ModeChanged;

    private static GalleryThemeMode _currentMode = GalleryThemeMode.Dark;

    /// <summary>
    /// Currently active visual mode. Assigning a new value fires
    /// <see cref="ModeChanged"/> so consumers can re-skin their chrome.
    /// </summary>
    public static GalleryThemeMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode == value) return;
            _currentMode = value;
            ModeChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    private static bool IsDark => _currentMode == GalleryThemeMode.Dark;

    private static Color Pick(Color dark, Color light) => IsDark ? dark : light;

    #region Surface palette

    /// <summary>App-level background (deepest).</summary>
    public static Color BackgroundDark => Pick(
        Color.FromRgb(0x0B, 0x08, 0x14),   // deep indigo black
        Color.FromRgb(0xF6, 0xF4, 0xFB));  // soft lavender-white

    /// <summary>Shell / navigation pane background.</summary>
    public static Color BackgroundMedium => Pick(
        Color.FromRgb(0x13, 0x10, 0x1F),
        Color.FromRgb(0xFF, 0xFF, 0xFF));

    /// <summary>Scroll host background below the cards.</summary>
    public static Color BackgroundLight => Pick(
        Color.FromRgb(0x17, 0x14, 0x26),
        Color.FromRgb(0xFA, 0xF8, 0xFF));

    /// <summary>Primary card surface (raised).</summary>
    public static Color BackgroundCard => Pick(
        Color.FromRgb(0x1B, 0x18, 0x2B),
        Color.FromRgb(0xFF, 0xFF, 0xFF));

    /// <summary>Nested card / inner example container.</summary>
    public static Color BackgroundCardInner => Pick(
        Color.FromRgb(0x12, 0x0F, 0x1E),
        Color.FromRgb(0xF2, 0xEE, 0xFA));

    /// <summary>Hover surface.</summary>
    public static Color BackgroundHover => Pick(
        Color.FromRgb(0x27, 0x22, 0x3C),
        Color.FromRgb(0xEC, 0xE8, 0xF5));

    /// <summary>Pressed / selected surface.</summary>
    public static Color BackgroundPressed => Pick(
        Color.FromRgb(0x33, 0x2B, 0x4E),
        Color.FromRgb(0xDC, 0xD3, 0xF0));

    #endregion

    #region Accent palette (warm amber / gold — identical in both modes)

    public static Color AccentPrimary => Color.FromRgb(0xFF, 0xD6, 0x0A);
    public static Color AccentSecondary => Color.FromRgb(0xFB, 0xBF, 0x24);
    public static Color AccentLight => Color.FromRgb(0xFF, 0xE4, 0x5C);
    public static Color AccentDark => Color.FromRgb(0xB8, 0x8A, 0x06);

    /// <summary>Soft tint used behind highlighted content.</summary>
    public static Color AccentSoft => IsDark
        ? Color.FromArgb(0x28, 0xFF, 0xD6, 0x0A)   // ~16% yellow on dark
        : Color.FromArgb(0x3B, 0xF5, 0x9E, 0x0B);  // ~23% amber on light

    /// <summary>Purple halo for hero headers / gradients.</summary>
    public static Color HaloPurple => Pick(
        Color.FromRgb(0x6D, 0x28, 0xD9),
        Color.FromRgb(0x8B, 0x5C, 0xF6));

    /// <summary>Blue halo used in gradients.</summary>
    public static Color HaloBlue => Pick(
        Color.FromRgb(0x1E, 0x3A, 0x8A),
        Color.FromRgb(0x60, 0xA5, 0xFA));

    #endregion

    #region Text palette

    public static Color TextPrimary => Pick(
        Color.FromRgb(0xFA, 0xFA, 0xFA),
        Color.FromRgb(0x18, 0x18, 0x1B));

    public static Color TextSecondary => Pick(
        Color.FromRgb(0xD4, 0xD4, 0xD8),
        Color.FromRgb(0x3F, 0x3F, 0x46));

    public static Color TextTertiary => Pick(
        Color.FromRgb(0xA1, 0xA1, 0xAA),
        Color.FromRgb(0x52, 0x52, 0x5B));

    public static Color TextMuted => Pick(
        Color.FromRgb(0x71, 0x71, 0x7A),
        Color.FromRgb(0x71, 0x71, 0x7A));

    public static Color TextDisabled => Pick(
        Color.FromRgb(0x52, 0x52, 0x5B),
        Color.FromRgb(0xA1, 0xA1, 0xAA));

    #endregion

    #region Borders

    public static Color BorderDefault => Pick(
        Color.FromRgb(0x2B, 0x27, 0x42),
        Color.FromRgb(0xE4, 0xE1, 0xEC));

    public static Color BorderSubtle => Pick(
        Color.FromRgb(0x23, 0x1F, 0x38),
        Color.FromRgb(0xEF, 0xEC, 0xF5));

    public static Color BorderStrong => Pick(
        Color.FromRgb(0x3B, 0x33, 0x59),
        Color.FromRgb(0xCF, 0xC9, 0xDE));

    public static Color BorderFocused => AccentPrimary;

    /// <summary>Faint inner highlight used for glass top-edge.</summary>
    public static Color BorderGlassHighlight => IsDark
        ? Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF)
        : Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF);

    #endregion

    #region Status colors (constant across modes)

    public static Color Success => Color.FromRgb(0x22, 0xC5, 0x5E);
    public static Color Warning => Color.FromRgb(0xF5, 0x9E, 0x0B);
    public static Color Error => Color.FromRgb(0xEF, 0x44, 0x44);
    public static Color Info => Color.FromRgb(0x38, 0xBD, 0xF8);

    #endregion

    #region Brushes (each getter returns a fresh brush so the theme flip is picked up on rebuild)

    public static SolidColorBrush BackgroundDarkBrush => new(BackgroundDark);
    public static SolidColorBrush BackgroundMediumBrush => new(BackgroundMedium);
    public static SolidColorBrush BackgroundLightBrush => new(BackgroundLight);
    public static SolidColorBrush BackgroundCardBrush => new(BackgroundCard);
    public static SolidColorBrush BackgroundCardInnerBrush => new(BackgroundCardInner);
    public static SolidColorBrush BackgroundHoverBrush => new(BackgroundHover);
    public static SolidColorBrush BackgroundPressedBrush => new(BackgroundPressed);

    public static SolidColorBrush AccentPrimaryBrush => new(AccentPrimary);
    public static SolidColorBrush AccentSecondaryBrush => new(AccentSecondary);
    public static SolidColorBrush AccentLightBrush => new(AccentLight);
    public static SolidColorBrush AccentDarkBrush => new(AccentDark);
    public static SolidColorBrush AccentSoftBrush => new(AccentSoft);

    public static SolidColorBrush TextPrimaryBrush => new(TextPrimary);
    public static SolidColorBrush TextSecondaryBrush => new(TextSecondary);
    public static SolidColorBrush TextTertiaryBrush => new(TextTertiary);
    public static SolidColorBrush TextMutedBrush => new(TextMuted);
    public static SolidColorBrush TextDisabledBrush => new(TextDisabled);

    public static SolidColorBrush BorderDefaultBrush => new(BorderDefault);
    public static SolidColorBrush BorderSubtleBrush => new(BorderSubtle);
    public static SolidColorBrush BorderStrongBrush => new(BorderStrong);
    public static SolidColorBrush BorderFocusedBrush => new(BorderFocused);
    public static SolidColorBrush BorderGlassHighlightBrush => new(BorderGlassHighlight);

    public static SolidColorBrush SuccessBrush => new(Success);
    public static SolidColorBrush WarningBrush => new(Warning);
    public static SolidColorBrush ErrorBrush => new(Error);
    public static SolidColorBrush InfoBrush => new(Info);

    public static SolidColorBrush TransparentBrush => new(Color.Transparent);

    #endregion

    #region DynamicResource keys and refresh

    // Keys used as {DynamicResource ...} across every gallery page so they
    // automatically re-skin when the mode flips.
    public const string KeyCardBackground = "GalleryCardBackground";
    public const string KeyCardInnerBackground = "GalleryCardInnerBackground";
    public const string KeyCardAltBackground = "GalleryCardAltBackground";
    public const string KeyShellBackground = "GalleryShellBackground";

    public const string KeyBorderDefault = "GalleryBorderDefault";
    public const string KeyBorderSubtle = "GalleryBorderSubtle";
    public const string KeyBorderStrong = "GalleryBorderStrong";

    public const string KeyTextPrimary = "GalleryTextPrimary";
    public const string KeyTextSecondary = "GalleryTextSecondary";
    public const string KeyTextTertiary = "GalleryTextTertiary";
    public const string KeyTextMuted = "GalleryTextMuted";

    public const string KeyAccentPrimary = "GalleryAccent";
    public const string KeyAccentLight = "GalleryAccentLight";
    public const string KeyAccentSecondary = "GalleryAccentSecondary";

    /// <summary>
    /// Writes every gallery-scoped brush into the supplied dictionary using
    /// the <c>Key*</c> constants above. Call at startup and whenever the mode
    /// flips so <c>{DynamicResource ...}</c> references in jalxaml pick up the
    /// new palette.
    /// </summary>
    public static void RegisterResources(ResourceDictionary resources)
    {
        if (resources == null) return;

        resources[KeyCardBackground] = BackgroundCardBrush;
        resources[KeyCardInnerBackground] = BackgroundCardInnerBrush;
        resources[KeyCardAltBackground] = BackgroundLightBrush;
        resources[KeyShellBackground] = BackgroundDarkBrush;

        resources[KeyBorderDefault] = BorderDefaultBrush;
        resources[KeyBorderSubtle] = BorderSubtleBrush;
        resources[KeyBorderStrong] = BorderStrongBrush;

        resources[KeyTextPrimary] = TextPrimaryBrush;
        resources[KeyTextSecondary] = TextSecondaryBrush;
        resources[KeyTextTertiary] = TextTertiaryBrush;
        resources[KeyTextMuted] = TextMutedBrush;

        resources[KeyAccentPrimary] = AccentPrimaryBrush;
        resources[KeyAccentLight] = AccentLightBrush;
        resources[KeyAccentSecondary] = AccentSecondaryBrush;
    }

    /// <summary>Ambient gradient used for the scroll host / shell background.</summary>
    public static LinearGradientBrush ShellBackgroundBrush
    {
        get
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            if (IsDark)
            {
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x14, 0x0F, 0x25), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0E, 0x0B, 0x1B), 0.55));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0A, 0x08, 0x14), 1.0));
            }
            else
            {
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0xFD, 0xFB, 0xFF), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0xF6, 0xF2, 0xFB), 0.55));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0xEF, 0xEB, 0xF7), 1.0));
            }

            return brush;
        }
    }

    /// <summary>Glowing hero gradient used on home / category headers.</summary>
    public static LinearGradientBrush HeroGradientBrush
    {
        get
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            if (IsDark)
            {
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x2A, 0x13, 0x4A), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x1B, 0x17, 0x36), 0.55));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x1F, 0x14, 0x35), 1.0));
            }
            else
            {
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0xFE, 0xF5, 0xC8), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0xEA, 0xDD, 0xFB), 0.55));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0xDF, 0xE7, 0xFF), 1.0));
            }

            return brush;
        }
    }

    #endregion

    #region Dimensions

    public static double CornerRadiusSmall => 8;
    public static double CornerRadiusMedium => 12;
    public static double CornerRadiusLarge => 16;
    public static double CornerRadiusXLarge => 22;

    public static double SpacingTiny => 4;
    public static double SpacingSmall => 8;
    public static double SpacingMedium => 12;
    public static double SpacingLarge => 16;
    public static double SpacingXLarge => 24;
    public static double SpacingHuge => 32;

    public static double NavigationWidth => 280;
    public static double NavigationCollapsedWidth => 56;

    public static double CardPadding => 20;
    public static double ContentPadding => 28;

    #endregion

    #region Typography

    public static double FontSizeCaption => 12;
    public static double FontSizeBody => 14;
    public static double FontSizeSubtitle => 16;
    public static double FontSizeTitle => 20;
    public static double FontSizeHeader => 28;
    public static double FontSizeDisplay => 36;
    public static double FontSizeHero => 44;

    // Aliases for convenience
    public static double FontSizeSmall => FontSizeCaption;
    public static double FontSizeNormal => FontSizeBody;
    public static double FontSizeLarge => FontSizeSubtitle;

    #endregion
}
