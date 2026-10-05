using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CheckingAnotherDevice.Helpers;

namespace CheckingAnotherDevice.ViewModels;

public class LogViewModel : ViewModelBase
{
    public ObservableCollection<LogListBoxItem> LogText => Log.GetInstance().LogText;

    public ReactiveCommand<Unit, Unit> ClearLogCommand => ReactiveCommand.Create(() => LogText.Clear());
    public ReactiveCommand<Unit, Task> SaveLogCommand => ReactiveCommand.Create(SaveLogInFile);

    private async Task SaveLogInFile()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
            return;

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filters = { new FileDialogFilter { Name = "Текстовые файлы (*.txt)", Extensions = { "txt" } } },
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            InitialFileName = $"log_{DateTime.Now:dd-MM-yyyy_HH-mm}.txt"
        };

        var path = await saveFileDialog.ShowAsync(mainWindow);
        if (string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            await File.WriteAllLinesAsync(path, LogText.Select(log => log.Text));
            Log.GetInstance().Write($"Журнал успешно сохранен в файл {path}", ELogMessageColors.Message);
        }
        catch (Exception ex)
        {
            Log.GetInstance().Write($"Ошибка при сохранении журнала: {ex.Message}", ELogMessageColors.Error);
        }
    }
    
}