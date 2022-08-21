using Avalonia.Threading;
using Avalonia.Interactivity;
using CaveStoryModdingFramework;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using CaveStoryModdingFramework.AutoDetection;
using CaveStoryModdingFramework.Stages;
using System.Collections.ObjectModel;

namespace TheKingsTable.ViewModels
{
    public class WizardViewModel : ViewModelBase
    {
        public Interaction<object, Unit> Close { get; } = new Interaction<object, Unit>();

        public ProjectFile Project { get; } = new ProjectFile();
        public AssetLayout BaseLayout { get; } = new AssetLayout()
        {
            DataPaths = new List<string>() { string.Empty },
            NpcPaths = new List<string>() { string.Empty },
            StagePaths = new List<string>() { string.Empty }
        };

        const int ThrottleTimeMS = 250;

        public WizardViewModel()
        {
            Project.Layouts.Add(BaseLayout);

            DetectEXECommand = ReactiveCommand.Create<RoutedEventArgs>(DetectEXE);
            DetectDataCommand = ReactiveCommand.Create<RoutedEventArgs>(DetectData);

            EXEOK = this.WhenAnyValue(x => x.EXEPath, x => File.Exists(x))
                    .Throttle(TimeSpan.FromMilliseconds(ThrottleTimeMS), AvaloniaScheduler.Instance);
            EXEOK.Subscribe(x => DataStart = Path.GetDirectoryName(EXEPath) ?? "");

            DataOK = this.WhenAnyValue(x => x.BaseDataPath, x => Directory.Exists(x))
                    .Throttle(TimeSpan.FromMilliseconds(ThrottleTimeMS), AvaloniaScheduler.Instance);
            DataOK.Subscribe(x => EXEStart = Path.GetDirectoryName(BaseDataPath) ?? "");

            BackCommand = ReactiveCommand.Create<RoutedEventArgs>(Back);
            NextCommand = ReactiveCommand.Create<RoutedEventArgs>(Next,
                MakePageConditions(new Dictionary<int, IObservable<bool>[]>()
                {
                    {
                        0,
                        new IObservable<bool>[]
                        {
                            DataOK
                        }
                    },
                    {
                        1,
                        new IObservable<bool>[]
                        {
                            this.WhenAnyValue(x => x.StageTableCount, x => x > 0)
                        }
                    },
                    {
                        2,
                        new IObservable<bool>[]
                        {
                            this.WhenAnyValue(x => x.LayoutDataPath, x => Directory.Exists(x))
                                .Throttle(TimeSpan.FromMilliseconds(ThrottleTimeMS), AvaloniaScheduler.Instance),
                            this.WhenAnyValue(x => x.ImageExtension, x => !string.IsNullOrWhiteSpace(x))
                        }
                    },
                    {
                        3,
                        new IObservable<bool>[]
                        {
                            this.WhenAnyValue(x => x.LayoutNpcPath, x => Directory.Exists(x))
                                .Throttle(TimeSpan.FromMilliseconds(ThrottleTimeMS), AvaloniaScheduler.Instance),
                            this.WhenAnyValue(x => x.SpritesheetPrefix, x => !string.IsNullOrWhiteSpace(x))
                        }
                    },
                    {
                        4,
                        new IObservable<bool>[]
                        {
                            this.WhenAnyValue(x => x.LayoutStagePath, x => Directory.Exists(x))
                                .Throttle(TimeSpan.FromMilliseconds(ThrottleTimeMS), AvaloniaScheduler.Instance),
                            this.WhenAnyValue(x => x.EntityExtension, x => !string.IsNullOrWhiteSpace(x)),
                            this.WhenAnyValue(x => x.MapExtension, x => !string.IsNullOrWhiteSpace(x)),
                            this.WhenAnyValue(x => x.ScriptExtension, x => !string.IsNullOrWhiteSpace(x)),
                            this.WhenAnyValue(x => x.AttributeExtension, x => !string.IsNullOrWhiteSpace(x))
                        }
                    }
                }));
        }

        int StageTableCount => Project.StageTables.Count;
         
        
        

        int selectedIndex = 0;
        public int SelectedIndex { get => selectedIndex; set => this.RaiseAndSetIfChanged(ref selectedIndex, value); }


        #region Page 1 - Base Data/EXE

