using System.Windows;
using UI.Commands.Basic.Project;
using UI.Queries.Group;
using UI.Windows.Group.ViewModels;

namespace UI;

/// <summary>
/// Interaction logic for GroupModalWindow.xaml
/// </summary>
public partial class GroupModalWindow : Window
{
    public Project? ProjectPath { get; set; }
    private readonly GroupWindowDataContext dataContext = new();
    private readonly GroupQuery? groupQuery;
    private readonly Commands.Grouping.IProjectCommand? projectGrouping;

    public event EventHandler? OnSave;
    public GroupModalWindow()
    {
        InitializeComponent();
        DataContext = dataContext;
    }

    public GroupModalWindow(Queries.Group.GroupQuery groupQuery, UI.Commands.Grouping.IProjectCommand projectGrouping)
    {
        InitializeComponent();
        DataContext = dataContext;
        this.groupQuery = groupQuery;
        this.projectGrouping = projectGrouping;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var groups = await groupQuery!.GetAll();
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

        this.dataContext.EnableSave = ProjectPath!.GroupId is not null;
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (dataContext.SelectedOption is null)
        {
            return;
        }

        if (this.ProjectPath == null)
        {
            return;
        }

        this.ProjectPath.GroupId = dataContext.SelectedOption.Id;

        await this.projectGrouping!.Group(this.ProjectPath.Id, this.ProjectPath.GroupId);

        this.OnSave?.Invoke(this, EventArgs.Empty);
    }

    private async void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        if (this.ProjectPath == null)
        {
            return;
        }

        this.ProjectPath.GroupId = null;
        await this.projectGrouping!.Group(this.ProjectPath.Id, this.ProjectPath.GroupId);
        this.ListBoxGroup.SelectedItem = null;
        this.dataContext.EnableSave = false;
    }

    private void ListBoxGroup_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        this.dataContext.EnableSave = true;
    }
}

