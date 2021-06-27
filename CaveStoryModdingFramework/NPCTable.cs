using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace CaveStoryModdingFramework.Entities
{
    public class NPCTableEntry : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public const int Size = 0x18;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void NotifyPropertyChanging([CallerMemberName] string name = "")
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
        protected void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void SetVal<T>(ref T var, T value, [CallerMemberName] string name = "") where T : IComparable
        {
            if (var == null || var.CompareTo(value) != 0)
            {
                NotifyPropertyChanging(name);
                var = value;
                NotifyPropertyChanged(name);
            }
        }

        private EntityFlags bits;
        private ushort life;
        private byte spriteSurface, hitSound, deathSound, smokeSize;
        private int xp, damage;
        private NPCHitRect hitbox;
        private NPCViewRect viewbox;
        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public EntityFlags Bits { get => bits; set => SetVal(ref bits, value); }
        public ushort Life { get => life; set => SetVal(ref life, value); }
        
        public byte SpriteSurface { get => spriteSurface; set => SetVal(ref spriteSurface, value); }
        public byte HitSound { get => hitSound; set => SetVal(ref hitSound, value); }
        public byte DeathSound { get => deathSound; set => SetVal(ref deathSound, value); }
        public byte SmokeSize { get => smokeSize; set => SetVal(ref smokeSize, value); }

        public int XP { get => xp; set => SetVal(ref xp, value); }
        public int Damage { get => damage; set => SetVal(ref damage, value); }

        [TypeConverter(typeof(ByteRectTypeConverter))]
        public NPCHitRect Hitbox { get => hitbox; set => SetVal(ref hitbox, value); }

        //despite sharing the same type as Hitbox (in the original game)
        //the values here are treated completely different, hence the seperate type
        [TypeConverter(typeof(ByteRectTypeConverter))]
        public NPCViewRect Viewbox { get => viewbox; set => SetVal(ref viewbox, value); }

        public NPCTableEntry()
        {
            Hitbox = new NPCHitRect();
            Viewbox = new NPCViewRect();
        }
    }

    public static class NPCTable
    {
        public const string NPCTBL = "npc.tbl";
        public static string NPCTBLFilter = $"{Dialog.NPCTable} ({NPCTBL})|{NPCTBL}";

        public static List<NPCTableEntry> Load(string path)
        {
            using(var br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                var npcCount = br.BaseStream.Length / NPCTableEntry.Size;
                //Unfortunately, initializing an array of classes like NPCTableEntry[npcCount]
                //just gives you a really long array of null
                var npcTable = new List<NPCTableEntry>((int)npcCount);
                while (npcTable.Count < npcCount)
                    npcTable.Add(new NPCTableEntry());

                //something something saving on variable delcarations something something
                int i;
                void ReadSequence(Action<NPCTableEntry> act)
                {
                    for (i = 0; i < npcCount; i++)
                        act(npcTable[i]);
                }
                ReadSequence((e) => e.Bits = (EntityFlags)br.ReadUInt16());
                ReadSequence((e) => e.Life = br.ReadUInt16());
                ReadSequence((e) => e.SpriteSurface = br.ReadByte());
                ReadSequence((e) => e.DeathSound = br.ReadByte());
                ReadSequence((e) => e.HitSound = br.ReadByte());
                ReadSequence((e) => e.SmokeSize = br.ReadByte());
                ReadSequence((e) => e.XP = br.ReadInt32());
                ReadSequence((e) => e.Damage = br.ReadInt32());
                ReadSequence((e) => e.Hitbox = br.ReadHitRect());
                ReadSequence((e) => e.Viewbox = br.ReadViewRect());

                return npcTable;
            }
        }

        public static void Save(IList<NPCTableEntry> table, string path)
        {
            using(var bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                int i;
                void WriteSequence(Action<NPCTableEntry> act)
                {
                    for (i = 0; i< table.Count; i++)
                        act(table[i]);
                }
                WriteSequence((e) => bw.Write((ushort)e.Bits));
                WriteSequence((e) => bw.Write(e.Life));
                WriteSequence((e) => bw.Write(e.SpriteSurface));
                WriteSequence((e) => bw.Write(e.DeathSound));
                WriteSequence((e) => bw.Write(e.HitSound));
                WriteSequence((e) => bw.Write(e.SmokeSize));
                WriteSequence((e) => bw.Write(e.XP));
                WriteSequence((e) => bw.Write(e.Damage));
                WriteSequence((e) => bw.Write((int)e.Hitbox));
                WriteSequence((e) => bw.Write((int)e.Viewbox));
            }
        }
    }
}
