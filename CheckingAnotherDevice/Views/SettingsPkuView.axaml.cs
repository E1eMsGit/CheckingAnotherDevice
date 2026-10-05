using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CheckingAnotherDevice.Views;

public partial class SettingsPkuView : UserControl
{
    public SettingsPkuView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        NumericUpDown s = (NumericUpDown)sender;
        if (s.Text == string.Empty)
        {
            s.Text = s.Value.ToString();
        }
    }
}