using CaveStoryModdingFramework;
using CaveStoryModdingFramework.TSC;
using System;
using System.IO;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class FormScriptEditor : Form
    {
        readonly Mod parentMod;
        public bool IsInStageFolder
        {
            get
            {
                //HACK not the most efficient way to check if a file is in a folder...
                var folder = Path.Combine(parentMod.DataFolderPath, parentMod.StageFolderPath);
                var s = Fullpath;
                do
                {
                    if (folder == (s = Path.GetDirectoryName(s)))
                    {
                        return true;
                    }
                } while (s.Length > 0);
                return false;
            }
        }
        public string Fullpath { get; private set; }
        public FormScriptEditor(Mod m, string path)
        {
            parentMod = m;
            Fullpath = path;

            Text = Path.GetFileName(Fullpath);

            InitializeComponent();

            if (File.Exists(path))
            {
                byte[] input = File.ReadAllBytes(Fullpath);
                if (parentMod.TSCEncrypted)
                    Encryptor.DecryptInPlace(input, parentMod.DefaultKey);
                mainScintilla.Text = parentMod.TSCEncoding.GetString(input);
            }
        }

        public event EventHandler ScriptSaved;

        void Save(string path)
        {
            var bytes = parentMod.TSCEncoding.GetBytes(mainScintilla.Text);
            if (parentMod.TSCEncrypted)
                Encryptor.EncryptInPlace(bytes);
            File.WriteAllBytes(path, bytes);

            ScriptSaved?.Invoke(this, new EventArgs());
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(Fullpath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Filter = string.Join("|", $"TSC Files (*.{parentMod.TSCExtension})|*.{parentMod.TSCExtension}", FormMain.AllFilesFilter)
            })
            {
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    Save(sfd.FileName);                    
                }
            }            
        }
    }
}
