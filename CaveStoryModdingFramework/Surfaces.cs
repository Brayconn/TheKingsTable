using System;

namespace CaveStoryModdingFramework
{
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
        public SearchLocations Folder { get; set; }
        public Prefixes Prefix { get; set; }
        public string Filename { get; set; }
        public string DisplayName { get; set; }
        public SurfaceSourceFile(SearchLocations folder, Prefixes prefix, string filename, string name)
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
}
