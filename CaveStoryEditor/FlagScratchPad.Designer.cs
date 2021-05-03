
namespace CaveStoryEditor
{
    partial class FlagScratchPad
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
            this.TSCTextBox = new System.Windows.Forms.TextBox();
            this.ValueNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.customOffsetNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.customRadioButton = new System.Windows.Forms.RadioButton();
            this.mapRadioButton = new System.Windows.Forms.RadioButton();
            this.skipRadioButton = new System.Windows.Forms.RadioButton();
            this.npcRadioButton = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bitNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.addressNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumericUpDown)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customOffsetNumericUpDown)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.addressNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TSCTextBox
            // 
            this.TSCTextBox.Location = new System.Drawing.Point(63, 3);
            this.TSCTextBox.Name = "TSCTextBox";
            this.TSCTextBox.Size = new System.Drawing.Size(133, 20);
            this.TSCTextBox.TabIndex = 0;
            this.TSCTextBox.TextChanged += new System.EventHandler(this.tscTextBox_TextChanged);
            // 
            // ValueNumericUpDown
            // 
            this.ValueNumericUpDown.Location = new System.Drawing.Point(63, 33);
            this.ValueNumericUpDown.Name = "ValueNumericUpDown";
            this.ValueNumericUpDown.Size = new System.Drawing.Size(133, 20);
            this.ValueNumericUpDown.TabIndex = 1;
            this.ValueNumericUpDown.ValueChanged += new System.EventHandler(this.valueNumericUpDown_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ValueNumericUpDown, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.TSCTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 216);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TSC";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Value";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Address";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.customOffsetNumericUpDown);
            this.panel1.Controls.Add(this.customRadioButton);
            this.panel1.Controls.Add(this.mapRadioButton);
            this.panel1.Controls.Add(this.skipRadioButton);
            this.panel1.Controls.Add(this.npcRadioButton);
            this.panel1.Location = new System.Drawing.Point(63, 93);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(133, 119);
            this.panel1.TabIndex = 6;
            // 
            // customOffsetNumericUpDown
            // 
            this.customOffsetNumericUpDown.Enabled = false;
            this.customOffsetNumericUpDown.Hexadecimal = true;
            this.customOffsetNumericUpDown.Location = new System.Drawing.Point(3, 95);
            this.customOffsetNumericUpDown.Name = "customOffsetNumericUpDown";
            this.customOffsetNumericUpDown.Size = new System.Drawing.Size(85, 20);
            this.customOffsetNumericUpDown.TabIndex = 4;
            this.customOffsetNumericUpDown.ValueChanged += new System.EventHandler(this.customOffsetnumericUpDown_ValueChanged);
            // 
            // customRadioButton
            // 
            this.customRadioButton.AutoSize = true;
            this.customRadioButton.Location = new System.Drawing.Point(3, 72);
            this.customRadioButton.Name = "customRadioButton";
            this.customRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customRadioButton.TabIndex = 3;
            this.customRadioButton.TabStop = true;
            this.customRadioButton.Text = "Custom";
            this.customRadioButton.UseVisualStyleBackColor = true;
            this.customRadioButton.CheckedChanged += new System.EventHandler(this.FlagTypeChanged);
            // 
            // mapRadioButton
            // 
            this.mapRadioButton.AutoSize = true;
            this.mapRadioButton.Location = new System.Drawing.Point(3, 49);
            this.mapRadioButton.Name = "mapRadioButton";
            this.mapRadioButton.Size = new System.Drawing.Size(46, 17);
            this.mapRadioButton.TabIndex = 2;
            this.mapRadioButton.TabStop = true;
            this.mapRadioButton.Text = "Map";
            this.mapRadioButton.UseVisualStyleBackColor = true;
            this.mapRadioButton.CheckedChanged += new System.EventHandler(this.FlagTypeChanged);
            // 
            // skipRadioButton
            // 
            this.skipRadioButton.AutoSize = true;
            this.skipRadioButton.Location = new System.Drawing.Point(3, 26);
            this.skipRadioButton.Name = "skipRadioButton";
            this.skipRadioButton.Size = new System.Drawing.Size(46, 17);
            this.skipRadioButton.TabIndex = 1;
            this.skipRadioButton.TabStop = true;
            this.skipRadioButton.Text = "Skip";
            this.skipRadioButton.UseVisualStyleBackColor = true;
            this.skipRadioButton.CheckedChanged += new System.EventHandler(this.FlagTypeChanged);
            // 
            // npcRadioButton
            // 
            this.npcRadioButton.AutoSize = true;
            this.npcRadioButton.Checked = true;
            this.npcRadioButton.Location = new System.Drawing.Point(3, 3);
            this.npcRadioButton.Name = "npcRadioButton";
            this.npcRadioButton.Size = new System.Drawing.Size(88, 17);
            this.npcRadioButton.TabIndex = 0;
            this.npcRadioButton.TabStop = true;
            this.npcRadioButton.Text = "NPC (default)";
            this.npcRadioButton.UseVisualStyleBackColor = true;
            this.npcRadioButton.CheckedChanged += new System.EventHandler(this.FlagTypeChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Flag Type";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bitNumericUpDown);
            this.panel2.Controls.Add(this.addressNumericUpDown);
            this.panel2.Location = new System.Drawing.Point(63, 63);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(133, 24);
            this.panel2.TabIndex = 8;
            // 
            // bitNumericUpDown
            // 
            this.bitNumericUpDown.Location = new System.Drawing.Point(99, 1);
            this.bitNumericUpDown.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.bitNumericUpDown.Name = "bitNumericUpDown";
            this.bitNumericUpDown.Size = new System.Drawing.Size(34, 20);
            this.bitNumericUpDown.TabIndex = 4;
            this.bitNumericUpDown.ValueChanged += new System.EventHandler(this.addressNumericUpDown_ValueChanged);
            // 
            // addressNumericUpDown
            // 
            this.addressNumericUpDown.Hexadecimal = true;
            this.addressNumericUpDown.Location = new System.Drawing.Point(0, 1);
            this.addressNumericUpDown.Name = "addressNumericUpDown";
            this.addressNumericUpDown.Size = new System.Drawing.Size(92, 20);
            this.addressNumericUpDown.TabIndex = 3;
            this.addressNumericUpDown.ValueChanged += new System.EventHandler(this.addressNumericUpDown_ValueChanged);
            // 
            // FlagScratchPad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FlagScratchPad";
            this.Size = new System.Drawing.Size(200, 216);
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumericUpDown)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customOffsetNumericUpDown)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bitNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addressNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox TSCTextBox;
        private System.Windows.Forms.NumericUpDown ValueNumericUpDown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown addressNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown customOffsetNumericUpDown;
        private System.Windows.Forms.RadioButton customRadioButton;
        private System.Windows.Forms.RadioButton mapRadioButton;
        private System.Windows.Forms.RadioButton skipRadioButton;
        private System.Windows.Forms.RadioButton npcRadioButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown bitNumericUpDown;
    }
}
