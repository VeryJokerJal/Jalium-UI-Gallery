using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ToolBarPage.jalxaml demonstrating toolbar functionality.
/// </summary>
public partial class ToolBarPage : Page
{
    public ToolBarPage()
    {
        InitializeComponent();

        // Set up event handlers
        if (NewButton != null) NewButton.Click += (s, e) => UpdateAction("New clicked");
        if (OpenButton != null) OpenButton.Click += (s, e) => UpdateAction("Open clicked");
        if (SaveButton != null) SaveButton.Click += (s, e) => UpdateAction("Save clicked");
        if (UndoButton != null) UndoButton.Click += (s, e) => UpdateAction("Undo clicked");
        if (RedoButton != null) RedoButton.Click += (s, e) => UpdateAction("Redo clicked");
    }

    private void UpdateAction(string action)
    {
        if (ActionText != null)
        {
            ActionText.Text = action;
        }
    }
}
