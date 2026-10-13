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
/// Forest-emerald palette with two modes — deep forest "Dark" and a crisp
/// mint "Light" — both anchored by the #207245 -> #1C8043 accent gradient
/// that matches the Jalium.UI framework theme. All gallery pages read tokens
/// from this class instead of hard-coding colors, so the shell can be
/// re-skinned live when <see cref="CurrentMode"/> changes.
/// </summary>
public static class GalleryTheme
{
    /// <summary>
    /// Fires whenever <see cref="CurrentMode"/> changes. Listeners should
    /// rebuild any parts of the UI that sampled the brushes eagerly.
    /// </summary>
    public static event EventHandler? ModeChanged;

    private static GalleryThemeMode _currentMode = GalleryThemeMode.Light;

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
            RefreshSharedBrushes();
            ModeChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    private static bool IsDark => _currentMode == GalleryThemeMode.Dark;

    private static Color Pick(Color dark, Color light) => IsDark ? dark : light;

    private readonly record struct ThemeColorPair(Color Dark, Color Light);

    #region Surface palette

    /// <summary>App-level background (deepest).</summary>
    public static Color BackgroundDark => Pick(
        Color.FromRgb(0x12, 0x17, 0x19),
        Color.FromRgb(0xF5, 0xF7, 0xF8));

    /// <summary>Shell / navigation pane background.</summary>
    public static Color BackgroundMedium => Pick(
        Color.FromRgb(0x17, 0x1D, 0x1F),
        Color.FromRgb(0xFB, 0xFC, 0xFC));

    /// <summary>Scroll host background below the cards.</summary>
    public static Color BackgroundLight => Pick(
        Color.FromRgb(0x15, 0x1B, 0x1D),
        Color.FromRgb(0xF7, 0xF9, 0xFA));

    /// <summary>Primary card surface (raised).</summary>
    public static Color BackgroundCard => Pick(
        Color.FromRgb(0x1C, 0x23, 0x26),
        Color.FromRgb(0xFF, 0xFF, 0xFF));

    /// <summary>Nested card / inner example container.</summary>
    public static Color BackgroundCardInner => Pick(
        Color.FromRgb(0x12, 0x17, 0x19),
        Color.FromRgb(0xF6, 0xF8, 0xF9));

    /// <summary>Hover surface.</summary>
    public static Color BackgroundHover => Pick(
        Color.FromRgb(0x24, 0x2D, 0x30),
        Color.FromRgb(0xED, 0xF7, 0xF5));

    /// <summary>Pressed / selected surface.</summary>
    public static Color BackgroundPressed => Pick(
        Color.FromRgb(0x2B, 0x36, 0x39),
        Color.FromRgb(0xD9, 0xEF, 0xEC));

    #endregion

    #region Accent palette (forest emerald — matches the #207245 -> #1C8043 gradient)

    public static Color AccentPrimary => Color.FromRgb(0x08, 0x94, 0x8A);
    public static Color AccentSecondary => Color.FromRgb(0x0B, 0x7F, 0x78);
    public static Color AccentLight => Color.FromRgb(0x12, 0xA8, 0x9B);
    public static Color AccentDark => Color.FromRgb(0x06, 0x6F, 0x69);

    /// <summary>
    /// Accent ink drawn directly on a neutral surface. The darker shade clears
    /// contrast on Light surfaces while the brighter shade stays legible on Dark.
    /// </summary>
    public static Color AccentOnSurface => Pick(AccentLight, AccentDark);

    /// <summary>High-contrast ink rendered on the primary accent fill.</summary>
    public static Color TextOnAccent => Color.FromRgb(0x10, 0x18, 0x27);

    /// <summary>Soft tint used behind highlighted content.</summary>
    public static Color AccentSoft => IsDark
        ? Color.FromArgb(0x40, 0x08, 0x94, 0x8A)
        : Color.FromArgb(0x34, 0x08, 0x94, 0x8A);

    /// <summary>Deep-emerald halo for hero headers / gradients.</summary>
    public static Color HaloPurple => Pick(
        Color.FromRgb(0x62, 0x5C, 0xD6),
        Color.FromRgb(0x4F, 0x46, 0xE5));

    /// <summary>Teal halo used in gradients.</summary>
    public static Color HaloBlue => Pick(
        Color.FromRgb(0x0E, 0x74, 0x8A),
        Color.FromRgb(0x0E, 0xA5, 0xE9));

    #endregion

    #region Text palette

    public static Color TextPrimary => Pick(
        Color.FromRgb(0xF4, 0xF7, 0xF7),
        Color.FromRgb(0x10, 0x18, 0x27));

    public static Color TextSecondary => Pick(
        Color.FromRgb(0xCF, 0xD7, 0xD9),
        Color.FromRgb(0x35, 0x40, 0x52));

    public static Color TextTertiary => Pick(
        Color.FromRgb(0x9E, 0xAA, 0xAE),
        Color.FromRgb(0x64, 0x70, 0x85));

