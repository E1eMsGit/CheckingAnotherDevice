using ReactiveUI;
using Rss.TmFramework.Base.Channels;
using Rss.TmFramework.Channels;
using Rss.TmFramework.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Modules;
using CheckingAnotherDevice.Models.Pku;


namespace CheckingAnotherDevice.ViewModels
{
    public class RkPkuModuleViewModel : ViewModelBase
    {
        private RkPkuModule _rkPkuModule;
        private UdpChannel _udpChannel;
        private EthernetModuleSettings? _settings;
        private Dictionary<int, ObservableCollection<PkuCommand>> _allPku;

        public RkPkuModule GetModule => _rkPkuModule;
        public string Name
        {
            get => _rkPkuModule.ModuleName; 
            set => _rkPkuModule.ModuleName = value;
        }
        public string ConnectionString
        {
            get => _rkPkuModule.ConnectionString;
            set => _rkPkuModule.ConnectionString = value;
        }
        public string MapVersion
        {
            get => _rkPkuModule.MapVersion;
            set => _rkPkuModule.MapVersion = value;
        }
        public bool IsConnected => _rkPkuModule.Channel?.ConnectionState == EConnectionState.Connected;
        public EConnectionState? ConnectionState => _rkPkuModule.Channel?.ConnectionState;
        
        public ReactiveCommand<Unit, Task> ConnectDisconnectCommand => ReactiveCommand.Create(async () =>
        {
            await Task.Run(ConnectDisconnect);
        });

        public RkPkuModuleViewModel(RkPkuModule module)
        {
            _allPku = PkuCommandsDict.GetInstance().PkuCommands;
            _rkPkuModule = module;
            _rkPkuModule.PkuReceived += (address, duration, moduleId) => { Log.GetInstance().Write($"{Name}: Модуль принял ПКУ - {address} ({_allPku[moduleId][(int)address].Name}), длительность - {duration} мс.", ELogMessageColors.Message); };
            _rkPkuModule.PkuModeReceived += (channelIndex, mode) => { Log.GetInstance().Write($"{Name}: ПКУ - {channelIndex}, режим - {mode} ", ELogMessageColors.Message); };
        }

        public static RkPkuModuleViewModel FromModule(RkPkuModule module)
        {
            return new RkPkuModuleViewModel(module);
        }

        #region Connect / Disconnect
        public void ConnectDisconnect()
        {
            if (IsConnected)
                Disconnect();
            else
                Connect();
        }
        public void Connect()
        {
            try
            {
                _udpChannel = new UdpChannel();
                if (_udpChannel == null)
                {
                    Log.GetInstance().Write("Ошибка открытия соединения! Канал передачи данных не инициализирован.", ELogMessageColors.Message);
                }
                else
                {
                    _udpChannel.ConnectionStateChanged += (channel, state) =>
                    {
                        switch (state)
                        {
                            case EConnectionState.Disconnected:
                                Log.GetInstance().Write(_rkPkuModule.ModuleName + ": соединение закрыто", ELogMessageColors.Message);
                                break;
                            case EConnectionState.Connecting:
                                Log.GetInstance().Write(_rkPkuModule.ModuleName + ": соединение устанавливается", ELogMessageColors.Message);
                                break;
                            case EConnectionState.Connected:
                                _rkPkuModule.TryReadSettings(out _settings);
                                if (_settings == null)
                                {
                                    Log.GetInstance().Write(_rkPkuModule.ModuleName + ": модуль не отвечает", ELogMessageColors.Message);
                                    _rkPkuModule.CloseChannel();
                                    break;
                                }
                                Log.GetInstance().Write($"{_rkPkuModule.ModuleName}: обнаружен \"{_settings.Name}\"", ELogMessageColors.Message);

                                if (_allPku.ContainsKey(_rkPkuModule.ModuleId))
                                {
                                    foreach (var pku in _allPku[_rkPkuModule.ModuleId])
                                    {
                                        _rkPkuModule.SetPkuChannelMode(pku.Number - 1, pku.Mode);
                                        _rkPkuModule.GetPkuChannelMode(pku.Number - 1, out var mode);

                                        if (mode != pku.Mode)
                                        {
                                            Log.GetInstance().Write($"Не удалось инициализировать ПКУ - {pku.Number}", ELogMessageColors.Message);
                                        }
                                    }
                                }
                                Log.GetInstance().Write(_rkPkuModule.ModuleName + ": соединение установлено", ELogMessageColors.Message);
                                break;
                            case EConnectionState.ConnectFail:
                                Log.GetInstance().Write(_rkPkuModule.ModuleName + ": не удалось установить соединение", ELogMessageColors.Message);
                                break;
                        }
                        
                        this.RaisePropertyChanged(nameof(ConnectionState));
                    };

                    _rkPkuModule.Channel = _udpChannel;
                    _rkPkuModule.OpenChannel(_rkPkuModule.ConnectionString);
                }
            }
            catch (Exception ex)
            {
                _rkPkuModule.Channel = null;
                Log.GetInstance().Write($"Ошибка открытия соединения : {ex}", ELogMessageColors.Message);
            }
        }
        public void Disconnect()
        {
            _rkPkuModule.CloseChannel();
            _rkPkuModule.Channel = null;
            _udpChannel = null;
        }
        #endregion
    }
}
