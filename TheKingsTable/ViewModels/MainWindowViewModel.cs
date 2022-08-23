using ReactiveUI;
using System;
using System.Collections.Generic;
using CaveStoryModdingFramework;
using TheKingsTable.Models;
using Avalonia.Interactivity;
using System.Reactive;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace TheKingsTable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ProjectFile? Project { get; private set; } = null;
        public EditorManager? EditorManager { get; private set; } = null;
        public MainWindowViewModel()
        {
            NewProjectCommand = ReactiveCommand.Create<RoutedEventArgs>(NewProject);
            LoadProjectCommand = ReactiveCommand.Create<RoutedEventArgs>(LoadProject);
            SaveProjectCommand = ReactiveCommand.Create<RoutedEventArgs>(SaveProject);

            ShowNewProjectWizard = new Interaction<Unit, WizardViewModel?>();
        }
        public Interaction<Unit, WizardViewModel?> ShowNewProjectWizard { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> NewProjectCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> LoadProjectCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> SaveProjectCommand { get; }

        private async Task<bool> ProjectOverwriteOK()
        {
            if (Project != null)
            {
                var okToOverwrite =
                    await CommonInteractions.IsOk.Handle(new Words(
                    "Warning!",
                    "You already have a project loaded! Are you sure you want to continue?"));
                return okToOverwrite;
            }
            return true;
        }
        public async void NewProject(RoutedEventArgs e)
        {
            if (await ProjectOverwriteOK())
            {
                var wizard = await ShowNewProjectWizard.Handle(new Unit());
                if (wizard != null)
                {
                    Project = wizard.Project;
                    if (wizard.SaveProject)
                        Project.Save(wizard.SavePath);
                    EditorManager = new EditorManager(Project);
                    await EditorManager.OpenStageTableEditor(Project.StageTables[Project.SelectedLayout.StageTables[0].Key]);
                }
            }
        }
        public async void LoadProject(RoutedEventArgs e)
        {
            if (await ProjectOverwriteOK())
            {
                var result = await CommonInteractions.BrowseToOpenFile.Handle(new FileSelection(
                    "Select Project",
                    new List<Tuple<string, string>>() { new Tuple<string, string>("Project Files", ProjectFile.Extension) },
                    ""
                    ));
                if (result != null)
                {
                    Project = ProjectFile.Load(result);
                    EditorManager = new EditorManager(Project);
                    await EditorManager.OpenStageTableEditor(Project.StageTables[Project.SelectedLayout.StageTables[0].Key]);
                }
            }
        }

        public async void SaveProject(RoutedEventArgs e)
        {
            if(Project != null)
            {
                var result = await CommonInteractions.BrowseToSaveFile.Handle(new FileSelection(
                    "Select save location",
                    new List<Tuple<string, string>>() { new Tuple<string, string>("Project Files", ProjectFile.Extension) },
                    ""
                    ));
                if(result != null)
                {
                    Project.Save(result);
                }
            }
        }
    }
}
