using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using CheckingAnotherDevice.Helpers;
using CheckingAnotherDevice.Models.Autotest;
using CheckingAnotherDevice.Models.Pku;
using CheckingAnotherDevice.Models.Rk;
using CheckingAnotherDevice.Properties;

namespace CheckingAnotherDevice.ViewModels
{
    public class DeviceCheckingViewModel : ViewModelBase
    {
        private readonly string _separator = new('-', 20);
        private ushort _duration;
        private FileInfo? _fileInfo;
        private string _startBtnText;
        private CancellationTokenSource? _cts;
        private bool _canStart;
        private bool _startProgressBar;
        private List<TestCase> _testCases;
        private Dictionary<int, ObservableCollection<PkuCommand>> _allPku;
        private bool _isAutotest;
        private int _currentTestCaseIndex; 
        private int _errorsCount;
        private int _pkuCount;
        private bool _isManualModeEnabled;
        private readonly object _pkuCountLock;

        public ObservableCollection<RkPkuModuleViewModel> AllModules => TA079ModulesViewModel._allModulesVm;
        public Dictionary<int, ObservableCollection<RkCommand>> RkCommands => RkCommandsDict.GetInstance().RkCommands;
        public ushort Duration
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }
        public FileInfo? FileInfo
        {
            get => _fileInfo;
            set => this.RaiseAndSetIfChanged(ref _fileInfo, value);
        }
        public bool StartProgressBar
        {
            get => _startProgressBar;
            set => this.RaiseAndSetIfChanged(ref _startProgressBar, value);
        }
        public string StartBtnText
        {
            get => _startBtnText;
            set => this.RaiseAndSetIfChanged(ref _startBtnText, value);
        }
        public bool IsManualModeEnabled
        {
            get => _isManualModeEnabled;
            set => this.RaiseAndSetIfChanged(ref _isManualModeEnabled, value);
        }

        public ReactiveCommand<Unit, Task> StartTestCommand => ReactiveCommand.Create(async () =>
        {
            if (_canStart)
            {
                if (!InitializeTest())
                {
                    return;
                }

                try
                {
                    await RunTestAsync(_cts.Token); 
                }
                catch (OperationCanceledException)
                {
                    await HandleCancellationAsync(); 
                }
                finally
                {
                    await CleanupAfterTestAsync(); 
                }
            }
            else
            {
                _canStart = true;
                _cts?.Cancel();
            }
        }, this.WhenAnyValue(x => x.FileInfo.Name, x => x.StartBtnText, (cond1, cond2) => !string.IsNullOrEmpty(cond1) && cond2 != Resources.EndingBtnText));
        public ReactiveCommand<Unit, Task> OpenFileCommand => ReactiveCommand.Create(async () =>
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filters = { new FileDialogFilter { Name = "XML Files", Extensions = { "xml" } } },
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                AllowMultiple = false
            };

            var result = await openFileDialog.ShowAsync(mainWindow); 

