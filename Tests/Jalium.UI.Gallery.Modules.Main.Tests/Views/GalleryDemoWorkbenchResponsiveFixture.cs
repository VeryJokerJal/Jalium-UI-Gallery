using System.Runtime.CompilerServices;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Media;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

/// <summary>
/// Workbench interaction tests target its ViewModel; the View is now purely
/// declarative and has no responsive or inspector state of its own.
/// </summary>
public sealed class GalleryDemoWorkbenchResponsiveFixture
{
    [Fact]
    public void Jalxaml_PreservesOriginalDetailWorkbenchVisualContract()
    {
        var repositoryRoot = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Views",
            "Pages",
            "GalleryDemoWorkbench.jalxaml"));

        // The chrome copy moved into the localization dictionaries; the contract is
        // that these landmarks still bind their shared keys.
        Assert.Contains("Text=\"{DynamicResource Loc.Workbench.Ui1}\"", source);
        Assert.Contains("GroupName=\"WorkbenchTabs\"", source);
        Assert.Contains("GroupName=\"WorkbenchViewports\"", source);
        Assert.Contains("x:Name=\"InspectorColumn\"", source);
        Assert.Contains("x:Name=\"InspectorPanel\"", source);
        Assert.Contains(
            "GalleryWorkbenchLayoutBehavior.IsEnabled",
            source);
        Assert.Contains(
            "GalleryInspectorNumericStepper",
            source);
        Assert.Contains(
            "GalleryInspectorColorEditor",
            source);
        Assert.Contains("Text=\"{DynamicResource Loc.Workbench.Ui11}\"", source);
        Assert.Contains("Text=\"{DynamicResource Loc.Workbench.Ui12}\"", source);
        Assert.Contains("Content=\"{DynamicResource Loc.Workbench.Ui13}\"", source);
        Assert.DoesNotContain(
            "Style=\"{StaticResource GalleryPageStack}\"",
            source);
        Assert.DoesNotContain("Text=\"Preview inspector\"", source);
    }

    [Fact]
    public void InitializeAndViewportCommand_UpdateBindableWorkbenchState()
    {
        var viewModel = CreateViewModel();
        var definition = CreateDefinition();

        viewModel.Initialize(definition);
        viewModel.SetViewportCommand.Execute("Mobile");

        Assert.Equal(definition.PageTag, viewModel.PageTag);
        Assert.Equal(definition.Title, viewModel.Title);
        Assert.Equal(definition.Category, viewModel.Category);
        Assert.Equal(definition.Description, viewModel.Description);
        Assert.Equal(definition.ExampleMarkup, viewModel.ExampleMarkup);
        Assert.Equal(definition.ExampleCSharp, viewModel.ExampleCSharp);
        Assert.Equal("Mobile", viewModel.ViewportMode);
        Assert.Equal(420, viewModel.PreviewMaxWidth);
        Assert.Contains("420", viewModel.ViewportStatus);
    }

    [Fact]
    public void CodeTabAndResetCommand_RestoreDefaultInspectorState()
    {
        var viewModel = CreateViewModel();
        viewModel.ShowCodeCommand.Execute(null);
        viewModel.PreviewPadding = 31;
        viewModel.CornerRadius = 23;
        viewModel.PreviewOpacity = 0.45;
        viewModel.IsPreviewEnabled = false;
        viewModel.SetViewportCommand.Execute("Tablet");

        Assert.True(viewModel.ShowingCode);
        Assert.False(viewModel.ShowingPreview);

        viewModel.ResetCommand.Execute(null);

        Assert.False(viewModel.ShowingCode);
        Assert.True(viewModel.ShowingPreview);
        Assert.Equal(18, viewModel.PreviewPadding);
        Assert.Equal(8, viewModel.CornerRadius);
        Assert.Equal(1, viewModel.PreviewOpacity);
        Assert.Equal(100, viewModel.PreviewOpacityPercent);
        Assert.True(viewModel.IsPreviewEnabled);
        Assert.Equal("Desktop", viewModel.ViewportMode);
    }

    [Fact]
    public void CustomSurface_IsPreservedAcrossThemeNotifications()
    {
        var originalMode = GalleryTheme.CurrentMode;
        var theme = new Mock<IGalleryThemeService>();
        theme.SetupGet(service => service.IsDark)
            .Returns(() =>
                GalleryTheme.CurrentMode == GalleryThemeMode.Dark);
        var viewModel =
            new GalleryDemoWorkbenchViewModel(theme.Object);
        var custom = Color.FromRgb(0x42, 0x63, 0x84);

        try
        {
            viewModel.Surface = custom;
            GalleryTheme.CurrentMode = Opposite(originalMode);
            theme.Raise(
                service => service.ThemeChanged += null,
                EventArgs.Empty);

            Assert.Equal(custom, viewModel.Surface);
        }
        finally
        {
            viewModel.Destroy();
            GalleryTheme.CurrentMode = originalMode;
        }
    }

    [Fact]
    public void ThemeCommands_DelegateToThemeService()
    {
        var theme = new Mock<IGalleryThemeService>();
        var viewModel =
            new GalleryDemoWorkbenchViewModel(theme.Object);

        viewModel.UseLightThemeCommand.Execute(null);
        viewModel.UseDarkThemeCommand.Execute(null);

        theme.Verify(service => service.SetDark(false), Times.Once);
        theme.Verify(service => service.SetDark(true), Times.Once);
    }

    private static GalleryDemoWorkbenchViewModel CreateViewModel() =>
        new(Mock.Of<IGalleryThemeService>());

    private static GalleryWorkbenchDefinition CreateDefinition() =>
        new(
            "fixture",
            "Fixture",
            "Layout",
            "A workbench fixture.",
            "<Border />",
            "var border = new Border();");

    private static GalleryThemeMode Opposite(
        GalleryThemeMode mode) =>
        mode == GalleryThemeMode.Dark
            ? GalleryThemeMode.Light
            : GalleryThemeMode.Dark;

    private static string FindRepositoryRoot(
        [CallerFilePath] string sourceFile = "")
    {
        var directory = new DirectoryInfo(
            Path.GetDirectoryName(sourceFile)
            ?? throw new InvalidOperationException(
                "Unable to locate test source."));

        while (directory != null &&
               !File.Exists(Path.Combine(
                   directory.FullName,
                   "Jalium.UI.Gallery.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                "Unable to locate repository root.");
    }
}
