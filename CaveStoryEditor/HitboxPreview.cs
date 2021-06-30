using CaveStoryModdingFramework.Entities;
using LayeredPictureBox;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class HitboxPreview : UserControl
    {
        //hitbox layers
        Layer<Image> Hitbox, HitboxCenter;

        public HitboxPreview()
        {
            InitializeComponent();

            hitboxLayeredPictureBox.CreateLayer(out Hitbox);
            hitboxLayeredPictureBox.CreateLayer(out HitboxCenter);

            var pix = new Bitmap(1, 1);
            pix.SetPixel(0, 0, Color.White);
            HitboxCenter.Image = pix;
        }

        public void DrawHitbox(NPCHitRect rect, bool facingRight)
        {
            DrawHitbox(rect.Front, rect.Top, rect.Back, rect.Bottom, facingRight);
        }
        public void DrawHitbox(int front, int top, int back, int bottom, bool facingRight)
        {
            var size = new Size(back + front, top + bottom);
            if (!size.IsEmpty)
            {
                var bit = new Bitmap(size.Width + 1, size.Height + 1);
                using (var g = Graphics.FromImage(bit))
                {
                    if (size.Width == 0 || size.Height == 0)
                        //for some reason DrawRectangle doesn't let you lines
                        g.DrawLine(Pens.Red, 0, 0, size.Width, size.Height);
                    else
                        g.DrawRectangle(Pens.Red, 0, 0, size.Width, size.Height);
                }
                Hitbox.Image = bit;
            }
            else
                Hitbox.Image = null;
            HitboxCenter.Location = new Point(facingRight ? back : front, top);
            UpdateBoxLocation(this, hitboxLayeredPictureBox);
        }

        static void UpdateBoxLocation(ScrollableControl parent, Control child)
        {
            //TODO figure out why the hecc auto scroll doesn't work
            child.Location = new Point(parent.Width / 2 - child.Width / 2, parent.Height / 2 - child.Height / 2);
        }

        private void hitboxContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            UpdateBoxLocation(this, hitboxLayeredPictureBox);
        }
    }
}