            if (result != null)
            {
                FileInfo = new FileInfo(result[0]);
            }
        });
        public ReactiveCommand<RkCommand, Task> SendCommand => ReactiveCommand.Create<RkCommand, Task>(async rk =>
        {
            await Task.Run(() => RkSendCommand(rk, Duration));
            Settings.Default.Duration = Duration;
            Settings.Default.Save();
        });

        public DeviceCheckingViewModel()
        {
            _allPku = PkuCommandsDict.GetInstance().PkuCommands;
            _isAutotest = false;
            Duration = Settings.Default.Duration;
            StartBtnText = Resources.StartBtnText;
            _canStart = true;
            IsManualModeEnabled = true;
            _pkuCountLock = new object();

            for (int i = 5; i < AllModules.Count; i++)
            {
                AllModules[i].GetModule.PkuReceived += AnalysisPku;
            }
        }

        private void RkSendCommand(RkCommand rk, ushort duration)
        {
            if (AllModules[rk.ModuleId].IsConnected)
            {
                Log.GetInstance()
                    .Write(
                        !AllModules[rk.ModuleId].GetModule.TrySendRk(rk.NumberRk - 1, duration, out var message)
                            ? $"{AllModules[rk.ModuleId].Name}: Не удалось выдать команду {rk.Name} длительностью {duration} мс. Ошибка: {message}"
                            : $"{AllModules[rk.ModuleId].Name}: Выдана команда {rk.Name} длительностью {duration} мс",
                        ELogMessageColors.Message);
            }
            else
            {
                Log.GetInstance().Write($"{AllModules[rk.ModuleId].Name}: Модуль не подключен", ELogMessageColors.Warning);
            }
        }
        private bool InitializeTest()
        {
            _testCases = XmlHelper.LoadTestCases(FileInfo.FullName);

            if (_testCases.Count == 0)
            {
                Log.GetInstance().Write("Ошибка: XML файл не соответствует", ELogMessageColors.Error);
                return false;
            }
 
            _pkuCount = 0;
            _errorsCount = 0;
            _isAutotest = true;
            _cts = new CancellationTokenSource();
            _canStart = false;
            IsManualModeEnabled = false;


            StartBtnText = Resources.StopBtnText;
            StartProgressBar = true;
            Log.GetInstance().Write($"Тест {FileInfo.Name} запущен", ELogMessageColors.Message);

            return true;
        }
        private async Task RunTestAsync(CancellationToken token)
        {
            for (_currentTestCaseIndex = 0; _currentTestCaseIndex < _testCases.Count; _currentTestCaseIndex++)
            {
                var testCase = _testCases[_currentTestCaseIndex];

                foreach (var command in testCase.RkCommands)
                {
                    token.ThrowIfCancellationRequested();
                    await ProcessCommandAsync(command, testCase, token);
                }
            }
        }
        private async Task ProcessCommandAsync(AutotestRkCommands commands, TestCase testCase, CancellationToken token)
        {
            await Task.Run(() => RkSendCommand(commands, commands.Duration));
            await Task.Delay(testCase.Delay, token);

            lock (_pkuCountLock)
            {
                if (_pkuCount != testCase.PkuCommands.Count)
                {
                    _errorsCount++;
                    Log.GetInstance().Write($"{_separator} Ошибка: Неверное количество принятых ПКУ {_separator}",
                        ELogMessageColors.Error);
                }
                _pkuCount = 0;
            }
        }
        private async Task HandleCancellationAsync()
        {
            StartBtnText = Resources.EndingBtnText;
            foreach (var testCase in _testCases)
            {
                foreach (var command in testCase.RkCommands.Where(c => c.Duration == 65535))
                {
                    await Task.Run(() => RkSendCommand(command, 0));
                    await Task.Delay(1500);
                }
            }
        }
        private Task CleanupAfterTestAsync()
        {
            StartBtnText = Resources.StartBtnText;
            _canStart = true;
            StartProgressBar = false;
            _isAutotest = false;
            IsManualModeEnabled = true;

            Settings.Default.Save();

            var hasErrors = _errorsCount > 0;

            var message = hasErrors
                ? $"Тест {FileInfo.Name} завершен с ошибками. Количество ошибок - {_errorsCount}"
                : $"Тест {FileInfo.Name} завершен без ошибок";

            var color = hasErrors
                ? ELogMessageColors.Error
                : ELogMessageColors.Success;

            Log.GetInstance().Write(message, color);
            
            return Task.CompletedTask;
        }
        private void AnalysisPku(uint address, ushort duration, int moduleId)
        {
            if (!_isAutotest || _testCases[_currentTestCaseIndex].PkuCommands.Count == 0)
                return;
            
            lock (_pkuCountLock)
            {
                _pkuCount++;

                var currentPkuName = _allPku[moduleId][(int)address].Name;
                var expectedPkuNames = _testCases[_currentTestCaseIndex].PkuCommands;

                if (!IsContainsPkuName(expectedPkuNames, currentPkuName))
                {
                    _errorsCount++;
                    Log.GetInstance().Write($"{_separator} Ошибка: Неверное ПКУ ({currentPkuName}) {_separator}",
                        ELogMessageColors.Error);
                }
            }
        }
        private bool IsContainsPkuName(IEnumerable<AutotestPkuCommand> pkuCommands, string name)
        {
            foreach (var pku in pkuCommands)
            {
                if (pku.Name == name)
                    return true;
            }
            return false;
        }
    }
}
