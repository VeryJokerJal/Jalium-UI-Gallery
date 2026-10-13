using Jalium.UI;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jalium.UI.Gallery.Modules.Main;

/// <summary>
/// WPF-style application composition root for the Gallery module. It performs
/// only application-wide theme bootstrap and resolves the initial Window
/// through <see cref="IViewFactory"/>; page state and interaction remain in
/// their independently registered ViewModels.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // The gallery is a component workbench first, so start with the crisp
        // light canvas used by the reference design. Users can still switch to
        // the dark palette from the navigation footer.
        // Pin the application mode as well as the current framework palette.
        // Calling ThemeManager alone leaves ThemeMode=None, which allows a later
        // platform preference refresh to put framework controls back on Dark
        // while the Gallery-scoped brushes remain Light.
#pragma warning disable WPF0001
        ThemeMode = ThemeMode.Light;
#pragma warning restore WPF0001
        ThemeManager.ApplyAccent(GalleryTheme.AccentPrimary);
        GalleryTheme.CurrentMode = GalleryThemeMode.Light;

        // Seed the gallery-scoped palette so every {DynamicResource Gallery...}
        // reference in the pages resolves before the first window renders. The
        // GalleryTheme static brushes also flip with the live mode toggle.
        GalleryTheme.RegisterResources(Resources);

        // Same contract for text: merge the string table so every
        // {DynamicResource Loc...} in the shell and the pages resolves on the
        // first frame. The language is the one the user last picked, falling
        // back to the operating system's UI language.
        LocalizationManager.Initialize(Resources, LanguageSettingsStore.Load());
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var viewFactory = Services?.GetRequiredService<IViewFactory>()
            ?? throw new InvalidOperationException("The MVVM view factory is not registered.");
        MainWindow = viewFactory.CreateView<MainWindow>()
            ?? throw new InvalidOperationException("MainWindow could not be resolved.");

        base.OnStartup(e);
    }
}
