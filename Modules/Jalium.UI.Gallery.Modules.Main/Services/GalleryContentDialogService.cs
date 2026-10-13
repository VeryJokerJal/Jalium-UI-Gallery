using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Services;

public sealed class GalleryContentDialogService
    : IGalleryContentDialogService
{
    public async Task<string> ShowRenameAsync()
    {
        var fileNameBox = new TextBox
        {
            Text = "Quarterly Roadmap.md",
            Width = 280,
            Height = 32
        };
        var dialog = new ContentDialog
        {
            Title = "Rename document",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Don't Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = Stack(
                Paragraph(
                    "Use ContentDialog for decisions that need more context than a flyout or message box."),
                new TextBlock
                {
                    Text = "Document name",
                    Margin = new Thickness(0, 0, 0, 4)
                },
                fileNameBox)
        };

        return (await dialog.ShowAsync()) switch
        {
            ContentDialogResult.Primary =>
                $"Saved as {fileNameBox.Text}.",
            ContentDialogResult.Secondary =>
                "Changes were discarded.",
            _ => "Dialog closed without saving."
        };
    }

    public async Task<string> ShowFullSizeAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Full-size review mode",
            FullSizeDesired = true,
            PrimaryButtonText = "Looks Good",
            CloseButtonText = "Close",
            DefaultButton = ContentDialogButton.Primary,
            Content = Stack(
                Paragraph(
                    "FullSizeDesired stretches the card toward the available host region while keeping modal behavior."),
                new Border
                {
                    Background = GalleryTheme.BackgroundCardBrush,
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(16),
                    Child = Paragraph(
                        "Use this for immersive previews, release notes, or review checklists that need more room.")
                })
        };

        return await dialog.ShowAsync() ==
               ContentDialogResult.Primary
            ? "Full-size dialog confirmed."
            : "Full-size dialog dismissed.";
    }

    public async Task<string> ShowDeferredValidationAsync(
        IProgress<string> progress)
    {
        var nameBox = new TextBox
        {
            Width = 260,
            Height = 32
        };
        var dialog = new ContentDialog
        {
            Title = "Create workspace",
            PrimaryButtonText = "Create",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = Stack(
                Paragraph(
                    "PrimaryButtonClick can use a deferral to run async validation before the dialog closes."),
                new TextBlock
                {
                    Text = "Workspace name",
                    Margin = new Thickness(0, 0, 0, 4)
                },
                nameBox)
        };

        dialog.PrimaryButtonClick += (_, args) =>
        {
            var deferral = args.GetDeferral();
            progress.Report("Validating name...");
            _ = ValidateAsync();

            async Task ValidateAsync()
            {
                try
                {
                    await Task.Delay(250);
                    if (string.IsNullOrWhiteSpace(nameBox.Text))
                    {
                        args.Cancel = true;
                        progress.Report(
                            "Name is required. The dialog stayed open.");
                    }
                }
                finally
                {
                    deferral.Complete();
                }
            }
        };

        return await dialog.ShowAsync() ==
               ContentDialogResult.Primary
            ? $"Workspace '{nameBox.Text}' created."
            : "Deferred dialog canceled.";
    }

    private static TextBlock Paragraph(string text) =>
        new()
        {
            Text = text,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12)
        };

    private static StackPanel Stack(params UIElement[] children)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }
}
