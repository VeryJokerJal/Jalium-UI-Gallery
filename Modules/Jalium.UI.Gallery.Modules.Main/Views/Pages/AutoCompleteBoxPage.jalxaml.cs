using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for AutoCompleteBoxPage.jalxaml demonstrating AutoCompleteBox functionality.
/// </summary>
public partial class AutoCompleteBoxPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Basic AutoCompleteBox with watermark -->
    <AutoCompleteBox x:Name=""SearchBox""
                     Width=""300""
                     Watermark=""Search programming languages...""
                     FilterMode=""Contains""
                     MinimumPrefixLength=""1""
                     MaxDropDownHeight=""200""
                     HorizontalAlignment=""Left""
                     Margin=""0,0,0,16""/>

    <!-- AutoCompleteBox with StartsWith filter -->
    <AutoCompleteBox x:Name=""StartsWithBox""
                     Width=""300""
                     Watermark=""StartsWith filter...""
                     FilterMode=""StartsWith""
                     MinimumPrefixLength=""2""
                     HorizontalAlignment=""Left""
                     Margin=""0,0,0,16""/>

    <!-- AutoCompleteBox with custom item template -->
    <AutoCompleteBox x:Name=""CountryBox""
                     Width=""300""
                     Watermark=""Search countries...""
                     FilterMode=""Contains""
                     TextMemberPath=""Name""
                     HorizontalAlignment=""Left""/>

    <!-- Display selected item -->
    <StackPanel Orientation=""Horizontal"" Margin=""0,16,0,0"">
        <TextBlock Text=""Selected: "" Foreground=""#888888""/>
        <TextBlock x:Name=""SelectedText"" Text=""(none)""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class AutoCompleteSample : Page
{
    public AutoCompleteSample()
    {
        InitializeComponent();
        SetupAutoCompleteBoxes();
    }

    private void SetupAutoCompleteBoxes()
    {
        // Basic string items
        var languages = new[]
        {
            ""C#"", ""C++"", ""JavaScript"", ""TypeScript"",
            ""Python"", ""Java"", ""Kotlin"", ""Swift"",
            ""Rust"", ""Go"", ""Ruby"", ""PHP""
        };

        SearchBox.ItemsSource = languages;
        SearchBox.FilterMode = AutoCompleteFilterMode.Contains;
        SearchBox.MinimumPrefixLength = 1;
        SearchBox.SelectionChanged += OnSearchSelectionChanged;

        // StartsWith filter mode
        StartsWithBox.ItemsSource = languages;
        StartsWithBox.FilterMode = AutoCompleteFilterMode.StartsWith;

        // Complex objects with TextMemberPath
        var countries = new[]
        {
            new Country(""United States"", ""US""),
            new Country(""United Kingdom"", ""UK""),
            new Country(""Germany"", ""DE""),
            new Country(""France"", ""FR""),
            new Country(""Japan"", ""JP"")
        };
        CountryBox.ItemsSource = countries;
        CountryBox.TextMemberPath = ""Name"";
    }

    private void OnSearchSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SearchBox.SelectedItem is string selected)
            SelectedText.Text = selected;
        else
            SelectedText.Text = ""(none)"";
    }

    private record Country(string Name, string Code);
}";

    public AutoCompleteBoxPage()
    {
        InitializeComponent();

        // Set up demo data
        SetupDemoData();
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

    private void SetupDemoData()
    {
        if (DemoAutoComplete == null) return;

        // Create sample data - programming languages
        var languages = new[]
        {
            "C#",
            "C++",
            "C",
            "JavaScript",
            "TypeScript",
            "Python",
            "Java",
            "Kotlin",
            "Swift",
            "Rust",
            "Go",
            "Ruby",
            "PHP",
            "Scala",
            "F#",
            "Haskell",
            "Clojure",
            "Erlang",
            "Elixir",
            "Dart",
            "Lua",
            "R",
            "MATLAB",
            "Julia",
            "Perl"
        };

        DemoAutoComplete.ItemsSource = languages;
        DemoAutoComplete.FilterMode = AutoCompleteFilterMode.Contains;
        DemoAutoComplete.MinimumPrefixLength = 1;
        DemoAutoComplete.SelectionChanged += OnAutoCompleteSelectionChanged;
    }

    private void OnAutoCompleteSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedItemText != null && DemoAutoComplete?.SelectedItem != null)
        {
            SelectedItemText.Text = DemoAutoComplete.SelectedItem.ToString() ?? "(none)";
        }
        else if (SelectedItemText != null)
        {
            SelectedItemText.Text = "(none)";
        }
    }
}
