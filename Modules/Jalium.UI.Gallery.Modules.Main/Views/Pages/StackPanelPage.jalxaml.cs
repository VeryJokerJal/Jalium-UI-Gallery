using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class StackPanelPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Vertical StackPanel (default) -->
    <StackPanel Orientation=""Vertical"" Margin=""0,0,0,16"">
        <Border Background=""#0078D4"" Height=""40"" Margin=""0,0,0,8"">
            <TextBlock Text=""Item 1"" Foreground=""#FFFFFF""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#107C10"" Height=""40"" Margin=""0,0,0,8"">
            <TextBlock Text=""Item 2"" Foreground=""#FFFFFF""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#881798"" Height=""40"">
            <TextBlock Text=""Item 3"" Foreground=""#FFFFFF""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
    </StackPanel>

    <!-- Horizontal StackPanel -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,16"">
        <Border Background=""#0078D4"" Width=""100"" Height=""60"" Margin=""0,0,8,0"">
            <TextBlock Text=""Left"" Foreground=""#FFFFFF""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#107C10"" Width=""100"" Height=""60"" Margin=""0,0,8,0"">
            <TextBlock Text=""Center"" Foreground=""#FFFFFF""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
        <Border Background=""#881798"" Width=""100"" Height=""60"">
            <TextBlock Text=""Right"" Foreground=""#FFFFFF""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
        </Border>
    </StackPanel>

    <!-- Nested StackPanels for complex layouts -->
    <StackPanel Orientation=""Vertical"">
        <TextBlock Text=""Form Layout"" FontSize=""16"" FontWeight=""Bold"" Margin=""0,0,0,8""/>
        <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,8"">
            <TextBlock Text=""Name:"" Width=""80"" VerticalAlignment=""Center""/>
            <TextBox Width=""200""/>
        </StackPanel>
        <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,8"">
            <TextBlock Text=""Email:"" Width=""80"" VerticalAlignment=""Center""/>
            <TextBox Width=""200""/>
        </StackPanel>
        <StackPanel Orientation=""Horizontal"" HorizontalAlignment=""Right"">
            <Button Content=""Cancel"" Width=""80"" Margin=""0,0,8,0""/>
            <Button Content=""Submit"" Width=""80""/>
        </StackPanel>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class StackPanelDemo : Page
{
    public StackPanelDemo()
    {
        InitializeComponent();
        BuildLayout();
    }

    private void BuildLayout()
    {
        // Create a vertical StackPanel
        var verticalPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(16)
        };

        // Add items dynamically
        var colors = new[] { ""#0078D4"", ""#107C10"", ""#881798"", ""#CA5010"" };
        for (int i = 0; i < colors.Length; i++)
        {
            var item = new Border
            {
                Height = 40,
                Background = new SolidColorBrush(ColorFromHex(colors[i])),
                Margin = new Thickness(0, 0, 0, 8),
                Child = new TextBlock
                {
                    Text = $""Item {i + 1}"",
                    Foreground = new SolidColorBrush(Color.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            verticalPanel.Children.Add(item);
        }

        // Create a horizontal toolbar
        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 16, 0, 0)
        };

        var buttons = new[] { ""New"", ""Open"", ""Save"", ""Export"" };
        foreach (var label in buttons)
        {
            toolbar.Children.Add(new Button
            {
                Content = label,
                Width = 80,
                Margin = new Thickness(0, 0, 8, 0)
            });
        }
        verticalPanel.Children.Add(toolbar);

        // Nest panels for form layout
        var formPanel = new StackPanel { Orientation = Orientation.Vertical };
        formPanel.Children.Add(CreateFormRow(""Username:"", 100));
        formPanel.Children.Add(CreateFormRow(""Password:"", 100));
        verticalPanel.Children.Add(formPanel);

        ContentArea.Child = verticalPanel;
    }

    private StackPanel CreateFormRow(string label, double labelWidth)
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 8)
        };
        row.Children.Add(new TextBlock
        {
            Text = label,
            Width = labelWidth,
            VerticalAlignment = VerticalAlignment.Center
        });
        row.Children.Add(new TextBox { Width = 200 });
        return row;
    }
}";

    public StackPanelPage()
    {
        InitializeComponent();
        LoadCodeExamples();
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }
}
