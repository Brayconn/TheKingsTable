
namespace CaveStoryEditor
{
    partial class ViewboxPreview
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
            this.viewboxLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.SuspendLayout();
            // 
            // viewboxLayeredPictureBox
            // 
            this.viewboxLayeredPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.viewboxLayeredPictureBox.AutoScroll = true;
            this.viewboxLayeredPictureBox.BackColor = System.Drawing.Color.Black;
            this.viewboxLayeredPictureBox.CanvasScale = 2;
            this.viewboxLayeredPictureBox.Location = new System.Drawing.Point(75, 75);
            this.viewboxLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.viewboxLayeredPictureBox.Name = "viewboxLayeredPictureBox";
            this.viewboxLayeredPictureBox.Size = new System.Drawing.Size(0, 0);
            this.viewboxLayeredPictureBox.TabIndex = 2;
            // 
            // ViewboxPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.viewboxLayeredPictureBox);
            this.Name = "ViewboxPreview";
            this.SizeChanged += new System.EventHandler(this.viewboxContainerPanel_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LayeredPictureBox.LayeredPictureBox viewboxLayeredPictureBox;
    }
}
