using System;
using System.Collections.Generic;
using System.IO;
using PixelModdingFramework;

namespace CaveStoryModdingFramework.Maps
{
    public class Attribute : IMap<short, List<byte>, byte>
    {
        public const string DefaultPrefix = "Prt";
        public const string DefaultExtension = "pxa";
        public short Width => 16;
        public short Height => 16;
        public List<byte> Tiles { get; private set; }

        public Attribute()
        {
            Tiles = new List<byte>(new byte[Width * Height]);
        }
        public Attribute(string path) : this()
        {
            Load(path);
        }
        public void Load(string path)
        {
            var tiles = File.ReadAllBytes(path);
            for (int i = 0; i < tiles.Length; i++)
                Tiles[i] = tiles[i];
        }
        public void Clear(byte val = 0)
        {
            for (int i = 0; i < Width * Height; i++)
                Tiles[i] = val;
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, Tiles.ToArray());
        }
    }
}
