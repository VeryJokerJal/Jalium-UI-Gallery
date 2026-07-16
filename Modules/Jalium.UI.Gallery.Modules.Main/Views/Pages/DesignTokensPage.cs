using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class DesignTokensPage : Page
{
    private const string TokenExample = """
        <ResourceDictionary>
            <SolidColorBrush x:Key="GalleryAccent" Color="#08948A" />
            <SolidColorBrush x:Key="GalleryCardBackground" Color="#FFFFFF" />
            <SolidColorBrush x:Key="GalleryTextPrimary" Color="#101827" />

            <CornerRadius x:Key="RadiusSmall">4</CornerRadius>
            <CornerRadius x:Key="RadiusMedium">8</CornerRadius>
            <Thickness x:Key="SpaceMedium">16</Thickness>
        </ResourceDictionary>
        """;

    public DesignTokensPage()
    {
        Title = "Design Tokens";
        Content = BuildContent();
    }

    private UIElement BuildContent()
    {
        var root = new StackPanel { Orientation = Orientation.Vertical };
        root.Children.Add(CreateHeader());
        root.Children.Add(CreateSectionHeading(
            "Color roles",
            "Semantic roles keep components consistent across light and dark modes."));
        root.Children.Add(CreateColorTokens());
        root.Children.Add(CreateSectionHeading(
            "Typography",
            "A compact type ramp for dense application surfaces and readable documentation."));
        root.Children.Add(CreateTypographyScale());
        root.Children.Add(CreateSectionHeading(
            "Spacing and shape",
            "Four-point spacing and restrained radii define the Gallery rhythm."));
        root.Children.Add(CreateSpacingAndRadius());
        root.Children.Add(CreateSectionHeading(
            "Motion",
            "Durations are short, purposeful, and paired with predictable easing."));
        root.Children.Add(CreateMotionTokens());
        root.Children.Add(CreateSectionHeading(
            "Resource export",
            "Core tokens expressed as a Jalium resource dictionary."));
        root.Children.Add(CreateTokenCode());
        return root;
    }

    private UIElement CreateHeader()
    {
        var header = new Grid { Margin = new Thickness(0, 0, 0, 24) };
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var copy = new StackPanel { Orientation = Orientation.Vertical };
        copy.Children.Add(new TextBlock
        {
            Text = "FOUNDATIONS",
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 5)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "Design Tokens",
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 5)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "Shared color, type, spacing, shape, and motion decisions used throughout the Gallery.",
            FontSize = 13,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        header.Children.Add(copy);

        var modes = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Top
        };
        modes.Children.Add(CreateModeButton("Light", GalleryThemeMode.Light));
        modes.Children.Add(CreateModeButton("Dark", GalleryThemeMode.Dark));
        Grid.SetColumn(modes, 1);
        header.Children.Add(modes);
        return header;
    }

    private static Button CreateModeButton(string text, GalleryThemeMode mode)
    {
        var selected = GalleryTheme.CurrentMode == mode;
        var button = new Button
        {
            Content = text,
            Width = 74,
            Height = 34,
            Margin = new Thickness(0, 0, 6, 0),
            Background = selected ? GalleryTheme.AccentSoftBrush : GalleryTheme.BackgroundCardBrush,
            BorderBrush = selected ? GalleryTheme.AccentPrimaryBrush : GalleryTheme.BorderDefaultBrush,
            Foreground = selected ? GalleryTheme.AccentDarkBrush : GalleryTheme.TextSecondaryBrush
        };
        button.Click += (_, _) =>
        {
            ThemeManager.ApplyTheme(mode == GalleryThemeMode.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light);
            GalleryTheme.CurrentMode = mode;
        };
        return button;
    }

    private static UIElement CreateColorTokens()
    {
        var wrap = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 28)
        };

        var tokens = new (string Name, string Role, Color Color)[]
        {
            ("Accent", "Primary actions", GalleryTheme.AccentPrimary),
            ("Accent soft", "Selected surfaces", GalleryTheme.AccentSoft),
            ("Canvas", "Application base", GalleryTheme.BackgroundDark),
            ("Surface", "Cards and panels", GalleryTheme.BackgroundCard),
            ("Surface inner", "Demo canvases", GalleryTheme.BackgroundCardInner),
            ("Border", "Default outline", GalleryTheme.BorderDefault),
            ("Text primary", "Headings and values", GalleryTheme.TextPrimary),
            ("Text muted", "Metadata", GalleryTheme.TextMuted),
            ("Success", "Positive state", GalleryTheme.Success),
            ("Warning", "Attention state", GalleryTheme.Warning),
            ("Error", "Destructive state", GalleryTheme.Error),
            ("Info", "Informational state", GalleryTheme.Info)
        };

        foreach (var token in tokens)
        {
            wrap.Children.Add(CreateColorToken(token.Name, token.Role, token.Color));
        }
        return wrap;
    }

    private static UIElement CreateColorToken(string name, string role, Color color)
    {
        var layout = new Grid();
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        layout.Children.Add(new Border
        {
            Width = 38,
            Height = 38,
            Background = new SolidColorBrush(color),
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            VerticalAlignment = VerticalAlignment.Center
        });

        var copy = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };
        copy.Children.Add(new TextBlock
        {
            Text = name,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush
        });
        copy.Children.Add(new TextBlock
        {
            Text = role,
            FontSize = 9,
            Foreground = GalleryTheme.TextMutedBrush,
            Margin = new Thickness(0, 2, 0, 0)
        });
        copy.Children.Add(new TextBlock
        {
            Text = GalleryThemeTuner.ToHex(color),
            FontSize = 9,
            Foreground = GalleryTheme.TextTertiaryBrush,
            Margin = new Thickness(0, 2, 0, 0)
        });
        Grid.SetColumn(copy, 1);
        layout.Children.Add(copy);

        return new Border
        {
            Width = 205,
            Height = 82,
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(7),
            Padding = new Thickness(12),
            Margin = new Thickness(0, 0, 10, 10),
            Child = layout
        };
    }

    private static UIElement CreateTypographyScale()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        var rows = new (string Token, double Size, string Sample)[]
        {
            ("Display / 28", 28, "Components"),
            ("Title / 20", 20, "Interactive states"),
            ("Heading / 16", 16, "Parameter controls"),
            ("Body / 14", 14, "Readable content for repeated workflows."),
            ("Caption / 12", 12, "Metadata and supporting context"),
            ("Micro / 10", 10, "TOKEN LABEL")
        };

        foreach (var row in rows)
        {
            var grid = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.Children.Add(new TextBlock
            {
                Text = row.Token,
                FontSize = 11,
                Foreground = GalleryTheme.TextMutedBrush,
                VerticalAlignment = VerticalAlignment.Center
            });
            var sample = new TextBlock
            {
                Text = row.Sample,
                FontSize = row.Size,
                FontWeight = row.Size >= 16 ? FontWeights.SemiBold : FontWeights.Normal,
                Foreground = GalleryTheme.TextPrimaryBrush,
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetColumn(sample, 1);
            grid.Children.Add(sample);
            stack.Children.Add(grid);
        }

        return new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 0, 28),
            Child = stack
        };
    }

    private static UIElement CreateSpacingAndRadius()
    {
        var wrap = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 28)
        };
        wrap.Children.Add(CreateSpacingScale());
        wrap.Children.Add(CreateRadiusScale());
        return wrap;
    }

    private static UIElement CreateSpacingScale()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(CreatePanelTitle("Spacing"));
        foreach (var value in new[] { 4, 8, 12, 16, 24, 32 })
        {
            var row = new Grid { Margin = new Thickness(0, 0, 0, 9) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.Children.Add(new TextBlock
            {
                Text = value.ToString(),
                FontSize = 10,
                Foreground = GalleryTheme.TextMutedBrush,
                VerticalAlignment = VerticalAlignment.Center
            });
            var bar = new Border
            {
                Width = value * 4,
                Height = 8,
                Background = GalleryTheme.AccentPrimaryBrush,
                CornerRadius = new CornerRadius(4),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(bar, 1);
            row.Children.Add(bar);
            stack.Children.Add(row);
        }
        return CreateFoundationPanel(stack);
    }

    private static UIElement CreateRadiusScale()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(CreatePanelTitle("Corner radius"));
        var row = new StackPanel { Orientation = Orientation.Horizontal };
        foreach (var value in new[] { 0, 4, 8, 12 })
        {
            var item = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 14, 0)
            };
            item.Children.Add(new Border
            {
                Width = 54,
                Height = 54,
                Background = GalleryTheme.AccentSoftBrush,
                BorderBrush = GalleryTheme.AccentPrimaryBrush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(value),
                Margin = new Thickness(0, 0, 0, 7)
            });
            item.Children.Add(new TextBlock
            {
                Text = value.ToString(),
                FontSize = 10,
                Foreground = GalleryTheme.TextMutedBrush,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            row.Children.Add(item);
        }
        stack.Children.Add(row);
        return CreateFoundationPanel(stack);
    }

    private static UIElement CreateFoundationPanel(UIElement child)
    {
        return new Border
        {
            Width = 410,
            MinHeight = 170,
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 12, 12),
            Child = child
        };
    }

    private static TextBlock CreatePanelTitle(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 14)
        };
    }

    private static UIElement CreateMotionTokens()
    {
        var grid = new UniformGrid
        {
            Columns = 3,
            ColumnSpacing = 12,
            Margin = new Thickness(0, 0, 0, 28)
        };
        grid.Children.Add(CreateMotionToken("Fast", "120ms", "Hover and pressed feedback"));
        grid.Children.Add(CreateMotionToken("Standard", "200ms", "Panels and state changes"));
        grid.Children.Add(CreateMotionToken("Emphasis", "320ms", "Focused view transitions"));
        return grid;
    }

    private static UIElement CreateMotionToken(string name, string duration, string role)
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(new TextBlock
        {
            Text = name,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush
        });
        stack.Children.Add(new TextBlock
        {
            Text = duration,
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 8, 0, 4)
        });
        stack.Children.Add(new TextBlock
        {
            Text = role,
            FontSize = 11,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        return new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Child = stack
        };
    }

    private static UIElement CreateTokenCode()
    {
        var editor = new EditControl
        {
            Height = 250,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsReadOnly = true,
            ShowLineNumbers = true,
            FontSize = 13
        };
        editor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
        editor.LoadText(TokenExample);
        return new Border
        {
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(7),
            ClipToBounds = true,
            Margin = new Thickness(0, 0, 0, 16),
            Child = editor
        };
    }

    private static UIElement CreateSectionHeading(string title, string caption)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 12)
        };
        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush
        });
        stack.Children.Add(new TextBlock
        {
            Text = caption,
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 4, 0, 0)
        });
        return stack;
    }
}
