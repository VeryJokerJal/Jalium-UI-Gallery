using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class HomePageVirtualizationFixture
{
    [Fact]
    public void CatalogState_ExposesAllComponentsWithoutViewOwnedCopies()
    {
        var viewModel =
            new HomeViewModel(Mock.Of<IGalleryNavigationCoordinator>());

        Assert.Equal(121, HomeCatalog.Items.Count);
        Assert.Equal(HomeCatalog.Items.Count, viewModel.VisibleItems.Count);
        Assert.Equal(HomeCatalog.Items[0], viewModel.SelectedItem);
        Assert.Equal("121 components", viewModel.ResultCountText);
    }

    [Fact]
    public void SelectedItemAndSelectedKey_StayInSync()
    {
        var viewModel =
            new HomeViewModel(Mock.Of<IGalleryNavigationCoordinator>());
        var byObject = HomeCatalog.Items[42];
        var byKey = HomeCatalog.Items[71];

        viewModel.SelectedItem = byObject;

        Assert.Equal(byObject.Key, viewModel.SelectedItemKey);

        viewModel.SelectedItemKey = byKey.Key;

        Assert.Same(byKey, viewModel.SelectedItem);
    }

    [Fact]
    public void SelectItemCommand_ChangesTheBindableSelection()
    {
        var viewModel =
            new HomeViewModel(Mock.Of<IGalleryNavigationCoordinator>());
        var expected = HomeCatalog.Items[^1];

        viewModel.SelectItemCommand.Execute(expected);

        Assert.Same(expected, viewModel.SelectedItem);
    }

    [Fact]
    public void OpenBestMatch_SelectsFirstResultAndRequestsOneNavigation()
    {
        var navigation = new Mock<IGalleryNavigationCoordinator>();
        var viewModel = new HomeViewModel(navigation.Object)
        {
            SearchQuery = "TreeSelector"
        };
        var expected = Assert.Single(viewModel.VisibleItems);

        viewModel.OpenBestMatchCommand.Execute(null);

        Assert.Same(expected, viewModel.SelectedItem);
        navigation.Verify(
            coordinator => coordinator.RequestNavigation(expected.PageTag),
            Times.Once);
    }

}
