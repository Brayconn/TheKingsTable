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
        public StageEntrySettings StageTableSettings { get; }

        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public StageTableReferences InternalStageTableReferences { get; } = new StageTableReferences();

        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public StageTableLocation StageTableLocation { get; set; }
        #endregion

        [Browsable(false), XmlIgnore]
        public List<StageEntry> StageTable { get; private set; }

        [Browsable(false), XmlIgnore]
        public List<NPCTableEntry> NPCTable { get; private set; }

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
        public SerializableDictionary<EntityInfo> EntityInfos { get; private set; } = new SerializableDictionary<EntityInfo>();

        [Browsable(false)]
        public SerializableDictionary<BulletInfo> BulletInfos { get; private set; } = new SerializableDictionary<BulletInfo>();

        [Browsable(false)]
        public SerializableDictionary<ISurfaceSource> SurfaceDescriptors { get; private set; } = new SerializableDictionary<ISurfaceSource>();

        [Browsable(false)]
        public SerializableDictionary<string> SoundEffects { get; private set; } = new SerializableDictionary<string>();

        [Browsable(false)]
        public SerializableDictionary<string> SmokeSizes { get; private set; } = new SerializableDictionary<string>();

        [Browsable(false)]
        public SerializableDictionary<string> BossNumbers { get; private set; } = new SerializableDictionary<string>();

        [Browsable(false)]
        public SerializableDictionary<string> BackgroundTypes { get; private set; } = new SerializableDictionary<string>();

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

            //TODO actually support choosing a bullet table type
            BulletTableLocation = new BulletTableLocation(EXEPath, BulletTablePresets.doukutsuexe);

            NpcTablePath = Path.Combine(BaseDataPath, Entities.NPCTable.NPCTBL);
            if(File.Exists(NpcTablePath))
                NPCTable = Entities.NPCTable.Load(NpcTablePath);

            //TODO test if this is really needed...
            foreach (var surface in Surfaces.SurfaceList)
                SurfaceDescriptors.Add(surface.Key, (ISurfaceSource)surface.Value.Clone());

            //TODO related to the one above
            SoundEffects = new SerializableDictionary<string>(CaveStoryModdingFramework.SoundEffects.SoundEffectList);
            SmokeSizes = new SerializableDictionary<string>(CaveStoryModdingFramework.SmokeSizes.SmokeSizeList);
            BossNumbers = new SerializableDictionary<string>(CaveStoryModdingFramework.Bosses.BossNameList);
            BackgroundTypes = new SerializableDictionary<string>(CaveStoryModdingFramework.BackgroundTypes.BackgroundTypeList);

            EntityInfos = new SerializableDictionary<EntityInfo>(EntityList.EntityInfos);
            BulletInfos = new SerializableDictionary<BulletInfo>(BulletList.BulletInfos);

            Commands = new List<Command>(CommandList.BaseCommands);
        }

        //This is slightly hacky, but I think it's the only way to go...
        private string relativeDataPath = null;

        /// <summary>
        /// Saves this mod to a given file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(Mod));
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };
            relativeDataPath = AssetManager.MakeRelative(path, BaseDataPath);
            using (var x = XmlWriter.Create(new FileStream(path, FileMode.Create, FileAccess.Write), settings))
                serializer.Serialize(x, this);
            relativeDataPath = null;
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a mod from a given file
        /// </summary>
        /// <param name="path"></param>
        public Mod(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);
            var root = doc["CaveStoryMod"];
            if (root == null)
                throw new FileLoadException();

            var dataPath = root["Paths"]?["BaseDataPath"]?.InnerText;
            if (dataPath == null)
                throw new ArgumentNullException();
            dataPath = AssetManager.MakeAbsolute(path, dataPath);
            if (!Directory.Exists(dataPath))
                throw new DirectoryNotFoundException();
            BaseDataPath = dataPath;

            void LoadEntities(XmlElement element, IDictionary<int, EntityInfo> dict)
            {
                foreach(XmlElement item in element.ChildNodes)
                {
                    var key = int.Parse(item.Attributes["Key"].InnerText);
                    var name = item.Attributes["Name"].InnerText;
                    var rect = RectExtensions.RectFromString(item.Attributes["Rect"].InnerText);
                    var categories = item.Attributes["Category"]?.InnerText;

                    dict.Add(key, new EntityInfo(name, rect, categories ?? ""));
                }
            }
            void LoadSurfaces(XmlElement element, IDictionary<int, ISurfaceSource> dict)
            {
                foreach (XmlElement item in element.ChildNodes)
                {
                    var key = int.Parse(item.Attributes["Key"].InnerText);
                    switch (item.Attributes["Type"].InnerText)
                    {
                        case "File":
                            dict.Add(key, new SurfaceSourceFile(
                                (SearchLocations)Enum.Parse(typeof(SearchLocations), item.Attributes["Folder"].InnerText),
                                (Prefixes)Enum.Parse(typeof(Prefixes), item.Attributes["Prefix"].InnerText),
                                item.Attributes["Filename"].InnerText,
                                item.InnerText));
                            break;
                        case "Index":
                            dict.Add(key, new SurfaceSourceIndex(int.Parse(item.Attributes["Index"].InnerText), item.InnerText));
                            break;
                        case "Runtime":
                            dict.Add(key, new SurfaceSourceRuntime(item.InnerText));
                            break;
                    }
                }
            }
            void LoadDict(XmlElement element, IDictionary<int, string> dict)
            {
                foreach(XmlElement item in element.ChildNodes)
                {
                    dict.Add(int.Parse(item.GetAttribute("Key")), item.InnerText);
                }
            }
            void LoadLongDict(XmlElement element, IDictionary<long, string> dict)
            {
                foreach (XmlElement item in element.ChildNodes)
                {
                    dict.Add(long.Parse(item.GetAttribute("Key")), item.InnerText);
                }
            }
            void LoadList(XmlElement element, List<string> list)
            {
                foreach(XmlElement item in element.ChildNodes)
                {
                    list.Add(item.InnerText);
                }
            }
            //HACK omg this is probably the worst code in TKT, I really need to redo mod saving/loading
            void LoadCommands(string elementName, List<Command> list)
            {
                List<Command> temp;
                using (var garbage = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    garbage.ReadToDescendant(elementName);
                    var xs = new XmlSerializer(typeof(List<Command>), new XmlRootAttribute(elementName));
                    temp = (List<Command>)xs.Deserialize(garbage);
                }                
                foreach (var command in temp)
                    list.Add(command);
            }

            FolderPaths = new AssetManager(this);
            var paths = root["Paths"];
            LoadList(paths["DataFolders"], FolderPaths.DataPaths);
            LoadList(paths["StageFolders"], FolderPaths.StagePaths);
            LoadList(paths["NpcFolders"], FolderPaths.NpcPaths);
            //TODO failsafe might not be safe
            if (paths["EXEPath"] != null)
                EXEPath = AssetManager.MakeAbsolute(BaseDataPath, paths["EXEPath"].InnerText);
            NpcTablePath = AssetManager.MakeAbsolute(BaseDataPath, paths["NpcTablePath"]?.InnerText ?? Entities.NPCTable.NPCTBL);


            var st = root["StageTable"];
            if(st != null)
            {
                StageTableLocation = new StageTableLocation(st["Location"], AssetManager.MakeAbsolute(BaseDataPath, st["Location"][nameof(Stages.StageTableLocation.Filename)].InnerText));
                //StageTableSettings = new StageEntrySettings(st["StageTableSettings"]);
                //TODO save references
            }

            TileSize = int.Parse(root["TileSize"].InnerText);

            var img = root["Images"];
            if (img != null)
            {
                TransparentColor = ColorTranslator.FromHtml(img["TransparentColor"].InnerText);
                CopyrightText = img["CopyrightText"].InnerText;
            }

            var fn = root["Filenames"];
            if(fn != null)
            {
                TilesetPrefix = fn["TilesetPrefix"].InnerText;
                SpritesheetPrefix = fn["SpritesheetPrefix"].InnerText;
                StageExtension = fn["StageExtension"].InnerText;
                EntityExtension = fn["EntityExtension"].InnerText;
                ImageExtension = fn["ImageExtension"].InnerText;
                TSCExtension = fn["TSCExtension"].InnerText;
                AttributeExtension = fn["AttributeExtension"].InnerText;
            }

            var tsc = root["TSC"];
            if(tsc != null)
            {
                UseScriptSource = bool.Parse(tsc["UseScriptSource"].InnerText);
                TSCEncrypted = bool.Parse(tsc["TSCEncrypted"].InnerText);
                DefaultKey = byte.Parse(tsc["DefaultKey"].InnerText);
                TSCEncoding = Encoding.GetEncoding(tsc["TSCEncoding"].InnerText);
            }

            var game = root["Gameplay"];
            if(game != null)
            {
                ScreenWidth = int.Parse(game["ScreenWidth"].InnerText);
                ScreenHeight = int.Parse(game["ScreenHeight"].InnerText);
            }
            LoadEntities(root["EntityInfoList"], EntityInfos);
            
            LoadSurfaces(root["SurfaceList"], SurfaceDescriptors);
            LoadDict(root["SoundEffects"], SoundEffects);
            LoadDict(root["SmokeSizes"], SmokeSizes);
            LoadDict(root["BossNumbers"], BossNumbers);
            LoadDict(root["BackgroundTypes"], BackgroundTypes);
            if (root["TSCCommands"] != null)
            {
                Commands = new List<Command>(root["TSCCommands"].ChildNodes.Count);
                LoadCommands("TSCCommands", Commands);
            }

            StageTable = Stages.StageTable.Read(StageTableLocation, StageTableSettings);
            //TODO check npc table loading from project file
            if(File.Exists(NpcTablePath))
                NPCTable = Entities.NPCTable.Load(NpcTablePath);
        }

        const string XmlBase = "CaveStoryMod";
        const string XmlPaths = "Paths";
        const string XmlStageTable = "StageTable";
        const string XmlNPCTable = "NPCTable";
        const string XmlBulletTable = "BulletTable";
        const string XmlImages = "Images";
        const string XmlFilenames = "Filenames";
        const string XmlTSC = "TSC";
        const string XmlGameplay = "Gameplay";
        public void WriteXml(XmlWriter writer)
        {
            var blankNamespace = new XmlSerializerNamespaces();
            blankNamespace.Add("", "");

            var relativeEXEPath = AssetManager.MakeRelative(BaseDataPath, EXEPath);
            var relativeNpcTablePath = AssetManager.MakeRelative(BaseDataPath, NpcTablePath);

            void SerializeRelativeDataLocation<T>(T dl) where T : DataLocation
            {
                var absolute = dl.Filename;
                dl.Filename = AssetManager.MakeRelative(BaseDataPath, absolute);
                var dataLocSerializer = new XmlSerializer(typeof(T));
                dataLocSerializer.Serialize(writer, dl, blankNamespace);
                dl.Filename = absolute;
            }

            writer.WriteStartElement(XmlPaths);
            {
                writer.WriteElementString(nameof(BaseDataPath), relativeDataPath ?? BaseDataPath);
                writer.WriteElementString(nameof(EXEPath), relativeEXEPath);

                //TODO these should say Item
                var listSerializer = new XmlSerializer(typeof(List<string>), new XmlRootAttribute(nameof(FolderPaths.DataPaths)));
                listSerializer.Serialize(writer, FolderPaths.DataPaths, blankNamespace);

                listSerializer = new XmlSerializer(typeof(List<string>), new XmlRootAttribute(nameof(FolderPaths.StagePaths)));
                listSerializer.Serialize(writer, FolderPaths.StagePaths, blankNamespace);

                listSerializer = new XmlSerializer(typeof(List<string>), new XmlRootAttribute(nameof(FolderPaths.NpcPaths)));
                listSerializer.Serialize(writer, FolderPaths.NpcPaths, blankNamespace);
            }
            writer.WriteEndElement();

            writer.WriteStartElement(XmlStageTable);
            {
                SerializeRelativeDataLocation(StageTableLocation);

                var settingsSerializer = new XmlSerializer(typeof(StageEntrySettings));
                settingsSerializer.Serialize(writer, StageTableSettings);

                var referencesSerializer = new XmlSerializer(typeof(StageTableReferences));
                referencesSerializer.Serialize(writer, InternalStageTableReferences, blankNamespace);
            }
            writer.WriteEndElement();

            writer.WriteStartElement(XmlNPCTable);
            {
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

            var entitySerializer = new XmlSerializer(typeof(SerializableDictionary<EntityInfo>), new XmlRootAttribute(nameof(EntityInfos)));
            entitySerializer.Serialize(writer, EntityInfos);

            var bulletSerializer = new XmlSerializer(typeof(SerializableDictionary<BulletInfo>), new XmlRootAttribute(nameof(BulletInfos)));
            bulletSerializer.Serialize(writer, BulletInfos);

            var surfaceSerializer = new XmlSerializer(typeof(SerializableDictionary<ISurfaceSource>), new XmlRootAttribute(nameof(SurfaceDescriptors)));
            surfaceSerializer.Serialize(writer, SurfaceDescriptors);

            var stringSerialzier = new XmlSerializer(typeof(SerializableDictionary<string>), new XmlRootAttribute(nameof(SoundEffects)));
            stringSerialzier.Serialize(writer, SoundEffects);

            stringSerialzier = new XmlSerializer(typeof(SerializableDictionary<string>), new XmlRootAttribute(nameof(SmokeSizes)));
            stringSerialzier.Serialize(writer, SmokeSizes);

            stringSerialzier = new XmlSerializer(typeof(SerializableDictionary<string>), new XmlRootAttribute(nameof(BossNumbers)));
            stringSerialzier.Serialize(writer, BossNumbers);

            stringSerialzier = new XmlSerializer(typeof(SerializableDictionary<string>), new XmlRootAttribute(nameof(BackgroundTypes)));
            stringSerialzier.Serialize(writer, BackgroundTypes);

            var commandSerializer = new XmlSerializer(typeof(List<Command>), new XmlRootAttribute("TSCCommands"));
            commandSerializer.Serialize(writer, Commands, blankNamespace);

        }
    }
}
