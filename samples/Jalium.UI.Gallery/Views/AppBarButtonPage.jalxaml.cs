using Jalium.UI.Controls;
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

    private static void UpdateStatus(TextBlock? statusText, string message)
    {
        if (statusText != null)
            statusText.Text = message;
    }
}
