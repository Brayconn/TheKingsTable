namespace CaveStoryEditor
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveStageTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportStageTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveNPCTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportNPCTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadEntityInfotxtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateFlagListingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTsclisttxtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.stageTableTabPage = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.stageTableDataGridView = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.stageTablePropertyGrid = new CaveStoryEditor.PropertyGridShell();
            this.openTilesButton = new System.Windows.Forms.Button();
            this.openScriptButton = new System.Windows.Forms.Button();
            this.openBothButton = new System.Windows.Forms.Button();
            this.stageTableFormatComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.insertButton = new System.Windows.Forms.Button();
            this.npcTableTabPage = new System.Windows.Forms.TabPage();
            this.npcTableEditor = new CaveStoryEditor.NPCTableEditor();
            this.bulletTableTabPage = new System.Windows.Forms.TabPage();
            this.bulletTableEditor1 = new CaveStoryEditor.BulletTableEditor();
            this.assetsTabPage = new System.Windows.Forms.TabPage();
            this.assetsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pxmListBox = new System.Windows.Forms.ListBox();
            this.pxeListBox = new System.Windows.Forms.ListBox();
            this.imageListBox = new System.Windows.Forms.ListBox();
            this.scriptListBox = new System.Windows.Forms.ListBox();
            this.pxmLabel = new System.Windows.Forms.Label();
            this.pxeLabel = new System.Windows.Forms.Label();
            this.imageLabel = new System.Windows.Forms.Label();
            this.scriptLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.attributeListBox = new System.Windows.Forms.ListBox();
            this.modSettingsTabPage = new System.Windows.Forms.TabPage();
            this.modPropertyGrid = new CaveStoryEditor.PropertyGridShell();
            this.editorSettingsTabPage = new System.Windows.Forms.TabPage();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveBulletTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBulletTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.stageTableTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stageTableDataGridView)).BeginInit();
            this.tableLayoutPanel7.SuspendLayout();
            this.npcTableTabPage.SuspendLayout();
            this.bulletTableTabPage.SuspendLayout();
            this.assetsTabPage.SuspendLayout();
            this.assetsTableLayoutPanel.SuspendLayout();
            this.modSettingsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveStageTableToolStripMenuItem,
            this.exportStageTableToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveNPCTableToolStripMenuItem,
            this.exportNPCTableToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveBulletTableToolStripMenuItem,
            this.exportBulletTableToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Enabled = false;
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveProjectAsToolStripMenuItem
            // 
            this.saveProjectAsToolStripMenuItem.Enabled = false;
            this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProjectAsToolStripMenuItem.Text = "Save Project As...";
            this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.saveProjectAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // saveStageTableToolStripMenuItem
            // 
            this.saveStageTableToolStripMenuItem.Enabled = false;
            this.saveStageTableToolStripMenuItem.Name = "saveStageTableToolStripMenuItem";
            this.saveStageTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveStageTableToolStripMenuItem.Text = "Save Stage Table";
            this.saveStageTableToolStripMenuItem.Click += new System.EventHandler(this.saveStageTableToolStripMenuItem_Click);
            // 
            // exportStageTableToolStripMenuItem
            // 
            this.exportStageTableToolStripMenuItem.Enabled = false;
            this.exportStageTableToolStripMenuItem.Name = "exportStageTableToolStripMenuItem";
            this.exportStageTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportStageTableToolStripMenuItem.Text = "Export Stage Table...";
            this.exportStageTableToolStripMenuItem.Click += new System.EventHandler(this.exportStageTableToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // saveNPCTableToolStripMenuItem
            // 
            this.saveNPCTableToolStripMenuItem.Enabled = false;
            this.saveNPCTableToolStripMenuItem.Name = "saveNPCTableToolStripMenuItem";
            this.saveNPCTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveNPCTableToolStripMenuItem.Text = "Save NPC Table";
            this.saveNPCTableToolStripMenuItem.Click += new System.EventHandler(this.saveNPCTableToolStripMenuItem_Click);
            // 
            // exportNPCTableToolStripMenuItem
            // 
            this.exportNPCTableToolStripMenuItem.Enabled = false;
            this.exportNPCTableToolStripMenuItem.Name = "exportNPCTableToolStripMenuItem";
            this.exportNPCTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportNPCTableToolStripMenuItem.Text = "Export NPC Table...";
            this.exportNPCTableToolStripMenuItem.Click += new System.EventHandler(this.exportNPCTableToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadEntityInfotxtToolStripMenuItem,
            this.generateFlagListingToolStripMenuItem,
            this.loadTsclisttxtToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // loadEntityInfotxtToolStripMenuItem
            // 
            this.loadEntityInfotxtToolStripMenuItem.Enabled = false;
            this.loadEntityInfotxtToolStripMenuItem.Name = "loadEntityInfotxtToolStripMenuItem";
            this.loadEntityInfotxtToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadEntityInfotxtToolStripMenuItem.Text = "Load EntityInfo.txt";
            this.loadEntityInfotxtToolStripMenuItem.Click += new System.EventHandler(this.loadEntityInfotxtToolStripMenuItem_Click);
            // 
            // generateFlagListingToolStripMenuItem
            // 
            this.generateFlagListingToolStripMenuItem.Enabled = false;
            this.generateFlagListingToolStripMenuItem.Name = "generateFlagListingToolStripMenuItem";
            this.generateFlagListingToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.generateFlagListingToolStripMenuItem.Text = "Generate Flag Listing";
            this.generateFlagListingToolStripMenuItem.Click += new System.EventHandler(this.generateFlagListingToolStripMenuItem_Click);
            // 
            // loadTsclisttxtToolStripMenuItem
            // 
            this.loadTsclisttxtToolStripMenuItem.Name = "loadTsclisttxtToolStripMenuItem";
            this.loadTsclisttxtToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadTsclisttxtToolStripMenuItem.Text = "Load tsc_list.txt";
            this.loadTsclisttxtToolStripMenuItem.Click += new System.EventHandler(this.loadTsclisttxtToolStripMenuItem_Click);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.stageTableTabPage);
            this.mainTabControl.Controls.Add(this.npcTableTabPage);
            this.mainTabControl.Controls.Add(this.bulletTableTabPage);
            this.mainTabControl.Controls.Add(this.assetsTabPage);
            this.mainTabControl.Controls.Add(this.modSettingsTabPage);
            this.mainTabControl.Controls.Add(this.editorSettingsTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 24);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(800, 426);
            this.mainTabControl.TabIndex = 1;
            // 
            // stageTableTabPage
            // 
            this.stageTableTabPage.Controls.Add(this.splitContainer3);
            this.stageTableTabPage.Location = new System.Drawing.Point(4, 22);
            this.stageTableTabPage.Name = "stageTableTabPage";
            this.stageTableTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.stageTableTabPage.Size = new System.Drawing.Size(792, 400);
            this.stageTableTabPage.TabIndex = 0;
            this.stageTableTabPage.Text = "Stage Table";
            this.stageTableTabPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.stageTableDataGridView);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.tableLayoutPanel7);
            this.splitContainer3.Size = new System.Drawing.Size(786, 394);
            this.splitContainer3.SplitterDistance = 578;
            this.splitContainer3.TabIndex = 1;
            // 
            // stageTableDataGridView
            // 
            this.stageTableDataGridView.AllowDrop = true;
            this.stageTableDataGridView.AllowUserToOrderColumns = true;
            this.stageTableDataGridView.AllowUserToResizeRows = false;
            this.stageTableDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stageTableDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stageTableDataGridView.Location = new System.Drawing.Point(0, 0);
            this.stageTableDataGridView.MultiSelect = false;
            this.stageTableDataGridView.Name = "stageTableDataGridView";
            this.stageTableDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.stageTableDataGridView.Size = new System.Drawing.Size(578, 394);
            this.stageTableDataGridView.TabIndex = 0;
            this.stageTableDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.stageTableDataGridView_CellFormatting);
            this.stageTableDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.StageTableDataGridView_DataError);
            this.stageTableDataGridView.SelectionChanged += new System.EventHandler(this.stageTableDataGridView_SelectionChanged);
            this.stageTableDataGridView.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.stageTableDataGridView_UserAddedRow);
            this.stageTableDataGridView.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.stageTableDataGridView_UserDeletedRow);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.stageTablePropertyGrid, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.openTilesButton, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.openScriptButton, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.openBothButton, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.stageTableFormatComboBox, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.label13, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.insertButton, 0, 3);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 6;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(204, 394);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // stageTablePropertyGrid
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.stageTablePropertyGrid, 2);
            this.stageTablePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stageTablePropertyGrid.Location = new System.Drawing.Point(3, 258);
            this.stageTablePropertyGrid.Name = "stageTablePropertyGrid";
            this.stageTablePropertyGrid.Size = new System.Drawing.Size(198, 133);
            this.stageTablePropertyGrid.TabIndex = 0;
            // 
            // openTilesButton
            // 
            this.openTilesButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openTilesButton.Enabled = false;
            this.openTilesButton.Location = new System.Drawing.Point(3, 32);
            this.openTilesButton.Name = "openTilesButton";
            this.openTilesButton.Size = new System.Drawing.Size(96, 23);
            this.openTilesButton.TabIndex = 1;
            this.openTilesButton.Text = "Open Tiles";
            this.openTilesButton.UseVisualStyleBackColor = true;
            this.openTilesButton.Click += new System.EventHandler(this.openTilesButton_Click);
            // 
            // openScriptButton
            // 
            this.openScriptButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openScriptButton.Enabled = false;
            this.openScriptButton.Location = new System.Drawing.Point(105, 32);
            this.openScriptButton.Name = "openScriptButton";
            this.openScriptButton.Size = new System.Drawing.Size(96, 23);
            this.openScriptButton.TabIndex = 2;
            this.openScriptButton.Text = "Open Script";
            this.openScriptButton.UseVisualStyleBackColor = true;
            this.openScriptButton.Click += new System.EventHandler(this.openScriptButton_Click);
            // 
            // openBothButton
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.openBothButton, 2);
            this.openBothButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openBothButton.Enabled = false;
            this.openBothButton.Location = new System.Drawing.Point(3, 61);
            this.openBothButton.Name = "openBothButton";
            this.openBothButton.Size = new System.Drawing.Size(198, 23);
            this.openBothButton.TabIndex = 3;
            this.openBothButton.Text = "Open Both";
            this.openBothButton.UseVisualStyleBackColor = true;
            this.openBothButton.Click += new System.EventHandler(this.openBothButton_Click);
            // 
            // stageTableFormatComboBox
            // 
            this.stageTableFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stageTableFormatComboBox.Enabled = false;
            this.stageTableFormatComboBox.FormattingEnabled = true;
            this.stageTableFormatComboBox.Location = new System.Drawing.Point(105, 3);
            this.stageTableFormatComboBox.Name = "stageTableFormatComboBox";
            this.stageTableFormatComboBox.Size = new System.Drawing.Size(96, 21);
            this.stageTableFormatComboBox.TabIndex = 4;
            this.stageTableFormatComboBox.SelectionChangeCommitted += new System.EventHandler(this.stageTableFormatComboBox_SelectionChangeCommitted);
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 1);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 26);
            this.label13.TabIndex = 5;
            this.label13.Text = "Stage Table Format";
            // 
            // insertButton
            // 
            this.insertButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.insertButton.Enabled = false;
            this.insertButton.Location = new System.Drawing.Point(3, 90);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(96, 23);
            this.insertButton.TabIndex = 6;
            this.insertButton.Text = "Insert Entry";
            this.insertButton.UseVisualStyleBackColor = true;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // npcTableTabPage
            // 
            this.npcTableTabPage.Controls.Add(this.npcTableEditor);
            this.npcTableTabPage.Location = new System.Drawing.Point(4, 22);
            this.npcTableTabPage.Name = "npcTableTabPage";
            this.npcTableTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.npcTableTabPage.Size = new System.Drawing.Size(792, 400);
            this.npcTableTabPage.TabIndex = 1;
            this.npcTableTabPage.Text = "NPC Table";
            this.npcTableTabPage.UseVisualStyleBackColor = true;
            // 
            // npcTableEditor
            // 
            this.npcTableEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcTableEditor.Location = new System.Drawing.Point(3, 3);
            this.npcTableEditor.Name = "npcTableEditor";
            this.npcTableEditor.Size = new System.Drawing.Size(786, 394);
            this.npcTableEditor.TabIndex = 0;
            // 
            // bulletTableTabPage
            // 
            this.bulletTableTabPage.Controls.Add(this.bulletTableEditor1);
            this.bulletTableTabPage.Location = new System.Drawing.Point(4, 22);
            this.bulletTableTabPage.Name = "bulletTableTabPage";
            this.bulletTableTabPage.Size = new System.Drawing.Size(792, 400);
            this.bulletTableTabPage.TabIndex = 5;
            this.bulletTableTabPage.Text = "Bullet Table";
            this.bulletTableTabPage.UseVisualStyleBackColor = true;
            // 
            // bulletTableEditor1
            // 
            this.bulletTableEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bulletTableEditor1.Location = new System.Drawing.Point(0, 0);
            this.bulletTableEditor1.Name = "bulletTableEditor1";
            this.bulletTableEditor1.Size = new System.Drawing.Size(792, 400);
            this.bulletTableEditor1.TabIndex = 0;
            // 
            // assetsTabPage
            // 
            this.assetsTabPage.Controls.Add(this.assetsTableLayoutPanel);
            this.assetsTabPage.Location = new System.Drawing.Point(4, 22);
            this.assetsTabPage.Name = "assetsTabPage";
            this.assetsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.assetsTabPage.Size = new System.Drawing.Size(792, 400);
            this.assetsTabPage.TabIndex = 3;
            this.assetsTabPage.Text = "Assets";
            this.assetsTabPage.UseVisualStyleBackColor = true;
            // 
            // assetsTableLayoutPanel
            // 
            this.assetsTableLayoutPanel.ColumnCount = 5;
            this.assetsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.assetsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.assetsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.assetsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.assetsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.assetsTableLayoutPanel.Controls.Add(this.pxmListBox, 0, 1);
            this.assetsTableLayoutPanel.Controls.Add(this.pxeListBox, 1, 1);
            this.assetsTableLayoutPanel.Controls.Add(this.imageListBox, 2, 1);
            this.assetsTableLayoutPanel.Controls.Add(this.scriptListBox, 3, 1);
            this.assetsTableLayoutPanel.Controls.Add(this.pxmLabel, 0, 0);
            this.assetsTableLayoutPanel.Controls.Add(this.pxeLabel, 1, 0);
            this.assetsTableLayoutPanel.Controls.Add(this.imageLabel, 2, 0);
            this.assetsTableLayoutPanel.Controls.Add(this.scriptLabel, 3, 0);
            this.assetsTableLayoutPanel.Controls.Add(this.label12, 4, 0);
            this.assetsTableLayoutPanel.Controls.Add(this.attributeListBox, 4, 1);
            this.assetsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetsTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.assetsTableLayoutPanel.Name = "assetsTableLayoutPanel";
            this.assetsTableLayoutPanel.RowCount = 3;
            this.assetsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.assetsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.assetsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.assetsTableLayoutPanel.Size = new System.Drawing.Size(786, 394);
            this.assetsTableLayoutPanel.TabIndex = 0;
            // 
            // pxmListBox
            // 
            this.pxmListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pxmListBox.FormattingEnabled = true;
            this.pxmListBox.Location = new System.Drawing.Point(3, 23);
            this.pxmListBox.Name = "pxmListBox";
            this.pxmListBox.Size = new System.Drawing.Size(151, 348);
            this.pxmListBox.TabIndex = 0;
            // 
            // pxeListBox
            // 
            this.pxeListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pxeListBox.FormattingEnabled = true;
            this.pxeListBox.Location = new System.Drawing.Point(160, 23);
            this.pxeListBox.Name = "pxeListBox";
            this.pxeListBox.Size = new System.Drawing.Size(151, 348);
            this.pxeListBox.TabIndex = 1;
            // 
            // imageListBox
            // 
            this.imageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageListBox.FormattingEnabled = true;
            this.imageListBox.Location = new System.Drawing.Point(317, 23);
            this.imageListBox.Name = "imageListBox";
            this.imageListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.imageListBox.Size = new System.Drawing.Size(151, 348);
            this.imageListBox.TabIndex = 2;
            this.imageListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageListBox_MouseDown);
            // 
            // scriptListBox
            // 
            this.scriptListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptListBox.FormattingEnabled = true;
            this.scriptListBox.Location = new System.Drawing.Point(474, 23);
            this.scriptListBox.Name = "scriptListBox";
            this.scriptListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.scriptListBox.Size = new System.Drawing.Size(151, 348);
            this.scriptListBox.TabIndex = 3;
            this.scriptListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scriptListBox_MouseDown);
            // 
            // pxmLabel
            // 
            this.pxmLabel.AutoSize = true;
            this.pxmLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pxmLabel.Location = new System.Drawing.Point(3, 0);
            this.pxmLabel.Name = "pxmLabel";
            this.pxmLabel.Size = new System.Drawing.Size(151, 20);
            this.pxmLabel.TabIndex = 4;
            this.pxmLabel.Text = "Stage Tile Data";
            this.pxmLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pxeLabel
            // 
            this.pxeLabel.AutoSize = true;
            this.pxeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pxeLabel.Location = new System.Drawing.Point(160, 0);
            this.pxeLabel.Name = "pxeLabel";
            this.pxeLabel.Size = new System.Drawing.Size(151, 20);
            this.pxeLabel.TabIndex = 5;
            this.pxeLabel.Text = "Stage Entity Data";
            this.pxeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // imageLabel
            // 
            this.imageLabel.AutoSize = true;
            this.imageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLabel.Location = new System.Drawing.Point(317, 0);
            this.imageLabel.Name = "imageLabel";
            this.imageLabel.Size = new System.Drawing.Size(151, 20);
            this.imageLabel.TabIndex = 6;
            this.imageLabel.Text = "Images";
            this.imageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // scriptLabel
            // 
            this.scriptLabel.AutoSize = true;
            this.scriptLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptLabel.Location = new System.Drawing.Point(474, 0);
            this.scriptLabel.Name = "scriptLabel";
            this.scriptLabel.Size = new System.Drawing.Size(151, 20);
            this.scriptLabel.TabIndex = 7;
            this.scriptLabel.Text = "Scripts";
            this.scriptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(631, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(152, 20);
            this.label12.TabIndex = 8;
            this.label12.Text = "Tileset Data";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // attributeListBox
            // 
            this.attributeListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributeListBox.FormattingEnabled = true;
            this.attributeListBox.Location = new System.Drawing.Point(631, 23);
            this.attributeListBox.Name = "attributeListBox";
            this.attributeListBox.Size = new System.Drawing.Size(152, 348);
            this.attributeListBox.TabIndex = 9;
            this.attributeListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.attributeListBox_MouseDown);
            // 
            // modSettingsTabPage
            // 
            this.modSettingsTabPage.Controls.Add(this.modPropertyGrid);
            this.modSettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.modSettingsTabPage.Name = "modSettingsTabPage";
            this.modSettingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.modSettingsTabPage.Size = new System.Drawing.Size(792, 400);
            this.modSettingsTabPage.TabIndex = 2;
            this.modSettingsTabPage.Text = "Mod Settings";
            this.modSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // modPropertyGrid
            // 
            this.modPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.modPropertyGrid.Name = "modPropertyGrid";
            this.modPropertyGrid.Size = new System.Drawing.Size(786, 394);
            this.modPropertyGrid.TabIndex = 0;
            // 
            // editorSettingsTabPage
            // 
            this.editorSettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.editorSettingsTabPage.Name = "editorSettingsTabPage";
            this.editorSettingsTabPage.Size = new System.Drawing.Size(792, 400);
            this.editorSettingsTabPage.TabIndex = 4;
            this.editorSettingsTabPage.Text = "Editor Settings";
            this.editorSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // saveBulletTableToolStripMenuItem
            // 
            this.saveBulletTableToolStripMenuItem.Enabled = false;
            this.saveBulletTableToolStripMenuItem.Name = "saveBulletTableToolStripMenuItem";
            this.saveBulletTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveBulletTableToolStripMenuItem.Text = "Save Bullet Table";
            this.saveBulletTableToolStripMenuItem.Click += new System.EventHandler(this.saveBulletTableToolStripMenuItem_Click);
            // 
            // exportBulletTableToolStripMenuItem
            // 
            this.exportBulletTableToolStripMenuItem.Enabled = false;
            this.exportBulletTableToolStripMenuItem.Name = "exportBulletTableToolStripMenuItem";
            this.exportBulletTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportBulletTableToolStripMenuItem.Text = "Export Bullet Table...";
            this.exportBulletTableToolStripMenuItem.Click += new System.EventHandler(this.exportBulletTableToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "The King\'s Table";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            this.stageTableTabPage.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stageTableDataGridView)).EndInit();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.npcTableTabPage.ResumeLayout(false);
            this.bulletTableTabPage.ResumeLayout(false);
            this.assetsTabPage.ResumeLayout(false);
            this.assetsTableLayoutPanel.ResumeLayout(false);
            this.assetsTableLayoutPanel.PerformLayout();
            this.modSettingsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage stageTableTabPage;
        private System.Windows.Forms.DataGridView stageTableDataGridView;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveStageTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportStageTableToolStripMenuItem;
        private System.Windows.Forms.TabPage modSettingsTabPage;
        private PropertyGridShell modPropertyGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveNPCTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportNPCTableToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private PropertyGridShell stageTablePropertyGrid;
        private System.Windows.Forms.TabPage assetsTabPage;
        private System.Windows.Forms.TableLayoutPanel assetsTableLayoutPanel;
        private System.Windows.Forms.ListBox pxmListBox;
        private System.Windows.Forms.ListBox pxeListBox;
        private System.Windows.Forms.ListBox imageListBox;
        private System.Windows.Forms.ListBox scriptListBox;
        private System.Windows.Forms.Label pxmLabel;
        private System.Windows.Forms.Label pxeLabel;
        private System.Windows.Forms.Label imageLabel;
        private System.Windows.Forms.Label scriptLabel;
        private System.Windows.Forms.TabPage editorSettingsTabPage;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox attributeListBox;
        private System.Windows.Forms.Button openScriptButton;
        private System.Windows.Forms.Button openTilesButton;
        private System.Windows.Forms.Button openBothButton;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadEntityInfotxtToolStripMenuItem;
        private System.Windows.Forms.ComboBox stageTableFormatComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.ToolStripMenuItem generateFlagListingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTsclisttxtToolStripMenuItem;
        private System.Windows.Forms.TabPage npcTableTabPage;
        private NPCTableEditor npcTableEditor;
        private System.Windows.Forms.TabPage bulletTableTabPage;
        private BulletTableEditor bulletTableEditor1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveBulletTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportBulletTableToolStripMenuItem;
    }
}

