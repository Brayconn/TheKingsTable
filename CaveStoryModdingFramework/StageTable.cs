using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using PETools;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml;

namespace CaveStoryModdingFramework.Stages
{
    public enum BackgroundTypes
    {
        FixedToCamera = 0,
        FollowSlowly,
        FixedToForeground,
        Water,
        NoDraw,
        ScrollItems,
        ParallaxItems,
        Parallax
    }
    public enum BossNumbers
    {
        None = 0,
        Omega,
        Balfrog,
        MonsterX,
        Core,
        Ironhead,
        Sisters,
        UndeadCore,
        HeavyPress,
        BallosBall
    }
    public class StageEntry : INotifyPropertyChanging, INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanging([CallerMemberName] string name = "")
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
        void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        void SetVal<T>(ref T variable, T value, [CallerMemberName] string name = "")
        {
            if(!EqualityComparer<T>.Default.Equals(variable, value))
            {
                NotifyPropertyChanging(name);
                variable = value;
                NotifyPropertyChanged(name);
            }
        }

        string tilesetName, filename, backgroundName, spritesheet1, spritesheet2, japaneseName, mapName;
        BackgroundTypes backgroundType;
        BossNumbers bossNumber;
        public string TilesetName { get => tilesetName; set => SetVal(ref tilesetName, value); }
        //SW doesn't allow "\empty" as a name, it won't show them n stuff
        public string Filename { get => filename; set => SetVal(ref filename,value); }
        public BackgroundTypes BackgroundType { get => backgroundType; set => SetVal(ref backgroundType, value); }
        public string BackgroundName { get => backgroundName; set => SetVal(ref backgroundName,value); }
        public string Spritesheet1 { get => spritesheet1; set => SetVal(ref spritesheet1, value); }
        public string Spritesheet2 { get => spritesheet2; set => SetVal(ref spritesheet2, value); }
        public BossNumbers BossNumber { get => bossNumber; set => SetVal(ref bossNumber, value); }
        public string JapaneseName { get => japaneseName; set => SetVal(ref japaneseName, value); }
        public string MapName { get => mapName; set => SetVal(ref mapName, value); }

        public byte[] Serialize(StageEntrySettings settings)
        {
            var data = new byte[settings.Size];
            var index = 0;

            void WriteNum(object num, Type t)
            {
                var bytes = Extensions.ConvertAndGetBytes(num, t);
                Array.Copy(bytes, 0, data, index, bytes.Length);
                index += bytes.Length;
            }
            void WriteString(string text, Encoding encoding, int max)
            {
                if (text != null)
                    Extensions.BufferCopy(encoding?.GetBytes(text) ?? Array.Empty<byte>(), data, index, max - 1);
                index += max;
            }
            WriteString(TilesetName, settings.FilenameEncoding, settings.TilesetNameBuffer);
            WriteString(Filename, settings.FilenameEncoding, settings.FilenameBuffer);
            WriteNum(BackgroundType, settings.BackgroundTypeType);
            WriteString(BackgroundName, settings.FilenameEncoding, settings.BackgroundNameBuffer);
            WriteString(Spritesheet1, settings.FilenameEncoding, settings.Spritesheet1Buffer);
            WriteString(Spritesheet2, settings.FilenameEncoding, settings.Spritesheet2Buffer);
            WriteNum(BossNumber, settings.BossNumberType);
            WriteString(JapaneseName, settings.JapaneseNameEncoding, settings.JapaneseNameBuffer);
            WriteString(MapName, settings.MapNameEncoding, settings.MapNameBuffer);

            return data;
        }

