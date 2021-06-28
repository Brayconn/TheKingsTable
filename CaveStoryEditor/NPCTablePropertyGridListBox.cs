using CaveStoryModdingFramework.Entities;
using System;

namespace CaveStoryEditor
{
    public partial class NPCTablePropertyGridListBox : PropertyGridListBox<NPCTableEntry>
    {
        public NPCTablePropertyGridListBox() : base()
        {
            InitializeComponent();
        }

        public override void Add()
        {
            List.Add(new NPCTableEntry());
            SafeRefreshItems();
            SelectedItem = List[List.Count - 1];
            UpdateCanAddItems();
            InvokeItemAdded();
        }

        public override void Insert()
        {
            var index = Math.Max(0, SelectedIndex);
            var entry = new NPCTableEntry();
            List.Insert(index, entry);
            SafeRefreshItems();
            SelectedItem = List[index];
            UpdateCanAddItems();
            InvokeItemInserted();            
        }

        public override void Remove()
        {
            var index = SelectedIndex;
            List.RemoveAt(index);
            SafeRefreshItems();
            var newSelect = Math.Max(index - 1, 0);
            SelectedItem = newSelect > 0 ? List[newSelect] : null;
            UpdateCanAddItems();
            InvokeItemRemoved();
        }
    }
}
