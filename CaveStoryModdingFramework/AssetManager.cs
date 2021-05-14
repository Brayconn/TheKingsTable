using System.Collections.Generic;
using System.IO;

namespace CaveStoryModdingFramework
{
    public enum Folders
    {
        Data,
        Stage,
        Npc,
        Script,
    }
    public enum Prefixes
    {
        None,
        Spritesheet,
        Tileset,
    }

    public class AssetManager
    {
        public List<string> DataPaths { get; }
        public List<string> StagePaths { get; }
        public List<string> NpcPaths { get; }
        public List<string> ScriptPaths { get; }

        Mod parentMod;

        public AssetManager(Mod m)
        {
            parentMod = m;
            DataPaths = new List<string>();
            StagePaths = new List<string>();
            NpcPaths = new List<string>();
            ScriptPaths = new List<string>();
        }

        public bool TryGetFile(Folders folder, Prefixes prefix, string name, out string path)
        {
            List<string> list;
            path = "";

            switch (folder)
            {
                case Folders.Data:
                    list = DataPaths;
                    break;
                case Folders.Stage:
                    list = StagePaths;
                    break;
                case Folders.Npc:
                    list = NpcPaths;
                    break;
                case Folders.Script:
                    list = ScriptPaths;
                    break;
                default:
                    return false;
            }
            switch (prefix)
            {                    
                case Prefixes.Tileset:
                    name = parentMod.TilesetPrefix + name;
                    break;
                case Prefixes.Spritesheet:
                    name = parentMod.SpritesheetPrefix + name;
                    break;
            }
            for (int i = 0; i < list.Count; i++)
            {
                var dir = new DirectoryInfo(list[i]);
                foreach(var file in dir.EnumerateFiles())
                {                    
                    if(file.Name == name)
                    {
                        path = file.FullName;
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetFile(Folders folder, Prefixes prefix, string name)
        {
            if(TryGetFile(folder,prefix,name,out string path))
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
