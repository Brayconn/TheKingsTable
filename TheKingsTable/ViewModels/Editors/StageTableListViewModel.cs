using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaveStoryModdingFramework.Stages;
using System.Collections.ObjectModel;
using Avalonia.Controls.Selection;
using CaveStoryModdingFramework;
using NP.Avalonia.UniDockService;
using TheKingsTable.Models;
using ReactiveUI;
using Avalonia.Interactivity;
using System.Reactive;

namespace TheKingsTable.ViewModels.Editors
{
    internal class StageTableListDockItemViewModel : DockItemViewModel<StageTableListViewModel> { }
    public class StageTableListViewModel : ViewModelBase
    {
        EditorManager Parent;
        public StageTableLocation Location { get; }
        public List<StageTableEntry> Stages { get; }
        public SelectionModel<StageTableEntry> Selection { get; }

        public ReactiveCommand<RoutedEventArgs, Unit> OpenStageCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> OpenScriptCommand { get; }
        public StageTableListViewModel(EditorManager parent, StageTableLocation location)
        {
            Parent = parent;
            Location = location;
            Selection = new SelectionModel<StageTableEntry>();

            Stages = location.Read();

            OpenStageCommand = ReactiveCommand.Create<RoutedEventArgs>(OpenStage);
            OpenScriptCommand = ReactiveCommand.Create<RoutedEventArgs>(OpenScript);
        }
        public async void OpenStage(RoutedEventArgs e)
        {
            await Parent.OpenStageEditor(Selection.SelectedItems.First());
        }
        public async void OpenScript(RoutedEventArgs e)
        {
            await Parent.OpenScriptEditor(Selection.SelectedItems.First());
        }
    }
}
