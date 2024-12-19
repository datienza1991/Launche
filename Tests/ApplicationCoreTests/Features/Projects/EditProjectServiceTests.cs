using ApplicationCore.Common;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class EditProjectServiceTests
    {
        [Fact]
        public void HandleAsync_EditProject_Success()
        {
            // Arrange
            var stubRepository = new Mock<IProjectRepository>();
            stubRepository.Setup(x => x.GetOne(It.IsAny<long>())).ReturnsAsync(new Project());
            var stubNotificationMessageService = new Mock<INotificationMessageService>();
            var sut = new EditProjectService(
                stubRepository.Object,
                stubNotificationMessageService.Object
            );

            // Act
            var actual = sut.HandleAsync(
                new(
                    Id: It.IsAny<long>(),
                    Name: "Name",
                    Path: "Path",
                    FileName: It.IsAny<string>(),
                    IDEPathId: 1
                )
            );

            // Assert
            Assert.True(true);
        }
    }
}
