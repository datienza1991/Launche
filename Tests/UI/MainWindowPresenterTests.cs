using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Projects;
using Moq;
using UI.MainWindowx.Presenter;
using Xunit;

namespace Tests.UI;

public class MainWindowPresenterTests
{
    private readonly Mock<IMainWindowView> mockPresenter = new();
    private readonly Mock<IAddProjectToGroupService> mock = new();
    private readonly Mock<IRemoveProjectFromGroupService> mockRemoveProjectFromGroupService = new();

    [Fact]
    public void DeleteDevAppEvent_DevApp_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter
            .Setup(x => x.GetAllDevAppService!.Handle())
            .Returns(() => Task.FromResult(new GetAllDevAppViewModel() { DevApps = [] }));

        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new()
            {
                SelectedIdePath = new() { Id = 1 },
                IdePathsModels = [new()],
            }
        );

        mockPresenter
            .Setup(x => x.DeleteDevAppService!.Delete(It.IsAny<DeleteDevAppCommand>()))
            .Returns(() => Task.FromResult(true));

        // Act
        mockPresenter.Raise(v => v.DeleteDevAppEvent += null, EventArgs.Empty);

        // Assert
        Assert.Empty(mockPresenter.Object.MainWindowViewModel.IdePathsModels ?? [new()]);
    }

    [Fact]
    public void DeleteDevAppEvent_NoSelectedDevApp_ReturnsNotEmptyDevApps()
    {
        // Arrange
        SetupSut();

        mockPresenter.SetupProperty(x => x.MainWindowViewModel, new() { IdePathsModels = [new()] });

        // Act
        mockPresenter.Raise(v => v.DeleteDevAppEvent += null, EventArgs.Empty);

        // Assert
        Assert.NotEmpty(mockPresenter.Object.MainWindowViewModel.IdePathsModels ?? []);
    }

    [Fact]
    public void FocusOnListViewEvent_Success()
    {
        // Arrange
        SetupSut();

        // Act
        mockPresenter.Raise(v => v.FocusOnListViewEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(v => v.FocusOnListViewWhenArrowDown(), Times.Once());
    }

    [Fact]
    public void OpenProjectFolderWindowEvent_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter.Setup(v =>
            v.OpenProjectFolderWindowService!.Handle(It.IsAny<OpenProjectFolderWindowCommand>())
        );
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new() { SelectedProjectPath = new() }
        );

        // Act
        mockPresenter.Raise(v => v.OpenProjectFolderWindowEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(
            v =>
                v.OpenProjectFolderWindowService!.Handle(
                    It.IsAny<OpenProjectFolderWindowCommand>()
                ),
            Times.Once()
        );
    }

    [Fact]
    public void OpenProjectDevAppEvent_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new() { SelectedProjectPath = new() }
        );
        mockPresenter.Setup(v =>
            v.OpenProjectDevAppService!.Handle(It.IsAny<OpenProjectDevAppCommand>())
        );

        // Act
        mockPresenter.Raise(v => v.OpenProjectDevAppEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(
            v => v.OpenProjectDevAppService!.Handle(It.IsAny<OpenProjectDevAppCommand>()),
            Times.Once()
        );
    }

    [Fact]
    public void SaveProjectEvent_AddNewProject_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new()
            {
                SelectedProjectPath = new(),
                SelectedIdePath = new(),
                ProjectPathModels = [],
            }
        );
        mockPresenter
            .Setup(x => x.AddProjectService!.AddAsync(It.IsAny<AddProjectCommand>()))
            .ReturnsAsync(true);
        mockPresenter
            .Setup(v => v.SearchProjectService!.Handle(It.IsAny<SearchProjectQuery>()))
            .ReturnsAsync(new SearchProjectViewModel() { Projects = [new()] });

        // Act
        mockPresenter.Raise(v => v.SaveProjectEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(v => v.SelectNewlyAddedItem(), Times.Once());
        Assert.NotEmpty(mockPresenter.Object.MainWindowViewModel.ProjectPathModels ?? []);
    }

    [Fact]
    public void SaveProjectEvent_EditProject_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new()
            {
                SelectedProjectPath = new() { Id = 1 },
                SelectedIdePath = new(),
                ProjectPathModels = [],
            }
        );
        mockPresenter
            .Setup(x => x.EditProjectService!.Edit(It.IsAny<EditProjectCommand>()))
            .ReturnsAsync(true);
        mockPresenter
            .Setup(v => v.SearchProjectService!.Handle(It.IsAny<SearchProjectQuery>()))
            .ReturnsAsync(new SearchProjectViewModel() { Projects = [new()] });

        // Act
        mockPresenter.Raise(v => v.SaveProjectEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(v => v.SelectEditedItem(), Times.Once());
        Assert.NotEmpty(mockPresenter.Object.MainWindowViewModel.ProjectPathModels ?? []);
    }

    [Fact]
    public void SortDownProjectEvent_Project_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new()
            {
                SelectedProjectPath = new(),
                SelectedIdePath = new(),
                ProjectPathModels = [new() { Id = 1 }, new() { Id = 2 }],
            }
        );
        mockPresenter
            .Setup(x => x.SortDownProjectService!.Handle(It.IsAny<SortDownProjectCommand>()))
            .ReturnsAsync(true);
        mockPresenter
            .Setup(v => v.SearchProjectService!.Handle(It.IsAny<SearchProjectQuery>()))
            .ReturnsAsync(
                new SearchProjectViewModel() { Projects = [new() { Id = 2 }, new() { Id = 1 }] }
            );

        // Act
        mockPresenter.Raise(v => v.SortDownProjectEvent += null, EventArgs.Empty);

        // Assert
        Assert.Equal(2, mockPresenter.Object.MainWindowViewModel.ProjectPathModels![0].Id);
        Assert.Equal(1, mockPresenter.Object.MainWindowViewModel.ProjectPathModels![1].Id);
    }

    [Fact]
    public void SortUpProjectEvent_Project_Success()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new()
            {
                SelectedProjectPath = new(),
                SelectedIdePath = new(),
                ProjectPathModels = [new() { Id = 2 }, new() { Id = 1 }],
            }
        );
        mockPresenter
            .Setup(x => x.SortUpProjectService!.Handle(It.IsAny<SortUpProjectCommand>()))
            .ReturnsAsync(true);
        mockPresenter
            .Setup(v => v.SearchProjectService!.Handle(It.IsAny<SearchProjectQuery>()))
            .ReturnsAsync(
                new SearchProjectViewModel() { Projects = [new() { Id = 1 }, new() { Id = 2 }] }
            );

        // Act
        mockPresenter.Raise(v => v.SortUpProjectEvent += null, EventArgs.Empty);

        // Assert
        Assert.Equal(1, mockPresenter.Object.MainWindowViewModel.ProjectPathModels![0].Id);
        Assert.Equal(2, mockPresenter.Object.MainWindowViewModel.ProjectPathModels![1].Id);
    }

    [Fact]
    public void SortDownProjectEvent_NoSelectedProject_ShowNoSelectedProjectMessage()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new() { SelectedProjectPath = new() }
        );

        mockPresenter
            .Setup(x => x.SortDownProjectService!.Handle(It.IsAny<SortDownProjectCommand>()))
            .ReturnsAsync(false);

        // Act
        mockPresenter.Raise(v => v.SortDownProjectEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(x => x.ShowNoSelectedProjectMessage(), Times.Once());
    }

    [Fact]
    public void SortUpProjectEvent_NoSelectedProject_ShowNoSelectedProjectMessage()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new() { SelectedProjectPath = new() }
        );

        mockPresenter
            .Setup(x => x.SortUpProjectService!.Handle(It.IsAny<SortUpProjectCommand>()))
            .ReturnsAsync(false);

        // Act
        mockPresenter.Raise(v => v.SortUpProjectEvent += null, EventArgs.Empty);

        // Assert
        mockPresenter.Verify(x => x.ShowNoSelectedProjectMessage(), Times.Once());
    }

    [Fact]
    public void SelectProjectEvent_Project_ReturnsCorrectData()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(
            x => x.MainWindowViewModel,
            new() { IdePathsModels = [new() { Id = 1 }] }
        );

        mockPresenter
            .SetupGet(x => x.SelectedProject)
            .Returns(new ProjectViewModel { Id = 1, IDEPathId = 1 });

        mockPresenter
            .Setup(x => x.GetCurrentGitBranchService!.Handle(It.IsAny<GetCurrentGitBranchQuery>()))
            .Returns("CurrentGitBranch");

        // Act
        mockPresenter.Raise(v => v.SelectProjectEvent += null, EventArgs.Empty);

        // Assert
        Assert.Equal(1, mockPresenter.Object.MainWindowViewModel.SelectedProjectPath?.Id);
        Assert.Equal(
            "CurrentGitBranch",
            mockPresenter.Object.MainWindowViewModel.SelectedProjectPath?.CurrentGitBranch
        );
        Assert.Equal(1, mockPresenter.Object.MainWindowViewModel.SelectedIdePath?.Id);
    }

    [Fact]
    public void SearchProjectEvent_Project_ReturnsCorrectData()
    {
        // Arrange
        SetupSut();
        mockPresenter.SetupProperty(x => x.MainWindowViewModel, new() { ProjectPathModels = [] });

        mockPresenter
            .Setup(x => x.SearchProjectService!.Handle(It.IsAny<SearchProjectQuery>()))
            .ReturnsAsync(new SearchProjectViewModel() { Projects = [new() { Id = 1 }] });

        // Act
        mockPresenter.Raise(v => v.SearchProjectEvent += null, EventArgs.Empty);

        // Assert
        Assert.Equal(1, mockPresenter.Object.MainWindowViewModel.ProjectPathModels?[0].Id);
    }

    private void SetupSut()
    {
        mockPresenter.Setup(x => x.AddProjectToGroupService).Returns(mock.Object);
        mockPresenter
            .Setup(x => x.RemoveProjectFromGroupService)
            .Returns(mockRemoveProjectFromGroupService.Object);

        _ = new MainWindowPresenter(mockPresenter.Object);
    }
}
