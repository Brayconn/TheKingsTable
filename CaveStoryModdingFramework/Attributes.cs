using System.Collections.Generic;
using System.IO;
using PixelModdingFramework;

namespace CaveStoryModdingFramework.Maps
{
    public class Attribute : IMap<short, List<byte>, byte>
    {
        public const string DefaultPrefix = "Prt";
        public short Width => 16;
        public short Height => 16;
        public List<byte> Tiles { get; private set; }

        public Attribute()
        {
            Tiles = new List<byte>(new byte[Width * Height]);
        }
        public Attribute(string path)
        {
            if(new FileInfo(path).Length != Width * Height)
                throw new FileLoadException(); //TODO text, or myabe don't throw?
            Tiles = new List<byte>(File.ReadAllBytes(path));
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, Tiles.ToArray());
        }
    }
}
