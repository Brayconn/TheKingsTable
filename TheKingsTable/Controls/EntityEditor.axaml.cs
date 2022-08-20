using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using System;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_XEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_YEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_FlagEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_EventEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_TypeEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_RawBitEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ListBitEditor, Type = typeof(BitEditor))]
    public class EntityEditor : TemplatedControl
    {
        public const string PART_XEditor = nameof(PART_XEditor);
        public const string PART_YEditor = nameof(PART_YEditor);
        public const string PART_FlagEditor = nameof(PART_FlagEditor);
        public const string PART_EventEditor = nameof(PART_EventEditor);
        public const string PART_TypeEditor = nameof(PART_TypeEditor);
        public const string PART_RawBitEditor = nameof(PART_RawBitEditor);
        public const string PART_ListBitEditor = nameof(PART_ListBitEditor);

        TextBox XEditor, YEditor, FlagEditor, EventEditor, TypeEditor, RawBitEditor;
        BitEditor ListBitEditor;

        #region X Styled Property
        public static readonly StyledProperty<short?> XProperty =
            AvaloniaProperty.Register<EntityEditor, short?>(nameof(X), null, defaultBindingMode:BindingMode.TwoWay);

        public short? X
        {
            get => GetValue(XProperty);
            set => SetValue(XProperty, value);
        }
        #endregion

        #region Y Styled Property
        public static readonly StyledProperty<short?> YProperty =
            AvaloniaProperty.Register<EntityEditor, short?>(nameof(Y), null, defaultBindingMode: BindingMode.TwoWay);

        public short? Y
        {
            get => GetValue(YProperty);
            set => SetValue(YProperty, value);
        }
        #endregion

        #region Flag Styled Property
        public static readonly StyledProperty<short?> FlagProperty =
            AvaloniaProperty.Register<EntityEditor, short?>(nameof(Flag), null, defaultBindingMode: BindingMode.TwoWay);

        public short? Flag
        {
            get => GetValue(FlagProperty);
            set => SetValue(FlagProperty, value);
        }
        #endregion

        #region Event Styled Property
        public static readonly StyledProperty<short?> EventProperty =
            AvaloniaProperty.Register<EntityEditor, short?>(nameof(Event), null, defaultBindingMode: BindingMode.TwoWay);

        public short? Event
        {
            get => GetValue(EventProperty);
            set => SetValue(EventProperty, value);
        }
        #endregion

        #region Type Styled Property
        public static readonly StyledProperty<short?> TypeProperty =
            AvaloniaProperty.Register<EntityEditor, short?>(nameof(Type), null, defaultBindingMode: BindingMode.TwoWay);

        public short? Type
        {
            get => GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }
        #endregion

        #region Bits Styled Property
        public static readonly StyledProperty<short?> BitsProperty =
            AvaloniaProperty.Register<EntityEditor, short?>(nameof(Bits), null, defaultBindingMode: BindingMode.TwoWay);

        public short? Bits
        {
            get => GetValue(BitsProperty);
            set => SetValue(BitsProperty, value);
        }
        #endregion

        Control lastFocused;
        Control LastFocused
        {
            get => lastFocused;
            set
            {
                if(value != LastFocused)
                {
                    if (lastFocused == XEditor)
                        SetIfDifferent(XProperty, GetNumber(XEditor.Text));
                    else if (LastFocused == YEditor)
                        SetIfDifferent(YProperty, GetNumber(YEditor.Text));
                    else if (LastFocused == FlagEditor)
                        SetIfDifferent(FlagProperty, GetFlag(FlagEditor.Text));
                    else if (LastFocused == EventEditor)
                        SetIfDifferent(EventProperty, short.TryParse(EventEditor.Text, out var e) ? e : null);
                    else if (LastFocused == TypeEditor)
                        SetIfDifferent(TypeProperty, GetEntityType(TypeEditor.Text));
                    else if(LastFocused == RawBitEditor)
                    {
                        if (short.TryParse(RawBitEditor.Text, out var b))
                            ListBitEditor.Value = cachedBits = b;
                        if (value != ListBitEditor)
                            SetIfDifferent(BitsProperty, cachedBits);
                    }
                    else if(LastFocused == ListBitEditor)
                    {
                        RawBitEditor.Text = (cachedBits = (short)ListBitEditor.Value).ToString();
                        if (value != RawBitEditor)
                            SetIfDifferent(BitsProperty, cachedBits);
                    }
                    lastFocused = value;
                }
            }
        }

        void SetIfDifferent(StyledProperty<short?> prop, short? value)
        {
            if(value != null && GetValue(prop) != value)
                SetValue(prop, value);
        }

        short? GetNumber(string text)
        {
            return short.TryParse(text, out var num) ? num : null;
        }
        //TODO get from project
        short? GetFlag(string text)
        {
            return short.TryParse(text, out var flag) ? flag : null;
        }
        //TODO get from project
        private short? GetEntityType(string text)
        {
            return short.TryParse(text, out var type) ? Type : null;
        }

        short cachedBits = 0;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            XEditor = e.NameScope.Find<TextBox>(PART_XEditor);
            YEditor = e.NameScope.Find<TextBox>(PART_YEditor);
            FlagEditor = e.NameScope.Find<TextBox>(PART_FlagEditor);
            EventEditor = e.NameScope.Find<TextBox>(PART_EventEditor);
            TypeEditor = e.NameScope.Find<TextBox>(PART_TypeEditor);
            RawBitEditor = e.NameScope.Find<TextBox>(PART_RawBitEditor);
            ListBitEditor = e.NameScope.Find<BitEditor>(PART_ListBitEditor);

            XEditor.GotFocus += UpdateFocus;
            YEditor.GotFocus += UpdateFocus;
            FlagEditor.GotFocus += UpdateFocus;
            EventEditor.GotFocus += UpdateFocus;
            TypeEditor.GotFocus += UpdateFocus;
            RawBitEditor.GotFocus += UpdateFocus;
            ListBitEditor.GotFocus += UpdateFocus;

            XEditor.LostFocus += UpdateFocus;
            YEditor.LostFocus += UpdateFocus;
            FlagEditor.LostFocus += UpdateFocus;
            EventEditor.LostFocus += UpdateFocus;
            TypeEditor.LostFocus += UpdateFocus;
            RawBitEditor.LostFocus += UpdateFocus;
            ListBitEditor.LostFocus += UpdateFocus;

            XProperty.Changed.Subscribe(x => XEditor.Text = x.NewValue.Value.ToString());
            YProperty.Changed.Subscribe(x => YEditor.Text = x.NewValue.Value.ToString());
            FlagProperty.Changed.Subscribe(x => FlagEditor.Text = x.NewValue.Value.ToString()); //TODO convert to proper value
            EventProperty.Changed.Subscribe(x => EventEditor.Text = x.NewValue.ToString());
            TypeProperty.Changed.Subscribe(x => TypeEditor.Text = x.NewValue.Value.ToString());
            BitsProperty.Changed.Subscribe(x =>
            {
                RawBitEditor.Text = x.NewValue.Value.ToString();
                ListBitEditor.Value = x.NewValue.Value;
            });
        }

        private void UpdateFocus(object? sender, EventArgs e)
        {
            if (sender is Control c)
                LastFocused = c;
        }
    }
}
