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
        public bool PadDamageAndHits { get; set; }

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
                    PadDamageAndHits = true;
                    break;
                case BulletTablePresets.csplus:
                    DataLocationType = DataLocationTypes.External;
                    SectionName = "";
                    Offset = 0;
                    MaximumSize = 0;
                    FixedSize = false;
                    BulletCount = BulletTable.CSBulletTableCount;
                    PadDamageAndHits = false;
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
        public BulletViewRect ViewBox { get; set; }
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
                    var entry = new BulletTableEntry()
                    {
                        Damage = br.ReadSByte(),
                        Hits = br.ReadSByte()
                    };
                    if (location.PadDamageAndHits)
                        br.BaseStream.Position += 2;
                    entry.Range = br.ReadInt32();
                    entry.Bits = br.ReadUInt32();
                    entry.enemyXL = br.ReadInt32();
                    entry.enemyYL = br.ReadInt32();
                    entry.blockXL = br.ReadInt32();
                    entry.blockYL = br.ReadInt32();
                    entry.ViewBox = br.ReadIntRect();

                    output.Add(entry);
                }
            }
            return output;
        }

        public static void Write(IList<BulletTableEntry> bullets, BulletTableLocation location)
        {
            var size = BulletTableEntry.Size;
            if (location.PadDamageAndHits)
                size += 2;
            var buff = new byte[bullets.Count * size];
            using(var bw = new BinaryWriter(new MemoryStream(buff)))
            {
                foreach(var bullet in bullets)
                {
                    bw.Write(bullet.Damage);
                    bw.Write(bullet.Hits);
                    if(location.PadDamageAndHits) //I'm so clever 😎
                        bw.Write((ushort)0);
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
