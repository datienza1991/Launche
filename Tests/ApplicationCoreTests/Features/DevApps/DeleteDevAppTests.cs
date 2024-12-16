using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.DevApps;

public class DeleteDevAppTests
{
    [Fact]
    public async Task HandleAsync_DeleteDevApp_Success()
    {
        // Arrange
        var stubDevAppRepository = new Mock<IDevAppRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubDevAppRepository.Setup(x => x.Delete(It.IsAny<long>())).ReturnsAsync(true);

        var sut = new DeleteDevAppService(stubDevAppRepository.Object);

        // Act
        var actual = await sut.HandleAsync(new() { Id = It.IsAny<long>() });

        // Assert
        Assert.True(actual);
    }
}
