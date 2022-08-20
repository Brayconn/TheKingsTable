using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CaveStoryModdingFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_EntityList, Type = typeof(ListBox))]
    public class EntitySelector : TemplatedControl
    {
        public const string PART_EntityList = nameof(PART_EntityList);

        #region SelectedEntity Styled Property
        public static readonly StyledProperty<short> SelectedEntityProperty =
            AvaloniaProperty.Register<EntitySelector, short>(nameof(SelectedEntity), 0, defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

        public short SelectedEntity
        {
            get => GetValue(SelectedEntityProperty);
            set => SetValue(SelectedEntityProperty, value);
        }
        #endregion

        #region Entities Styled Property
        public static readonly StyledProperty<Dictionary<int,EntityInfo>?> EntitiesProperty =
            AvaloniaProperty.Register<EntitySelector, Dictionary<int,EntityInfo>?>(nameof(Entities), null);

        public Dictionary<int,EntityInfo>? Entities
        {
            get => GetValue(EntitiesProperty);
            set => SetValue(EntitiesProperty, value);
        }
        #endregion

        ListBox EntityList;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            EntityList = e.NameScope.Find<ListBox>(PART_EntityList);
            EntityList.SelectionChanged += SelectionChanged;
        }

        void SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if(Entities != null && e.AddedItems.Count > 0 && e.AddedItems[0] is KeyValuePair<int,EntityInfo> ent)
            {
                SelectedEntity = (short)ent.Key;
                //SelectedEntity = (short)Entities.First(x => x.Value == ent).Key;
            }
        }
    }
}
