using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class FormGoto : Form
    {
        public string Value => inputTextBox.Text;

        public FormGoto()
        {
            InitializeComponent();

            eventLabel.Text = FormScriptEditor.EventStart_String;
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                okButton.PerformClick();
            }
        }
    }
}
