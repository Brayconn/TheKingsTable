using System;
using System.Collections.Generic;

namespace CaveStoryModdingFramework.TSC
{
    #region Arguments
    public enum ArgumentTypes
    {
        //BL/CE compatability
        Arms,               //a
        //Ammo,             //A
        Direction,          //d
        Event,              //e
        EquipFlags,         //E
        Face,               //f
        NpcFlags,           //F
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
        Unit,
        IslandFalling,
        ASCII,
    }

    public class Argument
    {
        public const int DefaultArgumentLength = 4;
        public string Name { get; set; } = "";

        public ArgumentTypes Type { get; set; } = ArgumentTypes.Number;

        /// <summary>
        /// 0 <= = variable length until Separator
        /// </summary>
        public int Length { get; set; } = DefaultArgumentLength;

        /// <summary>
        /// This is used in a regular string comparison if Length <= 0, otherwise it just uses the length of the string
        /// </summary>
        public string Separator { get; set; } = ":";

        public Argument() { }
        public Argument(string name) : this(name, DefaultArgumentLength, ArgumentTypes.Number) { }
        //TODO maybe mke a dictionary for auto names
        public Argument(ArgumentTypes arg) : this(arg.ToString(), DefaultArgumentLength, arg) { }
        public Argument(string name, int argLen, ArgumentTypes arg)
        {
            Name = name;
            Length = argLen;
            Type = arg;
        }        

        internal static List<object> ParseArguments(object[] args)
        {
            var arguments = new List<object>(args.Length);
            if (args.Length > 0)
            {
                string shortName = null;
                ArgumentTypes? argType = null;
                int? argLen = null;
                void AddWIPArg()
                {
                    if (shortName != null || argType != null || argLen != null)
                    {
                        arguments.Add(new Argument(shortName ?? "", argLen ?? DefaultArgumentLength, argType ?? ArgumentTypes.Number));
                        shortName = null;
                        argLen = null;
                        argType = null;
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    //Proper arguments are added normally
                    if (args[i] is Argument arg)
                    {
                        arguments.Add(arg);
                    }
                    //So are repeat structures
                    else if (args[i] is RepeatStructure rep)
                    {
                        arguments.Add(rep);
                    }
                    //strings get queued
                    else if (args[i] is string s)
                    {
                        AddWIPArg();
                        shortName = s;
                    }
                    //so do arg lengths
                    else if(args[i] is int val)
                    {
                        AddWIPArg();
                        argLen = val;
                    }
                    //and arg types
                    else if (args[i] is ArgumentTypes at)
                    {
                        AddWIPArg();
                        argType = at;
                    }
                    //anything else could be bad
                    else
                    {
                        throw new InvalidCastException($"Unable to recognize {args[i]} as a valid type!\n"
                            + "(Argument, RepeatStructure, string, or ArgumentTypes)");
                    }
                }
                //catch any leftovers
                AddWIPArg();
            }
            return arguments;
        }
    }
    #endregion

    #region Repeats
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

        public RepeatStructure() { }
        public RepeatStructure(RepeatTypes type, int value, params object[] args)
        {
            RepeatType = type;
            Value = value;
            Arguments = Argument.ParseArguments(args);
        }
    }
    #endregion

    #region Commands
    [Flags]
    public enum CommandProperties
    {
        None = 0,
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

        public Command() { }
        public Command(string shortName, string longName, string description, params object[] args)
            : this(shortName, longName, description, CommandProperties.None, args) { }
        public Command(string shortName, string longName, string description, CommandProperties properties, params object[] args)
        {
            ShortName = shortName;
            LongName = longName;
            Description = description;
            Properties = properties;
            Arguments = Argument.ParseArguments(args);
        }
    }
    #endregion
}
