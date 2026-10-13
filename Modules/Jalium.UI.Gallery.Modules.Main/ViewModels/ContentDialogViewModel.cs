using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class ContentDialogViewModel
    : IContentDialogStateSink
{
    private readonly IGalleryContentDialogService _dialogService;
    private long _inlineSequence;
    private string _popupResult =
        "Open a dialog to see the result here.";
    private string _inlineResult = "The inline host is idle.";
    private string _deferredStatus =
        "Try confirming with an empty field to keep the dialog open.";
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

    public void SetInlineResult(ContentDialogResult result) =>
        InlineResult = result switch
        {
            ContentDialogResult.Primary =>
                "Preset applied to the preview area.",
            ContentDialogResult.Secondary =>
                "Preview kept without changing the preset.",
            _ => "Inline dialog canceled."
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
