using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_BitList, Type = typeof(ListBox))]
    public class BitEditor : TemplatedControl
    {
        public const string PART_BitList = nameof(PART_BitList);

        #region Length Styled Property
        public static readonly StyledProperty<int> LengthProperty =
            AvaloniaProperty.Register<BitEditor, int>(nameof(Length), 0);

        public int Length
        {
            get => GetValue(LengthProperty);
            set => SetValue(LengthProperty, value);
        }
        #endregion

        #region Value Styled Property
        public static readonly StyledProperty<long?> ValueProperty =
            AvaloniaProperty.Register<BitEditor, long?>(nameof(Value), null);

        public long? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        public BitEditor()
        {
            //LengthProperty.Changed.Subscribe(x => UpdateBitAmount(x.NewValue.Value));
            //ValueProperty.Changed.Subscribe(x => UpdateBitValues(x.NewValue.Value));
        }

        ListBox BitList;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            BitList = e.NameScope.Find<ListBox>(PART_BitList);
        }

        public void UpdateBitAmount(int amount)
        {
            
        }
        public void UpdateBitValues(long? value)
        {

        }

    }
}
