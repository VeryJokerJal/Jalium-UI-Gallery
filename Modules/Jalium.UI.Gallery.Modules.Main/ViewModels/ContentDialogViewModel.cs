using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class ContentDialogViewModel
{
    private readonly IGalleryContentDialogService _dialogService;
    private long _inlineSequence;
    private string _popupResult = LocalizationManager.Get("Loc.Dialog.Status.Popup");
    private string _inlineResult = LocalizationManager.Get("Loc.Dialog.Status.Inline");
    private string _deferredStatus = LocalizationManager.Get("Loc.Dialog.Status.Validation");
    private ContentDialogResult? _inlineDialogResult;
    private ContentDialogRequest? _inlineRequest;

    public ContentDialogViewModel(
        IGalleryContentDialogService dialogService)
    {
        _dialogService = dialogService;
        ShowBasicDialogCommand = new AsyncRelayCommand(
            ShowBasicDialogAsync);
        ShowFullSizeDialogCommand = new AsyncRelayCommand(
            ShowFullSizeDialogAsync);
        ShowInlineDialogCommand = new RelayCommand(() =>
            InlineRequest =
                new ContentDialogRequest(++_inlineSequence));
        ShowDeferredDialogCommand = new AsyncRelayCommand(
            ShowDeferredDialogAsync);
    }

    public ICommand ShowBasicDialogCommand { get; }

    public ICommand ShowFullSizeDialogCommand { get; }

    public ICommand ShowInlineDialogCommand { get; }

    public ICommand ShowDeferredDialogCommand { get; }

    public string PopupResult
    {
        get => _popupResult;
        private set => SetProperty(ref _popupResult, value);
    }

    public string InlineResult
    {
        get => _inlineResult;
        private set => SetProperty(ref _inlineResult, value);
    }

    public string DeferredStatus
    {
        get => _deferredStatus;
        private set => SetProperty(ref _deferredStatus, value);
    }

    public ContentDialogRequest? InlineRequest
    {
        get => _inlineRequest;
        private set => SetProperty(ref _inlineRequest, value);
    }

    public ContentDialogResult? InlineDialogResult
    {
        get => _inlineDialogResult;
        set
        {
            if (SetProperty(ref _inlineDialogResult, value) && value.HasValue) SetInlineResult(value.Value);
        }
    }

    public void SetInlineResult(ContentDialogResult result) =>
        InlineResult = result switch
        {
            ContentDialogResult.Primary =>
                LocalizationManager.Get("Loc.Dialog.Result.Applied"),
            ContentDialogResult.Secondary =>
                LocalizationManager.Get("Loc.Dialog.Result.PreviewOnly"),
            _ => LocalizationManager.Get("Loc.Dialog.Result.InlineCanceled")
        };

    private async Task ShowBasicDialogAsync() =>
        PopupResult = await _dialogService.ShowRenameAsync();

    private async Task ShowFullSizeDialogAsync() =>
        PopupResult = await _dialogService.ShowFullSizeAsync();

    private async Task ShowDeferredDialogAsync()
    {
        var progress = new Progress<string>(
            message => DeferredStatus = message);
        DeferredStatus =
            await _dialogService.ShowDeferredValidationAsync(progress);
    }
}
