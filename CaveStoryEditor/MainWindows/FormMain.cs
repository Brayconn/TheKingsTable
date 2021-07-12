using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework.TSC;
using CaveStoryModdingFramework.Entities;

namespace CaveStoryEditor
{
    public partial class FormMain : Form
    {
        Mod mod;
        BindingSource stageTableBinding;
        FileSystemWatcher imageWatcher, scriptWatcher; //TODO use script watcher to tell any open script editors to update, and maybe to tell any open tile editors to update state?
        EditorManager manager;
        SpriteCache cache;

        public const string AllFilesFilter = "All Files (*.*)|*.*";

        public FormMain()
        {
            InitializeComponent();
            stageTableDataGridView.AutoGenerateColumns = false;

            
        }

        public void Init()
        {
            cache = new SpriteCache(mod);
            manager?.Clear();
            manager = new EditorManager(mod,cache);

            mod.ImageExtensionChanged += Mod_ImageExtensionChanged;
            mod.TSCExtensionChanged += Mod_TSCExtensionChanged;
            mod.StageTableTypeChanged += Mod_StageTableTypeChanged;

            modPropertyGrid.SelectedObject = mod;
                       

            StageTableUnsaved = false;
            

            //tool strip menu buttons
            saveProjectToolStripMenuItem.Enabled = true;
            saveProjectAsToolStripMenuItem.Enabled = true;
            loadEntityInfotxtToolStripMenuItem.Enabled = true;
            generateFlagListingToolStripMenuItem.Enabled = true;

            //stage table
            stageTableBinding = new BindingSource(new BindingList<StageEntry>(mod.StageTable), null)
            {
                
            };
            InitStageTableColumns();
            stageTableDataGridView.DataSource = stageTableBinding;

            //setup stage table type quick switcher
            if(stageTableFormatComboBox.DataSource == null)
                stageTableFormatComboBox.DataSource = Enum.GetValues(typeof(StageTablePresets));
            stageTableFormatComboBox.Enabled = true;
            lockMod = true;
            stageTableFormatComboBox.SelectedItem = mod.StageTablePreset;
            lockMod = false;

            //asset tab
            FillListbox(pxmListBox, SearchLocations.Stage, Prefixes.None, Extension.TileData);
            FillListbox(pxeListBox, SearchLocations.Stage, Prefixes.None, Extension.EntityData);
            FillImagesListbox();
            FillScriptListBox();
            FillListbox(attributeListBox, SearchLocations.Stage, Prefixes.None, Extension.TilesetData);

            //Menu buttons
            saveStageTableToolStripMenuItem.Enabled = true;
            exportStageTableToolStripMenuItem.Enabled = true;
            UpdateCanAddStageTableEntries();

            //npc table
            npcTableEditor.UnsavedChangesChanged += NpcTableEditor_UnsavedChangesChanged;
            npcTableEditor.InitMod(mod);
            npcTableEditor.LoadTable(mod.NPCTable);

            saveNPCTableToolStripMenuItem.Enabled = true;
            exportNPCTableToolStripMenuItem.Enabled = true;

            //bullet table
            bulletTableEditor1.UnsavedChangesChanged += BulletTableEditor1_UnsavedChangesChanged;
            bulletTableEditor1.InitMod(mod);
            bulletTableEditor1.LoadTable(mod.BulletTable);

            saveBulletTableToolStripMenuItem.Enabled = true;
            exportBulletTableToolStripMenuItem.Enabled = true;
            
            InitScriptWatcher();
            InitImageWatcher();
        }

        private void BulletTableEditor1_UnsavedChangesChanged(object sender, EventArgs e)
        {
            if (bulletTableEditor1.HasUnsavedChanges)
                bulletTableTabPage.Text += "*";
            else
                bulletTableTabPage.Text = bulletTableTabPage.Text.Remove(bulletTableTabPage.Text.Length - 1);
        }

