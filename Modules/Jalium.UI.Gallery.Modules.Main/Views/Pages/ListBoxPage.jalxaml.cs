using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Primitives;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for ListBoxPage.jalxaml demonstrating ListBox functionality.
/// </summary>
public partial class ListBoxPage : Page
{
    public ListBoxPage()
    {
        InitializeComponent();

        // Populate the demo ListBox
        if (DemoListBox != null)
        {
            var items = new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry", "Fig", "Grape" };
            foreach (var item in items)
            {
                DemoListBox.Items.Add(item);
            }

            DemoListBox.SelectionChanged += OnSelectionChanged;
        }

        LoadCodeExamples();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItemText != null && DemoListBox != null)
        {
            SelectedItemText.Text = DemoListBox.SelectedItem?.ToString() ?? "(none)";
        }
    }

    private const string XamlExample =
@"<!-- Basic ListBox -->
<ListBox x:Name=""DemoListBox""
         Height=""150""
         Margin=""0,0,0,16""/>

<!-- ListBox with inline items -->
<ListBox Height=""150"" Width=""200"">
    <ListBoxItem Content=""Apple""/>
    <ListBoxItem Content=""Banana""/>
    <ListBoxItem Content=""Cherry""/>
    <ListBoxItem Content=""Date""/>
    <ListBoxItem Content=""Elderberry""/>
</ListBox>

<!-- Selection mode -->
<ListBox SelectionMode=""Multiple""
         Height=""150"" Width=""200"">
    <ListBoxItem Content=""Option 1""/>
    <ListBoxItem Content=""Option 2""/>
    <ListBoxItem Content=""Option 3""/>
</ListBox>";

    private const string CSharpExample =
@"// Create and populate a ListBox
var listBox = new ListBox();
var items = new[] {
    ""Apple"", ""Banana"", ""Cherry"",
    ""Date"", ""Elderberry"", ""Fig"", ""Grape""
};
foreach (var item in items)
{
    listBox.Items.Add(item);
}

// Handle selection changes
listBox.SelectionChanged += (s, e) =>
{
    var selected = listBox.SelectedItem?.ToString();
    statusText.Text = selected ?? ""(none)"";
};

// Bind to a collection
listBox.ItemsSource = myObservableCollection;

// Multiple selection mode
listBox.SelectionMode = SelectionMode.Multiple;

// Get all selected items
foreach (var item in listBox.SelectedItems)
{
    Debug.WriteLine(item.ToString());
}";

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
