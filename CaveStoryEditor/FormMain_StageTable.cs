using CaveStoryModdingFramework.Stages;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    partial class FormMain
    {
        const string IndexColumnName = "Index";
        const string ColumnPostFix = "Column";
        void InitStageTableColumns()
        {
            stageTableDataGridView.Columns.Clear();

            #region index column
            //index column belongs no matter what
            var indexColumn = new DataGridViewColumn()
            {
                HeaderText = IndexColumnName,
                Name = IndexColumnName,
                ReadOnly = true,
                CellTemplate = new DataGridViewTextBoxCell(),
            };
            //width = whichever needs more room, the header, or the biggest index
            //the ??'s regarding the font are to fix a bug in mono where it was null instead of DefaultFont
            indexColumn.Width = Math.Max(
            (int)(TextRenderer.MeasureText(indexColumn.HeaderText, indexColumn.HeaderCell.Style.Font ?? DefaultFont).Width * 1.5),
            TextRenderer.MeasureText(mod.StageTable.Count.ToString(), indexColumn.CellTemplate.Style.Font ?? DefaultFont).Width
            );
            stageTableDataGridView.Columns.Add(indexColumn);
            #endregion
                        
            void AddComboBoxColumn<T>(string Text, string Property, IDictionary<T, string> dict)
            {
                var list = new List<KeyValuePair<T, string>>(dict.Count);
                foreach (var item in dict)
                    list.Add(item);
                stageTableDataGridView.Columns.Add(new DataGridViewColumn()
                {
                    HeaderText = Text,
                    Name = Property + ColumnPostFix,
                    DataPropertyName = Property,
                    CellTemplate = new DataGridViewComboBoxCell()
                    {
                        ValueMember = nameof(KeyValuePair<T,string>.Key),
                        DisplayMember = nameof(KeyValuePair<T,string>.Value),
                        DataSource = list
                    },
                    ValueType = typeof(T)
                });
            }
            void AddColumn(string Text, string Property)
            {
                stageTableDataGridView.Columns.Add(new DataGridViewColumn()
                {
                    HeaderText = Text,
                    Name = Property + ColumnPostFix,
                    DataPropertyName = Property,
                    CellTemplate = new DataGridViewTextBoxCell(),
                    ValueType = typeof(string)
                });
            }
            //adding the actual content
            AddColumn("Map Name", nameof(StageEntry.MapName));
            //TODO need to add/remove Japanese Name column as needed
            if (mod.StageTableFormat == StageTableTypes.stagetbl)
                AddColumn("Japanese Name", nameof(StageEntry.JapaneseName));
            AddColumn("Tileset", nameof(StageEntry.TilesetName));
            AddColumn("Filename", nameof(StageEntry.Filename));

            AddComboBoxColumn("Background Type", nameof(StageEntry.BackgroundType), mod.BackgroundTypes);
            
            AddColumn("Background Name", nameof(StageEntry.BackgroundName));
            AddColumn("Spritesheet 1", nameof(StageEntry.Spritesheet1));
            AddColumn("Spritesheet 2", nameof(StageEntry.Spritesheet2));
            
            AddComboBoxColumn("Boss Number", nameof(StageEntry.BossNumber), mod.BossNumbers);
        }

        private void StageTableDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var cell = stageTableDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case nameof(StageEntry.BackgroundType) + ColumnPostFix:
                    e.Cancel = true;
                    break;
                case nameof(StageEntry.BossNumber) + ColumnPostFix:
                    e.Cancel = true;
                    break;
            }
        }

        private void stageTableDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //no negative rows
            if (0 <= e.RowIndex &&
                //either the NewRowIndex doesn't exist, or the row we're on has to be lower
                (stageTableDataGridView.NewRowIndex == -1 || e.RowIndex < stageTableDataGridView.NewRowIndex))
            {
                var cell = stageTableDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                switch (cell.OwningColumn.Name)
                {
                    case IndexColumnName:
                        e.Value = e.RowIndex;
                        break;
                    case nameof(StageEntry.BackgroundType) + ColumnPostFix:
                        e.Value = $"{mod.StageTable[e.RowIndex].BackgroundType} - {e.Value ?? "???"}";
                        break;
                    case nameof(StageEntry.BossNumber) + ColumnPostFix:
                        e.Value = $"{mod.StageTable[e.RowIndex].BossNumber} - {e.Value ?? "???"}";
                        break;
                }
            }
        }

        StageEntry SelectedStageTableEntry
        {
            get => stageTablePropertyGrid.SelectedObject as StageEntry;
            set
            {
                if(value != SelectedStageTableEntry)
                {
                    if(SelectedStageTableEntry != null)
                        SelectedStageTableEntry.PropertyChanged -= SelectedStageTableEntry_PropertyChanged;

                    stageTablePropertyGrid.SelectedObject = value;
                    if(SelectedStageTableEntry != null)
                        SelectedStageTableEntry.PropertyChanged += SelectedStageTableEntry_PropertyChanged;
                }
            }
        }

        bool stageTableUnsaved = false;
        bool StageTableUnsaved
        {
            get => stageTableUnsaved;
            set
            {
                if(value != stageTableUnsaved)
                {
                    if (!stageTableUnsaved)
                        stageTableTabPage.Text += "*";
                    else
                        stageTableTabPage.Text = stageTableTabPage.Text.Remove(stageTableTabPage.Text.Length - 1);
                    stageTableUnsaved = value;
                }
            }
        }

        private void SelectedStageTableEntry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StageTableUnsaved = true;
        }

        private void stageTableDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            var oneSelected = stageTableDataGridView.SelectedRows.Count == 1
                && stageTableDataGridView.SelectedRows[0].Index < mod.StageTable.Count;
            openTilesButton.Enabled = openScriptButton.Enabled = openBothButton.Enabled = oneSelected;
            SelectedStageTableEntry = oneSelected
                ? mod.StageTable[stageTableDataGridView.SelectedRows[0].Index]
                : null;
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            mod.StageTable.Insert(stageTableDataGridView.SelectedRows[0].Index, new StageEntry());
            stageTableBinding.ResetBindings(false);
            UpdateCanAddStageTableEntries();
            StageTableUnsaved = true;
        }

        void UpdateCanAddStageTableEntries()
        {
            switch (mod.StageTableFormat)
            {
                case StageTableTypes.doukutsuexe:
                    stageTableDataGridView.AllowUserToAddRows = mod.StageTable.Count * mod.StageTableSettings.Size < StageTable.CSStageTableSize;
                    break;
                case StageTableTypes.stagetbl:
                    //TODO max value is 95 or something?
                default:
                    stageTableDataGridView.AllowUserToAddRows = true;
                    break;
            }
            insertButton.Enabled = stageTableDataGridView.AllowUserToAddRows;
        }

        private void stageTableDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            StageTableUnsaved = true;
            UpdateCanAddStageTableEntries();
        }

        private void stageTableDataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            StageTableUnsaved = true;
            UpdateCanAddStageTableEntries();
        }

        private void saveStageTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check that the actual format is ok
            if (!StageTable.VerifyStageTable(mod.StageTable, mod.StageTableFormat, mod.StageTableSettings))
            {
                MessageBox.Show("Error: something broke before we got very far");
                return;
            }

            //safety check that you won't overwrite something important
            bool VerifyExtension(string ext)
            {
                return mod.StageTableLocation.EndsWith(ext)
                    || MessageBox.Show("Warning: You might be about to save over the wrong file, are you sure you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            bool result = false;
            switch (mod.StageTableFormat)
            {
                case StageTableTypes.doukutsuexe:
                case StageTableTypes.swdata:
                case StageTableTypes.csmap:
                    result = VerifyExtension("exe");
                    break;
                case StageTableTypes.stagetbl:
                    result = VerifyExtension("tbl");
                    break;
                case StageTableTypes.mrmapbin:
                    result = VerifyExtension("bin");
                    break;
            }
            if (!result)
                return;

            //actually do the writing
            StageTable.Write(mod.StageTable, mod.StageTableLocation, mod.StageTableFormat);

            //final check if SW worked
            if (mod.StageTableFormat == StageTableTypes.swdata && !StageTable.VerifySW(mod.StageTableLocation))
                MessageBox.Show("Warning: SW won't be able to load this exe, sorry.");

            StageTableUnsaved = false;
        }

        private void exportStageTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormStageTableExporter(mod.StageTable).ShowDialog();
        }
    }
}
