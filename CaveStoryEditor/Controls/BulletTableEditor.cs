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

            bulletTablePropertyGridListBox1.SelectedItemChanging += BulletTablePropertyGridListBox1_SelectedItemChanging;
            bulletTablePropertyGridListBox1.SelectedItemChanged += BulletTablePropertyGridListBox1_SelectedItemChanged;

            damageNumericUpDown.Minimum = sbyte.MinValue;
            damageNumericUpDown.Maximum = sbyte.MaxValue;

            hitsNumericUpDown.Minimum = sbyte.MinValue;
            hitsNumericUpDown.Maximum = sbyte.MaxValue;

            rangeNumericUpDown.Minimum = int.MinValue;
            rangeNumericUpDown.Maximum = int.MaxValue;

            bulletTablePropertyGridListBox1.Format += BulletTablePropertyGridListBox1_Format;

            bitEditor1.BitChanged += BitEditor1_BitChanged;
        }

        private void BulletTablePropertyGridListBox1_SelectedItemChanging(object sender, EventArgs e)
        {
            if(bulletTablePropertyGridListBox1.SelectedItem is BulletTableEntry ent)
            {
                ent.PropertyChanged -= SelectedBulletTableEntry_PropertyChanged;
                ent.ViewBox.PropertyChanged -= ViewBox_PropertyChanged;
            }
        }

        private void BitEditor1_BitChanged(object sender, EventArgs e)
        {
            if (!lockEntry)
            {
                lockEntry = true;

                selectedBulletTableEntry.Bits = (BulletFlags)(uint)bitEditor1.UnderlyingBitValue;
                bulletTablePropertyGridListBox1.RefreshPropertyGrid();

                lockEntry = false;
            }
        }

        private void BulletTablePropertyGridListBox1_SelectedItemChanged(object sender, EventArgs e)
        {
            if(selectedBulletTableEntry != null)
            {
                //rebind the number things
                void bindNumericUpDown(NumericUpDown nud, string prop, object bs)
                {
                    nud.DataBindings.Clear();
                    //The OnPropertyChanged is very important for the hitbox/viewbox to look natural
                    //other properties are not affected
                    nud.DataBindings.Add(nameof(nud.Value), bs, prop, false, DataSourceUpdateMode.OnPropertyChanged);
                }

                var entrybs = new BindingSource(selectedBulletTableEntry, null);

                //Damage, hits, range
                bindNumericUpDown(damageNumericUpDown, nameof(selectedBulletTableEntry.Damage), entrybs);
                bindNumericUpDown(hitsNumericUpDown, nameof(selectedBulletTableEntry.Hits), entrybs);
                bindNumericUpDown(rangeNumericUpDown, nameof(selectedBulletTableEntry.Range), entrybs);

                //don't need to rebind bits

                //hitbox width/height
                bindNumericUpDown(enemyWidthNumericUpDown, nameof(selectedBulletTableEntry.EnemyHitboxWidth), entrybs);
                bindNumericUpDown(enemyHeightNumericUpDown, nameof(selectedBulletTableEntry.EnemyHitboxHeight), entrybs);

                //tile hitbox/width
                bindNumericUpDown(tileWidthNumericUpDown, nameof(selectedBulletTableEntry.TileHitboxWidth), entrybs);
                bindNumericUpDown(tileHeightNumericUpDown, nameof(selectedBulletTableEntry.TileHitboxHeight), entrybs);

                //viewbox
                var viewBS = new BindingSource(selectedBulletTableEntry.ViewBox, null);
                bindNumericUpDown(leftNumericUpDown, nameof(BulletViewRect.LeftOffset), viewBS);
                bindNumericUpDown(verticalNumericUpDown, nameof(BulletViewRect.YOffset), viewBS);
                bindNumericUpDown(rightNumericUpDown, nameof(BulletViewRect.RightOffset), viewBS);
                bindNumericUpDown(unusedNumericUpDown, nameof(BulletViewRect.Unused), viewBS);

                hitboxPreview1.DrawHitbox(selectedBulletTableEntry.EnemyHitboxWidth, selectedBulletTableEntry.EnemyHitboxHeight, selectedBulletTableEntry.EnemyHitboxWidth, selectedBulletTableEntry.EnemyHitboxHeight, false);
                hitboxPreview2.DrawHitbox(selectedBulletTableEntry.TileHitboxWidth, selectedBulletTableEntry.TileHitboxHeight, selectedBulletTableEntry.TileHitboxWidth, selectedBulletTableEntry.TileHitboxHeight, false);

                selectedBulletTableEntry.ViewBox.PropertyChanged += ViewBox_PropertyChanged;
                viewboxPreview1.DrawViewbox(selectedBulletTableEntry.ViewBox, Mirrored);

                selectedBulletTableEntry.PropertyChanged += SelectedBulletTableEntry_PropertyChanged;


                bitEditor1.UnderlyingBitValue = (ulong)selectedBulletTableEntry.Bits;
            }
        }

        bool lockEntry = false;
        private void SelectedBulletTableEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!lockEntry)
            {
                lockEntry = true;
                switch (e.PropertyName)
                {
                    case nameof(BulletTableEntry.EnemyHitboxWidth):
                    case nameof(BulletTableEntry.EnemyHitboxHeight):
                        Invoke(new Action<int, int, int, int, bool>(hitboxPreview1.DrawHitbox),
                            selectedBulletTableEntry.EnemyHitboxWidth, selectedBulletTableEntry.EnemyHitboxHeight, selectedBulletTableEntry.EnemyHitboxWidth, selectedBulletTableEntry.EnemyHitboxHeight, false);
                        break;
                    case nameof(BulletTableEntry.TileHitboxWidth):
                    case nameof(BulletTableEntry.TileHitboxHeight):
                        Invoke(new Action<int, int, int, int, bool>(hitboxPreview2.DrawHitbox),
                            selectedBulletTableEntry.TileHitboxWidth, selectedBulletTableEntry.TileHitboxHeight, selectedBulletTableEntry.TileHitboxWidth, selectedBulletTableEntry.TileHitboxHeight, false);
                        break;
                    case nameof(BulletTableEntry.Bits):
                        bitEditor1.UnderlyingBitValue = (ulong)selectedBulletTableEntry.Bits;
                        break;
                }
                bulletTablePropertyGridListBox1.RefreshPropertyGrid();
                lockEntry = false;
            }
            HasUnsavedChanges = true;
        }

        bool Mirrored { get; set; }
        private void ViewBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bulletTablePropertyGridListBox1.RefreshPropertyGrid();
            if (e.PropertyName != nameof(BulletViewRect.Unused))
                Invoke(new Action<BulletViewRect, bool, string>(viewboxPreview1.DrawViewbox), selectedBulletTableEntry.ViewBox, Mirrored, e.PropertyName);
            HasUnsavedChanges = true;
        }

        private void BulletTablePropertyGridListBox1_Format(object sender, ListControlConvertEventArgs e)
        {
            if(e.ListItem is BulletTableEntry bte && e.DesiredType == typeof(string))
            {
                var index = bulletTablePropertyGridListBox1.List.IndexOf(bte);
                var val = $"{index} - ";
                if (bulletInfos.TryGetValue(index, out var inf))
                    val += inf.Name;
                e.Value = val;
            }
        }

        Dictionary<int, BulletInfo> bulletInfos { get; set; }

        public void InitMod(Mod m)
        {
            bulletInfos = m.BulletInfos;

            //TODO customize bits
            bitEditor1.Initialize(typeof(BulletFlags), sizeof(int) * 8);
        }

        public void LoadTable(List<BulletTableEntry> list)
        {
            bulletTablePropertyGridListBox1.List = list;
        }

        private void mirroredButton_Click(object sender, EventArgs e)
        {
            Mirrored = !Mirrored;
            mirroredButton.Text = Mirrored ? "Vertical" : "Horizontal";
            viewboxPreview1.DrawViewbox(selectedBulletTableEntry.ViewBox, Mirrored);
        }
    }
}
