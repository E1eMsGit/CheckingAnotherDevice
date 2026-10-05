using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CheckingAnotherDevice.Helpers;

public static class LinqToObservable
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> _result)
    {
        return new ObservableCollection<T>(_result);
    }
}