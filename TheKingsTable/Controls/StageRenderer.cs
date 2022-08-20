using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using CaveStoryModdingFramework.Editors;
using CaveStoryModdingFramework.Entities;
using CaveStoryModdingFramework.Maps;
using System;
using System.Collections.Generic;

using BindingFlags = System.Reflection.BindingFlags;
using static TheKingsTable.Utilities;
using System.Reactive.Linq;
using Avalonia.Data;

namespace TheKingsTable.Controls
{
    public class StageRenderer : TileMouseHelper, IStyleable
    {
        Type IStyleable.StyleKey => typeof(Control);

        static StageRenderer()
        {
            //Since literally every styled property in this class affects rendering
            //reflection seems like the way to go
            var props = new List<AvaloniaProperty>(Properties);
            foreach (var f in typeof(StageRenderer).GetFields(BindingFlags.Public | BindingFlags.Static))
                if (f.FieldType.IsAssignableTo(typeof(AvaloniaProperty)))
                    props.Add((AvaloniaProperty)f.GetValue(null)!);
            AffectsRender<StageRenderer>(props.ToArray());
        }

        //Background stuff
        #region ShowBackground Styled Property
        public static readonly StyledProperty<bool> ShowBackgroundProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(ShowBackground), true);

        public bool ShowBackground
        {
            get => GetValue(ShowBackgroundProperty);
            set => SetValue(ShowBackgroundProperty, value);
        }
        #endregion

        #region BackgroundType Styled Property
        public static readonly StyledProperty<long> BackgroundTypeProperty =
            AvaloniaProperty.Register<StageRenderer, long>(nameof(BackgroundType), 0);

        public long BackgroundType
        {
            get => GetValue(BackgroundTypeProperty);
            set => SetValue(BackgroundTypeProperty, value);
        }
        #endregion

        #region Background Styled Property
        public static readonly StyledProperty<IImage?> BackgroundProperty =
            AvaloniaProperty.Register<StageRenderer, IImage?>(nameof(Background), null);

        public IImage? Background
        {
            get => GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        #endregion

        //Tileset stuff
        #region TilesetWidth Styled Property
        public static readonly StyledProperty<int> TilesetWidthProperty =
            AvaloniaProperty.Register<StageRenderer, int>(nameof(TilesetWidth), 16);

        public int TilesetWidth
        {
            get => GetValue(TilesetWidthProperty);
            set => SetValue(TilesetWidthProperty, value);
        }
        #endregion

        #region TilesetAttributes Styled Property
        public static readonly StyledProperty<CaveStoryModdingFramework.Maps.Attribute?> TilesetAttributesProperty =
            AvaloniaProperty.Register<StageRenderer, CaveStoryModdingFramework.Maps.Attribute?>(nameof(TilesetAttributes), null);

        public CaveStoryModdingFramework.Maps.Attribute? TilesetAttributes
        {
            get => GetValue(TilesetAttributesProperty);
            set => SetValue(TilesetAttributesProperty, value);
        }
        #endregion

        #region TilesetImage Styled Property
        public static readonly StyledProperty<Bitmap?> TilesetImageProperty =
            AvaloniaProperty.Register<StageRenderer, Bitmap?>(nameof(TilesetImage), null);

        public Bitmap? TilesetImage
        {
            get => GetValue(TilesetImageProperty);
            set => SetValue(TilesetImageProperty, value);
        }
        #endregion

        #region TileTypesImage Styled Property
        public static readonly StyledProperty<Bitmap?> TileTypesImageProperty =
            AvaloniaProperty.Register<StageRenderer, Bitmap?>(nameof(TileTypesImage), null);

        public Bitmap? TileTypesImage
        {
            get => GetValue(TileTypesImageProperty);
            set => SetValue(TileTypesImageProperty, value);
        }
        #endregion

        //Tile stuff
        #region Map Styled Property
        public static readonly StyledProperty<Map?> MapProperty =
            AvaloniaProperty.Register<StageRenderer, Map?>(nameof(Map), null);

        public Map? Map
        {
            get => GetValue(MapProperty);
            set => SetValue(MapProperty, value);
        }
        #endregion

        #region ShowTileTypes Styled Property
        public static readonly StyledProperty<bool> ShowTileTypesProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(ShowTileTypes), false);

        public bool ShowTileTypes
        {
            get => GetValue(ShowTileTypesProperty);
            set => SetValue(ShowTileTypesProperty, value);
        }
        #endregion

        #region TileChangeQueue Styled Property
        public static readonly StyledProperty<Dictionary<int, TileChange>?> TileChangeQueueProperty =
            AvaloniaProperty.Register<StageRenderer, Dictionary<int, TileChange>?>(nameof(TileChangeQueue), null);

