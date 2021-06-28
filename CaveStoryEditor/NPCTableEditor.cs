using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Entities;
using LayeredPictureBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class NPCTableEditor : UserControl
    {
        //hitbox layers
        Layer<Image> Hitbox, HitboxCenter;

        //viewbox layers
        Layer<Image> LeftOffsetTriangle, LeftOffsetLine,
                     RightOffsetTriangle, RightOffsetLine,
                     YOffsetTriangle, YOffsetLine,
                     ViewCenter;
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

            #region listbox/viewbox
            hitboxLayeredPictureBox.CreateLayer(out Hitbox);
            hitboxLayeredPictureBox.CreateLayer(out HitboxCenter);

            viewboxLayeredPictureBox.CreateLayer(out YOffsetTriangle);
            viewboxLayeredPictureBox.CreateLayer(out YOffsetLine);
            viewboxLayeredPictureBox.CreateLayer(out LeftOffsetTriangle);
            viewboxLayeredPictureBox.CreateLayer(out LeftOffsetLine);
            viewboxLayeredPictureBox.CreateLayer(out RightOffsetTriangle);
            viewboxLayeredPictureBox.CreateLayer(out RightOffsetLine);
            viewboxLayeredPictureBox.CreateLayer(out ViewCenter);

            //create a shared image for the npc's location (Since it never gets changed)
            var pix = new Bitmap(1, 1);
            pix.SetPixel(0, 0, Color.White);
            HitboxCenter.Image = pix;
            ViewCenter.Image = pix;

            YOffsetTriangle.Image = MakeRightTriangle(Color.Yellow);
            LeftOffsetTriangle.Image = MakeRightTriangle(Color.Green, -180);
            RightOffsetTriangle.Image = MakeRightTriangle(Color.Red, 90);
            #endregion

            propertyGridListBox1.SelectedItemChanging += PropertyGridListBox1_SelectedItemChanging;
            propertyGridListBox1.SelectedItemChanged += PropertyGridListBox1_SelectedItemChanged;
            
            propertyGridListBox1.ItemAdded += PropertyGridListBox1_ItemCountChanged;
            propertyGridListBox1.ItemInserted += PropertyGridListBox1_ItemCountChanged;
            propertyGridListBox1.ItemRemoved += PropertyGridListBox1_ItemCountChanged;

            propertyGridListBox1.Format += npcTableListBox_Format;
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
            var names = Enum.GetNames(typeof(EntityFlags));
            bitsCheckedListBox.Items.Clear();
            foreach (var name in names)
                bitsCheckedListBox.Items.Add(name);
        }
        
        public void LoadTable(List<NPCTableEntry> list)
        {
            propertyGridListBox1.List = list;

            HasUnsavedChanges = false;
            NPCTableEntryUIEnabled = true;
        }
        public void UnloadTable()
        {
            propertyGridListBox1.List = null; ;

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
                        SetComboBoxValue(spriteSurfaceComboBox, surfaceDescriptors, selectedNPCTableEntry.SpriteSurface);
                        break;
                    case nameof(NPCTableEntry.HitSound):
                        SetComboBoxValue(hitSoundComboBox, soundEffects, selectedNPCTableEntry.HitSound);
                        break;
                    case nameof(NPCTableEntry.DeathSound):
                        SetComboBoxValue(deathSoundComboBox, soundEffects, selectedNPCTableEntry.DeathSound);
                        break;
                    case nameof(NPCTableEntry.SmokeSize):
                        SetComboBoxValue(smokeSizeComboBox, smokeSizes, selectedNPCTableEntry.SmokeSize);
                        break;
                    case nameof(NPCTableEntry.Bits):
                        SetBitsUI(selectedNPCTableEntry.Bits);
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
        private void bitsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;
                EntityFlags flag = (EntityFlags)(1 << e.Index);
                switch (e.NewValue)
                {
                    case CheckState.Checked:
                        selectedNPCTableEntry.Bits |= flag;
                        break;
                    case CheckState.Unchecked:
                        selectedNPCTableEntry.Bits &= ~flag;
                        break;
                    default:
                        throw new ArgumentException($"Invalid check stage: {e.NewValue}");
                }
                propertyGridListBox1.RefreshPropertyGrid();
                lockNpcTableEntry = false;
            }
        }
        //entry -> UI
        private void SetBitsUI(EntityFlags flags)
        {
            if (!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;
                var newVal = new BitArray(BitConverter.GetBytes((ushort)flags));
                for (int i = 0; i < newVal.Length; i++)
                    bitsCheckedListBox.SetItemChecked(i, newVal[i]);
                lockNpcTableEntry = false;
            }
        }
        #endregion

        #region comboboxes

        #region entry -> UI
        void SetComboBoxValue(ComboBox cb, IDictionary<int, ISurfaceSource> dict, int value)
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
        void SetComboBoxValue(ComboBox cb, IDictionary<int, string> dict, int value)
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

        #endregion

        #region UI -> entry
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

        #endregion

        #region Hitbox/Viewbox rendering

        #region graphics functions for the hitbox/viewbox
        static Bitmap MakeRightTriangle(Color c, float rotate = 0)
        {
            var tri = new Bitmap(MinBoxSize / 2, MinBoxSize / 2);
            using (var g = Graphics.FromImage(tri))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                if (rotate != 0)
                {
                    var w = (float)tri.Width / 2;
                    var h = (float)tri.Height / 2;
                    g.TranslateTransform(w, h);
                    g.RotateTransform(rotate);
                    g.TranslateTransform(-w, -h);
                }
                g.FillPolygon(new SolidBrush(c), new[]
                {
                    //top left
                    new Point(0,-1),
                    //bottom left
                    new Point(0,tri.Height),
                    //bottom right
                    new Point(tri.Width,tri.Height),
                });
            }
            return new Bitmap(tri);
        }
        static Bitmap MakeLine(int width, int height, Color c)
        {
            if (width <= 0 || height <= 0)
                return null;
            var l = new Bitmap(width, height);
            using (var g = Graphics.FromImage(l))
            {
                g.DrawLine(new Pen(c), 0, 0, width - 1, height - 1);
            }
            return new Bitmap(l);
        }
        #endregion

        const int MinBoxSize = 8;

        bool hitboxFacingRight = true;
        private void changeHitboxDirectionButton_Click(object sender, EventArgs e)
        {
            hitboxFacingRight = !hitboxFacingRight;
            changeHitboxDirectionButton.Text = hitboxFacingRight ? "--->" : "<---";
            DrawHitbox();
        }

        private void Hitbox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyGridListBox1.RefreshPropertyGrid();
            Invoke(new Action(DrawHitbox));
            HasUnsavedChanges = true;
        }

        void DrawHitbox()
        {
            var size = (Size)selectedNPCTableEntry.Hitbox;
            if (!size.IsEmpty)
            {
                var bit = new Bitmap(size.Width + 1, size.Height + 1);
                using (var g = Graphics.FromImage(bit))
                {
                    if (size.Width == 0 || size.Height == 0)
                        //for some reason DrawRectangle doesn't let you lines
                        g.DrawLine(Pens.Red, 0, 0, size.Width, size.Height);
                    else
                        g.DrawRectangle(Pens.Red, 0, 0, size.Width, size.Height);
                }
                Hitbox.Image = bit;
            }
            else
                Hitbox.Image = null;
            HitboxCenter.Location = new Point(hitboxFacingRight ? selectedNPCTableEntry.Hitbox.Back : selectedNPCTableEntry.Hitbox.Front, selectedNPCTableEntry.Hitbox.Top);
            UpdateBoxLocation(hitboxContainerPanel, hitboxLayeredPictureBox);
        }

        private void Viewbox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyGridListBox1.RefreshPropertyGrid();
            if (e.PropertyName != nameof(NPCViewRect.Unused))
                Invoke(new Action<string>(DrawViewbox), e.PropertyName);
            HasUnsavedChanges = true;
        }
        
        void DrawViewbox(string prop = null)
        {
            int Clamp(int value) => Math.Max(value, MinBoxSize);

            //Display location of the NPC's actual position, clamped to MinBoxSize
            var viewX = Clamp(Math.Max(selectedNPCTableEntry.Viewbox.LeftOffset, selectedNPCTableEntry.Viewbox.RightOffset));
            var viewY = Clamp(selectedNPCTableEntry.Viewbox.YOffset);

            //functions to update the images for all the lines
            void UpdateYImage() => YOffsetLine.Image = MakeLine((viewX + 1) * 2, 1, Color.Yellow);
            void UpdateLImage() => LeftOffsetLine.Image = MakeLine(1, (viewY + 1) * 2, Color.Green);
            void UpdateRImage() => RightOffsetLine.Image = MakeLine(1, (viewY + 1) * 2, Color.Red);

            //functions to update the images locations
            void UpdateYLocation()
            {
                YOffsetLine.Location = new Point(0, ViewCenter.Location.Y - selectedNPCTableEntry.Viewbox.YOffset);
                YOffsetTriangle.Location = new Point(0, YOffsetLine.Location.Y - YOffsetTriangle.Image.Height + 1);
            }
            void UpdateLLocation()
            {
                LeftOffsetLine.Location = new Point(ViewCenter.Location.X - selectedNPCTableEntry.Viewbox.LeftOffset, 0);
                LeftOffsetTriangle.Location = new Point(LeftOffsetLine.Location.X - LeftOffsetTriangle.Image.Width + 1, 0);
            }
            void UpdateRLocation()
            {
                RightOffsetLine.Location = new Point(ViewCenter.Location.X - selectedNPCTableEntry.Viewbox.RightOffset, 0);
                RightOffsetTriangle.Location = RightOffsetLine.Location;
            }

            //setting the NPC's center to 1.5x its value leaves room on the left so it looks pretty
            ViewCenter.Location = new Point(viewX + viewX / 2, viewY + viewY / 2);

            //reset all images
            if (prop == null)
            {
                UpdateLImage();
                UpdateRImage();
                UpdateYImage();
            }
            else
            {
                switch (prop)
                {
                    case nameof(NPCViewRect.YOffset):
                        UpdateLImage();
                        UpdateRImage();
                        break;
                    //only reset the Y image if the line being edited is the bigger one
                    case nameof(NPCViewRect.LeftOffset):
                    case nameof(NPCViewRect.RightOffset):
                        if (YOffsetLine.Image.Width != (viewX + 1) * 2)
                            UpdateYImage();
                        break;
                }
            }
            //always update line locations
            UpdateYLocation();
            UpdateLLocation();
            UpdateRLocation();
            UpdateBoxLocation(viewboxContainerPanel, viewboxLayeredPictureBox);
        }
        void UpdateBoxLocation(ScrollableControl parent, Control child)
        {
            //TODO figure out why the hecc auto scroll doesn't work
            child.Location = new Point(parent.Width / 2 - child.Width / 2, parent.Height / 2 - child.Height / 2);
        }

        private void viewboxContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            UpdateBoxLocation(viewboxContainerPanel, viewboxLayeredPictureBox);
        }

        private void hitboxContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            UpdateBoxLocation(hitboxContainerPanel, hitboxLayeredPictureBox);
        }
        #endregion

        bool npcTableEntryUIEnabled = false;
        bool NPCTableEntryUIEnabled
        {
            get => npcTableEntryUIEnabled;
            set
            {
                //the_man.gif
                bitsCheckedListBox.Enabled = value;
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
                DrawHitbox();
                //viewbox
                selectedNPCTableEntry.Viewbox.PropertyChanged += Viewbox_PropertyChanged;
                DrawViewbox();
                //all the enums
                selectedNPCTableEntry.PropertyChanged += currentNPCTableEntry_PropertyChanged;
                SetComboBoxValue(spriteSurfaceComboBox, surfaceDescriptors, selectedNPCTableEntry.SpriteSurface);
                SetComboBoxValue(hitSoundComboBox, soundEffects, selectedNPCTableEntry.HitSound);
                SetComboBoxValue(deathSoundComboBox, soundEffects, selectedNPCTableEntry.DeathSound);
                SetComboBoxValue(smokeSizeComboBox, smokeSizes, selectedNPCTableEntry.SmokeSize);

                //bits are down here because the event has been subscribed by this point
                SetBitsUI(selectedNPCTableEntry.Bits);
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
