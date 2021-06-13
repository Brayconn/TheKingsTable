using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

namespace CaveStoryModdingFramework
{
    public enum SearchLocations
    {
        Data,
        Stage,
        Npc,
    }
    public enum Prefixes
    {
        None,
        Spritesheet,
        Tileset,
    }
    public enum Extension
    {
        None,
        Image,
        Script,
        TileData,
        EntityData,
        TilesetData,
    }    

    public class AssetManager
    {
        readonly Mod parentMod;
        private string BaseDataPath => parentMod.BaseDataPath;

        //HACK turns out property grids need to have a custom type editor to make List<string> editable
        //Double check this isn't producing a reliance on winforms
        const string StringEditorLocation = "System.Windows.Forms.Design.StringCollectionEditor, " +
                                            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        /// <summary>
        /// Directories to be searched for Backgrounds, Global Scripts, and other misc. images
        /// </summary>
        [Editor(StringEditorLocation, typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> DataPaths { get; } = new List<string>();
        /// <summary>
        /// Directories to be searched for Tile Data, Entity Data, and Tileset data/images
        /// </summary>
        [Editor(StringEditorLocation, typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> StagePaths { get; } = new List<string>();
        /// <summary>
        /// Directories to be searched for Npc Spritesheets
        /// </summary>
        [Editor(StringEditorLocation, typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> NpcPaths { get; } = new List<string>();

        public static string MakeRelative(string basePath, string subPath)
        {
            //If the base path doesn't end with a \ or a /, the Uri class won't remove it from the relative path
            if (!basePath.EndsWith(new string(Path.DirectorySeparatorChar,1)) && !basePath.EndsWith(new string(Path.AltDirectorySeparatorChar, 1)))
                basePath += Path.DirectorySeparatorChar;
            return Uri.UnescapeDataString(new Uri(basePath).MakeRelativeUri(new Uri(subPath)).ToString());
        }
        public static string MakeAbsolute(params string[] paths)
        {
            return Path.GetFullPath(Path.Combine(paths));
        }
        public string MakeRelativeToBase(string path)
        {
            return MakeRelative(BaseDataPath, path);
        }
        public string MakeAbsoluteFromBase(string path)
        {
            return MakeAbsolute(BaseDataPath, path);
        }

        public bool TryGetList(SearchLocations folder, out List<string> list)
        {
            switch (folder)
            {
                case SearchLocations.Data: 
                    //TODO not sure if this is the best place to do this?
                    if (DataPaths.Count > 0)
                        list = DataPaths;
                    else
                        list = new List<string>() { BaseDataPath };
                    break;
                case SearchLocations.Stage:
                    list = StagePaths;
                    break;
                case SearchLocations.Npc:
                    list = NpcPaths;
                    break;
                default:
                    list = null;
                    break;
            }
            return list != null;
        }

        private string ApplyPrefixAndExtension(Prefixes prefix, string name, Extension extension)
        {
            switch (prefix)
            {
                //case Prefixes.None:
                //    break;
                case Prefixes.Tileset:
                    name = parentMod.TilesetPrefix + name;
                    break;
                case Prefixes.Spritesheet:
                    name = parentMod.SpritesheetPrefix + name;
                    break;                
            }
            switch (extension)
            {
                case Extension.Image:
                    name = Path.ChangeExtension(name, parentMod.ImageExtension);
                    break;
                case Extension.Script:
                    name = Path.ChangeExtension(name, parentMod.TSCExtension);
                    break;
                case Extension.TileData:
                    name = Path.ChangeExtension(name, parentMod.StageExtension);
                    break;
                case Extension.EntityData:
                    name = Path.ChangeExtension(name, parentMod.EntityExtension);
                    break;
                case Extension.TilesetData:
                    name = Path.ChangeExtension(name, parentMod.AttributeExtension);
                    break;
            }
            return name;
        }

        public AssetManager(Mod m)
        {
            parentMod = m;
        }
        public AssetManager(Mod m, string stage, string npc) : this(m)
        {
            StagePaths.Add(stage);
            NpcPaths.Add(npc);
        }

        public IEnumerable<string> EnumerateFiles(SearchLocations folder)
        {
            return EnumerateFiles(folder, Prefixes.None, Extension.None);
        }
        public IEnumerable<string> EnumerateFiles(SearchLocations folder, Extension extension)
        {
            return EnumerateFiles(folder, Prefixes.None, extension);
        }

        /// <summary>
        /// Get the full paths of every item in each list
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="prefix"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public IEnumerable<string> EnumerateFiles(SearchLocations folder, Prefixes prefix, Extension extension)
        {
            TryGetList(folder, out var list);
            string filter = ApplyPrefixAndExtension(prefix, "*", extension);

            foreach(var item in list)
            {
                var dir = new DirectoryInfo(MakeAbsoluteFromBase(item));
                if(dir.Exists)
                {
                    foreach(var file in dir.EnumerateFiles(filter))
                    {
                        yield return file.FullName;
                    }
                }
            }
        }
        public bool TryGetFile(SearchLocations folder, string name, Extension extension, out string path)
        {
            return TryGetFile(folder, Prefixes.None, name, extension, out path);
        }
        public bool TryGetFile(SearchLocations folder, Prefixes prefix, string name, Extension extension, out string path)
        {
            if (!TryGetList(folder, out var list))
            {
                path = null;
                return false;
            }
            name = ApplyPrefixAndExtension(prefix, name, extension);

            for (int i = 0; i < list.Count; i++)
            {
                var dir = new DirectoryInfo(MakeAbsoluteFromBase(list[i]));
                if (dir.Exists)
                {
                    //TODO this can probably be made much simpler since we really should only ever get one result?
                    foreach (var file in dir.EnumerateFiles(name))
                    {
                        if (file.Name == name)
                        {
                            path = file.FullName;
                            return true;
                        }
                    }
                }
            }
            path = name;
            return false;
        }

        public string GetFile(SearchLocations folder, string name, Extension extension)
        {
            return GetFile(folder, Prefixes.None, name, extension);
        }
        public string GetFile(SearchLocations folder, Prefixes prefix, string name, Extension extension)
        {
            if(TryGetFile(folder,prefix,name,extension,out string path))
            {
                return path;
            }
            else
            {
                throw new FileNotFoundException("The requested file could not be found in the given directory(s) with the given prefix!", name);
            }
        }
    }
}
