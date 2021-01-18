using LayeredPictureBox;
using System;
using System.Drawing;
using static CaveStoryEditor.SharedGraphics;

namespace CaveStoryEditor
{
    partial class FormStageEditor
    {
        readonly Layer<Image> backgroundPreview, baseMap, mapTileTypes, mapTileGrid,
            entityIcons, entityBoxes, selectedEntityIcons, selectedEntityBoxes,
            screenPreview, playerPreview, mouseOverlay;

        #region grid

        void RedrawGrid()
        {
            using(var g = Graphics.FromImage(mapTileGrid.Image))
            {
                g.Clear(Color.Transparent);

                var width = map.Width * parentMod.TileSize;
                var height = map.Height * parentMod.TileSize;

                //vertical lines
                for (int i = 0; i < map.Width; i++)
                {
                    var x = (i * parentMod.TileSize) - 1;
                    g.DrawLine(new Pen(UI.Default.GridColor), x, 0, x, height);
                }
                //horizontal lines
                for (int i = 1; i < map.Height; i++)
                {
                    var y = (i * parentMod.TileSize) - 1;
                    g.DrawLine(new Pen(UI.Default.GridColor), 0, y, width, y);
                }
            }
        }

#endregion

#region entity

        void ResizeAllEntityLayers()
        {
            entityIcons.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            entityBoxes.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            selectedEntityIcons.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            selectedEntityBoxes.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
        }

        void RedrawAllEntityLayers()
        {
            DrawEntityIcons();
            DrawEntityBoxes();
            DrawSelectedEntityIcons();
            DrawSelectedEntityBoxes();
        }

        void DrawEntityIcons()
        {
            using (var g = Graphics.FromImage(entityIcons.Image))
            {
                g.Clear(Color.Transparent);

                foreach(var e in Entities)
                {
                    if (!selectedEntities.Contains(e) &&
                        Cache.TryGetSprite(e.Type, out Image img, tilesetSurfaceSource, backgroundSurfaceSource, spritesheet1SurfaceSource, spritesheet2SurfaceSource))
                    {
                        var vbox = parentMod.NPCTable[e.Type].Viewbox;

                        var half = (parentMod.TileSize / 2);
                        var x = e.X * parentMod.TileSize + half - (((e.Flag | (short)CaveStoryModdingFramework.Entities.EntityFlags.SpawnInOtherDirection) != 0) ? vbox.RightOffset : vbox.LeftOffset);
                        var y = e.Y * parentMod.TileSize + half - vbox.YOffset;
                        g.DrawImage(img, x, y, img.Width, img.Height);
                    }
                }
            }
        }
        void DrawSelectedEntityIcons()
        {
            using (var g = Graphics.FromImage(selectedEntityIcons.Image))
            {
                g.Clear(Color.Transparent);

                foreach (var e in selectedEntities)
                {
                    if (Cache.TryGetSprite(e.Type, out Image img, tilesetSurfaceSource, backgroundSurfaceSource, spritesheet1SurfaceSource, spritesheet2SurfaceSource))
                    {
                        var vbox = parentMod.NPCTable[e.Type].Viewbox;

                        var half = (parentMod.TileSize / 2);
                        var x = e.X * parentMod.TileSize + half - vbox.LeftOffset;
                        var y = e.Y * parentMod.TileSize + half - vbox.YOffset;
                        g.DrawImage(img, x, y, img.Width, img.Height);
                    }
                }
            }
        }
        void DrawEntityBoxes()
        {
            using (var g = Graphics.FromImage(entityBoxes.Image))
            {
                g.Clear(Color.Transparent);

                //used to not draw selected entities, but that broke inserting new entities
                foreach (var e in Entities)
                {
                    if (!selectedEntities.Contains(e))
                    {
                        var y = e.Y * parentMod.TileSize;
                        var x = e.X * parentMod.TileSize;

                        g.DrawRectangle(new Pen(UI.Default.EntityBoxColor), x, y, parentMod.TileSize - 1, parentMod.TileSize - 1);
                    }
                }
            }
        }
        void DrawSelectedEntityBoxes()
        {
            using (var g = Graphics.FromImage(selectedEntityBoxes.Image))
            {
                g.Clear(Color.Transparent);

                //don't need to draw selected entities on this layer
                foreach (var e in selectedEntities)
                {
                    var y = e.Y * parentMod.TileSize;
                    var x = e.X * parentMod.TileSize;

                    g.DrawRectangle(new Pen(UI.Default.SelectedEntityBoxColor), x, y, parentMod.TileSize - 1, parentMod.TileSize - 1);
                }
            }
        }
#endregion

#region screen preview
        private void UpdateScreenPreviewLocation(int h, int v)
        {
            hScreenPreviewScrollBar.Value = h;
            vScreenPreviewScrollBar.Value = v;
            screenPreview.Location = new Point(h, v);
        }
        private void InitScreenPreview()
        {
            var sp = new Bitmap(parentMod.ScreenWidth, parentMod.ScreenHeight);
            using (var g = Graphics.FromImage(sp))
            {
                g.DrawRectangle(new Pen(UI.Default.ScreenPreviewColor), 0, 0, sp.Width - 1, sp.Height - 1);
            }
            screenPreview.Image = sp;
        }
#endregion

#region mouse stuff

        /// <summary>
        /// Sets the mouse back to the right size for whatever edit mode we're in
        /// </summary>
        void RestoreMouseSize()
        {
            switch (editMode)
            {
                case EditModes.Tile:
                    mouseOverlay.Image = MakeMouseImage(SelectedTiles.Width * parentMod.TileSize,
                           SelectedTiles.Height * parentMod.TileSize, UI.Default.CursorColor);
                    break;
                case EditModes.Entity:
                    mouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.CursorColor);
                    break;
            }            
        }

        void MoveMouse(Point gridPosition, Point offset)
        {
            mouseOverlay.Location = new Point((gridPosition.X - offset.X)*parentMod.TileSize,
                                              (gridPosition.Y - offset.Y)*parentMod.TileSize);
        }

#endregion
    }
}
