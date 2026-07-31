using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;
using Jalium.UI.Gallery.Modules.Main.Support;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Landing poster that greets the user before the main Gallery shell takes
/// over — "Issue Nº 1", an editorial / magazine-cover treatment.
///
/// The structure (masthead, vertical rule, specimen rail, colophon) lives in
/// <c>OverviewPage.jalxaml</c> using framework-native controls and
/// <c>{DynamicResource ...}</c> tokens. Everything the markup white-list
/// forbids — gradient fills, glows, shadows, and the staggered entrance — is
/// applied here in code-behind via element-level <see cref="OuterGlowEffect"/> /
/// <see cref="DropShadowEffect"/> and the <see cref="GalleryTheme"/> brushes.
/// The only contract back to the shell is <see cref="EnterRequested"/>, raised
/// from the central "Enter the Gallery" button; <c>MainWindow</c> cross-fades
/// the overlay out in response.
///
/// The host fades this whole page 0→1; every animated child starts at
/// Opacity 0 and is tweened back to 1, so the final state is fully opaque
/// regardless of whether the per-element choreography is interrupted.
/// </summary>
public partial class OverviewPage : Page
{
    private static readonly Color AmberAccent = Color.FromRgb(0xF5, 0x9E, 0x0B);

    /// <summary>
    /// Raised when the user clicks the central "Enter the Gallery" button.
    /// MainWindow handles this by fading the overlay out and fading the
    /// navigation shell in.
    /// </summary>
    public event EventHandler? EnterRequested;

    public OverviewPage()
    {
        InitializeComponent();

        PaintAtmosphere();
        PaintAccents();
        ApplyModeColors();
        ComposeEntrance();

        // Re-skin the code-behind-painted layers when the gallery flips between
        // its forest-dark and mint-light palettes. (Everything bound through
        // {DynamicResource} re-skins on its own.)
        GalleryTheme.ModeChanged += OnGalleryModeChanged;
        Unloaded += (_, _) => GalleryTheme.ModeChanged -= OnGalleryModeChanged;
    }

