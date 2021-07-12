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

            InitTriangles(lastMirror);
        }

        bool lastMirror = false;
        void InitTriangles(bool mirror)
        {
            YOffsetTriangle.Image = MakeRightTriangle(Color.Yellow, mirror ? -180 : 0);
            LeftOffsetTriangle.Image = MakeRightTriangle(Color.Green, mirror ? 0 : -180);
            RightOffsetTriangle.Image = MakeRightTriangle(Color.Red, mirror ? 90 : 90);

            lastMirror = mirror;
        }

        public void DrawViewbox(NPCViewRect rect, bool mirror, string prop = null)
        {
            DrawViewbox(rect.LeftOffset, rect.YOffset, rect.RightOffset, mirror, prop);
        }
        public void DrawViewbox(BulletViewRect rect, bool mirror, string prop = null)
        {
            DrawViewbox(rect.LeftOffset, rect.YOffset, rect.RightOffset, mirror, prop);
        }
        public void DrawViewbox(int leftOffset, int yOffset, int rightOffset, bool mirror, string prop = null)
        {
            int Clamp(int value) => Math.Max(value, MinBoxSize);

            //Display location of the NPC's actual position, clamped to MinBoxSize
            var centerX = Clamp(Math.Max(leftOffset, rightOffset));
            var centerY = Clamp(yOffset);

            //setting the center location to 1.5x its value leaves room on the top/left so it looks pretty
            var centerXD = centerX + centerX / 2;
            var centerYD = centerY + centerY / 2;

            var LRLineWidth = 1;
            var LRLineHeight = (centerY + 1) * 2;

            var LLineX = centerXD - leftOffset;
            var LLineY = 0;

            var RLineX = centerXD - rightOffset;
            var RLineY = 0;

            var YLineHeight = 1;
            var YLineWidth = (centerX + 1) * 2;

            var YLineX = 0;
            var YLineY = centerYD - yOffset;

            if (mirror)
            {
                (centerX, centerY) = (centerY, centerX);
                (centerXD, centerYD) = (centerYD, centerXD);

                (LRLineWidth, LRLineHeight) = (LRLineHeight, LRLineWidth);
                (LLineX, LLineY) = (LLineY, LLineX);
                (RLineX, RLineY) = (RLineY, RLineX);

                (YLineWidth, YLineHeight) = (YLineHeight, YLineWidth);
                (YLineX, YLineY) = (YLineY, YLineX);
            }
            if (mirror != lastMirror)
                InitTriangles(mirror);
            
            ViewCenter.Location = new Point(centerXD, centerYD);

            //functions to update the images for all the lines
            void UpdateYImage()
            {
                YOffsetLine.Image = MakeLine(YLineWidth, YLineHeight, Color.Yellow); 
            }
            void UpdateLImage()
            {
                LeftOffsetLine.Image = MakeLine(LRLineWidth, LRLineHeight, Color.Green);
            }
            void UpdateRImage()
            {
                RightOffsetLine.Image = MakeLine(LRLineWidth, LRLineHeight, Color.Red);
            }

            //functions to update the images locations
            void UpdateYLocation()
            {
                YOffsetLine.Location = new Point(YLineX, YLineY);
                if(mirror)
                {
                    YLineX -= YOffsetTriangle.Image.Width;
                }
                else
                {
                    YLineY -= YOffsetTriangle.Image.Height;
                }
                YOffsetTriangle.Location = new Point(YLineX, YLineY);
            }
            void UpdateLLocation()
            {
                LeftOffsetLine.Location = new Point(LLineX, LLineY);
                if (mirror)
                {
                    LLineY -= LeftOffsetTriangle.Image.Height;
                }
                else
                {
                    LLineX -= LeftOffsetTriangle.Image.Width;
                }
                LeftOffsetTriangle.Location = new Point(LLineX, LLineY);
            }
            void UpdateRLocation()
            {
                RightOffsetLine.Location = new Point(RLineX, RLineY);
                if (mirror)
                {
                    //RLineY -= RightOffsetTriangle.Image.Height;
                }
                RightOffsetTriangle.Location = new Point(RLineX, RLineY);
            }            

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
                    case nameof(NPCViewRect.LeftOffset):
                    //case nameof(BulletViewRect.LeftOffset):
                    case nameof(NPCViewRect.RightOffset):
                    //case nameof(BulletViewRect.RightOffset):
                        //only reset the Y image if the line being edited is the bigger one
                        if ((mirror
                            ? YOffsetLine.Image.Height != (centerY + 1) * 2
                            : YOffsetLine.Image.Width  != (centerX + 1) * 2
                            ))
                            UpdateYImage();
                        break;
                }
            }
            //always update line locations
            UpdateYLocation();
            UpdateLLocation();
            UpdateRLocation();
            UpdateBoxLocation(this, viewboxLayeredPictureBox);
            lastMirror = mirror;
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
