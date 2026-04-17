using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for UniformGridPage.jalxaml demonstrating the UniformGrid layout panel.
/// </summary>
public partial class UniformGridPage : Page
{
    public UniformGridPage()
    {
        InitializeComponent();

        // Rows slider
        if (RowsSlider != null)
            RowsSlider.ValueChanged += OnRowsChanged;

        // Columns slider
        if (ColumnsSlider != null)
            ColumnsSlider.ValueChanged += OnColumnsChanged;

        // FirstColumn slider
        if (FirstColumnSlider != null)
            FirstColumnSlider.ValueChanged += OnFirstColumnChanged;

        LoadCodeExamples();
    }

    private void OnRowsChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var val = (int)e.NewValue;
        if (InteractiveGrid != null)
            InteractiveGrid.Rows = val;
        if (RowsText != null)
            RowsText.Text = val == 0 ? "Auto" : val.ToString();
    }

    private void OnColumnsChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var val = (int)e.NewValue;
        if (InteractiveGrid != null)
            InteractiveGrid.Columns = val;
        if (ColumnsText != null)
            ColumnsText.Text = val == 0 ? "Auto" : val.ToString();
    }

    private void OnFirstColumnChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var val = (int)e.NewValue;
        if (InteractiveGrid != null)
            InteractiveGrid.FirstColumn = val;
        if (FirstColumnText != null)
            FirstColumnText.Text = val.ToString();
    }

    private const string XamlExample = @"<!-- Auto layout (rows/columns calculated automatically) -->
<UniformGrid>
    <Button Content=""Item 1""/>
    <Button Content=""Item 2""/>
    <Button Content=""Item 3""/>
    <Button Content=""Item 4""/>
</UniformGrid>

<!-- Explicit 2x3 grid -->
<UniformGrid Rows=""2"" Columns=""3"">
    <Button Content=""A""/>
    <Button Content=""B""/>
    <Button Content=""C""/>
    <Button Content=""D""/>
    <Button Content=""E""/>
    <Button Content=""F""/>
</UniformGrid>

<!-- Calendar-style offset with FirstColumn -->
<UniformGrid Columns=""7"" FirstColumn=""3"">
    <TextBlock Text=""1""/>
    <TextBlock Text=""2""/>
    <!-- ... day 1 starts on Wednesday -->
</UniformGrid>";

    private const string CSharpExample = @"// Create and configure UniformGrid
var grid = new UniformGrid
{
    Rows = 3,
    Columns = 4,
    FirstColumn = 0
};

// Add children dynamically
for (int i = 0; i < 12; i++)
{
    grid.Children.Add(new Button
    {
        Content = $""Item {i + 1}""
    });
}

// Auto-calculate: set Rows=0 and/or Columns=0
grid.Rows = 0;    // Auto
grid.Columns = 3; // Fixed 3 columns

// FirstColumn offsets the first row
grid.FirstColumn = 2; // skip 2 cells in first row";

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
