﻿using UI.Commands.Basic.Project;

namespace UI.ViewModels
{
    public class ProjectPathViewModel : Project
    {
        private string _currentGitBranch = "";

        public string CurrentGitBranch { get { return $"Current Git Branch: {_currentGitBranch}"; } set { _currentGitBranch = value; } }
        public static ProjectPathViewModel Transform(Project from, string repoName)
        {
            return new()
            {
                Id = from.Id,
                IDEPathId = from.IDEPathId,
                Name = from.Name,
                Path = from.Path,
                SortId = from.SortId,
                Filename = from.Filename,
                CurrentGitBranch = repoName,
                GroupId = from.GroupId,
            };
        }

        public static Project Transform(ProjectPathViewModel from)
        {
            return new()
            {
                Id = from.Id,
                IDEPathId = from.IDEPathId,
                Name = from.Name,
                Path = from.Path,
                Filename = from.Filename,
                GroupId = from.GroupId,
            };
        }
    }
}
