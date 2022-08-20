using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKingsTable.ViewModels
{
    public class WizardWindowViewModel : ViewModelBase
    {
        public WizardViewModel WizardView { get; }
        public WizardWindowViewModel()
        {
            WizardView = new WizardViewModel();
        }
    }
}
