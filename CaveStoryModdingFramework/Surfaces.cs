using System;
using System.Xml.Serialization;

namespace CaveStoryModdingFramework
{
    public static class SurfaceSource
    {
        public const string XmlType = "Type";
        public const string XmlRuntime = "Runtime";
        public const string XmlFile = "File";
        public const string XmlIndex = "Index";
    }
    public interface ISurfaceSource : ICloneable
    {
        string DisplayName { get; }
    }
    public class SurfaceSourceRuntime : ISurfaceSource
    {
        [XmlText]
        public string DisplayName { get; set; }

        private SurfaceSourceRuntime() { }
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
        [XmlAttribute]
        public Prefixes Prefix { get; set; }
        [XmlAttribute]
        public SearchLocations Folder { get; set; }
        [XmlAttribute]
        public string Filename { get; set; }
        [XmlText]
        public string DisplayName { get; set; }
        private SurfaceSourceFile() { }
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
        [XmlAttribute]
        public int Index { get; set; }
        [XmlText]
        public string DisplayName { get; set; }

        private SurfaceSourceIndex() { }
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
