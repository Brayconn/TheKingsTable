using System.Drawing;
using System.Windows.Forms;
using LayeredPictureBox;
using static PixelModdingFramework.Rendering;
using static CaveStoryEditor.SharedGraphics;
using System;

namespace CaveStoryEditor
{
    partial class FormStageEditor
    {
        readonly Layer<Image> baseTileset;
        readonly Layer<Image> tilesetTileTypes;
        readonly Layer<Image> tilesetMouseOverlay;

        /// <summary>
        /// Resets the tileset buffer, and draws the given image on it
        /// </summary>
        /// <param name="t"></param>
        void InitTilesetAndTileTypes(Image t)
        {
            tilesetLayeredPictureBox.UnlockCanvasSize();

            var tileset = new Bitmap(TilesetWidth * parentMod.TileSize, TilesetHeight * parentMod.TileSize);
            using (Graphics g = Graphics.FromImage(tileset))
            {
                g.Clear(parentMod.TransparentColor);
                g.DrawImage(t, 0, 0, t.Width, t.Height);
            }
            tileset.MakeTransparent(parentMod.TransparentColor);
            
            var tiletypes = new Bitmap(TilesetWidth * parentMod.TileSize, TilesetHeight * parentMod.TileSize);
            RenderTiles(tiletypes, attributes, tileTypes, parentMod.TileSize);

            baseTileset.Image = tileset;
            tilesetTileTypes.Image = tiletypes;
            tilesetMouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.SelectedTileColor);

            tilesetLayeredPictureBox.LockCanvasSize();
        }

        Point attributesStartPos = new Point(-1, -1);
        Point attributesLastPos = new Point(-1, -1);
        private void tilesetLayeredPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tilesetMouseOverlay.Shown = true;
                attributesStartPos = attributesLastPos = GetMousePointOnTileset(e.Location);
                UpdateMouseMarquee(attributesStartPos, attributesLastPos, tilesetMouseOverlay, parentMod.TileSize, UI.Default.SelectedTileColor);
            }
        }

        private void tilesetLayeredPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var p = GetMousePointOnTileset(e.Location);
                //TODO could do with making a Clamp method
                p.X = Math.Max(0, Math.Min(p.X, TilesetWidth));
                p.Y = Math.Max(0, Math.Min(p.Y, TilesetHeight));
                //if we're still on the same grid space, stop
                if (p == attributesLastPos)
                    return;

                UpdateMouseMarquee(attributesStartPos, attributesLastPos = p, tilesetMouseOverlay, parentMod.TileSize, UI.Default.SelectedTileColor);
            }
        }

        private void tilesetLayeredPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                SelectTilesFromTileset(attributesStartPos, attributesLastPos);
                RestoreMouseSize();
            }
        }

        private void tilesetLayeredPictureBox_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}
