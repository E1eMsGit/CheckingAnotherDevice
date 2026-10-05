using System;
using System.Collections.ObjectModel;
using CheckingAnotherDevice.Helpers;

namespace CheckingAnotherDevice.Models.Modules;

public class TA079Modules 
{
    private static TA079Modules _instance;
    public ObservableCollection<RkPkuModule> AllModules { get; set; }

    private TA079Modules()
    {
        try
        {
            AllModules = new ObservableCollection<RkPkuModule>(XmlHelper.LoadModulesConnectionSettings("settings.xml"));
            // Для их уебанских приборов которые на модулях РК принимают ПКУ.
            for (int i = 0; i < 5; i++)
            {
                AllModules[i].UnsubscribeRequestReceived();
            }
        }
        catch (Exception e)
        {
            AllModules = new ObservableCollection<RkPkuModule>(DefaultModulesSettings.DefaultSettings());
        }
    }

    public static TA079Modules GetInstance()
    {
        if (_instance == null)
        {
            _instance = new TA079Modules();
        }

        return _instance;
    }
}