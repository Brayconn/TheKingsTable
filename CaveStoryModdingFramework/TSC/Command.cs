using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

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

    [DebuggerDisplay("Name = {Name} Type = {Type} Length = {Length} Separator = {Separator}")]
    public class Argument
    {
        public const int DefaultArgumentLength = 4;
        public const string DefaultSeparator = ":";

        [XmlAttribute]
        public string Name { get; set; } = "";
        [XmlAttribute]
        public ArgumentTypes Type { get; set; } = ArgumentTypes.Number;

        /// <summary>
        /// 0 <= = variable length until Separator
        /// </summary>
        [XmlAttribute]
        public int Length { get; set; } = DefaultArgumentLength;

        /// <summary>
        /// This is used in a regular string comparison if Length <= 0, otherwise it just uses the length of the string
        /// </summary>
        [XmlAttribute]
        public string Separator { get; set; } = DefaultSeparator;

        public Argument() { }
        public Argument(string name, string separator = DefaultSeparator) : this(name, DefaultArgumentLength, ArgumentTypes.Number, DefaultSeparator) { }
        //TODO maybe mke a dictionary for auto names
        public Argument(ArgumentTypes arg) : this(arg.ToString(), DefaultArgumentLength, arg, DefaultSeparator) { }
        public Argument(string name, int argLen) : this(name, argLen, ArgumentTypes.Number, DefaultSeparator) { }
        public Argument(ArgumentTypes arg, string separator) : this(arg.ToString(), DefaultArgumentLength, arg, separator) { }
        public Argument(string name, ArgumentTypes arg, string separator) : this(name, DefaultArgumentLength, arg, separator) { }
        public Argument(string name, int argLen, ArgumentTypes arg, string separator = DefaultSeparator)
        {
            Name = name;
            Length = argLen;
            Type = arg;
            Separator = separator;
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
                    //if nothing has been queued quit
                    if (shortName == null && argType == null && argLen == null)
                        return;
                    //otherwise fill in the values
                    if(argType == null)
                        argType = ArgumentTypes.Number;
                    if (shortName == null)
                        shortName = argType.ToString();
                    if (argLen == null)
                        argLen = DefaultArgumentLength;

                    //and add the argument
                    arguments.Add(new Argument(shortName, (int)argLen, (ArgumentTypes)argType));
                    
                    //then reset
                    shortName = null;
                    argLen = null;
                    argType = null;
                }

                for (int i = 0; i < args.Length; i++)
                {
                    //Proper arguments are added normally
                    if (args[i] is Argument arg)
                    {
                        AddWIPArg();
                        arguments.Add(arg);
                    }
                    //So are repeat structures
                    else if (args[i] is RepeatStructure rep)
                    {
                        AddWIPArg();
                        arguments.Add(rep);
                    }
                    //strings get queued
                    else if (args[i] is string s)
                    {
                        if(shortName != null)
                            AddWIPArg();
                        shortName = s;
                    }
                    //so do arg lengths
                    else if(args[i] is int val)
                    {
                        if(argLen != null)
                            AddWIPArg();
                        argLen = val;
                    }
                    //and arg types
                    else if (args[i] is ArgumentTypes at)
                    {
                        if (argType != null)
                            AddWIPArg();
                        argType = at;
                    }
                    //anything else could be bad
                    else
                    {
                        throw new InvalidCastException($"Unable to recognize {args[i]} as a valid type!\n"
                            + "(Argument, RepeatStructure, string, int, or ArgumentTypes)");
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

    [DebuggerDisplay("RepeatType = {RepeatType} Value = {Value}")]
    public class RepeatStructure
    {
        [XmlAttribute]
        public RepeatTypes RepeatType { get; set; }
        [XmlAttribute]
        public int Value { get; set; }

        [XmlElement(ElementName = nameof(Argument), Type = typeof(Argument)),
         XmlElement(ElementName = nameof(RepeatStructure), Type = typeof(RepeatStructure))]
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

    [DebuggerDisplay("ShortName = {ShortName} LongName = {LongName} Author = {Author} Properties = {Properties}")]
    [XmlRoot(XMLName)]
    public class Command
    {
        public const string XMLName = "TSCCommand";
        public const string DefaultAuthor = "Pixel";
        [XmlIgnore]
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
        [XmlAttribute]
        public string ShortName { get; set; }
        [XmlAttribute]
        public string LongName { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string Author { get; set; } = DefaultAuthor;
        [XmlAttribute]
        public CommandProperties Properties { get; set; }
        [XmlElement(ElementName = nameof(Argument), Type = typeof(Argument)),
         XmlElement(ElementName = nameof(RepeatStructure), Type = typeof(RepeatStructure))]
        public List<object> Arguments { get; set; } = new List<object>();
                
        private Command() { }
        public Command(string shortName, string longName, string description)
            : this(shortName, longName, description, CommandProperties.None, Array.Empty<object>()) { }
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
