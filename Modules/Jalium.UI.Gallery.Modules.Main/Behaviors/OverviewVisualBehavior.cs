using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Support;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Owns the landing page's visual-only effects and entrance choreography.
/// No application state or commands are stored here.
/// </summary>
public static class OverviewVisualBehavior
{
    private static readonly string[] EntranceElementNames =
    [
        "HaloEmerald",
        "HaloAmber",
        "GhostGlyph",
        "RunningHead",
        "HeadRule",
        "Kicker",
        "TitleLine1",
        "TitleLine2",
        "TitleLine3",
        "Deck",
        "CtaRow",
        "SpineRule",
        "RailHead",
        "RailSub",
        "Specimen1",
        "RailDiv1",
        "Specimen2",
        "RailDiv2",
        "Specimen3",
        "RailDiv3",
        "Specimen4",
        "RailDiv4",
        "Specimen5",
        "Colophon"
    ];

    private static readonly HashSet<FrameworkElement> ActiveRoots = [];
    private static readonly Color AmberAccent =
        Color.FromRgb(0xF5, 0x9E, 0x0B);
    private static readonly Brush DarkEmeraldHaloBrush =
        new SolidColorBrush(
            Color.FromArgb(0x3A, 0x1F, 0x79, 0x3F));
    private static readonly Brush LightEmeraldHaloBrush =
        new SolidColorBrush(
            Color.FromArgb(0x2A, 0x1F, 0x79, 0x3F));
    private static readonly Brush DarkAmberHaloBrush =
        new SolidColorBrush(
            Color.FromArgb(0x1E, 0xF5, 0x9E, 0x0B));
    private static readonly Brush LightAmberHaloBrush =
        new SolidColorBrush(
            Color.FromArgb(0x18, 0xF5, 0x9E, 0x0B));
    private static readonly Brush DarkGhostBrush =
        new SolidColorBrush(
            Color.FromArgb(0x14, 0x27, 0x8A, 0x52));
    private static readonly Brush LightGhostBrush =
        new SolidColorBrush(
            Color.FromArgb(0x12, 0x27, 0x8A, 0x52));

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(OverviewVisualBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    private static readonly DependencyProperty EntranceComposedProperty =
        DependencyProperty.RegisterAttached(
            "EntranceComposed",
            typeof(bool),
            typeof(OverviewVisualBehavior),
            new PropertyMetadata(false));

    static OverviewVisualBehavior()
    {
        GalleryTheme.ModeChanged += OnThemeModeChanged;
    }

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FrameworkElement root)
        {
            return;
        }

