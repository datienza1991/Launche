using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.DevApps;

public class AddDevAppTests
{
    [Fact]
    public async Task HandleAsync_AddDevApp_Success()
    {
        // Arrange
        var stubDevAppRepository = new Mock<IDevAppRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubDevAppRepository.Setup(x => x.Add(It.IsAny<IDEPath>())).ReturnsAsync(true);

        var sut = new AddDevAppService(
            stubDevAppRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(new() { Path = "Path" });

        // Assert
        Assert.True(actual);
    }
}