        public object Clone()
        {
            return new StageEntry()
            {
                TilesetName = tilesetName,
                Filename = filename,
                BackgroundType = backgroundType,
                BackgroundName = backgroundName,
                Spritesheet1 = spritesheet1,
                Spritesheet2 = spritesheet2,
                BossNumber = bossNumber,
                JapaneseName = japaneseName,
                MapName = mapName,
            };
        }
    }

    public class EncodingTypeConverter : TypeConverter
    {
        static readonly HashSet<Encoding> encounteredEncodings = new HashSet<Encoding>();
        static StandardValuesCollection AvailableEncodings = null;
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (AvailableEncodings == null)
            {
                //*
                foreach (var enc in Encoding.GetEncodings().Select(x => x.GetEncoding()))
                    encounteredEncodings.Add(enc);
                /*/
                var baseEncoding = typeof(Encoding);
                foreach (var prop in baseEncoding.GetProperties())
                {
                    if (prop.PropertyType == typeof(Encoding))
                    {
                        encounteredEncodings.Add((Encoding)prop.GetValue(baseEncoding));
                    }
                }
                */
                AvailableEncodings = new StandardValuesCollection(encounteredEncodings.ToList());
            }
            return AvailableEncodings;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(int);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Encoding enc = null;
            try
            {
                if (value is string s)
                {
                    if (int.TryParse(s, out int i))
                        enc = Encoding.GetEncoding(i);
                    else
                        enc = Encoding.GetEncoding(s);
                }
                else if (value is int i)
                {
                    enc = Encoding.GetEncoding(i);
                }
            }
            catch (ArgumentException) { }
            catch (NotSupportedException) { }

            if (enc != null && !encounteredEncodings.Contains(enc))
            {
                encounteredEncodings.Add(enc);
                AvailableEncodings = new StandardValuesCollection(encounteredEncodings.ToList());
            }
            return enc;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value is Encoding enc ? enc.WebName : null;
        }
    }
    public class EnumTypeTypeConverter : TypeConverter
    {
        static readonly Type[] integerTypes = new[]
        {
            typeof(sbyte),
            typeof(byte),            
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
        };
        static readonly StandardValuesCollection standardTypes = new StandardValuesCollection(integerTypes);
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => standardTypes;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(value is string s)
            {
                var t = Type.GetType(s, false, true);
                if(integerTypes.Contains(t))
                    return t;
            }
            return null;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return (value as Type)?.FullName;
        }
    }
    
    public class StageEntrySettings
    {
        const string VariableTypes = "Variable Types";
        const string BufferSizes = "Buffer Sizes";

        [Category(VariableTypes), TypeConverter(typeof(EnumTypeTypeConverter))]
        public Type BackgroundTypeType { get; set; }
        [Category(VariableTypes), TypeConverter(typeof(EnumTypeTypeConverter))]
        public Type BossNumberType { get; set; }

        [Category(VariableTypes), TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding FilenameEncoding { get; set; }
        [Category(VariableTypes), TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding MapNameEncoding { get; set; }
        [Category(VariableTypes), TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding JapaneseNameEncoding { get; set; }

        [Category(BufferSizes)]
        public int TilesetNameBuffer { get; set; }
        [Category(BufferSizes)]
        public int FilenameBuffer { get; set; }        
        [Category(BufferSizes)]
        public int BackgroundNameBuffer { get; set; }
        [Category(BufferSizes)]
        public int Spritesheet1Buffer { get; set; }
        [Category(BufferSizes)]
        public int Spritesheet2Buffer { get; set; }        
        [Category(BufferSizes)]
        public int JapaneseNameBuffer { get; set; }        
        [Category(BufferSizes)]
        public int MapNameBuffer { get; set; }
        [Category(BufferSizes)]
        public int Padding { get; set; }
        [Category(BufferSizes)]
        public int Size => TilesetNameBuffer
                         + FilenameBuffer
                         + Marshal.SizeOf(BackgroundTypeType)
                         + BackgroundNameBuffer
                         + Spritesheet1Buffer
                         + Spritesheet2Buffer
                         + JapaneseNameBuffer
                         + Marshal.SizeOf(BossNumberType)
                         + MapNameBuffer
                         + Padding;        
        public XElement ToXML(string name)
        {
            return new XElement(name,
                    new XElement(nameof(BackgroundTypeType), BackgroundTypeType.FullName),
                    new XElement(nameof(BossNumberType), BossNumberType.FullName),

                    new XElement(nameof(FilenameEncoding), FilenameEncoding.WebName),
                    new XElement(nameof(MapNameEncoding), MapNameEncoding.WebName),
                    new XElement(nameof(JapaneseNameEncoding), JapaneseNameEncoding.WebName),

                    new XElement(nameof(TilesetNameBuffer), TilesetNameBuffer),
                    new XElement(nameof(FilenameBuffer), FilenameBuffer),
                    new XElement(nameof(BackgroundNameBuffer), BackgroundNameBuffer),
                    new XElement(nameof(Spritesheet1Buffer), Spritesheet1Buffer),
                    new XElement(nameof(Spritesheet2Buffer), Spritesheet2Buffer),
                    new XElement(nameof(JapaneseNameBuffer), JapaneseNameBuffer),
                    new XElement(nameof(MapNameBuffer), MapNameBuffer),
                    new XElement(nameof(Padding), Padding)
                );
        }
        public StageEntrySettings(XmlElement xml)
        {
            BackgroundTypeType = Type.GetType(xml[nameof(BackgroundTypeType)].InnerText);
            BossNumberType = Type.GetType(xml[nameof(BossNumberType)].InnerText);

            FilenameEncoding = Encoding.GetEncoding(xml[nameof(FilenameEncoding)].InnerText);
            MapNameEncoding = Encoding.GetEncoding(xml[nameof(MapNameEncoding)].InnerText);
            JapaneseNameEncoding = Encoding.GetEncoding(xml[nameof(JapaneseNameEncoding)].InnerText);

            TilesetNameBuffer = int.Parse(xml[nameof(TilesetNameBuffer)].InnerText);
            FilenameBuffer = int.Parse(xml[nameof(FilenameBuffer)].InnerText);
            BackgroundNameBuffer = int.Parse(xml[nameof(BackgroundNameBuffer)].InnerText);
            Spritesheet1Buffer = int.Parse(xml[nameof(Spritesheet1Buffer)].InnerText);
            Spritesheet2Buffer = int.Parse(xml[nameof(Spritesheet2Buffer)].InnerText);
            JapaneseNameBuffer = int.Parse(xml[nameof(JapaneseNameBuffer)].InnerText);
            MapNameBuffer = int.Parse(xml[nameof(MapNameBuffer)].InnerText);
            Padding = int.Parse(xml[nameof(Padding)].InnerText);
        }
        public StageEntrySettings()
        { }
        public StageEntrySettings(StageTableTypes type)
        {
            switch (type)
            {
                case StageTableTypes.doukutsuexe:
                case StageTableTypes.swdata:
                case StageTableTypes.csmap:
                case StageTableTypes.stagetbl:
                    FilenameEncoding = Encoding.ASCII;
                    TilesetNameBuffer = 0x20;
                    FilenameBuffer = 0x20;
                    BackgroundTypeType = typeof(int);
                    BackgroundNameBuffer = 0x20;
                    Spritesheet1Buffer = 0x20;
                    Spritesheet2Buffer = 0x20;
                    BossNumberType = typeof(sbyte);
                    //stage.tbl is the only one that has japanese names
                    JapaneseNameEncoding = Encoding.GetEncoding(932); //Shift JIS
                    JapaneseNameBuffer = type == StageTableTypes.stagetbl ? 0x20 : 0;
                    MapNameEncoding = Encoding.ASCII;
                    MapNameBuffer = 0x20;
                    Padding = type != StageTableTypes.stagetbl ? 3 : 0;
                    break;
                //this is apparently emulating the MOD_MR format
                //using it makes BL load png images instead of bmp
                //this editor doesn't care though, so use whatever you want :3
                case StageTableTypes.mrmapbin:
                    FilenameEncoding = Encoding.ASCII;
                    TilesetNameBuffer = 0x10;
                    FilenameBuffer = 0x10;
                    BackgroundTypeType = typeof(byte);
                    BackgroundNameBuffer = 0x10;
                    Spritesheet1Buffer = 0x10;
                    Spritesheet2Buffer = 0x10;
                    BossNumberType = typeof(sbyte);
                    MapNameEncoding = Encoding.ASCII;
                    MapNameBuffer = 0x22;
                    Padding = 0;
                    break;
            }
        }
    }

    public enum StageTableTypes
    {
        doukutsuexe = 0,
        swdata,
        csmap,
        stagetbl,
        mrmapbin
    }
    public static class StageTable
    {
        public const string EXEFilter = "Executables (*.exe)|*.exe";
        public const string CSFilter = "Cave Story (Doukutsu.exe)|Doukutsu.exe";
        public const string MRMAPFilter = "CSE2 (mrmap.bin)|mrmap.bin";
        public const string STAGETBLFilter = "CS+ (stage.tbl)|stage.tbl";

        public const int CSStageCount = 95;
        public const int CSStageTableSize = CSStageCount * 0xC8;
        public const int CSStageTableAddress = 0x937B0;

        const string SWDATASectionName = ".swdata";
        public const int SWDATAExpectedAddress = 0x169000;
        const string SWDATAHeader = "Sue's Workshop01";

        const string CSMAPSectionName = ".csmap";

        #region section manipulation

        #region section constants
        private static char[] GetSectionHeaderSafeName(string value)
        {
            var buff = new char[8];
            Extensions.BufferCopy(value.ToCharArray(), buff, 0, 8);
            return buff;
        }

        public static readonly IMAGE_SECTION_HEADER SWDATASectionHeader = new IMAGE_SECTION_HEADER()
        {
            Name = GetSectionHeaderSafeName(SWDATASectionName),
            Characteristics = IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_CODE |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_INITIALIZED_DATA |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_UNINITIALIZED_DATA |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_EXECUTE |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_READ |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_WRITE,
            VirtualAddress = 0x18E000,
        };
        public static readonly IMAGE_SECTION_HEADER CSMAPSectionHeader = new IMAGE_SECTION_HEADER()
        {
            Name = GetSectionHeaderSafeName(CSMAPSectionName),
            Characteristics = IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_INITIALIZED_DATA |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_READ |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_WRITE,
            VirtualAddress = 0xBF000
        };

        #endregion

        public static bool TryDetectTableType(string path, out StageTableTypes type)
        {
            var pe = PEFile.FromFile(path);
            bool SWdata = pe.ContainsSection(SWDATASectionName);
            bool CSMap = pe.ContainsSection(CSMAPSectionName);
            if (SWdata && CSMap)
            {
                type = (StageTableTypes)(-1);
                return false;
            }
            else if (SWdata)
                type = StageTableTypes.swdata;
            else if (CSMap)
                type = StageTableTypes.csmap;
            else
                type = StageTableTypes.doukutsuexe;
            return true;
        }

        /// <summary>
        /// Removes the specificed stage table type from the exe. This does not update any references to the stage table however
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public static void CleanEXE(string path, StageTableTypes type)
        {
            string sectName;
            switch (type)
            {
                case StageTableTypes.csmap:
                    sectName = CSMAPSectionName;
                    break;
                case StageTableTypes.swdata:
                    sectName = SWDATASectionName;
                    break;
                default:
                    return;
            }
            var pe = PEFile.FromFile(path);
            if (pe.ContainsSection(sectName))
            {
                pe.RemoveSection(sectName);
                pe.UpdateSectionLayout();
                pe.WriteFile(path);
            }
        }

        public static bool VerifyStageTable(IList<StageEntry> table, StageTableTypes type)
        {
            return VerifyStageTable(table, type, new StageEntrySettings(type));
        }
        public static bool VerifyStageTable(IList<StageEntry> table, StageTableTypes type, StageEntrySettings settings)
        {
            //everything other than Doukutsu.exe is already verified
            //For Doukutsu.exe, just need to make sure that we aren't exceeding the buffer size
            return type != StageTableTypes.doukutsuexe || table.Count * settings.Size <= CSStageTableSize;
        }

        public static bool VerifySW(string path)
        {
            return VerifySW(PEFile.FromFile(path));
        }
        public static bool VerifySW(PEFile pe)
        {
            //SWData HAS to be at this address, otherwise SW will break the mod on opening it...
            return pe.TryGetSection(SWDATASectionName, out PESection sw) && sw.PhysicalAddress == SWDATAExpectedAddress;
        }

        public static void AddSection(string path, StageTableTypes type)
        {
            if (!(type == StageTableTypes.csmap || type == StageTableTypes.swdata))
                return;
            AddSection(PEFile.FromFile(path), type);
        }
        public static void AddSection(PEFile pe, StageTableTypes type)
        {
            switch (type)
            {
                case StageTableTypes.csmap:
                    if (!pe.ContainsSection(CSMAPSectionName))
                        pe.InsertSection(pe.sections.IndexOf(pe.GetSection(".rsrc")), new PESection(CSMAPSectionHeader));
                    break;
                case StageTableTypes.swdata:
                    if (!pe.ContainsSection(SWDATASectionName))
                        pe.AddSection(new PESection(SWDATASectionHeader));
                    break;
            }
        }

        #endregion

        #region loading

        static StageEntry ReadStage(BinaryReader br, StageEntrySettings settings)
        {
            var s = new StageEntry()
            {
                TilesetName = br.ReadString(settings.TilesetNameBuffer, settings.FilenameEncoding),
                Filename = br.ReadString(settings.FilenameBuffer, settings.FilenameEncoding),
                BackgroundType = (BackgroundTypes)br.Read(settings.BackgroundTypeType),
                BackgroundName = br.ReadString(settings.BackgroundNameBuffer, settings.FilenameEncoding),
                Spritesheet1 = br.ReadString(settings.Spritesheet1Buffer, settings.FilenameEncoding),
                Spritesheet2 = br.ReadString(settings.Spritesheet2Buffer, settings.FilenameEncoding),
                JapaneseName = br.ReadString(settings.JapaneseNameBuffer, settings.JapaneseNameEncoding),
                BossNumber = (BossNumbers)br.Read(settings.BossNumberType),
                MapName = br.ReadString(settings.MapNameBuffer, settings.MapNameEncoding),
            };
            br.BaseStream.Seek(settings.Padding, SeekOrigin.Current);
            return s;
        }
        static List<StageEntry> ReadStages(BinaryReader br, int stageCount, StageEntrySettings settings )
        {
            var stages = new List<StageEntry>(stageCount);
            for(int i = 0; i < stageCount; i++)
            {
                stages.Add(ReadStage(br,settings));
            }
            return stages;
        }
        private static List<StageEntry> ReadSW(BinaryReader br, int stageCount, StageEntrySettings settings)
        {
            var header = br.ReadString(SWDATAHeader.Length, Encoding.ASCII);
            if (header != SWDATAHeader)
                throw new FileLoadException(); //TODO
            var stages = new List<StageEntry>(stageCount);
            while (true)
            {                
                var buffer = br.ReadBytes(settings.Size);
                if (buffer.All(x => x == 0xFF))
                    break;
                using (var buff = new BinaryReader(new MemoryStream(buffer)))
                {
                    stages.Add(ReadStage(buff, settings));
                }
            }
            return stages;
        }

        public static List<StageEntry> Load(string path, StageTableTypes type)
        {
            return Load(path, type, new StageEntrySettings(type));
        }
        public static List<StageEntry> Load(string path, StageTableTypes type, StageEntrySettings settings)
        {
            int stageCount = 0;
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                void SeekToSectionOffset(string sectionName)
                {
                    var pe = PEFile.FromFile(path);
                    if (!pe.TryGetSection(sectionName, out PESection s))
                        throw new FileLoadException(); //TODO text
                    br.BaseStream.Seek(s.PhysicalAddress, SeekOrigin.Begin);
                    stageCount = (int)(s.RawSize / settings.Size);
                }
                switch (type)
                {
                    case StageTableTypes.doukutsuexe:
                        stageCount = CSStageCount;
                        br.BaseStream.Seek(CSStageTableAddress, SeekOrigin.Begin);
                        break;
                    case StageTableTypes.swdata:
                        SeekToSectionOffset(SWDATASectionName);
                        break;
                    case StageTableTypes.csmap:
                        SeekToSectionOffset(CSMAPSectionName);
                        break;
                    case StageTableTypes.stagetbl:
                        stageCount = (int)(br.BaseStream.Length / settings.Size);
                        break;
                    case StageTableTypes.mrmapbin:
                        stageCount = br.ReadInt32();
                        break;
                }
                return type != StageTableTypes.swdata
                    ? ReadStages(br, stageCount, settings)
                    : ReadSW(br, stageCount, settings);
            }
        }

        #endregion

        public static void Save(IList<StageEntry> table, string path, StageTableTypes type)
        {
            Save(table, path, type, new StageEntrySettings(type));
        }
        public static void Save(IList<StageEntry> table, string path, StageTableTypes type, StageEntrySettings settings)
        {
            if (!VerifyStageTable(table, type, settings))
                throw new ArgumentException();
            Write(table, path, type, settings);
            switch(type)
            {
                case StageTableTypes.doukutsuexe:
                case StageTableTypes.swdata:
                case StageTableTypes.csmap:
                    UpdateStageTableLocation(path, type);
                    break;
            }
            if (type == StageTableTypes.swdata && !VerifySW(path))
                throw new WarningException();
        }

        #region saving
        static void WriteSW(IEnumerable<StageEntry> table, BinaryWriter bw, StageEntrySettings settings)
        {
            bw.Write(Encoding.ASCII.GetBytes(SWDATAHeader));
            WriteStages(table, bw, settings);
            bw.Write(Enumerable.Repeat<byte>(0xFF, settings.Size).ToArray());
        }
        static void WriteStages(IEnumerable<StageEntry> table, BinaryWriter bw, StageEntrySettings settings)
        {
            foreach(var stage in table)
            {
                bw.Write(stage.Serialize(settings));
            }
        }
        public static void Write(IList<StageEntry> table, string path, StageTableTypes type)
        {
            Write(table, path, type, new StageEntrySettings(type));
        }
        public static void Write(IList<StageEntry> table, string path, StageTableTypes type, StageEntrySettings settings)
        {
            using(var destinationbw = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite)))
            {
                void UpdateSection(StageTableTypes t)
                {
                    if (t != StageTableTypes.csmap && t != StageTableTypes.swdata)
                        throw new ArgumentException();

                    string section = null;
                    var pe = PEFile.FromFile(path);
                    var buffer = new byte[(t == StageTableTypes.swdata
                                            //header + stage table + ending marker
                                            ? (SWDATAHeader.Length + ((table.Count + 1) * settings.Size))
                                            : (table.Count * settings.Size))
                                         ];
                    using (var bufferWriter = new BinaryWriter(new MemoryStream(buffer, true)))
                    {
                        void HandleSection(string sectionName, Action<IList<StageEntry>,BinaryWriter,StageEntrySettings> stageTableWriter)
                        {
                            bool sectionAdded = false;
                            //add the section if it doesn't exist
                            if (sectionAdded = !pe.ContainsSection(sectionName))
                                AddSection(pe, type);
                            var sect = pe.GetSection(sectionName);
                            //if we had to add the section, or we need to make the section bigger TODO this won't work
                            if (sectionAdded || sect.RawSize < buffer.Length)
                            {
                                //write to the buffer, and update the sectionName to write to
                                stageTableWriter(table, bufferWriter, settings);
                                section = sectionName;
                                sect.VirtualSize = (uint)buffer.Length; //PEUtility.AlignUp((uint)buffer.Length, pe.optionalHeader32.FileAlignment);
                            }
                            else
                            {
                                //otherwise we can directly write to the file and be done
                                destinationbw.BaseStream.Seek(sect.PhysicalAddress, SeekOrigin.Begin);
                                stageTableWriter(table, destinationbw, settings);
                            }
                        }

                        switch(t)
                        {
                            case StageTableTypes.swdata:
                                HandleSection(SWDATASectionName, WriteSW);
                                break;
                            case StageTableTypes.csmap:
                                HandleSection(CSMAPSectionName, WriteStages);
                                break;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        pe.WriteSectionData(section, buffer);
                        pe.UpdateSectionLayout();
                        pe.WriteFile(path);
                    }
                }
                switch(type)
                {
                    case StageTableTypes.doukutsuexe:
                        destinationbw.BaseStream.Seek(CSStageTableAddress, SeekOrigin.Begin);
                        break;
                    case StageTableTypes.csmap:
                    case StageTableTypes.swdata:
                        UpdateSection(type);
                        return;
                    case StageTableTypes.mrmapbin:
                        destinationbw.Write(table.Count);
                        break;
                    //stage.tbl doesn't need anything special
                }
                WriteStages(table, destinationbw, settings);
            }
        }

        #endregion

        #region stage table location updating


        #region stage table references

        public static readonly IReadOnlyList<long> TilesetReferences = new List<long>()
        {
            //tileset?
            0x020C2F,
            0x020C73,
        };
        public static readonly IReadOnlyList<long> PartsReferences = new List<long>()
        {
            //reference 0x0937D0 not 0x0937B0 (file name)
            0x020CB5,
            0x020CF6,
            0x020D38,
        };
        public static readonly IReadOnlyList<long> BackgroundTypeReferences = new List<long>()
        {
            //reference 0x0937F0 not 0x0937B0 (background type)
            0x020D9E,
        };
        public static readonly IReadOnlyList<long> BackgroundNameReferences = new List<long>()
        {
            //reference 0x0937F4 not 0x0937B0 (background)
            0x020D7A,
        };
        public static readonly IReadOnlyList<long> Spritesheet1References = new List<long>()
        {
            //reference 0x093814 not 0x0937B0 (npc tileset 1)
            0x020DD9,
        };
        public static readonly IReadOnlyList<long> Spritesheet2References = new List<long>()
        {
            //reference 0x093834 not 0x0937B0 (npc tileset 2)
            0x020E1C,
        };
        public static readonly IReadOnlyList<long> BossNumberReferences = new List<long>()
        {
            //reference 0x093854 not 0x0937B0 (boss number)
            0x020EA8,
        };
        public static readonly IReadOnlyList<long> MapNameReferences = new List<long>()
        {
            //reference 0x093855 not 0x0937B0 (caption)
            0x020E6A,
        };

        #endregion

        public static void UpdateStageTableLocation(string path, StageTableTypes type)
        {
            UpdateStageTableLocation(path, type, new StageEntrySettings(type));
        }

        public static void UpdateStageTableLocation(string path, StageTableTypes type, StageEntrySettings settings)
        {
            if (type != StageTableTypes.doukutsuexe && type != StageTableTypes.swdata && type != StageTableTypes.csmap)
                return;

            uint startOfStageTable = 0;
            PEFile pe;
            switch(type)
            {
                case StageTableTypes.doukutsuexe:
                    startOfStageTable = CSStageTableAddress;
                    break;
                case StageTableTypes.swdata:
                    pe = PEFile.FromFile(path);
                    if (!pe.TryGetSection(SWDATASectionName, out PESection sw))
                        throw new FileLoadException();
                    startOfStageTable = (uint)(sw.VirtualAddress + SWDATAHeader.Length);    
                    break;
                case StageTableTypes.csmap:
                    pe = PEFile.FromFile(path);
                    if (!pe.TryGetSection(CSMAPSectionName, out PESection cs))
                        throw new FileLoadException();
                    startOfStageTable = pe.optionalHeader32.ImageBase + cs.VirtualAddress;
                    break;
            }

            //update the address
            using (var bw = new BinaryWriter(new FileStream(path, FileMode.Open, FileAccess.Write)))
            {
                void UpdateAddressList(IEnumerable<long> addresses, uint value)
                {
                    foreach(var a in addresses)
                    {
                        bw.BaseStream.Seek(a, SeekOrigin.Begin);
                        bw.Write(value);
                    }
                }
                UpdateAddressList(TilesetReferences, startOfStageTable);
                UpdateAddressList(PartsReferences, startOfStageTable += (uint)settings.TilesetNameBuffer);
                UpdateAddressList(BackgroundTypeReferences, startOfStageTable += (uint)settings.FilenameBuffer);
                UpdateAddressList(BackgroundNameReferences, startOfStageTable += (uint)Marshal.SizeOf(settings.BackgroundTypeType));
                UpdateAddressList(Spritesheet1References, startOfStageTable += (uint)settings.BackgroundNameBuffer);
                UpdateAddressList(Spritesheet2References, startOfStageTable += (uint)settings.Spritesheet1Buffer);
                UpdateAddressList(BossNumberReferences, startOfStageTable += (uint)settings.Spritesheet2Buffer);
                UpdateAddressList(MapNameReferences, startOfStageTable += (uint)Marshal.SizeOf(settings.BossNumberType));
            }
        }

        #endregion
    }
}
