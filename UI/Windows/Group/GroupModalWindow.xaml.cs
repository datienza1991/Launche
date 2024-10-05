using ApplicationCore.Features.Basic.Group;
using ApplicationCore.Features.Grouping;
using System.Windows;
using UI.Windows.Group.ViewModels;

namespace UI.Windows.Group;

/// <summary>
/// Interaction logic for GroupModalWindow.xaml
/// </summary>
public partial class GroupModalWindow : Window
{
    public Infrastructure.Models.Project? ProjectPath { get; set; }
    private readonly GroupWindowDataContext dataContext = new();
    private readonly GroupDataService? groupDataService;
    private readonly IGroupProject? projectGrouping;

    public event EventHandler? OnSave;
    public GroupModalWindow()
    {
        InitializeComponent();
        DataContext = dataContext;
    }

    public GroupModalWindow(GroupDataService groupQuery, IGroupProject projectGrouping)
    {
        InitializeComponent();
        DataContext = dataContext;
        groupDataService = groupQuery;
        this.projectGrouping = projectGrouping;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var groups = await groupDataService!.GetAll();
        dataContext.Groups =
        [..
            groups.Select
            (
                group => new GroupViewModel() { Id = group.Id, Name = group.Name }
            )
        ];
        this.ListBoxGroup.SelectedItem = this.ListBoxGroup.Items.SourceCollection
                 .Cast<GroupViewModel>()
                 .FirstOrDefault(groupViewModel => groupViewModel.Id == ProjectPath?.GroupId);

        dataContext.EnableSave = ProjectPath!.GroupId is not null;
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

        ProjectPath.GroupId = dataContext.SelectedOption.Id;

        await projectGrouping!.Group(ProjectPath.Id, ProjectPath.GroupId);

        OnSave?.Invoke(this, EventArgs.Empty);
    }

    private async void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        if (ProjectPath == null)
        {
            return;
        }

        ProjectPath.GroupId = null;
        await projectGrouping!.Group(ProjectPath.Id, ProjectPath.GroupId);
        this.ListBoxGroup.SelectedItem = null;
        dataContext.EnableSave = false;
    }

    private void ListBoxGroup_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        dataContext.EnableSave = true;
    }
}

