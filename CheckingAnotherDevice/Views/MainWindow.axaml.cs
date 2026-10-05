using System;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using JetBrains.Annotations;


namespace CheckingAnotherDevice.Views
{
    public partial class MainWindow : Window
    {
        [CanBeNull]
        public event EventHandler<EventArgs> Loaded; // Костыль, не нашел в Авалоне события Loaded на Window.

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Loaded?.Invoke(this, new EventArgs());
        }
    }
}
