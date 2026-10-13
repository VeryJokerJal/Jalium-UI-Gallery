using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class BackdropEffectsViewModel
{
    private readonly IGalleryWindowBackdropService _windowBackdropService;
    private WindowBackdropMode _currentBackdrop;

    public BackdropEffectsViewModel(
        IGalleryWindowBackdropService windowBackdropService)
    {
        _windowBackdropService = windowBackdropService;
        SetBackdropCommand =
            new RelayCommand<WindowBackdropMode>(SetBackdrop);
    }

    public WindowBackdropMode CurrentBackdrop
    {
        get => _currentBackdrop;
        private set
        {
            if (SetProperty(ref _currentBackdrop, value))
            {
                OnPropertyChanged(nameof(SystemBackdropStatus));
            }
        }
    }

    public string SystemBackdropStatus =>
        $"Current: {CurrentBackdrop}";

    public ICommand SetBackdropCommand { get; }

    private void SetBackdrop(WindowBackdropMode mode)
    {
        if (_windowBackdropService.TrySetBackdrop(mode))
        {
            CurrentBackdrop = mode;
            Status = $"Window backdrop changed to {mode}.";
            return;
        }

        Status = "No application window is available.";
    }
}
