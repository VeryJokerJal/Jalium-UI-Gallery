using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Services.Interfaces;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.ViewModels;

/// <summary>
/// Fixture for <see cref="MainWindowViewModel"/>. Its service dependencies are
/// mocked so shell behavior is exercised without constructing a Window.
/// </summary>
public class MainWindowViewModelFixture
{
    private const string MessageServiceDefaultMessage = "Some Value";

    private static Mock<IMessageService> BuildMessageService(string message)
    {
        var messageService = new Mock<IMessageService>();
        messageService.Setup(x => x.GetMessage()).Returns(message);
        return messageService;
    }

    [Fact]
    public void Constructor_ShouldSeedMessageFromService()
    {
        var messageService = BuildMessageService(MessageServiceDefaultMessage);
        var themeService = Mock.Of<IGalleryThemeService>();

        var vm = CreateViewModel(
            messageService.Object,
            themeService);

        messageService.Verify(x => x.GetMessage(), Times.Once);
        Assert.Equal(MessageServiceDefaultMessage, vm.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenMessageServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => new MainWindowViewModel(
                null!,
                Mock.Of<IGalleryThemeService>(),
                Mock.Of<IGalleryLocalizationService>(),
                Mock.Of<IGalleryNavigationCoordinator>(),
                Mock.Of<IExternalUriService>(),
                new HomeViewModel(
                    Mock.Of<IGalleryNavigationCoordinator>())));
    }

    [Fact]
    public void ToggleThemeCommand_DelegatesToThemeService()
    {
        var themeService = new Mock<IGalleryThemeService>();
        var vm = CreateViewModel(
            BuildMessageService(MessageServiceDefaultMessage).Object,
            themeService.Object);

        vm.ToggleThemeCommand.Execute(null);

        themeService.Verify(service => service.Toggle(), Times.Once);
    }

    [Fact]
    public void ThemeChanged_RefreshesShellThemeState()
    {
        var isDark = false;
        var themeService = new Mock<IGalleryThemeService>();
        themeService.SetupGet(service => service.IsDark)
            .Returns(() => isDark);
        var vm = CreateViewModel(
            BuildMessageService(MessageServiceDefaultMessage).Object,
            themeService.Object);
        var changed = new List<string?>();
        vm.PropertyChanged += (_, args) =>
            changed.Add(args.PropertyName);

        isDark = true;
        themeService.Raise(
            service => service.ThemeChanged += null,
            EventArgs.Empty);

        Assert.True(vm.IsDarkTheme);
        Assert.False(vm.IsLightTheme);
        Assert.Equal("Dark theme", vm.ThemeLabel);
        Assert.Contains(nameof(vm.IsDarkTheme), changed);
        Assert.Contains(nameof(vm.IsLightTheme), changed);
        Assert.Contains(nameof(vm.ThemeLabel), changed);
        vm.Destroy();
    }

    private static MainWindowViewModel CreateViewModel(
        IMessageService messageService,
        IGalleryThemeService themeService)
    {
        var navigation =
            Mock.Of<IGalleryNavigationCoordinator>();
        return new MainWindowViewModel(
            messageService,
            themeService,
            Mock.Of<IGalleryLocalizationService>(),
            navigation,
            Mock.Of<IExternalUriService>(),
            new HomeViewModel(navigation));
    }
}
