using LocalizeableComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CaveStoryModdingFramework.Entities
{
    [Flags]
    public enum EntityFlags : ushort
    {
        None = 0x00,
        SoftSolid = 0x01,               // Pushes Quote out
        IgnoreTile44 = 0x02,            // Ignores tile 44, which normally blocks NPCs
        Invulnerable = 0x04,            // Can't be hurt
        IgnoreSolidity = 0x08,          // Doesn't collide with anything
        Bouncy = 0x10,                  // Quote bounces on top of NPC
        Shootable = 0x20,               // Can be shot
        HardSolid = 0x40,               // Essentially acts as level tiles
        RearAndTopDontHurt = 0x80,      // Rear and top don't hurt when touched
        RunEventOnTouch = 0x0100,       // Run event when touched
        RunEventWhenKilled = 0x0200,    // Run event when killed
        Unused = 0x0400,                // Unused
        AppearWhenFlagSet = 0x0800,     // Only appear when flag is set
        SpawnInOtherDirection = 0x1000, // Spawn facing to the right (or however the NPC interprets the direction)
        Interactable = 0x2000,          // Run event when interacted with
        HideWhenFlagSet = 0x4000,       // Hide when flag is set
        ShowDamage = 0x8000             // Show the number of damage taken when harmed
    };

    /// <summary>
    /// The actual entity class
    /// </summary>
    public class Entity : PropertyChangedHelper
    {
        short x,y,flag,@event,type;
        EntityFlags bits;

        public short X { get => x; set => SetVal(ref x, value); }
        public short Y { get => y; set => SetVal(ref y, value); }
        public short Flag { get => flag; set => SetVal(ref flag, value); }
        public short Event { get => @event; set => SetVal(ref @event, value); }
        public short Type { get => type; set => SetVal(ref type, value); }
        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public EntityFlags Bits { get => bits; set => SetVal(ref bits, value); }

        public Entity(Entity e) : this(e.X, e.Y, e.Flag, e.Event, e.Type, e.Bits) { }
        public Entity(short x, short y, short flag, short @event, short type, EntityFlags bits)
        {
            X = x;
            Y = y;
            Flag = flag;
            Event = @event;
            Type = type;
            Bits = bits;
        }        
    }
    
    /// <summary>
    /// Shell over multiple entities to allow for editing multiple entities at once
    /// </summary>
    public class MultiEntityShell : PropertyChangedHelper
    {
        readonly Entity[] hosts;
        private T GetProperty<T>(Entity e, PropertyInfo property)
        {
            return (T)property.GetValue(e);
        }
        private T? GetProperty<T>([CallerMemberName] string propertyName = "") where T : struct
        {
            var property = typeof(Entity).GetProperty(propertyName);
            T? val = GetProperty<T?>(hosts[0], property);
            for (int i = 1; i < hosts.Length; i++)
                if (!val.Equals(GetProperty<T>(hosts[i], property)))
                    return null;
            return val;
        }

        private void SetProperty<T>(T value, [CallerMemberName] string propertyName = "")
        {
            if (value == null)
                return;
            NotifyPropertyChanging(propertyName);
            var property = typeof(Entity).GetProperty(propertyName);
            foreach (var entity in hosts)
            {
                property.SetValue(entity, value);
            }
            NotifyPropertyChanged(propertyName);
        }

        [LocalizeableDescription(nameof(Dialog.XDescription), typeof(Dialog))]
        public short? X { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.YDescription), typeof(Dialog))]
        public short? Y { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.FlagDescription), typeof(Dialog))]
        public short? Flag { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.EventDescription), typeof(Dialog))]
        public short? Event { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.TypeDescription), typeof(Dialog))]
        public short? Type { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.BitsDescription), typeof(Dialog))]
        public EntityFlags? Bits { get => GetProperty<EntityFlags>(); set => SetProperty(value); }


        public MultiEntityShell(params Entity[] ents)
        {
            if (ents.Length < 1)
                throw new ArgumentException("You must supply at least one entity.", nameof(ents));
            hosts = ents;
        }
    }

    /// <summary>
    /// Shell over an entity to allow for adding entity specific interfaces
    /// </summary>
    public abstract class EntityShell
    {
        readonly Entity host;


        [LocalizeableDescription(nameof(Dialog.XDescription), typeof(Dialog))]
        public short X { get => host.X; set => host.X = value; }

        [LocalizeableDescription(nameof(Dialog.YDescription), typeof(Dialog))]
        public short Y { get => host.Y; set => host.Y = value; }
        [LocalizeableDescription(nameof(Dialog.FlagDescription), typeof(Dialog))]
        public short Flag { get => host.Flag; set => host.Flag = value; }
        [LocalizeableDescription(nameof(Dialog.EventDescription), typeof(Dialog))]
        public short Event { get => host.Event; set => host.Event = value; }

        [LocalizeableDescription(nameof(Dialog.TypeDescription), typeof(Dialog))]
        public short Type { get => host.Type; set => host.Type = value; }

        [LocalizeableDescription(nameof(Dialog.BitsDescription), typeof(Dialog))]
        public EntityFlags Bits { get => host.Bits; set => host.Bits = value; }

        protected EntityShell(Entity e)
        {
            host = e;
        }
    }

    public static class PXE
    {
        public const string DefaultExtension = "pxe";
        const string DefaultHeader = "PXE\0";
        public static List<Entity> Read(string path, string header = DefaultHeader)
        {
            using(var br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                if (Encoding.ASCII.GetString(br.ReadBytes(header.Length)) != header)
                    throw new FileLoadException(); //TODO message n stuff
                var length = br.ReadInt32();
                var ents = new List<Entity>(length);
                for (int i = 0; i < length; i++)
                    ents.Add(new Entity(
                        br.ReadInt16(),
                        br.ReadInt16(),
                        br.ReadInt16(),
                        br.ReadInt16(),
                        br.ReadInt16(),
                        (EntityFlags)br.ReadUInt16())
                        );
                return ents;
            }
        }

        public static void Write(IList<Entity> input, string path, string header = DefaultHeader)
        {
            using(var bw = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                bw.Write(Encoding.ASCII.GetBytes(header));
                bw.Write(input.Count);
                foreach(var ent in input)
                {
                    bw.Write(ent.X);
                    bw.Write(ent.Y);
                    bw.Write(ent.Flag);
                    bw.Write(ent.Event);
                    bw.Write(ent.Type);
                    bw.Write((ushort)ent.Bits);
                }
            }
        }
    }
}
