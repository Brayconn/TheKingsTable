using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheKingsTable.Controls
{
    public partial class StageTableLocationEditor : UserControl
    {
        public StageTableLocationEditor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
