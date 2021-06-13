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
    public enum StageTableFormats
    {
        normal,
        swdata,
        mrmapbin,
    }
    public enum StageTablePresets
    {
        custom = -1,
        doukutsuexe = 0,
        swdata,
        csmap,
        stagetbl,
        mrmapbin
    }

    #region type converters

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
            if (value is string s)
            {
                var t = Type.GetType(s, false, true);
                if (integerTypes.Contains(t))
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

    #endregion

    public class StageTableLocation : DataLocation
    {
        StageTableFormats stageTableFormat;
        int stageCount;
        public StageTableFormats StageTableFormat { get => stageTableFormat; set => SetVal(ref stageTableFormat, value); }
        public int StageCount { get => stageCount; set => SetVal(ref stageCount, value); }

        public StageTableLocation(string path)
        {
            Filename = path;
        }
        public StageTableLocation(string path, StageTablePresets type) : this(path)
        {
            ResetToDefault(type);
        }
        public StageTableLocation(XmlElement xml, string filename)
        {
            Filename = filename;
            //Filename = xml[nameof(Filename)].InnerText;
            DataLocationType = (DataLocationTypes)Enum.Parse(typeof(DataLocationTypes), xml[nameof(DataLocationType)].InnerText);
            Offset = int.Parse(xml[nameof(Offset)].InnerText);
            SectionName = xml[nameof(SectionName)].InnerText;
            FixedSize = bool.Parse(xml[nameof(FixedSize)].InnerText);
            MaximumSize = int.Parse(xml[nameof(MaximumSize)].InnerText);

            StageTableFormat = (StageTableFormats)Enum.Parse(typeof(StageTableFormats), xml[nameof(StageTableFormat)].InnerText);
            StageCount = int.Parse(xml[nameof(StageCount)].InnerText);
        }
        public override XElement ToXML(string elementName, string relativeBase)
        {
            var x = base.ToXML(elementName, relativeBase);
            x.Add(
                new XElement(nameof(StageTableFormat), StageTableFormat),
                new XElement(nameof(StageCount), StageCount)
                );
            return x;
        }
        public void ResetToDefault(StageTablePresets type)
        {
            switch (type)
            {
                case StageTablePresets.doukutsuexe:
                    DataLocationType = DataLocationTypes.Internal;
                    SectionName = "";
                    Offset = StageTable.CSStageTableAddress;
                    MaximumSize = StageTable.CSStageTableSize;
                    FixedSize = true;
                    StageTableFormat = StageTableFormats.normal;
                    StageCount = StageTable.CSStageCount;
                    break;
                case StageTablePresets.swdata:
                    DataLocationType = DataLocationTypes.Internal;
                    SectionName = StageTable.SWDATASectionName;
                    Offset = 0;
                    MaximumSize = 0;
                    FixedSize = false;
                    StageTableFormat = StageTableFormats.swdata;
                    StageCount = 0;
                    break;
                case StageTablePresets.csmap:
                    DataLocationType = DataLocationTypes.Internal;
                    SectionName = StageTable.CSMAPSectionName;
                    Offset = 0;
                    MaximumSize = 0;
                    FixedSize = false;
                    StageTableFormat = StageTableFormats.normal;
                    StageCount = 0;
                    break;
                case StageTablePresets.stagetbl:
                case StageTablePresets.mrmapbin:
                    DataLocationType = DataLocationTypes.External;
                    SectionName = "";
                    Offset = 0;
                    MaximumSize = type == StageTablePresets.stagetbl ? StageTable.STAGETBLSize : 0;
                    FixedSize = false; //TODO check on the real limits of stage.tbl
                    StageTableFormat = type == StageTablePresets.stagetbl ? StageTableFormats.normal : StageTableFormats.mrmapbin;
                    StageCount = 0;
                    break;
                case StageTablePresets.custom:
                    throw new ArgumentException("There is no preset for custom", nameof(type));
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is StageTableLocation l)
            {
                return DataLocationType == l.DataLocationType &&
                    SectionName == l.SectionName &&
                    Offset == l.Offset &&
                    MaximumSize == l.MaximumSize &&
                    FixedSize == l.FixedSize &&
                    StageTableFormat == l.StageTableFormat &&
                    StageCount == l.StageCount;
            }
            else return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return (2 * DataLocationType.GetHashCode()) +
                (3 * SectionName.GetHashCode()) +
                (5 * Offset.GetHashCode()) +
                (7 * MaximumSize.GetHashCode()) +
                (11 * FixedSize.GetHashCode()) +
                (13 * StageTableFormat.GetHashCode()) +
                (17 * StageCount.GetHashCode());
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

                    new XElement(nameof(FilenameEncoding), FilenameEncoding?.WebName ?? ""),
                    new XElement(nameof(MapNameEncoding), MapNameEncoding?.WebName ?? ""),
                    new XElement(nameof(JapaneseNameEncoding), JapaneseNameEncoding?.WebName ?? ""),

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

            Encoding ReadEncoding(XmlElement element)
            {
                var str = element?.InnerText;
                if (!string.IsNullOrWhiteSpace(str))
                    return Encoding.GetEncoding(str);
                else
                    return null;
            }
            FilenameEncoding = ReadEncoding(xml[nameof(FilenameEncoding)]);
            MapNameEncoding = ReadEncoding(xml[nameof(MapNameEncoding)]);
            JapaneseNameEncoding = ReadEncoding(xml[nameof(JapaneseNameEncoding)]);

            TilesetNameBuffer = int.Parse(xml[nameof(TilesetNameBuffer)].InnerText);
            FilenameBuffer = int.Parse(xml[nameof(FilenameBuffer)].InnerText);
            BackgroundNameBuffer = int.Parse(xml[nameof(BackgroundNameBuffer)].InnerText);
            Spritesheet1Buffer = int.Parse(xml[nameof(Spritesheet1Buffer)].InnerText);
            Spritesheet2Buffer = int.Parse(xml[nameof(Spritesheet2Buffer)].InnerText);
            JapaneseNameBuffer = int.Parse(xml[nameof(JapaneseNameBuffer)].InnerText);
            MapNameBuffer = int.Parse(xml[nameof(MapNameBuffer)].InnerText);
            Padding = int.Parse(xml[nameof(Padding)].InnerText);
        }
        public void ResetToDefault(StageTablePresets type)
        {
            switch (type)
            {
                case StageTablePresets.doukutsuexe:
                case StageTablePresets.swdata:
                case StageTablePresets.csmap:
                case StageTablePresets.stagetbl:
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
                    JapaneseNameBuffer = type == StageTablePresets.stagetbl ? 0x20 : 0;
                    MapNameEncoding = Encoding.ASCII;
                    MapNameBuffer = 0x20;
                    Padding = type != StageTablePresets.stagetbl ? 3 : 0;
                    break;
                //this is apparently emulating the MOD_MR format
                //using it makes BL load png images instead of bmp
                //this editor doesn't care though, so use whatever you want :3
                case StageTablePresets.mrmapbin:
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
                case StageTablePresets.custom:
                    throw new ArgumentException("No preset for custom!", nameof(type));
            }
        }
        public StageEntrySettings()
        { }
        public StageEntrySettings(StageTablePresets type)
        {
            ResetToDefault(type);
        }
        public override bool Equals(object obj)
        {
            if(obj is StageEntrySettings s)
            {
                return FilenameEncoding == s.FilenameEncoding &&
                    TilesetNameBuffer == s.TilesetNameBuffer &&
                    FilenameBuffer == s.FilenameBuffer &&
                    BackgroundTypeType == s.BackgroundTypeType &&
                    BackgroundNameBuffer == s.BackgroundNameBuffer &&
                    Spritesheet1Buffer == s.Spritesheet1Buffer &&
                    Spritesheet2Buffer == s.Spritesheet2Buffer &&
                    JapaneseNameEncoding == s.JapaneseNameEncoding &&
                    JapaneseNameBuffer == s.JapaneseNameBuffer &&
                    BossNumberType == s.BossNumberType &&
                    MapNameBuffer == s.MapNameBuffer &&
                    Padding == s.Padding;
            }
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return (2 * FilenameEncoding?.GetHashCode() ?? 0) +
                (3 * TilesetNameBuffer.GetHashCode()) +
                (5 * FilenameBuffer.GetHashCode()) +
                (7 * BackgroundTypeType.GetHashCode()) +
                (11 * BackgroundNameBuffer.GetHashCode()) +
                (13 * Spritesheet1Buffer.GetHashCode()) +
                (17 * Spritesheet2Buffer.GetHashCode()) +
                (19 * JapaneseNameEncoding?.GetHashCode() ?? 0) +
                (23 * JapaneseNameBuffer.GetHashCode()) +
                (29 * BossNumberType.GetHashCode()) +
                (31 * MapNameBuffer.GetHashCode()) +
                (37 * Padding.GetHashCode());
        }
    }

    public class StageTableReferences
    {
        public List<long> TilesetReferences { get; set; } = new List<long>()
        {
            //tileset?
            0x020C2F,
            0x020C73,
        };
        public List<long> FilenameReferences { get; set; } = new List<long>()
        {
            //reference 0x0937D0 not 0x0937B0 (file name)
            0x020CB5,
            0x020CF6,
            0x020D38,
        };
        public List<long> BackgroundTypeReferences { get; set; } = new List<long>()
        {
            //reference 0x0937F0 not 0x0937B0 (background type)
            0x020D9E,
        };
        public List<long> BackgroundNameReferences { get; set; } = new List<long>()
        {
            //reference 0x0937F4 not 0x0937B0 (background)
            0x020D7A,
        };
        public List<long> Spritesheet1References { get; set; } = new List<long>()
        {
            //reference 0x093814 not 0x0937B0 (npc tileset 1)
            0x020DD9,
        };
        public List<long> Spritesheet2References { get; set; } = new List<long>()
        {
            //reference 0x093834 not 0x0937B0 (npc tileset 2)
            0x020E1C,
        };
        public List<long> BossNumberReferences { get; set; } = new List<long>()
        {
            //reference 0x093854 not 0x0937B0 (boss number)
            0x020EA8,
        };
        public List<long> MapNameReferences { get; set; } = new List<long>()
        {
            //reference 0x093855 not 0x0937B0 (caption)
            0x020E6A,
        };
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
            if (!EqualityComparer<T>.Default.Equals(variable, value))
            {
                NotifyPropertyChanging(name);
                variable = value;
                NotifyPropertyChanged(name);
            }
        }

        string tilesetName, filename, backgroundName, spritesheet1, spritesheet2, japaneseName, mapName;
        long backgroundType, bossNumber;
        public string TilesetName { get => tilesetName; set => SetVal(ref tilesetName, value); }
        //SW doesn't allow "\empty" as a name, it won't show them n stuff
        public string Filename { get => filename; set => SetVal(ref filename, value); }
        public long BackgroundType { get => backgroundType; set => SetVal(ref backgroundType, value); }
        public string BackgroundName { get => backgroundName; set => SetVal(ref backgroundName, value); }
        public string Spritesheet1 { get => spritesheet1; set => SetVal(ref spritesheet1, value); }
        public string Spritesheet2 { get => spritesheet2; set => SetVal(ref spritesheet2, value); }
        public long BossNumber { get => bossNumber; set => SetVal(ref bossNumber, value); }
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

    public static class StageTable
    {
        #region constants

        public const string EXEFilter = "Executables (*.exe)|*.exe";
        public const string CSFilter = "Cave Story (Doukutsu.exe)|Doukutsu.exe";
        public const string MRMAPFilter = "CSE2 (mrmap.bin)|mrmap.bin";
        public const string STAGETBLFilter = "CS+ (stage.tbl)|stage.tbl";

        public const int CSStageCount = 95;
        public const int CSStageTableEntrySize = 0xC8;
        public const int CSStageTableSize = CSStageCount * CSStageTableEntrySize;
        public const int CSStageTableAddress = 0x937B0;

        public const string SWDATASectionName = ".swdata";
        public const int SWDATAExpectedAddress = 0x169000;
        public const string SWDATAHeader = "Sue's Workshop01";
        //Belongs at index after .rsrc
        public static readonly IMAGE_SECTION_HEADER SWDATASectionHeader = new IMAGE_SECTION_HEADER()
        {
            Name = DataLocation.GetSectionHeaderSafeName(SWDATASectionName).ToCharArray(),
            Characteristics = IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_CODE |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_INITIALIZED_DATA |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_UNINITIALIZED_DATA |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_EXECUTE |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_READ |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_WRITE,
            VirtualAddress = 0x18E000,
        };

        public const string CSMAPSectionName = ".csmap";
        //Belongs at index before .rsrc
        public static readonly IMAGE_SECTION_HEADER CSMAPSectionHeader = new IMAGE_SECTION_HEADER()
        {
            Name = DataLocation.GetSectionHeaderSafeName(CSMAPSectionName).ToCharArray(),
            Characteristics = IMAGE_SECTION_FLAGS.IMAGE_SCN_CNT_INITIALIZED_DATA |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_READ |
                              IMAGE_SECTION_FLAGS.IMAGE_SCN_MEM_WRITE,
            VirtualAddress = 0xBF000
        };

        public const string STAGETBL = "stage.tbl";
        public const int STAGETBLEntrySize = 0xE5;
        public const int STAGETBLSize = CSStageCount * STAGETBLEntrySize;

        public const string MRMAPBIN = "mrmap.bin";

        #endregion

        #region section manipulation

        public static bool TryDetectTableType(string path, out StageTablePresets type)
        {
            var pe = PEFile.FromFile(path);
            bool SWdata = pe.ContainsSection(SWDATASectionName);
            bool CSMap = pe.ContainsSection(CSMAPSectionName);
            if (SWdata && CSMap)
            {
                type = (StageTablePresets)(-1);
                return false;
            }
            else if (SWdata)
                type = StageTablePresets.swdata;
            else if (CSMap)
                type = StageTablePresets.csmap;
            else
                type = StageTablePresets.doukutsuexe;
            return true;
        }

        /// <summary>
        /// Removes the specificed stage table type from the exe. This does not update any references to the stage table however
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public static void CleanEXE(string path, StageTablePresets type)
        {
            string sectName;
            switch (type)
            {
                case StageTablePresets.csmap:
                    sectName = CSMAPSectionName;
                    break;
                case StageTablePresets.swdata:
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

        public static bool VerifySW(string path)
        {
            return VerifySW(PEFile.FromFile(path));
        }
        public static bool VerifySW(PEFile pe)
        {
            //SWData HAS to be at this address, otherwise SW will break the mod on opening it...
            return pe.TryGetSection(SWDATASectionName, out PESection sw) && sw.PhysicalAddress == SWDATAExpectedAddress;
        }

        public static void AddSection(string path, StageTablePresets type)
        {
            if (!(type == StageTablePresets.csmap || type == StageTablePresets.swdata))
                return;
            AddSection(PEFile.FromFile(path), type);
        }
        public static void AddSection(PEFile pe, StageTablePresets type)
        {
            switch (type)
            {
                case StageTablePresets.csmap:
                    if (!pe.ContainsSection(CSMAPSectionName))
                        pe.InsertSection(pe.sections.IndexOf(pe.GetSection(".rsrc")), new PESection(CSMAPSectionHeader));
                    break;
                case StageTablePresets.swdata:
                    if (!pe.ContainsSection(SWDATASectionName))
                        pe.AddSection(new PESection(SWDATASectionHeader));
                    break;
            }
        }

        #endregion

        public static int GetBufferSize(StageTableFormats format, int stageCount, StageEntrySettings settings)
        {
            switch (format)
            {
                case StageTableFormats.normal:
                    return stageCount * settings.Size;
                case StageTableFormats.swdata:
                    return SWDATAHeader.Length + ((stageCount + 1) * settings.Size);
                case StageTableFormats.mrmapbin:
                    return sizeof(int) + stageCount * settings.Size;
                default:
                    return -1;
            }
        }

        public static bool TryDetectPreset(StageTableLocation location, StageEntrySettings settings, out StageTablePresets preset)
        {
            preset = StageTablePresets.custom;
            switch (location.DataLocationType)
            {
                case DataLocationTypes.Internal:
                    //doukutsuexe, csmap, and swdata share the same entry settings
                    var internalSettings = new StageEntrySettings(StageTablePresets.doukutsuexe);
                    if(settings == internalSettings)
                    {
                        if(location == new StageTableLocation("", StageTablePresets.doukutsuexe))
                        {
                            preset = StageTablePresets.doukutsuexe;
                        }
                        else if(location == new StageTableLocation("", StageTablePresets.csmap))
                        {
                            preset = StageTablePresets.csmap;
                        }
                        else if(location == new StageTableLocation("", StageTablePresets.swdata))
                        {
                            preset = StageTablePresets.swdata;
                        }
                    }
                    break;
                case DataLocationTypes.External:
                    if(location == new StageTableLocation("", StageTablePresets.stagetbl) && settings == new StageEntrySettings(StageTablePresets.stagetbl))
                    {
                        preset = StageTablePresets.stagetbl;
                    }
                    else if(location == new StageTableLocation("", StageTablePresets.mrmapbin) && settings == new StageEntrySettings(StageTablePresets.mrmapbin))
                    {
                        preset = StageTablePresets.mrmapbin;
                    }
                    break;
            }
            return preset != StageTablePresets.custom;
        }

        #region Reading
        public static StageEntry ReadStage(this BinaryReader br, StageEntrySettings settings)
        {
            var s = new StageEntry()
            {
                TilesetName = br.ReadString(settings.TilesetNameBuffer, settings.FilenameEncoding),
                Filename = br.ReadString(settings.FilenameBuffer, settings.FilenameEncoding),
                BackgroundType = (long)br.Read(settings.BackgroundTypeType),
                BackgroundName = br.ReadString(settings.BackgroundNameBuffer, settings.FilenameEncoding),
                Spritesheet1 = br.ReadString(settings.Spritesheet1Buffer, settings.FilenameEncoding),
                Spritesheet2 = br.ReadString(settings.Spritesheet2Buffer, settings.FilenameEncoding),
                JapaneseName = br.ReadString(settings.JapaneseNameBuffer, settings.JapaneseNameEncoding),
                BossNumber = (long)br.Read(settings.BossNumberType),
                MapName = br.ReadString(settings.MapNameBuffer, settings.MapNameEncoding),
            };
            br.BaseStream.Seek(settings.Padding, SeekOrigin.Current);
            return s;
        }
        public static List<StageEntry> ReadStages(this BinaryReader br, int stageCount, StageEntrySettings settings )
        {
            var stages = new List<StageEntry>(stageCount);
            for(int i = 0; i < stageCount; i++)
            {
                stages.Add(ReadStage(br,settings));
            }
            return stages;
        }
        public static List<StageEntry> ReadSWData(this BinaryReader br, int stageCount, StageEntrySettings settings)
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
        public static List<StageEntry> Read(StageTableLocation location, StageEntrySettings settings)
        {
            var stageCount = location.StageCount;
            //an offset of 0 (or less) means we're probably using the whole section/file...
            if (location.Offset <= 0)
            {
                switch(location.DataLocationType)
                {
                    case DataLocationTypes.Internal:
                        //if it's a section, use the length of it
                        if (!string.IsNullOrEmpty(location.SectionName))
                            stageCount = (int)(location.GetSection().RawSize / settings.Size);
                        //if we get here that means we just have the stage table right at the beginning of the entire file...?!
                        else
                            throw new ArgumentException("Is your stage table seriously at the very start of an exe?!",
                                $"{nameof(location.Offset)}, {nameof(location.SectionName)}");
                        break;
                    //if it's a file, use the length of that
                    case DataLocationTypes.External:
                        stageCount = (int)(new FileInfo(location.Filename).Length / settings.Size);
                        break;
                }                
            }
            using (var br = new BinaryReader(location.GetStream(FileMode.Open, FileAccess.Read)))
            {
                switch (location.StageTableFormat)
                {
                    case StageTableFormats.mrmapbin:
                        stageCount = br.ReadInt32();
                        goto case StageTableFormats.normal;
                    case StageTableFormats.normal:
                        return br.ReadStages(stageCount, settings);
                    case StageTableFormats.swdata:
                        return br.ReadSWData(stageCount, settings);
                    default:
                        throw new ArgumentException("Invalid stage table format!", nameof(location.StageTableFormat));
                }
            }
        }
        #endregion

        #region Writing
        public static void WriteStages(this BinaryWriter bw, IEnumerable<StageEntry> table, StageEntrySettings settings)
        {
            foreach(var stage in table)
            {
                bw.Write(stage.Serialize(settings));
            }
        }
        public static void Write(IList<StageEntry> stages, StageTableLocation location, StageEntrySettings settings, StageTableReferences references)
        {
            Write(stages, location, settings);
            if(location.DataLocationType == DataLocationTypes.Internal)
                PatchStageTableLocation(location, settings, references);
        }
        public static void Write(IList<StageEntry> stages, StageTableLocation location, StageEntrySettings settings)
        {
            var size = StageTable.GetBufferSize(location.StageTableFormat, stages.Count, settings);
            //stop if we're about to write too much data
            if (location.MaximumSize > 0 && size > location.MaximumSize)
                throw new ArgumentOutOfRangeException();
            var buffer = new byte[size];

            //if we're saving to an internal file, and need to go off of a section...
            if (location.DataLocationType == DataLocationTypes.Internal && !string.IsNullOrEmpty(location.SectionName))
            {
                PEFile pe = PEFile.FromFile(location.Filename);
                //...but the requested section doesn't exist...
                if (!pe.ContainsSection(location.SectionName))
                {
                    //...we might be able to fix it!
                    switch (location.SectionName)
                    {
                        case StageTable.CSMAPSectionName:
                            pe.InsertSection(pe.sections.IndexOf(pe.GetSection(".rsrc")), new PESection(StageTable.CSMAPSectionHeader)
                            {
                                Data = new byte[size]
                            });
                            break;
                        case StageTable.SWDATASectionName:
                            pe.AddSection(new PESection(StageTable.SWDATASectionHeader)
                            {
                                Data = new byte[size]
                            });
                            break;
                        //...or not
                        default:
                            throw new KeyNotFoundException();
                    }
                    pe.UpdateSectionLayout();
                    pe.WriteFile(location.Filename);
                }
            }
            using (var bw = new BinaryWriter(new MemoryStream(buffer)))
            {
                switch (location.StageTableFormat)
                {
                    case StageTableFormats.mrmapbin:
                        bw.Write((int)stages.Count);
                        bw.WriteStages(stages, settings);
                        break;
                    case StageTableFormats.normal:
                        bw.WriteStages(stages, settings);
                        break;
                    case StageTableFormats.swdata:
                        bw.Write(Encoding.ASCII.GetBytes(StageTable.SWDATAHeader));
                        bw.WriteStages(stages, settings);
                        bw.Write(Enumerable.Repeat<byte>(0xFF, settings.Size).ToArray());
                        break;
                }
            }
            DataLocation.Write(location, buffer);
        }

        public static void PatchStageTableLocation(StageTableLocation location, StageEntrySettings settings, StageTableReferences r)
        {
            if (location.DataLocationType != DataLocationTypes.Internal)
                throw new ArgumentException("Can only patch internal stage tables!", nameof(location.DataLocationType));
            var pe = PEFile.FromFile(location.Filename);
            uint startOfStageTable = pe.optionalHeader32.ImageBase;
            bool set = false;
            if(!string.IsNullOrEmpty(location.SectionName))
            {
                if (!pe.TryGetSection(location.SectionName, out var sect))
                    throw new KeyNotFoundException();
                startOfStageTable += sect.VirtualAddress;
                set = true;
            }

            using (var bw = new BinaryWriter(location.GetStream(FileMode.Open, FileAccess.ReadWrite)))
            {
                //this might not work in *every* case, but it can be circumvented by using the actual section name anyways, so...
                if(!set)
                    startOfStageTable += (uint)bw.BaseStream.Position;
                switch (location.StageTableFormat)
                {
                    case StageTableFormats.swdata:
                        startOfStageTable += (uint)StageTable.SWDATAHeader.Length;
                        break;
                    case StageTableFormats.mrmapbin:
                        startOfStageTable += (uint)sizeof(int);
                        break;
                }
                void UpdateAddressList(IEnumerable<long> addresses, uint value)
                {
                    foreach (var a in addresses)
                    {
                        bw.BaseStream.Seek(a, SeekOrigin.Begin);
                        bw.Write(value);
                    }
                }
                UpdateAddressList(r.TilesetReferences, startOfStageTable);
                UpdateAddressList(r.FilenameReferences, startOfStageTable += (uint)settings.TilesetNameBuffer);
                UpdateAddressList(r.BackgroundTypeReferences, startOfStageTable += (uint)settings.FilenameBuffer);
                UpdateAddressList(r.BackgroundNameReferences, startOfStageTable += (uint)Marshal.SizeOf(settings.BackgroundTypeType));
                UpdateAddressList(r.Spritesheet1References, startOfStageTable += (uint)settings.BackgroundNameBuffer);
                UpdateAddressList(r.Spritesheet2References, startOfStageTable += (uint)settings.Spritesheet1Buffer);
                UpdateAddressList(r.BossNumberReferences, startOfStageTable += (uint)settings.Spritesheet2Buffer);
                UpdateAddressList(r.MapNameReferences, startOfStageTable += (uint)Marshal.SizeOf(settings.BossNumberType));
            }
        }
        #endregion
    }
}
