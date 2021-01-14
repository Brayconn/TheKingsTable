namespace CaveStoryEditor
{
    partial class FormStageEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStageEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.entitySpritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entityBoxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.editModeTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tilesetLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.mapResizeControl = new CaveStoryEditor.MapResizeControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.entityPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.entityListView = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.entityListBox = new System.Windows.Forms.ListBox();
            this.mapStatesTabPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.vScreenPreviewScrollBar = new System.Windows.Forms.VScrollBar();
            this.hScreenPreviewScrollBar = new System.Windows.Forms.HScrollBar();
            this.pictureBoxPanel = new System.Windows.Forms.Panel();
            this.mapLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.editModeTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.mapStatesTabPage.SuspendLayout();
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
            this.saveToolStripMenuItem,
            this.saveImageToolStripMenuItem});
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
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator2,
            this.selectAllEntitiesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // selectAllEntitiesToolStripMenuItem
            // 
            this.selectAllEntitiesToolStripMenuItem.Name = "selectAllEntitiesToolStripMenuItem";
            this.selectAllEntitiesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectAllEntitiesToolStripMenuItem.Text = "Select All Entities";
            this.selectAllEntitiesToolStripMenuItem.Click += new System.EventHandler(this.selectAllEntitiesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileTypesToolStripMenuItem,
            this.gridToolStripMenuItem,
            this.toolStripSeparator1,
            this.entitySpritesToolStripMenuItem,
            this.entityBoxesToolStripMenuItem,
            this.screenPreviewToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // tileTypesToolStripMenuItem
            // 
            this.tileTypesToolStripMenuItem.CheckOnClick = true;
            this.tileTypesToolStripMenuItem.Name = "tileTypesToolStripMenuItem";
            this.tileTypesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.tileTypesToolStripMenuItem.Text = "Tile Types";
            this.tileTypesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.tileTypesToolStripMenuItem_CheckedChanged);
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.CheckOnClick = true;
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            this.gridToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.gridToolStripMenuItem.Text = "Grid";
            this.gridToolStripMenuItem.CheckedChanged += new System.EventHandler(this.gridToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // entitySpritesToolStripMenuItem
            // 
            this.entitySpritesToolStripMenuItem.Checked = true;
            this.entitySpritesToolStripMenuItem.CheckOnClick = true;
            this.entitySpritesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.entitySpritesToolStripMenuItem.Name = "entitySpritesToolStripMenuItem";
            this.entitySpritesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.entitySpritesToolStripMenuItem.Text = "Entitiy Sprites";
            this.entitySpritesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.entitySpritesToolStripMenuItem_CheckedChanged);
            // 
            // entityBoxesToolStripMenuItem
            // 
            this.entityBoxesToolStripMenuItem.Checked = true;
            this.entityBoxesToolStripMenuItem.CheckOnClick = true;
            this.entityBoxesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.entityBoxesToolStripMenuItem.Name = "entityBoxesToolStripMenuItem";
            this.entityBoxesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.entityBoxesToolStripMenuItem.Text = "Entity Boxes";
            this.entityBoxesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.entityBoxesToolStripMenuItem_CheckedChanged);
            // 
            // screenPreviewToolStripMenuItem
            // 
            this.screenPreviewToolStripMenuItem.CheckOnClick = true;
            this.screenPreviewToolStripMenuItem.Name = "screenPreviewToolStripMenuItem";
            this.screenPreviewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.screenPreviewToolStripMenuItem.Text = "Screen Preview";
            this.screenPreviewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.screenPreviewToolStripMenuItem_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.editModeTabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.vScreenPreviewScrollBar);
            this.splitContainer1.Panel2.Controls.Add(this.hScreenPreviewScrollBar);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBoxPanel);
            this.splitContainer1.Size = new System.Drawing.Size(800, 426);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 1;
            // 
            // editModeTabControl
            // 
            this.editModeTabControl.Controls.Add(this.tabPage1);
            this.editModeTabControl.Controls.Add(this.tabPage2);
            this.editModeTabControl.Controls.Add(this.tabPage3);
            this.editModeTabControl.Controls.Add(this.mapStatesTabPage);
            this.editModeTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editModeTabControl.Location = new System.Drawing.Point(0, 0);
            this.editModeTabControl.Name = "editModeTabControl";
            this.editModeTabControl.SelectedIndex = 0;
            this.editModeTabControl.Size = new System.Drawing.Size(266, 426);
            this.editModeTabControl.TabIndex = 0;
            this.editModeTabControl.SelectedIndexChanged += new System.EventHandler(this.editModeTabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(258, 400);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Map";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tilesetLayeredPictureBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.mapResizeControl, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(252, 394);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tilesetLayeredPictureBox
            // 
            this.tilesetLayeredPictureBox.AutoScroll = true;
            this.tilesetLayeredPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilesetLayeredPictureBox.Location = new System.Drawing.Point(3, 200);
            this.tilesetLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.tilesetLayeredPictureBox.Name = "tilesetLayeredPictureBox";
            this.tilesetLayeredPictureBox.Size = new System.Drawing.Size(246, 191);
            this.tilesetLayeredPictureBox.TabIndex = 2;
            this.tilesetLayeredPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tilesetLayeredPictureBox_MouseDown);
            this.tilesetLayeredPictureBox.MouseLeave += new System.EventHandler(this.tilesetLayeredPictureBox_MouseLeave);
            this.tilesetLayeredPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tilesetLayeredPictureBox_MouseMove);
            this.tilesetLayeredPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tilesetLayeredPictureBox_MouseUp);
            // 
            // mapResizeControl
            // 
            this.mapResizeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapResizeControl.Location = new System.Drawing.Point(3, 3);
            this.mapResizeControl.Name = "mapResizeControl";
            this.mapResizeControl.Size = new System.Drawing.Size(246, 191);
            this.mapResizeControl.TabIndex = 3;
            this.mapResizeControl.MapResizeInitialized += new System.EventHandler<CaveStoryEditor.MapResizeInitiatedEventArgs>(this.mapResizeControl1_MapResizeInitialized);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(258, 400);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Entity";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.entityPropertyGrid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.entityListView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 394);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // entityPropertyGrid
            // 
            this.entityPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.entityPropertyGrid.Name = "entityPropertyGrid";
            this.entityPropertyGrid.Size = new System.Drawing.Size(246, 191);
            this.entityPropertyGrid.TabIndex = 0;
            // 
            // entityListView
            // 
            this.entityListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityListView.HideSelection = false;
            this.entityListView.Location = new System.Drawing.Point(3, 200);
            this.entityListView.MultiSelect = false;
            this.entityListView.Name = "entityListView";
            this.entityListView.Size = new System.Drawing.Size(246, 191);
            this.entityListView.TabIndex = 1;
            this.entityListView.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.entityListBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(258, 400);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Entity List";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // entityListBox
            // 
            this.entityListBox.AllowDrop = true;
            this.entityListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityListBox.FormattingEnabled = true;
            this.entityListBox.Location = new System.Drawing.Point(3, 3);
            this.entityListBox.Name = "entityListBox";
            this.entityListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.entityListBox.Size = new System.Drawing.Size(252, 394);
            this.entityListBox.TabIndex = 0;
            this.entityListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.entityListBox_Format);
            this.entityListBox.SelectedValueChanged += new System.EventHandler(this.entityListBox_SelectedValueChanged);
            this.entityListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.entityListBox_DragDrop);
            this.entityListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.entityListBox_DragEnter);
            this.entityListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.entityListBox_MouseDown);
            this.entityListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.entityListBox_MouseMove);
            this.entityListBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.entityListBox_MouseUp);
            // 
            // mapStatesTabPage
            // 
            this.mapStatesTabPage.Controls.Add(this.label1);
            this.mapStatesTabPage.Location = new System.Drawing.Point(4, 22);
            this.mapStatesTabPage.Name = "mapStatesTabPage";
            this.mapStatesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mapStatesTabPage.Size = new System.Drawing.Size(258, 400);
            this.mapStatesTabPage.TabIndex = 3;
            this.mapStatesTabPage.Text = "Map States";
            this.mapStatesTabPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 394);
            this.label1.TabIndex = 0;
            this.label1.Text = ";)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // vScreenPreviewScrollBar
            // 
            this.vScreenPreviewScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScreenPreviewScrollBar.Location = new System.Drawing.Point(513, 0);
            this.vScreenPreviewScrollBar.Name = "vScreenPreviewScrollBar";
            this.vScreenPreviewScrollBar.Size = new System.Drawing.Size(17, 409);
            this.vScreenPreviewScrollBar.TabIndex = 3;
            this.vScreenPreviewScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScreenPreviewScrollChanged);
            // 
            // hScreenPreviewScrollBar
            // 
            this.hScreenPreviewScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScreenPreviewScrollBar.Location = new System.Drawing.Point(0, 409);
            this.hScreenPreviewScrollBar.Name = "hScreenPreviewScrollBar";
            this.hScreenPreviewScrollBar.Size = new System.Drawing.Size(513, 17);
            this.hScreenPreviewScrollBar.TabIndex = 2;
            this.hScreenPreviewScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScreenPreviewScrollChanged);
            // 
            // pictureBoxPanel
            // 
            this.pictureBoxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPanel.AutoScroll = true;
            this.pictureBoxPanel.Controls.Add(this.mapLayeredPictureBox);
            this.pictureBoxPanel.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPanel.Name = "pictureBoxPanel";
            this.pictureBoxPanel.Size = new System.Drawing.Size(513, 409);
            this.pictureBoxPanel.TabIndex = 1;
            // 
            // mapLayeredPictureBox
            // 
            this.mapLayeredPictureBox.AutoScroll = true;
            this.mapLayeredPictureBox.Location = new System.Drawing.Point(0, 0);
            this.mapLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.mapLayeredPictureBox.Name = "mapLayeredPictureBox";
            this.mapLayeredPictureBox.Size = new System.Drawing.Size(200, 100);
            this.mapLayeredPictureBox.TabIndex = 1;
            this.mapLayeredPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPictureBox_Paint);
            this.mapLayeredPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseDown);
            this.mapLayeredPictureBox.MouseEnter += new System.EventHandler(this.mapLayeredPictureBox_MouseEnter);
            this.mapLayeredPictureBox.MouseLeave += new System.EventHandler(this.mapPictureBox_MouseLeave);
            this.mapLayeredPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseMove);
            this.mapLayeredPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseUp);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveImageToolStripMenuItem.Text = "Save Image...";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // FormStageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormStageEditor";
            this.Text = "FormStageEditor";
            this.Activated += new System.EventHandler(this.FormStageEditor_Activated);
            this.Load += new System.EventHandler(this.FormStageEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormStageEditor_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.editModeTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.mapStatesTabPage.ResumeLayout(false);
            this.pictureBoxPanel.ResumeLayout(false);
            this.pictureBoxPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl editModeTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PropertyGrid entityPropertyGrid;
        private System.Windows.Forms.ListView entityListView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem tileTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entitySpritesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllEntitiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entityBoxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem screenPreviewToolStripMenuItem;
        private LayeredPictureBox.LayeredPictureBox tilesetLayeredPictureBox;
        private System.Windows.Forms.Panel pictureBoxPanel;
        private LayeredPictureBox.LayeredPictureBox mapLayeredPictureBox;
        private System.Windows.Forms.VScrollBar vScreenPreviewScrollBar;
        private System.Windows.Forms.HScrollBar hScreenPreviewScrollBar;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox entityListBox;
        private MapResizeControl mapResizeControl;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TabPage mapStatesTabPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
    }
}