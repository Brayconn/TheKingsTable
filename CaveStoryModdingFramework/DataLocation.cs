using System;
using System.Collections.Generic;
using System.IO;
using PETools;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CaveStoryModdingFramework
{
    public enum DataLocationTypes
    {
        Internal,
        External
    }
    public class DataLocation : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        string filename, sectionName;
        DataLocationTypes dataLocationType;
        bool fixedSize;
        int offset, maximumSize;

        protected void NotifyPropertyChanging([CallerMemberName] string name = "")
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
        protected void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void SetVal<T>(ref T variable, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(variable, value))
            {
                NotifyPropertyChanging(name);
                variable = value;
                NotifyPropertyChanged(name);
            }
        }

        public string Filename { get => filename; set => SetVal(ref filename, value); }
        public DataLocationTypes DataLocationType { get => dataLocationType; set => SetVal(ref dataLocationType, value); }

        public int Offset { get => offset; set => SetVal(ref offset, value); }

        /// <summary>
        /// empty string/null = no section/start of file
        /// </summary>
        public string SectionName { get => sectionName; set => SetVal(ref sectionName, value); }

        /// <summary>
        /// If set, always write either MaximumSize bytes, or enough bytes to overwrite SectionName completely
        /// </summary>
        public bool FixedSize { get => fixedSize; set => SetVal(ref fixedSize, value); }
        /// <summary>
        /// val <= 0 = no maximum
        /// </summary>
        public int MaximumSize { get => maximumSize; set => SetVal(ref maximumSize, value); }

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
        public PESection GetSection()
        {
            var pe = PEFile.FromFile(Filename);
            if (!pe.TryGetSection(SectionName, out var sect))
                throw new KeyNotFoundException();
            return sect;
        }
        public byte[] GetSectionData()
        {
            return GetSection().Data;
        }
        public Stream GetStream(FileMode mode, FileAccess access)
        {
            if (!File.Exists(Filename))
                throw new FileNotFoundException();
            var offset = Math.Max(0, Offset);

            FileStream fs = null;
            switch (DataLocationType)
            {
                case DataLocationTypes.Internal:
                    //for internal data, we might need to go to the PE section and add its offset
                    if (!string.IsNullOrEmpty(SectionName))
                        offset += (int)GetSection().PhysicalAddress;
                    //but other than that it's the same code to get a stream
                    goto case DataLocationTypes.External;
                case DataLocationTypes.External:
                    fs = new FileStream(Filename, mode, access);
                    fs.Seek(offset, SeekOrigin.Begin);
                    break;
            }
            return fs;
        }
        public static byte[] Read(DataLocation location, int length)
        {
            if (!File.Exists(location.Filename))
                throw new FileNotFoundException();
            using(var br = new BinaryReader(location.GetStream(FileMode.Open, FileAccess.Read)))
            {
                return br.ReadBytes(length);
            }            
        }
        public static void Write(DataLocation location, byte[] data)
        {
            //don't write if the data exceeds the max
            if (location.MaximumSize > 0 && data.Length > location.MaximumSize)
                throw new ArgumentOutOfRangeException();

            var offset = Math.Max(0, location.Offset);
            var fileMode = FileMode.Create;
            var fixedSize = location.FixedSize;
            var fixedSizeMakeupLength = location.MaximumSize - data.Length;

            switch (location.DataLocationType)
            {
                case DataLocationTypes.Internal:
                    //can't write to internal data if the file doesn't exist
                    if (!File.Exists(location.Filename))
                        throw new FileNotFoundException();
                                        
                    //grab the requested section where applicable
                    if(!string.IsNullOrEmpty(location.SectionName))
                    {
                        var pe = PEFile.FromFile(location.Filename);
                        if (!pe.TryGetSection(location.SectionName, out var sect))
                            throw new KeyNotFoundException();
#if COMPARE_SECTION_SIZES
                        var nextSect = pe.sections[pe.sections.IndexOf(sect) + 1];
#endif
                        offset += (int)sect.PhysicalAddress;

                        //if we were told to write at the *very* start of the section...
                        if (location.Offset <= 0)
                        {
                            //...we must be overwriting ALL the data
                            fixedSize = true;
                            //calculate the true size of the section for this write
                            var newSectionSize = PEUtility.AlignUp((uint)data.Length, pe.FileAlignment);
                            //use that size to update the makeupLength
                            fixedSizeMakeupLength = (int)(newSectionSize - data.Length);

                            //if this would result in a resize, use the stuff below to make the write
#if COMPARE_SECTION_SIZES
                            if (sect.PhysicalAddress + newSectionSize != nextSect.PhysicalAddress)
#else
                            if(newSectionSize != sect.RawSize)
#endif
                            {
                                //TODO I have some uncertainty about this line but I don't know why
                                pe.WriteSectionData(location.SectionName, data);

                                pe.UpdateSectionLayout();
                                pe.WriteFile(location.Filename);
                                return;
                            }
                            //otherwise we can just use the function below
                        }
                    }

                    fileMode = FileMode.Open;
                    goto case DataLocationTypes.External;
                case DataLocationTypes.External:
                    using (var bw = new BinaryWriter(new FileStream(location.Filename, fileMode, FileAccess.Write)))
                    {
                        bw.Seek(offset, SeekOrigin.Begin);
                        bw.Write(data);
                        //write 00s to fill space when needed
                        if (location.FixedSize && fixedSizeMakeupLength > 0)
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
}
