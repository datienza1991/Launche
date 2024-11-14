using ApplicationCore.Features.Projects;
using Moq;
using UI.MainWindowx.Presenter;
using Xunit;

namespace Tests.UI;

public class MainWindowPresenterTests
{
    private readonly Mock<IMainWindowView> mockPresenter = new();
    private readonly Mock<IAddProjectToGroupService> mock = new();
    private MainWindowPresenter sut;

    [Fact]
    public void FirstTest()
    {
        SetupView();
        SetupSut();
        mockPresenter.Raise(v => v.DeleteDevAppEvent += null, EventArgs.Empty);
    }

    private void SetupSut()
    {
        sut = new MainWindowPresenter(mockPresenter.Object);
    }

    private void SetupView()
    {
        mockPresenter.Setup(x => x.AddProjectToGroupService).Returns(mock.Object);
    }
}
