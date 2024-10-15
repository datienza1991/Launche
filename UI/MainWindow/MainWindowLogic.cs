using ApplicationCore.Features.Projects;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Windows.Group;

namespace UI;

public partial class MainWindow
{
    private async Task OpenDevAppDialog()
    {
        var openFolderDialog = new OpenFileDialog
        {
            Filter = "Executable Files | *.exe"
        };
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        string filePath = openFolderDialog.FileName;
        var resultSave = await this.addDevAppService!.Add(new() { Path = filePath });
        if (resultSave)
        {
            await this.FetchDevApps();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
    }

    private async Task FetchProjects()
    {
        var getAllProjectVm = await this.getAllProjectService!.Handle();
        this.mainWindowViewModel!.ProjectPathModels = [.. getAllProjectVm.Projects];
    }

    private async Task FetchDevApps()
    {
        var getAllDevAppVm = await this.getAllDevAppService!.Handle();
        this.mainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
    }

    private async Task Search()
    {
        var searchViewModel = await this.searchProductService!.Handle(new() { Search = this.mainWindowViewModel!.Search });
        this.mainWindowViewModel.EnableAddNewProject = searchViewModel.EnableAddNewProject;
        this.mainWindowViewModel!.ProjectPathModels = [.. searchViewModel.Projects];

    }

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
        this.groupModalWindow = new GroupModalWindow
        (
            getAllGroupService!,
            addProjectToGroupService!,
            removeProjectFromGroupService!
        );

        this.groupModalWindow!.ProjectPath = this.mainWindowViewModel!.SelectedProjectPath;
        groupModalWindow?.ShowDialog();
    }

    private async Task DeleteProject()
    {
        if (this.mainWindowViewModel!.SelectedProjectPath!.Id == 0) return;

        var result = await this.deleteProjectService!.Delete(this.mainWindowViewModel!.SelectedProjectPath!.Id);

        if (result)
        {
            await Search();
            this.mainWindowViewModel!.SelectedProjectPath = new();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
    }
    private async Task SortUpProject()
    {
        var result = await this.sortUpProjectService!.Handle(new() { SortId = this.mainWindowViewModel!.SelectedProjectPath!.SortId });
        if (!result)
        {
            return;
        }

        await this.FetchProjects();
    }

    private async Task SortDownProject()
    {
        var result = await this.sortDownProjectService!.Handle(new() { SortId = this.mainWindowViewModel!.SelectedProjectPath!.SortId });
        if (!result)
        {
            return;
        }
        await this.FetchProjects();
    }

    private void SelectProject()
    {
        if (lvProjectPaths.SelectedIndex == -1) { return; }
        var project = (ProjectViewModel)lvProjectPaths.SelectedItem;
        var currentGitBranch = getCurrentGitBranchService!.Handle(new() { DirectoryPath = project.Path });
        project.CurrentGitBranch = currentGitBranch;
        this.mainWindowViewModel!.SelectedProjectPath = project.Copy();

        // Selected Dev App : Must be same reference, data must be on view model list to set the value
        this.mainWindowViewModel!.SelectedIdePath = this.mainWindowViewModel!.IdePathsModels!.First(x => x.Id == project.IDEPathId);
    }

    private void OpenProjectDialog()
    {
        var openFolderDialog = new OpenFolderDialog();
        var result = openFolderDialog.ShowDialog() ?? false;

        if (result)
        {
            string filePath = openFolderDialog.FolderName;
            string name = openFolderDialog.SafeFolderName;
            var project = this.mainWindowViewModel!.SelectedProjectPath!.Copy();
            project.Name = name;
            project.Path = filePath;
            this.mainWindowViewModel!.SelectedProjectPath = project;
        }
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

                await this.Search();
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
            await this.Search();
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

    private async Task DeleteDevApp()
    {
        if (this.mainWindowViewModel!.SelectedIdePath!.Id == 0) return;

        var result = await this.deleteDevAppService!.Delete(new() { Id = this.mainWindowViewModel!.SelectedIdePath!.Id });

        if (result)
        {
            await this.FetchDevApps();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
    }
}

