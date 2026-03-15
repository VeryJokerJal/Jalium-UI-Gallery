using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for CommandBarFlyoutPage.jalxaml demonstrating CommandBarFlyout functionality.
/// </summary>
public partial class CommandBarFlyoutPage : Page
{
    private CommandBarFlyout? _basicFlyout;
    private CommandBarFlyout? _mixedFlyout;

    public CommandBarFlyoutPage()
    {
        InitializeComponent();
        BuildBasicFlyoutDemo();
        BuildMixedFlyoutDemo();
    }

    private void BuildBasicFlyoutDemo()
    {
        if (BasicFlyoutContainer == null) return;

        // Create the flyout with primary commands only
        _basicFlyout = new CommandBarFlyout();

        var cutButton = CreateAppBarButton("\u2702", "Cut");
        cutButton.Click += (_, _) => UpdateStatus(BasicFlyoutStatus, "Cut command executed.");

        var copyButton = CreateAppBarButton("\u2398", "Copy");
        copyButton.Click += (_, _) => UpdateStatus(BasicFlyoutStatus, "Copy command executed.");

        var pasteButton = CreateAppBarButton("\u2399", "Paste");
        pasteButton.Click += (_, _) => UpdateStatus(BasicFlyoutStatus, "Paste command executed.");

        var selectAllButton = CreateAppBarButton("\u2610", "Select All");
        selectAllButton.Click += (_, _) => UpdateStatus(BasicFlyoutStatus, "Select All command executed.");

        _basicFlyout.PrimaryCommands.Add(cutButton);
        _basicFlyout.PrimaryCommands.Add(copyButton);
        _basicFlyout.PrimaryCommands.Add(pasteButton);
        _basicFlyout.PrimaryCommands.Add(selectAllButton);

        // Create the trigger button
        var triggerButton = new Button
        {
            Content = "Show Editing Commands",
            Height = 36,
            Width = 200,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        triggerButton.Click += (_, _) =>
        {
            UpdateStatus(BasicFlyoutStatus, "CommandBarFlyout opened.");
            _basicFlyout.ShowAt(triggerButton);
        };

        _basicFlyout.Closed += (_, _) =>
        {
            UpdateStatus(BasicFlyoutStatus, "CommandBarFlyout closed.");
        };

        BasicFlyoutContainer.Children.Add(triggerButton);
    }

    private void BuildMixedFlyoutDemo()
    {
        if (MixedFlyoutContainer == null) return;

        // Create the flyout with primary and secondary commands
        _mixedFlyout = new CommandBarFlyout();

        // Primary commands (shown in the main bar)
        var shareButton = CreateAppBarButton("\u2197", "Share");
        shareButton.Click += (_, _) => UpdateStatus(MixedFlyoutStatus, "Share command executed.");

        var favoriteButton = CreateAppBarButton("\u2605", "Favorite");
        favoriteButton.Click += (_, _) => UpdateStatus(MixedFlyoutStatus, "Favorite command executed.");

        _mixedFlyout.PrimaryCommands.Add(shareButton);
        _mixedFlyout.PrimaryCommands.Add(favoriteButton);

        // Secondary commands (shown in the overflow menu)
        var renameButton = CreateAppBarButton("\u270F", "Rename");
        renameButton.Click += (_, _) => UpdateStatus(MixedFlyoutStatus, "Rename command executed (from overflow).");

        var deleteButton = CreateAppBarButton("\u2716", "Delete");
        deleteButton.Click += (_, _) => UpdateStatus(MixedFlyoutStatus, "Delete command executed (from overflow).");

        var propertiesButton = CreateAppBarButton("\u2699", "Properties");
        propertiesButton.Click += (_, _) => UpdateStatus(MixedFlyoutStatus, "Properties command executed (from overflow).");

        _mixedFlyout.SecondaryCommands.Add(renameButton);
        _mixedFlyout.SecondaryCommands.Add(deleteButton);
        _mixedFlyout.SecondaryCommands.Add(propertiesButton);

        // Create the trigger button
        var triggerButton = new Button
        {
            Content = "Show Mixed Commands Flyout",
            Height = 36,
            Width = 240,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        triggerButton.Click += (_, _) =>
        {
            UpdateStatus(MixedFlyoutStatus, "Mixed CommandBarFlyout opened.");
            _mixedFlyout.ShowAt(triggerButton);
        };

        _mixedFlyout.Closed += (_, _) =>
        {
            UpdateStatus(MixedFlyoutStatus, "Mixed CommandBarFlyout closed.");
        };

        MixedFlyoutContainer.Children.Add(triggerButton);
    }

    private static AppBarButton CreateAppBarButton(string iconText, string label)
    {
        var button = new AppBarButton
        {
            Label = label,
            IsCompact = true,
            Width = 48,
            Height = 48
        };

        button.Content = new TextBlock
        {
            Text = iconText,
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        button.ToolTip = label;
        return button;
    }

    private static void UpdateStatus(TextBlock? statusText, string message)
    {
        if (statusText != null)
            statusText.Text = message;
    }
}
