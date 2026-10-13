using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

// Content for the CSS styling pages. The pages themselves are pure markup; anything
// tabular lives here as plain data and is rendered through an ItemsControl.

public sealed partial class CssStylingViewModel
{
    public IReadOnlyList<CssReferenceRow> Mappings { get; } = new[]
    {
        new CssReferenceRow(
            "margin, padding",
            "Thickness",
            "CSS writes top/right/bottom/left; Thickness stores left/top/right/bottom, so the order is rotated for you."),
        new CssReferenceRow(
            "background-color, color",
            "Background, Foreground",
            "Colors wrap into a SolidColorBrush. #RRGGBBAA puts alpha last, per CSS."),
        new CssReferenceRow(
            "background: linear-gradient(…)",
            "LinearGradientBrush",
            "CSS angles (0deg points up, growing clockwise) convert to StartPoint/EndPoint."),
        new CssReferenceRow(
            "border: 1px solid red",
            "BorderThickness + BorderBrush",
            "One declaration feeds two properties; border-style: none clears the brush."),
        new CssReferenceRow(
            "border-radius",
            "CornerRadius",
            "Percentages and slash syntax retain independent X/Y radii; native ImageBrush backgrounds follow the same contour."),
        new CssReferenceRow(
            "box-shadow: 3px 4px 5px …",
            "DropShadowEffect",
            "Cartesian offsets become polar Direction/ShadowDepth; inset gives an InnerShadowEffect; several shadows become an EffectGroup."),
        new CssReferenceRow(
            "cursor: pointer",
            "Cursors.Hand",
            "A full keyword table: not-allowed → No, move → SizeAll, col-resize → SizeWE, and so on."),
        new CssReferenceRow(
            "display: none, visibility: hidden",
            "Visibility",
            "Collapsed and Hidden respectively. Other display values stay visible; use FlexPanel for display: flex."),
        new CssReferenceRow(
            "transform: rotate(…) scale(…)",
            "RenderTransform",
            "A TransformGroup that does not affect layout — exactly the CSS behaviour."),
        new CssReferenceRow(
            "transition",
            "TransitionProperty, TransitionDuration",
            "One shared duration and curve per element; pseudo-class flips animate automatically."),
        new CssReferenceRow(
            "z-index, grid-row, grid-column, gap",
            "Panel.ZIndex, Grid.Row/Column, panel spacing",
            "Grid lines convert from CSS's 1-based numbering to the framework's 0-based one."),
        new CssReferenceRow(
            "outline, outline-offset",
            "OutlineBrush, OutlineThickness, OutlineStyle, OutlineOffset",
            "A self-drawn ring outside the bounds that takes no layout space."),
        new CssReferenceRow(
            "width: 50%, aspect-ratio, box-sizing",
            "resolved during layout",
            "Percentages resolve against the containing block rather than materialising as a fixed value."),
        new CssReferenceRow(
            "is-tab-stop, horizontal-content-alignment",
            "any dependency property",
            "The kebab-case fallback channel: values go through the XAML converter stack."),
    };

    public override string XamlExample => """
        <!-- Channel 1: an inline declaration block on a single element -->
        <Border Css.Style="background-color: #0078D4; border-radius: 8px;
                           padding: 12px 16px; box-shadow: 0 2px 8px rgba(0,0,0,.35)">
            <TextBlock Text="Styled by CSS" Foreground="White" />
        </Border>

        <!-- Channel 2: a style sheet scoped to this subtree, declared in markup -->
        <StackPanel Css.StyleSheet="
                Border { background-color: #334155; border-radius: 8px; padding: 12px }
                Border TextBlock { color: #e2e8f0; font-size: 13px }
                .accent { background-color: #2563eb }
                #hero { background-color: #0f766e; border-radius: 16px }">

            <Border x:Name="hero"><TextBlock Text="Matched by #hero" /></Border>
            <Border><TextBlock Text="Matched by the Border type selector" /></Border>
            <Border Css.Class="accent"><TextBlock Text="Matched by .accent" /></Border>
        </StackPanel>
        """;

