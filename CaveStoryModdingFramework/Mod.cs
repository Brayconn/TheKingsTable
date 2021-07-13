using CaveStoryModdingFramework.Entities;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using LocalizeableComponentModel;
using System.Text;
using System.Drawing;
using System.Xml;
using CaveStoryModdingFramework.TSC;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace CaveStoryModdingFramework
{
    [XmlRoot(XmlBase)]
    public class Mod : IXmlSerializable
    {
        public static readonly string CaveStoryProjectFilter = $"{Dialog.ModFilterText} (*.cav)|*.cav";
        #region stage table
        
        public event EventHandler StageTableTypeChanged;
        
        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog))]
        [XmlIgnore]
        public StageTablePresets StageTablePreset
        {
            get
            {
                if (Stages.StageTable.TryDetectPreset(StageTableLocation, StageTableSettings, out var preset) &&
                    //internal stage tables must be pointing at your exe, and external ones must NOT
                    (StageTableLocation.DataLocationType == DataLocationTypes.Internal) == (StageTableLocation.Filename == EXEPath))
                    return preset;
                else
                    return StageTablePresets.custom;
            }
            set
            {
                if(value != StageTablePresets.custom && StageTablePreset != value)
                {
                    StageTableLocation.ResetToDefault(value);
                    switch (value)
                    {
                        case StageTablePresets.doukutsuexe:
                        case StageTablePresets.csmap:
                        case StageTablePresets.swdata:
                            StageTableLocation.Filename = EXEPath;
                            break;
                        case StageTablePresets.stagetbl:
                        case StageTablePresets.mrmapbin:
                            StageTableLocation.Filename = Path.Combine(BaseDataPath,
                                    value == StageTablePresets.stagetbl
                                    ? Stages.StageTable.STAGETBL
                                    : Stages.StageTable.MRMAPBIN
                                );
                            break;
                    }
                    StageTableSettings.ResetToDefault(value);
                }
            }
        }

        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public StageEntrySettings StageTableSettings { get; private set; }

        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public StageTableReferences InternalStageTableReferences { get; private set; } = new StageTableReferences();

        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public StageTableLocation StageTableLocation { get; set; }
        #endregion

        [Browsable(false)]
        public List<StageEntry> StageTable { get; private set; }

        [Browsable(false)]
        public List<NPCTableEntry> NPCTable { get; private set; }

        [Browsable(false)]
        public List<BulletTableEntry> BulletTable { get; private set; }

        #region path/folder stuff

        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog))]
        public string EXEPath { get; set; }
        
        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog))]
        public string BaseDataPath { get; set; }

        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public AssetManager FolderPaths { get; private set; }


        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog))]
        public string NpcTablePath { get; set; }

        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public BulletTableLocation BulletTableLocation { get; private set; }

        #endregion

        [LocalizeableCategory(nameof(Dialog.TilesCategory), typeof(Dialog))]
        public int TileSize { get; set; } = 16;

        #region images

        [LocalizeableCategory(nameof(Dialog.ImagesCategory), typeof(Dialog))]
        public Color TransparentColor { get; set; } = Color.Black;

        public event EventHandler CopyrightTextChanged;
        string copyrightText = Images.DefaultCopyrightString;
        /// <summary>
        /// What text should appear at the end of each image.
        /// NOT null terminated.
        /// </summary>
        [LocalizeableCategory(nameof(Dialog.ImagesCategory), typeof(Dialog))]
        public string CopyrightText
        {
            get => copyrightText;
            set
            {
                if (value != copyrightText)
                {
                    copyrightText = value;
                    CopyrightTextChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion

        #region filenames

        public event EventHandler TilesetPrefixChanged;
        string tilesetPrefix = Maps.Attribute.DefaultPrefix;
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue(Maps.Attribute.DefaultPrefix)]
        public string TilesetPrefix
        {
            get => tilesetPrefix;
            set
            {
                if(value != tilesetPrefix)
                {
                    tilesetPrefix = value;
                    TilesetPrefixChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue("Npc")]
        public string SpritesheetPrefix { get; set; } = "Npc";

        public event EventHandler StageExtensionChanged;
        string stageExtension = Map.DefaultExtension;
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue(Map.DefaultExtension)]
        public string StageExtension
        {
            get => stageExtension;
            set
            {
                if(value != stageExtension)
                {
                    stageExtension = value;
                    StageExtensionChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler EntityExtensionChanged;
        string entityExtension = PXE.DefaultExtension;
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue(PXE.DefaultExtension)]
        public string EntityExtension
        {
            get => entityExtension;
            set
            {
                if(value != entityExtension)
                {
                    entityExtension = value;
                    EntityExtensionChanged?.Invoke(this, new EventArgs());
                }
            }
        }
                
        public event EventHandler ImageExtensionChanged;
        string imageExtension = Images.DefaultImageExtension;
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue(Images.DefaultImageExtension)]
        public string ImageExtension
        {
            get => imageExtension;
            set
            {
                if(value != imageExtension)
                {
                    imageExtension = value;
                    ImageExtensionChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler TSCExtensionChanged;
        string tscExtension = "tsc";
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue("tsc")]
        public string TSCExtension
        {
            get => tscExtension;
            set
            {
                if(value != tscExtension)
                {
                    tscExtension = value;
                    TSCExtensionChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler AttributeExtensionChanged;
        string attributeExtension = Maps.Attribute.DefaultExtension; //TODO why is this hardcoded? there's a class for this
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue(Maps.Attribute.DefaultExtension)]
        public string AttributeExtension
        {
            get => attributeExtension;
            set
            {
                if(value != attributeExtension)
                {
                    attributeExtension = value;
                    AttributeExtensionChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion

        #region TSC
        [LocalizeableCategory(nameof(Dialog.ModTSCOptions), typeof(Dialog)), DefaultValue(false)]
        public bool UseScriptSource { get; set; } = false;
        [LocalizeableCategory(nameof(Dialog.ModTSCOptions),typeof(Dialog)), DefaultValue(true)]
        public bool TSCEncrypted { get; set; } = true;
        [LocalizeableCategory(nameof(Dialog.ModTSCOptions), typeof(Dialog)), DefaultValue(7)]
        public byte DefaultKey { get; set; } = 7;
        [LocalizeableCategory(nameof(Dialog.ModTSCOptions), typeof(Dialog)), TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding TSCEncoding { get; set; } = Encoding.ASCII;
                
        public List<Command> Commands { get; set; }

        #endregion

        #region gameplay

        [LocalizeableCategory(nameof(Dialog.GameplayCategory), typeof(Dialog))]
        public int ScreenWidth { get; set; } = 320;
        [LocalizeableCategory(nameof(Dialog.GameplayCategory), typeof(Dialog))]
        public int ScreenHeight { get; set; } = 240;

        #endregion

        #region user configurable enums

        [Browsable(false)]
        public SerializableDictionary<int,EntityInfo> EntityInfos { get; private set; } = new SerializableDictionary<int,EntityInfo>();

        [Browsable(false)]
        public SerializableDictionary<int,BulletInfo> BulletInfos { get; private set; } = new SerializableDictionary<int,BulletInfo>();

        [Browsable(false)]
        public SerializableDictionary<int,ISurfaceSource> SurfaceDescriptors { get; private set; } = new SerializableDictionary<int,ISurfaceSource>();

        [Browsable(false)]
        public SerializableDictionary<int,string> SoundEffects { get; private set; } = new SerializableDictionary<int,string>();

        [Browsable(false)]
        public SerializableDictionary<int,string> SmokeSizes { get; private set; } = new SerializableDictionary<int,string>();

        [Browsable(false)]
        public SerializableDictionary<long,string> BossNumbers { get; private set; } = new SerializableDictionary<long,string>();

        [Browsable(false)]
        public SerializableDictionary<long,string> BackgroundTypes { get; private set; } = new SerializableDictionary<long,string>();

        #endregion

        //TODO this should do something
        public Mod()
        {
            FolderPaths = new AssetManager(this);
        }

        /// <summary>
        /// Create a new mod from the given data folder, stage table location, and stage table type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="type"></param>
        public Mod(string data, string stage, StageTablePresets type)
        {
            if (!Directory.Exists(data))
                throw new DirectoryNotFoundException();
            if (!File.Exists(stage))
                throw new FileNotFoundException();

            BaseDataPath = data;

            FolderPaths = new AssetManager(this, "Stage", "Npc");

            StageTableLocation = new StageTableLocation(stage, type);
            StageTableSettings = new StageEntrySettings(type);
            StageTable = Stages.StageTable.Read(StageTableLocation, StageTableSettings);

            switch (StageTableLocation.DataLocationType)
            {
                case DataLocationTypes.Internal:
                    EXEPath = stage;
                    break;
                case DataLocationTypes.External:
                    //TODO this breaks if you have more than one exe in the directory
                    EXEPath = Directory.GetFiles(Path.GetDirectoryName(data), "*.exe")[0];
                    break;
            }

            switch(type)
            {
                case StageTablePresets.doukutsuexe:
                case StageTablePresets.swdata:
                    ImageExtension = Images.DefaultImageExtension;
                    break;
                case StageTablePresets.csmap:
                    UseScriptSource = true;
                    goto default;
                case StageTablePresets.stagetbl:
                    TileSize = 32;
                    goto default;
                default:
                    ImageExtension = "bmp";
                    break;
            }

            //TODO this still isn't good enough
            switch(StageTableLocation.DataLocationType)
            {
                case DataLocationTypes.Internal:
                    BulletTableLocation = new BulletTableLocation(EXEPath, BulletTablePresets.doukutsuexe);
                    break;
                case DataLocationTypes.External:
                    BulletTableLocation = new BulletTableLocation(Path.Combine(BaseDataPath, CaveStoryModdingFramework.BulletTable.BULLETTABLE), BulletTablePresets.csplus);
                    break;
            }
            
            BulletTable = CaveStoryModdingFramework.BulletTable.Read(BulletTableLocation);

            NpcTablePath = Path.Combine(BaseDataPath, Entities.NPCTable.NPCTBL);
            if(File.Exists(NpcTablePath))
                NPCTable = Entities.NPCTable.Load(NpcTablePath);

            //TODO test if this is really needed...
            foreach (var surface in Surfaces.SurfaceList)
                SurfaceDescriptors.Add(surface.Key, (ISurfaceSource)surface.Value.Clone());

            //TODO related to the one above
            SoundEffects = new SerializableDictionary<int,string>(CaveStoryModdingFramework.SoundEffects.SoundEffectList);
            SmokeSizes = new SerializableDictionary<int,string>(CaveStoryModdingFramework.SmokeSizes.SmokeSizeList);
            BossNumbers = new SerializableDictionary<long,string>(CaveStoryModdingFramework.Bosses.BossNameList);
            BackgroundTypes = new SerializableDictionary<long,string>(CaveStoryModdingFramework.BackgroundTypes.BackgroundTypeList);

            EntityInfos = new SerializableDictionary<int,EntityInfo>(EntityList.EntityInfos);
            BulletInfos = new SerializableDictionary<int,BulletInfo>(BulletList.BulletInfos);

            Commands = new List<Command>(CommandList.BaseCommands);
        }

        //This is slightly hacky, but I think it's the only way to go...
        private string xmlWorkingPath = null;

        /// <summary>
        /// Loads a mod from a given file
        /// </summary>
        /// <param name="path"></param>
        public Mod(string path) : this()
        {
            xmlWorkingPath = path;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var x = XmlReader.Create(fs, new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true }))
                ReadXml(x);
            xmlWorkingPath = null;

            StageTable = Stages.StageTable.Read(StageTableLocation, StageTableSettings);

            BulletTable = CaveStoryModdingFramework.BulletTable.Read(BulletTableLocation);

            if (File.Exists(NpcTablePath))
                NPCTable = Entities.NPCTable.Load(NpcTablePath);
        }

        /// <summary>
        /// Saves this mod to a given file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            xmlWorkingPath = path;
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var x = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true }))
                    WriteXml(x);
            xmlWorkingPath = null;
        }

        #region xml serialization

        const string XmlBase = "CaveStoryMod";
        const string XmlPaths = "Paths";
        const string XmlStageTable = "StageTable";
        const string XmlNPCTable = "NPCTable";
        const string XmlBulletTable = "BulletTable";
        const string XmlImages = "Images";
        const string XmlFilenames = "Filenames";
        const string XmlTSC = "TSC";
        const string XmlGameplay = "Gameplay";

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(XmlBase);
            {
                reader.ReadStartElement(XmlPaths);
                {
                    BaseDataPath = reader.ReadElementContentAsString(nameof(BaseDataPath),"");
                    if (xmlWorkingPath != null)
                        BaseDataPath = AssetManager.MakeAbsolute(xmlWorkingPath, BaseDataPath);

                    EXEPath = AssetManager.MakeAbsolute(BaseDataPath, reader.ReadElementContentAsString(nameof(EXEPath), ""));

                    FolderPaths.DataPaths = reader.DeserializeAs(FolderPaths.DataPaths, nameof(FolderPaths.DataPaths));
                    FolderPaths.StagePaths = reader.DeserializeAs(FolderPaths.StagePaths, nameof(FolderPaths.StagePaths));
                    FolderPaths.NpcPaths = reader.DeserializeAs(FolderPaths.NpcPaths, nameof(FolderPaths.NpcPaths));
                }
                reader.ReadEndElement();

                reader.ReadStartElement(XmlStageTable);
                {
                    StageTableLocation = reader.DeserializeAs(StageTableLocation, nameof(StageTableLocation));
                    StageTableLocation.Filename = AssetManager.MakeAbsolute(BaseDataPath, StageTableLocation.Filename);

                    StageTableSettings = reader.DeserializeAs(StageTableSettings, nameof(StageTableSettings));

                    InternalStageTableReferences = reader.DeserializeAs(InternalStageTableReferences, nameof(InternalStageTableReferences));
                }
                reader.ReadEndElement();

                reader.ReadStartElement(XmlNPCTable);
                {
                    NpcTablePath = AssetManager.MakeAbsolute(BaseDataPath, reader.ReadElementContentAsString(nameof(NpcTablePath),""));
                }
                reader.ReadEndElement();

                reader.ReadStartElement(XmlBulletTable);
                {
                    BulletTableLocation = reader.DeserializeAs(BulletTableLocation, nameof(BulletTableLocation));
                    BulletTableLocation.Filename = AssetManager.MakeAbsolute(BaseDataPath, BulletTableLocation.Filename);
                }
                reader.ReadEndElement();

                TileSize = reader.ReadElementContentAsInt(nameof(TileSize), "");

                reader.ReadStartElement(XmlImages);
                {
                    TransparentColor = ColorTranslator.FromHtml(reader.ReadElementContentAsString(nameof(TransparentColor), ""));

                    CopyrightText = reader.ReadElementContentAsString(nameof(CopyrightText), "");
                }
                reader.ReadEndElement();

                reader.ReadStartElement(XmlFilenames);
                {
                    TilesetPrefix = reader.ReadElementContentAsString(nameof(TilesetPrefix),"");
                    SpritesheetPrefix = reader.ReadElementContentAsString(nameof(SpritesheetPrefix), "");
                    StageExtension = reader.ReadElementContentAsString(nameof(StageExtension), "");
                    EntityExtension = reader.ReadElementContentAsString(nameof(EntityExtension), "");
                    ImageExtension = reader.ReadElementContentAsString(nameof(ImageExtension), "");
                    TSCExtension = reader.ReadElementContentAsString(nameof(TSCExtension), "");
                    AttributeExtension = reader.ReadElementContentAsString(nameof(AttributeExtension), "");
                }
                reader.ReadEndElement();

                reader.ReadStartElement(XmlTSC);
                {
                    UseScriptSource = bool.Parse(reader.ReadElementContentAsString(nameof(UseScriptSource), ""));
                    TSCEncrypted = bool.Parse(reader.ReadElementContentAsString(nameof(TSCEncrypted), ""));
                    DefaultKey = (byte)reader.ReadElementContentAsInt(nameof(DefaultKey), "");
                    TSCEncoding = Encoding.GetEncoding(reader.ReadElementContentAsString(nameof(TSCEncoding), ""));
                }
                reader.ReadEndElement();

                reader.ReadStartElement(XmlGameplay);
                {
                    ScreenWidth = reader.ReadElementContentAsInt(nameof(ScreenWidth), "");
                    ScreenHeight = reader.ReadElementContentAsInt(nameof(ScreenHeight), "");
                }
                reader.ReadEndElement();

                EntityInfos = reader.DeserializeAs(EntityInfos, nameof(EntityInfos));

                BulletInfos = reader.DeserializeAs(BulletInfos, nameof(BulletInfos));

                SurfaceDescriptors = reader.DeserializeAs(SurfaceDescriptors, nameof(SurfaceDescriptors));

                SoundEffects = reader.DeserializeAs(SoundEffects, nameof(SoundEffects));
                SmokeSizes = reader.DeserializeAs(SmokeSizes, nameof(SmokeSizes));
                BossNumbers = reader.DeserializeAs(BossNumbers, nameof(BossNumbers));
                BackgroundTypes = reader.DeserializeAs(BackgroundTypes, nameof(BackgroundTypes));

                Commands = reader.DeserializeAs(Commands, "TSCCommands");
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            void SerializeRelativeDataLocation<T>(T dl) where T : DataLocation
            {
                var absolute = dl.Filename;
                dl.Filename = AssetManager.MakeRelative(BaseDataPath, absolute);
                var dataLocSerializer = new XmlSerializer(typeof(T));
                dataLocSerializer.Serialize(writer, dl, Extensions.BlankNamespace);
                dl.Filename = absolute;
            }

            writer.WriteStartElement(XmlBase);
            {

                writer.WriteStartElement(XmlPaths);
                {
                    string path = BaseDataPath;
                    if (xmlWorkingPath != null)
                        path = AssetManager.MakeRelative(xmlWorkingPath, BaseDataPath);
                    writer.WriteElementString(nameof(BaseDataPath), path);

                    var relativeEXEPath = AssetManager.MakeRelative(BaseDataPath, EXEPath);
                    writer.WriteElementString(nameof(EXEPath), relativeEXEPath);

                    //TODO these should say Item
                    writer.SerializeAsRoot(FolderPaths.DataPaths, nameof(FolderPaths.DataPaths));
                    writer.SerializeAsRoot(FolderPaths.StagePaths, nameof(FolderPaths.StagePaths));
                    writer.SerializeAsRoot(FolderPaths.NpcPaths, nameof(FolderPaths.NpcPaths));
                }
                writer.WriteEndElement();

                writer.WriteStartElement(XmlStageTable);
                {
                    SerializeRelativeDataLocation(StageTableLocation);

                    writer.SerializeAsRoot(StageTableSettings, nameof(StageTableSettings));

                    writer.SerializeAsRoot(InternalStageTableReferences, nameof(InternalStageTableReferences));
                }
                writer.WriteEndElement();

                writer.WriteStartElement(XmlNPCTable);
                {
                    var relativeNpcTablePath = AssetManager.MakeRelative(BaseDataPath, NpcTablePath);
                    writer.WriteElementString(nameof(NpcTablePath), relativeNpcTablePath);
                }
                writer.WriteEndElement();

                writer.WriteStartElement(XmlBulletTable);
                {
                    SerializeRelativeDataLocation(BulletTableLocation);
                }
                writer.WriteEndElement();

                writer.WriteElementString(nameof(TileSize), TileSize.ToString());

                writer.WriteStartElement(XmlImages);
                {
                    writer.WriteElementString(nameof(TransparentColor), ColorTranslator.ToHtml(TransparentColor));

                    writer.WriteElementString(nameof(CopyrightText), copyrightText);
                }
                writer.WriteEndElement();

                writer.WriteStartElement(XmlFilenames);
                {
                    writer.WriteElementString(nameof(TilesetPrefix), TilesetPrefix);
                    writer.WriteElementString(nameof(SpritesheetPrefix), SpritesheetPrefix);
                    writer.WriteElementString(nameof(StageExtension), StageExtension);
                    writer.WriteElementString(nameof(EntityExtension), EntityExtension);
                    writer.WriteElementString(nameof(ImageExtension), ImageExtension);
                    writer.WriteElementString(nameof(TSCExtension), TSCExtension);
                    writer.WriteElementString(nameof(AttributeExtension), AttributeExtension);
                }
                writer.WriteEndElement();

                writer.WriteStartElement(XmlTSC);
                {
                    writer.WriteElementString(nameof(UseScriptSource), UseScriptSource.ToString());
                    writer.WriteElementString(nameof(TSCEncrypted), TSCEncrypted.ToString());
                    writer.WriteElementString(nameof(DefaultKey), DefaultKey.ToString());
                    writer.WriteElementString(nameof(TSCEncoding), TSCEncoding.WebName);
                }
                writer.WriteEndElement();

                writer.WriteStartElement(XmlGameplay);
                {
                    writer.WriteElementString(nameof(ScreenWidth), ScreenWidth.ToString());
                    writer.WriteElementString(nameof(ScreenHeight), ScreenHeight.ToString());
                }
                writer.WriteEndElement();

                writer.SerializeAsRoot(EntityInfos, nameof(EntityInfos));

                writer.SerializeAsRoot(BulletInfos, nameof(BulletInfos));

                writer.SerializeAsRoot(SurfaceDescriptors, nameof(SurfaceDescriptors));

                writer.SerializeAsRoot(SoundEffects, nameof(SoundEffects));
                writer.SerializeAsRoot(SmokeSizes, nameof(SmokeSizes));
                writer.SerializeAsRoot(BossNumbers, nameof(BossNumbers));
                writer.SerializeAsRoot(BackgroundTypes, nameof(BackgroundTypes));

                writer.SerializeAsRoot(Commands, "TSCCommands");
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
