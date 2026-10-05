using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Modules;

namespace CheckingAnotherDevice.Models.Pku;

public class PkuCommandsDict
{
    public Dictionary<int, ObservableCollection<PkuCommand>> PkuCommands { get; set; } = new();

    private static PkuCommandsDict _instance;

    private PkuCommandsDict()
    {
        var allModules = TA079Modules.GetInstance().AllModules;
        var allPku = new List<PkuCommand>(XmlHelper.LoadPkuCommands("settingsPku.xml"));

        var pkuByModuleId = allPku.ToLookup(x => x.ModuleId);

        for (int i = 5; i <= 17; i++)
        {
            var moduleId = allModules[i].ModuleId;
            PkuCommands.Add(moduleId, pkuByModuleId[moduleId].ToObservableCollection());
        }
    }

    public static PkuCommandsDict GetInstance()
    {
        if (_instance == null)
        {
            _instance = new PkuCommandsDict();
        }

        return _instance;
    }
}