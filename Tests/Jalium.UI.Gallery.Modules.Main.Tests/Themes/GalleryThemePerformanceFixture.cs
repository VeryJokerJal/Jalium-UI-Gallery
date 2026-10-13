using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Themes;

public sealed class GalleryThemePerformanceFixture
{
    [Fact]
    public void ModeChange_MutatesSharedBrushesWithoutReplacingThem()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var nextMode = Opposite(originalMode);
        var cardBrush = GalleryTheme.BackgroundCardBrush;
        var shellBrush = GalleryTheme.ShellBackgroundBrush;
        var originalCardColor = cardBrush.Color;
        var originalGradientStart = shellBrush.GradientStops[0].Color;

        try
        {
            GalleryTheme.CurrentMode = nextMode;

            Assert.Same(cardBrush, GalleryTheme.BackgroundCardBrush);
            Assert.Same(shellBrush, GalleryTheme.ShellBackgroundBrush);
            Assert.Equal(GalleryTheme.BackgroundCard, cardBrush.Color);
            Assert.NotEqual(originalCardColor, cardBrush.Color);
            Assert.NotEqual(originalGradientStart, shellBrush.GradientStops[0].Color);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void RegisterResources_BatchesFirstRegistrationAndSkipsIdenticalRepeat()
    {
        var resources = new ResourceDictionary();
        var notificationCount = 0;
        IReadOnlySet<object>? changedKeys = null;
        resources.ChangedWithKeys += (_, args) =>
        {
            notificationCount++;
            changedKeys = args.ChangedKeys;
        };

        GalleryTheme.RegisterResources(resources);

        Assert.Equal(1, notificationCount);
        Assert.NotNull(changedKeys);

        // The invariant is that the single batched notification covers every key the
        // registration published -- not any particular palette size. Asserting against
        // the dictionary itself keeps this test honest when tokens are added.
        Assert.NotEmpty(changedKeys!);
        Assert.Equal(resources.Count, changedKeys!.Count);
        Assert.Same(GalleryTheme.BackgroundCardBrush, resources[GalleryTheme.KeyCardBackground]);
        Assert.Same(GalleryTheme.TextPrimaryBrush, resources[GalleryTheme.KeyTextPrimary]);

        GalleryTheme.RegisterResources(resources);

        Assert.Equal(1, notificationCount);
    }

    [Fact]
    public void PageStyle_UsesThemeReactiveGalleryForeground()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var styles = new GalleryStyles();
        var page = new Page();
        GalleryTheme.RegisterResources(page.Resources);
        page.Style = Assert.IsType<Style>(styles[typeof(Page)]);

        var foreground = Assert.IsType<SolidColorBrush>(page.Foreground);
        Assert.Same(
            page.Resources[GalleryTheme.KeyTextPrimary],
            foreground);
        Assert.Equal(GalleryTheme.TextPrimary, foreground.Color);

        try
        {
            GalleryTheme.CurrentMode = Opposite(originalMode);

            Assert.Same(foreground, page.Foreground);
            Assert.Equal(GalleryTheme.TextPrimary, foreground.Color);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void EditorSyntaxPalette_RecolorsCachedBrushesWhenModeChanges()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var editor = new EditControl();
        GalleryTheme.RegisterResources(editor.Resources);

        try
        {
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;

            var plainText = ResolveEditorClassification(
                editor,
                TokenClassification.PlainText);
            var comment = ResolveEditorClassification(
                editor,
                TokenClassification.Comment);

            Assert.Same(
                editor.Resources["OneEditorSyntaxPlainText"],
                plainText);
            Assert.Same(
                editor.Resources["EditorSyntaxPlainText"],
                plainText);
            Assert.Equal(
                Color.FromRgb(0x1D, 0x1D, 0x1F),
                Assert.IsType<SolidColorBrush>(plainText).Color);
            Assert.Equal(
                Color.FromRgb(0x5D, 0x6C, 0x79),
                Assert.IsType<SolidColorBrush>(comment).Color);

            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;

            // EditControl returns the same cached brush objects, but their colors
            // now match the dark editor surface.
            Assert.Same(
                plainText,
                ResolveEditorClassification(
                    editor,
                    TokenClassification.PlainText));
            Assert.Same(
                comment,
                ResolveEditorClassification(
                    editor,
                    TokenClassification.Comment));
            Assert.Equal(
                Color.FromRgb(0xF5, 0xF5, 0xF7),
                Assert.IsType<SolidColorBrush>(plainText).Color);
            Assert.Equal(
                Color.FromRgb(0x7F, 0x8C, 0x98),
                Assert.IsType<SolidColorBrush>(comment).Color);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void DarkEditor_RendersCachedIdentifierTextWithVisibleContrast()
    {
        var originalMode = GalleryTheme.CurrentMode;

        try
        {
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;
            var editor = new EditControl
            {
                Width = 640,
                Height = 180,
                Background = GalleryTheme.BackgroundCardInnerBrush,
                BorderThickness = new Thickness(0),
                ShowLineNumbers = false,
                ShowMinimap = false,
                HighlightCurrentLine = false,
                FontSize = 16,
                Language = "csharp",
                Text = string.Join(
                    '\n',
                    Enumerable.Repeat(
                        "visibleIdentifier anotherVisibleIdentifier",
                        8))
            };
            GalleryTheme.RegisterResources(editor.Resources);

            // Reproduce the original sequence: classification is cached while
            // Light is active, then the existing editor is switched to Dark.
            _ = ResolveEditorClassification(
                editor,
                TokenClassification.Identifier);
            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;

            editor.Measure(new Size(640, 180));
            editor.Arrange(new Rect(0, 0, 640, 180));

            var target = new RenderTargetBitmap(
                640,
                180,
                96,
                96,
                PixelFormat.Bgra32);
            target.Clear(GalleryTheme.BackgroundCardInner);
            target.Render(editor);

            var pixels = new byte[640 * 180 * 4];
            target.CopyPixels(
                new Int32Rect(0, 0, 640, 180),
                pixels,
                640 * 4,
                0);

            var highContrastPixels = 0;
            for (var offset = 0; offset < pixels.Length; offset += 4)
            {
                if (Math.Max(
                        pixels[offset],
                        Math.Max(pixels[offset + 1], pixels[offset + 2])) >= 120)
                {
                    highContrastPixels++;
                }
            }

            Assert.True(
                highContrastPixels > 500,
                $"Dark editor rendered only {highContrastPixels} visible text pixels.");
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void AccentOnSurface_RemainsReadableAcrossThemeModes()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var brush = GalleryTheme.AccentOnSurfaceBrush;

        try
        {
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;
            Assert.Same(brush, GalleryTheme.AccentOnSurfaceBrush);
            Assert.Equal(GalleryTheme.AccentDark, brush.Color);

            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;
            Assert.Same(brush, GalleryTheme.AccentOnSurfaceBrush);
            Assert.Equal(GalleryTheme.AccentLight, brush.Color);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void ThemeService_KeepsFrameworkAndGalleryModesAligned()
    {
        var originalFrameworkTheme = ThemeManager.CurrentTheme;
        var originalGalleryMode = GalleryTheme.CurrentMode;
        var service = new GalleryThemeService();

        try
        {
            service.SetDark(true);

            Assert.True(service.IsDark);
            Assert.Equal(ThemeVariant.Dark, ThemeManager.CurrentTheme);
            Assert.Equal(GalleryThemeMode.Dark, GalleryTheme.CurrentMode);

            service.SetDark(false);

            Assert.False(service.IsDark);
            Assert.Equal(ThemeVariant.Light, ThemeManager.CurrentTheme);
            Assert.Equal(GalleryThemeMode.Light, GalleryTheme.CurrentMode);
        }
        finally
        {
            ThemeManager.ApplyTheme(originalFrameworkTheme);
            GalleryTheme.CurrentMode = originalGalleryMode;
        }
    }

    [Fact]
    public void CurrentMode_RaisesModeChangedOnlyWhenValueChanges()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var nextMode = Opposite(originalMode);
        var notificationCount = 0;
        EventHandler handler = (_, _) => notificationCount++;
        GalleryTheme.ModeChanged += handler;

        try
        {
            GalleryTheme.CurrentMode = originalMode;
            Assert.Equal(0, notificationCount);

            GalleryTheme.CurrentMode = nextMode;
            Assert.Equal(1, notificationCount);

            GalleryTheme.CurrentMode = nextMode;
            Assert.Equal(1, notificationCount);
        }
        finally
        {
            GalleryTheme.ModeChanged -= handler;
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void ModeChange_PreservesCurrentPageInstance()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var window = new MainWindow();
        var contentFrame = GetField<Border>(window, "ContentFrame");
        var currentPage = new Border();
        contentFrame.Child = currentPage;

        try
        {
            GalleryTheme.CurrentMode = Opposite(originalMode);

            Assert.Same(currentPage, contentFrame.Child);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
            window.Close();
        }
    }

    [Fact]
    public void ThemeStudio_DefaultSurfaceFollowsMode_AndCustomSurfaceIsPreserved()
    {
        GalleryThemeTuner.Reset();
        var originalMode = GalleryTheme.CurrentMode;
        var theme = CreateThemeService();
        var viewModel = new ThemeStudioViewModel(theme.Object);

        try
        {
            GalleryTheme.CurrentMode = Opposite(originalMode);
            RaiseThemeChanged(theme);

            Assert.Equal(GalleryTheme.BackgroundCard, viewModel.Surface);

            var custom = Color.FromRgb(0x31, 0x52, 0x73);
            viewModel.Surface = custom;
            GalleryTheme.CurrentMode = originalMode;
            RaiseThemeChanged(theme);

            Assert.Equal(custom, viewModel.Surface);
        }
        finally
        {
            viewModel.Destroy();
            GalleryTheme.CurrentMode = originalMode;
            GalleryThemeTuner.Reset();
        }
    }

    [Fact]
    public void Workbench_DefaultCanvasFollowsMode_AndCustomCanvasIsPreserved()
    {
        GalleryThemeTuner.Reset();
        var originalMode = GalleryTheme.CurrentMode;
        var theme = CreateThemeService();
        var viewModel =
            new GalleryDemoWorkbenchViewModel(theme.Object);

        try
        {
            GalleryTheme.CurrentMode = Opposite(originalMode);
            RaiseThemeChanged(theme);

            Assert.Equal(
                GalleryTheme.BackgroundCard,
                viewModel.Surface);

            var custom = Color.FromRgb(0x42, 0x63, 0x84);
            viewModel.Surface = custom;
            GalleryTheme.CurrentMode = originalMode;
            RaiseThemeChanged(theme);

            Assert.Equal(custom, viewModel.Surface);
        }
        finally
        {
            viewModel.Destroy();
            GalleryTheme.CurrentMode = originalMode;
            GalleryThemeTuner.Reset();
        }
    }

    [Fact]
    public void TunedAccent_ReachesCodeBuiltVisualsAndSurvivesModeFlip()
    {
        GalleryThemeTuner.Reset();
        var originalMode = GalleryTheme.CurrentMode;
        var resources = new ResourceDictionary();
        GalleryTheme.RegisterResources(resources);
        var accent = Color.FromRgb(0x31, 0x72, 0xB4);

        try
        {
            GalleryThemeTuner.ApplyAccent(resources, accent);

            // The gallery reads the accent through two paths. Replacing the dictionary
            // entry only redirected markup and left every code-built visual (catalog
            // preview tiles, page eyebrows, drag-drop feedback) on the untuned accent.
            Assert.Equal(accent, GalleryTheme.AccentPrimaryBrush.Color);
            Assert.Same(
                GalleryTheme.AccentPrimaryBrush,
                resources[GalleryTheme.KeyAccentPrimary]);
            Assert.Equal(
                GalleryThemeTuner.TextOnAccentOf(accent),
                GalleryTheme.TextOnAccentBrush.Color);
            Assert.Same(
                GalleryTheme.TextOnAccentBrush,
                resources[GalleryTheme.KeyTextOnAccent]);

            GalleryTheme.CurrentMode = Opposite(originalMode);
            GalleryThemeTuner.RefreshForCurrentMode(resources);

            // A mode flip re-derives the shared brushes, so the override has to be
            // layered back on rather than silently reverting to the stock accent.
            Assert.Equal(accent, GalleryTheme.AccentPrimaryBrush.Color);
            Assert.Equal(
                GalleryThemeTuner.TextOnAccentOf(accent),
                GalleryTheme.TextOnAccentBrush.Color);

            GalleryThemeTuner.Reset();

            Assert.Equal(
                GalleryTheme.AccentPrimary,
                GalleryTheme.AccentPrimaryBrush.Color);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
            GalleryThemeTuner.Reset();
        }
    }

    [Fact]
    public void ModeChange_UpdatesCustomAccentSoftInPlace()
    {
        GalleryThemeTuner.Reset();
        var originalMode = GalleryTheme.CurrentMode;
        var resources = new ResourceDictionary();
        GalleryTheme.RegisterResources(resources);
        var accent = Color.FromRgb(0x31, 0x72, 0xB4);
        GalleryThemeTuner.ApplyAccent(resources, accent);
        var softBrush = Assert.IsType<SolidColorBrush>(resources[GalleryTheme.KeyAccentSoft]);

        try
        {
            GalleryTheme.CurrentMode = Opposite(originalMode);
            GalleryThemeTuner.RefreshForCurrentMode(resources);

            var expectedAlpha = GalleryTheme.CurrentMode == GalleryThemeMode.Dark
                ? (byte)0x42
                : (byte)0x30;
            Assert.Same(softBrush, resources[GalleryTheme.KeyAccentSoft]);
            Assert.Equal(Color.FromArgb(expectedAlpha, accent.R, accent.G, accent.B), softBrush.Color);
        }
        finally
        {
            GalleryTheme.CurrentMode = originalMode;
            GalleryThemeTuner.Reset();
        }
    }

    [Fact]
    public void PrimaryButton_ActiveTemplateTrigger_FollowsAccentResource()
    {
        GalleryThemeTuner.Reset();
        var styles = new GalleryStyles();
        var button = new Button
        {
            Style = Assert.IsType<Style>(styles[GalleryStyles.PrimaryButtonStyleKey]),
            Content = "Open demo",
            Width = 240,
            Height = 38
        };
        GalleryTheme.RegisterResources(button.Resources);

        var host = new StackPanel { Width = 260, Height = 60 };
        host.Children.Add(button);
        host.Measure(new Size(260, 60));
        host.Arrange(new Rect(0, 0, 260, 60));
        button.ApplyTemplate();
        SetMouseOver(button, true);

        try
        {
            var rootBorder = Assert.IsType<Border>(button.FindName("PrimaryRootBorder"));
            Assert.Same(button.Resources[GalleryTheme.KeyAccentLight], rootBorder.Background);

            var customAccent = Color.FromRgb(0x4F, 0x46, 0xE5);
            GalleryThemeTuner.ApplyAccent(button.Resources, customAccent);

            Assert.Same(button.Resources[GalleryTheme.KeyAccentLight], rootBorder.Background);
            Assert.Equal(
                GalleryThemeTuner.Mix(customAccent, Color.FromRgb(0xFF, 0xFF, 0xFF), 0.18),
                Assert.IsType<SolidColorBrush>(rootBorder.Background).Color);

            SetMouseOver(button, false);
            Assert.Same(button.Resources[GalleryTheme.KeyAccentPrimary], rootBorder.Background);
        }
        finally
        {
            GalleryThemeTuner.Reset();
        }
    }

    [Fact]
    public void PrimaryButton_PressedBorderWinsAfterFocusOnEveryPress()
    {
        GalleryThemeTuner.Reset();
        var styles = new GalleryStyles();
        var button = new Button
        {
            Style = Assert.IsType<Style>(
                styles[GalleryStyles.PrimaryButtonStyleKey]),
            Content = "Open demo",
            Width = 240,
            Height = 38
        };
        GalleryTheme.RegisterResources(button.Resources);

        var host = new StackPanel { Width = 260, Height = 60 };
        host.Children.Add(button);
        host.Measure(new Size(260, 60));
        host.Arrange(new Rect(0, 0, 260, 60));
        button.ApplyTemplate();
        SetMouseOver(button, true);

        try
        {
            var rootBorder = Assert.IsType<Border>(
                button.FindName("PrimaryRootBorder"));

            SetPressed(button, true);
            Assert.True(button.Focus());
            Assert.Same(
                button.Resources[GalleryTheme.KeyAccentSecondary],
                rootBorder.BorderBrush);

            SetPressed(button, false);
            SetPressed(button, true);
            Assert.Same(
                button.Resources[GalleryTheme.KeyAccentSecondary],
                rootBorder.BorderBrush);
        }
        finally
        {
            SetPressed(button, false);
            Jalium.UI.Input.Keyboard.ClearFocus();
            GalleryThemeTuner.Reset();
        }
    }

    private static void SetMouseOver(UIElement element, bool value)
    {
        var method = typeof(UIElement).GetMethod(
            "SetIsMouseOver",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        method!.Invoke(element, [value]);
    }

    private static void SetPressed(UIElement element, bool value)
    {
        var method = typeof(UIElement).GetMethod(
            "SetIsPressed",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        method!.Invoke(element, [value]);
    }

    private static GalleryThemeMode Opposite(GalleryThemeMode mode) =>
        mode == GalleryThemeMode.Dark ? GalleryThemeMode.Light : GalleryThemeMode.Dark;

    private static Mock<IGalleryThemeService> CreateThemeService()
    {
        var theme = new Mock<IGalleryThemeService>();
        theme.SetupGet(service => service.IsDark)
            .Returns(() =>
                GalleryTheme.CurrentMode ==
                GalleryThemeMode.Dark);
        return theme;
    }

    private static void RaiseThemeChanged(
        Mock<IGalleryThemeService> theme) =>
        theme.Raise(
            service => service.ThemeChanged += null,
            EventArgs.Empty);

    private static T GetField<T>(object owner, string name)
        where T : class
    {
        var field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        return Assert.IsAssignableFrom<T>(field!.GetValue(owner));
    }

    private static Brush ResolveEditorClassification(
        EditControl editor,
        TokenClassification classification)
    {
        var method = typeof(EditControl).GetMethod(
            "ResolveClassificationBrush",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        return Assert.IsAssignableFrom<Brush>(method!.Invoke(
            editor,
            [classification, new SolidColorBrush(Color.Transparent)]));
    }
}