    public override string CSharpExample => """"
        using Jalium.UI.Styling;

        // Inline declarations from code.
        Css.SetStyle(border, "background-color: #0078D4; border-radius: 8px; padding: 12px 16px");

        // Classes drive class selectors.
        Css.SetClass(panel, "card elevated");

        // A sheet scoped to one subtree.
        Css.GetStyleSheets(panel).Add(CssStyleSheet.Parse("""
            TextBlock { color: #94a3b8 }
            .card { background-color: #1e293b; border-radius: 12px }
            """));

        // A sheet for the whole application — parse once, share everywhere.
        Application.Current.StyleSheets.Add(CssStyleSheet.Parse("""
            Button { border-radius: 6px; transition: background-color 0.15s ease-out }
            Button:hover { background-color: #2563eb }
            """));

        // Or load one from a pack resource.
        Application.Current.StyleSheets.Add(
            CssStyleSheet.FromUri(new Uri("/MyApp;component/styles/app.css", UriKind.Relative)));

        // Parsing never throws; malformed rules are reported instead.
        var sheet = CssStyleSheet.Parse(userSuppliedCss);
        foreach (var diagnostic in sheet.Diagnostics)
        {
            Console.WriteLine(diagnostic);   // Warning (line 4): unknown pseudo-class ':nth-child'
        }
        """";
}

public sealed partial class CssSelectorsViewModel
{
    public override string XamlExample => """
        <StackPanel Css.StyleSheet="
                /* Type, class, id, universal */
                Border      { background-color: #334155; border-radius: 6px; padding: 10px 14px }
                .tinted     { background-color: #4338ca }
                #named      { background-color: #0f766e }
                *           { margin: 0 0 8px 0 }

                /* Combinators: descendant vs direct child */
                .outer TextBlock   { color: #fbbf24 }
                .outer > TextBlock { color: #34d399; font-weight: 600 }

                /* Dynamic pseudo-classes drive interaction */
                Button              { background-color: #1e293b; color: #e2e8f0;
                                      transition: background-color 0.15s ease-out }
                Button:hover        { background-color: #2563eb }
                Button:active       { background-color: #1d4ed8 }
                Button:focus-visible { outline: 2px solid #f472b6; outline-offset: 3px }
                Button:disabled     { color: #64748b }
                CheckBox:checked    { color: #34d399; font-weight: 600 }

                /* !important survives a more specific normal rule */
                Border  { background-color: #be123c !important }">

            <Border Css.Class="tinted"><TextBlock Text="A" /></Border>
            <Border x:Name="named"><TextBlock Text="B" /></Border>
            <Button Content="Hover, press, or tab to me" />
            <CheckBox Content="Toggle me" />
        </StackPanel>
        """;

    public override string CSharpExample => """"
        using Jalium.UI.Styling;

        // Classes are a plain space-separated list; order does not matter.
        Css.SetClass(card, "card elevated");

        var sheet = CssStyleSheet.Parse("""
            .card            { background-color: #1e293b; border-radius: 12px; padding: 16px }
            .card.elevated   { box-shadow: 0 8px 24px rgba(0,0,0,.45) }
            .card:hover      { background-color: #243044 }
            .card > TextBlock { color: #e2e8f0 }
            """);

        Css.GetStyleSheets(page).Add(sheet);

        // Rules using an unsupported construct are dropped rather than silently mismatching.
        var broken = CssStyleSheet.Parse(".card > > .title { color: red }");
        foreach (var diagnostic in broken.Diagnostics)
        {
            Console.WriteLine(diagnostic);
        }

        // The cascade decides; the property system records where the value came from.
        // CSS lands above style setters and below a local value, so this still wins:
        card.Background = new SolidColorBrush(Color.FromRgb(0x66, 0x33, 0x99));
        """";
}

public sealed partial class CssLayoutViewModel
{
    public override string XamlExample => """
        <!-- Percentages resolve against the containing block, every layout pass -->
        <Border Width="420" Height="120">
            <StackPanel>
                <Border Css.Style="width: 100%; height: 25%; background-color: #0ea5e9" />
                <Border Css.Style="width: 50%;  height: 25%; background-color: #a855f7" />
            </StackPanel>
        </Border>

        <!-- Every margin and padding edge resolves against the block's width, per CSS -->
        <Border Css.Style="margin: 10%; padding: 12% 8%" />

        <!-- One axis given, the other derived -->
        <Border Css.Style="width: 160px; aspect-ratio: 16/9" />

        <!-- content-box adds padding and border on top of the declared width -->
        <Border Css.Style="box-sizing: content-box; width: 200px; padding: 20px; border: 4px solid #f472b6" />

        <!-- Out of flow: contributes nothing to the panel, placed by the insets -->
        <StackPanel>
            <Border Css.Style="position: absolute; top: 12px; right: 12px; width: 120px; height: 44px" />
            <Border Css.Style="position: absolute; left: 12px; right: 12px; bottom: 12px" />
            <Border Css.Style="position: absolute; left: 50%; top: 50%" />
        </StackPanel>
        """;

    public override string CSharpExample => """
        // Any panel can host absolutely positioned children: skip them in the flow loops
        // and hand them to the two helpers. That is the whole protocol.
        public sealed class DiagonalPanel : Panel
        {
            protected override Size MeasureOverride(Size availableSize)
            {
                var width = 0.0;
                var height = 0.0;

                foreach (UIElement child in Children)
                {
                    if (IsCssAbsolute(child))
                    {
                        continue;               // out of flow — not part of the desired size
                    }

                    child.Measure(availableSize);
                    width = Math.Max(width, child.DesiredSize.Width);
                    height += child.DesiredSize.Height;
                }

                MeasureCssAbsoluteChildren(availableSize);
                return new Size(width, height);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                var offset = 0.0;
                foreach (UIElement child in Children)
                {
                    if (IsCssAbsolute(child))
                    {
                        continue;
                    }

                    child.Arrange(new Rect(offset, offset, child.DesiredSize.Width, child.DesiredSize.Height));
                    offset += 12;
                }

                ArrangeCssAbsoluteChildren(finalSize);
                return finalSize;
            }
        }
        """;
}

public sealed partial class FlexPanelViewModel
{
    public override string XamlExample => """
        <!-- As a normal panel: container properties plus attached item properties -->
        <FlexPanel Direction="Row"
                   Wrap="Wrap"
                   JustifyContent="SpaceBetween"
                   AlignItems="Center"
                   ColumnSpacing="10"
                   RowSpacing="10">

            <Border FlexPanel.Basis="120" />
            <Border FlexPanel.Basis="0" FlexPanel.Grow="1" />
            <Border FlexPanel.Basis="90" FlexPanel.Shrink="0" />
            <Border FlexPanel.AlignSelf="FlexEnd" />
            <Border FlexPanel.Order="-1" />
        </FlexPanel>

        <!-- Or drive the whole thing from CSS — the flex family maps onto the same panel -->
        <FlexPanel Css.StyleSheet="
                FlexPanel { flex-direction: row; justify-content: space-between;
                            align-items: center; gap: 10px }
                .sidebar { flex: 0 0 120px }
                .main    { flex: 1 }
                .aside   { flex: 0 1 90px }">
            <Border Css.Class="sidebar" />
            <Border Css.Class="main" />
            <Border Css.Class="aside" />
        </FlexPanel>
        """;

    public override string CSharpExample => """
        var panel = new FlexPanel
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceBetween,
            AlignItems = FlexAlign.Center,
            ColumnSpacing = 10,
            RowSpacing = 10,
        };

        var sidebar = new Border();
        FlexPanel.SetBasis(sidebar, 120);
        FlexPanel.SetShrink(sidebar, 0);

        var main = new Border();
        FlexPanel.SetBasis(main, 0);      // flex: 1 — start from nothing, take all the slack
        FlexPanel.SetGrow(main, 1);

        var badge = new Border();
        FlexPanel.SetAlignSelf(badge, FlexAlign.FlexEnd);
        FlexPanel.SetOrder(badge, -1);    // laid out first; the Children order is untouched

        panel.Children.Add(sidebar);
        panel.Children.Add(main);
        panel.Children.Add(badge);

        // Use Basis, not Width, for anything that should flex: an explicit Width is a hard
        // constraint the arrange pass enforces, so it survives Grow untouched.
        """;
}

public sealed partial class CssOutlineViewModel
{
    public override string XamlExample => """
        <!-- Through CSS: the shorthand always resets width, style, and color -->
        <Border Css.Style="outline: 2px solid #38bdf8; outline-offset: 3px" />
        <Border Css.Style="outline: 2px dashed #facc15" />
        <Border Css.Style="outline: none" />
        <Border Css.Style="border-radius:50px 20px / 12px 30px;
                           outline:3px solid #5eead4; outline-offset:5px" />

        <!-- The focus ring, in two lines -->
        <StackPanel Css.StyleSheet="
                Button:focus-visible { outline: 3px solid #38bdf8; outline-offset: 3px }">
            <Button Content="Tab to me" />
        </StackPanel>

        <!-- Or the properties directly — they bind, animate, and work in styles -->
        <Border OutlineBrush="#c4b5fd"
                OutlineThickness="3"
                OutlineStyle="Dashed"
                OutlineOffset="4" />
        """;

    public override string CSharpExample => """
        // A validation halo that never disturbs the layout around the field.
        void ShowValidationState(FrameworkElement field, bool isValid)
        {
            if (isValid)
            {
                field.OutlineBrush = null;         // or OutlineStyle = OutlineStyle.None
                return;
            }

            field.OutlineBrush = new SolidColorBrush(Color.FromRgb(0xF4, 0x3F, 0x5E));
            field.OutlineThickness = 2;
            field.OutlineOffset = 2;
            field.OutlineStyle = OutlineStyle.Solid;
        }

        // Because the ring is painted outside the bounds and after the element's own effects,
        // none of this triggers a measure or arrange pass — only a repaint.
        """;
}

public sealed partial class CssMappingsViewModel
{
    public CssMappingsViewModel() => GalleryCssExtensions.EnsureRegistered();

    public override string XamlExample => """
        <!-- brand-corner is an alias of border-radius -->
        <Border Css.Style="brand-corner: 14px" />

        <!-- gallery-elevation: 1..5 builds a matching drop shadow -->
        <Border Css.Style="gallery-elevation: 3" />

        <!-- gallery-tint takes any CSS color and paints it back at 35% alpha -->
        <Border Css.Style="gallery-tint: rebeccapurple" />

        <!-- gallery-square writes Width and Height from one length -->
        <Border Css.Style="gallery-square: 90px" />

        <!-- No registration at all: kebab-case reaches any dependency property -->
        <TextBlock Css.Style="horizontal-alignment: Right; text-wrapping: Wrap; is-tab-stop: false" />
        """;

    public override string CSharpExample => """"
        using Jalium.UI.Styling;

        // 1. An alias: one name redirected to another, resolved at compile time.
        CssMappings.RegisterAlias("brand-corner", "border-radius");

        // 2. A custom name bound to a fixed dependency property, with your own converter.
        //    Returning null drops the declaration — the CSS error model, not an exception.
        CssMappings.RegisterProperty("gallery-elevation", UIElement.EffectProperty, static raw =>
        {
            if (!int.TryParse(raw, out var level) || level is < 1 or > 5)
            {
                return null;
            }

            var shadow = new DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 270,
                ShadowDepth = level * 2,
                BlurRadius = level * 6,
                Opacity = 0.18 + (level * 0.06),
            };

            // cacheByValue defaults to true: one conversion per distinct declaration text,
            // shared by every matched element — so return something frozen.
            shadow.Freeze();
            return shadow;
        });

        // 3. Target resolved by name against each element's runtime type.
        CssMappings.RegisterProperty("gallery-tint", "Background", static raw =>
        {
            if (!CssValueParsing.TryParseColor(raw, out var color))
            {
                return null;
            }

            var brush = new SolidColorBrush(Color.FromArgb(0x59, color.R, color.G, color.B));
            brush.Freeze();
            return brush;
        });

        // 4. An adapter sees the element and can assign several properties at once.
        public sealed class SquareAdapter : CssPropertyAdapter
        {
            public override bool TryConvert(
                FrameworkElement element, string rawValue, IList<CssPropertyAssignment> assignments)
            {
                if (!CssValueParsing.TryParseLength(rawValue, out var side) || side <= 0)
                {
                    return false;
                }

                assignments.Add(new CssPropertyAssignment(FrameworkElement.WidthProperty, side));
                assignments.Add(new CssPropertyAssignment(FrameworkElement.HeightProperty, side));
                return true;
            }
        }

        CssMappings.RegisterProperty("gallery-square", new SquareAdapter());

        // A panel can claim a property for itself instead of registering globally.
        protected internal override bool TryApplyCssPropertyCore(
            string propertyName, string rawValue, in CssDeclarationSetter setter)
        {
            if (propertyName != "flex-direction")
            {
                return false;
            }

            switch (rawValue)
            {
                case "row":    setter.Set(OrientationProperty, Orientation.Horizontal); return true;
                case "column": setter.Set(OrientationProperty, Orientation.Vertical);   return true;
                default:       return false;
            }
        }
        """";
}