        public Dictionary<int, TileChange>? TileChangeQueue
        {
            get => GetValue(TileChangeQueueProperty);
            set => SetValue(TileChangeQueueProperty, value);
        }
        #endregion

        #region CurrentTileEditorAction Styled Property
        public static readonly StyledProperty<TileEditorActions> CurrentTileEditorActionProperty =
            AvaloniaProperty.Register<StageRenderer, TileEditorActions>(nameof(CurrentTileEditorAction), TileEditorActions.None);

        public TileEditorActions CurrentTileEditorAction
        {
            get => GetValue(CurrentTileEditorActionProperty);
            set => SetValue(CurrentTileEditorActionProperty, value);
        }
        #endregion

        #region TileSelection Styled Property
        public static readonly StyledProperty<TileSelection?> TileSelectionProperty =
            AvaloniaProperty.Register<StageRenderer, TileSelection?>(nameof(TileSelection), null);

        public TileSelection? TileSelection
        {
            get => GetValue(TileSelectionProperty);
            set => SetValue(TileSelectionProperty, value);
        }
        #endregion

        //Entity stuff
        #region Entities Styled Property
        public static readonly StyledProperty<List<Entity>?> EntitiesProperty =
            AvaloniaProperty.Register<StageRenderer, List<Entity>?>(nameof(Entities), null);

        public List<Entity>? Entities
        {
            get => GetValue(EntitiesProperty);
            set => SetValue(EntitiesProperty, value);
        }
        #endregion

        #region SelectedEntities Styled Property
        public static readonly StyledProperty<HashSet<Entity>?> SelectedEntitiesProperty =
            AvaloniaProperty.Register<StageRenderer, HashSet<Entity>?>(nameof(SelectedEntities), null);

        public HashSet<Entity>? SelectedEntities
        {
            get => GetValue(SelectedEntitiesProperty);
            set => SetValue(SelectedEntitiesProperty, value);
        }
        #endregion

        #region SelectedEntityOffsetX Styled Property
        public static readonly StyledProperty<int> SelectedEntityOffsetXProperty =
            AvaloniaProperty.Register<StageRenderer, int>(nameof(SelectedEntityOffsetX), 0);

        public int SelectedEntityOffsetX
        {
            get => GetValue(SelectedEntityOffsetXProperty);
            set => SetValue(SelectedEntityOffsetXProperty, value);
        }
        #endregion

        #region SelectedEntityOffsetY Styled Property
        public static readonly StyledProperty<int> SelectedEntityOffsetYProperty =
            AvaloniaProperty.Register<StageRenderer, int>(nameof(SelectedEntityOffsetY), 0);

        public int SelectedEntityOffsetY
        {
            get => GetValue(SelectedEntityOffsetYProperty);
            set => SetValue(SelectedEntityOffsetYProperty, value);
        }
        #endregion

