using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ExpanderPage.jalxaml demonstrating expander functionality.
/// </summary>
public partial class ExpanderPage : Page
{
    public ExpanderPage()
    {
        InitializeComponent();

        // Set up event handlers for the interactive demo
        if (ExpandButton != null && ControlledExpander != null)
        {
            ExpandButton.Click += (s, e) => ControlledExpander.IsExpanded = true;
        }

        if (CollapseButton != null && ControlledExpander != null)
        {
            CollapseButton.Click += (s, e) => ControlledExpander.IsExpanded = false;
        }

        if (ToggleButton != null && ControlledExpander != null)
        {
            ToggleButton.Click += (s, e) => ControlledExpander.IsExpanded = !ControlledExpander.IsExpanded;
        }
    }
}
