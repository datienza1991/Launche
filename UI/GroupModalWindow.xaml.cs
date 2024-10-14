using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using System.Windows;
using UI.Windows.Group.ViewModels;

namespace UI.Windows.Group;

/// <summary>
/// Interaction logic for GroupModalWindow.xaml
/// </summary>
public partial class GroupModalWindow : Window
{
    public ProjectViewModel? ProjectPath { get; set; }
    public event EventHandler? OnSave;

    private readonly GroupWindowDataContext dataContext = new();
    private readonly IGetAllGroupService? getAllGroupService;
    private readonly IAddProjectToGroupService? addProjectToGroupService;
    private readonly IRemoveProjectFromGroupService? removeProjectToGroupService;

    public GroupModalWindow()
    {
        InitializeComponent();
        DataContext = dataContext;
    }

    public GroupModalWindow
    (
        IGetAllGroupService getAllGroupService,
        IAddProjectToGroupService addProjectToGroupService,
        IRemoveProjectFromGroupService removeProjectFromGroupService
    )
    {
        InitializeComponent();
        DataContext = dataContext;

        this.getAllGroupService = getAllGroupService;
        this.addProjectToGroupService = addProjectToGroupService;
        removeProjectToGroupService = removeProjectFromGroupService;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var getAllGroupVm = await getAllGroupService!.Handle(new()); ;
        dataContext.Groups = [.. getAllGroupVm.Groups];
        this.ListBoxGroup.SelectedItem = this.ListBoxGroup.Items.SourceCollection
                 .Cast<GroupViewModel>()
                 .FirstOrDefault(groupViewModel => groupViewModel.Id == ProjectPath?.GroupId);

        dataContext.EnableReset = ProjectPath!.EnabledGroupReset;
        dataContext.EnableSave = false;
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (dataContext.SelectedOption is null)
        {
            return;
        }

        if (ProjectPath == null)
        {
            return;
        }

        var groupId = dataContext.SelectedOption.Id;

        await addProjectToGroupService!.Handle(new() { ProjectId = ProjectPath.Id, GroupId = groupId });

        OnSave?.Invoke(this, EventArgs.Empty);
    }

    private async void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        if (ProjectPath == null)
        {
            return;
        }

        await removeProjectToGroupService!.Handle(new() { ProjectId = ProjectPath.Id });
        this.ListBoxGroup.SelectedItem = null;
        dataContext.EnableSave = false;
        OnSave?.Invoke(this, EventArgs.Empty);
    }

    private void ListBoxGroup_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        dataContext.EnableSave = true;
    }
}

