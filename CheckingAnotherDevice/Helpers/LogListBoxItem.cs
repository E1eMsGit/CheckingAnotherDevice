using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckingAnotherDevice.Helpers;

public class LogListBoxItem : INotifyPropertyChanged
{
    private string _text;
    private ELogMessageColors _textColor;

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public ELogMessageColors TextColor
    {
        get => _textColor;
        set => SetField(ref _textColor, value);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}