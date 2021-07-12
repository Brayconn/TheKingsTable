using LocalizeableComponentModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace CaveStoryModdingFramework
{
    public static class RectExtensions
    {
        public static NPCHitRect ReadHitRect(this BinaryReader br)
        {
            return new NPCHitRect(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
        }
        public static NPCViewRect ReadViewRect(this BinaryReader br)
        {
            return new NPCViewRect(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
        }
        public static BulletViewRect ReadIntRect(this BinaryReader br)
        {
            return new BulletViewRect(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
        }
        public static void Write(this BinaryWriter bw, BulletViewRect rect)
        {
            bw.Write(rect.LeftOffset);
            bw.Write(rect.YOffset);
            bw.Write(rect.RightOffset);
            bw.Write(rect.Unused);
        }

        public static Rectangle RectFromString(string input)
        {
            var nums = input.Split(',');
            return new Rectangle(int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]), int.Parse(nums[3]));
        }
        public static string RectToString(Rectangle rect)
        {
            return $"{rect.X},{rect.Y},{rect.Width},{rect.Height}";
        }
    }

    #region Bullet rect

    [DebuggerDisplay("LeftOffset = {LeftOffset} YOffset = {YOffset} RightOffset = {RightOffset} Unused = {Unused}")]
    public class BulletViewRect : PropertyChangedHelper
    {
        int leftOffset, yOffset, rightOffset, unused;

        /// <summary>
        /// Labeled as "Front" in CS.
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.LeftOffsetDescription), typeof(Dialog))]
        public int LeftOffset { get => leftOffset; set => SetVal(ref leftOffset, value); }
        /// <summary>
        /// Labeled as "Top" in CS.
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.YOffsetDescription), typeof(Dialog))]
        public int YOffset { get => yOffset; set => SetVal(ref yOffset, value); }
        /// <summary>
        /// Labeled as "Back" in CS.
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.RightOffsetDescription), typeof(Dialog))]
        public int RightOffset { get => rightOffset; set => SetVal(ref rightOffset, value); }
        /// <summary>
        /// Labeled as "Bottom" in CS. It goes unused in the default sprite renderer
        /// </summary>
        [LocalizeableDescription(nameof(Dialog.UnusedDescription), typeof(Dialog))]
        public int Unused { get => unused; set => SetVal(ref unused, value); }

        public BulletViewRect(int leftOffset, int yOffset, int rightOffset, int unused)
        {
            LeftOffset = leftOffset;
            YOffset = yOffset;
            RightOffset = rightOffset;
            Unused = unused;
        }
    }

    public class BullletViewRectTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is BulletViewRect br)
                return string.Join(", ", br.LeftOffset, br.YOffset, br.RightOffset, br.Unused);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion

    #region npc.tbl rects

    /// <summary>
    /// A rectangle with four byte values
    /// </summary>
    public abstract class ByteRect : PropertyChangedHelper, IComparable
    {
        internal byte a, b, c, d;
        /*
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

        protected void SetVal(ref byte prop, byte value, [CallerMemberName] string name = "")
        {
            if (prop != value)
            {
                NotifyPropertyChanging(name);
                prop = value;
                NotifyPropertyChanged(name);
            }
        }
        */
        public static explicit operator uint(ByteRect br)
        {
            return (uint)(int)br;
        }
        public static explicit operator int(ByteRect br)
        {
            return br.a | br.b << 8 | br.c << 16 | br.d << 24;
        }
        public static explicit operator byte[](ByteRect br)
        {
            return new[] { br.a, br.b, br.c, br.d };
        }

        public int CompareTo(object obj)
        {
            if (obj is ByteRect br)
            {
                return ((int)this).CompareTo((int)br);
            }
            else
                return -1;
        }
    }
    
    public class ByteRectTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is ByteRect br)
                return string.Join(", ", br.a, br.b, br.c, br.d);
            else
                return base.ConvertTo(context, culture, value, destinationType);
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
}
