using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework.TSC;

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
            manager = new EditorManager(mod,cache);

            mod.ImageExtensionChanged += Mod_ImageExtensionChanged;
            mod.TSCExtensionChanged += Mod_TSCExtensionChanged;
            
            modPropertyGrid.SelectedObject = mod;

            InitCheckboxList();
            InitComboBoxDataSources();

            //tool strip menu buttons
            saveProjectToolStripMenuItem.Enabled = true;
            saveProjectAsToolStripMenuItem.Enabled = true;
            loadEntityInfotxtToolStripMenuItem.Enabled = true;

            //stage table
            stageTableBinding = new BindingSource(new BindingList<StageEntry>(mod.StageTable), null)
            {

            };
            InitStageTableColumns();
            stageTableDataGridView.DataSource = stageTableBinding;

            FillListbox(pxmListBox, mod.StageExtension);
            FillListbox(pxeListBox, mod.EntityExtension);
            FillListbox(imageListBox, mod.ImageExtension);
            FillListbox(scriptListBox, mod.TSCExtension);
            FillListbox(attributeListBox, mod.AttributeExtension);

            //stage table
            saveStageTableToolStripMenuItem.Enabled = true;
            exportStageTableToolStripMenuItem.Enabled = true;

            //npc table
            npcTableListBox.DataSource = mod.NPCTable;
            NPCTableListEditingUIEnabled = true;
            NPCTableEntryUIEnabled = true;

            saveNPCTableToolStripMenuItem.Enabled = true;
            exportNPCTableToolStripMenuItem.Enabled = true;

            InitScriptWatcher();
            InitImageWatcher();
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
        bool LoadWarning()
        {
            //TODO localize
            return mod == null || MessageBox.Show("You already have a mod loaded! Loading another will clear all unsaved changes!\n"+
                "Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    Init();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!LoadWarning())
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
                var inText = File.ReadAllBytes(inPath);
                var outText = Encryptor.Encrypt(inText, mod.DefaultKey);
                File.WriteAllBytes(outPath, outText);
            }
        }

        private void Decrypt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach(string item in scriptListBox.SelectedItems)
            {
                GetTXTAndTSC(item, out string outPath, out string inPath);

                var inText = File.ReadAllBytes(inPath);
                var outText = Encryptor.Decrypt(inText, mod.DefaultKey);
                File.WriteAllBytes(outPath, outText);
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

        StageEntry selectedEntry => mod.StageTable[stageTableDataGridView.SelectedRows[0].Index];

        private void openTilesButton_Click(object sender, EventArgs e)
        {
            manager.OpenTileEditor(selectedEntry);
        }

        private void openScriptButton_Click(object sender, EventArgs e)
        {
            manager.OpenScriptEditor(Path.Combine(mod.DataFolderPath, mod.StageFolderPath, selectedEntry.Filename + "." + mod.TSCExtension));
        }

        private void openBothButton_Click(object sender, EventArgs e)
        {
            manager.OpenTileEditor(selectedEntry);
            manager.OpenScriptEditor(Path.Combine(mod.DataFolderPath, mod.StageFolderPath, selectedEntry.Filename + "." + mod.TSCExtension));
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

        private void onImageChanged(object sender, FileSystemEventArgs e)
        {
            if (mod.CopyrightText.Length > 0)
                Images.UpdateCopyright(e.FullPath, mod.CopyrightText);
        }
    }
}
