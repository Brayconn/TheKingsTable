using System;
using System.Drawing;
using System.Windows.Forms;
using CaveStoryModdingFramework.TSC;

namespace CaveStoryEditor
{
    public partial class FlagScratchPad : UserControl
    {
        int AddressOffset = FlagConverter.NPCFlagAddress;
        bool locked = false;

        int Length => (int)flagLengthNumericUpDown.Value;

        int Value
        {
            get => (int)ValueNumericUpDown.Value;
            set
            {
                ValueNumericUpDown.Value = value;
            }
        }

        string TSCText
        {
            get => TSCTextBox.Text;
            set
            {
                TSCTextBox.Text = value;
            }
        }

        int Address
        {
            get => (int)addressNumericUpDown.Value;
            set
            {
                addressNumericUpDown.ForeColor = 0 <= value ? DefaultForeColor : Color.Red;
                addressNumericUpDown.Value = value;
            }
        }

        int Bit
        {
            get => (int)bitNumericUpDown.Value;
            set
            {
                bitNumericUpDown.Value = value;
            }
        }


        public FlagScratchPad()
        {
            InitializeComponent();

            flagLengthNumericUpDown.Minimum = 0;
            flagLengthNumericUpDown.Maximum = int.MaxValue;

            ValueNumericUpDown.Minimum = int.MinValue;
            ValueNumericUpDown.Maximum = int.MaxValue;

            addressNumericUpDown.Minimum = int.MinValue;
            addressNumericUpDown.Maximum = int.MaxValue;

            customOffsetNumericUpDown.Minimum = int.MinValue;
            customOffsetNumericUpDown.Maximum = int.MaxValue;
        }

        private void tscTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!locked)
            {
                locked = true;

                if (TSCText.Length > Length)
                {
                    var s = TSCTextBox.SelectionStart;
                    TSCText = TSCText.Substring(0, Length);
                    TSCTextBox.SelectionStart = s;
                }
                var val = FlagConverter.FlagToRealValue(TSCText, Length);
                Value = val;
                Address = FlagConverter.RealValueToAddress(val, out var bit, AddressOffset);
                Bit = bit;
                locked = false;
            }
        }

        private void valueNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!locked)
            {
                locked = true;
                var val = Value;
                TSCText = FlagConverter.RealValueToFlag(val, Length);
                Address = FlagConverter.RealValueToAddress(val, out var bit, AddressOffset);
                Bit = bit;
                locked = false;
            }
        }

        private void addressNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!locked)
            {
                locked = true;
                var val = FlagConverter.AddressToRealValue(Address, Bit, AddressOffset);
                TSCText = FlagConverter.RealValueToFlag(val, Length);
                Value = val;
                locked = false;
            }
        }

        private void FlagTypeChanged(object sender, EventArgs e)
        {
            if (npcRadioButton.Checked)
                AddressOffset = FlagConverter.NPCFlagAddress;
            else if (skipRadioButton.Checked)
                AddressOffset = FlagConverter.SkipFlagAddress;
            else if (mapRadioButton.Checked)
                AddressOffset = FlagConverter.MapFlagAddress;
            //This one is NOT an "else if", to force the custom offset to toggle
            if (customOffsetNumericUpDown.Enabled = customRadioButton.Checked)
                AddressOffset = (int)customOffsetNumericUpDown.Value;            
            //simulate changing the raw value so the address updates
            valueNumericUpDown_ValueChanged(sender, e);
        }
        
        private void customOffsetnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(!locked)
            {
                locked = true;
                AddressOffset = (int)customOffsetNumericUpDown.Value;
                var add = FlagConverter.RealValueToAddress(Value, out var bit, AddressOffset);
                Address = add;
                Bit = bit;
                locked = false;
            }
        }

        private void flagLengthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            valueNumericUpDown_ValueChanged(sender, e);
        }
    }
}
