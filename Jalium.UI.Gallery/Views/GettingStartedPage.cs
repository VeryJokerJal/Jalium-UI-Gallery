using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Getting Started page with introduction and quick start guide.
/// </summary>
public class GettingStartedPage : Page
{
    public GettingStartedPage()
    {
        BuildContent();
    }

    private void BuildContent()
    {
        var mainStack = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        // Page Title
        mainStack.Children.Add(new TextBlock
        {
            Text = "Getting Started",
            FontSize = 32,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = new Thickness(0, 0, 0, 8)
        });

        // Description
        mainStack.Children.Add(new TextBlock
        {
            Text = "Learn how to build beautiful desktop applications with Jalium.UI.",
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
            Margin = new Thickness(0, 0, 0, 32)
        });

        // Installation Section
        mainStack.Children.Add(CreateSectionTitle("Installation"));
        mainStack.Children.Add(CreateCodeBlock(
            "dotnet add package Jalium.UI"));

        // Basic Usage Section
        mainStack.Children.Add(CreateSectionTitle("Create Your First Window", new Thickness(0, 24, 0, 12)));
        mainStack.Children.Add(CreateParagraph(
            "Create a new window by extending the Window class:"));
        mainStack.Children.Add(CreateCodeBlock(
@"using Jalium.UI;
using Jalium.UI.Controls;

public class MainWindow : Window
{
    public MainWindow()
    {
        Title = ""My First App"";
        Width = 800;
        Height = 600;

        Content = new TextBlock
        {
            Text = ""Hello, Jalium.UI!"",
            FontSize = 24
        };
    }
}"));

        // XAML Section
        mainStack.Children.Add(CreateSectionTitle("Using XAML", new Thickness(0, 24, 0, 12)));
        mainStack.Children.Add(CreateParagraph(
            "Jalium.UI supports XAML for declarative UI design:"));
        mainStack.Children.Add(CreateCodeBlock(
@"<Window xmlns=""http://schemas.jalium.ui/2024""
        Title=""My First App""
        Width=""800"" Height=""600"">
    <StackPanel Orientation=""Vertical"" Margin=""20"">
        <TextBlock Text=""Welcome!"" FontSize=""24"" />
        <Button Content=""Click Me"" Margin=""0,10,0,0"" />
    </StackPanel>
</Window>"));

        // Layout Section
        mainStack.Children.Add(CreateSectionTitle("Layout Controls", new Thickness(0, 24, 0, 12)));
        mainStack.Children.Add(CreateParagraph(
            "Jalium.UI provides several layout controls to arrange your UI:"));

        var layoutCards = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };
        layoutCards.Children.Add(CreateInfoCard("StackPanel", "Arranges children in a horizontal or vertical line.", Color.FromRgb(0, 120, 212)));
        layoutCards.Children.Add(CreateInfoCard("Grid", "Arranges children in rows and columns.", Color.FromRgb(16, 124, 16)));
        layoutCards.Children.Add(CreateInfoCard("Canvas", "Positions children at absolute coordinates.", Color.FromRgb(136, 23, 152)));
        layoutCards.Children.Add(CreateInfoCard("DockPanel", "Docks children to edges of the container.", Color.FromRgb(202, 80, 16), false));
        mainStack.Children.Add(layoutCards);

        // Data Binding Section
        mainStack.Children.Add(CreateSectionTitle("Data Binding", new Thickness(0, 24, 0, 12)));
        mainStack.Children.Add(CreateParagraph(
            "Bind your UI to data models using the familiar {Binding} syntax:"));
        mainStack.Children.Add(CreateCodeBlock(
@"<TextBlock Text=""{Binding UserName}"" />
<Button Content=""Save"" Command=""{Binding SaveCommand}"" />"));

        // Next Steps Section
        mainStack.Children.Add(CreateSectionTitle("Next Steps", new Thickness(0, 24, 0, 12)));
        mainStack.Children.Add(CreateParagraph(
            "Explore the controls in the navigation panel to learn more about each component."));

        Content = mainStack;
    }

    private TextBlock CreateSectionTitle(string text, Thickness? margin = null)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 20,
            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Margin = margin ?? new Thickness(0, 0, 0, 12)
        };
    }

    private TextBlock CreateParagraph(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 8)
        };
    }

    private Border CreateCodeBlock(string code)
    {
        var border = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 0, 8)
        };

        border.Child = new TextBlock
        {
            Text = code,
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 170)), // Code yellow color
            TextWrapping = TextWrapping.Wrap
        };

        return border;
    }

    private Border CreateInfoCard(string title, string description, Color accentColor, bool hasMargin = true)
    {
        var card = new Border
        {
            Width = 200,
            Height = 90,
            Background = new SolidColorBrush(Color.FromRgb(37, 37, 37)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(53, 53, 53)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14),
            Margin = hasMargin ? new Thickness(0, 0, 12, 0) : new Thickness(0)
        };

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        // Accent bar
        stack.Children.Add(new Border
        {
            Width = 20,
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
            FontSize = 11,
            Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)),
            TextWrapping = TextWrapping.Wrap
        });

        card.Child = stack;
        return card;
    }
}
