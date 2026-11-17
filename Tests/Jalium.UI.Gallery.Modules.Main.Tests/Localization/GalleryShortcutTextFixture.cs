using Jalium.UI.Input;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Localization;

public sealed class GalleryShortcutTextFixture
{
    [Fact]
    public void DisplayFormatting_PreservesGestureAndExplicitOverride()
    {
        var gesture = new KeyGesture(Key.K, ModifierKeys.Control | ModifierKeys.Shift);
        Assert.Equal("⇧⌘K", GalleryShortcutText.Format(gesture, macOS: true));
        Assert.Equal("Ctrl+Shift+K", GalleryShortcutText.Format(gesture, macOS: false));
        Assert.Equal("Ctrl+Shift+K", gesture.GetDisplayStringForCulture(null));
        Assert.True(gesture.Matches(new object(),
            new KeyEventArgs(UIElement.KeyDownEvent, Key.K,
                ModifierKeys.Control | ModifierKeys.Shift, true, false, 1)));

        var custom = new KeyGesture(Key.K, ModifierKeys.Control, "Custom shortcut");
        Assert.Equal("Custom shortcut", GalleryShortcutText.Format(custom, macOS: true));
        Assert.Equal("Custom shortcut", GalleryShortcutText.Format(custom, macOS: false));
    }

    [Fact]
    public void SwitchingLanguages_ResolvesHintsAndPreservesSharedDictionary()
    {
        var originalLanguage = LocalizationManager.CurrentLanguage;
        var resources = new ResourceDictionary();
        try
        {
            LocalizationManager.Initialize(resources, GalleryLanguage.English);
            var active = Assert.Single(resources.MergedDictionaries);
            var copy = OperatingSystem.IsMacOS() ? "⌘C" : "Ctrl+C";
            var redo = OperatingSystem.IsMacOS() ? "⇧⌘Z" : "Ctrl+Y";

            foreach (var language in new[]
            {
                GalleryLanguage.ChineseSimplified, GalleryLanguage.Japanese,
                GalleryLanguage.Korean, GalleryLanguage.English,
                GalleryLanguage.ChineseSimplified
            })
            {
                LocalizationManager.SetLanguage(language);
                Assert.Same(active, Assert.Single(resources.MergedDictionaries));
                Assert.Equal(OperatingSystem.IsMacOS() ? "⌘\u00a0K" : "Ctrl K",
                    LocalizationManager.Get("Loc.Shell.SearchShortcut"));
                Assert.Contains(copy, LocalizationManager.Get("Loc.P.TextBox.CC2"));
                Assert.Contains(redo, LocalizationManager.Get("Loc.P.TextBox.Body6"));
                Assert.Contains(OperatingSystem.IsMacOS() ? "⌥" : "Ctrl",
                    LocalizationManager.Get("Loc.P.EditControl.Body5"));
                Assert.Contains(OperatingSystem.IsMacOS() ? "⌘↑ / ⌘↓" : "Ctrl+Home/End",
                    LocalizationManager.Get("Loc.P.EditControl.Body6"));
                Assert.Contains(copy, LocalizationManager.Get("Loc.P.TextBox.CopySample"));
                Assert.Equal(copy, LocalizationManager.Get("Loc.Shortcut.Copy"));
                foreach (var key in active.Keys)
                {
                    if (active[key] is string text)
                        Assert.DoesNotContain("{Shortcut.", text);
                }
            }
        }
        finally
        {
            LocalizationManager.SetLanguage(originalLanguage);
        }
    }

    [Fact]
    public void ShortcutExpansion_PreservesCompositeFormatArguments()
    {
        var expanded = GalleryShortcutText.Expand("{Shortcut.Copy}: {0}");
        Assert.Equal((OperatingSystem.IsMacOS() ? "⌘C" : "Ctrl+C") + ": selected",
            string.Format(expanded, "selected"));
    }
}
