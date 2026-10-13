using System.Text;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Localization;

/// <summary>
/// Formats Gallery hints for the platform's primary command key. Jalium's
/// Window input maps Command to the existing Control command gestures on macOS.
/// Serialized gestures and explicitly supplied display strings stay unchanged.
/// </summary>
internal static class GalleryShortcutText
{
    private static readonly IReadOnlyDictionary<string, string> Shortcuts =
        CreateShortcuts(OperatingSystem.IsMacOS());

    internal static string Format(KeyGesture gesture) =>
        Format(gesture, OperatingSystem.IsMacOS());

    internal static string Format(KeyGesture gesture, bool macOS)
    {
        ArgumentNullException.ThrowIfNull(gesture);

        if (!macOS || !string.IsNullOrEmpty(gesture.DisplayString) ||
            (gesture.Modifiers & ModifierKeys.Windows) != 0)
        {
            return gesture.GetDisplayStringForCulture(null);
        }

        var text = new StringBuilder();
        if ((gesture.Modifiers & ModifierKeys.Alt) != 0) text.Append('⌥');
        if ((gesture.Modifiers & ModifierKeys.Shift) != 0) text.Append('⇧');
        if ((gesture.Modifiers & ModifierKeys.Control) != 0) text.Append('⌘');
        text.Append(gesture.Key switch
        {
            Key.Left => "←",
            Key.Right => "→",
            Key.Up => "↑",
            Key.Down => "↓",
            Key.Back => "⌫",
            Key.Delete => "⌦",
            Key.Enter => "↩",
            _ => new KeyGesture(gesture.Key).GetDisplayStringForCulture(null)
        });
        return text.ToString();
    }

    internal static string Expand(string text)
    {
        if (!text.Contains("{Shortcut.", StringComparison.Ordinal)) return text;

        foreach (var (name, shortcut) in Shortcuts)
        {
            text = text.Replace("{Shortcut." + name + "}", shortcut, StringComparison.Ordinal);
        }
        return text;
    }

    internal static void AddResources(ResourceDictionary resources)
    {
        foreach (var (name, shortcut) in Shortcuts)
        {
            resources["Loc.Shortcut." + name] = shortcut;
        }
    }

    private static IReadOnlyDictionary<string, string> CreateShortcuts(bool macOS)
    {
        string Command(Key key, ModifierKeys extra = ModifierKeys.None) =>
            Format(new KeyGesture(key, ModifierKeys.Control | extra), macOS);

        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Search"] = macOS ? "⌘\u00a0K" : "Ctrl K",
            ["Copy"] = Command(Key.C),
            ["Cut"] = Command(Key.X),
            ["Paste"] = Command(Key.V),
            ["SelectAll"] = Command(Key.A),
            ["Undo"] = Command(Key.Z),
            ["Redo"] = macOS ? Command(Key.Z, ModifierKeys.Shift) : Command(Key.Y),
            ["New"] = Command(Key.N),
            ["Open"] = Command(Key.O),
            ["Save"] = Command(Key.S),
            ["SaveAs"] = Command(Key.S, ModifierKeys.Shift),
            ["Reload"] = Command(Key.R),
            ["ZoomIn"] = macOS ? "⌘+" : "Ctrl++",
            ["ZoomOut"] = macOS ? "⌘−" : "Ctrl+-",
            ["PrimaryModifier"] = macOS ? "⌘" : "Ctrl",
            ["ShiftModifier"] = macOS ? "⇧" : "Shift",
            ["WordModifier"] = macOS ? "⌥" : "Ctrl",
            ["DocumentBoundary"] = macOS ? "⌘↑ / ⌘↓" : "Ctrl+Home/End",
            ["XamlUICommand"] = string.Join("  ·  ",
                Command(Key.D), Command(Key.L, ModifierKeys.Shift), Command(Key.E, ModifierKeys.Shift))
        };
    }
}
