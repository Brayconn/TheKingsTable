using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using CaveStoryModdingFramework.TSC;

namespace CaveStoryModdingFramework.Compatability
{
    public static class TSCListTXT
    {
        public const string CEHeader = "[CE_TSC]";
        public const string BLHeader = "[BL_TSC]";

        static Dictionary<char, ArgumentTypes> CEArgumentTypesTable = new Dictionary<char, ArgumentTypes>()
        {
            { 'a', ArgumentTypes.Arms },
            { 'A', ArgumentTypes.Number },
            { 'd', ArgumentTypes.Direction },
            { 'e', ArgumentTypes.Event },
            { 'E', ArgumentTypes.EquipFlags },
            { 'f', ArgumentTypes.Face },
            { 'F', ArgumentTypes.NpcFlags },
            { 'g', ArgumentTypes.ItemGraphic },
            { 'l', ArgumentTypes.CreditIllustration },
            { 'i', ArgumentTypes.Item },
            { 'm', ArgumentTypes.Map },
            { 'u', ArgumentTypes.Music },
            { 'N', ArgumentTypes.NPCEvent },
            { 'n', ArgumentTypes.NPCType },
            { 's', ArgumentTypes.Sound },
            { 't', ArgumentTypes.TileIndex },
            { 'x', ArgumentTypes.XCoord },
            { 'y', ArgumentTypes.YCoord },
            { '#', ArgumentTypes.Number },
            { '.', ArgumentTypes.Number },
        };

        public static List<Command> Load(string path)
        {
            List<Command> output;
            
            using (var sr = new StreamReader(path))
            {
                string[] headerLine;
                //scan for the header
                do
                {
                    headerLine = sr.ReadLine().Split('\t');
                }
                while (sr.EndOfStream || (headerLine[0] != CEHeader && headerLine[0] != BLHeader));
                //stop if we didn't find it
                if (sr.EndOfStream)
                    return null;

                //try to initialize with the length of the array
                if (int.TryParse(headerLine[1], out int argCount))
                    output = new List<Command>(argCount);
                else
                    output = new List<Command>();
                
                //init the actual reader
                using (var tfp = new TextFieldParser(sr)
                {
                    TextFieldType = FieldType.Delimited,
                    Delimiters = new[] { "\t" },
                })
                {
                    while (!tfp.EndOfData)
                    {
                        Command cmd;
                        //switch to the right reader
                        switch (headerLine[0])
                        {
                            case BLHeader:
                                cmd = ReadBLLine(tfp.ReadFields());
                                break;
                            case CEHeader:
                                cmd = ReadCELine(tfp.ReadFields());
                                break;
                            //this should never be possible to hit
                            default:
                                throw new ArgumentException("A red spy is in the base!", headerLine[0]);
                        }
                        output.Add(cmd);
                    }
                }
            }
            return output;
        }
        
        //Shared
        const int ShortNameIndex = 0;
        const int ArgumentCountIndex = 1;
        const int ParameterTypesIndex = 2;
        const int LongNameIndex = 3;
        const int DescriptionIndex = 4;
        //BL only
        const int EndsEventIndex = 5;
        const int ClearsTextBoxIndex = 6;
        const int SeparatedIndex = 7;
        const int ParameterLengthsIndex = 8; //Reads {ArgumentLength} Values

        static Command ReadBase(string[] line)
        {
            return new Command(line[ShortNameIndex], line[LongNameIndex], line[DescriptionIndex]);
        }

        static Command ReadCELine(string[] line)
        {
            var cmd = ReadBase(line);

            string argString = line[ParameterTypesIndex];

            int argCount;
            if(!int.TryParse(line[ArgumentCountIndex], out argCount))
            {
                foreach (var c in argString)
                {
                    if (c == '-')
                        break;
                    argCount++;
                }
            }            
            
            for(int i = 0; i < argCount; i++)
            {
                var arg = new Argument(CEArgumentTypesTable[argString[i]]);
                switch(argString[i])
                {
                    case '.':
                        arg.Name = "Ticks";
                        break;
                    case 'A':
                        arg.Name = "Ammo";
                        break;
                }
                cmd.Arguments.Add(arg);
            }

            //setting ends event
            switch (cmd.ShortName)
            {
                case "<ESC":
                case "<INI":
                case "<LDP":
                case "<TRA":
                case "<EVE":
                case "<SLP":
                case "<END":
                    cmd.Properties |= CommandProperties.EndsEvent;
                    break;
            }
            //setting clears textbox
            switch (cmd.ShortName)
            {
                case "<CLO": //TBD
                case "<END":
                case "<CLR":
                case "<MSG":
                case "<MS2":
                case "<MS3":
                    cmd.Properties |= CommandProperties.ClearsTextbox;
                    break;
            }

            return cmd;
        }

        static Command ReadBLLine(string[] line)
        {
            var cmd = ReadCELine(line);

            //override what CE set for ClearsTextBox
            if (int.TryParse(line[ClearsTextBoxIndex], out int clear) && clear == 1)
                cmd.Properties |= CommandProperties.ClearsTextbox;
            else
                cmd.Properties &= ~CommandProperties.ClearsTextbox;

            //override what CE set for EndsEvent
            if (int.TryParse(line[EndsEventIndex], out int ends) && ends == 1)
                cmd.Properties |= CommandProperties.EndsEvent;
            else
                cmd.Properties &= ~CommandProperties.EndsEvent;

            int.TryParse(line[SeparatedIndex], out int SeparatedInt);
            bool Separated = SeparatedInt == 1;

            for(int i = 0; i < cmd.Arguments.Count; i++)
            {
                var arg = cmd.Arguments[i] as Argument;
                arg.Separator = Separated ? "." : "";

                if (int.TryParse(line[ParameterLengthsIndex + i], out int argLen))
                    arg.Length = argLen;
            }

            return cmd;
        }
    }
}
