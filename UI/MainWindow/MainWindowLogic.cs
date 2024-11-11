using ApplicationCore.Features.Projects;
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
        ListViewItem? firstListViewItem = lvProjectPaths.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
        firstListViewItem?.Focus();
    }

    private void OpenAddToGroupDialog()
    {

    }

    private async Task SaveProject()
    {
        if (this.mainWindowViewModel!.SelectedProjectPath?.Id == 0)
        {
            var addResult = await this.addProjectService!.AddAsync
            (
                new
                (
                    this.mainWindowViewModel!.SelectedProjectPath!.Name,
                    this.mainWindowViewModel!.SelectedProjectPath!.Path,
                    this.mainWindowViewModel!.SelectedIdePath!.Id,
                    this.mainWindowViewModel!.SelectedProjectPath!.Filename
                )
            );

            if (addResult)
            {

                this.SearchProjectEvent.Invoke(this, EventArgs.Empty);
                SelectNewlyAddedItem();
            }
            return;
        }

        var editResult = await this.editProjectService!.Edit
        (
            new
            (
                this.mainWindowViewModel!.SelectedProjectPath!.Id,
                this.mainWindowViewModel!.SelectedProjectPath!.Name,
                this.mainWindowViewModel!.SelectedProjectPath!.Path,
                this.mainWindowViewModel!.SelectedIdePath!.Id,
                this.mainWindowViewModel!.SelectedProjectPath!.Filename
            )
        );

        if (editResult)
        {
            this.SearchProjectEvent.Invoke(this, EventArgs.Empty);
            SelectEditedItem();
        }
    }

    private void SelectEditedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths.Items.SourceCollection
                 .Cast<ProjectViewModel>()
                 .FirstOrDefault(projectPathsViewModel => projectPathsViewModel.Id == this.mainWindowViewModel?.SelectedProjectPath?.Id);

        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    private void SelectNewlyAddedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths.Items[^1];
        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    private void OpenProject()
    {
        var project = this.mainWindowViewModel!.SelectedProjectPath;

        if (project is null)
        {
            return;
        }

        this.openProjectDevAppService!.Handle
        (
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

