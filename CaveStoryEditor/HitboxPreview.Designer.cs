
namespace CaveStoryEditor
{
    partial class HitboxPreview
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hitboxLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.SuspendLayout();
            // 
            // hitboxLayeredPictureBox
            // 
            this.hitboxLayeredPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.hitboxLayeredPictureBox.AutoScroll = true;
            this.hitboxLayeredPictureBox.BackColor = System.Drawing.Color.Black;
            this.hitboxLayeredPictureBox.CanvasScale = 2;
            this.hitboxLayeredPictureBox.Location = new System.Drawing.Point(75, 75);
            this.hitboxLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.hitboxLayeredPictureBox.Name = "hitboxLayeredPictureBox";
            this.hitboxLayeredPictureBox.Size = new System.Drawing.Size(0, 0);
            this.hitboxLayeredPictureBox.TabIndex = 1;
            // 
            // HitboxPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.hitboxLayeredPictureBox);
            this.Name = "HitboxPreview";
            this.SizeChanged += new System.EventHandler(this.hitboxContainerPanel_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LayeredPictureBox.LayeredPictureBox hitboxLayeredPictureBox;
    }
}
