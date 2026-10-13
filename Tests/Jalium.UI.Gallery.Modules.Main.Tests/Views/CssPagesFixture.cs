using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

/// <summary>
/// The styling pages exist to show CSS working, so it is not enough that they instantiate:
/// each demo must actually be styled by the engine. These tests lay a page out and assert
/// the CSS-supplied values landed on the elements.
/// </summary>
public sealed class CssPagesFixture
{
    static CssPagesFixture()
    {
        // In the app the page's AutoWireViewModel resolves the view model (which
        // registers the custom vocabulary) before the page's children are built.
        // The tests construct pages directly, so register up front — otherwise the
        // Css.Style declarations compile before the names exist, and headless runs
        // have no application root invalidator to recover them.
        Jalium.UI.Gallery.Modules.Main.Models.GalleryCssExtensions.EnsureRegistered();
    }

    private const double Width = 1200;
    private const double Height = 3000;

    [Fact]
    public void StylingPage_PercentageRadiusFollowsNativeWidthBinding()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var slider = Assert.IsType<Slider>(page.FindName("CssRadiusWidthSlider"));
        var oval = FindBorder(page, "CssPercentageRadiusTile");
        var native = FindBorder(page, "CssNativeRadiusTile");
        var binding = Jalium.UI.Data.BindingOperations.GetBindingExpression(oval, FrameworkElement.WidthProperty);
        Assert.NotNull(binding);
        Assert.Equal(200, oval.ActualWidth, 3);
        Assert.Equal(new CornerRadius(100), oval.CornerRadius);
        slider.Value = 300; page.UpdateLayout();
        Assert.Equal(300, oval.ActualWidth, 3);
        Assert.Equal(new CornerRadius(150), oval.CornerRadius);
        Assert.Equal(new CornerRadius(8), native.CornerRadius);
        Assert.Same(binding, Jalium.UI.Data.BindingOperations.GetBindingExpression(oval, FrameworkElement.WidthProperty));
    }

    [Fact]
    public void StylingPage_EllipticalOverflowClipsTheOriginalChild()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var host = FindBorder(page, "CssRadiusClipHost");
        var child = FindBorder(page, "CssRadiusClipContent");
        Assert.True(host.ClipToBounds);
        Assert.Same(child, host.Child);
        Assert.Same(host, child.VisualParent);
        var origin = host.VisualBounds;
        Assert.Null(host.HitTest(new Point(origin.X + 1, origin.Y + 1)));
        Assert.Same(child, host.HitTest(new Point(origin.X + 80, origin.Y + 40))?.VisualHit);
    }

    [Fact]
    public void StylingPage_ImageBrushesKeepTheirNativeSourcesAndCssContours()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var ellipse = FindBorder(page, "CssImageBrushEllipse");
        var corners = FindBorder(page, "CssImageBrushCorners");
        var ellipseBrush = Assert.IsType<ImageBrush>(ellipse.Background);
        var cornerBrush = Assert.IsType<ImageBrush>(corners.Background);
        Assert.IsType<Jalium.UI.Media.Imaging.BitmapImage>(ellipseBrush.ImageSource);
        Assert.Same(ellipseBrush.ImageSource, cornerBrush.ImageSource);
        Assert.Equal(new CornerRadius(90), ellipse.CornerRadius);
        Assert.Equal(TileMode.FlipX, cornerBrush.TileMode);
        Assert.Equal(new Rect(0, 0, .5, 1), cornerBrush.Viewport);
        Assert.True(ellipse.ClipToBounds);
        Assert.True(corners.ClipToBounds);

        var shadow = FindBorder(page, "CssEllipticalShadowTile");
        Assert.Equal(new CornerRadius(60, 18, 60, 18), shadow.CornerRadius);
        Assert.IsType<DropShadowEffect>(shadow.Effect);
        var inset = FindBorder(page, "CssInsetShadowTile");
        Assert.Equal(new Thickness(6), inset.BorderThickness);
        Assert.Equal(new CornerRadius(60, 18, 60, 18), inset.CornerRadius);
        Assert.Equal(5, Assert.IsType<InnerShadowEffect>(inset.Effect).SpreadRadius);
    }

    [Fact]
    public void StylingPage_ButtonTemplateKeepsBothRadiusAxesAndItsBinding()
    {
        // This case exercises an implicit native theme template, so provide
        // the same application context as the running Gallery.
        var previousApplication = Application.Current;
        var application = previousApplication ?? new Application();
        try
        {
            var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
            var button = Assert.IsType<Button>(page.FindName("CssTemplateRadiusButton"));
            var chrome = Assert.IsType<Border>(button.FindName("RootBorder"));
            Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpressionBase(chrome, Border.CornerRadiusProperty));
            var origin = button.VisualBounds;
            Assert.Null(button.HitTest(new Point(origin.X + 1, origin.Y + 1)));
            Assert.Equal("Template corners", button.Content);
            button.CornerRadius = new CornerRadius(0);
            Assert.NotNull(button.HitTest(new Point(origin.X + 1, origin.Y + 1)));
            Assert.Equal(new CornerRadius(0), chrome.CornerRadius);
        }
        finally
        {
            typeof(Application).GetField("_current", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
                .SetValue(null, previousApplication);
            GC.KeepAlive(application);
        }
    }

    [Fact]
    public void StylingPage_ResolvesVariablesMathLayersAutoMarginsAndPercentageBasis()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var tile = FindBorder(page, "ComputedCssTile");
        Assert.Equal(388, tile.ActualWidth, 3);
        Assert.Equal(16, tile.VisualBounds.X, 3);
        Assert.Equal(Hex(0x2563EB), ColorOf(tile.Background));
        Assert.Equal(105, FindBorder(page, "PercentBasisCssTile").ActualWidth, 3);
    }

    [Fact]
    public void StylingPage_RegisteredPropertiesValidateNativeBoundDeclarationText()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var input = Assert.IsType<TextBox>(page.FindName("CssRegisteredDeclarationInput"));
        var meter = FindBorder(page,"CssRegisteredMeter");
        Assert.Equal(220,meter.ActualWidth,3); Assert.Equal(Hex(0x0F766E),ColorOf(meter.Background));
        const string invalid = "--gallery-meter-width:invalid; --gallery-meter-color:42px";
        input.Text = invalid; page.UpdateLayout();
        Assert.Equal(160,meter.ActualWidth,3); Assert.Equal(Hex(0x2563EB),ColorOf(meter.Background));
        Assert.Equal(invalid,input.Text);
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(meter,Jalium.UI.Styling.Css.StyleProperty));
    }

    [Fact]
    public void StylingPage_MathFunctionsFollowTheNativeFontBinding()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var slider = Assert.IsType<Slider>(page.FindName("CssMathFontSlider"));
        var box = Assert.IsType<TextBox>(page.FindName("CssMathDemoBox"));
        var binding = Jalium.UI.Data.BindingOperations.GetBindingExpression(box, Control.FontSizeProperty);
        Assert.NotNull(binding);
        Assert.Equal(160, box.ActualWidth, 3); Assert.Equal(.5, box.Opacity, 6);
        slider.Value = 40; page.UpdateLayout();
        Assert.Equal(40, box.FontSize); Assert.Equal(320, box.ActualWidth, 3); Assert.Equal(1, box.Opacity, 6);
        Assert.Equal("CSS math", box.Text);
        Assert.Same(binding, Jalium.UI.Data.BindingOperations.GetBindingExpression(box, Control.FontSizeProperty));
    }

    [Fact]
    public void StylingPage_DownloadedFontPreservesTextBindingAndNativeSize()
    {
        using var context = Jalium.UI.Interop.RenderContext.GetOrCreateCurrent(Jalium.UI.Interop.RenderBackend.Software, forceReplace: true);
        var page = new CssStylingPage { DataContext = new CssStylingViewModel() };
        page.FontStylesReady.GetAwaiter().GetResult(); Layout(page);
        Jalium.UI.Styling.Css.WaitForFontsAsync(page).GetAwaiter().GetResult(); page.UpdateLayout();
        var text = Assert.IsType<TextBlock>(page.FindName("CssFontFaceText"));
        var input = Assert.IsType<TextBox>(page.FindName("CssFontFaceInput"));
        Assert.Empty(Jalium.UI.Styling.Css.GetFontLoadErrors(page));
        Assert.Equal(20, text.FontSize);
        Assert.Equal(82, text.DesiredSize.Width, 3);
        input.Text = "XXXXXX"; page.UpdateLayout();
        Assert.Equal("XXXXXX", text.Text); Assert.Equal(122, text.DesiredSize.Width, 3);
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(text, TextBlock.TextProperty));
        Jalium.UI.Interop.TextMeasurement.ClearCache();
    }

    [Fact]
    public void StylingPage_FontUnitsAndViewportVariantsUpdateIndependently()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });
        var fontSlider = Assert.IsType<Slider>(page.FindName("CssMathFontSlider"));
        var viewportSlider = Assert.IsType<Slider>(page.FindName("CssViewportDynamicSlider"));
        var fontBox = FindBorder(page, "CssGlyphUnitDemoBox");
        Assert.Equal(56, fontBox.ActualHeight, 3);
        Assert.Equal(12d * Jalium.UI.Interop.TextMeasurement.GetFontUnitMetrics("Consolas", 20).ZeroAdvance, fontBox.ActualWidth, 3);
        Assert.Equal(20, FindBorder(page, "CssSmallViewportBox").ActualHeight, 3);
        Assert.Equal(30, FindBorder(page, "CssDynamicViewportBox").ActualHeight, 3);
        Assert.Equal(40, FindBorder(page, "CssLargeViewportBox").ActualHeight, 3);
        fontSlider.Value = 40; viewportSlider.Value = 200; page.UpdateLayout();
        Assert.Equal(112, fontBox.ActualHeight, 3);
        Assert.Equal(20, FindBorder(page, "CssSmallViewportBox").ActualHeight, 3);
        Assert.Equal(40, FindBorder(page, "CssDynamicViewportBox").ActualHeight, 3);
        Assert.Equal(40, FindBorder(page, "CssLargeViewportBox").ActualHeight, 3);
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(fontBox, Jalium.UI.Documents.TextElement.FontSizeProperty));
    }

    [Fact]
    public async Task StylingPage_ConditionalImportsLoadEmbeddedAssetsAndKeepNativeBindings()
    {
        var page=new CssStylingPage {DataContext=new CssStylingViewModel()};
        await page.ImportStylesReady; Layout(page);
        var slider=Assert.IsType<Slider>(page.FindName("CssImportWidthSlider"));
        var host=Assert.IsType<StackPanel>(page.FindName("CssImportDemoHost"));
        var first=FindBorder(page,"CssImportFirstTile"); var second=FindBorder(page,"CssImportSecondTile");
        Assert.Equal(206,first.ActualWidth,3); Assert.Equal(214,second.VisualBounds.X,3);
        Assert.Equal(Hex(0x2563EB),ColorOf(first.Background));
        slider.Value=300; page.UpdateLayout();
        Assert.Equal(300,first.ActualWidth,3); Assert.Equal(48,second.VisualBounds.Y,3); Assert.Equal(Hex(0x475569),ColorOf(first.Background));
        slider.Value=420; page.Measure(new Size(500,Height)); page.Arrange(new Rect(0,0,500,Height)); page.UpdateLayout();
        Assert.Equal(420,first.ActualWidth,3); Assert.Equal(Hex(0x475569),ColorOf(first.Background));
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(host,FrameworkElement.WidthProperty));
    }

    private static T Layout<T>(T page)
        where T : Page
    {
        // The first measure establishes the constraint UpdateLayout reuses; UpdateLayout then
        // settles the pending CSS and lays the page out with it applied.
        page.Measure(new Size(Width, Height));
        page.UpdateLayout();
        page.Arrange(new Rect(0, 0, Width, Height));
        return page;
    }

    private static Border FindBorder(Page page, string name)
        => Assert.IsType<Border>(page.FindName(name));

    private static Color ColorOf(Brush? brush)
        => Assert.IsType<SolidColorBrush>(brush).Color;

    private static Color Hex(uint rgb)
        => Color.FromArgb(0xFF, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);

    [Fact]
    public void StylingPage_AppliesInlineDeclarationsAndTheDeclaredSheet()
    {
        var page = Layout(new CssStylingPage { DataContext = new CssStylingViewModel() });

        // Css.Style — an inline declaration block.
        var inline = FindBorder(page, "InlineTile");
        Assert.Equal(Hex(0x0078D4), ColorOf(inline.Background));
        Assert.Equal(new CornerRadius(8), inline.CornerRadius);
        Assert.Equal(new Thickness(18, 14, 18, 14), inline.Padding);

        // Css.StyleSheet — an id selector inside a sheet declared in markup.
        var hero = FindBorder(page, "hero");
        Assert.Equal(Hex(0x0F766E), ColorOf(hero.Background));
        Assert.Equal(new CornerRadius(16), hero.CornerRadius);
    }

    [Fact]
    public void SelectorsPage_ScopeLimitsFollowNativeEnabledBindings()
    {
        var page=Layout(new CssSelectorsPage {DataContext=new CssSelectorsViewModel()});
        var toggle=Assert.IsType<CheckBox>(page.FindName("CssScopeEnabledToggle"));
        var boundary=Assert.IsType<StackPanel>(page.FindName("CssScopeBoundary"));
        var always=FindBorder(page,"CssScopeAlwaysTile"); var conditional=FindBorder(page,"CssScopeConditionalTile");
        Assert.Equal(Hex(0x2563EB),ColorOf(always.Background)); Assert.Equal(Hex(0x2563EB),ColorOf(conditional.Background));
        toggle.IsChecked=false; page.UpdateLayout();
        Assert.False(boundary.IsEnabled); Assert.Equal(Hex(0x2563EB),ColorOf(always.Background)); Assert.Equal(Hex(0x475569),ColorOf(conditional.Background));
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(boundary,UIElement.IsEnabledProperty));
        Assert.Same(boundary,conditional.VisualParent);
    }

    [Fact]
    public void SelectorsPage_NamespacesPreserveCompiledNamesAndNativeTagBindings()
    {
        var page=Layout(new CssSelectorsPage {DataContext=new CssSelectorsViewModel()});
        var plain=FindBorder(page,"CssNamespaceDefaultTile"); var qualified=FindBorder(page,"CssNamespaceQualifiedTile");
        var input=Assert.IsType<TextBox>(page.FindName("CssNamespaceValueInput"));
        Assert.Equal(Hex(0x2563EB),ColorOf(plain.Background)); Assert.Equal(Hex(0x0F766E),ColorOf(qualified.Background));
        Assert.Equal(2,qualified.OutlineThickness);
        input.Text="normal"; page.UpdateLayout(); Assert.Equal(0,qualified.OutlineThickness);
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(qualified,FrameworkElement.TagProperty));
    }

    [Fact]
    public void SelectorsPage_ResolvesSpecificityAndImportant()
    {
        var page = Layout(new CssSelectorsPage { DataContext = new CssSelectorsViewModel() });

        // An id selector outranks every class selector.
        Assert.Equal(Hex(0x6D28D9), ColorOf(FindBorder(page, "winner").Background));

        // !important on a type rule survives a more specific normal rule.
        Assert.Equal(Hex(0xBE123C), ColorOf(FindBorder(page, "louder").Background));
    }

    [Fact]
    public void LayoutPage_ResolvesPercentagesAspectRatioAndBoxSizing()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });

        // width: 50% of the 420-wide block, less its 6px padding on each side.
        Assert.Equal(204, FindBorder(page, "HalfBar").ActualWidth, 3);

        // A vertical percentage resolves where the constraint is determinate: 60% of the
        // 150-high block less its 6px padding on each side.
        Assert.Equal(82.8, FindBorder(page, "TallBar").ActualHeight, 3);

        // aspect-ratio: 16/9 derives the height from the declared width.
        var ratio = FindBorder(page, "RatioTile");
        Assert.Equal(160, ratio.ActualWidth, 3);
        Assert.Equal(90, ratio.ActualHeight, 3);

        // content-box adds the 20px padding and 4px border on each side to the 200px width.
        Assert.Equal(248, FindBorder(page, "ContentBoxTile").ActualWidth, 3);
    }

    [Fact]
    public void LayoutPage_UsesCssGridOnItsExistingStackPanel()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });
        var panel = Assert.IsType<StackPanel>(page.FindName("CssGridDemo"));
        var wide = FindBorder(page, "CssGridWideTile"); var last = FindBorder(page, "CssGridLastTile");
        Assert.Equal((420d - 16) / 3 * 2 + 8, wide.ActualWidth, 3);
        Assert.Equal(420, last.ActualWidth, 3); Assert.Equal(64, last.VisualBounds.Y, 3);
        Assert.Same(panel, wide.VisualParent); Assert.Same(wide, panel.Children[0]);
    }

    [Fact]
    public void LayoutPage_SubgridSharesColumnsAndKeepsItsNativeChildren()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });
        var subgrid = Assert.IsType<StackPanel>(page.FindName("CssSubgridDemo"));
        var first = FindBorder(page, "CssSubgridFirstTile"); var last = FindBorder(page, "CssSubgridLastTile");
        Assert.Equal(96, first.ActualWidth, 3); Assert.Equal(4, first.VisualBounds.X, 3);
        Assert.Equal(148, last.ActualWidth, 3); Assert.Equal(268, last.VisualBounds.X, 3);
        Assert.Same(subgrid, first.VisualParent); Assert.Same(first, subgrid.Children[0]);
    }

    [Fact]
    public void LayoutPage_FlowWrapsControlsAndCollapsesAdjacentMargins()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });
        var root = Assert.IsType<StackPanel>(page.FindName("CssFlowDemo"));
        var first = FindBorder(page, "CssFlowFirstTile");
        Assert.Equal(8, first.VisualBounds.X, 3); Assert.Equal(170, first.ActualWidth, 3);
        Assert.Equal(58, FindBorder(page, "CssFlowWrappedTile").VisualBounds.Y, 3);
        var block = FindBorder(page, "CssFlowBlockTile");
        Assert.Equal(124, block.VisualBounds.Y, 3); Assert.Equal(404, block.ActualWidth, 3);
        Assert.Equal(56, FindBorder(page, "CssCollapsedMarginTile").VisualBounds.Y, 3);
        Assert.Same(root, first.VisualParent);
    }

    [Fact]
    public void LayoutPage_FloatsShapeInlineRowsAndClearTheFooter()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });
        var root = Assert.IsType<StackPanel>(page.FindName("CssFloatDemo"));
        var floating = FindBorder(page, "CssFloatTile");
        Assert.Equal(8, floating.VisualBounds.X, 3); Assert.Equal(96, floating.ActualHeight, 3);
        Assert.Equal(140, FindBorder(page, "CssFloatInlineTile").VisualBounds.X, 3);
        Assert.Equal(32, FindBorder(page, "CssFloatWrappedTile").VisualBounds.Y, 3);
        Assert.Equal(104, FindBorder(page, "CssClearTile").VisualBounds.Y, 3);
        Assert.Same(root, floating.VisualParent); Assert.Same(floating, root.Children[0]);
    }

    [Fact]
    public void LayoutPage_ContainerQueriesFollowNativeSliderBindings()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });
        var slider = Assert.IsType<Slider>(page.FindName("CssContainerWidthSlider"));
        var container = FindBorder(page, "CssContainerDemo");
        var first = FindBorder(page, "CssContainerFirstTile"); var second = FindBorder(page, "CssContainerSecondTile");
        var bar = FindBorder(page, "CssContainerUnitBar");
        Assert.Equal(420, container.ActualWidth, 3); Assert.Equal(192, first.ActualWidth, 3);
        Assert.Equal(204, second.VisualBounds.X, 3); Assert.Equal(39.6, bar.ActualWidth, 3);
        slider.Value = 300; page.UpdateLayout();
        Assert.Equal(300, container.ActualWidth, 3); Assert.Equal(276, first.ActualWidth, 3);
        Assert.Equal(60, second.VisualBounds.Y, 3); Assert.Equal(27.6, bar.ActualWidth, 3);
        Assert.NotNull(Jalium.UI.Data.BindingOperations.GetBindingExpression(container, FrameworkElement.WidthProperty));
        Assert.Same(page.FindName("CssContainerLayout"), first.VisualParent);
    }

    [Fact]
    public void LayoutPage_PinsAbsoluteChildrenWithoutDisturbingTheFlow()
    {
        var page = Layout(new CssLayoutPage { DataContext = new CssLayoutViewModel() });
        var pinned = FindBorder(page, "PinnedTile");

        Assert.Equal(120, pinned.ActualWidth, 3);
        Assert.Equal(44, pinned.ActualHeight, 3);

        // The host is 440 wide with right: 12px, so the tile starts at 440 − 12 − 120.
        Assert.Equal(308, pinned.VisualBounds.X, 3);
        Assert.Equal(12, pinned.VisualBounds.Y, 3);
    }

    [Fact]
    public void OutlinePage_AppliesTheOutlineWithoutTakingLayoutSpace()
    {
        var page = Layout(new CssOutlinePage { DataContext = new CssOutlineViewModel() });

        var solid = FindBorder(page, "SolidOutlineTile");
        Assert.Equal(2, solid.OutlineThickness);
        Assert.Equal(OutlineStyle.Solid, solid.OutlineStyle);
        Assert.Equal(Hex(0x38BDF8), ColorOf(solid.OutlineBrush));

        // The ring is painted outside the bounds: the tile keeps its declared size.
        Assert.Equal(150, solid.ActualWidth, 3);
        Assert.Equal(72, solid.ActualHeight, 3);

        Assert.Equal(OutlineStyle.Dashed, FindBorder(page, "DashedOutlineTile").OutlineStyle);
        var elliptical = FindBorder(page, "EllipticalOutlineTile");
        Assert.Equal(new CornerRadius(50, 20, 50, 20), elliptical.CornerRadius);
        Assert.Equal(3, elliptical.OutlineThickness);
        Assert.Equal(5, elliptical.OutlineOffset);
    }

    [Fact]
    public void MappingsPage_AppliesEveryCustomRegistrationKind()
    {
        var page = Layout(new CssMappingsPage { DataContext = new CssMappingsViewModel() });

        // Alias → the built-in longhand.
        Assert.Equal(new CornerRadius(14), FindBorder(page, "AliasTile").CornerRadius);

        // Custom converter bound to a fixed dependency property.
        Assert.IsType<DropShadowEffect>(FindBorder(page, "ElevationTile").Effect);

        // A converter returning null drops that declaration instead of throwing.
        Assert.Null(FindBorder(page, "RejectedElevationTile").Effect);

        // Adapter — one declaration, two assignments.
        var square = FindBorder(page, "SquareTile");
        Assert.Equal(90, square.Width);
        Assert.Equal(90, square.Height);

        // Target resolved by dependency-property name, with the engine's own color parsing.
        var tint = FindBorder(page, "TintTile");
        var tintColor = ColorOf(tint.Background);
        Assert.Equal(0x59, tintColor.A);
        Assert.Equal(0xF4, tintColor.R);
    }

    [Fact]
    public void MappingsPage_SharesOneConverterAcrossTwoRegistrations()
    {
        var page = Layout(new CssMappingsPage { DataContext = new CssMappingsViewModel() });

        // Same converter instance, same declared color — only the alpha passed at
        // registration differs (0.35 vs 0.85).
        var weak = ColorOf(FindBorder(page, "TintWeakTile").Background);
        var strong = ColorOf(FindBorder(page, "TintStrongTile").Background);

        Assert.Equal(0x59, weak.A);
        Assert.Equal(0xD9, strong.A);
        Assert.Equal(weak.R, strong.R);
        Assert.Equal(weak.G, strong.G);
        Assert.Equal(weak.B, strong.B);
    }

    [Fact]
    public void MappingsPage_LetsStackPanelInterceptFlexDirection()
    {
        var page = Layout(new CssMappingsPage { DataContext = new CssMappingsViewModel() });

        // StackPanel has no notion of flex-direction; its TryApplyCssPropertyCore override
        // maps the keyword onto Orientation.
        var row = Assert.IsType<StackPanel>(page.FindName("InterceptedRow"));
        var column = Assert.IsType<StackPanel>(page.FindName("InterceptedColumn"));

        Assert.Equal(Orientation.Horizontal, row.Orientation);
        Assert.Equal(Orientation.Vertical, column.Orientation);
    }

    [Fact]
    public void FlexPanelPage_GrowsTheFlexibleItemAndHonoursTheCssShorthand()
    {
        var page = Layout(new FlexPanelPage { DataContext = new FlexPanelViewModel() });

        // Three items with Basis 90; only the middle one grows, so it takes all the slack.
        var row = Assert.IsType<FlexPanel>(page.FindName("GrowRow"));
        var grown = FindBorder(page, "GrowItem");
        Assert.True(
            grown.ActualWidth > 200,
            $"The Grow item should absorb the free space but measured {grown.ActualWidth} " +
            $"inside a {row.ActualWidth}-wide panel.");

        // flex: 1 from the style sheet maps to Basis 0 + Grow 1 and does the same job.
        var cssFlex = Assert.IsType<FlexPanel>(page.FindName("CssDrivenFlex"));
        var main = FindBorder(page, "CssFlexMain");
        Assert.Equal(FlexDirection.Row, cssFlex.Direction);
        Assert.Equal(FlexJustify.SpaceBetween, cssFlex.JustifyContent);
        Assert.True(
            main.ActualWidth > 200,
            $"flex: 1 should take the remaining space but measured {main.ActualWidth}.");
    }
}
