using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LayeredPictureBox;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Linq;

namespace CaveStoryEditor
{
    partial class FormMain
    {
        Layer<Image> Hitbox, HitboxCenter;
        Layer<Image> LeftOffsetTriangle, LeftOffsetLine, RightOffsetTriangle, RightOffsetLine, YOffsetTriangle, YOffsetLine, ViewCenter;
        NPCTableEntry selectedNPCTableEntry { get; set; }

        bool lockNpcTableEntry = false;

        bool npcTableUnsaved = false;
        bool NPCTableUnsaved
        {
            get => npcTableUnsaved;
            set
            {
                if (value != npcTableUnsaved)
                {
                    if (!npcTableUnsaved)
                        npcTableTabPage.Text += "*";
                    else
                        npcTableTabPage.Text = npcTableTabPage.Text.Remove(npcTableTabPage.Text.Length - 1);
                    npcTableUnsaved = value;
                }
            }
        }

        #region bits
        void InitCheckboxList()
        {
            //TODO allow for custom flag names
            var names = Enum.GetNames(typeof(EntityFlags));
            bitsCheckedListBox.Items.Clear();
            foreach (var name in names)
                bitsCheckedListBox.Items.Add(name);
        }
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
                lockNpcTableEntry = false;
            }
        }
        private void BitChanged(object sender, PropertyChangedEventArgs e)
        {
            SetBitsUI((EntityFlags)sender.GetType().GetProperty(e.PropertyName).GetValue(sender));
        }
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
        
        #region viewbox
        void InitHitboxViewboxLayers()
        {            
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
            LeftOffsetTriangle.Image = MakeRightTriangle(Color.Green,-180);
            RightOffsetTriangle.Image = MakeRightTriangle(Color.Red, 90);
        }
        Bitmap MakeRightTriangle(Color c, float rotate = 0)
        {
            var tri = new Bitmap(MinBoxSize/2, MinBoxSize/2);
            using (var g = Graphics.FromImage(tri))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                if (rotate != 0)
                {
                    var w = (float)tri.Width / 2;
                    var h = (float)tri.Height / 2;
                    g.TranslateTransform(w,h);
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
        Bitmap MakeLine(int width, int height, Color c)
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

        #region init comboboxes/display
        void InitComboBoxDataSources()
        {
            void bind<T>(ComboBox cb, IDictionary<int, T> source)
            {
                var sorted = new SortedDictionary<int,T>(source);
                cb.Items.Clear();
                foreach (var item in sorted)
                    cb.Items.Add(item);

                UpdateComboBoxDropDownWidth(cb);
            }
            bind(spriteSurfaceComboBox, mod.SurfaceDescriptors);
            bind(hitSoundComboBox, mod.SoundEffects);
            bind(deathSoundComboBox, mod.SoundEffects);
            bind(smokeSizeComboBox, mod.SmokeSizes);            
        }
        void UpdateComboBoxDropDownWidth(ComboBox cb)
        {
            var biggest = 0;
            foreach (object item in cb.Items)
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
            }
            cb.DropDownWidth = biggest + SystemInformation.VerticalScrollBarWidth;
        }
        private void spriteSurfaceComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if(e.ListItem is KeyValuePair<int, ISurfaceSource> kvp && e.DesiredType == typeof(string))
            {
                e.Value = FormatKVP(kvp);
            }
        }
        private void hitSoundComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if(e.ListItem is KeyValuePair<int, string> kvp && e.DesiredType == typeof(string))
            {
                e.Value = FormatKVP(kvp);
            }
        }

        string FormatKVP<T>(KeyValuePair<int, T> kvp)
        {
            string @base = $"{kvp.Key} - ";
            if (typeof(T) == typeof(ISurfaceSource))
                @base += ((ISurfaceSource)kvp.Value).DisplayName;
            else if (typeof(T) == typeof(string))
                @base += kvp.Value.ToString();
            return @base;
        }
        #endregion

        #region selected npc -> combobox
        void SetComboBoxValue(ComboBox cb, IDictionary<int, ISurfaceSource> dict, int value)
        {
            //remove the extra entry
            if (cb.Items.Count > dict.Count)
                cb.Items.RemoveAt(cb.Items.Count - 1);

            if (dict?.ContainsKey(value) ?? false)
            {
                //select the item if it exists
                cb.SelectedItem = new KeyValuePair<int,ISurfaceSource>(value, dict[value]);
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

            if (dict?.ContainsKey(value) ?? false)
            {
                cb.SelectedItem = new KeyValuePair<int, string>(value, dict[value]);
            }
            else
            {
                cb.Items.Add(new KeyValuePair<int, string>(value, "???"));
                cb.SelectedIndex = cb.Items.Count - 1;
            }
        }

        /// <summary>
        /// Set the combobox to reflect the selected npc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EnumChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;
                var newVal = (byte)sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
                switch(e.PropertyName)
                {
                    case nameof(NPCTableEntry.SpriteSurface):
                        SetComboBoxValue(spriteSurfaceComboBox, mod.SurfaceDescriptors, newVal);
                        break;
                    case nameof(NPCTableEntry.HitSound):
                        SetComboBoxValue(hitSoundComboBox, mod.SoundEffects, newVal);
                        break;
                    case nameof(NPCTableEntry.DeathSound):
                        SetComboBoxValue(deathSoundComboBox, mod.SoundEffects, newVal);
                        break;
                    case nameof(NPCTableEntry.SmokeSize):
                        SetComboBoxValue(smokeSizeComboBox, mod.SmokeSizes, newVal);
                        break;
                }
                lockNpcTableEntry = false;                
            }
        }
        #endregion

        #region combobox -> selected npc
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

                lockNpcTableEntry = false;
            }
        }
        void SurfaceChanged(object sender, EventArgs e)
        {
            if (!lockNpcTableEntry)
            {
                lockNpcTableEntry = true;

                selectedNPCTableEntry.SpriteSurface = (byte)((KeyValuePair<int, ISurfaceSource>)((ComboBox)sender).SelectedItem).Key;

                if (spriteSurfaceComboBox.Items.Count > mod.SurfaceDescriptors.Count)
                    spriteSurfaceComboBox.Items.RemoveAt(spriteSurfaceComboBox.Items.Count - 1);

                lockNpcTableEntry = false;
            }
        }
        #endregion

        #region selected npc init/bindings

        void currentNPCTableEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(NPCTableEntry.SpriteSurface):
                case nameof(NPCTableEntry.HitSound):
                case nameof(NPCTableEntry.DeathSound):
                case nameof(NPCTableEntry.SmokeSize):
                    EnumChanged(sender, e);
                    break;
                case nameof(NPCTableEntry.Bits):
                    BitChanged(sender, e);
                    break;
            }
            NPCTableUnsaved = true;
        }

        private void UpdateUIBindings()
        {
            void bindNumericUpDown(NumericUpDown nud, string prop, object bs)
            {
                nud.DataBindings.Clear();
                //The OnPropertyChanged is very important for the hitbox/viewbox to look natural
                //other properties are not affected
                nud.DataBindings.Add(nameof(nud.Value), bs, prop, false, DataSourceUpdateMode.OnPropertyChanged);
            }

            var entrybs = new BindingSource(selectedNPCTableEntry, null);

            //Bits are done in a different function
            
            //Life
            bindNumericUpDown(lifeNumericUpDown, nameof(NPCTableEntry.Life), entrybs);

            //Sprite Surface, Hit, Death, Smoke
            //are all done in a different function

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
        }

        private void UpdateSelectedNPCTableEntry()
        {
            if (npcTableListBox.SelectedItem != selectedNPCTableEntry)
            {
                //un-hook the old entry
                if (selectedNPCTableEntry != null)
                {
                    selectedNPCTableEntry.Hitbox.PropertyChanged -= DrawHitboxEventHandler;
                    selectedNPCTableEntry.Viewbox.PropertyChanged -= DrawViewboxEventHandler;
                    selectedNPCTableEntry.PropertyChanged -= currentNPCTableEntry_PropertyChanged;
                }
                //set the new entry
                selectedNPCTableEntry = mod.NPCTable[npcTableListBox.SelectedIndex];
                npcTableEntryPropertyGrid.SelectedObject = selectedNPCTableEntry;
                //update all data bindings
                UpdateUIBindings();

                //update some of the hardcoded things
                //hitbox
                selectedNPCTableEntry.Hitbox.PropertyChanged += DrawHitboxEventHandler;
                DrawHitbox();
                //viewbox
                selectedNPCTableEntry.Viewbox.PropertyChanged += DrawViewboxEventHandler;
                DrawViewbox();
                //all the enums
                selectedNPCTableEntry.PropertyChanged += currentNPCTableEntry_PropertyChanged;
                SetComboBoxValue(spriteSurfaceComboBox, mod.SurfaceDescriptors, selectedNPCTableEntry.SpriteSurface);
                SetComboBoxValue(hitSoundComboBox, mod.SoundEffects, selectedNPCTableEntry.HitSound);
                SetComboBoxValue(deathSoundComboBox, mod.SoundEffects, selectedNPCTableEntry.DeathSound);
                SetComboBoxValue(smokeSizeComboBox, mod.SmokeSizes, selectedNPCTableEntry.SmokeSize);

                //bits are down here because the event has been subscribed by this point
                SetBitsUI(selectedNPCTableEntry.Bits);
            }
        }

        #endregion

        #region Hitbox/Viewbox rendering
        bool hitboxFacingRight = true;
        private void changeHitboxDirectionButton_Click(object sender, EventArgs e)
        {
            hitboxFacingRight = !hitboxFacingRight;
            changeHitboxDirectionButton.Text = hitboxFacingRight ? "--->" : "<---";
            DrawHitbox();
        }
        void DrawHitboxEventHandler(object sender, EventArgs e)
        {
            Invoke(new Action(DrawHitbox));
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
        void DrawViewboxEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != nameof(NPCViewRect.Unused))
                Invoke(new Action<string>(DrawViewbox),e.PropertyName);
        }
        const int MinBoxSize = 8;
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
            ViewCenter.Location = new Point(viewX + viewX/2, viewY + viewY/2);

            //reset all images
            if(prop == null)
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
                        if(YOffsetLine.Image.Width != (viewX+1)*2)
                            UpdateYImage();
                        break;
                }
            }
            //always update line locations
            UpdateYLocation();
            UpdateLLocation();
            UpdateRLocation();
            UpdateBoxLocation(viewboxContainerPanel,viewboxLayeredPictureBox);
        }
        void UpdateBoxLocation(ScrollableControl parent, Control child)
        {
            //TODO figure out why the hecc auto scroll doesn't work
            child.Location = new Point(parent.Width/2 - child.Width/2, parent.Height/2 - child.Height/2);
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

        #region refresh npc list
        readonly static MethodInfo refreshItemsMethodInfo = typeof(ListBox).GetMethod("RefreshItems",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
        null, Array.Empty<Type>(), null);
        //HACK need to refresh the entity list, and for whatever reason that method is private
        void npcTableListBox_RefreshItems()
        {
            refreshItemsMethodInfo.Invoke(npcTableListBox, Array.Empty<object>());
        }
        //HACK as if the other method wasn't bad enough...
        //the listbox doesn't seem to like refreshing its items when you remove things, so I have to just ignore the exception...?!
        //also put in the listboxCanUpdateSelection thing just for convienince
        void SafeRefreshItems()
        {
            //listboxCanUpdateSelection = false;
            try
            {
                npcTableListBox_RefreshItems();
            }
            catch (TargetInvocationException)
            {

            }
            //listboxCanUpdateSelection = true;
        }
        #endregion

        private void npcTableListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedNPCTableEntry();
        }

        #region add/insert/remove
        //TODO might actually remove this one, since it's never read from, can probably just set manually
        bool npcTableListEditingUIEnabled = false;
        bool NPCTableListEditingUIEnabled
        {
            get => npcTableListEditingUIEnabled;
            set
            {
                addNPCTableEntryButton.Enabled = value;
                insertNPCTableEntryButton.Enabled = value;
                removeNPCTableEntryButton.Enabled = value;

                npcTableListEditingUIEnabled = value;
            }
        }
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

        private void SelectNPCTableEntry(int index)
        {
            if (0 <= index && index < mod.NPCTable.Count)
            {
                if (!NPCTableEntryUIEnabled)
                {
                    NPCTableEntryUIEnabled = true;
                    removeNPCTableEntryButton.Enabled = true;
                }
                npcTableListBox.SelectedIndex = index;
            }
            else
            {
                NPCTableEntryUIEnabled = false;
                removeNPCTableEntryButton.Enabled = false;
                npcTableEntryPropertyGrid.SelectedObject = null;
            }
        }
        private void addNPCTableEntryButton_Click(object sender, EventArgs e)
        {
            mod.NPCTable.Add(new NPCTableEntry());
            SafeRefreshItems();
            SelectNPCTableEntry(mod.NPCTable.Count - 1);
            NPCTableUnsaved = true;
        }

        private void insertNPCTableEntryButton_Click(object sender, EventArgs e)
        {
            var index = Math.Max(0, npcTableListBox.SelectedIndex);
            var entry = new NPCTableEntry();
            mod.NPCTable.Insert(index, entry);
            SafeRefreshItems();
            SelectNPCTableEntry(index);
            NPCTableUnsaved = true;
        }

        private void removeNPCTableEntryButton_Click(object sender, EventArgs e)
        {
            var index = npcTableListBox.SelectedIndex;
            mod.NPCTable.RemoveAt(index);
            SafeRefreshItems();
            SelectNPCTableEntry(Math.Max(index - 1, 0));
            NPCTableUnsaved = true;
        }
        #endregion

        private void npcTableListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is NPCTableEntry te && e.DesiredType == typeof(string))
            {
                var index = mod.NPCTable.IndexOf(te);
                var val = $"{index} - ";
                if (mod.EntityInfos.TryGetValue(index, out EntityInfo ei))
                    val += ei.Name;
                e.Value = val;
            }
        }

        #region saving
        private void saveNPCTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NPCTable.Save(mod.NPCTable, Path.Combine(mod.DataFolderPath, NPCTable.NPCTBL));
            NPCTableUnsaved = false;
        }

        private void exportNPCTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Title = "Choose a location...",
                Filter = string.Join("|", NPCTable.NPCTBLFilter, "All Files (*.*)|*.*")
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    NPCTable.Save(mod.NPCTable, sfd.FileName);
                }
            }
        }
        #endregion
    }
}
