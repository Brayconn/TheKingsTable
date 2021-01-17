using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Entities;
using CaveStoryModdingFramework.Maps;
using static PixelModdingFramework.Rendering;
using static CaveStoryEditor.SharedGraphics;
using System.ComponentModel;
using CaveStoryModdingFramework.Stages;

namespace CaveStoryEditor
{
    public partial class FormStageEditor : Form
    {

        #region unsaved edits

        bool unsavedEdits = false;
        public bool UnsavedEdits
        {
            get => unsavedEdits;
            private set
            {
                if(unsavedEdits != value)
                {
                    unsavedEdits = value;
                    UpdateTitle();
                }
            }
        }
        #endregion

        /// <summary>
        /// Updates the window title
        /// </summary>
        private void UpdateTitle()
        {
            this.Text = stageEntry.MapName;
            if (UnsavedEdits)
                this.Text += "*";
        }

        private readonly ImageList EntityIcons = new ImageList();

        readonly Mod parentMod;
        readonly IDictionary<WinFormsKeybinds.KeyInput, string> Keybinds;
        
        /// <summary>
        /// List of loaded entities
        /// </summary>
        List<Entity> Entities
        {
            get => entities;
            set
            {
                entities = value;

                listboxCanUpdateSelection = false;
                entityListBox.DataSource = entities;
                listboxCanUpdateSelection = true;
            }
        }
        List<Entity> entities;

        readonly SpriteCache Cache;

        /// <summary>
        /// The map object this is editing
        /// </summary>
        Map map;
        /// <summary>
        /// The attributes for this map's tiles
        /// </summary>
        CaveStoryModdingFramework.Maps.Attribute attributes;
        /// <summary>
        /// Image for all tile types
        /// </summary>
        readonly Bitmap tileTypes;

        public string Filename => stageEntry.Filename;
        public StageEntry stageEntry { get; private set; }

        void UnlinkEntry()
        {
            stageEntry.PropertyChanged -= Entry_PropertyChanged;
            stageEntry = (StageEntry)stageEntry.Clone();
        }

        private void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StageEntry.TilesetName):
                    
                    break;
                case nameof(StageEntry.Filename):
                    
                    break;
                case nameof(StageEntry.BackgroundType):
                    
                    break;
                case nameof(StageEntry.BackgroundName):
                    
                    break;
                case nameof(StageEntry.Spritesheet1):
                    
                    break;
                case nameof(StageEntry.Spritesheet2):
                    
                    break;
                case nameof(StageEntry.BossNumber):
                    
                    break;
                //No check for Japanese Name since we wouldn't need to do anything
                case nameof(StageEntry.MapName):

