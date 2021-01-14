namespace CaveStoryEditor
{
    partial class BitEditor
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
            this.flagCheckedListBox1 = new CaveStoryModdingFramework.FlagCheckedListBox();
            this.SuspendLayout();
            // 
            // flagCheckedListBox1
            // 
            this.flagCheckedListBox1.CheckOnClick = true;
            this.flagCheckedListBox1.FormattingEnabled = true;
            this.flagCheckedListBox1.Location = new System.Drawing.Point(3, 3);
            this.flagCheckedListBox1.Name = "flagCheckedListBox1";
            this.flagCheckedListBox1.Size = new System.Drawing.Size(120, 94);
            this.flagCheckedListBox1.TabIndex = 0;
            // 
            // BitEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flagCheckedListBox1);
            this.Name = "BitEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private CaveStoryModdingFramework.FlagCheckedListBox flagCheckedListBox1;
    }
}