    public static Color TextMuted => Pick(
        Color.FromRgb(0x78, 0x85, 0x89),
        Color.FromRgb(0x87, 0x92, 0xA5));

    public static Color TextDisabled => Pick(
        Color.FromRgb(0x58, 0x64, 0x68),
        Color.FromRgb(0xA7, 0xB0, 0xBC));

    #endregion

    #region Borders

    public static Color BorderDefault => Pick(
        Color.FromRgb(0x32, 0x3C, 0x40),
        Color.FromRgb(0xDE, 0xE4, 0xE8));

    public static Color BorderSubtle => Pick(
        Color.FromRgb(0x28, 0x32, 0x35),
        Color.FromRgb(0xE9, 0xED, 0xF0));

    public static Color BorderStrong => Pick(
        Color.FromRgb(0x45, 0x52, 0x56),
        Color.FromRgb(0xC8, 0xD2, 0xD9));

    public static Color BorderFocused => AccentPrimary;

    /// <summary>Faint inner highlight used for glass top-edge.</summary>
    public static Color BorderGlassHighlight => IsDark
        ? Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF)
        : Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF);

    #endregion

    #region Status colors

    // Mode-invariant FILL colors. Correct for a filled badge / chip / swatch,
    // where the hue sits on its own opaque area and carries its own ink.
    public static Color Success => Color.FromRgb(0x22, 0xC5, 0x5E);
    public static Color Warning => Color.FromRgb(0xF5, 0x9E, 0x0B);
    public static Color Error => Color.FromRgb(0xEF, 0x44, 0x44);
    public static Color Info => Color.FromRgb(0x38, 0xBD, 0xF8);

    // Mode-aware ON-SURFACE variants. The fill colors above are NOT usable as a
    // foreground: measured against the light demo surface, Warning #F59E0B is
    // only 2.1:1 and Success #22C55E only 2.1:1. These pick a darker shade in
    // Light and the brighter fill shade in Dark, so status text and status
    // glyphs clear 4.5:1 in both modes. Use these for Foreground; use the fills
    // above for Background.
    public static Color SuccessOnSurface => Pick(
        Color.FromRgb(0x22, 0xC5, 0x5E),
        Color.FromRgb(0x15, 0x80, 0x3D));

    public static Color WarningOnSurface => Pick(
        Color.FromRgb(0xF5, 0x9E, 0x0B),
        Color.FromRgb(0xB4, 0x53, 0x09));

    public static Color ErrorOnSurface => Pick(
        Color.FromRgb(0xF8, 0x71, 0x71),
        Color.FromRgb(0xB9, 0x1C, 0x1C));

    public static Color InfoOnSurface => Pick(
        Color.FromRgb(0x38, 0xBD, 0xF8),
        Color.FromRgb(0x03, 0x69, 0xA1));

    // Translucent status washes, mirroring the AccentSoft alpha scheme so a
    // status container keeps the same visual weight in both modes.
    private static byte SoftAlpha => IsDark ? (byte)0x40 : (byte)0x34;

    public static Color SuccessSoft => Color.FromArgb(SoftAlpha, 0x22, 0xC5, 0x5E);
    public static Color WarningSoft => Color.FromArgb(SoftAlpha, 0xF5, 0x9E, 0x0B);
    public static Color ErrorSoft => Color.FromArgb(SoftAlpha, 0xEF, 0x44, 0x44);
    public static Color InfoSoft => Color.FromArgb(SoftAlpha, 0x38, 0xBD, 0xF8);

    #endregion

    #region Deliberately mode-invariant mats

    // These two are the only surfaces in the palette that intentionally do NOT
    // flip with the mode, so the intent is declarative rather than an accident
    // of a page hardcoding a literal.

    /// <summary>
    /// Letterbox mat behind an image or video. Stays near-black in both modes:
    /// the Stretch.None / Fill / Uniform comparison only reads if the unfilled
    /// area contrasts with the content, and every image viewer mats dark.
    /// </summary>
    public static Color MediaMat => Color.FromRgb(0x08, 0x14, 0x0C);

    /// <summary>
    /// Near-white "paper" mat for diagram hosts. FlowchartDiagram fixes its own
    /// node fills, node ink and edge strokes to a light palette, so a diagram
    /// needs a light mat in Dark mode too or its near-black labels vanish.
    /// </summary>
    public static Color DiagramPaper => Color.FromRgb(0xFA, 0xFB, 0xFE);

    /// <summary>
    /// Code-fence panel. Stays dark in both modes because the syntax palette
    /// painted on top of it is the fixed VS Code Dark+ token set, which cannot
    /// be re-tinted per mode. Matches the documentation-site convention of a
    /// dark code block on a light page.
    /// </summary>
    public static Color CodeSurface => Color.FromRgb(0x08, 0x14, 0x0C);

    /// <summary>Hairline that separates <see cref="CodeSurface"/> from the page.</summary>
    public static Color CodeBorder => Color.FromRgb(0x2D, 0x36, 0x40);

    /// <summary>
    /// Default ink on <see cref="CodeSurface"/>. The panel is dark in both modes, so code text
    /// cannot inherit the page foreground — in light mode that lands dark-on-dark and the snippet
    /// disappears. This is the Dark+ default token colour, matching the rest of the palette
    /// painted on that surface.
    /// </summary>
    public static Color CodeText => Color.FromRgb(0xD4, 0xD4, 0xD4);

    // EditControl caches the resolved brush instance for each token classification.
    // Publishing Gallery-owned aliases and recoloring those same instances on a
    // mode flip prevents an editor created in Light mode from retaining dark ink
    // after its background has already switched to Dark.
    private static readonly IReadOnlyDictionary<string, ThemeColorPair> s_editorSyntaxPalette =
        new Dictionary<string, ThemeColorPair>(StringComparer.Ordinal)
        {
            ["PlainText"] = new(Color.FromRgb(0xF5, 0xF5, 0xF7), Color.FromRgb(0x1D, 0x1D, 0x1F)),
            ["Keyword"] = new(Color.FromRgb(0xFC, 0x5F, 0xA3), Color.FromRgb(0x9B, 0x23, 0x93)),
            ["ControlKeyword"] = new(Color.FromRgb(0xFC, 0x5F, 0xA3), Color.FromRgb(0x9B, 0x23, 0x93)),
            ["TypeName"] = new(Color.FromRgb(0x5D, 0xD8, 0xFF), Color.FromRgb(0x0F, 0x68, 0xA0)),
            ["StructName"] = new(Color.FromRgb(0x5D, 0xD8, 0xFF), Color.FromRgb(0x0F, 0x68, 0xA0)),
            ["EnumName"] = new(Color.FromRgb(0x5D, 0xD8, 0xFF), Color.FromRgb(0x0F, 0x68, 0xA0)),
            ["InterfaceName"] = new(Color.FromRgb(0x5D, 0xD8, 0xFF), Color.FromRgb(0x0F, 0x68, 0xA0)),
            ["DelegateName"] = new(Color.FromRgb(0x5D, 0xD8, 0xFF), Color.FromRgb(0x0F, 0x68, 0xA0)),
            ["String"] = new(Color.FromRgb(0xFC, 0x6A, 0x5D), Color.FromRgb(0xC4, 0x1A, 0x16)),
            ["Character"] = new(Color.FromRgb(0xD0, 0xBF, 0x69), Color.FromRgb(0xC4, 0x1A, 0x16)),
            ["Number"] = new(Color.FromRgb(0xD0, 0xBF, 0x69), Color.FromRgb(0x1C, 0x00, 0xCF)),
            ["Comment"] = new(Color.FromRgb(0x7F, 0x8C, 0x98), Color.FromRgb(0x5D, 0x6C, 0x79)),
            ["XmlDoc"] = new(Color.FromRgb(0x8F, 0xA8, 0x8F), Color.FromRgb(0x4F, 0x6B, 0x5A)),
            ["Preprocessor"] = new(Color.FromRgb(0xFD, 0x8F, 0x3F), Color.FromRgb(0x64, 0x38, 0x20)),
            ["Operator"] = new(Color.FromRgb(0xF5, 0xF5, 0xF7), Color.FromRgb(0x1D, 0x1D, 0x1F)),
            ["Punctuation"] = new(Color.FromRgb(0xAE, 0xAE, 0xB2), Color.FromRgb(0x6E, 0x6E, 0x73)),
            ["Identifier"] = new(Color.FromRgb(0xF5, 0xF5, 0xF7), Color.FromRgb(0x1D, 0x1D, 0x1F)),
            ["LocalVariable"] = new(Color.FromRgb(0xF5, 0xF5, 0xF7), Color.FromRgb(0x1D, 0x1D, 0x1F)),
            ["Parameter"] = new(Color.FromRgb(0xF5, 0xF5, 0xF7), Color.FromRgb(0x1D, 0x1D, 0x1F)),
            ["Field"] = new(Color.FromRgb(0x67, 0xB7, 0xA4), Color.FromRgb(0x32, 0x6D, 0x74)),
            ["EnumMember"] = new(Color.FromRgb(0x67, 0xB7, 0xA4), Color.FromRgb(0x32, 0x6D, 0x74)),
            ["Property"] = new(Color.FromRgb(0x67, 0xB7, 0xA4), Color.FromRgb(0x32, 0x6D, 0x74)),
            ["Method"] = new(Color.FromRgb(0x67, 0xB7, 0xA4), Color.FromRgb(0x32, 0x6D, 0x74)),
            ["Namespace"] = new(Color.FromRgb(0x5D, 0xD8, 0xFF), Color.FromRgb(0x0F, 0x68, 0xA0)),
            ["Attribute"] = new(Color.FromRgb(0xFC, 0x5F, 0xA3), Color.FromRgb(0x9B, 0x23, 0x93)),
            ["BindingKeyword"] = new(Color.FromRgb(0xFC, 0x5F, 0xA3), Color.FromRgb(0x9B, 0x23, 0x93)),
            ["BindingParameter"] = new(Color.FromRgb(0xFD, 0x8F, 0x3F), Color.FromRgb(0xB3, 0x5C, 0x00)),
            ["BindingPath"] = new(Color.FromRgb(0x0A, 0x84, 0xFF), Color.FromRgb(0x00, 0x7A, 0xFF)),
            ["BindingOperator"] = new(Color.FromRgb(0xAE, 0xAE, 0xB2), Color.FromRgb(0x6E, 0x6E, 0x73)),
            ["Error"] = new(Color.FromRgb(0xFF, 0x45, 0x3A), Color.FromRgb(0xFF, 0x3B, 0x30)),
        };

    /// <summary>
    /// Recessed dark stage for glow / drop-shadow / emboss / transition demos.
    /// These effects only read against a dark backdrop, so the stage is
    /// mode-invariant on purpose; its labels pin their own light ink.
    /// </summary>
    public static Color EffectStage => Color.FromRgb(0x1A, 0x1A, 0x2E);

    /// <summary>
    /// Mid-tone slab for relief effects (inner shadow, emboss). Unlike
    /// <see cref="EffectStage"/> this cannot be dark: a relief effect renders a dark
    /// shadow AND a light highlight on the same surface, so it needs headroom in both
    /// directions. Mode-invariant for the same reason as the stage — no mode-aware
    /// surface token can sit in the middle of the range in both palettes. Labels drawn
    /// on it pin light ink.
    /// </summary>
    public static Color EffectRelief => Color.FromRgb(0x55, 0x66, 0x77);

    #endregion

    #region Shared brushes

    // Gallery pages are largely built in code and keep the brush instance they sampled at
    // construction time. Shared mutable brushes let a mode flip recolor those existing visuals
    // in place instead of destroying and rebuilding the current page (including images, editor
    // state, scroll position, and every control template).
    private static readonly SolidColorBrush s_backgroundDarkBrush = new(BackgroundDark);
    private static readonly SolidColorBrush s_backgroundMediumBrush = new(BackgroundMedium);
    private static readonly SolidColorBrush s_backgroundLightBrush = new(BackgroundLight);
    private static readonly SolidColorBrush s_backgroundCardBrush = new(BackgroundCard);
    private static readonly SolidColorBrush s_backgroundCardInnerBrush = new(BackgroundCardInner);
    private static readonly SolidColorBrush s_backgroundHoverBrush = new(BackgroundHover);
    private static readonly SolidColorBrush s_backgroundPressedBrush = new(BackgroundPressed);

    private static readonly SolidColorBrush s_accentPrimaryBrush = new(AccentPrimary);
    private static readonly SolidColorBrush s_accentSecondaryBrush = new(AccentSecondary);
    private static readonly SolidColorBrush s_accentLightBrush = new(AccentLight);
    private static readonly SolidColorBrush s_accentDarkBrush = new(AccentDark);
    private static readonly SolidColorBrush s_accentOnSurfaceBrush = new(AccentOnSurface);
    private static readonly SolidColorBrush s_textOnAccentBrush = new(TextOnAccent);
    private static readonly SolidColorBrush s_accentSoftBrush = new(AccentSoft);

    private static readonly SolidColorBrush s_textPrimaryBrush = new(TextPrimary);
    private static readonly SolidColorBrush s_textSecondaryBrush = new(TextSecondary);
    private static readonly SolidColorBrush s_textTertiaryBrush = new(TextTertiary);
    private static readonly SolidColorBrush s_textMutedBrush = new(TextMuted);
    private static readonly SolidColorBrush s_textDisabledBrush = new(TextDisabled);

    private static readonly SolidColorBrush s_borderDefaultBrush = new(BorderDefault);
    private static readonly SolidColorBrush s_borderSubtleBrush = new(BorderSubtle);
    private static readonly SolidColorBrush s_borderStrongBrush = new(BorderStrong);
    private static readonly SolidColorBrush s_borderFocusedBrush = new(BorderFocused);
    private static readonly SolidColorBrush s_borderGlassHighlightBrush = new(BorderGlassHighlight);

    private static readonly SolidColorBrush s_successBrush = new(Success);
    private static readonly SolidColorBrush s_warningBrush = new(Warning);
    private static readonly SolidColorBrush s_errorBrush = new(Error);
    private static readonly SolidColorBrush s_infoBrush = new(Info);
    private static readonly SolidColorBrush s_transparentBrush = new(Color.Transparent);

    private static readonly SolidColorBrush s_successOnSurfaceBrush = new(SuccessOnSurface);
    private static readonly SolidColorBrush s_warningOnSurfaceBrush = new(WarningOnSurface);
    private static readonly SolidColorBrush s_errorOnSurfaceBrush = new(ErrorOnSurface);
    private static readonly SolidColorBrush s_infoOnSurfaceBrush = new(InfoOnSurface);

    private static readonly SolidColorBrush s_successSoftBrush = new(SuccessSoft);
    private static readonly SolidColorBrush s_warningSoftBrush = new(WarningSoft);
    private static readonly SolidColorBrush s_errorSoftBrush = new(ErrorSoft);
    private static readonly SolidColorBrush s_infoSoftBrush = new(InfoSoft);

    private static readonly SolidColorBrush s_mediaMatBrush = new(MediaMat);
    private static readonly SolidColorBrush s_diagramPaperBrush = new(DiagramPaper);
    private static readonly SolidColorBrush s_codeSurfaceBrush = new(CodeSurface);
    private static readonly SolidColorBrush s_codeBorderBrush = new(CodeBorder);
    private static readonly SolidColorBrush s_codeTextBrush = new(CodeText);
    private static readonly SolidColorBrush s_effectStageBrush = new(EffectStage);
    private static readonly SolidColorBrush s_effectReliefBrush = new(EffectRelief);
    private static readonly IReadOnlyDictionary<string, SolidColorBrush> s_editorSyntaxBrushes =
        CreateEditorSyntaxBrushes();

    private static readonly LinearGradientBrush s_shellBackgroundBrush = CreateGradient(
        Color.FromRgb(0xFA, 0xFB, 0xFC),
        Color.FromRgb(0xF4, 0xF7, 0xF8));
    private static readonly LinearGradientBrush s_heroGradientBrush = CreateGradient(
        Color.FromRgb(0xE7, 0xF5, 0xF3),
        Color.FromRgb(0xF6, 0xF8, 0xF9));

    public static SolidColorBrush BackgroundDarkBrush => s_backgroundDarkBrush;
    public static SolidColorBrush BackgroundMediumBrush => s_backgroundMediumBrush;
    public static SolidColorBrush BackgroundLightBrush => s_backgroundLightBrush;
    public static SolidColorBrush BackgroundCardBrush => s_backgroundCardBrush;
    public static SolidColorBrush BackgroundCardInnerBrush => s_backgroundCardInnerBrush;
    public static SolidColorBrush BackgroundHoverBrush => s_backgroundHoverBrush;
    public static SolidColorBrush BackgroundPressedBrush => s_backgroundPressedBrush;

    public static SolidColorBrush AccentPrimaryBrush => s_accentPrimaryBrush;
    public static SolidColorBrush AccentSecondaryBrush => s_accentSecondaryBrush;
    public static SolidColorBrush AccentLightBrush => s_accentLightBrush;
    public static SolidColorBrush AccentDarkBrush => s_accentDarkBrush;
    public static SolidColorBrush AccentOnSurfaceBrush => s_accentOnSurfaceBrush;
    public static SolidColorBrush TextOnAccentBrush => s_textOnAccentBrush;
    public static SolidColorBrush AccentSoftBrush => s_accentSoftBrush;

    public static SolidColorBrush TextPrimaryBrush => s_textPrimaryBrush;
    public static SolidColorBrush TextSecondaryBrush => s_textSecondaryBrush;
    public static SolidColorBrush TextTertiaryBrush => s_textTertiaryBrush;
    public static SolidColorBrush TextMutedBrush => s_textMutedBrush;
    public static SolidColorBrush TextDisabledBrush => s_textDisabledBrush;

    public static SolidColorBrush BorderDefaultBrush => s_borderDefaultBrush;
    public static SolidColorBrush BorderSubtleBrush => s_borderSubtleBrush;
    public static SolidColorBrush BorderStrongBrush => s_borderStrongBrush;
    public static SolidColorBrush BorderFocusedBrush => s_borderFocusedBrush;
    public static SolidColorBrush BorderGlassHighlightBrush => s_borderGlassHighlightBrush;

    public static SolidColorBrush SuccessBrush => s_successBrush;
    public static SolidColorBrush WarningBrush => s_warningBrush;
    public static SolidColorBrush ErrorBrush => s_errorBrush;
    public static SolidColorBrush InfoBrush => s_infoBrush;
    public static SolidColorBrush TransparentBrush => s_transparentBrush;

    public static SolidColorBrush SuccessOnSurfaceBrush => s_successOnSurfaceBrush;
    public static SolidColorBrush WarningOnSurfaceBrush => s_warningOnSurfaceBrush;
    public static SolidColorBrush ErrorOnSurfaceBrush => s_errorOnSurfaceBrush;
    public static SolidColorBrush InfoOnSurfaceBrush => s_infoOnSurfaceBrush;

    public static SolidColorBrush SuccessSoftBrush => s_successSoftBrush;
    public static SolidColorBrush WarningSoftBrush => s_warningSoftBrush;
    public static SolidColorBrush ErrorSoftBrush => s_errorSoftBrush;
    public static SolidColorBrush InfoSoftBrush => s_infoSoftBrush;

    public static SolidColorBrush MediaMatBrush => s_mediaMatBrush;
    public static SolidColorBrush DiagramPaperBrush => s_diagramPaperBrush;
    public static SolidColorBrush CodeSurfaceBrush => s_codeSurfaceBrush;
    public static SolidColorBrush CodeBorderBrush => s_codeBorderBrush;
    public static SolidColorBrush CodeTextBrush => s_codeTextBrush;
    public static SolidColorBrush EffectStageBrush => s_effectStageBrush;
    public static SolidColorBrush EffectReliefBrush => s_effectReliefBrush;

    public static LinearGradientBrush ShellBackgroundBrush => s_shellBackgroundBrush;
    public static LinearGradientBrush HeroGradientBrush => s_heroGradientBrush;

    private static void RefreshSharedBrushes()
    {
        // The accent hues are mode-invariant, so this used to leave them alone. They
        // still have to be re-derived here: GalleryThemeTuner now mutates these very
        // instances to apply a tuned accent, and without a reset a Theme Studio accent
        // would survive both a mode flip and GalleryThemeTuner.Reset(). The tuner
        // re-applies its override afterwards via RefreshForCurrentMode.
        s_accentPrimaryBrush.Color = AccentPrimary;
        s_accentLightBrush.Color = AccentLight;
        s_accentSecondaryBrush.Color = AccentSecondary;
        s_accentDarkBrush.Color = AccentDark;
        s_accentOnSurfaceBrush.Color = AccentOnSurface;
        s_textOnAccentBrush.Color = TextOnAccent;

        s_backgroundDarkBrush.Color = BackgroundDark;
        s_backgroundMediumBrush.Color = BackgroundMedium;
        s_backgroundLightBrush.Color = BackgroundLight;
        s_backgroundCardBrush.Color = BackgroundCard;
        s_backgroundCardInnerBrush.Color = BackgroundCardInner;
        s_backgroundHoverBrush.Color = BackgroundHover;
        s_backgroundPressedBrush.Color = BackgroundPressed;
        s_accentSoftBrush.Color = AccentSoft;
        s_textPrimaryBrush.Color = TextPrimary;
        s_textSecondaryBrush.Color = TextSecondary;
        s_textTertiaryBrush.Color = TextTertiary;
        s_textMutedBrush.Color = TextMuted;
        s_textDisabledBrush.Color = TextDisabled;
        s_borderDefaultBrush.Color = BorderDefault;
        s_borderSubtleBrush.Color = BorderSubtle;
        s_borderStrongBrush.Color = BorderStrong;
        s_borderGlassHighlightBrush.Color = BorderGlassHighlight;

        // Mode-aware status variants. The plain Success/Warning/Error/Info fills
        // and the two mats above are mode-invariant by design, so they are
        // deliberately absent here.
        s_successOnSurfaceBrush.Color = SuccessOnSurface;
        s_warningOnSurfaceBrush.Color = WarningOnSurface;
        s_errorOnSurfaceBrush.Color = ErrorOnSurface;
        s_infoOnSurfaceBrush.Color = InfoOnSurface;
        s_successSoftBrush.Color = SuccessSoft;
        s_warningSoftBrush.Color = WarningSoft;
        s_errorSoftBrush.Color = ErrorSoft;
        s_infoSoftBrush.Color = InfoSoft;

        foreach (var (classification, colors) in s_editorSyntaxPalette)
        {
            s_editorSyntaxBrushes[classification].Color = Pick(colors.Dark, colors.Light);
        }

        UpdateGradient(
            s_shellBackgroundBrush,
            IsDark ? Color.FromRgb(0x17, 0x1D, 0x1F) : Color.FromRgb(0xFA, 0xFB, 0xFC),
            IsDark ? Color.FromRgb(0x12, 0x17, 0x19) : Color.FromRgb(0xF4, 0xF7, 0xF8));
        UpdateGradient(
            s_heroGradientBrush,
            IsDark ? Color.FromRgb(0x1C, 0x2D, 0x2F) : Color.FromRgb(0xE7, 0xF5, 0xF3),
            IsDark ? Color.FromRgb(0x1C, 0x23, 0x26) : Color.FromRgb(0xF6, 0xF8, 0xF9));
    }

    /// <summary>
    /// Re-derives every shared brush from <see cref="CurrentMode"/>, discarding any
    /// Theme Studio override. Paired with the Override* methods below.
    /// </summary>
    internal static void RestoreSharedBrushes() => RefreshSharedBrushes();

    /// <summary>
    /// Points the accent brushes at a tuned accent by mutating the shared instances.
    ///
    /// Mutating rather than replacing the dictionary entries is the whole point: the
    /// gallery reaches these colors through two paths — <c>{DynamicResource
    /// GalleryAccent}</c> in markup and <c>GalleryTheme.AccentPrimaryBrush</c> in
    /// code-built visuals — and swapping in a fresh brush only redirects the first,
    /// leaving every code-built visual (the catalog preview tiles, page eyebrows,
    /// drag-drop feedback) painted with the untuned accent. It also avoids writing to
    /// the ResourceDictionary, whose collection writes synchronously re-evaluate the
    /// style stack for the whole tree.
    /// </summary>
    internal static void OverrideAccentBrushes(
        Color primary,
        Color light,
        Color secondary,
        Color soft,
        Color textOnAccent)
    {
        s_accentPrimaryBrush.Color = primary;
        s_accentLightBrush.Color = light;
        s_accentSecondaryBrush.Color = secondary;
        s_accentSoftBrush.Color = soft;
        s_textOnAccentBrush.Color = textOnAccent;
    }

    /// <summary>
    /// Surface counterpart of <see cref="OverrideAccentBrushes"/>. Same reasoning:
    /// keep the shared instances so markup and code-built surfaces cannot diverge.
    /// </summary>
    internal static void OverrideSurfaceBrushes(
        Color card,
        Color cardInner,
        Color cardAlt)
    {
        s_backgroundCardBrush.Color = card;
        s_backgroundCardInnerBrush.Color = cardInner;
        s_backgroundLightBrush.Color = cardAlt;
    }

    private static LinearGradientBrush CreateGradient(Color start, Color end)
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };
        brush.GradientStops.Add(new GradientStop(start, 0));
        brush.GradientStops.Add(new GradientStop(end, 1));
        return brush;
    }

    private static IReadOnlyDictionary<string, SolidColorBrush> CreateEditorSyntaxBrushes()
    {
        var brushes = new Dictionary<string, SolidColorBrush>(StringComparer.Ordinal);
        foreach (var (classification, colors) in s_editorSyntaxPalette)
        {
            brushes[classification] = new SolidColorBrush(Pick(colors.Dark, colors.Light));
        }

        return brushes;
    }

    private static void UpdateGradient(LinearGradientBrush brush, Color start, Color end)
    {
        brush.GradientStops[0].Color = start;
        brush.GradientStops[1].Color = end;
    }

    #endregion

    #region DynamicResource keys and refresh

    // Keys used as {DynamicResource ...} across every gallery page so they
    // automatically re-skin when the mode flips.
    public const string KeyCardBackground = "GalleryCardBackground";
    public const string KeyCardInnerBackground = "GalleryCardInnerBackground";
    public const string KeyCardAltBackground = "GalleryCardAltBackground";
    public const string KeyShellBackground = "GalleryShellBackground";
    public const string KeyPaneBackground = "GalleryPaneBackground";
    public const string KeyBackgroundHover = "GalleryBackgroundHover";
    public const string KeyBackgroundPressed = "GalleryBackgroundPressed";

    public const string KeyBorderDefault = "GalleryBorderDefault";
    public const string KeyBorderSubtle = "GalleryBorderSubtle";
    public const string KeyBorderStrong = "GalleryBorderStrong";

    public const string KeyTextPrimary = "GalleryTextPrimary";
    public const string KeyTextSecondary = "GalleryTextSecondary";
    public const string KeyTextTertiary = "GalleryTextTertiary";
    public const string KeyTextMuted = "GalleryTextMuted";
    public const string KeyTextDisabled = "GalleryTextDisabled";

    public const string KeyAccentPrimary = "GalleryAccent";
    public const string KeyAccentLight = "GalleryAccentLight";
    public const string KeyAccentSecondary = "GalleryAccentSecondary";
    public const string KeyAccentSoft = "GalleryAccentSoft";
    public const string KeyAccentDark = "GalleryAccentDark";
    public const string KeyAccentOnSurface = "GalleryAccentOnSurface";
    public const string KeyTextOnAccent = "GalleryTextOnAccent";

    // Status fills (mode-invariant) — for a filled badge / chip / swatch.
    public const string KeySuccess = "GallerySuccess";
    public const string KeyWarning = "GalleryWarning";
    public const string KeyError = "GalleryError";
    public const string KeyInfo = "GalleryInfo";

    // Status foregrounds (mode-aware) — for status text and status glyphs.
    public const string KeySuccessOnSurface = "GallerySuccessOnSurface";
    public const string KeyWarningOnSurface = "GalleryWarningOnSurface";
    public const string KeyErrorOnSurface = "GalleryErrorOnSurface";
    public const string KeyInfoOnSurface = "GalleryInfoOnSurface";

    // Translucent status washes — for a status icon container.
    public const string KeySuccessSoft = "GallerySuccessSoft";
    public const string KeyWarningSoft = "GalleryWarningSoft";
    public const string KeyErrorSoft = "GalleryErrorSoft";
    public const string KeyInfoSoft = "GalleryInfoSoft";

    // Deliberately mode-invariant surfaces. Referencing these instead of a raw
    // literal is what marks a surface as "intentionally does not flip".
    public const string KeyMediaMat = "GalleryMediaMat";
    public const string KeyDiagramPaper = "GalleryDiagramPaper";
    public const string KeyCodeSurface = "GalleryCodeSurface";
    public const string KeyCodeBorder = "GalleryCodeBorder";
    public const string KeyCodeText = "GalleryCodeText";
    public const string KeyEffectStage = "GalleryEffectStage";
    public const string KeyEffectRelief = "GalleryEffectRelief";

    /// <summary>
    /// Writes the shared gallery brushes into the supplied dictionary. Subsequent mode
    /// switches mutate these instances in place, so registration is only needed at startup
    /// (or to reset a Theme Studio override).
    /// </summary>
    public static void RegisterResources(ResourceDictionary resources)
    {
        if (resources == null) return;

        using var notifications = resources.DeferNotifications();
        SetIfChanged(KeyCardBackground, BackgroundCardBrush);
        SetIfChanged(KeyCardInnerBackground, BackgroundCardInnerBrush);
        SetIfChanged(KeyCardAltBackground, BackgroundLightBrush);
        SetIfChanged(KeyShellBackground, BackgroundDarkBrush);
        SetIfChanged(KeyPaneBackground, BackgroundMediumBrush);
        SetIfChanged(KeyBackgroundHover, BackgroundHoverBrush);
        SetIfChanged(KeyBackgroundPressed, BackgroundPressedBrush);
        SetIfChanged(KeyBorderDefault, BorderDefaultBrush);
        SetIfChanged(KeyBorderSubtle, BorderSubtleBrush);
        SetIfChanged(KeyBorderStrong, BorderStrongBrush);
        SetIfChanged(KeyTextPrimary, TextPrimaryBrush);
        SetIfChanged(KeyTextSecondary, TextSecondaryBrush);
        SetIfChanged(KeyTextTertiary, TextTertiaryBrush);
        SetIfChanged(KeyTextMuted, TextMutedBrush);
        SetIfChanged(KeyTextDisabled, TextDisabledBrush);
        SetIfChanged(KeyAccentPrimary, AccentPrimaryBrush);
        SetIfChanged(KeyAccentLight, AccentLightBrush);
        SetIfChanged(KeyAccentSecondary, AccentSecondaryBrush);
        SetIfChanged(KeyAccentSoft, AccentSoftBrush);
        SetIfChanged(KeyAccentDark, AccentDarkBrush);
        SetIfChanged(KeyAccentOnSurface, AccentOnSurfaceBrush);
        SetIfChanged(KeyTextOnAccent, TextOnAccentBrush);
        SetIfChanged(KeySuccess, SuccessBrush);
        SetIfChanged(KeyWarning, WarningBrush);
        SetIfChanged(KeyError, ErrorBrush);
        SetIfChanged(KeyInfo, InfoBrush);
        SetIfChanged(KeySuccessOnSurface, SuccessOnSurfaceBrush);
        SetIfChanged(KeyWarningOnSurface, WarningOnSurfaceBrush);
        SetIfChanged(KeyErrorOnSurface, ErrorOnSurfaceBrush);
        SetIfChanged(KeyInfoOnSurface, InfoOnSurfaceBrush);
        SetIfChanged(KeySuccessSoft, SuccessSoftBrush);
        SetIfChanged(KeyWarningSoft, WarningSoftBrush);
        SetIfChanged(KeyErrorSoft, ErrorSoftBrush);
        SetIfChanged(KeyInfoSoft, InfoSoftBrush);
        SetIfChanged(KeyMediaMat, MediaMatBrush);
        SetIfChanged(KeyDiagramPaper, DiagramPaperBrush);
        SetIfChanged(KeyCodeSurface, CodeSurfaceBrush);
        SetIfChanged(KeyCodeBorder, CodeBorderBrush);
        SetIfChanged(KeyCodeText, CodeTextBrush);
        SetIfChanged(KeyEffectStage, EffectStageBrush);
        SetIfChanged(KeyEffectRelief, EffectReliefBrush);

        foreach (var (classification, brush) in s_editorSyntaxBrushes)
        {
            // OneEditorSyntax* is intentionally checked first by EditControl.
            // The EditorSyntax* alias also covers the editor's unclassified-text
            // fallback and its implicit framework style.
            SetIfChanged($"OneEditorSyntax{classification}", brush);
            SetIfChanged($"EditorSyntax{classification}", brush);
        }

        void SetIfChanged(string key, object value)
        {
            if (!resources.TryGetValue(key, out var existing) || !ReferenceEquals(existing, value))
            {
                resources[key] = value;
            }
        }
    }

    #endregion

    #region Dimensions

    public static double CornerRadiusSmall => 8;
    public static double CornerRadiusMedium => 12;
    public static double CornerRadiusLarge => 16;
    public static double CornerRadiusXLarge => 20;

    public static double SpacingTiny => 4;
    public static double SpacingSmall => 8;
    public static double SpacingMedium => 12;
    public static double SpacingLarge => 16;
    public static double SpacingXLarge => 24;
    public static double SpacingHuge => 32;

    public static double NavigationWidth => 248;
    public static double NavigationCollapsedWidth => 56;

    public static double CardPadding => 20;
    public static double ContentPadding => 24;

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
