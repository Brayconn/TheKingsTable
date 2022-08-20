using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using System;
using System.Windows.Input;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_Carousel, Type = typeof(Carousel))]
    [TemplatePart(Name = PART_BackButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_NextButton, Type = typeof(Button))]
    public class WizardControl : Carousel, IStyleable
    {
        public const string PART_Carousel = nameof(PART_Carousel);
        public const string PART_BackButton = nameof(PART_BackButton);
        public const string PART_NextButton = nameof(PART_NextButton);

        Type IStyleable.StyleKey => typeof(WizardControl);

        //TODO I kinda wish these had default functions that just ++/--
        #region BackCommand Styled Property
        public static readonly StyledProperty<ICommand> BackCommandProperty =
            AvaloniaProperty.Register<WizardControl, ICommand>(nameof(BackCommand));
        public ICommand BackCommand
        {
            get => GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }
        #endregion

        #region NextCommand Styled Property
        public static readonly StyledProperty<ICommand> NextCommandProperty =
            AvaloniaProperty.Register<WizardControl, ICommand>(nameof(NextCommand));
        public ICommand NextCommand
        {
            get => GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }
        #endregion

        Carousel Carousel;
        Button BackButton, NextButton;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Carousel = e.NameScope.Find<Carousel>(PART_Carousel);
            BackButton = e.NameScope.Find<Button>(PART_BackButton);
            NextButton = e.NameScope.Find<Button>(PART_NextButton);
            Carousel.SelectionChanged += Carousel_SelectionChanged;
            Carousel_SelectionChanged(Carousel, null);
        }

        private void Carousel_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            BackButton.IsEnabled = Carousel.SelectedIndex > 0;
            NextButton.Content = Carousel.SelectedIndex < Carousel.ItemCount - 1 ? "Next" : "Finish";
        }
    }
}
