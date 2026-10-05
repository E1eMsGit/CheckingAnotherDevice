using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Avalonia.Media;
using CheckingAnotherDevice.Helpers;

namespace CheckingAnotherDevice.Converters;

public class LogMessageColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Brushes.White;
        }

        if (!(value is ELogMessageColors eMessageColor))
            throw new NotImplementedException();
        switch (eMessageColor)
        {
            case ELogMessageColors.Message:
                return Brushes.White;
            case ELogMessageColors.Warning:
                return Brushes.Yellow;
            case ELogMessageColors.Success:
                return Brushes.Green;
            case ELogMessageColors.Error:
                return Brushes.Red;
            default:
                throw new NotImplementedException();
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}