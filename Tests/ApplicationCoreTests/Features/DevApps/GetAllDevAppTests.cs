using ApplicationCore.Features.DevApps;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.DevApps;

public class GetAllDevAppTests
{
    [Fact]
    public async Task HandleAsync_GetAllDevApp_Success()
    {
        // Arrange
        var stubDevAppRepository = new Mock<IDevAppRepository>();

        stubDevAppRepository.Setup(x => x.GetAll()).ReturnsAsync([new()]);

        var sut = new GetAllDevAppService(stubDevAppRepository.Object);

        // Act
        var actual = await sut.HandleAsync();

        // Assert
        Assert.NotEmpty(actual.DevApps);
    }
}
