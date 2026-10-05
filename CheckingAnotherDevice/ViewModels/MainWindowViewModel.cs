using ReactiveUI;
using System.Diagnostics;
using System.Reactive;

namespace CheckingAnotherDevice.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OnWindowLoaded => ReactiveCommand.Create(() => Debug.WriteLine("Loaded"));

        public ReactiveCommand<Unit, Unit> OnWindowClosing => ReactiveCommand.Create(() => Debug.WriteLine("Closing"));
        
    }
}
