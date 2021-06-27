using CaveStoryModdingFramework.Entities;
using System.Collections.Generic;
using System.IO;

namespace CaveStoryModdingFramework
{
    public enum BulletTablePresets
    {
        doukutsuexe,
        csplus
    }
    public class BulletTableLocation : DataLocation
    {
        public int BulletCount { get; set; }

        public BulletTableLocation(string path)
        {
            Filename = path;
        }
        public BulletTableLocation(string path, BulletTablePresets preset) : this(path)
        {
            ResetToDefault(preset);
        }
        public void ResetToDefault(BulletTablePresets preset)
        {
            switch (preset)
            {
                case BulletTablePresets.doukutsuexe:
                    DataLocationType = DataLocationTypes.Internal;
                    SectionName = "";
                    Offset = BulletTable.CSBulletTableAddress;
                    MaximumSize = BulletTable.CSBulletTableSize;
                    FixedSize = true;
                    BulletCount = BulletTable.CSBulletTableCount;
                    break;
                case BulletTablePresets.csplus:
                    DataLocationType = DataLocationTypes.External;
                    SectionName = "";
                    Offset = 0;
                    MaximumSize = 0;
                    FixedSize = false;
                    BulletCount = BulletTable.CSBulletTableCount;
                    break;
            }
        }
    }

    public class BulletTableEntry
    {
        public const int Size = 42;
        public sbyte Damage { get; set; }
        //Life
        public sbyte Hits { get; set; }
        //life_count
        public int Range { get; set; }

        public uint Bits { get; set; }
        public int enemyXL { get; set; }
        public int enemyYL { get; set; }
        public int blockXL { get; set; }
        public int blockYL { get; set; }
        public IntRect ViewBox { get; set; }
    }

    public static class BulletTable
    {
        public const int CSBulletTableAddress = 0x8F048;
        public const int CSBulletTableCount = 46;
        public const int CSBulletTableSize = CSBulletTableCount * BulletTableEntry.Size;

        public static List<BulletTableEntry> Read(BulletTableLocation location)
        {
            var count = location.BulletCount;
            if (location.DataLocationType == DataLocationTypes.External)
                count = (int)(new FileInfo(location.Filename).Length / BulletTableEntry.Size);
            var output = new List<BulletTableEntry>(count);
            using(var br = new BinaryReader(location.GetStream(FileMode.Open, FileAccess.Read)))
            {
                for (int i = 0; i < count; i++)
                {
                    output.Add(new BulletTableEntry()
                    {
                        Damage = br.ReadSByte(),
                        Hits = br.ReadSByte(),
                        Range = br.ReadInt32(),
                        Bits = br.ReadUInt32(),
                        enemyXL = br.ReadInt32(),
                        enemyYL = br.ReadInt32(),
                        blockXL = br.ReadInt32(),
                        blockYL = br.ReadInt32(),
                        ViewBox = br.ReadIntRect()
                    });
                }
            }
            return output;
        }

        public static void Write(IList<BulletTableEntry> bullets, BulletTableLocation location)
        {
            var buff = new byte[bullets.Count * BulletTableEntry.Size];
            using(var bw = new BinaryWriter(new MemoryStream(buff)))
            {
                foreach(var bullet in bullets)
                {
                    bw.Write(bullet.Damage);
                    bw.Write(bullet.Hits);
                    bw.Write(bullet.Range);
                    bw.Write(bullet.Bits);
                    bw.Write(bullet.enemyXL);
                    bw.Write(bullet.enemyYL);
                    bw.Write(bullet.blockYL);
                    bw.Write(bullet.ViewBox);
                }
            }
            DataLocation.Write(location, buff);
        }
    }

}
