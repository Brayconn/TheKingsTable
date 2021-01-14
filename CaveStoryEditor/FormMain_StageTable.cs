using CaveStoryModdingFramework.Stages;
using System;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    partial class FormMain
    {
        const string IndexColumnName = "Index";
        void InitStageTableColumns()
        {
            stageTableDataGridView.Columns.Clear();

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


            void AddCustomColumn(string Text, string Property, Type t, DataGridViewCell template)
            {
                stageTableDataGridView.Columns.Add(new DataGridViewColumn()
                {
                    HeaderText = Text,
                    Name = Property + "Column",
                    DataPropertyName = Property,
                    CellTemplate = template,
                    ValueType = t
                });
            }
            void AddColumn(string Text, string Property)
            {
                AddCustomColumn(Text, Property, typeof(string), new DataGridViewTextBoxCell()
                {

                });
            }
            //adding the actual content
            AddColumn("Map Name", nameof(StageEntry.MapName));
            //TODO need to add/remove Japanese Name column as needed
            if (mod.StageTableFormat == StageTableTypes.stagetbl)
                AddColumn("Japanese Name", nameof(StageEntry.JapaneseName));
            AddColumn("Tileset", nameof(StageEntry.TilesetName));
            AddColumn("Filename", nameof(StageEntry.Filename));
            AddCustomColumn("Background Type", nameof(StageEntry.BackgroundType), typeof(BackgroundTypes), new DataGridViewComboBoxCell()
            {
                DataSource = Enum.GetValues(typeof(BackgroundTypes))
            });
            AddColumn("Background Name", nameof(StageEntry.BackgroundName));
            AddColumn("Spritesheet 1", nameof(StageEntry.Spritesheet1));
            AddColumn("Spritesheet 2", nameof(StageEntry.Spritesheet2));
            AddCustomColumn("Boss Number", nameof(StageEntry.BossNumber), typeof(BossNumbers), new DataGridViewComboBoxCell()
            {
                DataSource = Enum.GetValues(typeof(BossNumbers))
            });
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

        private void stageTableDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //filter to valid rows
            if (0 <= e.RowIndex && e.RowIndex < stageTableDataGridView.NewRowIndex
                //filter to index column
                && stageTableDataGridView.Columns[e.ColumnIndex].Name == IndexColumnName)
            {
                //set the index
                e.Value = e.RowIndex;
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
