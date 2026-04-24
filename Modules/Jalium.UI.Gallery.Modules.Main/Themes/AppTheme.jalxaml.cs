using Jalium.UI;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Theme preset: Aureate Amber.
/// Warm amber accent on deep indigo.
///
/// The actual brush palette lives in the companion <c>AppTheme.jalxaml</c>. Code-behind
/// just triggers XAML parsing at construction; consumers merge the dictionary via
/// <c>app.Resources.MergedDictionaries.Add(new AppTheme())</c>.
/// </summary>
public partial class AppTheme : ResourceDictionary
{
    public AppTheme()
    {
        InitializeComponent();
    }
}
