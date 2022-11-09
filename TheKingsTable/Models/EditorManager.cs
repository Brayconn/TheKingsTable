using Avalonia.Media.Imaging;
using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Entities;
using CaveStoryModdingFramework.Maps;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework.Utilities;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TheKingsTable.Controls;
using TheKingsTable.ViewModels.Editors;

namespace TheKingsTable.Models
{
    public class EditorManager
    {
        public ProjectFile Project { get; private set; }

        SpriteCache Cache;
        Dictionary<string, Bitmap> BackgroundCache = new Dictionary<string, Bitmap>();
        Dictionary<string, Bitmap> TilesetCache = new Dictionary<string, Bitmap>();
        Dictionary<string, CaveStoryModdingFramework.Maps.Attribute> AttributeCache = new Dictionary<string, CaveStoryModdingFramework.Maps.Attribute>();

        //TODO MAKE EDITOR SETTING
        bool CaseSensitive { get; set; } = false;

        public static readonly Interaction<StageTableListViewModel, Unit> StageTableEditorOpened
            = new Interaction<StageTableListViewModel, Unit>();
        public static readonly Interaction<StageEditorViewModel, Unit> StageEditorOpened
            = new Interaction<StageEditorViewModel, Unit>();
        public static readonly Interaction<StageEditorViewModel, Unit> StageEditorSelected
            = new Interaction<StageEditorViewModel, Unit>();
        public static readonly Interaction<TextScriptEditorViewModel, Unit> ScriptEditorOpened
            = new Interaction<TextScriptEditorViewModel, Unit>();
        public static readonly Interaction<ProjectEditorViewModel, Unit> ProjectSettingsOpened
            = new Interaction<ProjectEditorViewModel, Unit>();

        public static readonly Interaction<Unit, bool> ProjectClosing
            = new Interaction<Unit, bool>();

        ProjectEditorViewModel? ProjectEditor = null;
        //TODO make a thing to toggle showing the project editor

        StageEditorToolMenu ToolMenu;
        public void UpdateGlobalSelection(StageEditorViewModel editor)
        {
            //GlobalSelector
        }

        public EditorManager(ProjectFile project)
        {
            Project = project;

            //StageEditorSelected.RegisterHandler()
        }

        private List<object> AttributeEditors { get; set; }
        private Dictionary<StageTableEntry, StageEditorViewModel> StageEditors { get; } = new Dictionary<StageTableEntry, StageEditorViewModel>();
        public async Task<StageEditorViewModel?> OpenStageEditor(StageTableEntry entry)
        {
            if (StageEditors.ContainsKey(entry))
                return StageEditors[entry];
                        
            var background = await TryGetBackground(entry.BackgroundName);
            if (background == null)
                return null;

            var tileset = await TryGetTileset(entry.TilesetName);
            if (tileset == null)
                return null;

            var attributes = await TryGetAttributes(entry.TilesetName);
            if (attributes == null)
                return null;

            var tiles = await TryGetTiles(entry.Filename);
            if (tiles == null)
                return null;

            var entities = await TryGetEntities(entry.Filename);
            if (entities == null)
                return null;

            var e = new StageEditorViewModel(Project, entry, background, tileset, attributes, tiles, entities);
            StageEditors.Add(entry, e);
            await StageEditorOpened.Handle(e);
            return e;
        }

        private List<StageTableListViewModel> StageTableEditors { get; } = new List<StageTableListViewModel>();
        public async Task<StageTableListViewModel?> OpenStageTableEditor(StageTableLocation location)
        {
            foreach(var item in StageTableEditors)
            {
                if(item.Location == location)
                {
                    return item;
                }
            }

            if(!File.Exists(location.Filename))
            {
                if(!await CommonInteractions.IsOk.Handle(new Words("Warning",
                    $"{location.Filename} doesn't exist! Would you like to create it and continue?")))
                {
                    File.Create(location.Filename);
                }
                else
                {
                    return null;
                }
            }

            var e = new StageTableListViewModel(this, location);
            StageTableEditors.Add(e);
            await StageTableEditorOpened.Handle(e);
            return e;
        }

        private List<TextScriptEditorViewModel> ScriptEditors { get; } = new List<TextScriptEditorViewModel>();
        public async Task<TextScriptEditorViewModel?> OpenScriptEditor(StageTableEntry entry)
        {
            foreach(var item in ScriptEditors)
            {
                //TODO check for the things
            }

            var path = await TryGetScript(entry.Filename);
            if (path == null)
                return null;

            if (!File.Exists(path))
            {
                if (!await CommonInteractions.IsOk.Handle(new Words("Warning",
                    $"{path} doesn't exist! Would you like to create it and continue?")))
                {
                    //don't need to do this since the script editor handles it already
                    //File.Create(path);
                }
                else
                {
                    return null;
                }
            }

            var e = new TextScriptEditorViewModel(Project, path);
            ScriptEditors.Add(e);
            await ScriptEditorOpened.Handle(e);
            return e;
        }

        private List<object> NpcTableEditors { get; } = new List<object>();
        private List<object> BulletTableEditors { get; } = new List<object>();
        private List<object> ArmsLevelTableEditors { get; } = new List<object>();



        #region Asset loading
        public async Task<string?> TryLoadAsset(bool caseOK, List<string> foundFiles)
        {
            //if we found something
            if (foundFiles.Count > 0
                //and we don't care about the case
                && !CaseSensitive
                //or we do care about the casing, but it was ok
                || caseOK
                //or we do care about the casing, but it doesn't matter 'cause the user said it was ok
                || await CommonInteractions.IsOk.Handle(new Words("Warning",
                    $"Case mismatch on {foundFiles[0]}! Continue using this file?"
                    )))
            {
                return foundFiles[0];
            }
            return null;
        }
        public async Task<Bitmap?> TryGetTileset(string name)
        {
            var tilesetPath = await TryLoadAsset(Project.GetTileset(name, out var t), t);
            if (tilesetPath == null)
                return null;
            if (!TilesetCache.ContainsKey(tilesetPath))
                TilesetCache.Add(tilesetPath, new Bitmap(tilesetPath));
            return TilesetCache[tilesetPath];
        }
        public async Task<CaveStoryModdingFramework.Maps.Attribute?> TryGetAttributes(string name)
        {
            var attributePath = await TryLoadAsset(Project.GetAttributes(name, out var a), a);
            if (attributePath == null)
                return null;
            if (!AttributeCache.ContainsKey(attributePath))
                AttributeCache.Add(attributePath, new CaveStoryModdingFramework.Maps.Attribute(attributePath));
            return AttributeCache[attributePath];
        }
        public async Task<Bitmap?> TryGetBackground(string name)
        {
            var backgroundPath = await TryLoadAsset(Project.GetBackground(name, out var b), b);
            if (backgroundPath == null)
                return null;
            if (!BackgroundCache.ContainsKey(backgroundPath))
                BackgroundCache.Add(backgroundPath, new Bitmap(backgroundPath));
            return BackgroundCache[backgroundPath];
        }
        public async Task<string?> TryGetTiles(string name)
        {
            return await TryLoadAsset(Project.GetTiles(name, out var t), t);
        }
        public async Task<string?> TryGetEntities(string name)
        {
            return await TryLoadAsset(Project.GetEntities(name, out var e), e);
        }
        public async Task<string?> TryGetScript(string name)
        {
            return await TryLoadAsset(Project.GetScript(name, out var e), e);
        }
        #endregion

    }
}
