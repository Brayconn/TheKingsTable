using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    partial class MapResizeControl
    {
        private TableLayoutPanel tableLayoutPanel3;
        private Button resizeButton;
        private Label label1;
        private NumericUpDown newWidthNumericUpDown;
        private Label label2;
        private ComboBox resizeModeComboBox;
        private NumericUpDown newHeightNumericUpDown;
        private CheckBox shrinkBufferCheckBox;
        private Label label3;
        private Label currentWidthLabel;
        private Label currentHeightLabel;
        private Label label6;
        private Label label7;

        private void InitializeComponent()
        {
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.resizeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.newWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.resizeModeComboBox = new System.Windows.Forms.ComboBox();
            this.newHeightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.shrinkBufferCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.currentWidthLabel = new System.Windows.Forms.Label();
            this.currentHeightLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.currentBufferSizeLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newWidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newHeightNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.resizeButton, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.newWidthNumericUpDown, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.resizeModeComboBox, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.newHeightNumericUpDown, 3, 2);
            this.tableLayoutPanel3.Controls.Add(this.shrinkBufferCheckBox, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.currentWidthLabel, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.currentHeightLabel, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.label6, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.label7, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.currentBufferSizeLabel, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(238, 183);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // resizeButton
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.resizeButton, 4);
            this.resizeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resizeButton.Enabled = false;
            this.resizeButton.Location = new System.Drawing.Point(3, 155);
            this.resizeButton.Name = "resizeButton";
            this.resizeButton.Size = new System.Drawing.Size(232, 25);
            this.resizeButton.TabIndex = 0;
            this.resizeButton.Text = "Resize";
            this.resizeButton.UseVisualStyleBackColor = true;
            this.resizeButton.Click += new System.EventHandler(this.resizeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 38);
            this.label1.TabIndex = 5;
            this.label1.Text = "New Size";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // newWidthNumericUpDown
            // 
            this.newWidthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.newWidthNumericUpDown.Location = new System.Drawing.Point(75, 85);
            this.newWidthNumericUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.newWidthNumericUpDown.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.newWidthNumericUpDown.Name = "newWidthNumericUpDown";
            this.newWidthNumericUpDown.Size = new System.Drawing.Size(66, 20);
            this.newWidthNumericUpDown.TabIndex = 3;
            this.newWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.newWidthNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 38);
            this.label2.TabIndex = 6;
            this.label2.Text = "Resize Mode";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // resizeModeComboBox
            // 
            this.resizeModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeModeComboBox.FormattingEnabled = true;
            this.resizeModeComboBox.Location = new System.Drawing.Point(75, 122);
            this.resizeModeComboBox.Name = "resizeModeComboBox";
            this.resizeModeComboBox.Size = new System.Drawing.Size(66, 21);
            this.resizeModeComboBox.TabIndex = 2;
            // 
            // newHeightNumericUpDown
            // 
            this.newHeightNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.newHeightNumericUpDown.Location = new System.Drawing.Point(167, 85);
            this.newHeightNumericUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.newHeightNumericUpDown.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.newHeightNumericUpDown.Name = "newHeightNumericUpDown";
            this.newHeightNumericUpDown.Size = new System.Drawing.Size(68, 20);
            this.newHeightNumericUpDown.TabIndex = 4;
            this.newHeightNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // shrinkBufferCheckBox
            // 
            this.shrinkBufferCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.shrinkBufferCheckBox.AutoSize = true;
            this.shrinkBufferCheckBox.Checked = true;
            this.shrinkBufferCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel3.SetColumnSpan(this.shrinkBufferCheckBox, 2);
            this.shrinkBufferCheckBox.Location = new System.Drawing.Point(147, 124);
            this.shrinkBufferCheckBox.Name = "shrinkBufferCheckBox";
            this.shrinkBufferCheckBox.Size = new System.Drawing.Size(88, 17);
            this.shrinkBufferCheckBox.TabIndex = 1;
            this.shrinkBufferCheckBox.Text = "Shrink Buffer";
            this.shrinkBufferCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 38);
            this.label3.TabIndex = 7;
            this.label3.Text = "Current Size";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currentWidthLabel
            // 
            this.currentWidthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.currentWidthLabel.AutoSize = true;
            this.currentWidthLabel.Location = new System.Drawing.Point(75, 50);
            this.currentWidthLabel.Name = "currentWidthLabel";
            this.currentWidthLabel.Size = new System.Drawing.Size(66, 13);
            this.currentWidthLabel.TabIndex = 8;
            this.currentWidthLabel.Text = "Width";
            this.currentWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // currentHeightLabel
            // 
            this.currentHeightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.currentHeightLabel.AutoSize = true;
            this.currentHeightLabel.Location = new System.Drawing.Point(167, 50);
            this.currentHeightLabel.Name = "currentHeightLabel";
            this.currentHeightLabel.Size = new System.Drawing.Size(68, 13);
            this.currentHeightLabel.TabIndex = 9;
            this.currentHeightLabel.Text = "Height";
            this.currentHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "x";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "x";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 38);
            this.label4.TabIndex = 12;
            this.label4.Text = "Buffer Size";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currentBufferSizeLabel
            // 
            this.currentBufferSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.currentBufferSizeLabel.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.currentBufferSizeLabel, 3);
            this.currentBufferSizeLabel.Location = new System.Drawing.Point(75, 12);
            this.currentBufferSizeLabel.Name = "currentBufferSizeLabel";
            this.currentBufferSizeLabel.Size = new System.Drawing.Size(160, 13);
            this.currentBufferSizeLabel.TabIndex = 13;
            this.currentBufferSizeLabel.Text = "Bytes";
            this.currentBufferSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MapResizeControl
            // 
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "MapResizeControl";
            this.Size = new System.Drawing.Size(238, 183);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newWidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newHeightNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        private Label label4;
        private Label currentBufferSizeLabel;
    }
}
