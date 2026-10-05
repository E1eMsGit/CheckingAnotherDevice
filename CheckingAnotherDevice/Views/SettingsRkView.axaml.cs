using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CheckingAnotherDevice.Views;

public partial class SettingsRkView : UserControl
{
    public SettingsRkView()
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