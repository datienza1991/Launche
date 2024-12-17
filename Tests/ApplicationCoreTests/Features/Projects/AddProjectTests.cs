using ApplicationCore.Common;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects;

public class AddProjectAppTests
{
    [Fact]
    public async Task HandleAsync_AddProject_Success()
    {
        // Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.Add(It.IsAny<Project>())).ReturnsAsync(true);

        var sut = new AddProjectService(
            stubRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(
            new(Name: "Name", Path: "Path", IDEPathId: 1, FileName: It.IsAny<string>())
        );

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task HandleAsync_EmptyName_ReturnFalse()
    {
        // Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.Add(It.IsAny<Project>())).ReturnsAsync(true);

        var sut = new AddProjectService(
            stubRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(
            new(
                Name: "",
                Path: It.IsAny<string>(),
                IDEPathId: It.IsAny<int>(),
                FileName: It.IsAny<string>()
            )
        );

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public async Task HandleAsync_EmptyPath_ReturnFalse()
    {
        // Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.Add(It.IsAny<Project>())).ReturnsAsync(true);

        var sut = new AddProjectService(
            stubRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(
            new(
                Name: "Name",
                Path: It.IsAny<string>(),
                IDEPathId: It.IsAny<int>(),
                FileName: It.IsAny<string>()
            )
        );

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public async Task HandleAsync_NoDevAppId_ReturnFalse()
    {
        // Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.Add(It.IsAny<Project>())).ReturnsAsync(true);

        var sut = new AddProjectService(
            stubRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(
            new(Name: "Name", Path: "Path", IDEPathId: 0, FileName: It.IsAny<string>())
        );

        // Assert
        Assert.False(actual);
    }
}
