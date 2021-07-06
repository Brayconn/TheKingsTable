using LayeredPictureBox;
using System;
using System.Drawing;

namespace CaveStoryEditor
{
    static class SharedGraphics
    {
        /// <summary>
        /// Generates a rectangle from two arbitrary points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Rectangle GetRect(Point p1, Point p2)
        {
            int x, y, width, height;
            x = Math.Min(p1.X, p2.X);
            y = Math.Min(p1.Y, p2.Y);
            width = Math.Max(p1.X, p2.X) - x;
            height = Math.Max(p1.Y, p2.Y) - y;
            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Returns an image containing the mouse cursor with a given size
        /// </summary>
        /// <param name="width">Width of the image in pixels</param>
        /// <param name="height">Height of the image in pixels</param>
        /// <returns>The image containing the mouse</returns>
        public static Image MakeMouseImage(int width, int height, Color c)
        {
            var img = new Bitmap(width, height);
            using (var g = Graphics.FromImage(img))
                g.DrawRectangle(new Pen(c), 0, 0, img.Width - 1, img.Height - 1);
            return img;
        }

        public static void UpdateMouseMarquee<T>(Point p1, Point p2, Layer<T> layer, int tileSize, Color color) where T : Image
        {
            UpdateMouseMarquee(GetRect(p1, p2), layer, tileSize, color);
        }
        public static void UpdateMouseMarquee<T>(Rectangle rect, Layer<T> layer, int tileSize, Color color) where T : Image
        {
            layer.Image = (T)MakeMouseImage((rect.Width+1)*tileSize, (rect.Height+1) * tileSize, color);
            layer.Location = new Point(rect.X*tileSize, rect.Y*tileSize);
        }
    }
}
