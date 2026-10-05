using Avalonia.Data.Converters;
using Rss.TmFramework.Base.Channels;
using System;
using System.Globalization;
using Avalonia.Media;

namespace CheckingAnotherDevice.Converters
{
    internal class ModuleConnectionStateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Brushes.DimGray;
            }

            if (!(value is EConnectionState econnectionState))
                throw new NotImplementedException();
            switch (econnectionState)
            {
                case EConnectionState.Disconnected:
                    return Brushes.DimGray;
                case EConnectionState.Connecting:
                    return Brushes.Yellow;
                case EConnectionState.Connected:
                    return Brushes.Green;
                case EConnectionState.ConnectFail:
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
}
