using CaveStoryModdingFramework;
using LayeredPictureBox;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class ViewboxPreview : UserControl
    {
        //viewbox layers
        readonly Layer<Image> LeftOffsetTriangle, LeftOffsetLine,
                     RightOffsetTriangle, RightOffsetLine,
                     YOffsetTriangle, YOffsetLine,
                     ViewCenter;

        const int MinBoxSize = 8;
        static Bitmap MakeRightTriangle(Color c, float rotate = 0)
        {
            var tri = new Bitmap(MinBoxSize / 2, MinBoxSize / 2);
            using (var g = Graphics.FromImage(tri))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                if (rotate != 0)
                {
                    var w = (float)tri.Width / 2;
                    var h = (float)tri.Height / 2;
                    g.TranslateTransform(w, h);
                    g.RotateTransform(rotate);
                    g.TranslateTransform(-w, -h);
                }
                g.FillPolygon(new SolidBrush(c), new[]
                {
                    //top left
                    new Point(0,-1),
                    //bottom left
                    new Point(0,tri.Height),
                    //bottom right
                    new Point(tri.Width,tri.Height),
                });
            }
            return new Bitmap(tri);
        }
        static Bitmap MakeLine(int width, int height, Color c)
        {
            if (width <= 0 || height <= 0)
                return null;
            var l = new Bitmap(width, height);
            using (var g = Graphics.FromImage(l))
            {
                g.DrawLine(new Pen(c), 0, 0, width - 1, height - 1);
            }
            return new Bitmap(l);
        }

        public ViewboxPreview()
        {
            InitializeComponent();

            viewboxLayeredPictureBox.CreateLayer(out YOffsetTriangle);
            viewboxLayeredPictureBox.CreateLayer(out YOffsetLine);
            viewboxLayeredPictureBox.CreateLayer(out LeftOffsetTriangle);
            viewboxLayeredPictureBox.CreateLayer(out LeftOffsetLine);
            viewboxLayeredPictureBox.CreateLayer(out RightOffsetTriangle);
            viewboxLayeredPictureBox.CreateLayer(out RightOffsetLine);
            viewboxLayeredPictureBox.CreateLayer(out ViewCenter);

            var pix = new Bitmap(1, 1);
            pix.SetPixel(0, 0, Color.White);
            ViewCenter.Image = pix;

            YOffsetTriangle.Image = MakeRightTriangle(Color.Yellow);
            LeftOffsetTriangle.Image = MakeRightTriangle(Color.Green, -180);
            RightOffsetTriangle.Image = MakeRightTriangle(Color.Red, 90);
        }
        public void DrawViewbox(NPCViewRect rect, string prop = null)
        {
            DrawViewbox(rect.LeftOffset, rect.YOffset, rect.RightOffset, prop);
        }
        public void DrawViewbox(BulletViewRect rect, string prop = null)
        {
            DrawViewbox(rect.LeftOffset, rect.YOffset, rect.RightOffset, prop);
        }
        public void DrawViewbox(int leftOffset, int yOffset, int rightOffset, string prop = null)
        {
            int Clamp(int value) => Math.Max(value, MinBoxSize);

            //Display location of the NPC's actual position, clamped to MinBoxSize
            var viewX = Clamp(Math.Max(leftOffset, rightOffset));
            var viewY = Clamp(yOffset);

            //functions to update the images for all the lines
            void UpdateYImage() => YOffsetLine.Image = MakeLine((viewX + 1) * 2, 1, Color.Yellow);
            void UpdateLImage() => LeftOffsetLine.Image = MakeLine(1, (viewY + 1) * 2, Color.Green);
            void UpdateRImage() => RightOffsetLine.Image = MakeLine(1, (viewY + 1) * 2, Color.Red);

            //functions to update the images locations
            void UpdateYLocation()
            {
                YOffsetLine.Location = new Point(0, ViewCenter.Location.Y - yOffset);
                YOffsetTriangle.Location = new Point(0, YOffsetLine.Location.Y - YOffsetTriangle.Image.Height + 1);
            }
            void UpdateLLocation()
            {
                LeftOffsetLine.Location = new Point(ViewCenter.Location.X - leftOffset, 0);
                LeftOffsetTriangle.Location = new Point(LeftOffsetLine.Location.X - LeftOffsetTriangle.Image.Width + 1, 0);
            }
            void UpdateRLocation()
            {
                RightOffsetLine.Location = new Point(ViewCenter.Location.X - rightOffset, 0);
                RightOffsetTriangle.Location = RightOffsetLine.Location;
            }

            //setting the NPC's center to 1.5x its value leaves room on the left so it looks pretty
            ViewCenter.Location = new Point(viewX + viewX / 2, viewY + viewY / 2);

            //reset all images
            if (prop == null)
            {
                UpdateLImage();
                UpdateRImage();
                UpdateYImage();
            }
            else
            {
                switch (prop)
                {
                    case nameof(NPCViewRect.YOffset):
                    //case nameof(BulletViewRect.YOffset):
                        UpdateLImage();
                        UpdateRImage();
                        break;
                    //only reset the Y image if the line being edited is the bigger one
                    case nameof(NPCViewRect.LeftOffset):
                    //case nameof(BulletViewRect.LeftOffset):
                    case nameof(NPCViewRect.RightOffset):
                    //case nameof(BulletViewRect.RightOffset):
                        if (YOffsetLine.Image.Width != (viewX + 1) * 2)
                            UpdateYImage();
                        break;
                }
            }
            //always update line locations
            UpdateYLocation();
            UpdateLLocation();
            UpdateRLocation();
            UpdateBoxLocation(this, viewboxLayeredPictureBox);
        }
        static void UpdateBoxLocation(ScrollableControl parent, Control child)
        {
            //TODO figure out why the hecc auto scroll doesn't work
            child.Location = new Point(parent.Width / 2 - child.Width / 2, parent.Height / 2 - child.Height / 2);
        }

        private void viewboxContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            UpdateBoxLocation(this, viewboxLayeredPictureBox);
        }
    }
}
