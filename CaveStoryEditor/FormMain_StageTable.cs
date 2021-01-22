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
            indexColumn.Width = Math.Max(
            (int)(TextRenderer.MeasureText(indexColumn.HeaderText, indexColumn.HeaderCell.Style.Font).Width * 1.5),
            TextRenderer.MeasureText(mod.StageTable.Count.ToString(), indexColumn.CellTemplate.Style.Font).Width
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
                        ValueMember = "Key",
                        DisplayMember = "Value",
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
            //filter to valid rows
            if (0 <= e.RowIndex && e.RowIndex < stageTableDataGridView.NewRowIndex)
            {
                var cell = stageTableDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                switch (cell.OwningColumn.Name)
                {
                    case IndexColumnName:
                        e.Value = e.RowIndex;
                        break;
                    case nameof(StageEntry.BackgroundType) + ColumnPostFix:
                        if(string.IsNullOrWhiteSpace((string)e.Value))
                            e.Value = "???";
                        break;
                    case nameof(StageEntry.BossNumber) + ColumnPostFix:
                        if (string.IsNullOrWhiteSpace((string)e.Value))
                            e.Value = "???";
                        break;
                }
            }
        }

        private void stageTableDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            var oneSelected = stageTableDataGridView.SelectedRows.Count == 1;
            openTilesButton.Enabled = openScriptButton.Enabled = openBothButton.Enabled = oneSelected;
            if (oneSelected)
            {
                var entry = mod.StageTable[stageTableDataGridView.SelectedRows[0].Index];
                if (stageTablePropertyGrid.SelectedObject != entry)
                    stageTablePropertyGrid.SelectedObject = entry;
            }
            else
            {
                stageTablePropertyGrid.SelectedObject = null;
            }
        }

        private void saveStageTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!StageTable.VerifyStageTable(mod.StageTable, mod.StageTableFormat, mod.StageTableSettings))
            {
                MessageBox.Show("Error: something broke before we got very far");
                return;
            }
            StageTable.Write(mod.StageTable, mod.StageTableLocation, mod.StageTableFormat);
            if (mod.StageTableFormat == StageTableTypes.swdata && !StageTable.VerifySW(mod.StageTableLocation))
                MessageBox.Show("Warning: SW won't be able to load this exe, sorry");
        }

        private void exportStageTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormStageTableExporter(mod.StageTable).ShowDialog();
        }
    }
}
