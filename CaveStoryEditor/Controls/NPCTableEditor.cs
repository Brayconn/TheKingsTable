using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class NPCTableEditor : UserControl
    {
        NPCTableEntry selectedNPCTableEntry => propertyGridListBox1.SelectedItem;

        Dictionary<int, ISurfaceSource> surfaceDescriptors { get; set; }
        Dictionary<int, string> soundEffects { get; set; }
        Dictionary<int, string> smokeSizes { get; set; }
        Dictionary<int, EntityInfo> entityInfos { get; set; }

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

        public NPCTableEditor()
        {
            InitializeComponent();

            propertyGridListBox1.SelectedItemChanging += PropertyGridListBox1_SelectedItemChanging;
            propertyGridListBox1.SelectedItemChanged += PropertyGridListBox1_SelectedItemChanged;
            
            propertyGridListBox1.ItemAdded += PropertyGridListBox1_ItemCountChanged;
            propertyGridListBox1.ItemInserted += PropertyGridListBox1_ItemCountChanged;
            propertyGridListBox1.ItemRemoved += PropertyGridListBox1_ItemCountChanged;

            propertyGridListBox1.Format += npcTableListBox_Format;

            bitEditor1.BitChanged += bitsCheckedListBox_ItemCheck;
        }

        
        /// <summary>
        /// Reset to a default state based on the given mod parameters
        /// </summary>
        /// <param name="mod"></param>
        public void InitMod(Mod mod)
        {
            surfaceDescriptors = mod.SurfaceDescriptors;
            soundEffects = mod.SoundEffects;
            smokeSizes = mod.SmokeSizes;
            entityInfos = mod.EntityInfos;

            //Initialize combobox data sources
            void bind<T>(ComboBox cb, IDictionary<int, T> source)
            {
                var sorted = new SortedDictionary<int, T>(source);
                cb.Items.Clear();
                var biggest = 0;
                foreach (var item in sorted)
                {
                    string val;
                    if (item is KeyValuePair<int, ISurfaceSource> ss)
                        val = FormatKVP(ss);
                    else if (item is KeyValuePair<int, string> s)
                        val = FormatKVP(s);
                    else
                        throw new ArgumentException();

                    var width = TextRenderer.MeasureText(val, cb.Font).Width;
                    if (width > biggest)
                        biggest = width;

                    cb.Items.Add(item);
                }
                cb.DropDownWidth = biggest + SystemInformation.VerticalScrollBarWidth;
            }
            bind(spriteSurfaceComboBox, surfaceDescriptors);
            bind(hitSoundComboBox, soundEffects);
            bind(deathSoundComboBox, soundEffects);
            bind(smokeSizeComboBox, smokeSizes);

            //TODO allow for custom flag names
            bitEditor1.Initialize(typeof(EntityFlags), sizeof(ushort) * 8);
        }
        
        public void LoadTable(List<NPCTableEntry> list)
        {
            propertyGridListBox1.List = list;

            HasUnsavedChanges = false;
            NPCTableEntryUIEnabled = true;
        }
        public void UnloadTable()
        {
            propertyGridListBox1.List = null;

            HasUnsavedChanges = false;
            NPCTableEntryUIEnabled = false;
        }

        #region values changing

        bool lockNpcTableEntry = false;
        void currentNPCTableEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;
                switch (e.PropertyName)
                {
                    case nameof(NPCTableEntry.SpriteSurface):
                        SetSurfaceComboBoxValue(spriteSurfaceComboBox, surfaceDescriptors, selectedNPCTableEntry.SpriteSurface);
                        break;
                    case nameof(NPCTableEntry.HitSound):
                        SetStringComboBoxValue(hitSoundComboBox, soundEffects, selectedNPCTableEntry.HitSound);
                        break;
                    case nameof(NPCTableEntry.DeathSound):
                        SetStringComboBoxValue(deathSoundComboBox, soundEffects, selectedNPCTableEntry.DeathSound);
                        break;
                    case nameof(NPCTableEntry.SmokeSize):
                        SetStringComboBoxValue(smokeSizeComboBox, smokeSizes, selectedNPCTableEntry.SmokeSize);
                        break;
                    case nameof(NPCTableEntry.Bits):
                        SetBitUI();
                        break;
                }
                propertyGridListBox1.RefreshPropertyGrid();
                lockNpcTableEntry = false;
            }            
            HasUnsavedChanges = true;
        }

        private void PropertyGridListBox1_ItemCountChanged(object sender, EventArgs e)
        {
            HasUnsavedChanges = true;
        }

        #region bits
        //UI -> entry
        private void bitsCheckedListBox_ItemCheck(object sender, EventArgs e)
        {
            if (!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;
                selectedNPCTableEntry.Bits = (EntityFlags)(ushort)bitEditor1.UnderlyingBitValue;
                propertyGridListBox1.RefreshPropertyGrid();
                lockNpcTableEntry = false;
            }
        }
        //entry -> UI
        private void SetBitUI()
        {
            bitEditor1.UnderlyingBitValue = (ulong)selectedNPCTableEntry.Bits;
        }
        #endregion

        #region comboboxes

        //entry -> UI
        void SetSurfaceComboBoxValue(ComboBox cb, IDictionary<int, ISurfaceSource> dict, int value)
        {
            //remove the extra entry
            if (cb.Items.Count > dict.Count)
                cb.Items.RemoveAt(cb.Items.Count - 1);

            if (dict.TryGetValue(value, out var ss))
            {
                //select the item if it exists
                cb.SelectedItem = new KeyValuePair<int, ISurfaceSource>(value, ss);
            }
            else
            {
                //otherwise add back the temp entry
                cb.Items.Add(new KeyValuePair<int, ISurfaceSource>(value, new SurfaceSourceRuntime("???")));
                cb.SelectedIndex = cb.Items.Count - 1;
            }
        }
        void SetStringComboBoxValue(ComboBox cb, IDictionary<int, string> dict, int value)
        {
            if (cb.Items.Count > dict.Count)
                cb.Items.RemoveAt(cb.Items.Count - 1);

            if (dict.TryGetValue(value, out var s))
            {
                cb.SelectedItem = new KeyValuePair<int, string>(value, s);
            }
            else
            {
                cb.Items.Add(new KeyValuePair<int, string>(value, "???"));
                cb.SelectedIndex = cb.Items.Count - 1;
            }
        }

        //UI -> entry
        void ComboBoxChanged(object sender, EventArgs e)
        {
            if (!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;

                var index = (byte)((KeyValuePair<int, string>)((ComboBox)sender).SelectedItem).Key;
                string prop;
                if (sender == hitSoundComboBox)
                    prop = nameof(NPCTableEntry.HitSound);
                else if (sender == deathSoundComboBox)
                    prop = nameof(NPCTableEntry.DeathSound);
                else if (sender == smokeSizeComboBox)
                    prop = nameof(NPCTableEntry.SmokeSize);
                else
                    throw new ArgumentException();
                typeof(NPCTableEntry).GetProperty(prop).SetValue(selectedNPCTableEntry, index);
                
                propertyGridListBox1.RefreshPropertyGrid();

                lockNpcTableEntry = false;
            }
        }
        void SurfaceChanged(object sender, EventArgs e)
        {
            if (!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;

                selectedNPCTableEntry.SpriteSurface = (byte)((KeyValuePair<int, ISurfaceSource>)((ComboBox)sender).SelectedItem).Key;

                if (spriteSurfaceComboBox.Items.Count > surfaceDescriptors.Count)
                    spriteSurfaceComboBox.Items.RemoveAt(spriteSurfaceComboBox.Items.Count - 1);
                
                propertyGridListBox1.RefreshPropertyGrid();

                lockNpcTableEntry = false;
            }
        }
        
        #endregion

        #endregion

        #region Hitbox/Viewbox rendering

        bool hitboxFacingRight = true;
        private void changeHitboxDirectionButton_Click(object sender, EventArgs e)
        {
            hitboxFacingRight = !hitboxFacingRight;
            changeHitboxDirectionButton.Text = hitboxFacingRight ? "--->" : "<---";
            hitboxPreview1.DrawHitbox(selectedNPCTableEntry.Hitbox, hitboxFacingRight);
        }

        private void Hitbox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyGridListBox1.RefreshPropertyGrid();
            Invoke(new Action<NPCHitRect, bool>(hitboxPreview1.DrawHitbox), selectedNPCTableEntry.Hitbox, hitboxFacingRight);
            HasUnsavedChanges = true;
        }

        private void Viewbox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyGridListBox1.RefreshPropertyGrid();
            if (e.PropertyName != nameof(NPCViewRect.Unused))
                Invoke(new Action<NPCViewRect, bool, string>(viewboxPreview1.DrawViewbox), selectedNPCTableEntry.Viewbox, false, e.PropertyName);
            HasUnsavedChanges = true;
        }
        
        #endregion

        bool npcTableEntryUIEnabled = false;
        bool NPCTableEntryUIEnabled
        {
            get => npcTableEntryUIEnabled;
            set
            {
                //the_man.gif
                bitEditor1.Enabled = value;
                lifeNumericUpDown.Enabled = value;
                spriteSurfaceComboBox.Enabled = value;
                hitSoundComboBox.Enabled = value;
                deathSoundComboBox.Enabled = value;
                smokeSizeComboBox.Enabled = value;
                XPNumericUpDown.Enabled = value;
                damageNumericUpDown.Enabled = value;
                //hitbox
                changeHitboxDirectionButton.Enabled = value;
                frontNumericUpDown.Enabled = value;
                topNumericUpDown.Enabled = value;
                backNumericUpDown.Enabled = value;
                bottomNumericUpDown.Enabled = value;
                //viewbox
                leftOffsetNumericUpDown.Enabled = value;
                yOffsetNumericUpDown.Enabled = value;
                rightOffsetNumericUpDown.Enabled = value;
                unusedNumericUpDown.Enabled = value;

                npcTableEntryUIEnabled = value;
            }
        }

        #region selected item changing

        private void PropertyGridListBox1_SelectedItemChanging(object sender, EventArgs e)
        {
            //un-hook the old entry
            if (propertyGridListBox1.SelectedItem is NPCTableEntry ent)
            {
                ent.Hitbox.PropertyChanged -= Hitbox_PropertyChanged;
                ent.Viewbox.PropertyChanged -= Viewbox_PropertyChanged;

                ent.PropertyChanged -= currentNPCTableEntry_PropertyChanged;
            }
        }

        private void PropertyGridListBox1_SelectedItemChanged(object sender, EventArgs e)
        {
            if (selectedNPCTableEntry != null)
            {
                //rebind the number things
                void bindNumericUpDown(NumericUpDown nud, string prop, object bs)
                {
                    nud.DataBindings.Clear();
                    //The OnPropertyChanged is very important for the hitbox/viewbox to look natural
                    //other properties are not affected
                    nud.DataBindings.Add(nameof(nud.Value), bs, prop, false, DataSourceUpdateMode.OnPropertyChanged);
                }

                var entrybs = new BindingSource(selectedNPCTableEntry, null);

                //Bits don't need to be rebound every time

                //Life
                bindNumericUpDown(lifeNumericUpDown, nameof(NPCTableEntry.Life), entrybs);

                //Sprite Surface, Hit, Death, Smoke
                //also don't need to be rebound every time            

                //XP and damage
                bindNumericUpDown(XPNumericUpDown, nameof(NPCTableEntry.XP), entrybs);
                bindNumericUpDown(damageNumericUpDown, nameof(NPCTableEntry.Damage), entrybs);

                //hitbox
                var hitboxbs = new BindingSource(selectedNPCTableEntry.Hitbox, null);
                bindNumericUpDown(frontNumericUpDown, nameof(NPCHitRect.Front), hitboxbs);
                bindNumericUpDown(topNumericUpDown, nameof(NPCHitRect.Top), hitboxbs);
                bindNumericUpDown(backNumericUpDown, nameof(NPCHitRect.Back), hitboxbs);
                bindNumericUpDown(bottomNumericUpDown, nameof(NPCHitRect.Bottom), hitboxbs);

                //viewbox
                var viewBS = new BindingSource(selectedNPCTableEntry.Viewbox, null);
                bindNumericUpDown(leftOffsetNumericUpDown, nameof(NPCViewRect.LeftOffset), viewBS);
                bindNumericUpDown(yOffsetNumericUpDown, nameof(NPCViewRect.YOffset), viewBS);
                bindNumericUpDown(rightOffsetNumericUpDown, nameof(NPCViewRect.RightOffset), viewBS);
                bindNumericUpDown(unusedNumericUpDown, nameof(NPCViewRect.Unused), viewBS);

                //update event subscriptions
                //hitbox
                selectedNPCTableEntry.Hitbox.PropertyChanged += Hitbox_PropertyChanged;
                hitboxPreview1.DrawHitbox(selectedNPCTableEntry.Hitbox, hitboxFacingRight);
                //viewbox
                selectedNPCTableEntry.Viewbox.PropertyChanged += Viewbox_PropertyChanged;
                viewboxPreview1.DrawViewbox(selectedNPCTableEntry.Viewbox, false);
                //all the enums
                selectedNPCTableEntry.PropertyChanged += currentNPCTableEntry_PropertyChanged;
                SetSurfaceComboBoxValue(spriteSurfaceComboBox, surfaceDescriptors, selectedNPCTableEntry.SpriteSurface);
                SetStringComboBoxValue(hitSoundComboBox, soundEffects, selectedNPCTableEntry.HitSound);
                SetStringComboBoxValue(deathSoundComboBox, soundEffects, selectedNPCTableEntry.DeathSound);
                SetStringComboBoxValue(smokeSizeComboBox, smokeSizes, selectedNPCTableEntry.SmokeSize);

                //bits are down here because the event has been subscribed by this point
                bitEditor1.UnderlyingBitValue = (ulong)selectedNPCTableEntry.Bits;
            }
        }

        #endregion

        #region string formatting
        static string FormatKVP<T>(KeyValuePair<int, T> kvp)
        {
            string @base = $"{kvp.Key} - ";
            if (typeof(T) == typeof(ISurfaceSource))
                @base += ((ISurfaceSource)kvp.Value).DisplayName;
            else if (typeof(T) == typeof(string))
                @base += kvp.Value.ToString();
            return @base;
        }
        
        private void npcTableListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is NPCTableEntry te && e.DesiredType == typeof(string))
            {
                var index = propertyGridListBox1.List.IndexOf(te);
                var val = $"{index} - ";
                if (entityInfos.TryGetValue(index, out EntityInfo ei))
                    val += ei.Name;
                e.Value = val;
            }
        }

        private void spriteSurfaceComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is KeyValuePair<int, ISurfaceSource> kvp && e.DesiredType == typeof(string))
            {
                e.Value = FormatKVP(kvp);
            }
        }
        private void stringStyleComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is KeyValuePair<int, string> kvp && e.DesiredType == typeof(string))
            {
                e.Value = FormatKVP(kvp);
            }
        }
        #endregion

        public void Save(string path)
        {
            NPCTable.Save(propertyGridListBox1.List, path);
            HasUnsavedChanges = false;
        }
    }
}
