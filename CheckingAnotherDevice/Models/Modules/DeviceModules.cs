using System;
using System.Collections.ObjectModel;
using CheckingAnotherDevice.Helpers;

namespace CheckingAnotherDevice.Models.Modules;

public class DeviceModules 
{
    private static DeviceModules _instance;
    public ObservableCollection<RkPkuModule> AllModules { get; set; }

    private DeviceModules()
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

    public static DeviceModules GetInstance()
    {
        if (_instance == null)
        {
            _instance = new DeviceModules();
        }

        return _instance;
    }
}