    private void OnEnterButtonClick(object? sender, EventArgs e)
    {
        EnterRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnGalleryModeChanged(object? sender, EventArgs e)
    {
        PaintAtmosphere();
        ApplyModeColors();
    }

    // ------------------------------------------------------------------ //
    //  Painting — field gradient, halo blooms, ghost glyph               //
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Paints the cover's deep-forest field plus two oversized, heavily-blurred
    /// halo discs — one emerald low-left, one amber high-right — that read as a
    /// pseudo-radial glow. A faint giant "J" ghosts behind the type as a
    /// watermark. The field reuses the gallery's verified
    /// <see cref="GalleryTheme.ShellBackgroundBrush"/> so the cover matches the
    /// shell exactly in both light and dark modes.
    /// </summary>
    private void PaintAtmosphere()
    {
        var dark = GalleryTheme.CurrentMode == GalleryThemeMode.Dark;

        if (PosterRoot != null)
        {
            PosterRoot.Background = GalleryTheme.ShellBackgroundBrush;
        }

        // Emerald halo — a translucent disc with a wide outer glow. Low-left,
        // mostly off-canvas, so only its bloom bleeds onto the cover.
        if (HaloEmerald != null)
        {
            HaloEmerald.Background = new SolidColorBrush(
                Color.FromArgb(dark ? (byte)0x3A : (byte)0x2A, 0x1F, 0x79, 0x3F));
            HaloEmerald.Effect = new OuterGlowEffect
            {
                GlowSize = 90,
                GlowColor = Color.FromArgb(0xFF, 0x1E, 0x79, 0x3F),
                Intensity = 0.7,
                Opacity = dark ? 0.6 : 0.35
            };
        }

        // Amber halo — the single warm bloom, high-right, a sharp counter-note
        // to the green. Kept small and faint so it only tints the corner.
        if (HaloAmber != null)
        {
            HaloAmber.Background = new SolidColorBrush(
                Color.FromArgb(dark ? (byte)0x1E : (byte)0x18, 0xF5, 0x9E, 0x0B));
            HaloAmber.Effect = new OuterGlowEffect
            {
                GlowSize = 58,
                GlowColor = AmberAccent,
                Intensity = 0.6,
                Opacity = dark ? 0.34 : 0.22
            };
        }

        // Ghost watermark glyph — barely there, riding behind the masthead.
        if (GhostGlyph != null)
        {
            GhostGlyph.Foreground = new SolidColorBrush(
                Color.FromArgb(dark ? (byte)0x14 : (byte)0x12, 0x27, 0x8A, 0x52));
        }
    }

    /// <summary>
    /// Adds the small editorial flourishes: an amber wash + glow on the kicker
    /// tick, an amber halo on the masthead's accent line, a live-status halo on
    /// the running-head dot, and a quiet ambient lift under the CTA button so it
    /// sits proud of the page.
    /// </summary>
    private void PaintAccents()
    {
        // The kicker tick is the cover's first stroke of amber — give it a faint
        // bloom so the eye lands there before the headline.
        if (KickerTick != null)
        {
            KickerTick.Background = new SolidColorBrush(AmberAccent);
            KickerTick.Effect = new OuterGlowEffect
            {
                GlowSize = 14,
                GlowColor = AmberAccent,
                Intensity = 1.0,
                Opacity = 0.8
            };
        }

        // The "live" status dot in the running head gets a tight emerald halo so
        // the journal reads as a living, online edition.
        if (LiveDot != null)
        {
            LiveDot.Effect = new OuterGlowEffect
            {
                GlowSize = 12,
                GlowColor = GalleryTheme.AccentLight,
                Intensity = 1.0,
                Opacity = 0.85
            };
        }

        // The amber headline word — "that ship." — gets a soft warm halo so the
        // single accent line reads as illuminated rather than merely coloured.
        if (TitleLine3 != null)
        {
            TitleLine3.Effect = new OuterGlowEffect
            {
                GlowSize = 26,
                GlowColor = AmberAccent,
                Intensity = 0.9,
                Opacity = 0.45
            };
        }

        // Lift the CTA off the page with a wide, directionless ambient shadow.
        if (EnterButton != null)
        {
            EnterButton.Effect = new DropShadowEffect
            {
                ShadowDepth = 0,
                BlurRadius = 34,
                Color = Color.FromArgb(0x66, 0x00, 0x00, 0x00),
                Direction = 270
            };
        }
    }

    /// <summary>
    /// Re-applies the few brushes that were sampled eagerly (not via
    /// DynamicResource) so a live theme flip stays in sync. The accent line
    /// stays amber across both modes — amber is the cover's signature and is
    /// constant by design.
    /// </summary>
    private void ApplyModeColors()
    {
        if (TitleLine3 != null)
        {
            TitleLine3.Foreground = new SolidColorBrush(AmberAccent);
        }
    }

    // ------------------------------------------------------------------ //
    //  Entrance choreography — the press "proof" sets line by line       //
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Choreographs the entrance. The host fades the whole page 0→1 over 520ms;
    /// on top of that we stagger the cover's elements so it "sets" like a press
    /// proof — atmosphere first, then the running head, the masthead lines
    /// cascading down, the deck and CTA, and finally the specimen rail ticking
    /// in line by line. Every element ends at Opacity = 1.
    /// </summary>
    private void ComposeEntrance()
    {
        // 1. Atmosphere blooms in first and slowest, establishing depth.
        FadeIn(HaloEmerald, 60, 720);
        FadeIn(HaloAmber, 120, 720);
        FadeIn(GhostGlyph, 160, 820);

        // 2. Running head + its hairline rule.
        FadeIn(RunningHead, 180, 420);
        FadeIn(HeadRule, 240, 460);

        // 3. The kicker, then the three masthead lines cascade down.
        FadeIn(Kicker, 300, 380);
        FadeIn(TitleLine1, 360, 440);
        FadeIn(TitleLine2, 440, 440);
        FadeIn(TitleLine3, 520, 480);

        // 4. The deck and the call to action settle in last on the left.
        FadeIn(Deck, 640, 420);
        FadeIn(CtaRow, 720, 460);

        // 5. The vertical spine draws, then the specimen rail ticks in,
        //    line by line, like a contents column being type-set.
        FadeIn(SpineRule, 560, 520);
        FadeIn(RailHead, 660, 360);
        FadeIn(RailSub, 700, 360);
        FadeIn(Specimen1, 760, 380);
        FadeIn(RailDiv1, 800, 320);
        FadeIn(Specimen2, 840, 380);
        FadeIn(RailDiv2, 880, 320);
        FadeIn(Specimen3, 920, 380);
        FadeIn(RailDiv3, 960, 320);
        FadeIn(Specimen4, 1000, 380);
        FadeIn(RailDiv4, 1040, 320);
        FadeIn(Specimen5, 1080, 400);

        // 6. The colophon closes the page.
        FadeIn(Colophon, 1140, 460);
    }

    /// <summary>
    /// Schedules an ease-out-cubic opacity fade (0→1) on
    /// <paramref name="element"/> after <paramref name="delayMs"/>. Null-safe so
    /// a missing named part can never break the cascade.
    /// </summary>
    private static void FadeIn(UIElement? element, int delayMs, int durationMs)
    {
        if (element == null)
        {
            return;
        }

        FadeAnimator.After(TimeSpan.FromMilliseconds(delayMs), () =>
            FadeAnimator.Fade(element, 0.0, 1.0, TimeSpan.FromMilliseconds(durationMs)));
    }
}
