using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// View-only renderer for the component preview used by the original HomePage
/// design. Featured controls use compact live samples; every other catalog
/// item renders the controls from the first real demo surface on its page.
/// It consumes immutable catalog data and owns no application state, commands,
/// navigation, or services.
/// </summary>
public sealed class HomeComponentPreview : ContentControl
{
    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(HomeCatalogItem),
            typeof(HomeComponentPreview),
            new PropertyMetadata(null, OnPreviewPropertyChanged));

    public static readonly DependencyProperty IsDetailedProperty =
        DependencyProperty.Register(
            nameof(IsDetailed),
            typeof(bool),
            typeof(HomeComponentPreview),
            new PropertyMetadata(true, OnPreviewPropertyChanged));

    public HomeComponentPreview()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        ClipToBounds = true;
    }

    public HomeCatalogItem? Item
    {
        get => GetValue(ItemProperty) as HomeCatalogItem;
        set => SetValue(ItemProperty, value);
    }

    public bool IsDetailed
    {
        get => (bool)(GetValue(IsDetailedProperty) ?? true);
        set => SetValue(IsDetailedProperty, value);
    }

    private static void OnPreviewPropertyChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is HomeComponentPreview preview)
        {
            preview.Rebuild();
        }
    }

    private void Rebuild()
    {
        Content = Item == null
            ? null
            : CreateComponentPreview(Item, IsDetailed);
    }

    private static UIElement CreateComponentPreview(
        HomeCatalogItem item,
        bool detailed) =>
        item.PreviewKind switch
        {
            "buttons" => CreateButtonsPreview(detailed),
            "inputs" => CreateInputsPreview(detailed),
            "cards" => CreateCardPreview(),
            "checkboxes" => CreateCheckboxPreview(detailed),
            "switches" => CreateSwitchPreview(detailed),
            "badges" => CreateBadgePreview(),
            "select" => CreateSelectPreview(),
            "progress" => CreateProgressPreview(),
            "alerts" => CreateAlertPreview(),
            _ => CreateActualComponentPreview(item)
        };

    private static UIElement CreateActualComponentPreview(
        HomeCatalogItem item)
    {
        if (Activator.CreateInstance(item.PageType) is not Page page)
        {
            throw new InvalidOperationException(
                $"{item.PageType.FullName} is not a Gallery Page.");
        }

        page.Width = 640;
        page.Height = double.NaN;
        page.MinWidth = 0;
        page.MinHeight = 0;
        page.Margin = new Thickness(0);
        page.Background = GalleryTheme.TransparentBrush;
        page.ClipToBounds = true;
        ViewportHeightBehavior.SetIsEnabled(page, false);

        KeepOnlyFirstDemoContent(page, item);
        DisableAutomaticPreviewSideEffects(page);

        return new Viewbox
        {
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.Both,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            ClipToBounds = true,
            Child = page
        };
    }

    private static void KeepOnlyFirstDemoContent(
        Page page,
        HomeCatalogItem item)
    {
        if (page.Content is not DependencyObject pageContent)
        {
            throw new InvalidOperationException(
                $"{item.PageType.FullName} has no preview content.");
        }

        var demoSurface = FindFirstDemoSurface(pageContent);

        // Theme Colors is intentionally a bespoke workbench and has no shared
        // demo-surface style. Its first bordered section still contains the
        // real controls, so use the same decoration-free extraction there.
        if (demoSurface == null &&
            item.PageTag == "themecolors")
        {
            demoSurface = FindFirstBorder(
                pageContent,
                static _ => true);
        }

        if (demoSurface?.Child is not UIElement demoContent)
        {
            throw new InvalidOperationException(
                $"{item.PageType.FullName} has no extractable demo controls.");
        }

        demoSurface.Child = null;
        page.Content = demoContent;
    }

    private static Border? FindFirstDemoSurface(
        DependencyObject element)
    {
        if (element is Border section &&
            section.Child is Panel sectionContent)
        {
            var foundCardCopy = false;
            foreach (var child in sectionContent.Children)
            {
                if (child is TextBlock)
                {
                    foundCardCopy = true;
                    continue;
                }

                if (foundCardCopy && child is Border demoSurface)
                {
                    return demoSurface;
                }
            }
        }

        switch (element)
        {
            case Panel panel:
                foreach (var child in panel.Children)
                {
                    if (child is DependencyObject dependencyObject &&
                        FindFirstDemoSurface(dependencyObject)
                            is Border nestedSurface)
                    {
                        return nestedSurface;
                    }
                }

                break;
            case Decorator decorator when decorator.Child != null:
                return FindFirstDemoSurface(decorator.Child);
            case ContentControl contentControl
                when contentControl.Content is DependencyObject content:
                return FindFirstDemoSurface(content);
        }

        return null;
    }

    private static Border? FindFirstBorder(
        DependencyObject element,
        Func<Border, bool> predicate)
    {
        if (element is Border candidate && predicate(candidate))
        {
            return candidate;
        }

        switch (element)
        {
            case Panel panel:
                foreach (var child in panel.Children)
                {
                    if (child is DependencyObject dependencyObject &&
                        FindFirstBorder(
                            dependencyObject,
                            predicate) is Border nestedBorder)
                    {
                        return nestedBorder;
                    }
                }

                break;
            case Decorator decorator when decorator.Child != null:
                return FindFirstBorder(
                    decorator.Child,
                    predicate);
            case ContentControl contentControl
                when contentControl.Content is DependencyObject content:
                return FindFirstBorder(
                    content,
                    predicate);
        }

        return null;
    }

    private static void DisableAutomaticPreviewSideEffects(
        DependencyObject element)
    {
        switch (element)
        {
            case Terminal terminal:
                terminal.AutoStartShell = false;
                terminal.IsReadOnly = true;
                TerminalBehavior.SetObserveProcess(terminal, false);
                TerminalBehavior.SetRequest(terminal, null);
                break;
            case WebView webView:
                webView.Source = null;
                WebViewBehavior.SetIsEnabled(webView, false);
                WebViewBehavior.SetRequest(webView, null);
                break;
            case MediaElement mediaElement:
                mediaElement.Source = null;
                mediaElement.LoadedBehavior = MediaState.Manual;
                MediaElementBehavior.SetTrackProgress(
                    mediaElement,
                    false);
                MediaElementBehavior.SetIsLooping(mediaElement, false);
                MediaElementBehavior.SetRequest(mediaElement, null);
                break;
        }

        switch (element)
        {
            case Panel panel:
                foreach (var child in panel.Children)
                {
                    if (child is DependencyObject dependencyObject)
                    {
                        DisableAutomaticPreviewSideEffects(
                            dependencyObject);
                    }
                }

                break;
            case Decorator decorator when decorator.Child != null:
                DisableAutomaticPreviewSideEffects(decorator.Child);
                break;
            case ContentControl contentControl
                when contentControl.Content is DependencyObject content:
                DisableAutomaticPreviewSideEffects(content);
                break;
        }
    }

    private static UIElement CreateButtonsPreview(bool detailed)
    {
        var row = new WrapPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        row.Children.Add(CreatePreviewButton("Primary", true));
        row.Children.Add(CreatePreviewButton("Secondary", false));
        row.Children.Add(CreateGhostButton());

        if (detailed)
        {
            row.Children.Add(new Button
            {
                Content = "Disabled",
                IsEnabled = false,
                Height = 36,
                Padding = new Thickness(12, 0, 12, 0),
                Margin = new Thickness(4)
            });
        }

        return row;
    }

    private static Button CreatePreviewButton(
        string label,
        bool primary) =>
        new()
        {
            Content = label,
            Height = 36,
            Padding = new Thickness(12, 0, 12, 0),
            Margin = new Thickness(4),
            Background = primary
                ? GalleryTheme.AccentPrimaryBrush
                : GalleryTheme.BackgroundCardBrush,
            BorderBrush = primary
                ? GalleryTheme.AccentPrimaryBrush
                : GalleryTheme.BorderStrongBrush,
            Foreground = primary
                ? new SolidColorBrush(
                    Color.FromRgb(0xFF, 0xFF, 0xFF))
                : GalleryTheme.TextSecondaryBrush,
            FontSize = 11,
            FontWeight = FontWeights.SemiBold
        };

    private static Button CreateGhostButton() =>
        new()
        {
            Content = "Ghost",
            Height = 36,
            Padding = new Thickness(10, 0, 10, 0),
            Margin = new Thickness(4),
            Background = GalleryTheme.TransparentBrush,
            BorderThickness = new Thickness(0),
            Foreground = GalleryTheme.AccentPrimaryBrush,
            FontSize = 11,
            FontWeight = FontWeights.SemiBold
        };

    private static UIElement CreateInputsPreview(bool detailed)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(new TextBox
        {
            Height = 34,
            Text = "Placeholder",
            Foreground = GalleryTheme.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 7)
        });
        stack.Children.Add(new TextBox
        {
            Height = 34,
            Text = "Focused input",
            Margin = new Thickness(0, 0, 0, detailed ? 7 : 0)
        });
        if (detailed)
        {
            stack.Children.Add(new TextBox
            {
                Height = 34,
                Text = "Disabled input",
                IsEnabled = false
            });
        }

        return stack;
    }

    private static UIElement CreateCardPreview()
    {
        var content = new Grid();
        content.ColumnDefinitions.Add(
            new ColumnDefinition { Width = new GridLength(42) });
        content.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        content.Children.Add(new Border
        {
            Width = 34,
            Height = 34,
            Background = GalleryTheme.AccentSoftBrush,
            CornerRadius = new CornerRadius(6),
            Child = new SymbolIcon
            {
                Symbol = Symbol.Component,
                Width = 16,
                Height = 16,
                Foreground = GalleryTheme.AccentPrimaryBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        });

        var copy = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center
        };
        copy.Children.Add(new TextBlock
        {
            Text = "Card title",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "Supporting content for this component.",
            FontSize = 10,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        Grid.SetColumn(copy, 1);
        content.Children.Add(copy);

        return new Border
        {
            Background = GalleryTheme.BackgroundCardBrush,
            BorderBrush = GalleryTheme.BorderDefaultBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            Child = content
        };
    }

    private static UIElement CreateCheckboxPreview(bool detailed)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(new CheckBox
        {
            Content = "Checked",
            IsChecked = true,
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, 6)
        });
        stack.Children.Add(new CheckBox
        {
            Content = "Unchecked",
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, detailed ? 6 : 0)
        });
        if (detailed)
        {
            stack.Children.Add(new CheckBox
            {
                Content = "Disabled",
                IsChecked = true,
                Foreground = GalleryTheme.TextDisabledBrush,
                IsEnabled = false
            });
        }

        return stack;
    }

    private static UIElement CreateSwitchPreview(bool detailed)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(new ToggleSwitch
        {
            Header = "Notifications",
            IsOn = true,
            Foreground = GalleryTheme.TextSecondaryBrush,
            OnBackground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 8)
        });
        stack.Children.Add(new ToggleSwitch
        {
            Header = "Auto update",
            IsOn = false,
            Foreground = GalleryTheme.TextSecondaryBrush,
            OnBackground = GalleryTheme.AccentPrimaryBrush,
            Margin = new Thickness(0, 0, 0, detailed ? 8 : 0)
        });
        if (detailed)
        {
            stack.Children.Add(new ToggleSwitch
            {
                Header = "Unavailable",
                Foreground = GalleryTheme.TextDisabledBrush,
                IsEnabled = false
            });
        }

        return stack;
    }

    private static UIElement CreateBadgePreview()
    {
        var row = new WrapPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        row.Children.Add(
            CreateBadge("Default", GalleryTheme.TextTertiary));
        row.Children.Add(CreateBadge("Success", GalleryTheme.Success));
        row.Children.Add(CreateBadge("Info", GalleryTheme.Info));
        row.Children.Add(CreateBadge("Warning", GalleryTheme.Warning));
        row.Children.Add(CreateBadge("Error", GalleryTheme.Error));
        return row;
    }

    private static UIElement CreateBadge(string label, Color color) =>
        new Border
        {
            Background = new SolidColorBrush(
                Color.FromArgb(0x2F, color.R, color.G, color.B)),
            BorderBrush = new SolidColorBrush(
                Color.FromArgb(0x66, color.R, color.G, color.B)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(7, 4, 7, 4),
            Margin = new Thickness(3),
            Child = new TextBlock
            {
                Text = label,
                FontSize = 9,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(color)
            }
        };

    private static UIElement CreateSelectPreview()
    {
        var comboBox = new ComboBox
        {
            MaxWidth = 220,
            Height = 38,
            PlaceholderText = "Select an option",
            Foreground = GalleryTheme.TextSecondaryBrush,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
        comboBox.Items.Add("Option one");
        comboBox.Items.Add("Option two");
        comboBox.Items.Add("Option three");
        return comboBox;
    }

    private static UIElement CreateProgressPreview()
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
        var valueRow = new Grid
        {
            Margin = new Thickness(0, 0, 0, 7)
        };
        valueRow.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        valueRow.ColumnDefinitions.Add(
            new ColumnDefinition { Width = GridLength.Auto });
        valueRow.Children.Add(new TextBlock
        {
            Text = "Uploading",
            FontSize = 10,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextSecondaryBrush
        });
        var value = new TextBlock
        {
            Text = "60%",
            FontSize = 10,
            Foreground = GalleryTheme.TextTertiaryBrush
        };
        Grid.SetColumn(value, 1);
        valueRow.Children.Add(value);
        stack.Children.Add(valueRow);
        stack.Children.Add(new ProgressBar
        {
            Value = 60,
            Height = 8,
            ProgressBrush = GalleryTheme.AccentPrimaryBrush
        });
        return stack;
    }

    private static UIElement CreateAlertPreview()
    {
        var content = new Grid();
        content.ColumnDefinitions.Add(
            new ColumnDefinition { Width = new GridLength(28) });
        content.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        content.Children.Add(new SymbolIcon
        {
            Symbol = Symbol.Info,
            Width = 20,
            Height = 20,
            Foreground = GalleryTheme.InfoBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });
        var copy = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        copy.Children.Add(new TextBlock
        {
            Text = "Update available",
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 2)
        });
        copy.Children.Add(new TextBlock
        {
            Text = "A new version is ready to install.",
            FontSize = 9,
            Foreground = GalleryTheme.TextTertiaryBrush,
            TextWrapping = TextWrapping.Wrap
        });
        Grid.SetColumn(copy, 1);
        content.Children.Add(copy);

        return new Border
        {
            Background = new SolidColorBrush(
                Color.FromArgb(0x22, 0x38, 0xBD, 0xF8)),
            BorderBrush = new SolidColorBrush(
                Color.FromArgb(0x77, 0x38, 0xBD, 0xF8)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            Child = content
        };
    }
}
