using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ToolTipPage.jalxaml demonstrating tooltip functionality.
/// </summary>
public partial class ToolTipPage : Page
{
    public ToolTipPage()
    {
        InitializeComponent();

        // Keep one example configured from code-behind.
        if (SaveButton != null)
        {
            SaveButton.ToolTip = "Save the current document";
        }
    }
}
