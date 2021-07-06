using CaveStoryModdingFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class BulletTableEditor : UserControl
    {
        BulletTableEntry selectedBulletTableEntry => bulletTablePropertyGridListBox1.SelectedItem;

        #region Unsaved changes

        public event EventHandler UnsavedChangesChanged;

        bool hasUnsavedChanges = false;
        public bool HasUnsavedChanges
        {
            get => hasUnsavedChanges;
            private set
            {
                if (value != hasUnsavedChanges)
                {
                    hasUnsavedChanges = value;

                    UnsavedChangesChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion

        public BulletTableEditor()
        {
            InitializeComponent();

            damageNumericUpDown.Minimum = sbyte.MinValue;
            damageNumericUpDown.Maximum = sbyte.MaxValue;

            hitsNumericUpDown.Minimum = sbyte.MinValue;
            hitsNumericUpDown.Maximum = sbyte.MaxValue;

            rangeNumericUpDown.Minimum = int.MinValue;
            rangeNumericUpDown.Maximum = int.MaxValue;

            bulletTablePropertyGridListBox1.Format += BulletTablePropertyGridListBox1_Format;
        }

        private void BulletTablePropertyGridListBox1_Format(object sender, ListControlConvertEventArgs e)
        {
            if(e.ListItem is BulletTableEntry bte && e.DesiredType == typeof(string))
            {
                var index = bulletTablePropertyGridListBox1.List.IndexOf(bte);
                var val = $"{index} - ";
                if(index < TEMP_VALUES.Count)
                {
                    val += TEMP_VALUES[index];
                }
                e.Value = val;
            }
        }

        public void InitMod(Mod m)
        {
            //TODO customize bits
            bitEditor1.Initialize(typeof(BulletFlags), sizeof(int) * 8);
        }

        public void LoadTable(List<BulletTableEntry> list)
        {
            bulletTablePropertyGridListBox1.List = list;
        }
    }
}
