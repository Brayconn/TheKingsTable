using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using AvaloniaEdit;

namespace TheKingsTable.Controls
{
    [TemplatePart(Name = PART_TextEditor, Type = typeof(TextEditor))]
    public class TextEditorWrapper : TemplatedControl
    {
        public const string PART_TextEditor = nameof(PART_TextEditor);

        #region Text Direct Property
        public static readonly DirectProperty<TextEditorWrapper, string> TextProperty =
            AvaloniaProperty.RegisterDirect<TextEditorWrapper, string>(nameof(Text),
                o => o.Text, (o,e) => o.Text = e, defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);

        //This only exists because binding takes place before the TextEditor is set
        string backupTextQueue = "";
        public string Text
        {
            get => TextEditor?.Text ?? backupTextQueue;
            private set
            {
                if(TextEditor == null)
                {
                    backupTextQueue = value;
                }
                else if(value != TextEditor.Text)
                {
                    var old = TextEditor.Text;
                    TextEditor.Text = value;
                    RaisePropertyChanged(TextProperty, old, Text);
                }
            }
        }
        #endregion

        TextEditor? TextEditor;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            TextEditor = e.NameScope.Find<TextEditor>(PART_TextEditor);
            TextEditor.TextChanged += (o, e) => this.RaisePropertyChanged(TextProperty, Text, Text);
            Text = backupTextQueue;
            backupTextQueue = "";
        }
    }
}
