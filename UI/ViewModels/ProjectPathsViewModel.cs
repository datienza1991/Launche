﻿using UI.Commands.Basic.Project;

namespace UI.ViewModels
{
    public class ProjectPathsViewModel : Project
    {
        public int Index { get; set; }
        public bool EnableMoveUp { get; set; }
        public bool EnableMoveDown { get; set; }
        public bool EnableAddToGroup { get; set; }
        public string? GroupName { get; set; }
    }
}
