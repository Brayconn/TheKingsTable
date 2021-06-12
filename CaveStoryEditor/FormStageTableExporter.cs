using CaveStoryModdingFramework.Stages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class FormStageTableExporter : Form
    {
        IList<StageEntry> Table { get; }
        StageEntrySettings ExportSettings { get; set; }
        string ExportPath
        {
            get => pathTextBox.Text;
            set
            {
                if (ExportPath != value)
                {
                    pathTextBox.Text = value;
                }
            }
        }
        StageTablePresets exportType = StageTablePresets.doukutsuexe;
        StageTablePresets ExportType
        {
            get => exportType;
            set
            {
                if(ExportType != value)
                {
                    exportType = value;
                    UpdateExportButton();
                }
            }
        }
        public FormStageTableExporter(IList<StageEntry> table)
        {
            Table = table;
            InitializeComponent();

            typeComboBox.Items.AddRange(Enum.GetNames(typeof(StageTablePresets)));
            typeComboBox.SelectedIndex = 0;

            resetButton_Click(this, new EventArgs());
        }

        void UpdateExportButton()
        {
            bool pathOk = !string.IsNullOrWhiteSpace(ExportPath);
            bool typeOk;
            switch(ExportType)
            {
                case StageTablePresets.doukutsuexe:
                case StageTablePresets.swdata:
                case StageTablePresets.csmap:
                    typeOk = File.Exists(ExportPath);
                    break;
                case StageTablePresets.stagetbl:
                case StageTablePresets.mrmapbin:
                    typeOk = true;
                    break;
                default:
                    typeOk = false;
                    break;
            }
            exportButton.Enabled = pathOk && typeOk;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExportType = (StageTablePresets)typeComboBox.SelectedIndex;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ExportSettings = new StageEntrySettings(ExportType);
            entrySettingsPropertyGrid.SelectedObject = ExportSettings;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("THIS IS DUMMIED RIGHT NOW");
                //StageTable.Save(Table, ExportPath, ExportType, ExportSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void pathButton_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Title = "Pick a save location...",
                Filter = string.Join("|", StageTable.MRMAPFilter, StageTable.STAGETBLFilter, StageTable.CSFilter, StageTable.EXEFilter)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportPath = sfd.FileName;
                }
            }
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateExportButton();
        }
    }
}
