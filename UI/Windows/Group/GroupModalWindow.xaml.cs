using System.Windows;
using UI.Group;
using UI.ProjectPath;
using UI.Windows.Group.ViewModels;

namespace UI;

/// <summary>
/// Interaction logic for GroupModalWindow.xaml
/// </summary>
public partial class GroupModalWindow : Window
{
    public ProjectPath.Project? ProjectPath { get; set; }
    private readonly GroupWindowDataContext dataContext = new();
    private readonly IGetAll? getAll;
    private readonly IEditProjectPath? editProjectPath;

    public event EventHandler? OnSave;
    public GroupModalWindow()
    {
        InitializeComponent();
        DataContext = dataContext;
    }

    public GroupModalWindow(IGetAll getAll, IEditProjectPath editProjectPath)
    {
        InitializeComponent();
        DataContext = dataContext;
        this.getAll = getAll;
        this.editProjectPath = editProjectPath;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var groups = await getAll!.ExecuteAsync();
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

        await this.editProjectPath!.ExecuteAsync(this.ProjectPath);

        this.OnSave?.Invoke(this, EventArgs.Empty);
    }

    private async void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        if (this.ProjectPath == null)
        {
            return;
        }

        this.ProjectPath.GroupId = null;
        await this.editProjectPath!.ExecuteAsync(this.ProjectPath);
        this.ListBoxGroup.SelectedItem = null;
        this.dataContext.EnableSave = false;
    }

    private void ListBoxGroup_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        this.dataContext.EnableSave = true;
    }
}

