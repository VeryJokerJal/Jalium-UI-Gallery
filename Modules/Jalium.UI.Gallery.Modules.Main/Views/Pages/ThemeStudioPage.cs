using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class ThemeStudioPage : Page
{
    private const double InspectorWidth = 330;

    private ColumnDefinition? _inspectorColumn;
    private Border? _inspectorPanel;
    private Border? _previewCanvas;
    private Border? _sampleCard;
    private Button? _primaryButton;
    private ProgressBar? _progress;
    private EditControl? _tokenEditor;
    private GalleryColorEditor? _accentEditor;
    private GalleryColorEditor? _surfaceEditor;
    private GalleryNumericStepper? _radiusEditor;
    private GalleryNumericStepper? _spacingEditor;
    private Color _accent = GalleryTheme.AccentPrimary;
    private Color _surface = GalleryTheme.BackgroundCard;

    public ThemeStudioPage()
    {
        Title = "Theme Studio";
        Content = BuildContent();
        Loaded += (_, _) => UpdateResponsiveLayout();
        SizeChanged += (_, _) => UpdateResponsiveLayout();
    }

    private UIElement BuildContent()
    {
        var root = new Grid { Background = GalleryTheme.TransparentBrush };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _inspectorColumn = new ColumnDefinition { Width = new GridLength(0) };
        root.ColumnDefinitions.Add(_inspectorColumn);
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var header = CreateHeader();
        Grid.SetColumnSpan(header, 2);
        root.Children.Add(header);

        var workspace = CreatePreviewWorkspace();
        Grid.SetRow(workspace, 1);
        root.Children.Add(workspace);

        _inspectorPanel = CreateInspector();
        Grid.SetRow(_inspectorPanel, 2);
        Grid.SetColumnSpan(_inspectorPanel, 2);
        root.Children.Add(_inspectorPanel);

        UpdateTokenCode();
        return root;
    }

    private static UIElement CreateHeader()
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 22)
        };
        stack.Children.Add(new TextBlock
        {
            Text = "THEME TOOLS",
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 5)
        });
        stack.Children.Add(new TextBlock
        {
            Text = "Theme Studio",
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 5)
        });
        stack.Children.Add(new TextBlock
        {
            Text = "Tune semantic colors and geometry against a live component composition.",
            FontSize = 13,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        return stack;
    }

    private UIElement CreatePreviewWorkspace()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        var toolbar = new Grid { Margin = new Thickness(0, 0, 0, 12) };
        toolbar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        toolbar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        toolbar.Children.Add(new TextBlock
        {
            Text = "Live composition",
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        });
        var status = new Border
        {
            Background = GalleryTheme.AccentSoftBrush,
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(8, 4, 8, 4),
            Child = new TextBlock
            {
                Text = "RESOURCE BOUND",
                FontSize = 9,
                FontWeight = FontWeights.SemiBold,
                Foreground = GalleryTheme.AccentDarkBrush
            }
        };
        Grid.SetColumn(status, 1);
        toolbar.Children.Add(status);
        stack.Children.Add(toolbar);

        _sampleCard = CreateSampleCard();
        _previewCanvas = new Border
        {
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(28),
            MinHeight = 430,
            Child = _sampleCard
        };
        stack.Children.Add(_previewCanvas);

        stack.Children.Add(new TextBlock
        {
            Text = "Generated resources",
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 20, 0, 8)
        });
        _tokenEditor = new EditControl
        {
            Height = 230,
            IsReadOnly = true,
            ShowLineNumbers = true,
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        _tokenEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
        stack.Children.Add(new Border
        {
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(7),
            ClipToBounds = true,
            Child = _tokenEditor
        });
        return stack;
    }

    private Border CreateSampleCard()
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            MaxWidth = 620,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        stack.Children.Add(new TextBlock
        {
            Text = "Workspace settings",
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush
        });
        stack.Children.Add(new TextBlock
        {
            Text = "A balanced sample of commands, fields, selection, and status.",
            FontSize = 12,
            Foreground = GalleryTheme.TextTertiaryBrush,
            Margin = new Thickness(0, 4, 0, 20),
            TextWrapping = TextWrapping.Wrap
        });

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 16)
        };
        _primaryButton = new Button
        {
            Content = "Save changes",
            Height = 36,
            Background = GalleryTheme.AccentPrimaryBrush,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            Margin = new Thickness(0, 0, 8, 0)
        };
        actions.Children.Add(_primaryButton);
        actions.Children.Add(new Button
        {
            Content = "Cancel",
            Height = 36,
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            Foreground = GalleryTheme.TextSecondaryBrush
        });
        stack.Children.Add(actions);

        stack.Children.Add(new TextBox
        {
            Text = "Project Aurora",
            Height = 38,
            Margin = new Thickness(0, 0, 0, 12),
            HorizontalAlignment = HorizontalAlignment.Stretch
        });
        stack.Children.Add(new CheckBox
        {
            Content = "Share updates with the team",
            IsChecked = true,
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, 10)
        });
        stack.Children.Add(new ToggleSwitch
        {
            Header = "Automatic snapshots",
            IsOn = true,
            OnBackground = GalleryTheme.AccentPrimaryBrush,
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, 18)
        });

        var progressHeading = new Grid { Margin = new Thickness(0, 0, 0, 6) };
        progressHeading.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        progressHeading.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        progressHeading.Children.Add(new TextBlock
        {
            Text = "Storage used",
            FontSize = 11,
            Foreground = GalleryTheme.TextSecondaryBrush
        });
        var progressValue = new TextBlock
        {
            Text = "68%",
            FontSize = 10,
            Foreground = GalleryTheme.TextMutedBrush
        };
        Grid.SetColumn(progressValue, 1);
        progressHeading.Children.Add(progressValue);
        stack.Children.Add(progressHeading);
        _progress = new ProgressBar
        {
            Value = 68,
            Height = 8,
            ProgressBrush = GalleryTheme.AccentPrimaryBrush
        };
        stack.Children.Add(_progress);

        return new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(24),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            Child = stack
        };
    }

    private Border CreateInspector()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(new TextBlock
        {
            Text = "Theme controls",
            FontSize = 17,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 16)
        });

        stack.Children.Add(CreateSectionLabel("Mode"));
        var modes = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 16)
        };
        modes.Children.Add(CreateModeButton("Light", GalleryThemeMode.Light));
        modes.Children.Add(CreateModeButton("Dark", GalleryThemeMode.Dark));
        stack.Children.Add(modes);

        stack.Children.Add(CreateSectionLabel("Colors"));
        _accentEditor = new GalleryColorEditor(
            "Accent",
            _accent,
            GalleryThemeTuner.AccentPalette,
            ApplyAccent);
        stack.Children.Add(_accentEditor.Root);
        _surfaceEditor = new GalleryColorEditor(
            "Surface",
            _surface,
            GalleryThemeTuner.SurfacePalette,
            ApplySurface);
        stack.Children.Add(_surfaceEditor.Root);

        stack.Children.Add(CreateSectionLabel("Geometry"));
        _radiusEditor = new GalleryNumericStepper(
            "Corner radius",
            0,
            24,
            8,
            2,
            value => $"{value:0}px",
            value =>
        {
            if (_sampleCard != null)
            {
                _sampleCard.CornerRadius = new CornerRadius(value);
            }
            UpdateTokenCode();
        });
        stack.Children.Add(_radiusEditor.Root);

        _spacingEditor = new GalleryNumericStepper(
            "Panel padding",
            8,
            36,
            24,
            2,
            value => $"{value:0}px",
            value =>
        {
            if (_sampleCard != null)
            {
                _sampleCard.Padding = new Thickness(value);
            }
            UpdateTokenCode();
        });
        stack.Children.Add(_spacingEditor.Root);

        var reset = new Button
        {
            Content = "Reset theme",
            Height = 36,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            Foreground = GalleryTheme.TextSecondaryBrush,
            ToolTip = "Restore the Gallery theme tokens"
        };
        reset.Click += (_, _) => ResetTheme();
        stack.Children.Add(reset);

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

    private static Button CreateModeButton(string text, GalleryThemeMode mode)
    {
        var selected = GalleryTheme.CurrentMode == mode;
        var button = new Button
        {
            Content = text,
            Width = 82,
            Height = 34,
            Margin = new Thickness(0, 0, 6, 0),
            Background = selected ? GalleryTheme.AccentSoftBrush : GalleryTheme.BackgroundCardInnerBrush,
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

    private static UIElement CreateSectionLabel(string text)
    {
        return new TextBlock
        {
            Text = text.ToUpperInvariant(),
            FontSize = 9,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 10)
        };
    }

    private void ApplyAccent(Color color)
    {
        _accent = color;
        GalleryThemeTuner.ApplyAccent(color);
        if (_primaryButton != null)
        {
            _primaryButton.Background = new SolidColorBrush(color);
        }
        if (_progress != null)
        {
            _progress.ProgressBrush = new SolidColorBrush(color);
        }
        if (_previewCanvas != null)
        {
            _previewCanvas.BorderBrush = new SolidColorBrush(color);
        }
        UpdateTokenCode();
    }

    private void ApplySurface(Color color)
    {
        _surface = color;
        GalleryThemeTuner.ApplySurface(color);
        if (_sampleCard != null)
        {
            _sampleCard.Background = new SolidColorBrush(color);
        }
        if (_previewCanvas != null)
        {
            var target = GalleryThemeTuner.IsDark(color)
                ? Color.FromRgb(0x00, 0x00, 0x00)
                : Color.FromRgb(0xFF, 0xFF, 0xFF);
            _previewCanvas.Background = new SolidColorBrush(GalleryThemeTuner.Mix(color, target, 0.14));
        }
        UpdateTokenCode();
    }

    private void ResetTheme()
    {
        GalleryThemeTuner.Reset();
        _accent = GalleryTheme.AccentPrimary;
        _surface = GalleryTheme.BackgroundCard;
        _accentEditor?.SetColor(_accent);
        _surfaceEditor?.SetColor(_surface);
        _radiusEditor?.SetValue(8);
        _spacingEditor?.SetValue(24);
        ApplyAccent(_accent);
        ApplySurface(_surface);
    }

    private void UpdateTokenCode()
    {
        if (_tokenEditor == null)
        {
            return;
        }

        var radius = _radiusEditor?.Value ?? 8;
        var spacing = _spacingEditor?.Value ?? 24;
        var code = $"""
            <ResourceDictionary>
                <SolidColorBrush x:Key="GalleryAccent" Color="{GalleryThemeTuner.ToHex(_accent)}" />
                <SolidColorBrush x:Key="GalleryCardBackground" Color="{GalleryThemeTuner.ToHex(_surface)}" />
                <CornerRadius x:Key="GalleryCardRadius">{radius:0}</CornerRadius>
                <Thickness x:Key="GalleryPanelPadding">{spacing:0}</Thickness>
            </ResourceDictionary>
            """;
        _tokenEditor.LoadText(code);
    }

    private void UpdateResponsiveLayout()
    {
        if (_inspectorColumn == null || _inspectorPanel == null || ActualWidth <= 0)
        {
            return;
        }

        var sideBySide = ActualWidth >= 1040;
        _inspectorColumn.Width = new GridLength(sideBySide ? InspectorWidth : 0);
        Grid.SetColumn(_inspectorPanel, sideBySide ? 1 : 0);
        Grid.SetRow(_inspectorPanel, sideBySide ? 1 : 2);
        Grid.SetColumnSpan(_inspectorPanel, sideBySide ? 1 : 2);
        _inspectorPanel.Margin = sideBySide
            ? new Thickness(16, 0, 0, 0)
            : new Thickness(0, 16, 0, 0);
    }
}
