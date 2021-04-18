using System;
using System.Collections.Generic;

namespace CaveStoryModdingFramework.TSC
{
    public enum ArgumentTypes
    {
        //BL/CE compatability
        Arms,               //a
        //Ammo,             //A
        Direction,          //d
        Event,              //e
        EquipFlags,         //E
        Face,               //f
        TSCFlags,           //F
        ItemGraphic,        //g
        CreditIllustration, //l
        Item,               //i
        Map,                //m
        Music,              //u
        NPCEvent,           //N
        NPCType,            //n
        Sound,              //s
        TileIndex,          //t
        XCoord,             //x
        YCoord,             //y
        Number,             //#
        //Ticks,            //.

        //New stuff
        SkipFlags,
        MapFlags,
        ANP,
        BOA,
        UNI,
        IslandFalling,
        ASCII,
    }

    public class Argument
    {
        public string Name { get; set; } = "";

        public ArgumentTypes Type { get; set; } = ArgumentTypes.Number;

        /// <summary>
        /// 0 <= = variable length until Separator
        /// </summary>
        public int Length { get; set; } = 4;

        /// <summary>
        /// This is read as a regex if 0, otherwise it just uses the length of the string
        /// </summary>
        public string Separator { get; set; } = ".";
    }
    public enum RepeatTypes
    {
        GlobalIndex,
        //LocalIndex
    }

    public class RepeatStructure
    {
        public RepeatTypes RepeatType { get; set; }
        public int Value { get; set; }
        public List<object> Arguments { get; set; } = new List<object>();
    }

    [Flags]
    public enum CommandProperties
    {
        EndsEvent = 1,
        ClearsTextbox
    }

    public class Command
    {
        public bool UsesRepeats
        {
            get
            {
                for (int i = 0; i < Arguments.Count; i++)
                    if (Arguments[i] is RepeatStructure)
                        return true;
                return false;
            }
        }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public List<object> Arguments { get; set; } = new List<object>();
        public CommandProperties Properties { get; set; }
    }
}
