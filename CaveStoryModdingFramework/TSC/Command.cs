using System;
using System.Collections.Generic;

namespace CaveStoryModdingFramework.TSC
{
    public enum ArgumentTypes
    {
        //BL/CE compatability
        Arms, //a
        //Ammo, //A
        Direction, //d
        Event, //e
        EquipFlags, //E
        Face, //f
        TSCFlags, //F
        ItemGraphic, //g
        CreditIllustration, //l
        Item, //i
        Map, //m
        Music, //u
        NPCEvent, //N
        NPCType, //n
        Sound, //s
        TileIndex, //t
        XCoord, //x
        YCoord, //y
        Number, //#
        //Ticks, //.

        //New stuff
        Separator,
        SkipFlags,
        MapFlags,
        ANP,
        BOA,
        UNI,
        IslandFalling,
    }

    public class Argument
    {
        public string Name { get; set; } = "";

        public ArgumentTypes Type { get; set; } = ArgumentTypes.Number;

        /// <summary>
        /// -1 = variable length until 
        /// </summary>
        public int Length { get; set; } = 4;

        /// <summary>
        /// This is a regex
        /// </summary>
        public string Separator { get; set; } = ".";
    }    

    [Flags]
    public enum CommandProperties
    {
        EndsEvent,
        ClearsTextbox
    }

    public class Command
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public List<Argument> Arguments { get; set; } = new List<Argument>();
        public CommandProperties Properties { get; set; }
    }
}
