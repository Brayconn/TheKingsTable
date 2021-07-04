using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class BitEditor : UserControl
    {
        public event EventHandler BitChanged;

        bool disableCheckEvent = false;

        ulong underlyingBitValue;
        public ulong UnderlyingBitValue
        {
            get => underlyingBitValue;
            set
            {
                if(underlyingBitValue != value)
                {
                    underlyingBitValue = value;
                    UpdateBitDisplay();
                }
            }
        }

        public BitEditor()
        {
            InitializeComponent();
        }

        public void Initialize(Dictionary<ulong, string> bitNames, int bitWidth)
        {
            checkedListBox.Items.Clear();
            int highestName = 0;
            for(int i = 0; i < bitWidth; i++)
            {
                var val = (ulong)1 << i;
                if (bitNames.TryGetValue(val, out var name))
                    highestName = i;
                else
                    name = "0x" + val.ToString($"{bitWidth}X");
                checkedListBox.Items.Add(name);
            }
            while (highestName < checkedListBox.Items.Count-1)
            {
                checkedListBox.Items.RemoveAt(highestName + 1);
            }
        }
        public void Initialize(Type @enum, int bitWidth)
        {
            var baseType = @enum.GetEnumUnderlyingType();
            var names = Enum.GetNames(@enum);

            var vals = new ulong[names.Length];
            var j = 0;
            foreach (var val in Enum.GetValues(@enum))
                vals[j++] = (ulong)Convert.ChangeType(val, typeof(ulong));

            var dict = new Dictionary<ulong, string>(names.Length);
            for (int i = 0; i < names.Length; i++)
                dict.Add(vals[i], names[i]);

            Initialize(dict, bitWidth);
        }

        public void UpdateBitDisplay()
        {
            disableCheckEvent = true;
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemChecked(i, (underlyingBitValue & ((ulong)1 << i)) != 0);
            }
            disableCheckEvent = false;
        }

        private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!disableCheckEvent)
            {
                disableCheckEvent = true;
                var bit = (ulong)1 << e.Index;
                switch (e.NewValue)
                {
                    case CheckState.Checked:
                        UnderlyingBitValue |= bit;
                        break;
                    case CheckState.Unchecked:
                        UnderlyingBitValue &= ~bit;
                        break;
                    default:
                        throw new ArgumentException($"Invalid check stage: {e.NewValue}");
                }
                BitChanged?.Invoke(this, new EventArgs());
                disableCheckEvent = false;
            }
        }
    }
}
