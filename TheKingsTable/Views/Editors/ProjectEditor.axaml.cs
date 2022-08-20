using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheKingsTable.Views.Editors
{
    public partial class ProjectEditor : UserControl
    {
        public ProjectEditor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
