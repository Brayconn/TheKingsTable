using System;
using System.Collections.Generic;
using System.IO;
using PETools;
using CaveStoryModdingFramework.Stages;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;

namespace CaveStoryModdingFramework
{
    public enum DataLocationTypes
    {
        Internal,
        External
    }
    public class DataLocation
    {
        public string Filename { get; set; }
        public DataLocationTypes DataLocationType { get; set; }

        public int Offset { get; set; }

        /// <summary>
        /// empty string/null = no section/start of file
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// If set, always write either MaximumSize bytes, or enough bytes to overwrite SectionName completely
        /// </summary>
        public bool FixedSize { get; set; }
        /// <summary>
        /// val <= 0 = no maximum
        /// </summary>
        public int MaximumSize { get; set; }

        public static string GetSectionHeaderSafeName(string value)
        {
            return GetSectionHeaderSafeName(value, Encoding.ASCII);
        }
        public static string GetSectionHeaderSafeName(string value, Encoding encoding)
        {
            var outBuff = new byte[IMAGE_SECTION_HEADER.IMAGE_SIZEOF_SHORT_NAME];
            var inBuff = encoding.GetBytes(value);
            var len = Math.Min(outBuff.Length, inBuff.Length);
            for (int i = 0; i < len; i++)
                outBuff[i] = inBuff[i];
            return encoding.GetString(outBuff);
        }
        protected PESection GetSection()
        {
            return GetSection(Filename);
        }
        protected PESection GetSection(string filename)
        {
            var pe = PEFile.FromFile(filename);
            if (!pe.TryGetSection(SectionName, out var sect))
                throw new KeyNotFoundException();
            return sect;
        }
        protected byte[] GetSectionData()
        {
            return GetSectionData(Filename);
        }
        protected byte[] GetSectionData(string filename)
        {
            return GetSection(filename).Data;
        }

        public Stream GetStream(FileMode mode, FileAccess access)
        {
            return GetStream(Filename, mode, access);
        }
        public Stream GetStream(string filename, FileMode mode, FileAccess access)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();
            var offset = Math.Max(0, Offset);

            FileStream fs = null;
            switch (DataLocationType)
            {
                case DataLocationTypes.Internal:
                    //for internal data, we might need to go to the PE section and add its offset
                    if (!string.IsNullOrEmpty(SectionName))
                    {
                        offset += (int)GetSection(filename).PhysicalAddress;
                    }
                    //but other than that it's the same code to get a stream
                    goto case DataLocationTypes.External;
                case DataLocationTypes.External:
                    fs = new FileStream(filename, mode, access);
                    fs.Seek(offset, SeekOrigin.Begin);
                    break;
            }
            return fs;
        }

