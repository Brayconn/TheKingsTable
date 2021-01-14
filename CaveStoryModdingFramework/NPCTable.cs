using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using LocalizeableComponentModel;

namespace CaveStoryModdingFramework.Entities
{
    #region custom rects

    /// <summary>
    /// A rectangle with four byte values
    /// </summary>
    public abstract class ByteRect : INotifyPropertyChanging, INotifyPropertyChanged, IComparable
    {
        internal byte a, b, c, d;

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

        protected void SetVal(ref byte b, byte value, [CallerMemberName] string name = "")
        {
            if (b != value)
            {
                NotifyPropertyChanging(name);
                b = value;
                NotifyPropertyChanged(name);
            }
        }
        
        protected int Serialize()
        {
            return BitConverter.ToInt32(new[] { a, b, c, d }, 0);
        }

        public int CompareTo(object obj)
        {
            if (obj is ByteRect br)
            {
                return this.Serialize().CompareTo(br.Serialize());
            }
            else
                return -1;
        }
    }

    /// <summary>
    /// A set of values used for offsetting a sprite's position
    /// </summary>
    [DebuggerDisplay("LeftOffset = {LeftOffset} YOffset = {YOffset} RightOffset = {RightOffset} Unused = {Unused}")]
    public class NPCViewRect : ByteRect
    {
        /// <summary>
        /// Labeled as "Front" in CS.
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.LeftOffsetDescription), typeof(Dialog))]
        public byte LeftOffset { get => a; set => SetVal(ref a, value); }
        /// <summary>
        /// Labeled as "Top" in CS.
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.YOffsetDescription), typeof(Dialog))]
        public byte YOffset { get => b; set => SetVal(ref b, value); }
        /// <summary>
        /// Labeled as "Back" in CS.
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.RightOffsetDescription), typeof(Dialog))]
        public byte RightOffset { get => c; set => SetVal(ref c, value); }
        /// <summary>
        /// Labeled as "Bottom" in CS. It goes unused in the default sprite renderer
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.UnusedDescription), typeof(Dialog))]
        public byte Unused { get => d; set => SetVal(ref d, value); }

        public NPCViewRect() { }
        public NPCViewRect(byte leftOffset, byte yOffset, byte rightOffset, byte unused)
        {
            LeftOffset = leftOffset;
            YOffset = yOffset;
            RightOffset = rightOffset;
            Unused = unused;
        }
    }

    /// <summary>
    /// A rectangle for defining an npc's hitbox, based off the center of their position
    /// </summary>
    [DebuggerDisplay("Front = {Front} Top = {Top} Back = {Back} Bottom = {Bottom}")]
    public class NPCHitRect : ByteRect
    {
        [LocalizeableDescription(nameof(Dialog.FrontDescription), typeof(Dialog))]
        public byte Front { get => a; set => SetVal(ref a, value); }

        [LocalizeableDescription(nameof(Dialog.TopDescription), typeof(Dialog))]
        public byte Top { get => b; set => SetVal(ref b, value); }
        
        [LocalizeableDescription(nameof(Dialog.BackDescription), typeof(Dialog))]
        public byte Back { get => c; set => SetVal(ref c, value); }
        
        [LocalizeableDescription(nameof(Dialog.BottomDescription), typeof(Dialog))]
        public byte Bottom { get => d; set => SetVal(ref d, value); }

        public NPCHitRect() { }
        public NPCHitRect(byte front, byte top, byte back, byte bottom)
        {
            Front = front;
            Top = top;
            Back = back;
            Bottom = bottom;
        }

        public static explicit operator Size(NPCHitRect rect)
        {
            return new Size(rect.Back + rect.Front, rect.Top + rect.Bottom);
        }
    }

    #endregion

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

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public NPCHitRect Hitbox { get => hitbox; set => SetVal(ref hitbox, value); }

        //despite sharing the same type as Hitbox (in the original game)
        //the values here are treated completely different, hence the seperate type
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
        public static NPCHitRect ReadHitRect(this BinaryReader br)
        {
            return new NPCHitRect(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
        }
        public static NPCViewRect ReadViewRect(this BinaryReader br)
        {
            return new NPCViewRect(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
        }
        public static void Write(this BinaryWriter bw, ByteRect rect)
        {
            bw.Write(rect.a);
            bw.Write(rect.b);
            bw.Write(rect.c);
            bw.Write(rect.d);
        }

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
                WriteSequence((e) => bw.Write(e.Hitbox));
                WriteSequence((e) => bw.Write(e.Viewbox));
            }
        }
    }
}
