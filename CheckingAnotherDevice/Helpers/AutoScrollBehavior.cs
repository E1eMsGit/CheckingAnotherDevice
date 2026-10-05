using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using System.Collections.Specialized;
using Avalonia.Threading;

namespace CheckingAnotherDevice.Helpers;

public class AutoScrollBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject.Items is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged += OnCollectionChanged;
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject.Items is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged -= OnCollectionChanged;
        }

        base.OnDetaching();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            Dispatcher.UIThread.Post(() => AssociatedObject.ScrollIntoView(e.NewItems[0]));
        }
    }
}