using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public abstract partial class PropertyGridListBox<T> : UserControl where T : new()
    {
        public PropertyGridListBox()
        {
            InitializeComponent();
        }

        private List<T> list;
        public List<T> List
        {
            get => list;
            set
            {
                listBox.DataSource = list = value;
                UIEnabled = value != null;
            }
        }

        bool uiEnabled = false;
        bool UIEnabled
        {
            get => uiEnabled;
            set
            {
                listBox.Enabled = value;

                UpdateCanAddItems();

                propertyGrid.Enabled = value;

                uiEnabled = value;
            }
        }

        readonly static MethodInfo refreshItemsMethodInfo = typeof(ListBox).GetMethod("RefreshItems",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
        null, Array.Empty<Type>(), null);
        
        //HACK need to refresh the list, and for whatever reason that method is private
        public void npcTableListBox_RefreshItems()
        {
            refreshItemsMethodInfo.Invoke(listBox, Array.Empty<object>());
        }

        //HACK as if the other method wasn't bad enough...
        //the listbox doesn't seem to like refreshing its items when you remove things, so I have to just ignore the exception...?!
        public void SafeRefreshItems()
        {
            try
            {
                npcTableListBox_RefreshItems();
            }
            catch (TargetInvocationException)
            {

            }
        }

        public void RefreshPropertyGrid()
        {
            propertyGrid.SelectedObject = listBox.SelectedItem;
        }

        int maxItems = 0;
        public int MaxItems
        {
            get => maxItems;
            set
            {
                maxItems = value;
                UpdateCanAddItems();
            }
        }
        protected void UpdateCanAddItems()
        {
            addButton.Enabled = insertButton.Enabled = List != null && (MaxItems <= 0 || List.Count < MaxItems);
            removeButton.Enabled = List != null && List.Count > 0;
        }

        public int SelectedIndex
        {
            get => listBox.SelectedIndex;
        }


        public event EventHandler SelectedItemChanging;
        public event EventHandler SelectedItemChanged;

        bool lockIndex = false;

        T selectedItem;
        public T SelectedItem
        {
            get => selectedItem;
            protected set
            {
                if(!EqualityComparer<T>.Default.Equals(selectedItem, value))
                {
                    SelectedItemChanging?.Invoke(this, new EventArgs());
                    
                    propertyGrid.SelectedObject = selectedItem = value;
                    if (!lockIndex)
                    {
                        lockIndex = true;

                        listBox.SelectedIndex = List.IndexOf(SelectedItem);

                        lockIndex = false;
                    }

                    SelectedItemChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lockIndex)
            {
                lockIndex = true;
                SelectedItem = (T)listBox.SelectedItem;
                lockIndex = false;
            }
        }

        public event EventHandler ItemAdded;
        protected void InvokeItemAdded()
        {
            ItemAdded?.Invoke(this, new EventArgs());
        }
        public event EventHandler ItemInserted;
        protected void InvokeItemInserted()
        {
            ItemInserted?.Invoke(this, new EventArgs());
        }
        public event EventHandler ItemRemoved;
        protected void InvokeItemRemoved()
        {
            ItemRemoved?.Invoke(this, new EventArgs());
        }
        public void Add()
        {
            List.Add(new T());
            SafeRefreshItems();
            SelectedItem = List[List.Count - 1];
            UpdateCanAddItems();
            InvokeItemAdded();
        }
        public void Insert()
        {
            var index = Math.Max(0, SelectedIndex);
            var entry = new T();
            List.Insert(index, entry);
            SafeRefreshItems();
            SelectedItem = List[index];
            UpdateCanAddItems();
            InvokeItemInserted();
        }
        public void Remove()
        {
            var index = SelectedIndex;
            List.RemoveAt(index);
            SafeRefreshItems();
            var newSelect = Math.Max(index - 1, 0);
            SelectedItem = newSelect > 0 ? List[newSelect] : default;
            UpdateCanAddItems();
            InvokeItemRemoved();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            Insert();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            Remove();
        }

        public event EventHandler<ListControlConvertEventArgs> Format; 
        private void listBox_Format(object sender, ListControlConvertEventArgs e)
        {
            Format?.Invoke(this, e);
        }
    }
}
