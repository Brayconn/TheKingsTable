using CaveStoryModdingFramework;
using CaveStoryModdingFramework.TSC;
using ReactiveUI;
using System;
using CaveStoryModdingFramework.Compatability;
using System.IO;
using Avalonia.Interactivity;
using System.Reactive;
using NP.Avalonia.UniDockService;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace TheKingsTable.ViewModels.Editors
{
    internal class ScriptEditorDockItemViewModel : DockItemViewModel<TextScriptEditorViewModel> { }
    public class TextScriptEditorViewModel : ViewModelBase
    {
        public ProjectFile Project { get; }
        public string TSCPath { get; private set; }
        string text = "";
        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }
        void Load(string tscPath)
        {
            byte[] bytes = Array.Empty<byte>();
            if (Project.UseScriptSource)
            {
                var sspath = ScriptSource.GetScriptSourcePath(tscPath);
                if(File.Exists(sspath))
                    bytes = File.ReadAllBytes(sspath);
            }
            if (bytes.Length <= 0 && File.Exists(tscPath))
            {
                bytes = File.ReadAllBytes(tscPath);
                if (Project.ScriptsEncrypted)
                    Encryptor.DecryptInPlace(bytes, Project.DefaultEncryptionKey);
            }
            if (bytes.Length > 0)
                Text = Project.ScriptEncoding.GetString(bytes);
            else
                Text = "";
        }
        void Save(string tscPath)
        {
            var bytes = Project.ScriptEncoding.GetBytes(Text);
            if(Project.UseScriptSource)
            {
                string ssdir = ScriptSource.GetScriptSourceDirectory(tscPath);
                if (!Directory.Exists(ssdir))
                    Directory.CreateDirectory(ssdir);
                File.WriteAllBytes(ScriptSource.GetScriptSourcePath(tscPath), bytes);
            }
            if (Project.ScriptsEncrypted)
                Encryptor.EncryptInPlace(bytes, Project.DefaultEncryptionKey);

            File.WriteAllBytes(tscPath, bytes);
        }
        async void SaveAs(RoutedEventArgs e)
        {
            //HACK send help all this code is garbage
            var path = await CommonInteractions.BrowseToSaveFile.Handle(new FileSelection(
                    "Select save location",
                    new List<Tuple<string, string>>() { new Tuple<string, string>("TSC Files", Project.ScriptExtension.Replace(".",""))
                    , new Tuple<string, string>("Text Files", "txt")},
                    ""
                    ));
            if(path != null)
            {
                var bytes = Project.ScriptEncoding.GetBytes(Text);
                if (path.EndsWith(Project.ScriptExtension) && Project.ScriptsEncrypted)
                    Encryptor.EncryptInPlace(bytes, Project.DefaultEncryptionKey);
                File.WriteAllBytes(path, bytes);
            }
        }
        public TextScriptEditorViewModel(ProjectFile project, string tscPath)
        {
            Project = project;
            TSCPath = tscPath;

            SaveCommand = ReactiveCommand.Create<RoutedEventArgs>(e => Save(TSCPath));
            SaveAsCommand = ReactiveCommand.Create<RoutedEventArgs>(SaveAs);

            Load(TSCPath);
        }

        public ReactiveCommand<RoutedEventArgs, Unit> SaveCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> SaveAsCommand { get; }
    }
}
