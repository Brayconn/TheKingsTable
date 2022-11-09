using CaveStoryModdingFramework;
using NP.Avalonia.UniDockService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKingsTable.ViewModels.Editors
{
    internal class ProjectEditorDockItemViewModel : DockItemViewModel<ProjectEditorViewModel> { }
    public class ProjectEditorViewModel : ViewModelBase
    {
        ProjectFile Project { get; set; }

        public ProjectEditorViewModel(ProjectFile project)
        {
            Project = project;
        }
    }
}
