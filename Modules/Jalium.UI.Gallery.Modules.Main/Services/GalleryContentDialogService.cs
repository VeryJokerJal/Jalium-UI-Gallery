using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;

namespace Jalium.UI.Gallery.Modules.Main.Services;

/// <summary>Dialog orchestration; each content control owns its replaceable theme template.</summary>
public sealed class GalleryContentDialogService : IGalleryContentDialogService
{
    public async Task<string> ShowRenameAsync()
    {
        var (dialog, form) = CreateRenameDialog();
        return (await dialog.ShowAsync()) switch
        {
            ContentDialogResult.Primary => LocalizationManager.Format("Loc.Dialog.Result.Saved", form.Text),
            ContentDialogResult.Secondary => LocalizationManager.Get("Loc.Dialog.Result.Discarded"),
            _ => LocalizationManager.Get("Loc.Dialog.Result.Closed")
        };
    }

    internal (ContentDialog Dialog, GalleryDialogForm Form) CreateRenameDialog()
    {
        var form = new GalleryDialogForm { Text = "Quarterly Roadmap.md" };
        Text(form, GalleryDialogForm.DescriptionProperty, "Loc.Dialog.RenameDescription");
        Text(form, GalleryDialogForm.LabelProperty, "Loc.Dialog.DocumentName");
        var dialog = CreateDialog(form, "Loc.Dialog.RenameTitle", "Loc.Dialog.Save", "Loc.Dialog.Cancel");
        Text(dialog, ContentDialog.SecondaryButtonTextProperty, "Loc.Dialog.Discard");
        return (dialog, form);
    }

    public async Task<string> ShowFullSizeAsync()
        => LocalizationManager.Get(await CreateFullSizeDialog().ShowAsync() == ContentDialogResult.Primary
            ? "Loc.Dialog.Result.FullConfirmed" : "Loc.Dialog.Result.FullDismissed");

    internal ContentDialog CreateFullSizeDialog()
    {
        var preview = new GalleryDialogPreview();
        Text(preview, GalleryDialogPreview.DescriptionProperty, "Loc.Dialog.FullDescription");
        Text(preview, GalleryDialogPreview.PreviewTextProperty, "Loc.Dialog.PreviewText");
        var dialog = CreateDialog(preview, "Loc.Dialog.FullTitle", "Loc.Dialog.Confirm", "Loc.Dialog.Close");
        dialog.FullSizeDesired = true;
        return dialog;
    }

    public async Task<string> ShowDeferredValidationAsync(IProgress<string> progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        var (dialog, form) = CreateValidationDialog(progress);
        return await dialog.ShowAsync() == ContentDialogResult.Primary
            ? LocalizationManager.Format("Loc.Dialog.Result.Created", form.Text)
            : LocalizationManager.Get("Loc.Dialog.Result.ValidationCanceled");
    }

    internal (ContentDialog Dialog, GalleryDialogForm Form) CreateValidationDialog(IProgress<string> progress)
    {
        var form = new GalleryDialogForm();
        Text(form, GalleryDialogForm.DescriptionProperty, "Loc.Dialog.ValidationDescription");
        Text(form, GalleryDialogForm.LabelProperty, "Loc.Dialog.WorkspaceName");
        var dialog = CreateDialog(form, "Loc.Dialog.CreateTitle", "Loc.Dialog.Create", "Loc.Dialog.Cancel");
        dialog.PrimaryButtonClick += (_, args) =>
        {
            var deferral = args.GetDeferral();
            form.ClearValue(GalleryDialogForm.ErrorMessageProperty);
            form.IsBusy = true;
            args.Cancel = true;
            _ = ValidateAsync();
            async Task ValidateAsync()
            {
                try
                {
                    progress.Report(LocalizationManager.Get("Loc.Dialog.Validating"));
                    await Task.Delay(250);
                    if (string.IsNullOrWhiteSpace(form.Text))
                    {
                        args.Cancel = true;
                        Text(form, GalleryDialogForm.ErrorMessageProperty, "Loc.Dialog.NameRequired");
                        progress.Report(LocalizationManager.Get("Loc.Dialog.NameRequired"));
                    }
                    else args.Cancel = false;
                }
                catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
                {
                    Text(form, GalleryDialogForm.ErrorMessageProperty, "Loc.Dialog.ValidationFailed");
                    System.Diagnostics.Debug.WriteLine($"Gallery dialog validation failed: {ex}");
                }
                finally
                {
                    form.IsBusy = false;
                    if (args.Cancel) form.FocusInput();
                    deferral.Complete();
                }
            }
        };
        return (dialog, form);
    }

    private static ContentDialog CreateDialog(object content, string title, string primary, string close)
    {
        var dialog = new ContentDialog { Content = content, DefaultButton = ContentDialogButton.Primary };
        Text(dialog, ContentDialog.TitleProperty, title);
        Text(dialog, ContentDialog.PrimaryButtonTextProperty, primary);
        Text(dialog, ContentDialog.CloseButtonTextProperty, close);
        return dialog;
    }
    private static void Text(FrameworkElement target, DependencyProperty property, string key)
        => target.SetResourceReference(property, key);
}
