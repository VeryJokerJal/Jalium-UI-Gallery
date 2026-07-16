using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class GalleryDemoWorkbench : Page
{
    private const double InspectorWidth = 316;
    private static readonly ControlTemplate ProgressBarRenderTemplate = CreateRangeRenderTemplate(typeof(ProgressBar));
    private static readonly ControlTemplate SliderRenderTemplate = CreateRangeRenderTemplate(typeof(Slider));
    private static readonly ControlTemplate RangeSliderRenderTemplate = CreateRangeRenderTemplate(typeof(RangeSlider));

    private readonly GalleryComponentDescriptor _descriptor;
    private readonly UIElement _demoContent;
    private readonly Dictionary<string, Button> _viewportButtons = new(StringComparer.OrdinalIgnoreCase);

    private ColumnDefinition? _inspectorColumn;
    private Border? _inspectorPanel;
    private Border? _previewFrame;
    private StackPanel? _codePanel;
    private Button? _previewTab;
    private Button? _codeTab;
    private GalleryNumericStepper? _paddingEditor;
    private GalleryNumericStepper? _radiusEditor;
    private GalleryNumericStepper? _opacityEditor;
    private ToggleSwitch? _enabledToggle;
    private GalleryColorEditor? _accentEditor;
    private GalleryColorEditor? _surfaceEditor;
    private bool _showingCode;

    public GalleryDemoWorkbench(GalleryComponentDescriptor descriptor, UIElement demoContent)
    {
        _descriptor = descriptor;
        _demoContent = demoContent;
        if (_demoContent is Control demoControl)
        {
            demoControl.Foreground = GalleryTheme.TextPrimaryBrush;
        }
        Title = descriptor.Title;
        Content = BuildContent();
        ApplyDefaultButtonForegrounds(_demoContent);
        StabilizeRangeControls(_demoContent);

        Loaded += (_, _) => UpdateResponsiveLayout();
        SizeChanged += (_, _) => UpdateResponsiveLayout();
    }

    private UIElement BuildContent()
    {
        var root = new Grid
        {
            Background = GalleryTheme.TransparentBrush
        };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _inspectorColumn = new ColumnDefinition { Width = new GridLength(0) };
        root.ColumnDefinitions.Add(_inspectorColumn);
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var header = CreateHeader();
        Grid.SetColumnSpan(header, 2);
        root.Children.Add(header);

        var main = CreateMainPanel();
        Grid.SetRow(main, 1);
        root.Children.Add(main);

        _inspectorPanel = CreateInspectorPanel();
        Grid.SetRow(_inspectorPanel, 2);
        Grid.SetColumn(_inspectorPanel, 0);
        Grid.SetColumnSpan(_inspectorPanel, 2);
        root.Children.Add(_inspectorPanel);

        SetViewport("Desktop");
        SetTab(showCode: false);
        return root;
    }

    private UIElement CreateHeader()
    {
        var header = new Grid
        {
            Margin = new Thickness(0, 0, 0, 18)
        };
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var copy = new StackPanel { Orientation = Orientation.Vertical };
        copy.Children.Add(new TextBlock
        {
            Text = _descriptor.Category.ToUpperInvariant(),
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 5)
        });
        copy.Children.Add(new TextBlock
        {
            Text = _descriptor.Title,
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 5)
        });
        copy.Children.Add(new TextBlock
        {
            Text = _descriptor.Description,
            FontSize = 13,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        header.Children.Add(copy);

        var badge = new Border
        {
            Background = GalleryTheme.AccentSoftBrush,
            BorderBrush = GalleryTheme.AccentPrimaryBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(9, 4, 9, 4),
            VerticalAlignment = VerticalAlignment.Top,
            Child = new TextBlock
            {
                Text = "LIVE DEMO",
                FontSize = 9,
                FontWeight = FontWeights.SemiBold,
                Foreground = GalleryTheme.AccentDarkBrush
            }
        };
        Grid.SetColumn(badge, 1);
        header.Children.Add(badge);
        return header;
    }

    private UIElement CreateMainPanel()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(CreateWorkbenchToolbar());

        _previewFrame = new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(18),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ClipToBounds = true,
            Child = _demoContent
        };
        stack.Children.Add(_previewFrame);

        _codePanel = CreateCodePanel();
        stack.Children.Add(_codePanel);
        return stack;
    }

    private UIElement CreateWorkbenchToolbar()
    {
        var toolbar = new Grid
        {
            Margin = new Thickness(0, 0, 0, 12)
        };
        toolbar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        toolbar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var tabs = new StackPanel { Orientation = Orientation.Horizontal };
        _previewTab = CreateSegmentButton("Preview", 78, () => SetTab(showCode: false));
        _codeTab = CreateSegmentButton("Code", 66, () => SetTab(showCode: true));
        tabs.Children.Add(_previewTab);
        tabs.Children.Add(_codeTab);
        toolbar.Children.Add(tabs);

        var viewports = new StackPanel { Orientation = Orientation.Horizontal };
        foreach (var mode in new[] { "Desktop", "Tablet", "Mobile" })
        {
            var capturedMode = mode;
            var button = CreateSegmentButton(mode, 70, () => SetViewport(capturedMode));
            _viewportButtons[mode] = button;
            viewports.Children.Add(button);
        }
        Grid.SetColumn(viewports, 1);
        toolbar.Children.Add(viewports);
        return toolbar;
    }

    private StackPanel CreateCodePanel()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Visibility = Visibility.Collapsed
        };
        panel.Children.Add(CreateCodeBlock(
            "JALXAML",
            "Declarative component setup",
            _descriptor.ExampleMarkup,
            isMarkup: true));
        panel.Children.Add(CreateCodeBlock(
            "C#",
            "Equivalent code-behind setup",
            _descriptor.ExampleCSharp,
            isMarkup: false));
        return panel;
    }

    private static UIElement CreateCodeBlock(string title, string caption, string code, bool isMarkup)
    {
        var section = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 18)
        };
        section.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 3)
        });
        section.Children.Add(new TextBlock
        {
            Text = caption,
            FontSize = 11,
            Foreground = GalleryTheme.TextTertiaryBrush,
            Margin = new Thickness(0, 0, 0, 8)
        });

        var editor = new EditControl
        {
            Height = 220,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsReadOnly = true,
            ShowLineNumbers = true,
            FontSize = 13
        };
        editor.SyntaxHighlighter = isMarkup
            ? JalxamlSyntaxHighlighter.Create()
            : RegexSyntaxHighlighter.CreateCSharpHighlighter();
        editor.LoadText(code);

        section.Children.Add(new Border
        {
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            ClipToBounds = true,
            Child = editor
        });
        return section;
    }

    private Border CreateInspectorPanel()
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var heading = new Grid { Margin = new Thickness(0, 0, 0, 16) };
        heading.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        heading.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        heading.Children.Add(new TextBlock
        {
            Text = "Inspector",
            FontSize = 17,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        });
        var live = new TextBlock
        {
            Text = "LIVE",
            FontSize = 9,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.SuccessBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(live, 1);
        heading.Children.Add(live);
        stack.Children.Add(heading);

        stack.Children.Add(CreateSectionLabel("Parameters"));
        _paddingEditor = new GalleryNumericStepper(
            "Canvas padding",
            0,
            40,
            18,
            2,
            value => $"{value:0}px",
            value =>
        {
            if (_previewFrame != null)
            {
                _previewFrame.Padding = new Thickness(value);
            }
        });
        stack.Children.Add(_paddingEditor.Root);

        _radiusEditor = new GalleryNumericStepper(
            "Corner radius",
            0,
            24,
            8,
            2,
            value => $"{value:0}px",
            value =>
        {
            if (_previewFrame != null)
            {
                _previewFrame.CornerRadius = new CornerRadius(value);
            }
        });
        stack.Children.Add(_radiusEditor.Root);

        _opacityEditor = new GalleryNumericStepper(
            "Content opacity",
            40,
            100,
            100,
            5,
            value => $"{value:0}%",
            value =>
        {
            _demoContent.Opacity = value / 100;
        });
        stack.Children.Add(_opacityEditor.Root);

        _enabledToggle = new ToggleSwitch
        {
            Header = "Enabled",
            IsOn = true,
            Foreground = GalleryTheme.TextSecondaryBrush,
            OnBackground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 2, 0, 16)
        };
        _enabledToggle.Toggled += (_, _) => _demoContent.IsEnabled = _enabledToggle.IsOn;
        stack.Children.Add(_enabledToggle);

        stack.Children.Add(CreateDivider());
        stack.Children.Add(CreateSectionLabel("Appearance"));
        stack.Children.Add(CreateThemeSelector());

        _accentEditor = new GalleryColorEditor(
            "Accent",
            GalleryTheme.AccentPrimary,
            GalleryThemeTuner.AccentPalette,
            color =>
            {
                GalleryThemeTuner.ApplyAccent(color);
                if (_previewFrame != null)
                {
                    _previewFrame.BorderBrush = new SolidColorBrush(color);
                }
            });
        stack.Children.Add(_accentEditor.Root);

        _surfaceEditor = new GalleryColorEditor(
            "Canvas",
            GalleryTheme.BackgroundCard,
            GalleryThemeTuner.SurfacePalette,
            color =>
            {
                if (_previewFrame != null)
                {
                    _previewFrame.Background = new SolidColorBrush(color);
                }
            });
        stack.Children.Add(_surfaceEditor.Root);

        var reset = new Button
        {
            Content = "Reset inspector",
            Height = 36,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = GalleryTheme.BackgroundCardInnerBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            Foreground = GalleryTheme.TextSecondaryBrush,
            ToolTip = "Restore the default preview settings"
        };
        reset.Click += (_, _) => ResetInspector();
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

    private UIElement CreateThemeSelector()
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 14)
        };
        row.Children.Add(CreateSegmentButton("Light", 82, () => ApplyTheme(GalleryThemeMode.Light)));
        row.Children.Add(CreateSegmentButton("Dark", 82, () => ApplyTheme(GalleryThemeMode.Dark)));
        return row;
    }

    private static UIElement CreateSectionLabel(string text)
    {
        return new TextBlock
        {
            Text = text.ToUpperInvariant(),
            FontSize = 9,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 12)
        };
    }

    private static UIElement CreateDivider()
    {
        return new Border
        {
            Height = 1,
            Background = GalleryTheme.BorderSubtleBrush,
            Margin = new Thickness(0, 0, 0, 16)
        };
    }

    private static Button CreateSegmentButton(string text, double width, Action action)
    {
        var button = new Button
        {
            Content = text,
            Width = width,
            Height = 34,
            Margin = new Thickness(0, 0, 6, 0),
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            Foreground = GalleryTheme.TextSecondaryBrush
        };
        button.Click += (_, _) => action();
        return button;
    }

    private void SetTab(bool showCode)
    {
        _showingCode = showCode;
        if (_previewFrame != null)
        {
            _previewFrame.Visibility = showCode ? Visibility.Collapsed : Visibility.Visible;
        }
        if (_codePanel != null)
        {
            _codePanel.Visibility = showCode ? Visibility.Visible : Visibility.Collapsed;
        }

        UpdateSegmentSelection(_previewTab, !showCode);
        UpdateSegmentSelection(_codeTab, showCode);
    }

    private void SetViewport(string mode)
    {
        if (_previewFrame != null)
        {
            _previewFrame.MaxWidth = mode switch
            {
                "Mobile" => 420,
                "Tablet" => 760,
                _ => 1600
            };
            _previewFrame.HorizontalAlignment = HorizontalAlignment.Center;
        }

        foreach (var pair in _viewportButtons)
        {
            UpdateSegmentSelection(pair.Value, pair.Key == mode);
        }
    }

    private static void UpdateSegmentSelection(Button? button, bool selected)
    {
        if (button == null)
        {
            return;
        }

        button.Background = selected
            ? GalleryTheme.AccentSoftBrush
            : GalleryTheme.BackgroundCardBrush;
        button.BorderBrush = selected
            ? GalleryTheme.AccentPrimaryBrush
            : GalleryTheme.BorderDefaultBrush;
        button.Foreground = selected
            ? GalleryTheme.AccentDarkBrush
            : GalleryTheme.TextSecondaryBrush;
    }

    private static void ApplyTheme(GalleryThemeMode mode)
    {
        ThemeManager.ApplyTheme(mode == GalleryThemeMode.Dark
            ? ThemeVariant.Dark
            : ThemeVariant.Light);
        GalleryTheme.CurrentMode = mode;
    }

    private static void ApplyDefaultButtonForegrounds(DependencyObject element)
    {
        if (element is Button button &&
            button.IsEnabled &&
            ReferenceEquals(button.ReadLocalValue(Control.ForegroundProperty), DependencyProperty.UnsetValue))
        {
            button.Foreground = GalleryTheme.TextPrimaryBrush;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (var index = 0; index < childCount; index++)
        {
            if (VisualTreeHelper.GetChild(element, index) is DependencyObject child)
            {
                ApplyDefaultButtonForegrounds(child);
            }
        }
    }

    private static void StabilizeRangeControls(DependencyObject element)
    {
        if (element is ProgressBar progressBar)
        {
            progressBar.Template = ProgressBarRenderTemplate;
        }
        else if (element is Slider slider)
        {
            slider.Template = SliderRenderTemplate;
        }
        else if (element is RangeSlider rangeSlider)
        {
            rangeSlider.Template = RangeSliderRenderTemplate;
        }

        if (element is Panel panel)
        {
            for (var index = 0; index < panel.Children.Count; index++)
            {
                StabilizeRangeControls(panel.Children[index]);
            }
            return;
        }

        if (element is Border { Child: DependencyObject borderChild })
        {
            StabilizeRangeControls(borderChild);
            return;
        }

        if (element is ContentControl { Content: DependencyObject contentChild })
        {
            StabilizeRangeControls(contentChild);
            return;
        }

        foreach (var child in LogicalTreeHelper.GetChildren(element))
        {
            if (child is DependencyObject dependencyChild)
            {
                StabilizeRangeControls(dependencyChild);
            }
        }
    }

    private static ControlTemplate CreateRangeRenderTemplate(Type targetType)
    {
        var template = new ControlTemplate(targetType);
        template.SetVisualTree(static () => new Grid { IsHitTestVisible = false });
        return template;
    }

    private void ResetInspector()
    {
        GalleryThemeTuner.Reset();
        _paddingEditor?.SetValue(18);
        _radiusEditor?.SetValue(8);
        _opacityEditor?.SetValue(100);
        if (_enabledToggle != null) _enabledToggle.IsOn = true;
        _accentEditor?.SetColor(GalleryTheme.AccentPrimary);
        _surfaceEditor?.SetColor(GalleryTheme.BackgroundCard);
        SetViewport("Desktop");
        SetTab(_showingCode);
    }

    private void UpdateResponsiveLayout()
    {
        if (_inspectorColumn == null || _inspectorPanel == null || ActualWidth <= 0)
        {
            return;
        }

        var sideBySide = ActualWidth >= 1060;
        _inspectorColumn.Width = new GridLength(sideBySide ? InspectorWidth : 0);
        Grid.SetColumn(_inspectorPanel, sideBySide ? 1 : 0);
        Grid.SetRow(_inspectorPanel, sideBySide ? 1 : 2);
        Grid.SetColumnSpan(_inspectorPanel, sideBySide ? 1 : 2);
        _inspectorPanel.Margin = sideBySide
            ? new Thickness(16, 0, 0, 0)
            : new Thickness(0, 16, 0, 0);
    }
}
