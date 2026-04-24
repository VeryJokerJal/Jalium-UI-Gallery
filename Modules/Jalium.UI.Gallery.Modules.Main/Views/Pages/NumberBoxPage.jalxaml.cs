using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for NumberBoxPage.jalxaml demonstrating NumberBox functionality.
/// </summary>
public partial class NumberBoxPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic NumberBox -->
    <TextBlock Text=""Quantity:"" Margin=""0,0,0,4""/>
    <NumberBox x:Name=""QuantityBox""
               Value=""1""
               Minimum=""1""
               Maximum=""999""
               SmallChange=""1""
               LargeChange=""10""
               Width=""200""
               HorizontalAlignment=""Left""
               Margin=""0,0,0,16""/>

    <!-- NumberBox with inline spin buttons -->
    <TextBlock Text=""Price ($):"" Margin=""0,0,0,4""/>
    <NumberBox x:Name=""PriceBox""
               Value=""9.99""
               Minimum=""0""
               Maximum=""10000""
               SmallChange=""0.50""
               LargeChange=""10""
               SpinButtonPlacementMode=""Inline""
               Width=""200""
               HorizontalAlignment=""Left""
               Margin=""0,0,0,16""/>

    <!-- NumberBox with percentage range -->
    <TextBlock Text=""Opacity (0-100%):"" Margin=""0,0,0,4""/>
    <NumberBox Value=""75""
               Minimum=""0""
               Maximum=""100""
               SmallChange=""5""
               SpinButtonPlacementMode=""Inline""
               Width=""200""
               HorizontalAlignment=""Left""
               Margin=""0,0,0,16""/>

    <!-- Data-bound NumberBox -->
    <TextBlock Text=""Font Size:"" Margin=""0,0,0,4""/>
    <NumberBox Value=""{Binding FontSize, Mode=TwoWay}""
               Minimum=""8""
               Maximum=""72""
               SmallChange=""1""
               SpinButtonPlacementMode=""Inline""
               Width=""200""
               HorizontalAlignment=""Left""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class NumberBoxSample : Page
{
    public NumberBoxSample()
    {
        InitializeComponent();
        SetupNumberBoxes();
    }

    private void SetupNumberBoxes()
    {
        // Create a NumberBox programmatically
        var quantityBox = new NumberBox
        {
            Value = 1,
            Minimum = 1,
            Maximum = 999,
            SmallChange = 1,
            LargeChange = 10,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline,
            Width = 200
        };

        // Handle value changed
        quantityBox.ValueChanged += OnQuantityChanged;

        // Read current value
        double currentValue = quantityBox.Value;

        // Set value programmatically
        quantityBox.Value = 42;

        // Configure price box with decimal steps
        PriceBox.Minimum = 0;
        PriceBox.Maximum = 10000;
        PriceBox.SmallChange = 0.50;
        PriceBox.LargeChange = 10;
        PriceBox.Value = 29.99;
    }

    private void OnQuantityChanged(NumberBox sender,
        NumberBoxValueChangedEventArgs args)
    {
        double oldValue = args.OldValue;
        double newValue = args.NewValue;
        TotalText.Text = $""Total: {newValue * 9.99:C2}"";
    }
}";

    public NumberBoxPage()
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
