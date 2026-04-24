using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;
using Jalium.UI.Media;
using Path = Jalium.UI.Controls.Shapes.Path;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Event args for navigation requests from HomePage.
/// </summary>
public class NavigationRequestEventArgs : EventArgs
{
    public string PageTag { get; }

    public NavigationRequestEventArgs(string pageTag)
    {
        PageTag = pageTag;
    }
}

public partial class HomePage : Page
{
    /// <summary>
    /// Occurs when a category card is clicked and navigation is requested.
    /// </summary>
    public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

    private const string XamlExample = @"<!-- Jalium.UI Application Entry Point -->
<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
      x:Class=""MyApp.Views.MainPage""
      Title=""Welcome"">

    <StackPanel Orientation=""Vertical"">
        <TextBlock Text=""Welcome to My App""
                   FontSize=""32""
                   Foreground=""#FAFAFA""
                   Margin=""0,0,0,8""/>

        <TextBlock Text=""Built with Jalium.UI""
                   FontSize=""14""
                   Foreground=""#A1A1AA""
                   Margin=""0,0,0,24""/>

        <Border Background=""#1B182B""
                BorderBrush=""#2B2742""
                BorderThickness=""1""
                CornerRadius=""14""
                Padding=""20"">
            <StackPanel Orientation=""Vertical"">
                <TextBlock Text=""Feature""
                           FontSize=""16""
                           Foreground=""#FAFAFA""/>
                <TextBlock Text=""Description here.""
                           FontSize=""12""
                           Foreground=""#A1A1AA""/>
            </StackPanel>
        </Border>
    </StackPanel>
</Page>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        BuildContent();
    }

    private void BuildContent()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(new TextBlock
        {
            Text = ""Welcome"",
            FontSize = 32,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA))
        });

        Content = stack;
    }
}";

    private EditControl? _xamlEditor;
    private EditControl? _csharpEditor;

    public HomePage()
    {
        InitializeComponent();
        BuildContent();
        LoadCodeExamples();
    }

    private void LoadCodeExamples()
    {
        if (_xamlEditor != null)
        {
            _xamlEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            _xamlEditor.LoadText(XamlExample);
        }
        if (_csharpEditor != null)
        {
            _csharpEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            _csharpEditor.LoadText(CSharpExample);
        }
    }

    private void BuildContent()
    {
        var root = new StackPanel { Orientation = Orientation.Vertical };

        root.Children.Add(CreateHero());

        // Section: Framework Features
        root.Children.Add(CreateSectionHeader("Framework features", "Built for modern Windows apps"));

        var featureGrid = new UniformGrid
        {
            Columns = 4,
            Margin = new Thickness(0, 0, 0, 32)
        };
        featureGrid.Children.Add(CreateFeatureCard(
            BuildApiIcon(GalleryTheme.AccentPrimary),
            "WPF-like API",
            "Familiar DependencyProperties, routed events, and XAML authoring.",
            GalleryTheme.AccentPrimary));
        featureGrid.Children.Add(CreateFeatureCard(
            BuildRenderingIcon(GalleryTheme.Info),
            "D3D12 rendering",
            "Native GPU acceleration for smooth 60+ FPS interfaces.",
            GalleryTheme.Info));
        featureGrid.Children.Add(CreateFeatureCard(
            BuildBindingIcon(GalleryTheme.HaloPurple),
            "Rich data binding",
            "Full MVVM with {Binding}, converters, and INotifyPropertyChanged.",
            GalleryTheme.HaloPurple));
        featureGrid.Children.Add(CreateFeatureCard(
            BuildFocusIcon(GalleryTheme.Warning),
            "Complete focus",
            "Tab traversal, focus scopes, and access keys out of the box.",
            GalleryTheme.Warning));
        root.Children.Add(featureGrid);

        // Section: Explore Controls
        root.Children.Add(CreateSectionHeader("Explore controls", "Jump into a category to see the components live"));

        var categories = new UniformGrid
        {
            Columns = 3,
            Margin = new Thickness(0, 0, 0, 32)
        };
        categories.Children.Add(CreateCategoryCard("Basic input", "Button, CheckBox, Slider, TextBox", "basic", GalleryTheme.AccentPrimary));
        categories.Children.Add(CreateCategoryCard("Text", "TextBlock, Markdown, RichTextBox", "text", GalleryTheme.Info));
        categories.Children.Add(CreateCategoryCard("Layout", "Grid, StackPanel, Dock, Wrap", "layout", GalleryTheme.HaloPurple));
        categories.Children.Add(CreateCategoryCard("Navigation", "TabControl, Menu, ToolBar", "navigation", GalleryTheme.Warning));
        categories.Children.Add(CreateCategoryCard("Media", "Image, WebView, MediaElement", "media", GalleryTheme.Success));
        categories.Children.Add(CreateCategoryCard("Collections", "ListBox, TreeView, DataGrid", "collections", GalleryTheme.Error));
        categories.Children.Add(CreateCategoryCard("Charts", "Line, Bar, Pie, Sankey, Gantt", "charts", GalleryTheme.AccentSecondary));
        categories.Children.Add(CreateCategoryCard("Maps", "MapView, MiniMap, Heatmap", "maps", GalleryTheme.Info));
        categories.Children.Add(CreateCategoryCard("Effects", "Backdrop, shader, transitions", "effects", GalleryTheme.HaloPurple));
        root.Children.Add(categories);

        // Section: Code examples
        root.Children.Add(CreateSectionHeader("Copy & paste", "Real snippets to kick-start your first page"));

        _xamlEditor = new EditControl
        {
            Height = 240,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsReadOnly = true,
            ShowLineNumbers = true,
            FontSize = 13
        };
        _csharpEditor = new EditControl
        {
            Height = 240,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsReadOnly = true,
            ShowLineNumbers = true,
            FontSize = 13
        };

        root.Children.Add(CreateCodeCard("JALXAML markup", "Declarative page in ~25 lines", _xamlEditor, true));
        root.Children.Add(CreateCodeCard("C# code-behind", "Fully equivalent imperative API", _csharpEditor, false));

        Content = root;
    }

    private Border CreateHero()
    {
        var hero = new Border
        {
            Background = GalleryTheme.HeroGradientBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusXLarge),
            Padding = new Thickness(36, 32, 36, 32),
            Margin = new Thickness(0, 0, 0, 28)
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(280) });

        // Left column: copy + CTA
        var copy = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };

        copy.Children.Add(new Border
        {
            Background = GalleryTheme.AccentSoftBrush,
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x55, 0xFF, 0xD6, 0x0A)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(999),
            Padding = new Thickness(12, 6, 12, 6),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 16),
            Child = new TextBlock
            {
                Text = "\u2728  Jalium.UI Gallery",
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = GalleryTheme.AccentPrimaryBrush
            }
        });

        copy.Children.Add(new TextBlock
        {
            Text = "Build native Windows experiences",
            FontSize = GalleryTheme.FontSizeHero,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 10)
        });

        copy.Children.Add(new TextBlock
        {
            Text = "A modern control library with WPF-like ergonomics, a GPU-accelerated renderer, and a gallery full of live, copy-pasteable demos.",
            FontSize = 15,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 560,
            Margin = new Thickness(0, 0, 0, 24)
        });

        var ctaRow = new StackPanel { Orientation = Orientation.Horizontal };

        var primaryCta = new Button
        {
            Content = "Get started  \u2192",
            Background = GalleryTheme.AccentPrimaryBrush,
            Foreground = new SolidColorBrush(Color.FromRgb(0x0B, 0x08, 0x14)),
            BorderThickness = new Thickness(0),
            FontWeight = FontWeights.Bold,
            FontSize = 14,
            Height = 44,
            Padding = new Thickness(20, 0, 20, 0),
            Margin = new Thickness(0, 0, 12, 0)
        };
        primaryCta.Click += (_, _) => NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("getting-started"));
        ctaRow.Children.Add(primaryCta);

        var ghostCta = new Button
        {
            Content = "Browse controls",
            Background = GalleryTheme.TransparentBrush,
            Foreground = GalleryTheme.TextPrimaryBrush,
            BorderBrush = GalleryTheme.BorderStrongBrush,
            BorderThickness = new Thickness(1),
            FontWeight = FontWeights.SemiBold,
            FontSize = 14,
            Height = 44,
            Padding = new Thickness(20, 0, 20, 0)
        };
        ghostCta.Click += (_, _) => NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("basic"));
        ctaRow.Children.Add(ghostCta);

        copy.Children.Add(ctaRow);

        Grid.SetColumn(copy, 0);
        grid.Children.Add(copy);

        // Right column: decorative preview
        var preview = BuildHeroPreview();
        Grid.SetColumn(preview, 1);
        grid.Children.Add(preview);

        hero.Child = grid;
        return hero;
    }

    private static Border BuildHeroPreview()
    {
        var outer = new Border
        {
            Width = 250,
            Height = 260,
            Background = new SolidColorBrush(Color.FromArgb(0x88, 0x1B, 0x18, 0x2B)),
            BorderBrush = GalleryTheme.BorderStrongBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(18),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Mock window titlebar
        var dotsRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 14) };
        dotsRow.Children.Add(MakeDot(Color.FromRgb(0xEF, 0x44, 0x44)));
        dotsRow.Children.Add(MakeDot(Color.FromRgb(0xF5, 0x9E, 0x0B)));
        dotsRow.Children.Add(MakeDot(Color.FromRgb(0x22, 0xC5, 0x5E)));
        stack.Children.Add(dotsRow);

        // Mock content blocks
        stack.Children.Add(MakeBar(160, GalleryTheme.BackgroundHover));
        stack.Children.Add(MakeBar(210, GalleryTheme.BorderDefault, 6));
        stack.Children.Add(MakeBar(190, GalleryTheme.BorderDefault, 6));

        var chartRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 14, 0, 0)
        };
        chartRow.Children.Add(MakeChartBar(24, GalleryTheme.BorderStrong));
        chartRow.Children.Add(MakeChartBar(42, GalleryTheme.HaloPurple));
        chartRow.Children.Add(MakeChartBar(70, GalleryTheme.AccentPrimary));
        chartRow.Children.Add(MakeChartBar(58, GalleryTheme.AccentSecondary));
        chartRow.Children.Add(MakeChartBar(36, GalleryTheme.BorderStrong));
        stack.Children.Add(chartRow);

        // Bottom label
        stack.Children.Add(new Border
        {
            Background = GalleryTheme.AccentPrimaryBrush,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(10, 6, 10, 6),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 16, 0, 0),
            Child = new TextBlock
            {
                Text = "Live",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0x0B, 0x08, 0x14))
            }
        });

        outer.Child = stack;
        return outer;
    }

    private static Border MakeDot(Color color)
    {
        return new Border
        {
            Width = 10,
            Height = 10,
            Background = new SolidColorBrush(color),
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(0, 0, 6, 0)
        };
    }

    private static Border MakeBar(double width, Color color, double topMargin = 0)
    {
        return new Border
        {
            Width = width,
            Height = 10,
            Background = new SolidColorBrush(color),
            CornerRadius = new CornerRadius(5),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, topMargin, 0, 0)
        };
    }

    private static Border MakeChartBar(double height, Color color)
    {
        return new Border
        {
            Width = 24,
            Height = height,
            Background = new SolidColorBrush(color),
            CornerRadius = new CornerRadius(6),
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 8, 0)
        };
    }

    private StackPanel CreateSectionHeader(string title, string subtitle)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(4, 0, 0, 16)
        };

        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = GalleryTheme.FontSizeTitle,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4)
        });

        stack.Children.Add(new TextBlock
        {
            Text = subtitle,
            FontSize = 13,
            Foreground = GalleryTheme.TextTertiaryBrush
        });

        return stack;
    }

    #region Feature icons (SVG paths)

    // Viewbox 1024×1024 — two chevron-arrow paths (upper + lower) representing an exchange / API.
    private const string ApiIconPath =
        "M310.345143 292.571429H877.714286a73.142857 73.142857 0 0 1 0 146.285714" +
        "H146.285714a73.142857 73.142857 0 0 1-72.850285-79.433143 72.996571 72.996571 0 0 1 21.138285-58.002286" +
        "L301.348571 94.573714a73.142857 73.142857 0 0 1 103.497143 103.424L310.345143 292.571429z" +
        "M713.654857 731.428571H146.285714a73.142857 73.142857 0 0 1 0-146.285714h731.428572" +
        "a73.142857 73.142857 0 0 1 72.850285 79.433143 72.996571 72.996571 0 0 1-21.138285 58.002286" +
        "L722.651429 929.426286a73.142857 73.142857 0 0 1-103.497143-103.424L713.654857 731.428571z";

    // Viewbox 1287×1024 — interlocked panels representing data binding.
    private const string BindingIconPath =
        "M300.44123 597.333333H85.641953v-513.080168h686.678722v513.080168h-129.118746" +
        "a43.322483 43.322483 0 1 0 0 86.60639h129.118746a85.487643 85.487643 0 0 0 85.641953-85.333334" +
        "v-513.080168A85.487643 85.487643 0 0 0 772.320675 0.192887H85.641953" +
        "A85.487643 85.487643 0 0 0 0 85.526221v513.080168a85.487643 85.487643 0 0 0 85.641953 85.333334" +
        "h214.799277a43.013864 43.013864 0 0 0 43.476793-43.283906 43.862568 43.862568 0 0 0-43.476793-43.322484z" +
        "m901.362266-257.273056h-214.799277a43.322483 43.322483 0 1 0 0 86.60639h214.799277v513.080168" +
        "h-686.678722v-513.080168H644.24352a43.322483 43.322483 0 1 0 0-86.60639h-129.118746" +
        "a85.487643 85.487643 0 0 0-85.641953 85.333334v513.080168a85.487643 85.487643 0 0 0 85.641953 85.333334" +
        "h686.678722a85.487643 85.487643 0 0 0 85.680531-85.333334v-513.080168" +
        "a85.487643 85.487643 0 0 0-85.680531-85.333334z";

    // Viewbox 1024×1024 — four L-shaped brackets (top-left, top-right, bottom-left,
    // bottom-right) suggesting a focus reticle. Each bracket is an EvenOdd ring
    // (outer contour + inner contour) painted as a single closed figure.
    //
    // NOTE on F1 (left-top bracket): the original iconfont data for this figure
    // was malformed — a `v-101.04` followed by an out-of-range `C` produced a
    // cubic that overshot to y≈30 and dragged the overall geometry bounds
    // into a 236×316 non-square rectangle. Uniform stretch then compressed all
    // four brackets off-grid. The replacement below is the Y-axis mirror of
    // F2 (the left-bottom bracket, reflected about y=512), which is
    // guaranteed consistent with F0/F2/F3 in curvature, arm length and
    // ring thickness.
    private const string FocusIconPath =
        // F0 — right-bottom bracket (original, correct)
        "M849.07153297 646.81872559c9.30432153 0 17.26391602 3.30249 23.82934617 9.88769507" +
        " 6.60992408 6.59509253 9.88769508 14.52502465 9.88769508 23.79473901v101.14617896" +
        " c0 27.90801978-9.87780761 51.70275879-29.61364722 71.47814965-19.75067115 19.77539086-43.56518578 29.66308594-71.48803711 29.66308594" +
        " h-101.1165166c-9.32409644 0-17.25402856-3.29754663-23.83428954-9.9865725" +
        "-6.59509253-6.49127173-9.90252662-14.52502465-9.90252662-23.7947383 0-9.26971435 3.30743408-17.20458984 9.90252662-23.79473901" +
        " 6.58026099-6.59014916 14.51019311-9.88769508 23.83428954-9.88769507h101.1165166" +
        " c9.29937744 0 17.26391602-3.29754663 23.82440137-9.88769579 6.61486816-6.59014916 9.88769508-14.52008057 9.88769579-23.89361573" +
        " v-101.04235815c0-9.36859107 3.28765845-17.30346656 9.89758254-23.78979493" +
        " 6.57531762-6.69396997 14.52502465-9.99151587 23.83923363-9.99151587l-0.06427025 0.09887671z" +
        // F1 — left-top bracket (repaired: Y-mirror of F2 about y=512)
        "M174.9877932 377.18127441c9.30432153 0 17.24414039-3.30249 23.81451393-9.88769507" +
        " 6.62475562-6.59509253 9.90252662-14.52502465 9.90252662-23.79473901v-101.14617896" +
        "c0-9.26971435 3.27282691-17.19964576 9.89758324-23.78979492 6.57531762-6.59014916 14.51513648-9.88769508 23.81451393-9.88769579" +
        "h101.12640404c9.29937744 0 17.25402856-3.29754663 23.82934547-9.88769507 6.60992408-6.59014916 9.88769508-14.52502465 9.88769579-23.89361572" +
        " 0-9.26971435-3.27777099-17.20458984-9.88769579-23.79473901-6.57531762-6.59014916-14.52996803-9.88769508-23.82934547-9.88769508" +
        "H242.41693092c-27.91296386 0-51.71264625 9.88769508-71.47814895 29.66308594-19.75561523 19.67651344-29.62353539 43.57012915-29.62353539 71.47814965" +
        "v101.04235816c0 9.26971435 3.27282691 17.30346656 9.88769507 23.89361573 6.58026099 6.59509253 14.52502465 9.88769508 23.81451464 9.88769507z" +
        // F2 — left-bottom bracket (original, correct)
        "M174.9877932 646.81872559c9.30432153 0 17.24414039 3.30249 23.81451393 9.88769507 6.62475562 6.59509253 9.90252662 14.52502465 9.90252662 23.79473901" +
        "v101.14617896c0 9.26971435 3.27282691 17.19964576 9.89758324 23.78979492 6.57531762 6.59014916 14.51513648 9.88769508 23.81451393 9.88769579" +
        "h101.12640404c9.29937744 0 17.25402856 3.29754663 23.82934547 9.88769507 6.60992408 6.59014916 9.88769508 14.52502465 9.88769579 23.89361572" +
        " 0 9.26971435-3.27777099 17.20458984-9.88769579 23.79473901-6.57531762 6.59014916-14.52996803 9.88769508-23.82934547 9.88769508" +
        "H242.41693092c-27.91296386 0-51.71264625-9.88769508-71.47814895-29.66308594-19.75561523-19.67651344-29.62353539-43.57012915-29.62353539-71.47814965" +
        "v-101.04235816c0-9.26971435 3.27282691-17.30346656 9.88769507-23.89361573 6.58026099-6.59509253 14.52502465-9.88769508 23.81451464-9.88769507z" +
        // F3 — right-top bracket (original, correct)
        "M680.57037329 141.3103025h101.1165166c27.92285133 0 51.73736596 9.88769508 71.48803711 29.56420922 19.73583961 19.77539086 29.61364722 43.57012915 29.61364722 71.47814965" +
        "v101.14617896c0 9.26971435-3.27777099 17.30346656-9.88769508 23.78979493-6.56542945 6.69396997-14.52502465 9.88769508-23.82934617 9.88769506" +
        "-9.29937744 0-17.26391602-3.19372583-23.82440139-9.88769506-6.61486816-6.48632836-9.88769508-14.52008057-9.88769579-23.78979493" +
        "V242.35266137c0-9.26971435-3.28765845-17.19964576-9.90252661-23.78979492-6.57037354-6.59509253-14.52008057-9.88769508-23.83428955-9.88769579" +
        "h-101.10168433c-9.31420898 0-17.2688601-3.29754663-23.82934618-9.88769507-6.60992408-6.59509253-9.89758325-14.52502465-9.89758254-23.79473902" +
        " 0-9.37353516 3.28765845-17.30346656 9.89758254-23.89361571 6.56048608-6.59014916 14.51513648-9.88769508 23.82934618-9.88769508z";

    // Rendering icon uses several sub-paths composed in a Viewbox (viewBox 128×128).
    private const string RenderingHexagonPath = "M64 34L86 47V73L64 86L42 73V47L64 34Z";
    private const string RenderingOrbitPath1 = "M36 52C42 34 58 26 74 28C90 30 102 42 102 58C102 74 92 88 76 94";
    private const string RenderingOrbitPath2 = "M92 76C86 94 70 102 54 100C38 98 26 86 26 70C26 54 36 40 52 34";
    private const string RenderingMeshPath =
        "M42 47L64 60L86 47 M42 73L64 60L86 73 M64 34V60V86 M42 47L42 73 M86 47V73";

    private static UIElement BuildApiIcon(Color accentColor) =>
        BuildSinglePathIcon(ApiIconPath, accentColor);

    private static UIElement BuildBindingIcon(Color accentColor) =>
        BuildSinglePathIcon(BindingIconPath, accentColor);

    private static UIElement BuildFocusIcon(Color accentColor) =>
        BuildSinglePathIcon(FocusIconPath, accentColor);

    private static UIElement BuildRenderingIcon(Color accentColor)
    {
        var accentBrush = new SolidColorBrush(accentColor);
        var fadedBrush = new SolidColorBrush(
            Color.FromArgb(0xB0, accentColor.R, accentColor.G, accentColor.B));

        var grid = new Grid { Width = 128, Height = 128 };

        // Outer orbit arcs
        grid.Children.Add(new Path
        {
            Data = RenderingOrbitPath1,
            Stroke = fadedBrush,
            StrokeThickness = 6,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            Stretch = Stretch.None
        });
        grid.Children.Add(new Path
        {
            Data = RenderingOrbitPath2,
            Stroke = fadedBrush,
            StrokeThickness = 6,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            Stretch = Stretch.None
        });

        // Hexagon outline
        grid.Children.Add(new Path
        {
            Data = RenderingHexagonPath,
            Stroke = accentBrush,
            StrokeThickness = 6,
            StrokeLineJoin = PenLineJoin.Round,
            Stretch = Stretch.None
        });

        // Internal mesh hinting at a rendering pipeline
        grid.Children.Add(new Path
        {
            Data = RenderingMeshPath,
            Stroke = accentBrush,
            StrokeThickness = 4,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round,
            Stretch = Stretch.None
        });

        return new Viewbox
        {
            Width = 28,
            Height = 28,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = grid
        };
    }

    private static UIElement BuildSinglePathIcon(string data, Color accentColor)
    {
        return new Path
        {
            Data = data,
            Fill = new SolidColorBrush(accentColor),
            Stretch = Stretch.Uniform,
            Width = 22,
            Height = 22,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    #endregion

    private Border CreateFeatureCard(UIElement icon, string title, string description, Color accentColor)
    {
        var card = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(20),
            Margin = new Thickness(6)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        stack.Children.Add(new Border
        {
            Width = 44,
            Height = 44,
            Background = new SolidColorBrush(Color.FromArgb(0x33, accentColor.R, accentColor.G, accentColor.B)),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusMedium),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 14),
            Child = icon
        });

        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 15,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 6)
        });

        stack.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });

        card.Child = stack;
        return card;
    }

    private Border CreateCategoryCard(string title, string description, string pageTag, Color accentColor)
    {
        var card = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(18, 16, 18, 16),
            Margin = new Thickness(6)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Row: colored indicator + title
        var titleRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
        titleRow.Children.Add(new Border
        {
            Width = 10,
            Height = 10,
            Background = new SolidColorBrush(accentColor),
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(0, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Center
        });
        titleRow.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 15,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(titleRow);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 10)
        });

        stack.Children.Add(new TextBlock
        {
            Text = "Explore  \u2192",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.AccentPrimaryBrush
        });

        card.Child = stack;

        card.MouseEnter += (_, _) =>
        {
            card.Background = GalleryTheme.BackgroundHoverBrush;
            card.BorderBrush = new SolidColorBrush(Color.FromArgb(0x88, accentColor.R, accentColor.G, accentColor.B));
        };
        card.MouseLeave += (_, _) =>
        {
            card.Background = GalleryTheme.BackgroundCardBrush;
            card.BorderBrush = GalleryTheme.BorderSubtleBrush;
        };
        card.MouseDown += (_, e) =>
        {
            if (e is MouseButtonEventArgs mouseArgs && mouseArgs.ChangedButton == MouseButton.Left)
            {
                NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(pageTag));
            }
        };

        return card;
    }

    private static Border CreateCodeCard(string title, string subtitle, EditControl editor, bool withBottomMargin)
    {
        var card = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusLarge),
            Padding = new Thickness(22),
            Margin = new Thickness(0, 0, 0, withBottomMargin ? 16 : 0)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = GalleryTheme.FontSizeSubtitle,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4)
        });

        stack.Children.Add(new TextBlock
        {
            Text = subtitle,
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            Margin = new Thickness(0, 0, 0, 16)
        });

        stack.Children.Add(new Border
        {
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(GalleryTheme.CornerRadiusMedium),
            Padding = new Thickness(4),
            Child = editor
        });

        card.Child = stack;
        return card;
    }
}
