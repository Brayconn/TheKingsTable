using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Stages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class FormStageTableExporter : Form
    {
        IList<StageEntry> Table { get; }
        private class Container
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public StageTableLocation Location { get; } = new StageTableLocation("");
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public StageEntrySettings Settings { get; } = new StageEntrySettings();
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public StageTableReferences References { get; } = new StageTableReferences();
        }
        Container Settings { get; } = new Container();

        public FormStageTableExporter(IList<StageEntry> table)
        {
            Table = table;
            InitializeComponent();

            foreach(StageTablePresets item in Enum.GetValues(typeof(StageTablePresets)))
            {
                if (item != StageTablePresets.custom)
                    typeComboBox.Items.Add(item);
            }
            typeComboBox.SelectedIndex = 0;

            Settings.Location.PropertyChanged += Location_PropertyChanged;

            resetButton_Click(this, new EventArgs());
        }

        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DataLocation.Filename):
                    if (!lockTextBox)
                    {
                        lockTextBox = true;

                        pathTextBox.Text = Settings.Location.Filename;
                        entrySettingsPropertyGrid.SelectedObject = Settings;
                        UpdateExportButton();

                        lockTextBox = false;
                    }
                    break;
            }
        }

        void UpdateExportButton()
        {
            bool pathOk = !string.IsNullOrWhiteSpace(Settings.Location.Filename);
            bool typeOk;
            switch(Settings.Location.DataLocationType)
            {
                case DataLocationTypes.Internal:
                    typeOk = File.Exists(Settings.Location.Filename);
                    break;
                case DataLocationTypes.External:
                    typeOk = true;
                    break;
                default:
                    typeOk = false;
                    break;
            }
            exportButton.Enabled = pathOk && typeOk;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            var preset = (StageTablePresets)typeComboBox.SelectedItem;
            Settings.Location.ResetToDefault(preset);
            Settings.Settings.ResetToDefault(preset);

            entrySettingsPropertyGrid.SelectedObject = Settings;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                StageTable.Write(Table, Settings.Location, Settings.Settings, Settings.References);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        bool lockTextBox = false;
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
                    lockTextBox = true;
                    
                    pathTextBox.Text = Settings.Location.Filename = sfd.FileName;
                    UpdateExportButton();

                    lockTextBox = false;
                }
            }
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!lockTextBox)
            {
                Settings.Location.Filename = pathTextBox.Text;
                entrySettingsPropertyGrid.SelectedObject = Settings;
                UpdateExportButton();
            }
        }
    }
}
