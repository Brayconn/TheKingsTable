using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace CaveStoryModdingFramework.Entities
{
    [Flags]
    public enum EntityFlags : ushort
    {
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

    public class Entity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        //TODO finish implementing
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public short X { get; set; }
        public short Y { get; set; }
        public short Flag { get; set; }
        public short Event { get; set; }
        public short Type { get; set; }
        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public EntityFlags Bits { get; set; }

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
