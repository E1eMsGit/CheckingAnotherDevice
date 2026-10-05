using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace CheckingAnotherDevice.Views
{
    public partial class DeviceCheckingView : UserControl
    {
        private NumericUpDown duration;
        public DeviceCheckingView()
        {
            InitializeComponent();
            duration = this.FindControl<NumericUpDown>("Duration");
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

        // Временный костыль, чисто на визуал. (надо из вм и контролировать не зажатали какая клавиша)
        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            
            switch (duration.Value)
            {
                case 0:
                    b.Background = Brushes.DimGray;
                    break;
                case 65535:
                    b.Background = Brushes.DodgerBlue;
                    break;
            }
        }
    }
}
