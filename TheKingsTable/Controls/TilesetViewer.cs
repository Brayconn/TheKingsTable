using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using CaveStoryModdingFramework.Editors;
using System;
using System.Linq;

namespace TheKingsTable.Controls
{
    public class TilesetViewer : TileMouseHelper, IStyleable
    {
        Type IStyleable.StyleKey => typeof(Control);

        static TilesetViewer()
        {
            AffectsRender<TilesetViewer>(new AvaloniaProperty[] {
                TilesetImageProperty,
                TileTypesImageProperty,
                TilesetAttributesProperty,
                ShowTileTypesProperty,
                SelectionProperty
            }.Concat(Properties).ToArray());
        }

        #region TilesetImage Styled Property
        public static readonly StyledProperty<Bitmap?> TilesetImageProperty =
            AvaloniaProperty.Register<TilesetViewer, Bitmap?>(nameof(TilesetImage), null);

        public Bitmap? TilesetImage
        {
            get => GetValue(TilesetImageProperty);
            set => SetValue(TilesetImageProperty, value);
        }
        #endregion

        #region TileTypesImage Styled Property
        public static readonly StyledProperty<Bitmap?> TileTypesImageProperty =
            AvaloniaProperty.Register<TilesetViewer, Bitmap?>(nameof(TileTypesImage), null);

        public Bitmap? TileTypesImage
        {
            get => GetValue(TileTypesImageProperty);
            set => SetValue(TileTypesImageProperty, value);
        }
        #endregion

        #region TilesetAttributes Styled Property
        public static readonly StyledProperty<CaveStoryModdingFramework.Maps.Attribute?> TilesetAttributesProperty =
            AvaloniaProperty.Register<TilesetViewer, CaveStoryModdingFramework.Maps.Attribute?>(nameof(TilesetAttributes), null);

        public CaveStoryModdingFramework.Maps.Attribute? TilesetAttributes
        {
            get => GetValue(TilesetAttributesProperty);
            set => SetValue(TilesetAttributesProperty, value);
        }
        #endregion

        #region ShowTileTypes Styled Property
        public static readonly StyledProperty<bool> ShowTileTypesProperty =
            AvaloniaProperty.Register<TilesetViewer, bool>(nameof(ShowTileTypes), false);

        public bool ShowTileTypes
        {
            get => GetValue(ShowTileTypesProperty);
            set => SetValue(ShowTileTypesProperty, value);
        }
        #endregion

        #region Selection Styled Property
        public static readonly StyledProperty<TileSelection> SelectionProperty =
            AvaloniaProperty.Register<TilesetViewer, TileSelection>(nameof(Selection), null);

        public TileSelection Selection
        {
            get => GetValue(SelectionProperty);
            set => SetValue(SelectionProperty, value);
        }
        #endregion

        PixelSize BufferSize => new PixelSize(TileSize * 16, TileSize * 16);
        Rect MainRect => new Rect(new Point(0, 0), BufferSize.ToSize(1));

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(Brushes.Black, MainRect);
            if (TilesetImage != null)
            {
                var src = new Rect(TilesetImage.Size);
                context.DrawImage(TilesetImage, src, MainRect.Intersect(src));
            }
            if (TilesetAttributes != null && TileTypesImage != null && ShowTileTypes)
            {
                for(int i = 0; i < TilesetAttributes.Tiles.Count; i++)
                    context.DrawImage(TileTypesImage,
                        GetTileRect(TilesetAttributes.Tiles[i],16)
                        ,GetTileRect((byte)i,16));
            }
            if(Selection != null
                && Selection.Contents.Tiles.Count > 0
                && Selection.Contents.Tiles[0] != null)
            {
                //a valid selection to display in the tileset should be of the form
                //n,        ..., n+x
                //n+(y*16), ..., n+(y*16)+x
                //so first we find n...
                var start = (byte)Selection.Contents.Tiles[0]!;
                int i = 0;
                for(int y = 0; y < Selection.Contents.Height; y++)
                {
                    for(int x = 0; x < Selection.Contents.Width; x++)
                    {
                        //...then we check it meets the condition
                        if (Selection.Contents.Tiles[i++] != start + (16 * y) + x)
                            return;
                    }
                }
                //if we made it to here, we can draw the selection box
                var sx = start % 16;
                var sy = start / 16;
                context.DrawRectangle(new Pen(Brushes.Gray),
                    new Rect(sx * TileSize, sy * TileSize,
                    (Selection.Contents.Width * TileSize) - 1,
                    (Selection.Contents.Height * TileSize) - 1
                    ));
            }
        }
    }
}
