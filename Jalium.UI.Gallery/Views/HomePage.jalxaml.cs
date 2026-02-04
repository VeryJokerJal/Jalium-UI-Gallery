using Jalium.UI.Controls;
using Jalium.UI.Gallery.Theme;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

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

    public HomePage()
    {
        InitializeComponent();
        BuildContent();
    }

    private void BuildContent()
    {
        // Create main container
        var mainStack = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        // Page Title
        mainStack.Children.Add(new TextBlock
        {
            Text = "Welcome to Jalium.UI Gallery",
            FontSize = 32,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 8)
        });

        // Description
        mainStack.Children.Add(new TextBlock
        {
            Text = "Explore the Jalium.UI framework controls and components.",
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
            Margin = new Thickness(0, 0, 0, 32)
        });

        // Framework Features Section
        mainStack.Children.Add(new TextBlock
        {
            Text = "Framework Features",
            FontSize = 20,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 16)
        });

        // Feature cards row
        var featureRow = new StackPanel { Orientation = Orientation.Horizontal };
        featureRow.Children.Add(CreateFeatureCard("WPF-Like API", "Familiar dependency property system, routed events, and XAML support.", Color.FromRgb(0, 120, 212)));
        featureRow.Children.Add(CreateFeatureCard("Modern Rendering", "Native D3D12 rendering backend for high performance graphics.", Color.FromRgb(16, 124, 16)));
        featureRow.Children.Add(CreateFeatureCard("Data Binding", "Full MVVM support with {Binding} syntax and INotifyPropertyChanged.", Color.FromRgb(136, 23, 152)));
        featureRow.Children.Add(CreateFeatureCard("Focus System", "Complete keyboard navigation and focus management.", Color.FromRgb(202, 80, 16), false));
        mainStack.Children.Add(featureRow);

        // Controls Section
        mainStack.Children.Add(new TextBlock
        {
            Text = "Explore Controls",
            FontSize = 20,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 32, 0, 16)
        });

        // Control category cards - first row (navigate to category overview pages)
        var controlRow1 = new StackPanel { Orientation = Orientation.Horizontal };
        controlRow1.Children.Add(CreateCategoryCard("Basic Input", "Button, CheckBox, RadioButton, Slider, TextBox", "basic", Color.FromRgb(0, 120, 212)));
        controlRow1.Children.Add(CreateCategoryCard("Text", "TextBlock and text formatting", "text", Color.FromRgb(16, 124, 16)));
        controlRow1.Children.Add(CreateCategoryCard("Layout", "StackPanel, Grid, Canvas, Border", "layout", Color.FromRgb(136, 23, 152)));
        controlRow1.Children.Add(CreateCategoryCard("Navigation", "TabControl", "navigation", Color.FromRgb(202, 80, 16), false));
        mainStack.Children.Add(controlRow1);

        // Control category cards - second row (navigate to category overview pages)
        var controlRow2 = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };
        controlRow2.Children.Add(CreateCategoryCard("Media", "Image display", "media", Color.FromRgb(0, 99, 177)));
        controlRow2.Children.Add(CreateCategoryCard("Collections", "ListBox, TreeView", "collections", Color.FromRgb(177, 70, 194)));
        mainStack.Children.Add(controlRow2);

        Content = mainStack;
    }

    private Border CreateFeatureCard(string title, string description, Color accentColor, bool hasMargin = true)
    {
        var card = new Border
        {
            Width = 220,
            Height = 140,
            Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = hasMargin ? new Thickness(0, 0, 12, 0) : new Thickness(0)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Accent bar
        stack.Children.Add(new Border
        {
            Width = 32,
            Height = 4,
            Background = new SolidColorBrush(accentColor),
            CornerRadius = new CornerRadius(2),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 12)
        });

        // Title
        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 8)
        });

        // Description
        stack.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
            TextWrapping = TextWrapping.Wrap,
            Height = 48
        });

        card.Child = stack;
        return card;
    }

    private Border CreateCategoryCard(string title, string description, string pageTag, Color accentColor, bool hasMargin = true)
    {
        var card = new Border
        {
            Width = 220,
            Height = 90,
            Background = new SolidColorBrush(Color.FromRgb(37, 37, 37)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(53, 53, 53)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16),
            Margin = hasMargin ? new Thickness(0, 0, 12, 0) : new Thickness(0)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Accent bar
        stack.Children.Add(new Border
        {
            Width = 24,
            Height = 3,
            Background = new SolidColorBrush(accentColor),
            CornerRadius = new CornerRadius(1.5),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 8)
        });

        // Title
        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 4)
        });

        // Description
        stack.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102))
        });

        card.Child = stack;

        // Add hover effect and click handler
        card.MouseEnter += (s, e) =>
        {
            card.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            card.BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        };

        card.MouseLeave += (s, e) =>
        {
            card.Background = new SolidColorBrush(Color.FromRgb(37, 37, 37));
            card.BorderBrush = new SolidColorBrush(Color.FromRgb(53, 53, 53));
        };

        card.MouseDown += (s, e) =>
        {
            if (e is MouseButtonEventArgs mouseArgs && mouseArgs.ChangedButton == MouseButton.Left)
            {
                NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(pageTag));
            }
        };

        return card;
    }
}
