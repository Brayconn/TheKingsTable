using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    class DataGridViewEnumCell<T> : DataGridViewTextBoxCell
    {
        Dictionary<T, string> Defaults;

        public DataGridViewEnumCell(Enum defaults)
        {
            if(Enum.GetUnderlyingType(defaults.GetType()) != typeof(T))
                throw new ArgumentException("types must match", nameof(defaults));
            
            var vals = (T[])Enum.GetValues(defaults.GetType());
            var names = Enum.GetNames(defaults.GetType());
            
            Defaults = new Dictionary<T, string>(vals.Length);
            for (int i = 0; i < vals.Length; i++)
                Defaults.Add(vals[i], names[i]);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
        }
    }
}
