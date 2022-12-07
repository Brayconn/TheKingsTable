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
using CaveStoryModdingFramework.Editors;
using System.Linq;
using System.Text;

namespace TheKingsTable.ViewModels.Editors
{
    internal class ScriptEditorDockItemViewModel : DockItemViewModel<TextScriptEditorViewModel> { }
    public class TextScriptEditorViewModel : ViewModelBase
    {
        public ProjectFile Project { get; }
        public TSCEditor Editor { get; private set; }
        public string TSCPath { get; private set; }
        string text = "";
        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }
        void Load(string tscPath)
        {
            byte[] bytes = Array.Empty<byte>();
            bool encrypted = Project.ScriptsEncrypted;
            if (Project.UseScriptSource)
            {
                var sspath = ScriptSource.GetScriptSourcePath(tscPath);
                if (File.Exists(sspath))
                {
                    bytes = File.ReadAllBytes(sspath);
                    encrypted = false;
                }
            }
            if (bytes.Length <= 0 && File.Exists(tscPath))
            {
                bytes = File.ReadAllBytes(tscPath);
            }
            if (bytes.Length > 0)
            {
                Editor = new TSCEditor(bytes, encrypted, Project.ScriptEncoding, Project.ScriptCommands?.Values.ToList());
            }
            else
            {
                Editor = new TSCEditor(Project.ScriptEncoding, Project.ScriptCommands.Values.ToList());
            }
            foreach(var line in Editor.Tokens)
            {
                foreach(var token in line)
                {
                    var sb = new StringBuilder();
                    sb.Append("(Text=\"");
                    sb.Append(token.GetString().Replace("\n", "\\n").Replace("\r","\\r"));
                    sb.Append("\" Type=");
                    if (token is TSCEventToken2 e)
                    {
                        sb.Append("Event Value=");
                        sb.Append(e.Value);
                    }
                    else if (token is TSCTextToken2 t)
                    {
                        sb.Append("Text");
                    }
                    else if (token is TSCCommandToken c)
                    {
                        sb.Append("Command ArgumentCount=");
                        sb.Append(c.Command?.Arguments.Count ?? -1);
                    }
                    else if(token is TSCArgumentToken a)
                    {
                        sb.Append("Argument Name=");
                        sb.Append(a.Argument.Name);
                        sb.Append(" Type=");
                        sb.Append(a.Argument.Type);
                    }
                    else
                        throw new ArgumentException("Unkown token type! " + token.GetType(), nameof(token));
                    sb.Append(" Validity=");
                    sb.Append(token.Validity);
                    sb.Append(")");
                    Text += sb.ToString();
                }
                Text += "\n";
            }
        }
        void Save(string tscPath)
        {
            var ms = new MemoryStream();
            Editor.Save(ms);
            var bytes = ms.ToArray();
            if (Project.UseScriptSource)
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
