using ApplicationCore.Features.DevApps;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.DevApps;

public class GetOneDevAppTests
{
    [Fact]
    public async Task HandleAsync_GetOneDevApp_Success()
    {
        // Arrange
        var stubDevAppRepository = new Mock<IDevAppRepository>();

        stubDevAppRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(new IDEPath());

        var sut = new GetOneDevAppService(stubDevAppRepository.Object);

        // Act
        var actual = await sut.HandleAsync(new());

        // Assert
        Assert.NotNull(actual);
    }
}
