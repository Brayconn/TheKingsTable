using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CaveStoryModdingFramework;
using NP.Avalonia.UniDockService;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using TheKingsTable.Models;
using TheKingsTable.ViewModels;
using TheKingsTable.ViewModels.Editors;

namespace TheKingsTable.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        IUniDockService UniDockService;
        const string ToolGroup = "ToolGroup";
        const string EditorGroup = "EditorGroup";
        const string DockManager = "TheDockManager";

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            UniDockService = (IUniDockService)this.FindResource(DockManager)!;
            UniDockService.DockItemsViewModels = new ObservableCollection<DockItemViewModelBase>();
            
            this.WhenActivated(d => d(ViewModel!.ShowNewProjectWizard
                .RegisterHandler(DoShowWizard)));

            this.WhenActivated(d => d(EditorManager.StageTableEditorOpened
                .RegisterHandler(AddStageTableEditor)));
            this.WhenActivated(d => d(EditorManager.StageEditorOpened
                .RegisterHandler(AddStageEditor)));
            this.WhenActivated(d => d(EditorManager.ScriptEditorOpened
                .RegisterHandler(AddScriptEditor)));
            this.WhenActivated(d => d(EditorManager.ProjectClosing
                .RegisterHandler(CloseProject)));


            this.WhenActivated(d => d(CommonInteractions.BrowseToOpenFile.
                RegisterHandler(x => CommonAvaloniaHandlers.ShowOpenFileBrowser(x, this))));
            this.WhenActivated(d => d(CommonInteractions.BrowseForFolder.
                RegisterHandler(x => CommonAvaloniaHandlers.ShowFolderBrowser(x, this))));
            this.WhenActivated(d => d(CommonInteractions.BrowseToSaveFile.
                RegisterHandler(x => CommonAvaloniaHandlers.ShowSaveFileBrowser(x, this))));
            this.WhenActivated(d => d(CommonInteractions.IsOk
                .RegisterHandler(x => CommonAvaloniaHandlers.ShowYesNoMessage(x, this))));

            Button.ClickEvent.AddClassHandler<MainWindow>((x, e) =>
            {
                if ((e.Source as Button)?.Name == "CloseButton")
                {
                    var t = e.RoutedEvent.EventArgsType;
                    
                    //TODO hook into FloatingWindow.Closing?????
                }
            });
        }

        private void CloseProject(InteractionContext<Unit, bool> obj)
        {
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task AddStageTableEditor(InteractionContext<StageTableListViewModel, Unit> context)
        {
            UniDockService.DockItemsViewModels.Add(new StageTableListDockItemViewModel()
            {
                //TODO this is an awful ID
                DockId = context.Input.Location.Filename,
                TheVM = context.Input,
                DefaultDockGroupId = EditorGroup,
                DefaultDockOrderInGroup = 0,
                HeaderContentTemplateResourceKey = "StageTableHeader",
                ContentTemplateResourceKey = "StageTableListView",
                IsSelected = true,
                IsActive = true,
                IsPredefined = false,
            });
            
            context.SetOutput(new Unit());
        }
        private async Task AddStageEditor(InteractionContext<StageEditorViewModel, Unit> context)
        {
            UniDockService.DockItemsViewModels.Add(new StageEditorDockItemViewModel()
            {
                //TODO this is an awful ID
                DockId = context.Input.Entry.GetHashCode().ToString(),
                TheVM = context.Input,
                DefaultDockGroupId = ToolGroup,
                DefaultDockOrderInGroup = 0,
                HeaderContentTemplateResourceKey = "StageEditorHeaderDataTemplate",
                ContentTemplateResourceKey = "StageEditorDataTemplate",
                IsSelected = true,
                IsActive = true,
                IsPredefined = false,
            });
            context.SetOutput(new Unit());
        }
        private async Task AddScriptEditor(InteractionContext<TextScriptEditorViewModel, Unit> context)
        {           
            UniDockService.DockItemsViewModels.Add(new ScriptEditorDockItemViewModel()
            {
                //TODO this is an awful ID
                DockId = context.Input.TSCPath.GetHashCode().ToString(),
                TheVM = context.Input,
                DefaultDockGroupId = ToolGroup,
                DefaultDockOrderInGroup = 0,
                HeaderContentTemplateResourceKey = "ScriptEditorHeaderDataTemplate",
                ContentTemplateResourceKey = "ScriptEditorDataTemplate",
                IsSelected = true,
                IsActive = true,
                IsPredefined = false,
            });
            context.SetOutput(new Unit());
        }

        private async Task DoShowWizard(InteractionContext<Unit, WizardViewModel> context)
        {
            var dialog = new WizardWindow();
            var vm = new WizardWindowViewModel();
            dialog.DataContext = vm;
            dialog.ViewModel = vm;
            var result = await dialog.ShowDialog<WizardViewModel>(this);
            context.SetOutput(result);
        }

        private async Task DoShowProjectEditor(InteractionContext<ProjectEditorViewModel, Unit> context)
        {

            context.SetOutput(new Unit());
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //see https://github.com/npolyak/NP.Avalonia.UniDock/issues/8
            //tl;dr avalonia moment
            Environment.Exit(0);
        }
    }
}