        private void NpcTableEditor_UnsavedChangesChanged(object sender, EventArgs e)
        {
            if (npcTableEditor.HasUnsavedChanges)
                npcTableTabPage.Text += "*";
            else
                npcTableTabPage.Text = npcTableTabPage.Text.Remove(npcTableTabPage.Text.Length - 1);
        }

        bool lockMod = false;
        private void stageTableFormatComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!lockMod)
            {
                lockMod = true;
                
                mod.StageTablePreset = (StageTablePresets)stageTableFormatComboBox.SelectedItem;
                UpdateCanAddStageTableEntries();
                
                lockMod = false;
            }
        }

        private void Mod_StageTableTypeChanged(object sender, EventArgs e)
        {
            if (!lockMod)
            {
                lockMod = true;

                stageTableFormatComboBox.SelectedItem = mod.StageTablePreset;
                UpdateCanAddStageTableEntries();
                
                lockMod = false;
            }
        }

        private void Mod_TSCExtensionChanged(object sender, EventArgs e)
        {
            FillScriptListBox();
            DestroyScriptWatcher();
        }

        void DestroyScriptWatcher()
        {
            if(scriptWatcher != null)
            {
                scriptWatcher.Changed -= manager.onScriptChanged;
                scriptWatcher.Dispose();
            }
        }
        private void InitScriptWatcher()
        {
            scriptWatcher = new FileSystemWatcher(mod.BaseDataPath, "*." + mod.TSCExtension)
            {
                IncludeSubdirectories = true,
            };
            scriptWatcher.Changed += manager.onScriptChanged;
            scriptWatcher.EnableRaisingEvents = true;
        }

        #region Project files
        string savePath = null;
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SafeToClose())
                return;

            using (var ofd = new OpenFileDialog()
            {
                Title = "Select your game...",
                Filter = string.Join("|", StageTable.CSFilter, StageTable.MRMAPFilter, StageTable.STAGETBLFilter, StageTable.EXEFilter)
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string data;
                    string stage = ofd.FileName;
                    StageTablePresets type;
                    switch(ofd.FilterIndex)
                    {
                        case 4: //EXE filter
                        case 1: //CS
                            if(!StageTable.TryDetectTableType(ofd.FileName, out type))
                            {
                                MessageBox.Show("Error detecting stage table type"); //TODO
                                return;
                            }
                            data = Path.Combine(Path.GetDirectoryName(ofd.FileName), "data");
                            break;
                        case 2: //CSE2
                            data = Path.GetDirectoryName(ofd.FileName);
                            type = StageTablePresets.mrmapbin;
                            break;
                        case 3: //CS+
                            data = Path.GetDirectoryName(ofd.FileName);
                            type = StageTablePresets.stagetbl;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                    mod = new Mod(data, stage, type);
                    savePath = null;
                    Init();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SafeToClose())
                return;

            using (var ofd = new OpenFileDialog()
            {
                Filter = string.Join("|", Mod.CaveStoryProjectFilter, AllFilesFilter)
            })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    mod = new Mod(savePath = ofd.FileName);
                    Init();
                }
            }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savePath != null)
                mod.Save(savePath);
            else
                saveProjectAsToolStripMenuItem_Click(sender, e);
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Filter = string.Join("|", Mod.CaveStoryProjectFilter, AllFilesFilter)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    mod.Save(savePath = sfd.FileName);
                }
            }
        }

        #endregion

        void FillListbox(ListBox l, SearchLocations folder, Prefixes prefix, Extension extension, string baseOverride = null)
        {
            string @base = baseOverride;
            if (baseOverride == null)
            {
                mod.FolderPaths.TryGetList(folder, out var list);
                @base = list.Count == 1 ? mod.FolderPaths.MakeAbsoluteFromBase(list[0]) : mod.BaseDataPath;
            }

            foreach(var file in mod.FolderPaths.EnumerateFiles(folder, prefix, extension))
            {
                l.Items.Add(AssetManager.MakeRelative(@base, file));
            }
        }
        void FillImagesListbox()
        {
            imageListBox.Items.Clear();
            FillListbox(imageListBox, SearchLocations.Data, Prefixes.None, Extension.Image);
            FillListbox(imageListBox, SearchLocations.Npc, Prefixes.None, Extension.Image);
            FillListbox(imageListBox, SearchLocations.Stage, Prefixes.None, Extension.Image);
        }
        void FillScriptListBox()
        {
            scriptListBox.Items.Clear();
            FillListbox(scriptListBox, SearchLocations.Data, Prefixes.None, Extension.Script, mod.BaseDataPath);
            FillListbox(scriptListBox, SearchLocations.Stage, Prefixes.None, Extension.Script, mod.BaseDataPath);
        }

        private void Mod_ImageExtensionChanged(object sender, EventArgs e)
        {
            cache.GenerateGlobal(true);

            FillImagesListbox();

            DestroyImageWatcher();
            if (mod.CopyrightText.Length > 0)
            {
                InitImageWatcher();
            }
        }
        void DestroyImageWatcher()
        {
            if (imageWatcher != null)
            {
                imageWatcher.Changed -= onImageChanged;
                imageWatcher.Dispose();
            }
        }
        void InitImageWatcher()
        {
            DestroyImageWatcher();
            imageWatcher = new FileSystemWatcher(mod.BaseDataPath, "*." + mod.ImageExtension)
            {
                IncludeSubdirectories = true,
            };
            imageWatcher.Changed += onImageChanged;
            imageWatcher.EnableRaisingEvents = true;
        }

        private void imageListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var imagesCMS = new ContextMenuStrip();
                var enabled = imageListBox.SelectedIndices.Count > 0;

                var update = new ToolStripMenuItem("Update copyright");
                update.Click += Update_Click;
                update.Enabled = enabled;

                imagesCMS.Items.Add(update);

                imagesCMS.Show(imageListBox, e.Location);
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            foreach(string item in imageListBox.SelectedItems)
            {
                string path = mod.FolderPaths.MakeAbsoluteFromBase(item);
                try
                {
                    bool success = Images.UpdateCopyright(path);
                    MessageBox.Show(success ? $"Copyright updated on {item}!" : $"Copyright already up to date on {item}!");
                }
                catch (IOException)
                {
                    MessageBox.Show($"Couldn't open {item}!");
                }                
            }
        }

        private void scriptListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var scriptCMS = new ContextMenuStrip();
                var enabled = scriptListBox.SelectedIndices.Count > 0;

                var open = new ToolStripMenuItem("Open...");
                open.Enabled = enabled;
                open.Click += Open_Click;
                scriptCMS.Items.Add(open);

                var decrypt = new ToolStripMenuItem("Decrypt to txt");
                decrypt.Enabled = enabled;
                decrypt.Click += Decrypt_Click;
                scriptCMS.Items.Add(decrypt);

                var encrypt = new ToolStripMenuItem("Encrypt txt");
                encrypt.Enabled = enabled;
                encrypt.Click += Encrypt_Click;
                scriptCMS.Items.Add(encrypt);

                scriptCMS.Show(scriptListBox, e.Location);
            }
        }

        #region TSC encryption/decryption
        void GetTXTAndTSC(string file, out string txtPath, out string tscPath)
        {
            var @base = mod.FolderPaths.MakeAbsoluteFromBase(file);
            txtPath = Path.ChangeExtension(@base, "txt");
            tscPath = Path.ChangeExtension(@base, "tsc");
        }
        private void Encrypt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach(string item in scriptListBox.SelectedItems)
            {
                GetTXTAndTSC(item, out string inPath, out string outPath);
                if (File.Exists(inPath))
                {
                    var inText = File.ReadAllBytes(inPath);
                    var outText = Encryptor.Encrypt(inText, mod.DefaultKey);
                    File.WriteAllBytes(outPath, outText);
                }
            }
        }

        private void Decrypt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach(string item in scriptListBox.SelectedItems)
            {
                GetTXTAndTSC(item, out string outPath, out string inPath);
                if (File.Exists(inPath))
                {
                    var inText = File.ReadAllBytes(inPath);
                    var outText = Encryptor.Decrypt(inText, mod.DefaultKey);
                    File.WriteAllBytes(outPath, outText);
                }
            }
        }
        #endregion

        private void Open_Click(object sender, EventArgs e)
        {
            foreach(string item in scriptListBox.SelectedItems)
            {
                manager.OpenScriptEditor(mod.FolderPaths.MakeAbsoluteFromBase(item), mod.TSCEncrypted, mod.UseScriptSource);
            }
        }

        private void attributeListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var attributeCMS = new ContextMenuStrip();
                var enabled = attributeListBox.SelectedIndices.Count > 0;

                var open = new ToolStripMenuItem("Open...");
                open.Enabled = enabled;
                open.Click += OpenAttribute_Click;

                attributeCMS.Items.Add(open);

                attributeCMS.Show(attributeListBox, e.Location);
            }
        }

        private void OpenAttribute_Click(object sender, EventArgs e)
        {
            manager.OpenAttributeFile(mod.FolderPaths.MakeAbsoluteFromBase(attributeListBox.SelectedItem.ToString()));
        }

        StageEntry selectedStageTableEntry => mod.StageTable[stageTableDataGridView.SelectedRows[0].Index];

        private void openTilesButton_Click(object sender, EventArgs e)
        {
            manager.OpenTileEditor(selectedStageTableEntry);
        }

        private void openScriptButton_Click(object sender, EventArgs e)
        {
            manager.OpenScriptEditor(selectedStageTableEntry);
        }

        private void openBothButton_Click(object sender, EventArgs e)
        {
            manager.OpenTileEditor(selectedStageTableEntry);
            manager.OpenScriptEditor(selectedStageTableEntry);
        }

        private void loadEntityInfotxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()
            {
                Filter = string.Join("|", "EntityInfo.txt|EntityInfo.txt", AllFilesFilter)
            })
            {
                if(ofd.ShowDialog() == DialogResult.OK && MessageBox.Show("This will overwrite ALL entity names/rects/categories etc.\nAre you sure you want to continue?","Warning",MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    mod.EntityInfos.Clear();
                    foreach(var entry in EntityInfoTXT.Load(ofd.FileName))
                    {
                        mod.EntityInfos.Add(entry.Key, entry.Value);
                    }
                }
            }
        }

        bool SafeToClose()
        {
            //TODO check for changes to the project file
            //It's only safe to close if...
            //there's no mod loaded
            return mod == null
                //there's no unsaved changes in any editor
                || (!StageTableUnsaved && !npcTableEditor.HasUnsavedChanges && !manager.UnsavedChanges) 
                //or the user says it's ok
                || MessageBox.Show("You have unsaved changes! Are you sure you want to continue?",
                    "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !SafeToClose();
        }

        private void generateFlagListingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //init
            string savePath = "";
            using(var sfd = new SaveFileDialog()
            {
                Filter = string.Join("|", "Text Files (*.txt)|*.txt", AllFilesFilter)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    savePath = sfd.FileName;
                }
                else return;
            }
            var flagList = new SortedDictionary<int, List<string>>();

            //add one flag entry to the list
            void AddFlag(int flag, string text)
            {
                if (!flagList.ContainsKey(flag))
                    flagList.Add(flag, new List<string>() { text });
                else
                    flagList[flag].Add(text);
            }

            //add flags from a TSC file
            void AddTSC(string tscPath, bool credits = false)
            {
                byte[] input = File.ReadAllBytes(tscPath);
                if (mod.TSCEncrypted)
                    Encryptor.DecryptInPlace(input, mod.DefaultKey);
                var text = mod.TSCEncoding.GetString(input);

                if (!credits)
                {
                    for (var index = text.IndexOf('<', 0); index != -1; index = text.IndexOf('<', index + 1))
                    {
                        var eve = text.Substring(text.LastIndexOf('#', index), 5);
                        var cmd = text.Substring(index, 4);
                        switch (cmd)
                        {
                            case "<FL+":
                            case "<FL-":
                            case "<FLJ":
                                AddFlag(FlagConverter.FlagToRealValue(text.Substring(index+4,4),4),
                                    $"{cmd} {Path.GetFileName(tscPath)} event {eve}");
                                break;
                        }
                    }
                }
                else
                {
                    var curreve = "N/A";
                    for(var index = 0; index < text.Length; index++)
                    {
                        switch(text[index])
                        {
                            //skip text
                            case '[':
                                index = text.IndexOf(']', index);
                                //HACK I hate how this breaks the nice flow the rest of the code has
                                if (index == -1)
                                    return;
                                break;
                            //l == # in credits
                            case 'l':
                                var l = text.Substring(index + 1, 4);
                                if (char.IsDigit(l[0]) && char.IsDigit(l[1]) && char.IsDigit(l[2]) && char.IsDigit(l[3]))
                                    curreve = l;
                                index += 4;
                                break;
                            //f == FLJ in credits
                            case 'f':
                                AddFlag(FlagConverter.FlagToRealValue(text.Substring(index+1,4), 4),$"<FLJ {Path.GetFileName(tscPath)} event #{curreve}");
                                index += 4;
                                break;
                        }
                    }
                }
            }
            
            //Add flags from a PXE file
            void AddPXE(string pxePath, EntityFlags filter)
            {
                var pxe = PXE.Read(pxePath);
                for (var i = 0; i < pxe.Count; i++)
                {
                    var filtered = pxe[i].Bits & filter;
                    if (pxe[i].Flag != 0 && filtered != 0)
                        AddFlag(pxe[i].Flag, $"{filtered} {Path.GetFileName(pxePath)} entity {i} ({mod.EntityInfos[pxe[i].Type].Name})");
                }
            }

            //global tsc files
            foreach (var tsc in mod.FolderPaths.EnumerateFiles(SearchLocations.Data, Extension.Script))
                AddTSC(tsc, tsc.Contains("Credit"));

            //stage table
            foreach(var entry in mod.StageTable)
            {
                if(mod.FolderPaths.TryGetFile(SearchLocations.Stage, entry.Filename, Extension.Script, out string tscPath))
                    AddTSC(tscPath);

                if(mod.FolderPaths.TryGetFile(SearchLocations.Stage, entry.Filename, Extension.EntityData, out string pxePath))
                    AddPXE(pxePath, EntityFlags.AppearWhenFlagSet | EntityFlags.HideWhenFlagSet);
            }

            //save the file
            using (var sw = new StreamWriter(savePath))
            {
                foreach (var evnt in flagList)
                {
                    sw.WriteLine("Flag " + evnt.Key);
                    foreach(var str in evnt.Value)
                    {
                        sw.WriteLine("\t" + str);
                    }
                }
            }
        }

        private void loadTsclisttxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var ofd = new OpenFileDialog()
            {
                Filter = string.Join("|", "tsc_list.txt|tsc_list.txt", AllFilesFilter),
            })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    mod.Commands = CaveStoryModdingFramework.Compatability.TSCListTXT.Load(ofd.FileName);
                }
            }
        }

        private void saveNPCTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            npcTableEditor.Save(mod.NpcTablePath);
        }

        private void exportNPCTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Title = "Choose a location...",
                Filter = string.Join("|", NPCTable.NPCTBLFilter, AllFilesFilter)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    npcTableEditor.Save(sfd.FileName);
                }
            }
        }

        private void saveBulletTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulletTable.Write(mod.BulletTable, mod.BulletTableLocation);
        }

        private void exportBulletTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var sfd = new SaveFileDialog()
            {
                Title = "Choose a location...",
                Filter = string.Join("|", BulletTable.BulletTableFilter, AllFilesFilter)
            })
            {
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    //HACK not that great that I'm creating a new stage table location hardcoded to one mode
                    BulletTable.Write(mod.BulletTable, new BulletTableLocation(sfd.FileName, BulletTablePresets.csplus));
                }
            }
        }

        private void onImageChanged(object sender, FileSystemEventArgs e)
        {
            if (mod.CopyrightText.Length > 0)
                Images.UpdateCopyright(e.FullPath, mod.CopyrightText);
        }
    }
}
