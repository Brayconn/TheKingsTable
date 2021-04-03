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

            InitHitboxViewboxLayers();
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

            InitCheckboxList();
            InitComboBoxDataSources();

            StageTableUnsaved = false;
            NPCTableUnsaved = false;

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
                stageTableFormatComboBox.DataSource = Enum.GetValues(typeof(StageTableTypes));
            stageTableFormatComboBox.Enabled = true;
            lockMod = true;
            stageTableFormatComboBox.SelectedItem = mod.StageTableFormat;
            lockMod = false;

            //asset tab
            FillListbox(pxmListBox, mod.StageExtension);
            FillListbox(pxeListBox, mod.EntityExtension);
            FillListbox(imageListBox, mod.ImageExtension);
            FillListbox(scriptListBox, mod.TSCExtension);
            FillListbox(attributeListBox, mod.AttributeExtension);

            //Menu buttons
            saveStageTableToolStripMenuItem.Enabled = true;
            exportStageTableToolStripMenuItem.Enabled = true;
            UpdateCanAddStageTableEntries();

            //npc table
            npcTableListBox.DataSource = mod.NPCTable;
            NPCTableListEditingUIEnabled = true;
            NPCTableEntryUIEnabled = true;

            saveNPCTableToolStripMenuItem.Enabled = true;
            exportNPCTableToolStripMenuItem.Enabled = true;

            InitScriptWatcher();
            InitImageWatcher();
        }

        bool lockMod = false;
        private void stageTableFormatComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!lockMod)
            {
                lockMod = true;
                
                mod.StageTableFormat = (StageTableTypes)stageTableFormatComboBox.SelectedItem;
                mod.StageTableSettings.Reset(mod.StageTableFormat);
                UpdateCanAddStageTableEntries();
                
                lockMod = false;
            }
        }

        private void Mod_StageTableTypeChanged(object sender, EventArgs e)
        {
            if (!lockMod)
            {
                lockMod = true;

                stageTableFormatComboBox.SelectedItem = mod.StageTableFormat;
                UpdateCanAddStageTableEntries();
                
                lockMod = false;
            }
        }

        private void Mod_TSCExtensionChanged(object sender, EventArgs e)
        {
            FillListbox(scriptListBox, mod.TSCExtension);
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
            scriptWatcher = new FileSystemWatcher(mod.DataFolderPath, "*." + mod.TSCExtension)
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
                    StageTableTypes type;
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
                            type = StageTableTypes.mrmapbin;
                            break;
                        case 3: //CS+
                            data = Path.GetDirectoryName(ofd.FileName);
                            type = StageTableTypes.stagetbl;
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

        void FillListbox(ListBox l, string filter)
        {
            l.Items.Clear();
            foreach(var file in Directory.EnumerateFiles(mod.DataFolderPath, "*." + filter, SearchOption.AllDirectories))
            {
                //HACK that remove might be unsafe with different seperators?
                l.Items.Add(file.Substring((mod.DataFolderPath + Path.DirectorySeparatorChar).Length));
            }
        }

        private void Mod_ImageExtensionChanged(object sender, EventArgs e)
        {
            cache.GenerateGlobal(true);
            FillListbox(imageListBox, mod.ImageExtension);
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
            imageWatcher = new FileSystemWatcher(mod.DataFolderPath, "*." + mod.ImageExtension)
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
                string path = Path.Combine(mod.DataFolderPath, item);
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

        void GetTXTAndTSC(string file, out string txtPath, out string tscPath)
        {
            var @base = Path.Combine(mod.DataFolderPath, file);
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

        private void Open_Click(object sender, EventArgs e)
        {
            foreach(string item in scriptListBox.SelectedItems)
            {
                manager.OpenScriptEditor(Path.Combine(mod.DataFolderPath, item));
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
            manager.OpenAttributeFile(Path.Combine(mod.DataFolderPath, attributeListBox.SelectedItem.ToString()));
        }

        StageEntry selectedStageTableEntry => mod.StageTable[stageTableDataGridView.SelectedRows[0].Index];

        private void openTilesButton_Click(object sender, EventArgs e)
        {
            manager.OpenTileEditor(selectedStageTableEntry);
        }

        private void openScriptButton_Click(object sender, EventArgs e)
        {
            manager.OpenScriptEditor(Path.Combine(mod.DataFolderPath, mod.ScriptFolderPath, selectedStageTableEntry.Filename + "." + mod.TSCExtension));
        }

        private void openBothButton_Click(object sender, EventArgs e)
        {
            manager.OpenTileEditor(selectedStageTableEntry);
            manager.OpenScriptEditor(Path.Combine(mod.DataFolderPath, mod.ScriptFolderPath, selectedStageTableEntry.Filename + "." + mod.TSCExtension));
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
            return mod == null || (!StageTableUnsaved && !NPCTableUnsaved && !manager.UnsavedChanges) 
                || MessageBox.Show("You have unsaved changes! Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
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
                                AddFlag(FlagConverter.FlagToRealValue(text.Substring(index+4,4)),
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
                                AddFlag(FlagConverter.FlagToRealValue(text.Substring(index+1,4)),$"<FLJ {Path.GetFileName(tscPath)} event #{curreve}");
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
            foreach (var tsc in Directory.EnumerateFiles(mod.DataFolderPath, "*." + mod.TSCExtension))
                AddTSC(tsc, tsc.Contains("Credit"));

            //stage table
            foreach(var entry in mod.StageTable)
            {
                var tscPath = Path.Combine(mod.DataFolderPath, mod.ScriptFolderPath, entry.Filename + "." + mod.TSCExtension);
                AddTSC(tscPath);

                var pxePath = Path.Combine(mod.DataFolderPath, mod.StageFolderPath, entry.Filename + "." + mod.EntityExtension);
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
                    mod.Commands.Add(new Command()
                    {
                        ShortName = "<RNJ",
                        LongName = "RaNdom Jump",
                        Description = "Jumps to a random event",
                        Arguments = new List<object>()
                        {
                            new Argument()
                            {
                                Name = "Event count"
                            },
                            new RepeatStructure()
                            {
                                RepeatType = RepeatTypes.GlobalIndex,
                                Value = 0,
                                Arguments = new List<object>()
                                {
                                    new Argument()
                                    {
                                        Type = ArgumentTypes.Event
                                    }
                                }
                            }
                        }
                    });
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
