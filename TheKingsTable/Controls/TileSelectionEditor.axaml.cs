using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using CaveStoryModdingFramework.Editors;
using CaveStoryModdingFramework.Maps;
using static TheKingsTable.Utilities;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_SelectionViewer, Type = typeof(StageRenderer))]
    [TemplatePart(Name = PART_TilesetViewer, Type = typeof(TilesetViewer))]
    public partial class TileSelectionEditor : TemplatedControl
    {
        public const string PART_SelectionViewer = nameof(PART_SelectionViewer);
        public const string PART_TilesetViewer = nameof(PART_TilesetViewer);

        #region ShowTileTypes Styled Property
        public static readonly StyledProperty<bool> ShowTileTypesProperty =
            AvaloniaProperty.Register<TileSelectionEditor, bool>(nameof(ShowTileTypes), false);

        public bool ShowTileTypes
        {
            get => GetValue(ShowTileTypesProperty);
            set => SetValue(ShowTileTypesProperty, value);
        }
        #endregion

        #region TheTileSelection Styled Property
        public static readonly StyledProperty<TileSelection?> TheTileSelectionProperty =
            AvaloniaProperty.Register<TileSelectionEditor, TileSelection?>(nameof(TheTileSelection), null);

        public TileSelection? TheTileSelection
        {
            get => GetValue(TheTileSelectionProperty);
            set => SetValue(TheTileSelectionProperty, value);
        }
        #endregion

        #region TileTypesImage Styled Property
        public static readonly StyledProperty<Bitmap?> TileTypesImageProperty =
            AvaloniaProperty.Register<TileSelectionEditor, Bitmap?>(nameof(TileTypesImage), null);

        public Bitmap? TileTypesImage
        {
            get => GetValue(TileTypesImageProperty);
            set => SetValue(TileTypesImageProperty, value);
        }
        #endregion

        #region TilesetAttributes Styled Property
        public static readonly StyledProperty<Attribute?> TilesetAttributesProperty =
            AvaloniaProperty.Register<TileSelectionEditor, Attribute?>(nameof(TilesetAttributes), null);

        public Attribute? TilesetAttributes
        {
            get => GetValue(TilesetAttributesProperty);
            set => SetValue(TilesetAttributesProperty, value);
        }
        #endregion

        #region TilesetImage Styled Property
        public static readonly StyledProperty<Bitmap?> TilesetImageProperty =
            AvaloniaProperty.Register<TileSelectionEditor, Bitmap?>(nameof(TilesetImage), null);

        public Bitmap? TilesetImage
        {
            get => GetValue(TilesetImageProperty);
            set => SetValue(TilesetImageProperty, value);
        }
        #endregion

        StageRenderer SelectionViewer;
        TilesetViewer TilesetView;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            SelectionViewer = e.NameScope.Find<StageRenderer>(PART_SelectionViewer);
            TilesetView = e.NameScope.Find<TilesetViewer>(PART_TilesetViewer);

            SelectionViewer.PointerPressedTile += SelectionViewer_PointerPressedTile;

            TilesetView.PointerPressedTile += TilesetView_PointerPressedTile;
            TilesetView.PointerMovedTile += TilesetView_PointerMovedTile;
            TilesetView.PointerReleasedTile += TilesetView_PointerReleasedTile;
        }

        private void SelectionViewer_PointerPressedTile(object? sender, TileEventArgs e)
        {
            if (TheTileSelection != null && e.Pressed == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
            {
                TheTileSelection.CursorX = e.X;
                TheTileSelection.CursorY = e.Y;
            }
        }

        bool selectionActive = false;
        private void TilesetView_PointerPressedTile(object? sender, TileEventArgs e)
        {
            if (e.Pressed == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
            {
                selectionActive = true;
                TilesetView.SelectionStartX = TilesetView.SelectionEndX = e.X;
                TilesetView.SelectionStartY = TilesetView.SelectionEndY = e.Y;
            }
        }

        private void TilesetView_PointerMovedTile(object? sender, TileEventArgs e)
        {
            if (selectionActive)
            {
                TilesetView.SelectionEndX = e.X;
                TilesetView.SelectionEndY = e.Y;
            }
        }

        private void TilesetView_PointerReleasedTile(object? sender, TileEventArgs e)
        {
            if (selectionActive)
            {
                var r = PointsToRect(TilesetView.SelectionStartX, TilesetView.SelectionStartY, TilesetView.SelectionEndX, TilesetView.SelectionEndY);
                var newSel = new Map((short)(r.Width+1), (short)(r.Height+1));
                int i = 0;
                for (int y = (int)r.Top; y < r.Bottom + 1; y++)
                {
                    for (int x = (int)r.Left; x < r.Right + 1; x++)
                    {
                        newSel.Tiles[i++] = (byte)((y * 16) + x);
                    }
                }
                /*
                TilesetView.Selection = new TileSelection(
                    TilesetView.SelectionEndX - (int)r.Left,
                    TilesetView.SelectionEndY - (int)r.Top,
                    newSel);
                /*/
                TilesetView.Selection.Contents = newSel;
                TilesetView.Selection.CursorX = TilesetView.SelectionEndX - (int)r.Left;
                TilesetView.Selection.CursorY = TilesetView.SelectionEndY - (int)r.Top;
                //*/
                selectionActive = false;
            }
        }
    }
}