        #region ShowEntityBoxes Styled Property
        public static readonly StyledProperty<bool> ShowEntityBoxesProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(ShowEntityBoxes), true);

        public bool ShowEntityBoxes
        {
            get => GetValue(ShowEntityBoxesProperty);
            set => SetValue(ShowEntityBoxesProperty, value);
        }
        #endregion

        #region ShowEntitySprites Styled Property
        public static readonly StyledProperty<bool> ShowEntitySpritesProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(ShowEntitySprites), true);

        public bool ShowEntitySprites
        {
            get => GetValue(ShowEntitySpritesProperty);
            set => SetValue(ShowEntitySpritesProperty, value);
        }
        #endregion

        //Player/camera stuff
        #region ShowPlayerPreview Styled Property
        public static readonly StyledProperty<bool> ShowPlayerPreviewProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(ShowPlayerPreview), false);
        public bool ShowPlayerPreview
        {
            get => GetValue(ShowPlayerPreviewProperty);
            set => SetValue(ShowPlayerPreviewProperty, value);
        }
        #endregion

        #region MyChar Styled Property
        public static readonly StyledProperty<IImage?> MyCharProperty =
            AvaloniaProperty.Register<StageRenderer, IImage?>(nameof(MyChar), null);

        public IImage? MyChar
        {
            get => GetValue(MyCharProperty);
            set => SetValue(MyCharProperty, value);
        }
        #endregion

        #region CameraX Styled Property
        public static readonly StyledProperty<int> CameraXProperty =
            AvaloniaProperty.Register<StageRenderer, int>(nameof(CameraX), 0);

        public int CameraX
        {
            get => GetValue(CameraXProperty);
            set => SetValue(CameraXProperty, value);
        }
        #endregion

        #region CameraY Styled Property
        public static readonly StyledProperty<int> CameraYProperty =
            AvaloniaProperty.Register<StageRenderer, int>(nameof(CameraY), 0);

        public int CameraY
        {
            get => GetValue(CameraYProperty);
            set => SetValue(CameraYProperty, value);
        }
        #endregion

        //flags

        #region RedrawTilesetNeeded Styled Property
        public static readonly StyledProperty<bool> RedrawTilesetNeededProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(RedrawTilesetNeeded), true, defaultBindingMode:BindingMode.TwoWay);

        public bool RedrawTilesetNeeded
        {
            get => GetValue(RedrawTilesetNeededProperty);
            set => SetValue(RedrawTilesetNeededProperty, value);
        }
        #endregion

        #region RedrawTilesNeeded Styled Property
        public static readonly StyledProperty<bool> RedrawTilesNeededProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(RedrawTilesNeeded), true, defaultBindingMode: BindingMode.TwoWay);

        public bool RedrawTilesNeeded
        {
            get => GetValue(RedrawTilesNeededProperty);
            set => SetValue(RedrawTilesNeededProperty, value);
        }
        #endregion

        #region RedrawTileTypesNeeded Styled Property
        public static readonly StyledProperty<bool> RedrawTileTypesNeededProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(RedrawTileTypesNeeded), true, defaultBindingMode:BindingMode.TwoWay);

        public bool RedrawTileTypesNeeded
        {
            get => GetValue(RedrawTileTypesNeededProperty);
            set => SetValue(RedrawTileTypesNeededProperty, value);
        }
        #endregion

        #region ShowCursor Styled Property
        public static readonly StyledProperty<bool> ShowCursorProperty =
            AvaloniaProperty.Register<StageRenderer, bool>(nameof(ShowCursor), true);

        public bool ShowCursor
        {
            get => GetValue(ShowCursorProperty);
            set => SetValue(ShowCursorProperty, value);
        }
        #endregion

        RenderTargetBitmap TilesetBuffer;

        RenderTargetBitmap? TileGraphics = null;
        RenderTargetBitmap? TileTypeGraphics = null;

        bool changeActive = false;
        RenderTargetBitmap? TileChangeGraphics = null;
        RenderTargetBitmap? TileTypeChangeGraphics = null;

        public StageRenderer()
        {
            var tilesetBufferSize = new PixelSize(TileSize * 16, TileSize * 16);
            TilesetBuffer = new RenderTargetBitmap(tilesetBufferSize);

            //tileset -> tileset, map tiles
            this.GetObservable(TilesetImageProperty).Subscribe(x =>
                RedrawTilesetNeeded = RedrawTilesNeeded = true);
            //map -> map tiles, map tile types
            this.GetObservable(MapProperty).Subscribe(x =>
                RedrawTilesNeeded = RedrawTileTypesNeeded = true);
            //both of these technically also update the tile types shown on the tileset,
            //but those are calculated on the fly anyways, so...
            //tile types -> map tile types
            this.GetObservable(TileTypesImageProperty).Subscribe(x =>
                RedrawTileTypesNeeded = true);
            //tileset attributes -> map tile types
            this.GetObservable(TilesetAttributesProperty).Subscribe(x =>
                RedrawTileTypesNeeded = true);
        }
        void DrawTileAndType(int x, int y, byte tile, DrawingContext? tileContext, DrawingContext? typeContext)
        {
            var destCoord = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
            tileContext?.DrawImage(TilesetBuffer, GetTileRect(tile, TilesetWidth), destCoord);

            if (TilesetAttributes != null)
            {
                var type = TilesetAttributes.Tiles[tile];
                typeContext?.DrawImage(TileTypesImage, GetTileRect(type, 16), destCoord);
            }
        }

        void redrawTileset()
        {
            using (var c = new DrawingContext(TilesetBuffer.CreateDrawingContext(null)))
            {
                c.PlatformImpl.Clear(Colors.Black);
                if(TilesetImage != null)
                {
                    var bufferSize = new Rect(0, 0, TileSize * 16, TileSize * 16);
                    var imageSize = new Rect(0, 0, TilesetImage.Size.Width, TilesetImage.Size.Height);
                    c.DrawImage(TilesetImage, imageSize, bufferSize.Intersect(imageSize));
                }
            }
        }
        
        
        void RedrawMap(bool tiles, bool types)
        {
            if (Map == null)
            {
                TileGraphics?.Dispose();
                TileTypeGraphics?.Dispose();
                TileChangeGraphics?.Dispose();
                TileTypeChangeGraphics?.Dispose();
                TileGraphics = TileTypeGraphics = TileChangeGraphics = TileTypeChangeGraphics
                    = null;
            }
            else
            {
                var size = new PixelSize(Map.Width * TileSize, Map.Height * TileSize);
                DrawingContext? tileDrawingContext = null, tileTypeDrawingContext = null;

                if (tiles)
                {
                    TileGraphics?.Dispose();
                    TileGraphics = new RenderTargetBitmap(size);
                    TileChangeGraphics?.Dispose();
                    TileChangeGraphics = new RenderTargetBitmap(size);

                    tileDrawingContext
                    = new DrawingContext(TileGraphics.CreateDrawingContext(null));
                }
                if (types)
                {
                    TileTypeGraphics?.Dispose();
                    TileTypeGraphics = new RenderTargetBitmap(size);

                    TileTypeChangeGraphics?.Dispose();
                    TileTypeChangeGraphics = new RenderTargetBitmap(size);
                    tileTypeDrawingContext
                    = new DrawingContext(TileTypeGraphics.CreateDrawingContext(null));
                } 

                for (int y = 0; y < Map.Height; y++)
                {
                    for (int x = 0; x < Map.Width; x++)
                    {
                        DrawTileAndType(x, y, (byte)Map.Tiles[(y * Map.Width) + x]!, tileDrawingContext, tileTypeDrawingContext);
                    }
                }

                tileDrawingContext?.Dispose();
                tileTypeDrawingContext?.Dispose();
            }
        }
        PixelSize MapSize => new PixelSize((Map?.Width ?? 0) * TileSize, (Map?.Height ?? 0) * TileSize);
        Rect MapRect => new Rect(new Point(0, 0), MapSize.ToSize(1));
        
        public override void Render(DrawingContext context)
        {
            //this is the correct way to use my education
            using var b = context.PushPostTransform(new Matrix(Scale, 0,
                                                               0, Scale,
                                                               0, 0));

            var playerOK = ShowPlayerPreview && MyChar != null;
            var backgroundOK = ShowBackground && Background != null;
            if (backgroundOK)
            {
                switch (BackgroundType)
                {
                    //don't move
                    case 0:
                        break;
                    //move slow (1/2 speed)
                    case 1 when playerOK:
                        break;
                    //move fast (same speed)
                    case 2 when playerOK:
                        break;
                    //water
                    case 3:
                    //black/no display
                    case 4:
                        break;
                    //ironhead
                    case 5:
                        break;
                    //bkfog/moon
                    case 6 when playerOK:
                    case 7 when playerOK:
                        break;
                }
            }

            if (RedrawTilesetNeeded)
            {
                redrawTileset();
                RedrawTilesetNeeded = false;
            }
            //drawing the map
            if(RedrawTilesNeeded || RedrawTileTypesNeeded)
            {
                RedrawMap(RedrawTileTypesNeeded, RedrawTileTypesNeeded);
                RedrawTilesNeeded = RedrawTileTypesNeeded = false;
            }
            if (TileGraphics != null)
            {
                context.DrawImage(TileGraphics, MapRect);
            }
            if (ShowTileTypes && TileTypeGraphics != null)
            {
                context.DrawImage(TileTypeGraphics, MapRect);
            }

            //drawing any active change
            switch (CurrentTileEditorAction)
            {
                //most changes fall under here
                case TileEditorActions.Draw:
                case TileEditorActions.Fill:
                case TileEditorActions.Replace:
                    if (TileChangeQueue?.Count > 0)
                    {
                        changeActive = true;
                        using (var tcg = new DrawingContext(TileChangeGraphics!.CreateDrawingContext(null)))
                        using (var ttcg = new DrawingContext(TileTypeChangeGraphics!.CreateDrawingContext(null)))
                        {
                            foreach (var change in TileChangeQueue)
                            {
                                DrawTileAndType(change.Key % Map.Width, change.Key / Map.Width, change.Value.New, tcg, ttcg);
                            }
                        }
                    }
                    break;
                //rectangles are a special case
                case TileEditorActions.Rectangle:
                    if (TileSelection != null)
                    {
                        changeActive = true;
                        var rect = PointsToRect(SelectionStartX, SelectionStartY, SelectionEndX, SelectionEndY);
                        using (var tcg = new DrawingContext(TileChangeGraphics!.CreateDrawingContext(null)))
                        using (var ttcg = new DrawingContext(TileTypeChangeGraphics!.CreateDrawingContext(null)))
                        {
                            //TODO more advanced rectangle calculation?
                            tcg.PlatformImpl.Clear(Colors.Transparent);
                            ttcg.PlatformImpl.Clear(Colors.Transparent);
                            for (int y = (int)rect.Top; y <= (int)rect.Bottom; y++)
                            {
                                for (int x = (int)rect.Left; x <= (int)rect.Right; x++)
                                {
                                    var thist = TileSelection.CoordsToTile(x - SelectionStartX, y - SelectionStartY);
                                    if (thist != null)
                                    {
                                        DrawTileAndType(x, y, (byte)thist, tcg, ttcg);
                                    }
                                }
                            }
                        }
                    }
                    break;
                //anything else means we need to commit/clear the changes
                default:
                    if (changeActive)
                    {
                        using (var tg = new DrawingContext(TileGraphics!.CreateDrawingContext(null)))
                        using (var ttg = new DrawingContext(TileTypeGraphics!.CreateDrawingContext(null)))
                        {
                            tg.DrawImage(TileChangeGraphics, MapRect);
                            ttg.DrawImage(TileTypeChangeGraphics, MapRect);
                        }
                        using (var tcg = new DrawingContext(TileChangeGraphics.CreateDrawingContext(null)))
                        using (var ttcg = new DrawingContext(TileTypeChangeGraphics.CreateDrawingContext(null)))
                        {
                            tcg.PlatformImpl.Clear(Colors.Transparent);
                            ttcg.PlatformImpl.Clear(Colors.Transparent);
                        }
                        changeActive = false;
                    }
                    break;
            }
            //draw any active change
            if (changeActive)
            {
                context.DrawImage(TileChangeGraphics, MapRect);
                if (ShowTileTypes)
                {
                    context.DrawImage(TileTypeChangeGraphics, MapRect);
                }
            }

            //draw the entities if there are any, and we're supposed to display them
            if (Entities?.Count > 0 && (ShowEntitySprites || ShowEntityBoxes))
            {
                void DrawEntityBox(int x, int y, IBrush brush)
                {
                    context.DrawRectangle(new Pen(brush),
                                new Rect(
                                    x * TileSize,
                                    y * TileSize,
                                    TileSize - 1,
                                    TileSize - 1
                                ));
                }
                void DrawEntitySprite(Entity entity, int offsetX = 0, int offsetY = 0)
                {

                }

                if(SelectedEntities?.Count > 0)
                {
                    //draw all normal entities
                    if (ShowEntitySprites)
                    {
                        foreach (var entity in Entities)
                        {
                            if (!SelectedEntities.Contains(entity))
                                DrawEntitySprite(entity);
                        }
                    }
                    if (ShowEntityBoxes)
                    {
                        foreach(var entity in Entities)
                        {
                            if(!SelectedEntities.Contains(entity))
                                DrawEntityBox(entity.X, entity.Y, Brushes.Green);
                        }
                    }
                    //then draw the selected ones
                    if (ShowEntitySprites)
                    {
                        foreach (var entity in SelectedEntities)
                        {
                            DrawEntitySprite(entity, SelectedEntityOffsetX, SelectedEntityOffsetY);
                        }
                    }
                    //selected entity boxes are always shown...?
                    //if (ShowEntityBoxes)
                    {
                        foreach (var entity in SelectedEntities)
                        {
                            DrawEntityBox(entity.X + SelectedEntityOffsetX, entity.Y + SelectedEntityOffsetY, Brushes.Teal); //TODO custom colors
                        }
                    }
                }
                else
                {
                    if (ShowEntitySprites)
                    {
                        foreach (var entity in Entities)
                        {
                            DrawEntitySprite(entity);
                        }
                    }
                    if (ShowEntityBoxes)
                    {
                        foreach (var entity in Entities)
                        {
                            DrawEntityBox(entity.X, entity.Y, Brushes.Green); //TODO custom colors
                        }
                    }
                }
            }

            if (playerOK)
            {
                context.DrawImage(MyChar, Bounds);
                
            }

            if (ShowCursor)
            {
                //draw the selection
                var selectionRect = PointsToRect(
                    SelectionStartX * TileSize,
                    SelectionStartY * TileSize,
                    SelectionEndX * TileSize,
                    SelectionEndY * TileSize,
                    TileSize - 1);

                if (MapRect.Intersects(selectionRect))
                {
                    context.DrawRectangle(new Pen(Brushes.Gray),
                        //Drawing the intersection here to prevent
                        //a giant selection rectangle escaping the bounds of the map
                        MapRect.Intersect(selectionRect));
                }
            }

            Width = MapRect.Width * Scale;
            Height = MapRect.Height * Scale;
        }
    }
}
