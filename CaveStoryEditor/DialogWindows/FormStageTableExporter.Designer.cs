namespace CaveStoryEditor
{
    partial class FormStageTableExporter
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.entrySettingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pathButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.cancelButton, 2);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelButton.Location = new System.Drawing.Point(3, 354);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(245, 21);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // exportButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.exportButton, 2);
            this.exportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportButton.Enabled = false;
            this.exportButton.Location = new System.Drawing.Point(254, 354);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(245, 21);
            this.exportButton.TabIndex = 1;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // entrySettingsPropertyGrid
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.entrySettingsPropertyGrid, 4);
            this.entrySettingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entrySettingsPropertyGrid.Location = new System.Drawing.Point(3, 45);
            this.entrySettingsPropertyGrid.Name = "entrySettingsPropertyGrid";
            this.entrySettingsPropertyGrid.Size = new System.Drawing.Size(496, 303);
            this.entrySettingsPropertyGrid.TabIndex = 2;
            // 
            // typeComboBox
            // 
            this.typeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(254, 18);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(170, 21);
            this.typeComboBox.TabIndex = 3;
            // 
            // pathTextBox
            // 
            this.pathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTextBox.Location = new System.Drawing.Point(3, 18);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(170, 20);
            this.pathTextBox.TabIndex = 4;
            this.pathTextBox.TextChanged += new System.EventHandler(this.pathTextBox_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.exportButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.pathTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.typeComboBox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.entrySettingsPropertyGrid, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pathButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.resetButton, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(502, 378);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // pathButton
            // 
            this.pathButton.Location = new System.Drawing.Point(179, 18);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(69, 21);
            this.pathButton.TabIndex = 5;
            this.pathButton.Text = "Browse...";
            this.pathButton.UseVisualStyleBackColor = true;
            this.pathButton.Click += new System.EventHandler(this.pathButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(430, 18);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(69, 21);
            this.resetButton.TabIndex = 6;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Filename";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(254, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Stage Table Type";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormStageTableExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 378);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormStageTableExporter";
            this.Text = "FormStageTableExporter";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.PropertyGrid entrySettingsPropertyGrid;
        private System.Windows.Forms.Button pathButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}