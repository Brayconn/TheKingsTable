using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using TheKingsTable.ViewModels;

namespace TheKingsTable.Views
{
    public partial class WizardWindow : ReactiveWindow<WizardWindowViewModel>
    {
        public WizardWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(d => d(CommonInteractions.BrowseToOpenFile
                .RegisterHandler(x => CommonAvaloniaHandlers.ShowOpenFileBrowser(x,this))));
            this.WhenActivated(d => d(CommonInteractions.BrowseForFolder
                .RegisterHandler(x => CommonAvaloniaHandlers.ShowFolderBrowser(x,this))));
            this.WhenActivated(d => d(ViewModel!.WizardView.Close
                .RegisterHandler(HandleClose)));
        }

        async Task HandleClose(InteractionContext<object,Unit> context)
        {
            Close(context.Input);
            context.SetOutput(new Unit());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
