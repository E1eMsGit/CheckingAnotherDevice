using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Rk;

namespace CheckingAnotherDevice.ViewModels;

public class SettingsRkViewModel : ViewModelBase
{
    public Dictionary<int, ObservableCollection<RkCommand>> RkCommands => RkCommandsDict.GetInstance().RkCommands;

    public ReactiveCommand<Unit, Task> SaveSettingsRkCommand => ReactiveCommand.Create( async () => { 
        await Task.Run(XmlHelper.SaveRkCommands);
        Log.GetInstance().Write("Изменения настроек РК-команд сохранены.", ELogMessageColors.Message);
    });
}