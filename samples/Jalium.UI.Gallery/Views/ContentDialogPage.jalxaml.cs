using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Demonstrates Jalium.UI ContentDialog usage in popup and in-place modes.
/// </summary>
public partial class ContentDialogPage : Page
{
    public ContentDialogPage()
    {
        InitializeComponent();

        if (ShowBasicDialogButton != null)
        {
            ShowBasicDialogButton.Click += OnShowBasicDialogClick;
        }

        if (ShowFullSizeDialogButton != null)
        {
            ShowFullSizeDialogButton.Click += OnShowFullSizeDialogClick;
        }

        if (ShowInlineDialogButton != null)
        {
            ShowInlineDialogButton.Click += OnShowInlineDialogClick;
        }

        if (ShowDeferredDialogButton != null)
        {
            ShowDeferredDialogButton.Click += OnShowDeferredDialogClick;
        }
    }

    private async void OnShowBasicDialogClick(object? sender, EventArgs e)
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
            Content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Use ContentDialog for decisions that need more context than a flyout or message box.",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 12)
                    },
                    new TextBlock
                    {
                        Text = "Document name",
                        Margin = new Thickness(0, 0, 0, 4)
                    },
                    fileNameBox
                }
            }
        };

        var result = await dialog.ShowAsync();
        if (PopupDialogResultText != null)
        {
            PopupDialogResultText.Text = result switch
            {
                ContentDialogResult.Primary => $"Saved as {fileNameBox.Text}.",
                ContentDialogResult.Secondary => "Changes were discarded.",
                _ => "Dialog closed without saving."
            };
        }
    }

    private async void OnShowFullSizeDialogClick(object? sender, EventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Full-size review mode",
            FullSizeDesired = true,
            PrimaryButtonText = "Looks Good",
            CloseButtonText = "Close",
            DefaultButton = ContentDialogButton.Primary,
            Content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children =
                {
                    new TextBlock
                    {
                        Text = "FullSizeDesired stretches the card toward the available host region while keeping modal behavior.",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 12)
                    },
                    new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)),
                        CornerRadius = new CornerRadius(6),
                        Padding = new Thickness(16),
                        Child = new TextBlock
                        {
                            Text = "Use this for immersive previews, release notes, or review checklists that need more room.",
                            TextWrapping = TextWrapping.Wrap
                        }
                    }
                }
            }
        };

        var result = await dialog.ShowAsync();
        if (PopupDialogResultText != null)
        {
            PopupDialogResultText.Text = result == ContentDialogResult.Primary
                ? "Full-size dialog confirmed."
                : "Full-size dialog dismissed.";
        }
    }

    private async void OnShowInlineDialogClick(object? sender, EventArgs e)
    {
        if (InlineContentDialog == null)
        {
            return;
        }

        var result = await InlineContentDialog.ShowAsync(ContentDialogPlacement.InPlace);
        if (InlineDialogResultText != null)
        {
            InlineDialogResultText.Text = result switch
            {
                ContentDialogResult.Primary => "Preset applied to the preview area.",
                ContentDialogResult.Secondary => "Preview kept without changing the preset.",
                _ => "Inline dialog canceled."
            };
        }
    }

    private async void OnShowDeferredDialogClick(object? sender, EventArgs e)
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
            Content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children =
                {
                    new TextBlock
                    {
                        Text = "PrimaryButtonClick can use a deferral to run async validation before the dialog closes.",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 12)
                    },
                    new TextBlock
                    {
                        Text = "Workspace name",
                        Margin = new Thickness(0, 0, 0, 4)
                    },
                    nameBox
                }
            }
        };

        dialog.PrimaryButtonClick += (_, args) =>
        {
            var deferral = args.GetDeferral();
            DeferredDialogStatusText!.Text = "Validating name...";

            Dispatcher.BeginInvokeCritical(async () =>
            {
                try
                {
                    await Task.Delay(250);

                    if (string.IsNullOrWhiteSpace(nameBox.Text))
                    {
                        args.Cancel = true;
                        DeferredDialogStatusText.Text = "Name is required. The dialog stayed open.";
                    }
                }
                finally
                {
                    deferral.Complete();
                }
            });
        };

        var result = await dialog.ShowAsync();
        if (DeferredDialogStatusText != null)
        {
            DeferredDialogStatusText.Text = result == ContentDialogResult.Primary
                ? $"Workspace '{nameBox.Text}' created."
                : "Deferred dialog canceled.";
        }
    }
}
