using ApplicationCore.Common;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects;

public class AddProjectToGroupTests
{
    [Fact]
    public async Task HandleAsync_AddProjectToGroup_Success()
    {
        /// Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubGroupRepository = new Mock<IGroupRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.GetOne(It.IsAny<long>())).ReturnsAsync(new Project());
        stubRepository.Setup(x => x.Edit(It.IsAny<Project>())).ReturnsAsync(true);
        stubGroupRepository.Setup(x => x.GetOne(It.IsAny<long>())).ReturnsAsync(new Group());

        var sut = new AddProjectToGroupService(
            stubGroupRepository.Object,
            stubRepository.Object,
            stubNotificationMessageService.Object
        );
        sut.Notify += (sender, args) => { };

        // Act
        var actual = await sut.HandleAsync(new());

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task HandleAsync_NoProjectFound_Failed()
    {
        /// Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubGroupRepository = new Mock<IGroupRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.GetOne(It.IsAny<long>()));

        var sut = new AddProjectToGroupService(
            stubGroupRepository.Object,
            stubRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(new());

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public async Task HandleAsync_NoGroupFound_Failed()
    {
        /// Arrange
        var stubRepository = new Mock<IProjectRepository>();
        var stubGroupRepository = new Mock<IGroupRepository>();
        var stubNotificationMessageService = new Mock<INotificationMessageService>();

        stubRepository.Setup(x => x.GetOne(It.IsAny<long>())).ReturnsAsync(new Project());
        stubRepository.Setup(x => x.Edit(It.IsAny<Project>())).ReturnsAsync(true);
        stubGroupRepository.Setup(x => x.GetOne(It.IsAny<long>()));

        var sut = new AddProjectToGroupService(
            stubGroupRepository.Object,
            stubRepository.Object,
            stubNotificationMessageService.Object
        );

        // Act
        var actual = await sut.HandleAsync(new());

        // Assert
        Assert.False(actual);
    }
}
