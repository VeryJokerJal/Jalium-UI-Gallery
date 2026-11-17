using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Jalium.UI.Media.Animation;
using Jalium.UI.Media.Effects;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class OverviewTemplateFixture
{
    [Fact]
    public void DisabledEntranceAndIndependentPreviewAreImmediatelyReadable()
    {
        var page = new OverviewPage { IsEntranceAnimationEnabled = false };
        Layout(page);
        Assert.Equal("Visible", VisualStateManager.GetCurrentStateName(page, "EntranceStates"));
        Assert.Equal(1, Part<TextBlock>(page, "TitleLine3").Opacity);
        Assert.Equal(1, Part<StackPanel>(page, "CtaRow").Opacity);
        Assert.Same(page.Background, Part<Border>(page, "PosterRoot").Background);
        var template = Assert.IsType<DataTemplate>(GalleryControlThemes.FindPreviewTemplate("Gallery.Preview.OverviewPage"));
        var preview = Assert.IsType<Viewbox>(template.LoadContent());
        var previewPage = Assert.IsType<OverviewPage>(preview.Child);
        Assert.NotSame(page, previewPage);
        Assert.False(previewPage.IsEntranceAnimationEnabled);
        Assert.Equal(1, Part<TextBlock>(previewPage, "TitleLine3").Opacity);
    }

    [Fact]
    public void EntranceStoryboardTargetsAllTemplatePartsAndUnloadStopsIt()
    {
        var page = new OverviewPage();
        Layout(page);
        var root = Part<Grid>(page, "PART_Root");
        var group = VisualStateManager.GetVisualStateGroups(root)!.OfType<VisualStateGroup>().Single(g => g.Name == "EntranceStates");
        var entering = group.States.OfType<VisualState>().Single(s => s.Name == "Entering");
        Assert.Equal(24, entering.Storyboard!.Children.Count);
        foreach (var animation in entering.Storyboard.Children.OfType<DoubleAnimation>())
            Assert.IsAssignableFrom<UIElement>(page.FindName(Storyboard.GetTargetName(animation)!));
        Loaded(page, true);
        try
        {
            Assert.True(page.HasEntered);
            Assert.Equal(SystemParameters.ClientAreaAnimation && SystemParameters.UIEffects ? "Entering" : "Visible", group.CurrentState?.Name);
            Loaded(page, false);
            Assert.Equal("Visible", group.CurrentState?.Name);
            Assert.Equal(1, Part<TextBlock>(page, "TitleLine3").Opacity);
            Loaded(page, true);
            Assert.Equal("Visible", group.CurrentState?.Name);
        }
        finally { Loaded(page, false); }
    }

    [Fact]
    public void PaletteIsTemplateOwnedAndEnvironmentSubscriptionsEndOnUnload()
    {
        var initial = GalleryTheme.CurrentMode;
        OverviewPage? page = null;
        try
        {
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;
            page = new OverviewPage { IsEntranceAnimationEnabled = false };
            Layout(page);
            Loaded(page, true);
            var halo = Part<Border>(page, "HaloEmerald");
            Assert.Equal(0.35, Assert.IsType<OuterGlowEffect>(halo.Effect).Opacity);
            Assert.Equal(Color.FromArgb(0x2A, 0x1F, 0x79, 0x3F), Assert.IsType<SolidColorBrush>(halo.Background).Color);
            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;
            Assert.Equal("DarkTheme", VisualStateManager.GetCurrentStateName(page, "PaletteStates"));
            Assert.Equal(0.6, Assert.IsType<OuterGlowEffect>(halo.Effect).Opacity);
            Assert.Equal(Color.FromArgb(0x3A, 0x1F, 0x79, 0x3F), Assert.IsType<SolidColorBrush>(halo.Background).Color);
            GalleryTheme.CurrentMode = GalleryThemeMode.Light;
            Assert.Equal(0.35, Assert.IsType<OuterGlowEffect>(halo.Effect).Opacity);
            Loaded(page, false);
            GalleryTheme.CurrentMode = GalleryThemeMode.Dark;
            Assert.Equal("LightTheme", VisualStateManager.GetCurrentStateName(page, "PaletteStates"));
            Loaded(page, true);
            Assert.Equal("DarkTheme", VisualStateManager.GetCurrentStateName(page, "PaletteStates"));
        }
        finally { if (page != null) Loaded(page, false); GalleryTheme.CurrentMode = initial; }
    }

    [Fact]
    public void TemplateReplacementStopsOldAnimationAndAcceptsMissingParts()
    {
        var page = new OverviewPage();
        Layout(page);
        Loaded(page, true);
        try
        {
            var oldRoot = Part<Grid>(page, "PART_Root");
            var oldTitle = Part<TextBlock>(page, "TitleLine3");
            var original = page.Template;
            var empty = new ControlTemplate(typeof(OverviewPage));
            empty.SetVisualTree(static () => new Border());
            page.Template = empty;
            Layout(page);
            var oldGroup = VisualStateManager.GetVisualStateGroups(oldRoot)!.OfType<VisualStateGroup>().Single(g => g.Name == "EntranceStates");
            Assert.NotEqual("Entering", oldGroup.CurrentState?.Name);
            page.Template = original;
            Layout(page);
            Assert.NotSame(oldTitle, Part<TextBlock>(page, "TitleLine3"));
            Assert.Equal(1, Part<TextBlock>(page, "TitleLine3").Opacity);
            Assert.Equal("Visible", VisualStateManager.GetCurrentStateName(page, "EntranceStates"));
        }
        finally { Loaded(page, false); }
    }

    [Theory]
    [InlineData(320)]
    [InlineData(390)]
    [InlineData(720)]
    public void CompactTemplateFitsTheViewportAndPlacesSamplesAfterTheHero(double width)
    {
        var page = new OverviewPage { IsEntranceAnimationEnabled = false };
        page.Measure(new Size(width, double.PositiveInfinity));
        page.Arrange(new Rect(0, 0, width, page.DesiredSize.Height));
        page.UpdateLayout();
        Assert.Equal("Compact", VisualStateManager.GetCurrentStateName(page, "LayoutStates"));
        var title = Part<TextBlock>(page, "TitleLine3");
        var button = Part<Button>(page, "EnterButton");
        var samples = Part<StackPanel>(page, "SpecimenRail");
        var footer = Part<Grid>(page, "Colophon");
        foreach (var element in new FrameworkElement[] { title, button, samples, footer })
        {
            var bounds = new Rect(element.TransformToAncestor(page), element.RenderSize);
            Assert.True(bounds.Left >= -0.1, $"{element.Name} starts outside the viewport: {bounds}");
            Assert.True(bounds.Right <= width + 0.1, $"{element.Name} exceeds the viewport: {bounds}");
        }
        var buttonBounds = new Rect(button.TransformToAncestor(page), button.RenderSize);
        var sampleBounds = new Rect(samples.TransformToAncestor(page), samples.RenderSize);
        var footerBounds = new Rect(footer.TransformToAncestor(page), footer.RenderSize);
        Assert.True(sampleBounds.Top >= buttonBounds.Bottom + 20);
        Assert.True(footerBounds.Top >= sampleBounds.Bottom);
        Layout(page);
        Assert.Equal("Wide", VisualStateManager.GetCurrentStateName(page, "LayoutStates"));
        Assert.Equal(3, Grid.GetColumn(samples));
        Assert.Equal(1, Grid.GetRow(samples));
        Assert.Equal(84, title.FontSize);
        Assert.Equal(Visibility.Visible, Part<Border>(page, "SpineRule").Visibility);
    }

    private static T Part<T>(FrameworkElement owner, string name) where T : class => Assert.IsAssignableFrom<T>(owner.FindName(name));
    private static void Layout(UIElement element) { element.Measure(new Size(1500, 920)); element.Arrange(new Rect(0, 0, 1500, 920)); }
    private static void Loaded(FrameworkElement element, bool loaded) => typeof(FrameworkElement).GetMethod("SetLoadedState", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(element, [loaded]);
}
