using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ExpanderPage.jalxaml demonstrating expander functionality.
/// </summary>
public partial class ExpanderPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Basic Expander -->
    <Expander Header=""Click to expand"">
        <Border Background=""#252525"" Padding=""16"" CornerRadius=""4"">
            <TextBlock Text=""This content is revealed when expanded.""
                       TextWrapping=""Wrap""/>
        </Border>
    </Expander>

    <!-- Initially expanded -->
    <Expander Header=""Settings"" IsExpanded=""True"">
        <Border Background=""#252525"" Padding=""16"" CornerRadius=""4"">
            <StackPanel Orientation=""Vertical"">
                <CheckBox Content=""Enable notifications"" Margin=""0,0,0,8""/>
                <CheckBox Content=""Auto-save documents"" Margin=""0,0,0,8""/>
                <CheckBox Content=""Show line numbers"" Margin=""0,0,0,8""/>
                <StackPanel Orientation=""Horizontal"">
                    <TextBlock Text=""Theme:"" VerticalAlignment=""Center"" Margin=""0,0,8,0""/>
                    <ComboBox Width=""120"" Height=""28"">
                        <ComboBoxItem Content=""Light""/>
                        <ComboBoxItem Content=""Dark"" IsSelected=""True""/>
                        <ComboBoxItem Content=""System""/>
                    </ComboBox>
                </StackPanel>
            </StackPanel>
        </Border>
    </Expander>

    <!-- Nested Expanders -->
    <Expander Header=""Category A"">
        <StackPanel Orientation=""Vertical"">
            <Expander Header=""Subcategory A1"" Margin=""16,0,0,4"">
                <TextBlock Text=""Content for A1"" Margin=""16,8""/>
            </Expander>
            <Expander Header=""Subcategory A2"" Margin=""16,0,0,4"">
                <TextBlock Text=""Content for A2"" Margin=""16,8""/>
            </Expander>
        </StackPanel>
    </Expander>

    <!-- Programmatically controlled Expander -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,16"">
        <Button x:Name=""ExpandBtn"" Content=""Expand"" Width=""80"" Margin=""0,0,8,0""/>
        <Button x:Name=""CollapseBtn"" Content=""Collapse"" Width=""80"" Margin=""0,0,8,0""/>
        <Button x:Name=""ToggleBtn"" Content=""Toggle"" Width=""80""/>
    </StackPanel>
    <Expander x:Name=""ControlledExpander"" Header=""Controlled Expander"">
        <TextBlock Text=""Controlled via buttons above."" Margin=""16""/>
    </Expander>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;

namespace MyApp;

public partial class ExpanderDemo : Page
{
    public ExpanderDemo()
    {
        InitializeComponent();

        // Control expander programmatically
        ExpandBtn.Click += (s, e) => ControlledExpander.IsExpanded = true;
        CollapseBtn.Click += (s, e) => ControlledExpander.IsExpanded = false;
        ToggleBtn.Click += (s, e) =>
            ControlledExpander.IsExpanded = !ControlledExpander.IsExpanded;

        // Listen for expand/collapse events
        ControlledExpander.Expanded += OnExpanderExpanded;
        ControlledExpander.Collapsed += OnExpanderCollapsed;
    }

    private void OnExpanderExpanded(object sender, RoutedEventArgs e)
    {
        var expander = (Expander)sender;
        StatusText.Text = $""{expander.Header} expanded"";
    }

    private void OnExpanderCollapsed(object sender, RoutedEventArgs e)
    {
        var expander = (Expander)sender;
        StatusText.Text = $""{expander.Header} collapsed"";
    }

    // Create an accordion-style layout where only one expander
    // can be open at a time
    private void SetupAccordion()
    {
        var expanders = new List<Expander>();

        for (int i = 0; i < 5; i++)
        {
            var expander = new Expander
            {
                Header = $""Section {i + 1}"",
                Margin = new Thickness(0, 0, 0, 4)
            };

            expander.Content = new TextBlock
            {
                Text = $""Content for section {i + 1}. This section contains "" +
                       ""detailed information that can be expanded or collapsed."",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(16)
            };

            // Collapse others when one expands
            expander.Expanded += (s, e) =>
            {
                foreach (var other in expanders)
                {
                    if (other != s)
                        other.IsExpanded = false;
                }
            };

            expanders.Add(expander);
            ContentPanel.Children.Add(expander);
        }
    }
}";

    public ExpanderPage()
    {
        InitializeComponent();

        // Set up event handlers for the interactive demo
        if (ExpandButton != null && ControlledExpander != null)
        {
            ExpandButton.Click += (s, e) => ControlledExpander.IsExpanded = true;
        }

        if (CollapseButton != null && ControlledExpander != null)
        {
            CollapseButton.Click += (s, e) => ControlledExpander.IsExpanded = false;
        }

        if (ToggleButton != null && ControlledExpander != null)
        {
            ToggleButton.Click += (s, e) => ControlledExpander.IsExpanded = !ControlledExpander.IsExpanded;
        }

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