                    break;
            }
        }

        public void NotifyMapStateRefreshNeeded()
        {

        }
        string mapPath => Path.Combine(parentMod.DataFolderPath, parentMod.StageFolderPath, stageEntry.Filename + "." + parentMod.StageExtension);
        void LoadTiles()
        {
            //TODO TEMP auto creation
            if (File.Exists(mapPath))
                map = new Map(mapPath);
            else
                map = new Map(21, 16, 0);
            map.MapResizing += MapResizing;
            map.MapResized += MapResized;
            mapResizeControl.InitSize(map.Width, map.Height, map.CurrentBufferSize);
            InitMapImage();
        }
        string tilesetPath => Path.Combine(parentMod.DataFolderPath, parentMod.StageFolderPath, parentMod.TilesetPrefix + stageEntry.TilesetName + "." + parentMod.ImageExtension);
        void LoadTileset()
        {
            InitTilesetAndTileTypes(new Bitmap(tilesetPath));
        }
        string attributePath => Path.Combine(parentMod.DataFolderPath, parentMod.StageFolderPath, stageEntry.TilesetName + "." + parentMod.AttributeExtension);
        void LoadAttributes()
        {
            //TODO TEMP autocreation
            if (File.Exists(attributePath))
                attributes = new CaveStoryModdingFramework.Maps.Attribute(attributePath);
            else
                attributes = new CaveStoryModdingFramework.Maps.Attribute();
        }

        string entityPath => Path.Combine(parentMod.DataFolderPath, parentMod.StageFolderPath, stageEntry.Filename + "." + parentMod.EntityExtension);
        void LoadEntities()
        {
            //clear existing
            if (Entities?.Count > 0)
            {
                foreach (var ent in Entities)
                {
                    ent.PropertyChanging += EntityPropertyChanging;
                    ent.PropertyChanged += EntityPropertyChanged;
                }
                Entities.Clear();
            }

            //load new
            //TODO TEMP autocreateion
            if (File.Exists(entityPath))
                Entities = PXE.Read(entityPath);
            else
                Entities = new List<Entity>();
            foreach (var ent in Entities)
            {
                ent.PropertyChanging += EntityPropertyChanging;
                ent.PropertyChanged += EntityPropertyChanged;
            }
        }

        public FormStageEditor(Mod m, SpriteCache cache, IDictionary<WinFormsKeybinds.KeyInput, string> keybinds, StageEntry entry)
        {
            //everything needs this stuff
            parentMod = m;
            Keybinds = keybinds;
            stageEntry = entry;
            Cache = cache;
            
            void GenerateMany(SurfaceSourceFile file, int index)
            {
                foreach(var surf in parentMod.SurfaceDescriptors)
                {
                    if(surf.Value is SurfaceSourceIndex ssi && ssi.Index == index)
                        Cache.GenerateLocal(file, surf.Key);
                }
            }
            GenerateMany(tilesetSurfaceSource, 0);
            GenerateMany(backgroundSurfaceSource, 1);
            GenerateMany(spritesheet1SurfaceSource, 2);
            GenerateMany(spritesheet2SurfaceSource, 3);

            //UI init
            InitializeComponent();
            UpdateTitle();
            mapLayeredPictureBox.MouseWheel += mapPictureBox_MouseWheel;

            #region init map layers
            backgroundPreview = mapLayeredPictureBox.CreateLayer();
            
            baseMap = mapLayeredPictureBox.CreateLayer();
            mapTileTypes = mapLayeredPictureBox.CreateLayer();
            mapTileGrid = mapLayeredPictureBox.CreateLayer();

            entityIcons = mapLayeredPictureBox.CreateLayer();
            entityBoxes = mapLayeredPictureBox.CreateLayer();

            selectedEntityIcons = mapLayeredPictureBox.CreateLayer();
            selectedEntityBoxes = mapLayeredPictureBox.CreateLayer();

            screenPreview = mapLayeredPictureBox.CreateLayer();
            InitScreenPreview();
            playerPreview = mapLayeredPictureBox.CreateLayer();
            
            mouseOverlay = mapLayeredPictureBox.CreateLayer();
            RestoreMouseSize();
            #endregion

            #region init tileset layers
            baseTileset = tilesetLayeredPictureBox.CreateLayer();
            tilesetTileTypes = tilesetLayeredPictureBox.CreateLayer();
            tilesetMouseOverlay = tilesetLayeredPictureBox.CreateLayer();
            #endregion

            tileTypes = new Bitmap(Editor.Default.TileTypePath); //TODO this won't work
            LoadAttributes();
            InitEntityList();
            LoadEntities();
            LoadTileset();
            LoadTiles();

            //need to init entity images after the map has been initialized so we actually have a good size
            //need to init them each seperately since otherwise I think it would all be a reference to the same image...
            ResizeAllEntityLayers();
            DrawEntityIcons();
            DrawEntityBoxes();

            tileTypesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            gridToolStripMenuItem_CheckedChanged(this, new EventArgs());
            entitySpritesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            entityBoxesToolStripMenuItem_CheckedChanged(this, new EventArgs());
        }

        private void FormStageEditor_Load(object sender, EventArgs e)
        {
            //need to update these scrollbars once the form has loaded, otherwise they are permanently hidden
            screenPreviewToolStripMenuItem_CheckedChanged(this, new EventArgs());
        }

        /// <summary>
        /// Saves the loaded map/entities
        /// </summary>
        private void Save()
        {
            map.Save(mapPath);
            //attributes.Save();
            PXE.Write(Entities, entityPath);
            
            LastSavedActionIndex = HistoryIndex;
            UnsavedEdits = false;
        }

        #region Zoom

        const int MaxZoom = 10;

        int ZoomLevel
        {
            get => mapLayeredPictureBox.CanvasScale;
            set
            {
                if (1 <= value && value <= MaxZoom)
                {
                    //changing the scroll value in here doesn't work, it gets set to something else shortly after
                    //so it has to wait until later...
                    scrollBarNeedsUpdate = true;
                    scrollBarMultiplier = pictureBoxPanel.VerticalScroll.Value / (decimal)(pictureBoxPanel.VerticalScroll.Maximum - pictureBoxPanel.VerticalScroll.LargeChange + 1);
                    //setting this last so the map gets updated at the right time
                    mapLayeredPictureBox.CanvasScale  = value;
                }
            }
        }
        private void mapPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if(ModifierKeys == Keys.Control)
                ZoomLevel += (e.Delta > 0) ? 1 : -1;
        }

        bool scrollBarNeedsUpdate = false;
        decimal scrollBarMultiplier = 0;
        //HACK need to find a better event to hook into than paint
        void UpdateMapScrollPosition()
        {
            if (scrollBarNeedsUpdate)
            {
                pictureBoxPanel.VerticalScroll.Value = (int)Math.Round((pictureBoxPanel.VerticalScroll.Maximum - pictureBoxPanel.VerticalScroll.LargeChange + 1) * scrollBarMultiplier);
                pictureBoxPanel.PerformLayout();
                scrollBarNeedsUpdate = false;
            }
        }

        #endregion

        #region map and entity init

        /// <summary>
        /// Initializes the entire map from scratch.
        /// only used on startup and resize
        /// </summary>
        void InitMapImage()
        {
            //hiding both of these so they can't affect the canvas size
            bool spShown = screenPreview.Shown;
            screenPreview.Shown = false;
            bool mShown = mouseOverlay.Shown;
            mouseOverlay.Shown = false;
            
            mapLayeredPictureBox.UnlockCanvasSize();
            {   
                baseMap.Image = RenderTiles(map, (Bitmap)baseTileset.Image, parentMod.TileSize);

                mapTileGrid.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
                RedrawGrid();

                //init screen preview max
                //TODO CS camera is more complicated than this
                var vMax = (map.Height * parentMod.TileSize) - parentMod.ScreenHeight;
                var hMax = (map.Width * parentMod.TileSize) - parentMod.ScreenWidth;
                vScreenPreviewScrollBar.Maximum = vMax + vScreenPreviewScrollBar.LargeChange - 1;
                hScreenPreviewScrollBar.Maximum = hMax + hScreenPreviewScrollBar.LargeChange - 1;
                //clamp the screen preview inside the map
                UpdateScreenPreviewLocation(Math.Min(screenPreview.Location.X, hMax), Math.Min(screenPreview.Location.Y, vMax));
                //this call won't do anything the first time this function is called, but it will work on subsequent calls
                UpdateScreenPreviewScrollbars();

                mapTileTypes.Image = RenderTiles(map, (Bitmap)tilesetTileTypes.Image, parentMod.TileSize);
            }
            mapLayeredPictureBox.LockCanvasSize();

            //show the ones that were shown again
            screenPreview.Shown = spShown;
            mouseOverlay.Shown = mShown;
        }
        int[] EntityIconsTransferIndexes;
        SurfaceSourceFile tilesetSurfaceSource => new SurfaceSourceFile(Folders.Stage, Prefixes.Tileset, stageEntry.TilesetName, "");
        SurfaceSourceFile backgroundSurfaceSource => new SurfaceSourceFile(Folders.Data, Prefixes.None, stageEntry.BackgroundName, "");
        SurfaceSourceFile spritesheet1SurfaceSource => new SurfaceSourceFile(Folders.Npc, Prefixes.Spritesheet, stageEntry.Spritesheet1, "");
        SurfaceSourceFile spritesheet2SurfaceSource => new SurfaceSourceFile(Folders.Npc, Prefixes.Spritesheet, stageEntry.Spritesheet2, "");
        void UpdateEntityIcons()
        {
            EntityIcons.Images.Clear();
            EntityIcons.Images.AddRange(Cache.GenerateList(out EntityIconsTransferIndexes,
                tilesetSurfaceSource, backgroundSurfaceSource, spritesheet1SurfaceSource, spritesheet2SurfaceSource));
        }

        /// <summary>
        /// Initializes the entity list
        /// </summary>
        void InitEntityList()
        {
            UpdateEntityIcons();
            entityListView.Clear();
            entityListView.LargeImageList = EntityIcons;
            for (int i = 0; i < parentMod.NPCTable.Count; i++)
            {
                entityListView.Items.Add(new ListViewItem()
                {
                    Text = parentMod.EntityInfos.TryGetValue(i, out var inf) ? inf.Name : i.ToString(),
                    ImageIndex = EntityIconsTransferIndexes[i],
                    //ToolTipText = ;)
                });
            }
        }

        #endregion


        #region history

        int historyIndex = -1;
        int HistoryIndex
        {
            get => historyIndex;
            set
            {
                if (historyIndex != value)
                {
                    historyIndex = value;
                    UnsavedEdits = LastSavedActionIndex != historyIndex;
                }
            }
        }
        int LastSavedActionIndex = -1;
        readonly List<IUndo> History = new List<IUndo>();

        IUndo CurrentAction;

        T InitUndoAction<T>(Func<T> func) where T : IUndo
        {
            if (CurrentAction == null)
            {
                var @new = func();
                CurrentAction = @new;
                return @new;
            }
            else
                throw new InvalidOperationException("Tried to overwrite an existing action");
        }
        T InitUndoAction<T>() where T : IUndo, new()
        {
            if (CurrentAction == null)
            {
                var @new = new T();
                CurrentAction = @new;
                return @new;
            }
            else
                throw new InvalidOperationException("Tried to overwrite an existing action");
        }

        void FinalizeUndoAction()
        {
            if (CurrentAction != null)
            {
                if (HistoryIndex < History.Count - 1)
                {
                    if(HistoryIndex < LastSavedActionIndex)
                        LastSavedActionIndex = -1;
                    History.RemoveRange(HistoryIndex + 1, History.Count - (HistoryIndex + 1));
                }
                History.Add(CurrentAction);
                CurrentAction = null;
                HistoryIndex++;
            }
        }

        #region entity history
        Entity entityWithChanges = null;
        int entityPreviousValue = -1;
        enum EntityPropertyWatchModes
        {
            Ignore,
            QuickUndo,
            SlowUndo
        }
        EntityPropertyWatchModes entityTrackChanges = EntityPropertyWatchModes.QuickUndo;


        void MultiEntityPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            entityTrackChanges = EntityPropertyWatchModes.SlowUndo;
        }

        void EntityPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (entityTrackChanges != EntityPropertyWatchModes.Ignore)
            {
                InitUndoAction(() => new EntityPropertiesChanged(e.PropertyName));
                entityWithChanges = (Entity)sender;
                entityPreviousValue = (int)typeof(Entity).GetProperty(e.PropertyName).GetValue(entityWithChanges);
            }
        }
        void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (entityTrackChanges != EntityPropertyWatchModes.Ignore)
            {
                if (entityWithChanges == null)
                    throw new NoNullAllowedException("A property changed event was fired without an accompanying property changing event");
                if (entityWithChanges != sender)
                    throw new ArgumentException("An entity failed to send a property changing event");

                var newVal = (int)typeof(Entity).GetProperty(e.PropertyName).GetValue(entityWithChanges);
                var c = (EntityPropertiesChanged)CurrentAction;
                if (!c.Entities.ContainsKey(entityWithChanges))
                    c.Entities.Add(entityWithChanges, new EntityPropertiesChanged.EntityPropertyChanged(entityPreviousValue, newVal));
                else
                    throw new ArgumentException("An entity somehow sent two edit messages without finalizing the undo action");

                entityWithChanges = null;

                if (entityTrackChanges == EntityPropertyWatchModes.QuickUndo)
                    FinalizeUndoAction();
            }
        }

        void MultiEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FinalizeUndoAction();
            entityTrackChanges = EntityPropertyWatchModes.QuickUndo;
        }
        #endregion

        #region map history

        bool TrackMapResizes = true;
        void MapResizing(object sender, EventArgs e)
        {
            if (TrackMapResizes)
            {
                InitUndoAction(() => new MapResized<short,byte?>(map.Width, map.Height, map.Tiles.ToArray()));
            }
        }
        void MapResized(object sender, EventArgs e)
        {
            if (TrackMapResizes)
            {
                var act = ((MapResized<short,byte?>)CurrentAction);
                act.NewWidth = map.Width;
                act.NewHeight = map.Height;
                act.NewTiles = map.Tiles.ToArray();
                FinalizeUndoAction();
                InitMapImage();
                ResizeAllEntityLayers();
                RedrawAllEntityLayers();
            }
        }

        #endregion

        void PerformUndo(IUndo action, bool undo)
        {
            if (action is TilesPlaced tp)
            {
                foreach (var tile in tp.Tiles)
                    SetTile(tile.Key, undo ? tile.Value.OldValue : tile.Value.NewValue);
            }
            else if (action is EntityPropertiesChanged epc)
            {
                entityTrackChanges = EntityPropertyWatchModes.Ignore;
                var property = typeof(Entity).GetProperty(epc.Property);
                foreach (var ent in epc.Entities)
                {
                    property.SetValue(ent.Key, undo ? ent.Value.OldValue : ent.Value.NewValue);
                }
                entityTrackChanges = EntityPropertyWatchModes.QuickUndo;
            }
            else if (action is EntitiesMoved em)
            {
                entityTrackChanges = EntityPropertyWatchModes.Ignore;
                foreach (var ent in em.Entities)
                {
                    if (undo)
                    {
                        ent.Key.X = (short)ent.Value.OldLocation.X;
                        ent.Key.Y = (short)ent.Value.OldLocation.Y;
                    }
                    else
                    {
                        ent.Key.X = (short)ent.Value.NewLocation.X;
                        ent.Key.Y = (short)ent.Value.NewLocation.Y;
                    }
                }
                entityTrackChanges = EntityPropertyWatchModes.QuickUndo;
            }
            else if (action is EntityListChanged elc)
            {
                Entities = new List<Entity>(undo ? elc.OldEntities : elc.NewEntities);
            }
            else if(action is MapResized<short,byte?> mr)
            {
                TrackMapResizes = false;
                if (undo)
                {
                    map.Tiles = mr.OldTiles.ToList();
                    map.Resize(mr.OldWidth, mr.OldHeight, ResizeModes.Buffer);
                }
                else
                {
                    map.Tiles = mr.NewTiles.ToList();
                    map.Resize(mr.NewWidth, mr.NewHeight, ResizeModes.Buffer);
                }
                mapResizeControl.InitSize(map.Width, map.Height, map.CurrentBufferSize);
                TrackMapResizes = true;
            }
        }
        IUndo Undo()
        {
            if (0 <= HistoryIndex && HistoryIndex < History.Count)
            {
                var h = History[HistoryIndex];
                PerformUndo(History[HistoryIndex], true);
                HistoryIndex--;
                return h;
            }
            return null;
        }
        IUndo Redo()
        {
            if(-1 <= HistoryIndex && HistoryIndex < History.Count - 1)
            {
                HistoryIndex++;
                PerformUndo(History[HistoryIndex], false);
                return History[HistoryIndex];
            }
            return null;
        }

        #endregion


        #region edit map

        Point selectedTilesOffset = new Point(0,0);
        Map SelectedTiles = new Map(1, 1, 0);
        void SelectTilesFromMap(Point p)
        {
            SelectTilesFromMap(p, p);
        }
        void SelectTilesFromMap(Point start, Point end)
        {
            var rect = GetRect(start, end);
            selectedTilesOffset = new Point(end.X - rect.X, end.Y - rect.Y);
            SelectedTiles = new Map((short)(rect.Width + 1), (short)(rect.Height + 1), 0);
            for (int i = 0; i < rect.Height + 1; i++)
                for (int j = 0; j < rect.Width + 1; j++)
                    SelectedTiles.Tiles[(i*SelectedTiles.Width) + j] = map.Tiles[((rect.Y + i) * map.Width) + rect.X + j];
        }
        void SelectTilesFromTileset(Point start, Point end)
        {
            var rect = GetRect(start, end);
            selectedTilesOffset = new Point(end.X - rect.X, end.Y - rect.Y);
            SelectedTiles = new Map((short)(rect.Width + 1), (short)(rect.Height + 1), 0);
            for (int i = 0; i < rect.Height + 1; i++)
                for (int j = 0; j < rect.Width + 1; j++)
                    SelectedTiles.Tiles[(i * SelectedTiles.Width) + j] = (byte)(((rect.Y + i) * TilesetWidth) + rect.X + j);
        }


        void SetTiles(Point p, Point offset)
        {
            SetTiles(new Point(p.X - offset.X, p.Y - offset.Y), SelectedTiles);
        }
        void SetTiles(int tileNum)
        {
            SetTiles(new Point(tileNum%map.Width, tileNum/map.Width), new Point(0,0));
        }
        void SetTile(int tileNum, byte? tileValue)
        {
            //this is here so the title updates right away when you start drawing, instead of after you mouse up
            UnsavedEdits = true;
            if (CurrentAction != null)
            {
                var act = (TilesPlaced)CurrentAction;
                if (!act.Tiles.ContainsKey(tileNum))
                    act.Tiles.Add(tileNum, new TilesPlaced.TileChanged<byte?>(map.Tiles[tileNum], tileValue));
                else
                    act.Tiles[tileNum].NewValue = tileValue;
            }
            map.Tiles[tileNum] = tileValue;
            DrawTile(baseMap.Image, map, tileNum, (Bitmap)baseTileset.Image, parentMod.TileSize);
            DrawTile(mapTileTypes.Image, map, tileNum, (Bitmap)tilesetTileTypes.Image, parentMod.TileSize, System.Drawing.Drawing2D.CompositingMode.SourceCopy);
        }
        void SetTiles(Point p, Map tileSource)
        {
            //clamp the bottom/right boundry
            var effectiveHeight = Math.Min(tileSource.Height, map.Height - p.Y);
            var effectiveWidth = Math.Min(tileSource.Width, map.Width - p.X);
            //if the coord is bigger when "* -1", that means we're outside the top/left boundry and need to clamp
            for (int y = Math.Max(0, p.Y * -1); y < effectiveHeight; y++)
            {
                for (int x = Math.Max(0, p.X * -1); x < effectiveWidth; x++)
                { 
                    SetTile(((p.Y + y) * map.Width) + p.X + x, tileSource.Tiles[(y * tileSource.Width) + x]);
                }
            }
        }

        #endregion

        #region entity stuff

        /// <summary>
        /// Whether or not the user has any entities selected
        /// </summary>
        bool userHasSelectedEntities { get => selectedEntities.Count > 0; }
        readonly HashSet<Entity> selectedEntities = new HashSet<Entity>();

        /// <summary>
        /// Whether or not the user as copied any entities
        /// </summary>
        bool entitiesInClipboard { get => entityClipboard.Count > 0; }
        readonly HashSet<Entity> entityClipboard = new HashSet<Entity>();

        private string GetEntityIndexAndName(Entity ent)
        {
            string entityName = "<no name>";
            if (parentMod.EntityInfos.TryGetValue(ent.Type, out var entInf))
                entityName = entInf.Name;
            return $"{Entities.IndexOf(ent)} - {entityName}";
        }
        private void AddEntity(Entity ent)
        {
            ent.PropertyChanging += EntityPropertyChanging;
            ent.PropertyChanged += EntityPropertyChanged;
            Entities.Add(ent);
        }
        private void InsertEntity(int index, Entity ent)
        {
            ent.PropertyChanging += EntityPropertyChanging;
            ent.PropertyChanged += EntityPropertyChanged;
            Entities.Insert(index, ent);
        }
        private void RemoveEntity(Entity ent)
        {
            Entities.Remove(ent);
            ent.PropertyChanging -= EntityPropertyChanging;
            ent.PropertyChanged -= EntityPropertyChanged;
        }

        private IEnumerable<Entity> GetEntitiesAtLocation(Point p)
        {
            return GetEntitiesAtLocation(new Rectangle(p, new Size(0,0)));
        }
        private IEnumerable<Entity> GetEntitiesAtLocation(Rectangle rect)
        {
            for(int i = 0; i < Entities.Count; i++)
                if (rect.X <= Entities[i].X && Entities[i].X <= rect.Right && rect.Y <= Entities[i].Y && Entities[i].Y <= rect.Bottom)
                    yield return Entities[i];
        }                
        void CreateNewEntity(Point pos)
        {
            if (entitySelected)
            {
                var ent = new Entity((short)pos.X, (short)pos.Y, 0, 0, (short)entityListView.SelectedIndices[0], 0);
                var act = InitUndoAction(() => new EntityListChanged(Entities.ToArray()));

                AddEntity(ent);
                SelectEntities(ent);
                //RedrawAllEntityLayers();

                act.NewEntities = Entities.ToArray();
                FinalizeUndoAction();
            }
        }
        void DeleteSelectedEntities()
        {
            var act = InitUndoAction(() => new EntityListChanged(Entities.ToArray()));
            foreach (var ent in selectedEntities.ToArray())
            {
                RemoveEntity(ent);
            }
            SafeRefreshItems();
            SelectEntities();
            //RedrawAllEntityLayers();

            act.NewEntities = Entities.ToArray();
            FinalizeUndoAction();
        }

        void MoveSelectedEntities(Point p)
        {
            var act = CurrentAction as EntitiesMoved ?? InitUndoAction<EntitiesMoved>();
            entityTrackChanges = EntityPropertyWatchModes.Ignore;

            var xd = lastMousePosition.X - p.X;
            var yd = lastMousePosition.Y - p.Y;
            foreach (var ent in selectedEntities)
            {
                var nx = ent.X - xd;
                var ny = ent.Y - yd;
                if (!act.Entities.ContainsKey(ent))
                    act.Entities.Add(ent, new EntitiesMoved.MovedEntity(new Point(ent.X, ent.Y), new Point(nx, ny)));
                else
                    act.Entities[ent].NewLocation = new Point(nx, ny);

                ent.X = (short)nx;
                ent.Y = (short)ny;
            }
            void UpdateLayerLocation(LayeredPictureBox.Layer<Image> layer)
            {
                layer.Location = new Point(layer.Location.X - (xd * parentMod.TileSize), layer.Location.Y - (yd * parentMod.TileSize));
            }
            UpdateLayerLocation(selectedEntityBoxes);
            UpdateLayerLocation(selectedEntityIcons);
            
            entityTrackChanges = EntityPropertyWatchModes.QuickUndo;
        }

        /// <summary>
        /// Set what entities are being edited in the entityPorpertyGrid
        /// </summary>
        /// <param name="ents">Entities to edit</param>
        private void SetEditingEntity(params Entity[] ents)
        {
            if(entityPropertyGrid.SelectedObject is MultiEntityShell mes)
            {
                mes.PropertyChanging -= MultiEntityPropertyChanging;
                mes.PropertyChanged -= MultiEntityPropertyChanged;
            }
            if (ents.Length == 0)
                entityPropertyGrid.SelectedObject = null;
            else if(ents.Length == 1)
            {
                //if the entity has a custom type, use that, otherwise edit the entity directly
                //TODO custom type needs to be adjusted for
                entityPropertyGrid.SelectedObject = 
                    (parentMod.EntityInfos.TryGetValue(ents[0].Type, out EntityInfo t) && t.CustomType != null)
                    ? Activator.CreateInstance(t.CustomType, ents) : ents[0];
                entityTrackChanges = EntityPropertyWatchModes.QuickUndo;
            }
            else
            {
                var multi = new MultiEntityShell(ents);
                multi.PropertyChanging += MultiEntityPropertyChanging;
                multi.PropertyChanged += MultiEntityPropertyChanged;
                entityPropertyGrid.SelectedObject = multi;
            }
        }
        /// <summary>
        /// Sets the given entities as selected. Passing no args will deselect
        /// </summary>
        /// <param name="ents">Entities to select</param>
        private void SelectEntities(params Entity[] ents)
        {
            //clear everything
            selectedEntities.Clear();
            listboxCanUpdateSelection = false;
            entityListBox.SelectedItems.Clear();
            SetEditingEntity();

            //only select things if there's something to select
            if(ents.Length > 0)
            { 
                foreach (var e in ents)
                {
                    selectedEntities.Add(e);
                    entityListBox.SelectedItems.Add(e);
                }
                //try to scroll to the selected entity type, if only one is selected
                if (selectedEntities.Count == 1)
                {
                    var e = selectedEntities.First();
                    //can't scroll to an entity that doesn't exist
                    if (0 <= e.Type && e.Type < entityListView.Items.Count)
                    {
                        entityListView.SelectedIndices.Clear();
                        entityListView.SelectedIndices.Add(e.Type);
                        entityListView.EnsureVisible(e.Type);
                    }
                }
                SetEditingEntity(ents);
            }
            selectedEntityBoxes.Location = selectedEntityIcons.Location = new Point(0, 0);
            
            listboxCanUpdateSelection = true;
            RedrawAllEntityLayers();
        }
        /// <summary>
        /// Copies the given entities to the clipboard
        /// </summary>
        /// <param name="ents">Entities to copy</param>
        void CopyEntities(params Entity[] ents)
        {
            Point topLeft = new Point(ents.Min(x => x.X), ents.Min(x => x.Y));
            entityClipboard.Clear();
            foreach (var e in ents)
            {
                entityClipboard.Add(new Entity(e) {
                X = (short)(e.X - topLeft.X),
                Y = (short)(e.Y - topLeft.Y)
                });
            }
        }
        /// <summary>
        /// Pastes the entities currently in the clipboard to the given grid position
        /// </summary>
        /// <param name="gridPos">Where on the grid to paste the entities</param>
        void PasteEntities(Point gridPos)
        {
            if (entitiesInClipboard)
            {
                var act = InitUndoAction(() => new EntityListChanged(Entities.ToArray()));

                foreach (var e in entityClipboard)
                {
                    AddEntity(new Entity(e)
                    {
                        X = (short)(gridPos.X + e.X),
                        Y = (short)(gridPos.Y + e.Y)
                    });
                }
                RedrawAllEntityLayers();
                SafeRefreshItems();
                SelectEntities(selectedEntities.ToArray());

                act.NewEntities = Entities.ToArray();
                FinalizeUndoAction();
            }
        }
        #endregion

        #region Mouse

        /// <summary>
        /// The mouse's current position on the grid
        /// </summary>
        Point lastMousePosition = new Point(-1, -1);

        /// <summary>
        /// The grid position the user started selecting entities from
        /// </summary>
        Point startMousePosition = new Point(-1, -1);

        /// <summary>
        /// All possible editing modes
        /// </summary>
        enum EditModes
        {
            Tile = 1,
            Entity = 2
        }
        /// <summary>
        /// What the user is currently editing
        /// </summary>
        EditModes editMode
        {
            get 
            {
                switch(editModeTabControl.SelectedIndex)
                {
                    case 0: //Tile editing
                        return EditModes.Tile;
                    case 1: //Entity Editing
                    case 2: //Entity List
                        return EditModes.Entity;
                    case 3: //Map States
                        return EditModes.Tile;
                    default: //Just in case...
                        return EditModes.Tile;
                }
            }
        }

        /// <summary>
        /// The bottom right point of the map
        /// </summary>
        Point maxGridPoint => new Point(map.Width - 1, map.Height - 1);


        /// <summary>
        /// converts a cursor location to a point on the tileset
        /// </summary>
        /// <param name="p">Cursor location</param>
        /// <returns>The coords of the tile the cursor is hovering over</returns>
        private Point GetMousePointOnTileset(Point p)
        {
            return new Point(p.X / parentMod.TileSize, p.Y / parentMod.TileSize);
        }
        /// <summary>
        /// converts a cursor location to a point on the map
        /// </summary>
        /// <param name="p">Cursor location</param>
        /// <returns>The coords of the tile the cursor is hovering over</returns>
        private Point GetMousePointOnMap(Point p)
        {
            return new Point(p.X / (parentMod.TileSize * ZoomLevel), p.Y / (parentMod.TileSize * ZoomLevel));
        }

        /// <summary>
        /// Whether or not the mouse is currently on the map
        /// </summary>
        bool mouseOnMap { get => mapLayeredPictureBox.ClientRectangle.Contains(mapLayeredPictureBox.PointToClient(Cursor.Position)); }
        /// <summary>
        /// Whether or not the user has selected an entity from the list of all entities
        /// </summary>
        bool entitySelected { get => entityListView.SelectedIndices.Count > 0; }

        enum HoldActions
        {
            DrawTiles,
            CopyTiles,
            SelectEntities,
            MoveEntities
        }

        HoldActions? HoldAction = null;

        #region context menu methods n stuff

        /// <summary>
        /// The delete context menu item, just to save on creating a new one every time
        /// </summary>
        ToolStripMenuItem delete = null;
        bool entityContextMenuShown => entityContextMenu?.Visible ?? false;
        ContextMenuStrip entityContextMenu = null;

        //can't do one for insert because it relies on the location of the click...
        void EntityContectMenu_CopyEntity(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                CopyEntities(Entities[int.Parse(tsmi.Name)]);
            }
        }
        //paste also relies on that location...
        void EntityContextMenu_Delete(object sender, EventArgs e)
        {
            DeleteSelectedEntities();
            MoveMouse(lastMousePosition, selectedTilesOffset);
        }
        void EntityContextMenu_SelectEntity(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                SelectEntities(Entities[int.Parse(tsmi.Name)]);
            }
        }

        void EntityContextMenu_VisibleChanged(object sender, EventArgs e)
        {
            if(sender is ContextMenuStrip cms && cms.Visible == false && !mouseOnMap)
            {
                mapPictureBox_MouseLeave(sender, e);
            }
        }
        #endregion

        private void mapPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBoxPanel.Select();
            
            Point p = GetMousePointOnMap(e.Location);

            switch (editMode)
            {
                //Map
                case EditModes.Tile:
                    var tile = (p.Y * map.Width) + p.X;
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            InitUndoAction<TilesPlaced>();
                            HoldAction = HoldActions.DrawTiles;
                            mouseOverlay.Shown = false;
                            SetTiles(p, selectedTilesOffset);
                            mapLayeredPictureBox.Invalidate();
                            break;
                        case MouseButtons.Middle: //Making these both do the same thing since not everyone has a middle mouse button
                        case MouseButtons.Right:
                            HoldAction = HoldActions.CopyTiles;
                            mouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.CursorColor);
                            MoveMouse(p, selectedTilesOffset = new Point(0, 0));
                            startMousePosition = p;
                            break;
                    }
                    break;
                //Entity
                case EditModes.Entity:
                    var entitiesWhereClicked = GetEntitiesAtLocation(p);
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            //if the user is clicking on an entity that's already selected
                            if(entitiesWhereClicked.Intersect(selectedEntities).Any())
                            {
                                //start moving
                                HoldAction = HoldActions.MoveEntities;
                                mouseOverlay.Shown = false;
                            }
                            //if the user isn't, that means they're either:
                            // a. clicking on a new entity, or
                            // b. clicking on an empty space
                            // either way, we're selecting something new
                            else
                            {
                                HoldAction = HoldActions.SelectEntities;
                                startMousePosition = p;
                            }
                            break;
                        //Context menu
                        case MouseButtons.Right:
                            #region Context menu stuff
                            //basic init
                            var hoveredEntitiesCount = entitiesWhereClicked.Count();
                            entityContextMenu = new ContextMenuStrip();

                            //Insert
                            var insert = new ToolStripMenuItem("Insert Entity");
                            insert.Enabled = entitySelected;
                            insert.Click += delegate { if (entitySelected) { CreateNewEntity(p); MoveMouse(lastMousePosition, selectedTilesOffset); SafeRefreshItems(); } };
                            entityContextMenu.Items.Add(insert);
                                                        
                            //Copy
                            //TODO expand on copy/paste functionality
                            var copy = new ToolStripMenuItem("Copy");
                            //copy enabled if only one entity selected, other stuff only initiallized then too
                            if (copy.Enabled = hoveredEntitiesCount == 1)
                            {
                                copy.Name = Entities.IndexOf(entitiesWhereClicked.First()).ToString();
                                copy.Click += EntityContectMenu_CopyEntity;
                            }
                            entityContextMenu.Items.Add(copy);

                            //Paste
                            var paste = new ToolStripMenuItem("Paste");
                            paste.Enabled = entitiesInClipboard;
                            paste.Click += delegate { PasteEntities(p); MoveMouse(lastMousePosition, selectedTilesOffset); SafeRefreshItems(); };
                            entityContextMenu.Items.Add(paste);

                            //Delete
                            if(delete == null)
                            {
                                delete = new ToolStripMenuItem();
                                delete.Click += EntityContextMenu_Delete;
                            }                            
                            delete.Text = $"Delete Entit{(selectedEntities.Count > 1 ? "ies" : "y")}";
                            delete.Enabled = userHasSelectedEntities;
                            entityContextMenu.Items.Add(delete);

                            //Add buttons to select stacked entities
                            if (hoveredEntitiesCount > 1)
                            {
                                entityContextMenu.Items.Add(new ToolStripSeparator());
                                foreach(var ent in entitiesWhereClicked)
                                {
                                    var index = Entities.IndexOf(ent);

                                    //TODO temp text
                                    
                                    var tsmi = new ToolStripMenuItem(GetEntityIndexAndName(ent));
                                    tsmi.Name = index.ToString();
                                    tsmi.Click += EntityContextMenu_SelectEntity;
                                    entityContextMenu.Items.Add(tsmi);
                                }
                            }
                            entityContextMenu.VisibleChanged += EntityContextMenu_VisibleChanged;
                            entityContextMenu.Show(mapLayeredPictureBox, e.Location);
                            #endregion
                            break;
                    }
                    break;
            }
        }

        private void mapLayeredPictureBox_MouseEnter(object sender, EventArgs e)
        {
            mouseOverlay.Shown = true;
        }
        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnMap(e.Location);
            //TODO could do with making a Clamp method
            p.X = Math.Max(0, Math.Min(p.X, maxGridPoint.X));
            p.Y = Math.Max(0, Math.Min(p.Y, maxGridPoint.Y));
            //if we're still on the same grid space, stop
            if (p == lastMousePosition)
                return;
            
            switch (HoldAction)
            {
                case HoldActions.DrawTiles:
                    SetTiles(p, selectedTilesOffset);
                    mapLayeredPictureBox.Invalidate();
                    break;
                case HoldActions.CopyTiles:
                    UpdateMouseMarquee(startMousePosition, p, mouseOverlay, parentMod.TileSize, UI.Default.CursorColor);
                    break;
                case HoldActions.MoveEntities:
                    MoveSelectedEntities(p);
                    mapLayeredPictureBox.Invalidate();
                    break;
                case HoldActions.SelectEntities:
                    UpdateMouseMarquee(startMousePosition, p, mouseOverlay, parentMod.TileSize, UI.Default.CursorColor);
                    break;
                default:
                    MoveMouse(p, selectedTilesOffset);
                    break;

            }
            lastMousePosition = p;
        }

        private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch(HoldAction)
            {
                case HoldActions.SelectEntities:
                    SelectEntities(GetEntitiesAtLocation(GetRect(startMousePosition, lastMousePosition)).ToArray());
                    RestoreMouseSize();
                    break;
                case HoldActions.CopyTiles:
                    SelectTilesFromMap(startMousePosition, lastMousePosition);
                    RestoreMouseSize();
                    if(SelectedTiles.Tiles.Count == 1 && SelectedTiles.Tiles[0] != null)
                    {
                        tilesetMouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.SelectedTileColor);
                        tilesetMouseOverlay.Location = new Point((SelectedTiles.Tiles[0].Value % TilesetWidth)*parentMod.TileSize, (SelectedTiles.Tiles[0].Value / TilesetWidth)*parentMod.TileSize);
                        tilesetMouseOverlay.Shown = true;
                    }
                    else
                    {
                        tilesetMouseOverlay.Shown = false;
                    }
                    break;
                case HoldActions.DrawTiles:
                case HoldActions.MoveEntities:
                    FinalizeUndoAction();
                    break;
            }
            MoveMouse(lastMousePosition, selectedTilesOffset);
            mouseOverlay.Shown = true;
            HoldAction = null;
        }

        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            HoldAction = null;
            //this check is here to stop the mouse hiding when the context menu appears, since that triggers MouseLeave
            if (!entityContextMenuShown)
                mouseOverlay.Shown = false;            
            RestoreMouseSize();
        }

        #endregion

        #region keyboard
        private void FormStageEditor_KeyDown(object sender, KeyEventArgs e)
        {
            //Kinda hacky way of checking if the user is editing something in a property grid
            if (entityPropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit" || mapResizeControl.IsBeingEdited)
                return;
            
            var input = new WinFormsKeybinds.KeyInput(e.KeyData);
            if (Keybinds.ContainsKey(input))
            {
                switch (Keybinds[input])
                {
                    case "ZoomIn":
                        ZoomLevel++;
                        break;
                    case "ZoomOut":
                        ZoomLevel--;
                        break;
                    case "PickTile" when editMode == EditModes.Tile && mouseOnMap:
                        SelectTilesFromMap(lastMousePosition);
                        tilesetMouseOverlay.Shown = false;
                        break;
                    case "DeleteEntities" when editMode == EditModes.Entity && userHasSelectedEntities:
                        DeleteSelectedEntities();
                        mapLayeredPictureBox.Invalidate();
                        break;
                    case "InsertEntity" when editMode == EditModes.Entity && mouseOnMap:
                        CreateNewEntity(lastMousePosition);
                        mapLayeredPictureBox.Invalidate();
                        SafeRefreshItems();
                        break;
                    case "Copy" when editMode == EditModes.Entity && userHasSelectedEntities:
                        CopyEntities(selectedEntities.ToArray());
                        break;
                    case "Paste" when editMode == EditModes.Entity && entitiesInClipboard && mouseOnMap:
                        PasteEntities(lastMousePosition);
                        mapLayeredPictureBox.Invalidate();
                        SafeRefreshItems();
                        break;
                    case "Undo" when !entityContextMenuShown:
                        PostUndoUIUpdate(Undo());
                        break;
                    case "Redo" when !entityContextMenuShown:
                        PostUndoUIUpdate(Redo());
                        break;
                    case "Save":
                        Save();
                        break;
                }
            }
        }

        void PostUndoUIUpdate(IUndo t)
        {
            if (t != null)
            {
                if (t is TilesPlaced)
                {
                    mapLayeredPictureBox.Invalidate();
                }
                else if (t is EntitiesMoved)
                {
                    RedrawAllEntityLayers();
                    mapLayeredPictureBox.Invalidate();
                }
                else if (t is EntityListChanged elc)
                {
                    if (elc.OldEntities.Length != elc.NewEntities.Length)
                        SelectEntities();
                    RedrawAllEntityLayers();
                    mapLayeredPictureBox.Invalidate();
                }
                else if (t is EntityPropertiesChanged epc)
                {
                    //these are the only two properties that don't change anything visible on the map
                    if (epc.Property != nameof(Entity.Flag) && epc.Property != nameof(Entity.Event) && epc.Property != nameof(Entity.Bits))
                    {
                        RedrawAllEntityLayers();
                        mapLayeredPictureBox.Invalidate();
                    }
                }
                else if(t is MapResized<short,byte>)
                {
                    InitMapImage();
                    ResizeAllEntityLayers();
                    RedrawAllEntityLayers();
                }
            }
        }

        #endregion

        #region menu buttons

        private void tileTypesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mapTileTypes.Shown = tileTypesToolStripMenuItem.Checked;
            tilesetTileTypes.Shown = tileTypesToolStripMenuItem.Checked;
        }

        private void gridToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mapTileGrid.Shown = gridToolStripMenuItem.Checked;
        }

        private void entitySpritesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            entityIcons.Shown = entitySpritesToolStripMenuItem.Checked;
        }

        private void entityBoxesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            entityBoxes.Shown = entityBoxesToolStripMenuItem.Checked;
        }


        private void selectAllEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectEntities(Entities.ToArray());
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        #endregion

        #region screen preview scroll bars

        private void ScreenPreviewScrollChanged(object sender, ScrollEventArgs e)
        {            
            int h = hScreenPreviewScrollBar.Value;
            int v = vScreenPreviewScrollBar.Value;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                v = e.NewValue;
            else if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                h = e.NewValue;
            else
                throw new ArgumentException();
            UpdateScreenPreviewLocation(h,v);
        }
                
        /// <summary>
        /// Toggles the visisbility of a scrollbar, and adjusts the size of the pictureboxpanel and the scrollbars to fit
        /// </summary>
        /// <param name="visible">Whether or not to show this scrollbar</param>
        /// <param name="sb1">The scrollbar to show/hide</param>
        /// <param name="sb2">A second scrollbar that may need to move out of the way, or that could be expanded</param>
        void ToggleScrollbarVisible(bool visible, ScrollBar sb1, ScrollBar sb2)
        {
            int GetScrollBarLength(ScrollBar sb)
            {
                if (sb is VScrollBar v)
                    return v.Height;
                else if (sb is HScrollBar h)
                    return h.Width;
                else
                    throw new ArgumentException();
            }
            void SetScrollBarLength(ScrollBar sb, int value)
            {
                if (sb is VScrollBar v)
                {
                    v.Height = value;
                    pictureBoxPanel.Height = value;
                }
                else if (sb is HScrollBar h)
                {
                    h.Width = value;
                    pictureBoxPanel.Width = value;
                }
                else
                    throw new ArgumentException();
            }
            int GetScrollBarWidth(ScrollBar sb)
            {
                if (sb is VScrollBar v)
                    return v.Width;
                else if (sb is HScrollBar h)
                    return h.Height;
                else
                    throw new ArgumentException();
            }
            //int SetScrollBarWidth(ScrollBar sb); //never need to set the scrollbar width
                        
            if (visible)
                //move the other one out of the way to make room for us
                SetScrollBarLength(sb2, GetScrollBarLength(sb2) - GetScrollBarWidth(sb1));
            else
                //the other one can take up all the space now that we're gone
                SetScrollBarLength(sb2, GetScrollBarLength(sb2) + GetScrollBarWidth(sb1));
            
            sb1.Visible = visible;
        }
        void UpdateScreenPreviewScrollbars()
        {
            bool showV = vScreenPreviewScrollBar.Maximum > vScreenPreviewScrollBar.LargeChange && screenPreviewToolStripMenuItem.Checked;
            if(vScreenPreviewScrollBar.Visible != showV)
                ToggleScrollbarVisible(showV, vScreenPreviewScrollBar, hScreenPreviewScrollBar);

            bool showH = hScreenPreviewScrollBar.Maximum > hScreenPreviewScrollBar.LargeChange && screenPreviewToolStripMenuItem.Checked;
            if(hScreenPreviewScrollBar.Visible != showH)
                ToggleScrollbarVisible(showH, hScreenPreviewScrollBar, vScreenPreviewScrollBar);
        }
        private void screenPreviewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateScreenPreviewScrollbars();
            screenPreview.Shown = screenPreviewToolStripMenuItem.Checked;
        }

        #endregion
        
        //HACK bad event to hook into for this
        private void mapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            UpdateMapScrollPosition();
        }

        private void editModeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            RestoreMouseSize();
        }

        #region entity list box

        bool listboxCanUpdateSelection = true;
        private void entityListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IEnumerable<Entity> listboxSelectedEntities()
            {
                var iter = entityListBox.SelectedItems.GetEnumerator();
                while (iter.MoveNext())
                    yield return (Entity)iter.Current;
            }
            //this check lets the user override whatever the default is being set to
            if (listboxCanUpdateSelection && !selectedEntities.SetEquals(listboxSelectedEntities()))
            {
                SelectEntities(listboxSelectedEntities().ToArray());
            }
        }

        private void entityListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Entity ent && e.DesiredType == typeof(string))
                e.Value = GetEntityIndexAndName(ent);
        }

        Rectangle dragBox = Rectangle.Empty;
        int startIndex = -1;
        
        private void entityListBox_MouseDown(object sender, MouseEventArgs e)
        {
            var clickedIndex = entityListBox.IndexFromPoint(new Point(e.X, e.Y));
            if (clickedIndex != ListBox.NoMatches && selectedEntities.Contains(Entities[clickedIndex]) && e.Button == MouseButtons.Left)
            {
                startIndex = clickedIndex;
                dragBox = new Rectangle(new Point(
                                        e.X - (SystemInformation.DragSize.Width / 2),
                                        e.Y - (SystemInformation.DragSize.Height / 2)),
                                        SystemInformation.DragSize);
            }
        }

        private void entityListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dragBox != Rectangle.Empty && !dragBox.Contains(e.Location))
            {
                listboxCanUpdateSelection = false;

                //if a drag has been started, restore the entities that are actually selected
                entityListBox.SelectedItems.Clear();
                foreach (var ent in selectedEntities)
                    entityListBox.SelectedItems.Add(ent);

                entityListBox.DoDragDrop(selectedEntities, DragDropEffects.Move);

                listboxCanUpdateSelection = true;
            }
        }

        private void entityListBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragBox = Rectangle.Empty;
        }

        private void entityListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(HashSet<Entity>)) && e.AllowedEffect == DragDropEffects.Move)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void entityListBox_DragDrop(object sender, DragEventArgs e)
        {
            int GetIndex()
            {
                var p = entityListBox.PointToClient(new Point(e.X, e.Y));
                int index = entityListBox.IndexFromPoint(p);
                //default to adding to the end of the list
                if (index < 0)
                    index = entityListBox.Items.Count;
                return index;
            }
            int endIndex = Math.Min(entityListBox.Items.Count - 1, GetIndex());
            var entitiesToMove = ((HashSet<Entity>)e.Data.GetData(typeof(HashSet<Entity>))).ToArray();
            
            //calculate how much to move each entity by
            var difference = endIndex - startIndex;
            if (0 < entitiesToMove.Length && entitiesToMove.Length < Entities.Count && difference != 0)
            {
                var act = InitUndoAction(() => new EntityListChanged(Entities.ToArray()));
                void MoveEntity(Entity ent, int newIndex)
                {
                    RemoveEntity(ent);
                    if (newIndex > Entities.Count)
                        AddEntity(ent);
                    else
                        InsertEntity(Math.Max(0, newIndex), ent);
                }
                //sort list by index
                var entsInOrder = (IEnumerable<Entity>)entitiesToMove.OrderBy(x => Entities.IndexOf(x));
                //moving down
                if (startIndex < endIndex)
                {
                    var lastIndex = Entities.Count;
                    foreach (var ent in entsInOrder.Reverse())
                    {
                        //clamp the new entity location to the highest entity placed
                        var newIndex = Math.Max(0, Math.Min(Entities.IndexOf(ent) + difference, lastIndex - 1));
                        MoveEntity(ent, newIndex);
                        lastIndex = newIndex;
                    }
                }
                //moving up
                else
                {
                    var lastIndex = -1;
                    foreach (var ent in entsInOrder)
                    {
                        //clamp the new entity location to the highest entity placed
                        var newIndex = Math.Min(Math.Max(lastIndex + 1, Entities.IndexOf(ent) + difference), Entities.Count);
                        MoveEntity(ent, newIndex);
                        lastIndex = newIndex;
                    }
                }
                act.NewEntities = Entities.ToArray();
                FinalizeUndoAction();
            }

            entityListBox_RefreshItems();

            //selecting entities after calling the above event to avoid the selected entities getting overwritten for some dumb reason
            SelectEntities(entitiesToMove);

            dragBox = Rectangle.Empty;
        }


        readonly static MethodInfo refreshItemsMethodInfo = typeof(ListBox).GetMethod("RefreshItems",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, Array.Empty<Type>(), null);
        //HACK need to refresh the entity list, and for whatever reason that method is private
        void entityListBox_RefreshItems()
        {
            refreshItemsMethodInfo.Invoke(entityListBox, Array.Empty<object>());
            /*
            typeof(ListBox).InvokeMember("RefreshItems",
              BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
              null, entityListBox, Array.Empty<object>());
            */
        }

        //HACK as if the other method wasn't bad enough...
        //the listbox doesn't seem to like refreshing its items when you remove things, so I have to just ignore the exception...?!
        //also put in the listboxCanUpdateSelection thing just for convienince
        void SafeRefreshItems()
        {
            listboxCanUpdateSelection = false;
            try
            {
                entityListBox_RefreshItems();
            }
            catch (TargetInvocationException)
            {

            }
            listboxCanUpdateSelection = true;
        }

        #endregion

        private void mapResizeControl1_MapResizeInitialized(object sender, MapResizeInitiatedEventArgs e)
        {
            map.Resize(e.NewWidth, e.NewHeight, e.ResizeMode, 0, e.ShrinkBuffer);
            e.NewWidth = map.Width;
            e.NewHeight = map.Height;
            e.NewBufferSize = map.CurrentBufferSize;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PostUndoUIUpdate(Undo());
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PostUndoUIUpdate(Redo());
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && UnsavedEdits)
            {
                switch(MessageBox.Show("You have unsaved changes! Would you like to save?", "Warning", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        Save();
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }                
            }
        }

        private void FormStageEditor_Activated(object sender, EventArgs e)
        {

        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formatNames = new[]
            {
                "Portable Network Graphics (*.png)|*.png",
                "Bitmap (*.bmp)|*.bmp",
                "Joint Picture Experts Group (*.jpg)|*.jpg"
            };
            var formatTypes = new[]
            {
                ImageFormat.Png,
                ImageFormat.Bmp,
                ImageFormat.Jpeg
            };
            using (var sfd = new SaveFileDialog()
            {
                Filter = string.Join("|", formatNames)
            })
            {
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    mapLayeredPictureBox.Flatten(1).Save(sfd.FileName, formatTypes[sfd.FilterIndex - 1]);
                }
            }            
        }
    }
}
