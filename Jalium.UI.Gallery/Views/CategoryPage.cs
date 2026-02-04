using Jalium.UI.Controls;
using Jalium.UI.Gallery.Theme;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Represents a control item to display in a category page.
/// </summary>
public record ControlInfo(string Name, string Description, string PageTag);

/// <summary>
/// Base class for category overview pages that display control cards.
/// </summary>
public abstract class CategoryPage : Page
{
    /// <summary>
    /// Occurs when a control card is clicked and navigation is requested.
    /// </summary>
    public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

    protected abstract string CategoryTitle { get; }
    protected abstract string CategoryDescription { get; }
    protected abstract Color AccentColor { get; }
    protected abstract IEnumerable<ControlInfo> Controls { get; }

    protected CategoryPage()
    {
        BuildContent();
    }

    private void BuildContent()
    {
        var mainStack = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        // Category Title
        mainStack.Children.Add(new TextBlock
        {
            Text = CategoryTitle,
            FontSize = 32,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 8)
        });

        // Category Description
        mainStack.Children.Add(new TextBlock
        {
            Text = CategoryDescription,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
            Margin = new Thickness(0, 0, 0, 32)
        });

        // Controls Section
        mainStack.Children.Add(new TextBlock
        {
            Text = "Controls",
            FontSize = 20,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 16)
        });

        // Create rows of control cards (4 per row)
        var controls = Controls.ToList();
        var currentRow = new StackPanel { Orientation = Orientation.Horizontal };
        var cardCount = 0;

        foreach (var control in controls)
        {
            var isLastInRow = (cardCount + 1) % 4 == 0 || cardCount == controls.Count - 1;
            currentRow.Children.Add(CreateControlCard(control, !isLastInRow));
            cardCount++;

            if (cardCount % 4 == 0)
            {
                mainStack.Children.Add(currentRow);
                currentRow = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 12, 0, 0)
                };
            }
        }

        // Add remaining cards
        if (currentRow.Children.Count > 0)
        {
            mainStack.Children.Add(currentRow);
        }

        Content = mainStack;
    }

    private Border CreateControlCard(ControlInfo control, bool hasMargin)
    {
        var card = new Border
        {
            Width = 220,
            Height = 100,
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
            Background = new SolidColorBrush(AccentColor),
            CornerRadius = new CornerRadius(1.5),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 10)
        });

        // Control name
        stack.Children.Add(new TextBlock
        {
            Text = control.Name,
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 6)
        });

        // Description
        stack.Children.Add(new TextBlock
        {
            Text = control.Description,
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            TextWrapping = TextWrapping.Wrap
        });

        card.Child = stack;

        // Hover effects
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

        // Click handler
        card.MouseDown += (s, e) =>
        {
            if (e is MouseButtonEventArgs mouseArgs && mouseArgs.ChangedButton == MouseButton.Left)
            {
                NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(control.PageTag));
            }
        };

        return card;
    }
}

/// <summary>
/// Basic Input controls category page.
/// </summary>
public class BasicCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Basic Input";
    protected override string CategoryDescription => "Basic input controls for user interaction.";
    protected override Color AccentColor => Color.FromRgb(0, 120, 212);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Button", "A control that responds to user clicks", "button"),
        new ControlInfo("CheckBox", "A control for boolean selection", "checkbox"),
        new ControlInfo("RadioButton", "A control for single selection from a group", "radiobutton"),
        new ControlInfo("Slider", "A control for selecting a value from a range", "slider"),
        new ControlInfo("ProgressBar", "A control for displaying progress", "progressbar"),
        new ControlInfo("TextBox", "A control for text input", "textbox"),
        new ControlInfo("PasswordBox", "A control for secure text input", "passwordbox"),
        new ControlInfo("ComboBox", "A control for dropdown selection", "combobox")
    };
}

/// <summary>
/// Text controls category page.
/// </summary>
public class TextCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Text";
    protected override string CategoryDescription => "Controls for displaying and formatting text.";
    protected override Color AccentColor => Color.FromRgb(16, 124, 16);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("TextBlock", "A control for displaying text", "textblock")
    };
}

/// <summary>
/// Layout controls category page.
/// </summary>
public class LayoutCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Layout";
    protected override string CategoryDescription => "Controls for arranging and positioning content.";
    protected override Color AccentColor => Color.FromRgb(136, 23, 152);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("StackPanel", "Arranges children in a single line", "stackpanel"),
        new ControlInfo("Grid", "Arranges children in rows and columns", "grid"),
        new ControlInfo("Canvas", "Positions children at absolute coordinates", "canvas"),
        new ControlInfo("Border", "Draws a border around content", "border"),
        new ControlInfo("DockPanel", "Docks children to edges", "dockpanel"),
        new ControlInfo("WrapPanel", "Wraps children to multiple lines", "wrappanel"),
        new ControlInfo("ScrollViewer", "Enables scrolling of content", "scrollviewer")
    };
}

/// <summary>
/// Navigation controls category page.
/// </summary>
public class NavigationCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Navigation";
    protected override string CategoryDescription => "Controls for navigating between content.";
    protected override Color AccentColor => Color.FromRgb(202, 80, 16);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("TabControl", "A control for tabbed content", "tabcontrol")
    };
}

/// <summary>
/// Media controls category page.
/// </summary>
public class MediaCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Media";
    protected override string CategoryDescription => "Controls for displaying media content.";
    protected override Color AccentColor => Color.FromRgb(0, 99, 177);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("Image", "A control for displaying images", "image")
    };
}

/// <summary>
/// Collections controls category page.
/// </summary>
public class CollectionsCategoryPage : CategoryPage
{
    protected override string CategoryTitle => "Collections";
    protected override string CategoryDescription => "Controls for displaying collections of items.";
    protected override Color AccentColor => Color.FromRgb(177, 70, 194);
    protected override IEnumerable<ControlInfo> Controls => new[]
    {
        new ControlInfo("ListBox", "A control for displaying a list of items", "listbox"),
        new ControlInfo("TreeView", "A control for displaying hierarchical data", "treeview")
    };
}
