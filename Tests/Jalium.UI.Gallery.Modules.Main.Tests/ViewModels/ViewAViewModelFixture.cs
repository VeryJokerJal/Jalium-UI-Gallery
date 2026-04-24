using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Services.Interfaces;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.ViewModels;

/// <summary>
/// Fixture for <see cref="ViewAViewModel"/>. Uses Moq to stand in for
/// <see cref="IMessageService"/> so the view model can be exercised in isolation.
/// </summary>
public class ViewAViewModelFixture
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

        var vm = new ViewAViewModel(messageService.Object);

        messageService.Verify(x => x.GetMessage(), Times.Once);
        Assert.Equal(MessageServiceDefaultMessage, vm.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenMessageServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ViewAViewModel(null!));
    }
}
