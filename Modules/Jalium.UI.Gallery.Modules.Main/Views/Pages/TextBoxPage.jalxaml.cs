using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class TextBoxPage : Page
{
    private const string XamlExample = @"<!-- Basic TextBox -->
<TextBox Width=""300""
         Height=""32""/>

<!-- TextBox with pre-filled text -->
<TextBox Width=""300""
         Height=""32""
         Text=""Pre-filled text""/>

<!-- Disabled TextBox -->
<TextBox Width=""300""
         Height=""32""
         Text=""Read-only content""
         IsEnabled=""False""/>

<!-- TextBox with event handling -->
<TextBox x:Name=""InputTextBox""
         Width=""400""
         Height=""32""
         TextChanged=""OnTextChanged""/>
<TextBlock x:Name=""CharCount""
           Text=""Characters: 0""/>

<!-- Clipboard demo: source and target -->
<StackPanel Orientation=""Vertical"" Spacing=""8"">
    <TextBlock Text=""Source (select and Ctrl+C):""/>
    <TextBox x:Name=""SourceTextBox""
             Width=""400""
             Height=""32""
             Text=""Select this text and copy""/>

    <TextBlock Text=""Target (Ctrl+V to paste):""/>
    <TextBox x:Name=""TargetTextBox""
             Width=""400""
             Height=""32""/>
</StackPanel>

<!-- Multiple TextBoxes in a form layout -->
<StackPanel Orientation=""Vertical"" Spacing=""12"">
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""First Name"" FontSize=""12""/>
        <TextBox Width=""300"" Height=""32""/>
    </StackPanel>
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""Last Name"" FontSize=""12""/>
        <TextBox Width=""300"" Height=""32""/>
    </StackPanel>
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""Email"" FontSize=""12""/>
        <TextBox Width=""300"" Height=""32""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class SearchPage : Page
{
    public SearchPage()
    {
        InitializeComponent();

        SearchBox.TextChanged += OnSearchTextChanged;
        SearchBox.KeyDown += OnSearchKeyDown;
    }

    private void OnSearchTextChanged(object sender, EventArgs e)
    {
        string query = SearchBox.Text ?? """";
        CharCount.Text = $""Characters: {query.Length}"";

        // Live search filtering
        if (query.Length >= 3)
        {
            FilterResults(query);
        }
        else
        {
            ClearResults();
        }
    }

    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            PerformSearch(SearchBox.Text);
        }
        else if (e.Key == Key.Escape)
        {
            SearchBox.Text = """";
            SearchBox.Focus();
        }
    }

    private void FilterResults(string query)
    {
        // Filter items based on search query
        ResultsPanel.Children.Clear();
        foreach (var item in _allItems)
        {
            if (item.Contains(query,
                StringComparison.OrdinalIgnoreCase))
            {
                ResultsPanel.Children.Add(
                    new TextBlock { Text = item });
            }
        }
    }

    // Programmatic TextBox creation
    private TextBox CreateLabeledInput(string label)
    {
        var textBox = new TextBox
        {
            Width = 300,
            Height = 32
        };
        return textBox;
    }
}";

    public TextBoxPage()
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
