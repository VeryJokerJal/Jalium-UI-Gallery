using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// C#-side mirror of the shared <c>GalleryStyles.jalxaml</c> design system.
///
/// Markup pages reference the styles by key (Style="{StaticResource GalleryCard}");
/// code-behind pages that build their UI imperatively (HomePage, ThemeColors and
/// the hybrid demo pages) call these factory helpers instead, so both paths render
/// the identical premium card scaffold — cards, demo surfaces, the typography ramp,
/// eyebrow/title/subtitle header — without duplicating literal colors and metrics.
///
/// Every value here is kept in lock-step with GalleryStyles.jalxaml. Colors come
/// from the theme-reactive <see cref="GalleryTheme"/> brushes, so anything built
/// through these helpers re-skins with a Dark/Light flip when the host rebuilds.
/// </summary>
public static class GalleryUi
{
    // ── Header ──────────────────────────────────────────────────────────

    /// <summary>Small accent label that sits above a page title.</summary>
    public static TextBlock Eyebrow(string text) => new()
    {
        Text = text,
        FontSize = 12.5,
        FontWeight = FontWeights.SemiBold,
        Foreground = GalleryTheme.AccentLightBrush,
        Margin = new Thickness(0, 0, 0, 6),
    };

    /// <summary>Page H1.</summary>
    public static TextBlock PageTitle(string text) => new()
    {
        Text = text,
        FontSize = 28,
        FontWeight = FontWeights.Bold,
        Foreground = GalleryTheme.TextPrimaryBrush,
        Margin = new Thickness(0, 0, 0, 6),
    };

    /// <summary>Page lead / description under the title.</summary>
    public static TextBlock Subtitle(string text) => new()
    {
        Text = text,
        FontSize = 15,
        Foreground = GalleryTheme.TextSecondaryBrush,
        TextWrapping = TextWrapping.Wrap,
        Margin = new Thickness(0, 0, 0, 24),
    };

    // ── Typography ramp ─────────────────────────────────────────────────

    /// <summary>Card / section heading.</summary>
    public static TextBlock CardTitle(string text) => new()
    {
        Text = text,
        FontSize = 16,
        FontWeight = FontWeights.SemiBold,
        Foreground = GalleryTheme.TextPrimaryBrush,
        Margin = new Thickness(0, 0, 0, 4),
    };

    /// <summary>Explanatory caption above a demo.</summary>
    public static TextBlock CardCaption(string text) => new()
    {
        Text = text,
        FontSize = 13,
        Foreground = GalleryTheme.TextTertiaryBrush,
        TextWrapping = TextWrapping.Wrap,
        Margin = new Thickness(0, 0, 0, 16),
    };

    /// <summary>Standard body copy.</summary>
    public static TextBlock BodyText(string text) => new()
    {
        Text = text,
        FontSize = 14,
        Foreground = GalleryTheme.TextSecondaryBrush,
        TextWrapping = TextWrapping.Wrap,
    };

    /// <summary>Muted caption / helper / metadata.</summary>
    public static TextBlock CaptionText(string text) => new()
    {
        Text = text,
        FontSize = 12,
        Foreground = GalleryTheme.TextMutedBrush,
        TextWrapping = TextWrapping.Wrap,
    };

    /// <summary>Live value readout beside a demo control.</summary>
    public static TextBlock ValueLabel(string text) => new()
    {
        Text = text,
        FontSize = 14,
        FontWeight = FontWeights.Medium,
        Foreground = GalleryTheme.TextPrimaryBrush,
        VerticalAlignment = VerticalAlignment.Center,
    };

    // ── Containers ──────────────────────────────────────────────────────

    /// <summary>Primary raised section card (caller sets <c>Child</c>).</summary>
    public static Border Card() => new()
    {
        Background = GalleryTheme.BackgroundCardBrush,
        BorderBrush = GalleryTheme.BorderDefaultBrush,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(16),
        Padding = new Thickness(20),
        Margin = new Thickness(0, 0, 0, 16),
    };

    /// <summary>Inner surface holding a live demo (caller sets <c>Child</c>).</summary>
    public static Border DemoSurface() => new()
    {
        Background = GalleryTheme.BackgroundCardInnerBrush,
        BorderBrush = GalleryTheme.BorderSubtleBrush,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(12),
        Padding = new Thickness(18),
    };

    // ── Composites ──────────────────────────────────────────────────────

    /// <summary>
    /// Builds a complete section card: heading + optional caption + a demo
    /// surface wrapping <paramref name="demo"/>. This is the imperative
    /// equivalent of the markup card the workflow produces on every page.
    /// </summary>
    public static Border SectionCard(string title, string? caption, UIElement demo)
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(CardTitle(title));
        if (!string.IsNullOrEmpty(caption))
        {
            stack.Children.Add(CardCaption(caption));
        }

        var surface = DemoSurface();
        surface.Child = demo;
        stack.Children.Add(surface);

        var card = Card();
        card.Child = stack;
        return card;
    }
}