        if (GetIsEnabled(root))
        {
            root.Loaded += OnLoaded;
            root.Unloaded += OnUnloaded;
        }
        else
        {
            root.Loaded -= OnLoaded;
            root.Unloaded -= OnUnloaded;
            ActiveRoots.Remove(root);
        }
    }

    private static void OnLoaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is not FrameworkElement root)
        {
            return;
        }

        ActiveRoots.Add(root);
        Paint(root);
        if (!(bool)(root.GetValue(EntranceComposedProperty) ?? false))
        {
            root.SetValue(EntranceComposedProperty, true);
            if (global::Jalium.UI.SystemParameters.ClientAreaAnimation &&
                global::Jalium.UI.SystemParameters.UIEffects)
            {
                ComposeEntrance(root);
            }
            else
            {
                RevealEntrance(root);
            }
        }
    }

    private static void OnUnloaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is FrameworkElement root)
        {
            ActiveRoots.Remove(root);
        }
    }

    private static void OnThemeModeChanged(
        object? sender,
        EventArgs args)
    {
        foreach (var root in ActiveRoots.ToArray())
        {
            Paint(root);
        }
    }

    private static void Paint(FrameworkElement root)
    {
        var dark =
            GalleryTheme.CurrentMode == GalleryThemeMode.Dark;
        var poster = Find<Border>(root, "PosterRoot");
        var emerald = Find<Border>(root, "HaloEmerald");
        var amber = Find<Border>(root, "HaloAmber");
        var ghost = Find<TextBlock>(root, "GhostGlyph");

        if (poster is not null)
        {
            poster.Background = GalleryTheme.ShellBackgroundBrush;
        }

        if (emerald is not null)
        {
            emerald.Background = dark
                ? DarkEmeraldHaloBrush
                : LightEmeraldHaloBrush;
            emerald.Effect = CreateEmeraldGlow(dark);
        }

        if (amber is not null)
        {
            amber.Background = dark
                ? DarkAmberHaloBrush
                : LightAmberHaloBrush;
            amber.Effect = CreateAmberGlow(dark);
        }

        if (ghost is not null)
        {
            ghost.Foreground = dark
                ? DarkGhostBrush
                : LightGhostBrush;
        }

        PaintAccents(root);
    }

    private static void PaintAccents(FrameworkElement root)
    {
        if (Find<Border>(root, "KickerTick") is { } kicker)
        {
            kicker.Background =
                GalleryTheme.WarningOnSurfaceBrush;
            kicker.Effect = new OuterGlowEffect
            {
                GlowSize = 14,
                GlowColor = AmberAccent,
                Intensity = 1,
                Opacity = 0.8
            };
        }

        if (Find<Border>(root, "LiveDot") is { } liveDot)
        {
            liveDot.Effect = new OuterGlowEffect
            {
                GlowSize = 12,
                GlowColor = GalleryTheme.AccentLight,
                Intensity = 1,
                Opacity = 0.85
            };
        }

        if (Find<TextBlock>(root, "TitleLine3") is { } title)
        {
            title.Foreground =
                GalleryTheme.WarningOnSurfaceBrush;
            title.Effect = new OuterGlowEffect
            {
                GlowSize = 26,
                GlowColor = AmberAccent,
                Intensity = 0.9,
                Opacity = 0.45
            };
        }

        if (Find<Button>(root, "EnterButton") is { } button)
        {
            button.Effect = new DropShadowEffect
            {
                ShadowDepth = 0,
                BlurRadius = 34,
                Color = Color.FromArgb(
                    0x66,
                    0x00,
                    0x00,
                    0x00),
                Direction = 270
            };
        }
    }

    private static OuterGlowEffect CreateEmeraldGlow(
        bool dark) =>
        new()
        {
            GlowSize = 90,
            GlowColor =
                Color.FromArgb(0xFF, 0x1E, 0x79, 0x3F),
            Intensity = 0.7,
            Opacity = dark ? 0.6 : 0.35
        };

    private static OuterGlowEffect CreateAmberGlow(bool dark) =>
        new()
        {
            GlowSize = 58,
            GlowColor = AmberAccent,
            Intensity = 0.6,
            Opacity = dark ? 0.34 : 0.22
        };

    private static void ComposeEntrance(FrameworkElement root)
    {
        FadeIn(root, "HaloEmerald", 60, 720);
        FadeIn(root, "HaloAmber", 120, 720);
        FadeIn(root, "GhostGlyph", 160, 820);
        FadeIn(root, "RunningHead", 180, 420);
        FadeIn(root, "HeadRule", 240, 460);
        FadeIn(root, "Kicker", 300, 380);
        FadeIn(root, "TitleLine1", 360, 440);
        FadeIn(root, "TitleLine2", 440, 440);
        FadeIn(root, "TitleLine3", 520, 480);
        FadeIn(root, "Deck", 640, 420);
        FadeIn(root, "CtaRow", 720, 460);
        FadeIn(root, "SpineRule", 560, 520);
        FadeIn(root, "RailHead", 660, 360);
        FadeIn(root, "RailSub", 700, 360);
        FadeIn(root, "Specimen1", 760, 380);
        FadeIn(root, "RailDiv1", 800, 320);
        FadeIn(root, "Specimen2", 840, 380);
        FadeIn(root, "RailDiv2", 880, 320);
        FadeIn(root, "Specimen3", 920, 380);
        FadeIn(root, "RailDiv3", 960, 320);
        FadeIn(root, "Specimen4", 1000, 380);
        FadeIn(root, "RailDiv4", 1040, 320);
        FadeIn(root, "Specimen5", 1080, 400);
        FadeIn(root, "Colophon", 1140, 460);
    }

    private static void RevealEntrance(FrameworkElement root)
    {
        foreach (var name in EntranceElementNames)
        {
            if (root.FindName(name) is UIElement element)
            {
                element.Opacity = 1;
            }
        }
    }

    private static void FadeIn(
        FrameworkElement root,
        string name,
        int delayMilliseconds,
        int durationMilliseconds)
    {
        if (root.FindName(name) is not UIElement element)
        {
            return;
        }

        FadeAnimator.After(
            TimeSpan.FromMilliseconds(delayMilliseconds),
            () => FadeAnimator.Fade(
                element,
                0,
                1,
                TimeSpan.FromMilliseconds(
                    durationMilliseconds)));
    }

    private static T? Find<T>(
        FrameworkElement root,
        string name)
        where T : class =>
        root.FindName(name) as T;
}
