using ApplicationCore.Features.Projects;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects;

public class DeleteProjectServiceTests
{
    [Fact]
    public async Task HandleAsync_DeleteProject_Success()
    {
        // Arrange
        var stubRepository = new Mock<IProjectRepository>();
        stubRepository.Setup(x => x.Delete(It.IsAny<long>())).ReturnsAsync(true);
        var sut = new DeleteProjectService(stubRepository.Object);

        // Act
        var actual = await sut.HandleAsync(It.IsAny<long>());

        // Assert
        Assert.True(actual);
    }
}