        string dataStart = "";
        public string DataStart { get => dataStart; set => this.RaiseAndSetIfChanged(ref dataStart, value); }
        string exeStart = "";
        public string EXEStart { get => exeStart; set => this.RaiseAndSetIfChanged(ref exeStart, value); }
        public string BaseDataPath
        {
            get => Project.BaseDataPath;
            set
            {
                if (Project.BaseDataPath != value)
                {
                    this.RaisePropertyChanging();
                    Project.BaseDataPath = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        public string EXEPath
        {
            get => Project.EXEPath;
            set
            {
                if (Project.EXEPath != value)
                {
                    this.RaisePropertyChanging();
                    Project.EXEPath = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public IObservable<bool> EXEOK { get; }
        public IObservable<bool> DataOK { get; }
        
        public ReactiveCommand<RoutedEventArgs, Unit> DetectEXECommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> DetectDataCommand { get; }

        private void DetectEXE(RoutedEventArgs e)
        {
            var d = Path.GetDirectoryName(BaseDataPath);
            if (d == null)
                return;

            //look for an exe
            EXEPath = AutoDetector.FindLargestFile(d, "*.exe");
            //if we didn't find any, settle for the largest file
            if (EXEPath == null)
                EXEPath = AutoDetector.FindLargestFile(d, "*.*");
            //if it's still null... idk, set it to the directory?
            if (EXEPath == null)
                EXEPath = d;
        }
        private void DetectData(RoutedEventArgs e)
        {
            var d = Path.GetDirectoryName(EXEPath);
            if (d == null)
                return;

            var p = Path.Combine(d, "data");
            if (Directory.Exists(p))
                BaseDataPath = p;
            else
                BaseDataPath = d;
        }

        #endregion

        #region Page 2 - Data folder/image extension

        public string LayoutDataPath
        {
            get => BaseLayout.DataPaths[0];
            set
            {
                if (BaseLayout.DataPaths[0] != value)
                {
                    this.RaisePropertyChanging();
                    BaseLayout.DataPaths[0] = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        public string ImageExtension
        {
            get => Project.ImageExtension;
            set
            {
                if (Project.ImageExtension != value)
                {
                    this.RaisePropertyChanging();
                    Project.ImageExtension = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        public string BackgroundPrefix
        {
            get => Project.BackgroundPrefix;
            set
            {
                if (Project.BackgroundPrefix != value)
                {
                    this.RaisePropertyChanging();
                    Project.BackgroundPrefix = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Page 3 - NPC Folder

        public string LayoutNpcPath
        {
            get => BaseLayout.NpcPaths[0];
            set
            {
                if (BaseLayout.NpcPaths[0] != value)
                {
                    this.RaisePropertyChanging();
                    BaseLayout.NpcPaths[0] = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string SpritesheetPrefix
        {
            get => Project.SpritesheetPrefix;
            set
            {
                if (Project.SpritesheetPrefix != value)
                {
                    this.RaisePropertyChanging();
                    Project.SpritesheetPrefix = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Page 4 - Stage Folder

        public string LayoutStagePath
        {
            get => BaseLayout.StagePaths[0];
            set
            {
                if (BaseLayout.StagePaths[0] != value)
                {
                    this.RaisePropertyChanging();
                    BaseLayout.StagePaths[0] = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string EntityExtension
        {
            get => Project.EntityExtension;
            set
            {
                if (Project.EntityExtension != value)
                {
                    this.RaisePropertyChanging();
                    Project.EntityExtension = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string MapExtension
        {
            get => Project.MapExtension;
            set
            {
                if (Project.MapExtension != value)
                {
                    this.RaisePropertyChanging();
                    Project.MapExtension = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string ScriptExtension
        {
            get => Project.ScriptExtension;
            set
            {
                if (Project.ScriptExtension != value)
                {
                    this.RaisePropertyChanging();
                    Project.ScriptExtension = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ScriptsEncrypted
        {
            get => Project.ScriptsEncrypted;
            set
            {
                if(Project.ScriptsEncrypted != value)
                {
                    this.RaisePropertyChanging();
                    Project.ScriptsEncrypted = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string AttributeExtension
        {
            get => Project.AttributeExtension;
            set
            {
                if (Project.AttributeExtension != value)
                {
                    this.RaisePropertyChanging();
                    Project.AttributeExtension = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string TilesetPrefix
        {
            get => Project.TilesetPrefix;
            set
            {
                if (Project.TilesetPrefix != value)
                {
                    this.RaisePropertyChanging();
                    Project.TilesetPrefix = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region All of the logic for autodetecting and next/back buttons
        public ReactiveCommand<RoutedEventArgs, Unit> BackCommand { get; }
        public ReactiveCommand<RoutedEventArgs, Unit> NextCommand { get; }

        void Back(RoutedEventArgs e)
        {
            SelectedIndex--;
        }
        int nextPageToAutodetect = 1;
        
        async void Next(RoutedEventArgs e)
        {
            int nextPage = SelectedIndex+1;
            switch (nextPage)
            {
                case 1:
                    if(nextPage == nextPageToAutodetect)
                    {
                        nextPageToAutodetect++;
                        if (AutoDetectStageTables())
                        {
                            nextPage++;
                            goto case 2;
                        }
                    }
                    break;
                case 2:
                    ReloadTables();
                    if (nextPage == nextPageToAutodetect)
                    {
                        nextPageToAutodetect++;
                        if (AutoDetectDataFolder())
                        {
                            nextPage++;
                            goto case 3;
                        }
                    }
                    break;
                case 3:
                    if(nextPage == nextPageToAutodetect)
                    {
                        nextPageToAutodetect = 5;
                        nextPage += AutoDetectStageNpcFolders();
                        if (nextPage == 5)
                            goto case 5;
                    }
                    break;
                case 5:
                    if(nextPage == nextPageToAutodetect)
                    {
                        nextPageToAutodetect = -1;
                        if(!AutoDetectMods())
                            nextPage++;
                    }
                    break;

                case 7:
                    await Close.Handle(Project);
                    return;
            }
            SelectedIndex = nextPage;
        }

        void ReloadTables()
        {
            LoadedTables?.Clear(); //this might be unecessary
            LoadedTables = new List<List<StageTableEntry>>(Project.StageTables.Count);
            foreach (var entry in Project.StageTables.Values)
                LoadedTables.Add(entry.Read());
        }

        bool AutoDetectStageTables()
        {
            var foundTables = 0;
            if (File.Exists(EXEPath))
            {
                int i = 0;
                foreach (var result in AutoDetector.FindInternalStageTables(EXEPath))
                {
                    var key = "Internal " + (++i);
                    Project.StageTables.Add(key, result);
                    BaseLayout.StageTables.Add(new TableLoadInfo(key, true));
                    foundTables++;
                }

                //TODO based on what internal data was found we probably need to add a bunch of other locations
                //these need to be hardcoded
                //stuff like if you load freeware it knows where the other tables are
                //or dsiware, etc.
            }

            //external tables in general
            var extTables = AutoDetector.FindExternalTables(BaseDataPath);
            if (extTables != null)
            {
                Project.AddTables(extTables, BaseLayout);
                if (extTables.StageTables?.Count > 0)
                    foundTables += extTables.StageTables.Count;
            }

            this.RaisePropertyChanged(nameof(StageTableCount));

            return foundTables > 0;
        }
        List<List<StageTableEntry>>? LoadedTables = null;
        bool AutoDetectDataFolder()
        {
            //find the data folder
            var firstDataPath = AutoDetector.FindDataFolderAndImageExtension(BaseDataPath, LoadedTables, out var foundExts);

            if (firstDataPath == null)
                return false;

            LayoutDataPath = firstDataPath;

            //find the image extension
            var possibleExtensions = AutoDetector.GetMaxes(foundExts);
            if (possibleExtensions.Count != 1)
                return false;
            ImageExtension = possibleExtensions[0];


            return true;
        }
        //haha
        int AutoDetectStageNpcFolders()
        {
            //Find the npc/stage folders
            var spritesheets = AutoDetector.GetSpritesheets(LoadedTables);
            var filenames = AutoDetector.GetFilenames(LoadedTables);
            var tilesets = AutoDetector.GetTilesets(LoadedTables);

            var foundPaths = AutoDetector.FindNpcAndStageFolders(LayoutDataPath,
                //note that the project file gets modified by these functions
                x => AutoDetector.TryInitFromNpcFolder(x, spritesheets, Project.ImageExtension, Project),
                x => AutoDetector.TryInitFromStageFolder(x, filenames, tilesets, Project.ImageExtension, Project));
            //this is changed by the first one
            this.RaisePropertyChanged(nameof(SpritesheetPrefix));
            //all these are changed by the second
            this.RaisePropertyChanged(nameof(EntityExtension));
            this.RaisePropertyChanged(nameof(MapExtension));
            this.RaisePropertyChanged(nameof(ScriptExtension));
            this.RaisePropertyChanged(nameof(ScriptsEncrypted));
            this.RaisePropertyChanged(nameof(AttributeExtension));
            this.RaisePropertyChanged(nameof(TilesetPrefix));

            if (foundPaths.Item1 != null)
                LayoutNpcPath = foundPaths.Item1;
            if(foundPaths.Item2 != null)
                LayoutStagePath = foundPaths.Item2;

            return foundPaths.Item1 != null ? (foundPaths.Item2 != null ? 2 : 1) : 0;
        }

        bool AutoDetectMods()
        {
            int layoutCount = Project.Layouts.Count;
            if (BaseDataPath != LayoutDataPath)
            {
                bool SubAdd(string curr)
                {
                    //the tables this layout will use
                    var localTables = new List<List<StageTableEntry>>();

                    var extTablesFound = false;
                    //if there were external tables, we're already done
                    if (extTablesFound = AutoDetector.TryFindExternalTables(curr, out var externalTables)
                        || AutoDetector.CountHardcodedDataFiles(curr, Project.ImageExtension) >= 2
                        //otherwise, we should check if backgrounds exist
                        || AutoDetector.ContainsBackgrounds(curr, localTables) > 0.5)
                    {
                        //we are now committed to making a layout, so make it
                        var localLayout = new AssetLayout(BaseLayout, true);
                        localLayout.DataPaths.Add(curr);

                        //also time to add the previously found tables...
                        foreach (var table in LoadedTables)
                            localTables.Add(table);

                        //...the external tables if any were found...
                        if (extTablesFound)
                        {
                            foreach (var tab in externalTables.StageTables)
                                localTables.Add(tab.Read());
                            Project.AddTables(externalTables, localLayout);
                        }

                        //...and merge them!
                        var merged = AutoDetector.MergeStageTables(localTables);

                        var localSpritesheets = AutoDetector.GetSpritesheets(merged);
                        var localFilenames = AutoDetector.GetFilenames(merged);
                        var localTilesets = AutoDetector.GetTilesets(merged);

                        var loc = AutoDetector.FindNpcAndStageFolders(curr,
                            x => AutoDetector.ContainsNpcFiles(localSpritesheets, x, Project.ImageExtension, Project.SpritesheetPrefix) >= 0.5,
                            x => AutoDetector.ContainsStageFiles(merged, x, Project.TilesetPrefix, Project.AttributeExtension,
                            Project.ImageExtension, Project.MapExtension, Project.EntityExtension, Project.ScriptExtension) >= 0.5);

                        if (loc.Item1 != null)
                            localLayout.NpcPaths.Add(loc.Item1);
                        if (loc.Item2 != null)
                            localLayout.StagePaths.Add(loc.Item2);

                        //we should have a valid layout by this point????
                        Project.Layouts.Add(localLayout);
                        return true;
                    }
                    return false;
                }

                AutoDetector.BreadthFirstSearch(
                    Directory.EnumerateDirectories(BaseDataPath).Where(x => x != LayoutDataPath),
                    (x) => SubAdd(x));
            }
            return Project.Layouts.Count > layoutCount;
        }

        const int PageCount = 7;
        static IObservable<bool> ObservableAND(IEnumerable<IObservable<bool>> observables)
        {
            return Observable.CombineLatest(observables, x => x.All(y => y));
        }
        static IObservable<bool> ObservableOR(IEnumerable<IObservable<bool>> observables)
        {
            return Observable.CombineLatest(observables, x => x.Any(y => y));
        }
        IObservable<bool> MakePageConditions(Dictionary<int, IObservable<bool>[]> pageParams)
        {
            var allConditions = new List<IObservable<bool>>(PageCount);
            for (int i = 0; i < PageCount; i++)
            {
                var pageConditions = new List<IObservable<bool>>();

                //HACK need to generate functions that do the comparison
                //otherwise i will stay at PageCount for every comparison
                Func<int, Func<int,bool>> f =
                    (int j) =>
                        (int x) =>
                            x == j;

                //Basic page check
                pageConditions.Add(this.WhenAnyValue(x => x.SelectedIndex, f(i)));

                //any other requirements on this page
                if (pageParams.ContainsKey(i))
                    pageConditions.AddRange(pageParams[i]);

                //add it to the list
                if (pageConditions.Count > 1)
                    allConditions.Add(ObservableAND(pageConditions));
                else
                    allConditions.Add(pageConditions[0]);
            }
            return ObservableOR(allConditions);
        }
        #endregion

    }
}
