namespace CaveStoryEditor
{
    partial class FormAttributeEditor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAttributeEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.availableTileTypesLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.pictureBoxPanel = new System.Windows.Forms.Panel();
            this.attributesLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pictureBoxPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileTypesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // tileTypesToolStripMenuItem
            // 
            this.tileTypesToolStripMenuItem.Checked = true;
            this.tileTypesToolStripMenuItem.CheckOnClick = true;
            this.tileTypesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tileTypesToolStripMenuItem.Name = "tileTypesToolStripMenuItem";
            this.tileTypesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tileTypesToolStripMenuItem.Text = "Tile Types";
            this.tileTypesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.tileTypesToolStripMenuItem_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBoxPanel);
            this.splitContainer1.Size = new System.Drawing.Size(800, 426);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.availableTileTypesLayeredPictureBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(266, 426);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // availableTileTypesLayeredPictureBox
            // 
            this.availableTileTypesLayeredPictureBox.AutoScroll = true;
            this.availableTileTypesLayeredPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableTileTypesLayeredPictureBox.Location = new System.Drawing.Point(3, 216);
            this.availableTileTypesLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.availableTileTypesLayeredPictureBox.Name = "availableTileTypesLayeredPictureBox";
            this.availableTileTypesLayeredPictureBox.Size = new System.Drawing.Size(260, 207);
            this.availableTileTypesLayeredPictureBox.TabIndex = 2;
            this.availableTileTypesLayeredPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.availableTileTypesPictureBox_MouseClick);
            // 
            // pictureBoxPanel
            // 
            this.pictureBoxPanel.AutoScroll = true;
            this.pictureBoxPanel.Controls.Add(this.attributesLayeredPictureBox);
            this.pictureBoxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPanel.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPanel.Name = "pictureBoxPanel";
            this.pictureBoxPanel.Size = new System.Drawing.Size(530, 426);
            this.pictureBoxPanel.TabIndex = 0;
            // 
            // attributesLayeredPictureBox
            // 
            this.attributesLayeredPictureBox.AutoScroll = true;
            this.attributesLayeredPictureBox.Location = new System.Drawing.Point(0, 0);
            this.attributesLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.attributesLayeredPictureBox.Name = "attributesLayeredPictureBox";
            this.attributesLayeredPictureBox.Size = new System.Drawing.Size(200, 100);
            this.attributesLayeredPictureBox.TabIndex = 0;
            this.attributesLayeredPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.attributesLayeredPictureBox_MouseDown);
            this.attributesLayeredPictureBox.MouseEnter += new System.EventHandler(this.attributesLayeredPictureBox_MouseEnter);
            this.attributesLayeredPictureBox.MouseLeave += new System.EventHandler(this.attributesLayeredPictureBox_MouseLeave);
            this.attributesLayeredPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.attributesLayeredPictureBox_MouseMove);
            this.attributesLayeredPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.attributesLayeredPictureBox_MouseUp);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 213);
            this.label1.TabIndex = 3;
            this.label1.Text = "Don\'t know what to put here...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormAttributeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormAttributeEditor";
            this.Text = "FormAttributeEditor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAttributeEditor_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pictureBoxPanel.ResumeLayout(false);
            this.pictureBoxPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileTypesToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pictureBoxPanel;
        private LayeredPictureBox.LayeredPictureBox availableTileTypesLayeredPictureBox;
        private LayeredPictureBox.LayeredPictureBox attributesLayeredPictureBox;
        private System.Windows.Forms.Label label1;
    }
}