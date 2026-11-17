using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Typed accessors for the declarative GalleryStyles resource dictionary.
/// Kept outside the JALXAML code-behind so that file remains initialization-only.
/// </summary>
public partial class GalleryStyles
{
    private static readonly Lazy<GalleryStyles> s_fallbackResources =
        new(static () => new GalleryStyles());

    public const string PrimaryButtonStyleKey =
        "GalleryPrimaryButton";
    public const string SecondaryButtonStyleKey =
        "GallerySecondaryButton";
    public const string GhostButtonStyleKey =
        "GalleryGhostButton";
    public const string SegmentButtonStyleKey =
        "GallerySegmentButton";

    public static Style GetStyle(string resourceKey)
    {
        return Application.Current?.TryFindResource(resourceKey) as Style
            ?? (s_fallbackResources.Value.TryGetValue(
                    resourceKey,
                    out var value)
                ? value as Style
                : null)
            ?? throw new InvalidOperationException(
                $"Gallery style resource '{resourceKey}' is not available.");
    }
}
