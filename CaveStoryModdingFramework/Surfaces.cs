using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaveStoryModdingFramework
{
    public enum Folders
    {
        Data,
        Stage,
        Npc,
    }
    public enum Prefixes
    {
        None,
        Spritesheet,
        Tileset
    }
    public interface ISurfaceSource : ICloneable
    {
        string DisplayName { get; }
    }
    public class SurfaceSourceRuntime : ISurfaceSource
    {
        public string DisplayName { get; set; }

        public SurfaceSourceRuntime(string displayName)
        {
            DisplayName = displayName;
        }

        public object Clone()
        {
            return new SurfaceSourceRuntime(DisplayName);
        }
    }
    public class SurfaceSourceFile : ISurfaceSource
    {
        public Folders Folder { get; set; }
        public Prefixes Prefix { get; set; }
        public string Filename { get; set; }
        public string DisplayName { get; set; }
        public SurfaceSourceFile(Folders folder, Prefixes prefix, string filename, string name)
        {
            Folder = folder;
            Prefix = prefix;
            Filename = filename;
            DisplayName = name;
        }

        public object Clone()
        {
            return new SurfaceSourceFile(Folder, Prefix, Filename, DisplayName);
        }

        public override bool Equals(object obj)
        {
            return (obj is SurfaceSourceFile s && Folder == s.Folder && Prefix == s.Prefix && Filename == s.Filename) || base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ((int)Folder * 7) + ((int)Prefix * 11) + Filename.GetHashCode();
        }
    }
    public class SurfaceSourceIndex : ISurfaceSource
    {
        public string DisplayName { get; set; }

        public int Index { get; set; }

        public SurfaceSourceIndex(int index, string displayName)
        {
            Index = index;
            DisplayName = displayName;
        }

        public object Clone()
        {
            return new SurfaceSourceIndex(Index, DisplayName);
        }
    }
    public static class Surfaces
    {
        public static readonly ReadOnlyDictionary<int, ISurfaceSource> SurfaceList
            = new ReadOnlyDictionary<int, ISurfaceSource>(new Dictionary<int, ISurfaceSource>()
        {
            {0, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Title", SurfaceNames.Title) },
            {1, new SurfaceSourceRuntime(SurfaceNames.PIXEL) },
            {2, new SurfaceSourceIndex(0, SurfaceNames.Tileset) },
            //3
            //4
            //5
            {6, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Fade", SurfaceNames.Fade) },
            //7
            {8, new SurfaceSourceFile(Folders.Data, Prefixes.None, "ItemImage", SurfaceNames.ItemImage) },
            {9, new SurfaceSourceRuntime(SurfaceNames.MapSystem) },
            {10, new SurfaceSourceRuntime(SurfaceNames.Screen) },
            {11, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Arms", SurfaceNames.Arms) },
            {12, new SurfaceSourceFile(Folders.Data, Prefixes.None, "ArmsImage", SurfaceNames.ArmsImage) },
            {13, new SurfaceSourceRuntime(SurfaceNames.MNA) },
            {14, new SurfaceSourceFile(Folders.Data, Prefixes.None, "StageImage", SurfaceNames.StageImage) },
            {15, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Loading", SurfaceNames.Loading) },
            {16, new SurfaceSourceFile(Folders.Data, Prefixes.None, "MyChar", SurfaceNames.MyChar) },
            {17, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Bullet", SurfaceNames.Bullet) },
            //18
            {19, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Caret", SurfaceNames.Caret) },
            {20, new SurfaceSourceFile(Folders.Npc, Prefixes.Spritesheet, "Sym", SurfaceNames.NpcSym) },
            {21, new SurfaceSourceIndex(2, SurfaceNames.Spritesheet1) },
            {22, new SurfaceSourceIndex(3, SurfaceNames.Spritesheet2) },
            {23, new SurfaceSourceFile(Folders.Npc, Prefixes.Spritesheet, "Regu", SurfaceNames.NpcRegu) },
            //24
            //25
            {26, new SurfaceSourceFile(Folders.Data, Prefixes.None, "TextBox", SurfaceNames.TextBox) },
            {27, new SurfaceSourceFile(Folders.Data, Prefixes.None, "Face", SurfaceNames.Face) },
            {28, new SurfaceSourceIndex(1, SurfaceNames.Background) },
            {29, new SurfaceSourceRuntime(SurfaceNames.Damage) },
            {30, new SurfaceSourceRuntime(SurfaceNames.TextLine1) },
            {31, new SurfaceSourceRuntime(SurfaceNames.TextLine2) },
            {32, new SurfaceSourceRuntime(SurfaceNames.TextLine3) },
            {33, new SurfaceSourceRuntime(SurfaceNames.TextLine4) },
            {34, new SurfaceSourceRuntime(SurfaceNames.TextLine5) },
            {35, new SurfaceSourceRuntime(SurfaceNames.CreditsText) },
            {36, new SurfaceSourceRuntime(SurfaceNames.CreditsImage) },
            {37, new SurfaceSourceFile(Folders.Data, Prefixes.None, "casts", SurfaceNames.Casts) },
            //38
            //39
            {40, new SurfaceSourceRuntime(SurfaceNames.MAX) },
        });
    }
}
