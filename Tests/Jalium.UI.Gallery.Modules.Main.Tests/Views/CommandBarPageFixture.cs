using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class CommandBarPageFixture
{
    [Theory]
    [InlineData(960)]
    [InlineData(390)]
    public void ResponsiveBarsKeepCommandsBindingStateAndLabelOptions(double width)
    {
        var model = new CommandBarViewModel();
        var page = new CommandBarPage { DataContext = model };
        using var stream = new MemoryStream();
        var defaults = ThemeManager.XamlLoader!(stream, "Themes/Generic.jalxaml", typeof(Control).Assembly)!;
        page.Resources.MergedDictionaries.Add(defaults);
        var window = new DisplayedWindow { Width = width, Height = 1000, Content = page };
        try
        {
            Layout(window, width, 1000);
            var basic = Assert.IsType<CommandBar>(page.FindName("PART_BasicCommandBar"));
            var overflow = Assert.IsType<CommandBar>(page.FindName("PART_OverflowCommandBar"));
            Assert.InRange(basic.ActualWidth, 1, width);
            Assert.InRange(overflow.ActualWidth, 1, width);
            Assert.Equal(4, basic.PrimaryCommands.Count);
            Assert.Equal(3, overflow.SecondaryCommands.Count);
            var add = Assert.IsType<AppBarButton>(basic.PrimaryCommands[0]);
            add.PerformClick();
            Assert.Equal("Add command executed.", model.BasicStatus);
            Assert.Equal(model.BasicStatus, Assert.IsType<TextBlock>(page.FindName("PART_BasicStatus")).Text);
            model.LabelPosition = CommandBarDefaultLabelPosition.Right;
            Layout(window, width, 1000);
            Assert.Equal(Orientation.Horizontal, Assert.IsType<StackPanel>(add.FindName("PART_ContentPanel")).Orientation);
            model.ClosedDisplayMode = CommandBarClosedDisplayMode.Minimal;
            Layout(window, width, 1000);
            Assert.Equal(Visibility.Collapsed, Assert.IsType<ItemsControl>(overflow.FindName("PART_PrimaryItemsControl")).Visibility);
            Assert.IsType<Popup>(overflow.FindName("PART_OverflowPopup")).ShouldConstrainToRootBounds = true;
            model.OverflowOpen = true;
            Layout(window, width, 1000);
            Assert.True(overflow.IsOpen);
            var settings = Assert.IsType<AppBarButton>(overflow.SecondaryCommands[0]);
            Assert.NotNull(settings.Command);
            settings.PerformClick();
            Assert.Equal("Settings command executed.", model.SecondaryStatus);
            Assert.False(model.OverflowOpen);
            model.KeepOverflowOpen = true;
            model.OverflowOpen = true;
            Layout(window, width, 1000);
            settings.PerformClick();
            Assert.True(model.OverflowOpen);
            Assert.True(overflow.IsOpen);
        }
        finally { window.Close(); }
    }

    [Fact]
    public void CommandsFollowLightAndDarkPaletteChangesWithoutReplacingTheirTemplate()
    {
        var originalLanguage = LocalizationManager.CurrentLanguage;
        var app = new App();
        var page = new CommandBarPage { DataContext = new CommandBarViewModel() };
        var window = new DisplayedWindow { Width = 390, Height = 1000, Content = page };
        app.MainWindow = window;
        var theme = new GalleryThemeService();
        try
        {
            Layout(window, 390, 1000);
            var bar = Assert.IsType<CommandBar>(page.FindName("PART_BasicCommandBar"));
            var command = Assert.IsType<AppBarButton>(bar.PrimaryCommands[0]);
            var template = command.Template;
            foreach (var dark in new[] { true, false, true })
            {
                theme.SetDark(dark);
                Layout(window, 390, 1000);
                var expected = Assert.IsType<SolidColorBrush>(command.TryFindResource("AppBarButtonForeground"));
                var actual = Assert.IsType<SolidColorBrush>(command.Foreground);
                Assert.True(expected.Color == actual.Color,
                    $"dark={dark}, expected={expected.Color}, command={actual.Color}, local={command.HasLocalValue(Control.ForegroundProperty)}, bar={bar.Foreground}");
                Assert.Same(command.Foreground, Assert.IsType<PathIcon>(command.Icon).Foreground);
                Assert.Same(template, command.Template);
            }
        }
        finally { theme.SetDark(false); window.Close(); LocalizationManager.SetLanguage(originalLanguage); }
    }

    [Fact]
    public void AccentCommandTextRetainsContrastOnNormalHoveredAndPressedToolbarSurfaces()
    {
        var originalLanguage = LocalizationManager.CurrentLanguage;
        var app = new App();
        var probe = new AppBarButton();
        var theme = new GalleryThemeService();
        var originalAccent = ThemeManager.CurrentAccentColor;
        var accents = new[] { originalAccent, Color.FromRgb(8, 148, 138), Color.FromRgb(79, 70, 229),
            Color.FromRgb(2, 132, 199), Color.FromRgb(22, 163, 74), Color.FromRgb(217, 119, 6),
            Color.FromRgb(225, 29, 72), Color.White, Color.Black };
        try
        {
            foreach (var dark in new[] { false, true })
            {
                theme.SetDark(dark);
                foreach (var accent in accents)
                {
                    ThemeManager.ApplyAccent(accent);
                    var foreground = Assert.IsType<SolidColorBrush>(probe.TryFindResource("AppBarButtonForeground")).Color;
                    foreach (var key in new[] { "CommandBarBackground", "AppBarButtonBackgroundHover", "AppBarButtonBackgroundPressed" })
                    {
                        var background = Assert.IsType<SolidColorBrush>(probe.TryFindResource(key)).Color;
                        var contrast = (Math.Max(Luminance(foreground), Luminance(background)) + .05)
                            / (Math.Min(Luminance(foreground), Luminance(background)) + .05);
                        Assert.True(contrast >= 4.5, $"dark={dark}, accent={accent}: {foreground} on {key}={background}: {contrast:0.00}");
                    }
                }
            }
        }
        finally { ThemeManager.ApplyAccent(originalAccent); theme.SetDark(false); LocalizationManager.SetLanguage(originalLanguage); GC.KeepAlive(app); }
    }

    private static double Luminance(Color color)
    {
        static double Linear(byte value)
        {
            double v = value / 255.0;
            return v <= .04045 ? v / 12.92 : Math.Pow((v + .055) / 1.055, 2.4);
        }
        return .2126 * Linear(color.R) + .7152 * Linear(color.G) + .0722 * Linear(color.B);
    }

    private static void Layout(FrameworkElement element, double width, double height)
    {
        element.Measure(new Size(width, height));
        element.Arrange(new Rect(0, 0, width, height));
        element.UpdateLayout();
    }
    private sealed class DisplayedWindow : Window
    {
        public DisplayedWindow()
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, new object[] { Visibility.Visible });
        }
    }
}
