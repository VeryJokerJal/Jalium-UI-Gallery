using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for AppBarButtonPage.jalxaml demonstrating AppBarButton functionality.
/// </summary>
public partial class AppBarButtonPage : Page
{
    public AppBarButtonPage()
    {
        InitializeComponent();
        SetupIcons();
        SetupEventHandlers();
        LoadCodeExamples();
    }

    private void SetupIcons()
    {
        // Set icon content using SymbolIcon (WinUI-style)
        if (AddButton != null)
            AddButton.Icon = new SymbolIcon(Symbol.Add);

        if (EditButton != null)
            EditButton.Icon = new SymbolIcon(Symbol.Edit);

        if (DeleteButton != null)
            DeleteButton.Icon = new SymbolIcon(Symbol.Delete);

        // Set icon content for compact buttons
        if (CompactAddButton != null)
            CompactAddButton.Icon = new SymbolIcon(Symbol.Add);

        if (CompactEditButton != null)
            CompactEditButton.Icon = new SymbolIcon(Symbol.Edit);

        if (CompactDeleteButton != null)
            CompactDeleteButton.Icon = new SymbolIcon(Symbol.Delete);

        // Set tooltips for compact buttons (shows label on hover)
        if (CompactAddButton != null) CompactAddButton.ToolTip = "Add";
        if (CompactEditButton != null) CompactEditButton.ToolTip = "Edit";
        if (CompactDeleteButton != null) CompactDeleteButton.ToolTip = "Delete";
    }

    private void SetupEventHandlers()
    {
        if (AddButton != null)
            AddButton.Click += (_, _) => UpdateStatus(BasicStatusText, "Add button clicked.");

        if (EditButton != null)
            EditButton.Click += (_, _) => UpdateStatus(BasicStatusText, "Edit button clicked.");

        if (DeleteButton != null)
            DeleteButton.Click += (_, _) => UpdateStatus(BasicStatusText, "Delete button clicked.");

        if (CompactAddButton != null)
            CompactAddButton.Click += (_, _) => UpdateStatus(CompactStatusText, "Compact Add clicked.");

        if (CompactEditButton != null)
            CompactEditButton.Click += (_, _) => UpdateStatus(CompactStatusText, "Compact Edit clicked.");

        if (CompactDeleteButton != null)
            CompactDeleteButton.Click += (_, _) => UpdateStatus(CompactStatusText, "Compact Delete clicked.");
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

    private const string XamlExample = @"<!-- Basic AppBarButton with icon and label -->
<AppBarButton Label=""Add""
              Icon=""Add""
              Width=""72""
              Height=""60"" />

<!-- Compact mode (icon only) -->
<AppBarButton Label=""Edit""
              Icon=""Edit""
              IsCompact=""True""
              Width=""40""
              Height=""40"" />";

    private const string CSharpExample = @"// Create an AppBarButton with icon
var button = new AppBarButton();
button.Label = ""Add"";
button.Icon = new SymbolIcon(Symbol.Add);

// Compact mode shows icon only
button.IsCompact = true;

// Handle click
button.Click += (sender, e) =>
{
    Console.WriteLine(""AppBarButton clicked"");
};";

    private static void UpdateStatus(TextBlock? statusText, string message)
    {
        if (statusText != null)
            statusText.Text = message;
    }
}
