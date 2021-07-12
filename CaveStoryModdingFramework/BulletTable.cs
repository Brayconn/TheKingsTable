using CaveStoryModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private BulletTableLocation()
        {

        }
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

    [Flags]
    public enum BulletFlags : uint
    {
        PierceTiles = 0x04,
        CollideWithTiles = 0x08,
        PierceInvincibleEntities = 0x10,
        BreakSnacks = 0x20,
        PierceSnacks = 0x40
    }

    public class BulletTableEntry : PropertyChangedHelper
    {
        //TODO this can actually be 42 for CS+, but I'm not using it for that, sooo...
        public const int Size = 44;

        sbyte damage, hits;
        int range, enemyHitboxWidth, enemyHitboxHeight, tileHitboxWidth, tileHitboxHeight;
        BulletFlags bits;
        BulletViewRect viewBox;

        public sbyte Damage { get => damage; set => SetVal(ref damage, value); }
        //Life
        public sbyte Hits { get => hits; set => SetVal(ref hits, value); }
        //Life_count
        public int Range { get => range; set => SetVal(ref range, value); }

        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public BulletFlags Bits { get => bits; set => SetVal(ref bits, value); }
        //EnemyXL
        public int EnemyHitboxWidth { get => enemyHitboxWidth; set => SetVal(ref enemyHitboxWidth, value); }
        //EnemyYL

        public int EnemyHitboxHeight { get => enemyHitboxHeight; set => SetVal(ref enemyHitboxHeight, value); }
        //BlockXL
        public int TileHitboxWidth { get => tileHitboxWidth; set => SetVal(ref tileHitboxWidth, value); }
        //BlockYL
        public int TileHitboxHeight { get => tileHitboxHeight; set => SetVal(ref tileHitboxHeight, value); }

        [TypeConverter(typeof(BullletViewRectTypeConverter))]
        public BulletViewRect ViewBox { get => viewBox; set => SetVal(ref viewBox, value); }
    }

    public static class BulletTable
    {
        public const string BULLETTABLE = "bullet.tbl";
        public static string BulletTableFilter = $"Bullet Table ({BULLETTABLE})|{BULLETTABLE}";

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
                    entry.Bits = (BulletFlags)br.ReadUInt32();
                    entry.EnemyHitboxWidth = br.ReadInt32();
                    entry.EnemyHitboxHeight = br.ReadInt32();
                    entry.TileHitboxWidth = br.ReadInt32();
                    entry.TileHitboxHeight = br.ReadInt32();
                    entry.ViewBox = br.ReadIntRect();

                    output.Add(entry);
                }
            }
            return output;
        }

        public static void Write(IList<BulletTableEntry> bullets, BulletTableLocation location)
        {
            var size = BulletTableEntry.Size;
            /*TODO related to the size thing
            if (location.PadDamageAndHits)
                size += 2;
            */
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
                    bw.Write((uint)bullet.Bits);
                    bw.Write(bullet.EnemyHitboxWidth);
                    bw.Write(bullet.EnemyHitboxHeight);
                    bw.Write(bullet.TileHitboxWidth);
                    bw.Write(bullet.TileHitboxHeight);
                    bw.Write(bullet.ViewBox);
                }
            }
            DataLocation.Write(location, buff);
        }
    }

}
