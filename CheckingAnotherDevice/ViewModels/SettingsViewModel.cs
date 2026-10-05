using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using CheckingAnotherDevice.Helpers;

namespace CheckingAnotherDevice.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<RkPkuModuleViewModel> AllModules => DeviceModulesViewModel._allModulesVm;
        public ReactiveCommand<Unit, Task> SaveSettingsCommand => ReactiveCommand.Create(async () => {
            await Task.Run(XmlHelper.SaveModuleConnectionSettings);
            Log.GetInstance().Write("Изменения настроек модулей сохранены.", ELogMessageColors.Message);
        });
    }
}
