using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Modules;

namespace CheckingAnotherDevice.Models.Rk;

public class RkCommandsDict
{
    public Dictionary<int, ObservableCollection<RkCommand>> RkCommands { get; set; } = new();
    
    private static RkCommandsDict _instance;

    private RkCommandsDict()
    {
        var allModules = DeviceModules.GetInstance().AllModules;
        var allRks = new List<RkCommand>(XmlHelper.LoadRkCommands("settingsRk.xml"));

        var rkByModuleId = allRks.ToLookup(x => x.ModuleId);

        for (int i = 0; i <= 4; i++)
        {
            var moduleId = allModules[i].ModuleId;
            RkCommands.Add(moduleId, rkByModuleId[moduleId].ToObservableCollection());
        }
    }

    public static RkCommandsDict GetInstance()
    {
        if (_instance == null)
        {
            _instance = new RkCommandsDict();
        }

        return _instance;
    }
}