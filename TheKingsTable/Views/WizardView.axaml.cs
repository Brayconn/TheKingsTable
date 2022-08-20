using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheKingsTable.Views
{
    public partial class WizardView : UserControl
    {
        public WizardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
