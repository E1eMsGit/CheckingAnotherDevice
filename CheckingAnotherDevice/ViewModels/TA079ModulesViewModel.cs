using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Modules;

namespace CheckingAnotherDevice.ViewModels;

public class TA079ModulesViewModel : ViewModelBase
{
    public static ObservableCollection<RkPkuModuleViewModel> _allModulesVm;
    private bool _isButtonEnable;

    public bool IsButtonEnable
    {
        get => _isButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _isButtonEnable, value);

    }
    public ObservableCollection<RkPkuModuleViewModel> AllModulesVm
    {
        get => _allModulesVm;
        set => this.RaiseAndSetIfChanged(ref _allModulesVm, value);
    }

    public ReactiveCommand<Unit, Task> ConnectAllCommand => ReactiveCommand.Create(async () =>
    {
        Log.GetInstance().Write("Идёт установка соединения с модулями...", ELogMessageColors.Message);
        IsButtonEnable = false;

        for (int i = 0; i < AllModulesVm.Count; i++)
        {
            if (AllModulesVm[i].IsConnected)
                continue;
            await Task.Run(() => AllModulesVm[i].Connect());
            await Task.Delay(100);
        }
   
        IsButtonEnable = true;
    });

    public TA079ModulesViewModel()
    {
        AllModulesVm = TA079Modules.GetInstance().AllModules.OfType<RkPkuModule>().Select(RkPkuModuleViewModel.FromModule).ToObservableCollection();
        IsButtonEnable = true;
    }
}