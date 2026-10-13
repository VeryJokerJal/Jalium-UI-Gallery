using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Services;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class OverviewViewModel
{
    private readonly IGalleryNavigationCoordinator _navigation;

    public OverviewViewModel(
        IGalleryNavigationCoordinator navigation)
    {
        _navigation = navigation;
        EnterGalleryCommand =
            new RelayCommand(EnterGallery);
    }

    public ICommand EnterGalleryCommand { get; }

    private void EnterGallery()
    {
        _navigation.RequestNavigation("home");
        Status = "Gallery opened.";
    }
}
