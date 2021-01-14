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
using System.Xml.Linq;
using System.Reflection;
using System.Xml;

namespace CaveStoryModdingFramework
{
    public class Mod
    {
        public const string CaveStoryProjectFilter = "Cave Story Mod Project (*.cav)|*.cav";
        #region stage table
        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog))]
        public StageTableTypes StageTableFormat { get; set; }
        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog)), TypeConverter(typeof(ExpandableObjectConverter))]
        public StageEntrySettings StageTableSettings { get; }
        [LocalizeableCategory(nameof(Dialog.ModStageTableCategory), typeof(Dialog))]
        public string StageTableLocation { get; set; }
        #endregion

        [Browsable(false)]
        public List<StageEntry> StageTable { get; private set; }

        [Browsable(false)]
        public List<NPCTableEntry> NPCTable { get; private set; }
                
        #region path/folder stuff
        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog))]
        public string DataFolderPath { get; private set; }
        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog)), DefaultValue("Stage")]
        public string StageFolderPath { get; private set; } = "Stage"; //HACK hardcoded
        [LocalizeableCategory(nameof(Dialog.FoldersCategory), typeof(Dialog)), DefaultValue("Npc")]
        public string NpcFolderPath { get; private set; } = "Npc"; //HACK hardcoded
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
        string attributeExtension = "pxa"; //TODO why is this hardcoded? there's a class for this
        [LocalizeableCategory(nameof(Dialog.ModFilenameCategory), typeof(Dialog)), DefaultValue("pxa")]
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

        [LocalizeableCategory(nameof(Dialog.ModTSCOptions),typeof(Dialog)), DefaultValue(true)]
        public bool TSCEncrypted { get; set; } = true;
        [LocalizeableCategory(nameof(Dialog.ModTSCOptions), typeof(Dialog)), DefaultValue(7)]
        public byte DefaultKey { get; set; } = 7;
        [LocalizeableCategory(nameof(Dialog.ModTSCOptions), typeof(Dialog)), TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding TSCEncoding { get; set; } = Encoding.ASCII;

        #endregion

        #region gameplay

        [LocalizeableCategory(nameof(Dialog.GameplayCategory), typeof(Dialog))]
        public int ScreenWidth { get; set; } = 320;
        [LocalizeableCategory(nameof(Dialog.GameplayCategory), typeof(Dialog))]
        public int ScreenHeight { get; set; } = 240;

        #endregion

        #region user configurable enums

        [Browsable(false)]
        public Dictionary<int, EntityInfo> EntityInfos { get; private set; } = new Dictionary<int, EntityInfo>();

        [Browsable(false)]
        public Dictionary<int, ISurfaceSource> SurfaceDescriptors { get; private set; } = new Dictionary<int, ISurfaceSource>();

        [Browsable(false)]
        public Dictionary<int, string> SoundEffects { get; private set; } = new Dictionary<int, string>();

        [Browsable(false)]
        public Dictionary<int, string> SmokeSizes { get; private set; } = new Dictionary<int, string>();

        [Browsable(false)]
        public Dictionary<int, string> BossNumbers { get; private set; } = new Dictionary<int, string>();

        [Browsable(false)]
        public Dictionary<int, string> BackgroundTypes { get; private set; } = new Dictionary<int, string>();

        #endregion


        /// <summary>
        /// Create a new mod from the given data folder, stage table location, and stage table type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="type"></param>
        public Mod(string data, string stage, StageTableTypes type)
        {
            if (!Directory.Exists(data))
                throw new DirectoryNotFoundException();
            if (!File.Exists(stage))
                throw new FileNotFoundException();

            DataFolderPath = data;

            StageTableLocation = stage;
            StageTableFormat = type;
            StageTableSettings = new StageEntrySettings(type);
            StageTable = Stages.StageTable.Load(stage, type);

            switch(type)
            {
                case StageTableTypes.stagetbl:
                    TileSize = 32;
                    goto default;
                default:
                    ImageExtension = "bmp";
                    break;
            }

            NPCTable = Entities.NPCTable.Load(Path.Combine(DataFolderPath, Entities.NPCTable.NPCTBL));

            //TODO test if this is really needed...
            foreach (var surface in Surfaces.SurfaceList)
                SurfaceDescriptors.Add(surface.Key, (ISurfaceSource)surface.Value.Clone());

            //TODO related to the one above
            SoundEffects = new Dictionary<int, string>(CaveStoryModdingFramework.SoundEffects.SoundEffectList);
            SmokeSizes = new Dictionary<int, string>(CaveStoryModdingFramework.SmokeSizes.SmokeSizeList);

            EntityInfos = new Dictionary<int, EntityInfo>(EntityList.EntityInfos);
        }

        static Rectangle RectFromString(string input)
        {
            var nums = input.Split(',');
            return new Rectangle(int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]), int.Parse(nums[3]));
        }
        static string RectToString(Rectangle rect)
        {
            return $"{rect.X},{rect.Y},{rect.Width},{rect.Height}";
        }

        /// <summary>
        /// Saves this mod to a given file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            var relativeDataPath = new Uri(path).MakeRelativeUri(new Uri(DataFolderPath));
            var relativeStageTablePath =  new Uri(path).MakeRelativeUri(new Uri(StageTableLocation));

            XElement SerializeEntities(IDictionary<int, EntityInfo> dict, string name, string valName, string keyName = "Key")
            {
                var baseAssembly = Assembly.GetAssembly(typeof(Mod));
                var @base = new XElement(name);
                foreach(var item in dict)
                {
                    var i = new XElement(valName,
                        new XAttribute(keyName, item.Key),
                        new XAttribute("Name", item.Value.Name),
                        new XAttribute("Rect", RectToString(item.Value.SpriteLocation))
                        );
                    if (!string.IsNullOrWhiteSpace(item.Value.Category))
                        i.Add(new XAttribute("Category", item.Value.Category));
                    if (item.Value.CustomType != null)
                    {
                        i.Add(new XAttribute("Type", item.Value.CustomType.FullName));
                        var a = Assembly.GetAssembly(item.Value.CustomType);
                        if (a != baseAssembly)
                            i.Add(new XAttribute("dll", a.GetName()));
                    }
                    @base.Add(i);                        
                }
                return @base;
            }
            XElement SerializeSurfaces(IDictionary<int, ISurfaceSource> dict, string name, string valName, string keyName = "Key")
            {
                var @base = new XElement(name);
                foreach(var item in dict)
                {
                    var i = new XElement(valName, new XAttribute(keyName, item.Key), item.Value.DisplayName);
                    if(item.Value is SurfaceSourceFile ssf)
                    {
                        i.Add(new XAttribute("Type", "File"),
                              new XAttribute("Prefix", ssf.Prefix),
                              new XAttribute("Folder", ssf.Folder),
                              new XAttribute("Filename", ssf.Filename)
                              );
                    }
                    else if(item.Value is SurfaceSourceIndex ssi)
                    {
                        i.Add(new XAttribute("Type", "Index"),
                              new XAttribute("Index", ssi.Index)
                              );
                    }
                    else if(item.Value is SurfaceSourceRuntime ssr)
                    {
                        i.Add(new XAttribute("Type", "Runtime"));
                    }
                    @base.Add(i);
                }
                return @base;
            }
            XElement SerializeDict(IDictionary<int, string> dict, string name, string valName, string keyName = "Key")
            {
                var @base = new XElement(name);
                foreach (var item in dict)
                {
                    var i = new XElement(valName, new XAttribute(keyName, item.Key), item.Value);
                    @base.Add(i);
                }
                return @base;
            }           

            new XDocument(
                new XElement("CaveStoryMod",
                    new XElement("Paths", 
                        new XElement("DataPath", Uri.UnescapeDataString(relativeDataPath.ToString())),
                        new XElement("StageFolder", StageFolderPath),
                        new XElement("NpcFolder", NpcFolderPath)
                    ),
                    new XElement("StageTable",
                        new XElement("Location", Uri.UnescapeDataString(relativeStageTablePath.ToString())),
                        new XElement("Type", StageTableFormat),
                        StageTableSettings.ToXML("StageTableSettings")
                    ),
                    new XElement("TileSize", TileSize),
                    new XElement("Images",
                        new XElement("TransparentColor", ColorTranslator.ToHtml(TransparentColor)),
                        new XElement("CopyrightText", copyrightText)
                    ),
                    new XElement("Filenames",
                        new XElement("TilesetPrefix", TilesetPrefix),
                        new XElement("SpritesheetPrefix", SpritesheetPrefix),
                        new XElement("StageExtension", StageExtension),
                        new XElement("EntityExtension", EntityExtension),
                        new XElement("ImageExtension", ImageExtension),
                        new XElement("TSCExtension", TSCExtension),
                        new XElement("AttributeExtension", AttributeExtension)
                    ),
                    new XElement("TSC",
                        new XElement("TSCEncrypted", TSCEncrypted),
                        new XElement("DefaultKey", DefaultKey),
                        new XElement("TSCEncoding", TSCEncoding.WebName)
                    ),
                    new XElement("Gameplay",
                        new XElement("ScreenWidth", ScreenWidth),
                        new XElement("ScreenHeight", ScreenHeight)
                    ),
                    SerializeEntities(EntityInfos, "EntityInfoList", "EntityInfo"),
                    SerializeSurfaces(SurfaceDescriptors, "SurfaceList", "SurfaceDescriptor"),
                    SerializeDict(SoundEffects, "SoundEffects", "SoundEffect"),
                    SerializeDict(SmokeSizes, "SmokeSizes", "SmokeSize"),
                    SerializeDict(BossNumbers, "BossNumbers", "BossNumber"),
                    SerializeDict(BackgroundTypes, "BackgroundTypes", "BackgroundType")
                )
            ).Save(path);
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

            var dataPath = root["Paths"]?["DataPath"]?.InnerText;
            if (dataPath == null)
                throw new ArgumentNullException();
            dataPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), dataPath));
            if (!Directory.Exists(dataPath))
                throw new DirectoryNotFoundException();
            DataFolderPath = dataPath;

            void LoadEntities(XmlElement element, IDictionary<int, EntityInfo> dict)
            {
                foreach(XmlElement item in element.ChildNodes)
                {
                    var key = int.Parse(item.Attributes["Key"].InnerText);
                    var name = item.Attributes["Name"].InnerText;
                    var rect = RectFromString(item.Attributes["Rect"].InnerText);
                    var categories = item.Attributes["Category"]?.InnerText;
                    var typeName = item.Attributes["Type"]?.InnerText;

                    if (typeName != null)
                    {
                        dict.Add(key, new EntityInfo(name, Type.GetType(typeName), rect, categories ?? ""));
                    }
                    else
                    {
                        dict.Add(key, new EntityInfo(name, rect, categories ?? ""));
                    }
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
                                (Folders)Enum.Parse(typeof(Folders), item.Attributes["Folder"].InnerText),
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


            var paths = root["Paths"];
            StageFolderPath = paths["StageFolder"].InnerText;
            NpcFolderPath = paths["NpcFolder"].InnerText;

            var st = root["StageTable"];
            if(st != null)
            {
                StageTableLocation = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), st["Location"].InnerText));
                StageTableFormat =  (StageTableTypes)Enum.Parse(typeof(StageTableTypes), st["Type"].InnerText);
                StageTableSettings = new StageEntrySettings(st["StageTableSettings"]);
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

            StageTable = Stages.StageTable.Load(StageTableLocation, StageTableFormat);
            NPCTable = Entities.NPCTable.Load(Path.Combine(DataFolderPath, Entities.NPCTable.NPCTBL));
        }
    }
}
