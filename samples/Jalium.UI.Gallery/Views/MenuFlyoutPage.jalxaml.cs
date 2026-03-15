using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for MenuFlyoutPage.jalxaml demonstrating MenuFlyout (ContextMenu) functionality.
/// </summary>
public partial class MenuFlyoutPage : Page
{
    private ContextMenu? _basicMenu;
    private ContextMenu? _iconMenu;
    private ContextMenu? _checkMenu;

    public MenuFlyoutPage()
    {
        InitializeComponent();
        BuildBasicMenuFlyout();
        BuildIconMenuFlyout();
        BuildCheckableMenuFlyout();
    }

    private void BuildBasicMenuFlyout()
    {
        if (ShowMenuButton == null) return;

        _basicMenu = new ContextMenu();

        var openItem = new MenuItem { Header = "Open" };
        openItem.Click += (s, e) =>
        {
            UpdateBasicStatus("Open selected");
            _basicMenu.IsOpen = false;
        };
        _basicMenu.Items.Add(openItem);

        var editItem = new MenuItem { Header = "Edit" };
        editItem.Click += (s, e) =>
        {
            UpdateBasicStatus("Edit selected");
            _basicMenu.IsOpen = false;
        };
        _basicMenu.Items.Add(editItem);

        var renameItem = new MenuItem { Header = "Rename" };
        renameItem.Click += (s, e) =>
        {
            UpdateBasicStatus("Rename selected");
            _basicMenu.IsOpen = false;
        };
        _basicMenu.Items.Add(renameItem);

        _basicMenu.Items.Add(new Separator());

        var shareItem = new MenuItem { Header = "Share" };
        shareItem.Click += (s, e) =>
        {
            UpdateBasicStatus("Share selected");
            _basicMenu.IsOpen = false;
        };
        _basicMenu.Items.Add(shareItem);

        var deleteItem = new MenuItem { Header = "Delete" };
        deleteItem.Click += (s, e) =>
        {
            UpdateBasicStatus("Delete selected");
            _basicMenu.IsOpen = false;
        };
        _basicMenu.Items.Add(deleteItem);

        ShowMenuButton.Click += (s, e) =>
        {
            _basicMenu.PlacementTarget = ShowMenuButton;
            _basicMenu.Placement = PlacementMode.Bottom;
            _basicMenu.IsOpen = !_basicMenu.IsOpen;
        };
    }

    private void BuildIconMenuFlyout()
    {
        if (ShowIconMenuButton == null) return;

        _iconMenu = new ContextMenu();

        var cutItem = new MenuItem { Header = "Cut", Icon = "\u2702", InputGestureText = "Ctrl+X" };
        cutItem.Click += (s, e) =>
        {
            UpdateIconStatus("Cut action triggered");
            _iconMenu.IsOpen = false;
        };
        _iconMenu.Items.Add(cutItem);

        var copyItem = new MenuItem { Header = "Copy", Icon = "\u2398", InputGestureText = "Ctrl+C" };
        copyItem.Click += (s, e) =>
        {
            UpdateIconStatus("Copy action triggered");
            _iconMenu.IsOpen = false;
        };
        _iconMenu.Items.Add(copyItem);

        var pasteItem = new MenuItem { Header = "Paste", Icon = "\u2399", InputGestureText = "Ctrl+V" };
        pasteItem.Click += (s, e) =>
        {
            UpdateIconStatus("Paste action triggered");
            _iconMenu.IsOpen = false;
        };
        _iconMenu.Items.Add(pasteItem);

        _iconMenu.Items.Add(new Separator());

        var undoItem = new MenuItem { Header = "Undo", Icon = "\u21B6", InputGestureText = "Ctrl+Z" };
        undoItem.Click += (s, e) =>
        {
            UpdateIconStatus("Undo action triggered");
            _iconMenu.IsOpen = false;
        };
        _iconMenu.Items.Add(undoItem);

        var redoItem = new MenuItem { Header = "Redo", Icon = "\u21B7", InputGestureText = "Ctrl+Y" };
        redoItem.Click += (s, e) =>
        {
            UpdateIconStatus("Redo action triggered");
            _iconMenu.IsOpen = false;
        };
        _iconMenu.Items.Add(redoItem);

        ShowIconMenuButton.Click += (s, e) =>
        {
            _iconMenu.PlacementTarget = ShowIconMenuButton;
            _iconMenu.Placement = PlacementMode.Bottom;
            _iconMenu.IsOpen = !_iconMenu.IsOpen;
        };
    }

    private void BuildCheckableMenuFlyout()
    {
        if (ShowCheckMenuButton == null) return;

        _checkMenu = new ContextMenu();

        var toolbarItem = new MenuItem { Header = "Toolbar", IsCheckable = true, IsChecked = true };
        toolbarItem.Click += (s, e) =>
            UpdateCheckStatus($"Toolbar: {(toolbarItem.IsChecked ? "Visible" : "Hidden")}");
        _checkMenu.Items.Add(toolbarItem);

        var statusBarItem = new MenuItem { Header = "Status Bar", IsCheckable = true, IsChecked = true };
        statusBarItem.Click += (s, e) =>
            UpdateCheckStatus($"Status Bar: {(statusBarItem.IsChecked ? "Visible" : "Hidden")}");
        _checkMenu.Items.Add(statusBarItem);

        var sidebarItem = new MenuItem { Header = "Sidebar", IsCheckable = true };
        sidebarItem.Click += (s, e) =>
            UpdateCheckStatus($"Sidebar: {(sidebarItem.IsChecked ? "Visible" : "Hidden")}");
        _checkMenu.Items.Add(sidebarItem);

        _checkMenu.Items.Add(new Separator());

        var miniMapItem = new MenuItem { Header = "Minimap", IsCheckable = true };
        miniMapItem.Click += (s, e) =>
            UpdateCheckStatus($"Minimap: {(miniMapItem.IsChecked ? "Visible" : "Hidden")}");
        _checkMenu.Items.Add(miniMapItem);

        var breadcrumbItem = new MenuItem { Header = "Breadcrumbs", IsCheckable = true, IsChecked = true };
        breadcrumbItem.Click += (s, e) =>
            UpdateCheckStatus($"Breadcrumbs: {(breadcrumbItem.IsChecked ? "Visible" : "Hidden")}");
        _checkMenu.Items.Add(breadcrumbItem);

        ShowCheckMenuButton.Click += (s, e) =>
        {
            _checkMenu.PlacementTarget = ShowCheckMenuButton;
            _checkMenu.Placement = PlacementMode.Bottom;
            _checkMenu.IsOpen = !_checkMenu.IsOpen;
        };
    }

    private void UpdateBasicStatus(string message)
    {
        if (BasicMenuStatusText != null)
            BasicMenuStatusText.Text = message;
        System.Diagnostics.Debug.WriteLine($"[MenuFlyoutPage] Basic: {message}");
    }

    private void UpdateIconStatus(string message)
    {
        if (IconMenuStatusText != null)
            IconMenuStatusText.Text = message;
        System.Diagnostics.Debug.WriteLine($"[MenuFlyoutPage] Icon: {message}");
    }

    private void UpdateCheckStatus(string message)
    {
        if (CheckMenuStatusText != null)
            CheckMenuStatusText.Text = message;
        System.Diagnostics.Debug.WriteLine($"[MenuFlyoutPage] Check: {message}");
    }
}
