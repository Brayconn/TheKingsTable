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
                //HACK these should be in the mod
                var TEMP_VALUES = new List<string>
                {
                    "Null",
                    "Snake Lv. 1",
                    "Snake Lv. 2",
                    "Snake Lv. 3",
                    "Polar Star Lv. 1",
                    "Polar Star Lv. 2",
                    "Polar Star Lv. 3",
                    "Fireball Lv. 1",
                    "Fireball Lv. 2",
                    "Fireball Lv. 3",
                    "Machine Gun Lv. 1",
                    "Machine Gun Lv. 2",
                    "Machine Gun Lv. 3",
                    "Missile Launcher Lv. 1",
                    "Missile Launcher Lv. 2",
                    "Missile Launcher Lv. 3",
                    "Missile Launcher (Explosion) Lv. 1",
                    "Missile Launcher (Explosion) Lv. 2",
                    "Missile Launcher (Explosion) Lv. 3",
                    "Bubbler Lv. 1",
                    "Bubbler Lv. 2",
                    "Bubbler Lv. 3",
                    "Bubbler (Spines) Lv. 3",
                    "Blade (Slashes) Lv. 3",
                    "Egg Corridor? Falling Spike",
                    "Blade Lv. 1",
                    "Blade Lv. 2",
                    "Blade Lv. 3",
                    "Super Missile Launcher Lv. 1",
                    "Super Missile Launcher Lv. 2",
                    "Super Missile Launcher Lv. 3",
                    "Super Missile Launcher (Explosion) Lv. 1",
                    "Super Missile Launcher (Explosion) Lv. 2",
                    "Super Missile Launcher (Explosion) Lv. 3",
                    "Nemesis Lv. 1",
                    "Nemesis Lv. 2",
                    "Nemesis Lv. 3",
                    "Spur Lv. 1",
                    "Spur Lv. 2",
                    "Spur Lv. 3",
                    "Spur (Trail Tail) Lv. 3",
                    "Spur (Trail Body) Lv. 3",
                    "Spur (Trail Head) Lv. 3",
                    "Curly's Nemesis",
                    "Debug Kill-all Bullet",
                    "Whimsical Star"
                };
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
