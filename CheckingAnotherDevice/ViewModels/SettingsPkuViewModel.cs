using ReactiveUI;
using Rss.TmFramework.Modules.RkPku;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Pku;

namespace CheckingAnotherDevice.ViewModels
{
    public class SettingsPkuViewModel : ViewModelBase
    {
        private ObservableCollection<RkPkuModuleViewModel> _allModules = TA079ModulesViewModel._allModulesVm;
       
        public Dictionary<int, ObservableCollection<PkuCommand>> PkuCommands => PkuCommandsDict.GetInstance().PkuCommands;
        public IEnumerable<EPkuMode> Modes { get; } = Enum.GetValues(typeof(EPkuMode)).Cast<EPkuMode>();
       
        public ReactiveCommand<Unit, Task> SaveSettingsPkuCommand => ReactiveCommand.Create(async () => {
            await Task.Run(XmlHelper.SavePkuCommands);

            await Task.Run(() =>
            {
                for (int i = 0; i < PkuCommands.Count; i++)
                {
                    foreach (var pku in PkuCommands.ElementAt(i).Value)
                    {
                        _allModules[i].GetModule.SetPkuChannelMode(pku.Number - 1, pku.Mode);
                    }
                }
            });
            Log.GetInstance().Write("Изменения настроек ПКУ-команд сохранены.", ELogMessageColors.Message);
        });
    }
}