        public byte[] Read(int length)
        {
            return Read(Filename, length);
        }
        public byte[] Read(string path, int length)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            using(var br = new BinaryReader(GetStream(path, FileMode.Open, FileAccess.Read)))
            {
                return br.ReadBytes(length);
            }            
        }
        public void Write(byte[] data)
        {
            Write(Filename, data);
        }
        public void Write(string path, byte[] data)
        {
            //don't write if the data exceeds the max
            if (MaximumSize > 0 && data.Length > MaximumSize)
                throw new ArgumentOutOfRangeException();

            var offset = Math.Max(0, Offset);
            var fileMode = FileMode.Create;
            var fixedSizeMakeupLength = MaximumSize - data.Length;

            switch (DataLocationType)
            {
                case DataLocationTypes.Internal:
                    //can't write to internal data if the file doesn't exist
                    if (!File.Exists(path))
                        throw new FileNotFoundException();
                                        
                    //grab the requested section where applicable
                    if(!string.IsNullOrEmpty(SectionName))
                    {
                        var pe = PEFile.FromFile(path);
                        if (!pe.TryGetSection(SectionName, out var sect))
                            throw new KeyNotFoundException();
                        offset += (int)sect.PhysicalAddress;

                        //if we were told to write at the *very* start of the section, we must be overwriting ALL the data
                        if (Offset <= 0)
                        {
                            //calculate the true size of the section for this write
                            var newSectionSize = PEUtility.AlignUp((uint)data.Length, pe.FileAlignment);
                            //use that size to update the makeupLength
                            fixedSizeMakeupLength = (int)(newSectionSize - data.Length);

                            //if this would result in a resize, use the stuff below to make the write
                            if (newSectionSize != sect.RawSize)
                            {
                                //TODO I have some uncertainty about this line but I don't know why
                                pe.WriteSectionData(SectionName, data);

                                pe.UpdateSectionLayout();
                                pe.WriteFile(path);
                                return;
                            }
                            //otherwise we can just use the function below
                        }
                    }

                    fileMode = FileMode.Open;
                    goto case DataLocationTypes.External;
                case DataLocationTypes.External:
                    using (var bw = new BinaryWriter(new FileStream(path, fileMode, FileAccess.Write)))
                    {
                        bw.Seek(offset, SeekOrigin.Begin);
                        bw.Write(data);
                        //write 00s to fill space when needed
                        if (FixedSize && fixedSizeMakeupLength > 0)
                            bw.Write(new byte[fixedSizeMakeupLength]);
                    }
                    break;
            }
        }

        public virtual XElement ToXML(string elementName, string relativeBase)
        {
            return new XElement(elementName,
                    new XElement(nameof(Filename), AssetManager.MakeRelative(relativeBase, Filename)),
                    new XElement(nameof(DataLocationType), DataLocationType),
                    new XElement(nameof(Offset), Offset),
                    new XElement(nameof(SectionName), SectionName),
                    new XElement(nameof(FixedSize), FixedSize),
                    new XElement(nameof(MaximumSize), MaximumSize)
                );
        }
    }
    public class StageTableLocation : DataLocation
    {
        public StageTableFormats StageTableFormat { get; set; }
        public int StageCount { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageTableReferences InternalStageTableReferences { get; } = new StageTableReferences();

        public StageTableLocation(string path)
        {
            Filename = path;
        }
        public StageTableLocation(string path, StageTablePresets type) : this(path)
        {
            ResetToDefault(type);
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

        public static readonly Dictionary<StageTableLocation, StageTablePresets> Presets;
        static StageTableLocation()
        {
            var values = Enum.GetValues(typeof(StageTablePresets));
            Presets = new Dictionary<StageTableLocation, StageTablePresets>(values.Length);
            foreach(StageTablePresets item in values)
            {
                //custom has no preset so we avoid it
                if(item != StageTablePresets.custom)
                    Presets.Add(new StageTableLocation("", item), item);
            }
        }

        public List<StageEntry> Read(StageEntrySettings settings)
        {
            return Read(Filename, settings);
        }
        public List<StageEntry> Read(string filename, StageEntrySettings settings)
        {
            var stageCount = StageCount;
            //an offset of 0 (or less) means we're probably using the whole section/file...
            if (Offset <= 0)
            {
                //if it's a section, use the length of it
                if (DataLocationType == DataLocationTypes.Internal && !string.IsNullOrEmpty(SectionName))
                {
                    stageCount = (int)(GetSection(filename).RawSize / settings.Size);
                }
                //if it's a file, use the length of that
                else if (DataLocationType == DataLocationTypes.External)
                {
                    stageCount = (int)(new FileInfo(filename).Length / settings.Size);
                }
                //if we get here that means we just have the stage table right at the beginning of the entire file...?!
                else
                    throw new ArgumentException("Is your stage table seriously at the very start of an exe?!", $"{nameof(Offset)}, {nameof(SectionName)}");
            }
            using (var br = new BinaryReader(GetStream(filename, FileMode.Open, FileAccess.Read)))
            {
                switch (StageTableFormat)
                {
                    case StageTableFormats.normal:
                        return br.ReadStages(stageCount, settings);
                    case StageTableFormats.swdata:
                        return br.ReadSWData(stageCount, settings);
                    case StageTableFormats.mrmapbin:
                        stageCount = br.ReadInt32();
                        return br.ReadStages(stageCount, settings);
                    default:
                        throw new ArgumentException("Invalid stage table format!", nameof(StageTableFormat));
                }
            }
        }

        public void Write(IList<StageEntry> stages, StageEntrySettings settings)
        {
            Write(Filename, stages, settings);            
        }
        public void Write(string filename, IList<StageEntry> stages, StageEntrySettings settings)
        {
            var size = StageTable.GetBufferSize(StageTableFormat, stages.Count, settings);
            //stop if we're about to write too much data
            if (MaximumSize > 0 && size > MaximumSize)
                throw new ArgumentOutOfRangeException();
            var buffer = new byte[size];

            //if we're saving to an internal file, and need to go off of a section...
            if (DataLocationType == DataLocationTypes.Internal && !string.IsNullOrEmpty(SectionName))
            {
                PEFile pe = PEFile.FromFile(filename);
                //...but the requested section doesn't exist...
                if (!pe.ContainsSection(SectionName))
                {
                    //...we might be able to fix it!
                    switch (SectionName)
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
                    pe.WriteFile(filename);
                }
            }
            using (var bw = new BinaryWriter(new MemoryStream(buffer)))
            {
                switch (StageTableFormat)
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
            Write(filename, buffer);
            StageTable.PatchStageTableLocation(this, settings, InternalStageTableReferences);
        }
    }
}
