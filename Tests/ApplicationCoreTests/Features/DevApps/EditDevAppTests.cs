using ApplicationCore.Features.DevApps;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.DevApps;

public class EditDevAppTests
{
    [Fact]
    public async Task HandleAsync_EditDevApp_Success()
    {
        // Arrange
        var stubDevAppRepository = new Mock<IDevAppRepository>();

        stubDevAppRepository.Setup(x => x.Edit(It.IsAny<IDEPath>())).ReturnsAsync(true);

        var sut = new EditDevAppService(stubDevAppRepository.Object);

        // Act
        var actual = await sut.HandleAsync(
            new() { Id = It.IsAny<int>(), Path = It.IsAny<string>() }
        );

        // Assert
        Assert.True(actual);
    }
}
