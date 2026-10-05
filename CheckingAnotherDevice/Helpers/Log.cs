using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CheckingAnotherDevice.ViewModels;

namespace CheckingAnotherDevice.Helpers
{
    public class Log : ViewModelBase
    {
        private const string TimeStampFormat = "yyyy.MM.dd HH:mm:ss.ffffff";
        private static Log _instance;
        private DateTime _timeStamp;

        public string TimeStampString
        {
            get => _timeStamp.ToString(TimeStampFormat, CultureInfo.InvariantCulture);
            set => _timeStamp = DateTime.ParseExact(value, TimeStampFormat, CultureInfo.InvariantCulture);
        }
        public ObservableCollection<LogListBoxItem> LogText { get; }

        private Log()
        {
            LogText = new ObservableCollection<LogListBoxItem>();
        }

        public static Log GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Log();
            }              
            
            return _instance;
        }

        public void Write(string message, ELogMessageColors textColor)
        {
            _timeStamp = DateTime.Now;
           
            Dispatcher.UIThread.Post(() =>
            {
                 LogText.Insert(0, new LogListBoxItem { Text = $"{TimeStampString} {message}", TextColor = textColor });
            });
        }
    }
}
