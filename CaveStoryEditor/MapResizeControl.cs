using CaveStoryModdingFramework.Maps;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    partial class MapResizeControl : UserControl
    {
        short CurrentWidth, CurrentHeight;
        short NewWidth { get => (short)newWidthNumericUpDown.Value; set => newWidthNumericUpDown.Value = value; }
        short NewHeight { get => (short)newHeightNumericUpDown.Value; set => newHeightNumericUpDown.Value = value; }
        ResizeModes ResizeMode { get => (ResizeModes)resizeModeComboBox.SelectedItem; set => resizeModeComboBox.SelectedItem = value; }
        bool ShrinkBuffer { get => shrinkBufferCheckBox.Checked; set => shrinkBufferCheckBox.Checked = value; }

        int CurrentBufferSize;

        public MapResizeControl()
        {
            InitializeComponent();

            editControls = new List<Control>()
            {
                newWidthNumericUpDown,
                newHeightNumericUpDown,
                resizeModeComboBox
            };

            resizeModeComboBox.DataSource = Enum.GetValues(typeof(ResizeModes));
            resizeModeComboBox.SelectedItem = ResizeModes.Logical;
        }
        public MapResizeControl(short width, short height, int bufferSize) : this()
        {
            InitSize(width, height, bufferSize);
        }
        public void InitSize(short width, short height, int bufferSize)
        {
            SetCurrentSize(width, height, bufferSize);
            NewWidth = width;
            NewHeight = height;
        }
        public void SetCurrentSize(short width, short height, int bufferSize)
        {
            CurrentWidth = width;
            CurrentHeight = height;
            CurrentBufferSize = bufferSize;

            currentWidthLabel.Text = CurrentWidth.ToString();
            currentHeightLabel.Text = CurrentHeight.ToString();
            currentBufferSizeLabel.Text = $"{CurrentBufferSize} Bytes";

            UpdateResizeButtonEnabled();
        }

        void UpdateResizeButtonEnabled()
        {
            resizeButton.Enabled = (NewWidth != CurrentWidth || NewHeight != CurrentHeight || NewWidth * NewHeight != CurrentBufferSize);
        }

        readonly List<Control> editControls;
        public bool IsBeingEdited => editControls.Contains(ActiveControl);

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateResizeButtonEnabled();
        }

        public event EventHandler<MapResizeInitiatedEventArgs> MapResizeInitialized;

        private void resizeButton_Click(object sender, EventArgs e)
        {
            if (MapResizeInitialized != null)
            {
                var args = new MapResizeInitiatedEventArgs(NewWidth, NewHeight, ResizeMode, ShrinkBuffer);
                MapResizeInitialized.Invoke(this, args);
                SetCurrentSize(args.NewWidth, args.NewHeight, args.NewBufferSize);
            }
        }
    }

    class MapResizeInitiatedEventArgs : EventArgs
    {
        public short NewWidth { get; set; }
        public short NewHeight { get; set; }
        public ResizeModes ResizeMode { get; }
        public bool ShrinkBuffer { get; }
        public int NewBufferSize { get; set; }

        public MapResizeInitiatedEventArgs(short width, short height, ResizeModes resizeMode, bool shrinkBuffer)
        {
            NewWidth = width;
            NewHeight = height;
            ResizeMode = resizeMode;
            ShrinkBuffer = shrinkBuffer;
        }
    }
}
