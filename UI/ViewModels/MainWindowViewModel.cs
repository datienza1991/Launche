using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Infrastructure.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            this.ProjectPathModels = [];
            this.IdePathsModels = [];
            this.SelectedProjectPath = null;
            this.SelectedIdePath = new();
            this.search = "";
        }

        private bool _enableAddNewProject = true;

        public bool EnableAddNewProject
        {
            get { return _enableAddNewProject; }
            set
            {
                _enableAddNewProject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.EnableAddNewProject)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ProjectViewModel>? projectPathModels;

        public ObservableCollection<ProjectViewModel>? ProjectPathModels
        {
            get { return projectPathModels; }
            set
            {
                projectPathModels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPathModels)));
            }
        }

        private ObservableCollection<IDEPathViewModel>? idePathsModels;

        public ObservableCollection<IDEPathViewModel>? IdePathsModels
        {
            get { return idePathsModels; }
            set
            {
                idePathsModels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IdePathsModels)));
            }
        }

        private ProjectViewModel? selectedProjectPath;

        public ProjectViewModel? SelectedProjectPath
        {
            get { return selectedProjectPath; }
            set
            {
                selectedProjectPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedProjectPath)));
            }
        }

        private IDEPathViewModel? selectedIdePath;

        public IDEPathViewModel? SelectedIdePath
        {
            get { return selectedIdePath; }
            set
            {
                selectedIdePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedIdePath)));
            }
        }

        private string search;
        public string Search
        {
            get { return search; }
            set
            {
                search = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Search)));
            }
        }
    }
}
