using System.Windows.Controls;
using System.Windows.Input;

namespace UI;

public partial class MainWindow
{
    private void OpenProjectWhenEnter(KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        OpenProject();
    }

    private void FocusOnListViewWhenArrowDown(KeyEventArgs e)
    {
        if (e.Key != Key.Down)
        {
            return;
        }

        if (lvProjectPaths.Items.Count == 0)
        {
            return;
        }

        lvProjectPaths.Focus();
        var item = lvProjectPaths.Items[0];
        lvProjectPaths.SelectedItem = item;
        ListViewItem? firstListViewItem =
            lvProjectPaths.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
        firstListViewItem?.Focus();
    }

    private void OpenProject()
    {
        var project = this.mainWindowViewModel!.SelectedProjectPath;

        if (project is null)
        {
            return;
        }

        this.openProjectDevAppService!.Handle(
            new()
            {
                DevAppPath = project.DevAppPath,
                DirectoryPath = project.Path,
                FullFilePath = project.FullPath,
                HasFileName = project.HasFileName,
            }
        );
    }
}
