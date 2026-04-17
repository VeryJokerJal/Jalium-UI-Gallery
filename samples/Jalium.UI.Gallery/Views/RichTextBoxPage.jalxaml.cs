using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for RichTextBoxPage.jalxaml demonstrating rich text box functionality.
/// </summary>
public partial class RichTextBoxPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic RichTextBox -->
    <RichTextBox x:Name=""BasicRichTextBox""
                 Width=""500""
                 Height=""200""
                 Margin=""0,0,0,16""/>

    <!-- RichTextBox with formatting toolbar -->
    <Border Background=""#1E1E1E"" CornerRadius=""4"" Padding=""8"">
        <StackPanel Orientation=""Vertical"">
            <ToolBar Background=""Transparent"" Margin=""0,0,0,8"">
                <Button x:Name=""BoldButton""
                        Content=""B""
                        Width=""32"" Height=""28""
                        FontWeight=""Bold""
                        Click=""OnBoldClick""/>
                <Button x:Name=""ItalicButton""
                        Content=""I""
                        Width=""32"" Height=""28""
                        FontStyle=""Italic""
                        Click=""OnItalicClick""/>
                <Button x:Name=""UnderlineButton""
                        Content=""U""
                        Width=""32"" Height=""28""
                        Click=""OnUnderlineClick""/>
                <Separator/>
                <ComboBox x:Name=""FontFamilyCombo""
                          Width=""140"" Height=""28""
                          SelectionChanged=""OnFontFamilyChanged"">
                    <ComboBoxItem Content=""Segoe UI"" IsSelected=""True""/>
                    <ComboBoxItem Content=""Arial""/>
                    <ComboBoxItem Content=""Times New Roman""/>
                    <ComboBoxItem Content=""Consolas""/>
                </ComboBox>
                <ComboBox x:Name=""FontSizeCombo""
                          Width=""60"" Height=""28""
                          SelectionChanged=""OnFontSizeChanged"">
                    <ComboBoxItem Content=""10""/>
                    <ComboBoxItem Content=""12"" IsSelected=""True""/>
                    <ComboBoxItem Content=""14""/>
                    <ComboBoxItem Content=""18""/>
                    <ComboBoxItem Content=""24""/>
                </ComboBox>
            </ToolBar>
            <RichTextBox x:Name=""FormattedRichTextBox""
                         Width=""500"" Height=""250""/>
        </StackPanel>
    </Border>

    <!-- Read-only RichTextBox for displaying content -->
    <RichTextBox x:Name=""ReadOnlyRichTextBox""
                 Width=""500""
                 Height=""150""
                 IsReadOnly=""True""
                 Margin=""0,16,0,0""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Controls.Documents;

public partial class RichTextEditorPage : Page
{
    public RichTextEditorPage()
    {
        InitializeComponent();
        SetupRichTextBox();
    }

    private void SetupRichTextBox()
    {
        // Create a FlowDocument with formatted content
        var document = new FlowDocument();

        var heading = new Paragraph();
        heading.Inlines.Add(new Run(""Welcome to RichTextBox"")
        {
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212))
        });
        document.Blocks.Add(heading);

        var body = new Paragraph();
        body.Inlines.Add(new Run(""This is a ""));
        body.Inlines.Add(new Run(""rich text"")
        {
            FontWeight = FontWeights.Bold
        });
        body.Inlines.Add(new Run("" editor with ""));
        body.Inlines.Add(new Run(""formatting"")
        {
            FontStyle = FontStyles.Italic,
            Foreground = new SolidColorBrush(Color.FromRgb(156, 39, 176))
        });
        body.Inlines.Add(new Run("" support.""));
        document.Blocks.Add(body);

        FormattedRichTextBox.Document = document;
    }

    private void OnBoldClick(object sender, RoutedEventArgs e)
    {
        var selection = FormattedRichTextBox.Selection;
        if (selection != null && !selection.IsEmpty)
        {
            var currentWeight = selection.GetPropertyValue(TextElement.FontWeightProperty);
            var newWeight = (currentWeight is FontWeight fw && fw == FontWeights.Bold)
                ? FontWeights.Normal
                : FontWeights.Bold;
            selection.ApplyPropertyValue(TextElement.FontWeightProperty, newWeight);
        }
    }

    private void OnItalicClick(object sender, RoutedEventArgs e)
    {
        var selection = FormattedRichTextBox.Selection;
        if (selection != null && !selection.IsEmpty)
        {
            var currentStyle = selection.GetPropertyValue(TextElement.FontStyleProperty);
            var newStyle = (currentStyle is FontStyle fs && fs == FontStyles.Italic)
                ? FontStyles.Normal
                : FontStyles.Italic;
            selection.ApplyPropertyValue(TextElement.FontStyleProperty, newStyle);
        }
    }
}";

    public RichTextBoxPage()
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
