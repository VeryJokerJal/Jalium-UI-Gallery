using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Demonstrates data binding features including ElementName binding.
/// </summary>
public partial class BindingPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- One-way ElementName binding: Slider to TextBlock -->
    <Slider x:Name=""ValueSlider""
            Width=""300""
            Minimum=""0"" Maximum=""100""
            Value=""50""
            Margin=""0,0,0,16""/>
    <TextBlock Text=""{Binding Value, ElementName=ValueSlider, StringFormat='Value: {0:F0}'}""
               FontSize=""24""
               Foreground=""#0078D4""/>

    <!-- Two-way binding between TextBox controls -->
    <TextBox x:Name=""SourceTextBox""
             Width=""300"" Height=""32""
             Text=""Type here...""
             Margin=""0,16,0,16""/>
    <TextBox Text=""{Binding Text, ElementName=SourceTextBox, Mode=TwoWay}""
             Width=""300"" Height=""32""/>

    <!-- Binding Modes: OneWay, TwoWay, OneWayToSource, OneTime -->
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Data;

public partial class BindingSample : Page
{
    public BindingSample()
    {
        InitializeComponent();
        SetupBindings();
    }

    private void SetupBindings()
    {
        // Create a binding in code-behind
        var binding = new Binding
        {
            Path = new PropertyPath(""Value""),
            ElementName = ""mySlider"",
            Mode = BindingMode.OneWay,
            StringFormat = ""Value: {0:F1}""
        };

        // Apply binding to a TextBlock
        myTextBlock.SetBinding(TextBlock.TextProperty, binding);

        // Two-way binding between controls
        var twoWayBinding = new Binding
        {
            Path = new PropertyPath(""Text""),
            ElementName = ""sourceTextBox"",
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
        targetTextBox.SetBinding(TextBox.TextProperty, twoWayBinding);
    }
}";

    public BindingPage()
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
