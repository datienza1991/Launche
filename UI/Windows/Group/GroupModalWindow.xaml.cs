using System.Windows;
using UI.Group;
using UI.Windows.Group.ViewModels;

namespace UI;

/// <summary>
/// Interaction logic for GroupModalWindow.xaml
/// </summary>
public partial class GroupModalWindow : Window
{
    private readonly GroupWindowDataContext dataContext = new();
    private readonly IGetAll? getAll;
    public event EventHandler<GroupModalEventArgs>? OnSave;
    public GroupModalWindow()
    {
        InitializeComponent();
        DataContext = dataContext;
    }

    public GroupModalWindow(IGetAll getAll)
    {
        InitializeComponent();
        DataContext = dataContext;
        this.getAll = getAll;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {

        GroupViewModel item2 = new() { Name = "Item2" };
        var groups = await getAll!.ExecuteAsync();
        dataContext.Groups =
        [..
            groups.Select
            (
                group => new GroupViewModel() { Id = group.Id, Name = group.Name }
            )
        ];
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (dataContext.SelectedOption is null)
        {
            return;
        }

        this.OnSave?.Invoke(this, new GroupModalEventArgs { GroupId = dataContext.SelectedOption.Id });
    }
}

public class GroupModalEventArgs : EventArgs
{
    public int GroupId { get; set; }
}
