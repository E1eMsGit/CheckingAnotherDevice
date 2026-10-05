using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CheckingAnotherDevice.Views;

public partial class ConnectionView : UserControl
{
    public ConnectionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}