using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheKingsTable.ViewModels.Editors;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_Tabs, Type = typeof(TabControl))]
    public class StageEditorToolMenu : TemplatedControl, IStyleable
    {
        public const string PART_Tabs = nameof(PART_Tabs);

        Type IStyleable.StyleKey => typeof(StageEditorToolMenu);

        #region StageEditor Styled Property
        public static readonly StyledProperty<StageEditorViewModel?> StageEditorProperty =
            AvaloniaProperty.Register<StageEditorToolMenu, StageEditorViewModel?>(nameof(StageEditor), null);

        public StageEditorViewModel? StageEditor
        {
            get => GetValue(StageEditorProperty);
            set => SetValue(StageEditorProperty, value);
        }
        #endregion

        #region EditorMode Styled Property
        public static readonly StyledProperty<StageEditorViewModel.Editors> EditorModeProperty =
            AvaloniaProperty.Register<StageEditorToolMenu, StageEditorViewModel.Editors>(nameof(EditorMode), StageEditorViewModel.Editors.Tile, defaultBindingMode:BindingMode.TwoWay);

        public StageEditorViewModel.Editors EditorMode
        {
            get => GetValue(EditorModeProperty);
            set => SetValue(EditorModeProperty, value);
        }
        #endregion

        #region TabType Attached Property
        public static readonly AttachedProperty<StageEditorViewModel.Editors?> TabTypeProperty
            = AvaloniaProperty.RegisterAttached<StageEditorToolMenu, IAvaloniaObject, StageEditorViewModel.Editors?>(
            "TabType", null);

        public static void SetTabType(AvaloniaObject element, StageEditorViewModel.Editors? value)
        {
            element.SetValue(TabTypeProperty, value);
        }

        public static StageEditorViewModel.Editors? GetTabType(AvaloniaObject element)
        {
            return element.GetValue(TabTypeProperty);
        }
        #endregion

        TabControl Tabs;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Tabs = e.NameScope.Find<TabControl>(PART_Tabs);
            Tabs.SelectionChanged += Tabs_SelectionChanged;
        }

        private void Tabs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0 && e.AddedItems[0] is AvaloniaObject a)
            {
                var t = GetTabType(a);
                if (t != null)
                    EditorMode = t.Value;
            }
        }
    }
}
