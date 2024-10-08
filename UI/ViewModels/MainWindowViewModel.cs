﻿using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            this.ProjectPathModels = [];
            this.IdePathsModels = [];
            this.SelectedProjectPath = new();
            this.SelectedIdePath = new();
            this.search = "";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ProjectPathsViewModel>? projectPathModels;

        public ObservableCollection<ProjectPathsViewModel>? ProjectPathModels
        {
            get { return projectPathModels; }
            set
            {
                projectPathModels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPathModels)));
            }
        }

        private ObservableCollection<IDEPathsViewModel>? idePathsModels;

        public ObservableCollection<IDEPathsViewModel>? IdePathsModels
        {
            get { return idePathsModels; }
            set
            {
                idePathsModels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IdePathsModels)));
            }
        }

        private ProjectPathViewModel? selectedProjectPath;

        public ProjectPathViewModel? SelectedProjectPath
        {
            get { return selectedProjectPath; }
            set
            {
                selectedProjectPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedProjectPath)));
            }
        }

        private IDEPathsViewModel? selectedIdePath;

        public IDEPathsViewModel? SelectedIdePath
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
