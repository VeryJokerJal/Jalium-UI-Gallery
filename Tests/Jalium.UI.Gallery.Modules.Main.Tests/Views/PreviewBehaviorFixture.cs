using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media.Effects;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class PreviewBehaviorFixture
{
    [Fact]
    public void EffectControls_UpdateTheirViewModelAndNativeEffectParameters()
    {
        var model = new ShaderEffectsViewModel();
        var page = new ShaderEffectsPage { DataContext = model };
        Layout(page);

        var blur = Assert.IsType<BlurEffect>(Assert.IsType<Border>(page.FindName("BlurPreview")).Effect);
        Assert.Equal(10, blur.Radius);
        Assert.Equal(KernelType.Gaussian, blur.KernelType);
        Assert.IsType<Slider>(page.FindName("BlurRadiusSlider")).Value = 24;
        Assert.IsType<CheckBox>(page.FindName("BoxBlurCheckBox")).IsChecked = true;
        Assert.Equal(24, model.BlurRadius);
        Assert.True(model.UseBoxBlur);
        Assert.Equal(24, blur.Radius);
        Assert.Equal(KernelType.Box, blur.KernelType);

        var shadow = Assert.IsType<DropShadowEffect>(Assert.IsType<Border>(page.FindName("ShadowPreview")).Effect);
        Assert.IsType<Slider>(page.FindName("ShadowDepthSlider")).Value = 20;
        Assert.Equal(20, model.ShadowDepth);
        Assert.Equal(20, shadow.ShadowDepth);

        var invert = Assert.IsType<GalleryInvertEffect>(Assert.IsType<Border>(page.FindName("HlslPreview")).Effect);
        Assert.Equal(0.75, invert.Amount);
        Assert.IsType<Slider>(page.FindName("InvertAmountSlider")).Value = 0.25;
        Assert.Equal(0.25, model.InvertAmount);
        Assert.Equal(0.25, invert.Amount);

        model.BlurRadius = 8;
        model.UseBoxBlur = false;
        Assert.Equal(8, Assert.IsType<Slider>(page.FindName("BlurRadiusSlider")).Value);
        Assert.Equal(8, blur.Radius);
        Assert.Equal(KernelType.Gaussian, blur.KernelType);
    }

    [Fact]
    public void ScrollButtons_PageOnceAfterRepeatedLayoutAndBehaviorReattachment()
    {
        var page = new ScrollViewerPage { DataContext = new ScrollViewerViewModel() };
        Layout(page);
        Layout(page);
        var viewer = Assert.IsType<ScrollViewer>(page.FindName("HorizontalPreviewViewer"));
        var right = Assert.IsType<Button>(page.FindName("ScrollRightButton"));
        var left = Assert.IsType<Button>(page.FindName("ScrollLeftButton"));
        Assert.True(viewer.ScrollableWidth > viewer.ViewportWidth);
        right.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, right));
        Assert.Equal(viewer.ViewportWidth, viewer.HorizontalOffset, 3);
        left.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, left));
        Assert.Equal(0, viewer.HorizontalOffset);

        ScrollViewerPreviewBehavior.SetIsEnabled(page, false);
        right.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, right));
        Assert.Equal(0, viewer.HorizontalOffset);
        ScrollViewerPreviewBehavior.SetIsEnabled(page, true);
        Layout(page);
        right.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, right));
        Assert.Equal(viewer.ViewportWidth, viewer.HorizontalOffset, 3);
    }

    private static void Layout(Page page)
    {
        page.Measure(new Size(520, 3200));
        page.Arrange(new Rect(0, 0, 520, 3200));
        page.UpdateLayout();
        Jalium.UI.Threading.Dispatcher.CurrentDispatcher.ProcessQueue();
    }
}
