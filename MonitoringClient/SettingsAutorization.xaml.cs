using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MonitoringClient
{
    /// <summary>
    /// Логика взаимодействия для SettingsAutorization.xaml
    /// </summary>
    public partial class SettingsAutorization : Window
    {
        Settings manager = new Settings(AppDomain.CurrentDomain.BaseDirectory + "MonitoringSetting.ini");
        string hostname, port;

        public SettingsAutorization()
        {
            InitializeComponent();

            hostname = manager.GetPrivateString("SERVER", "ip_server");
            port = manager.GetPrivateString("SERVER", "port");

            FillSettings();
        }

        private void SaveSettingButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "Вы уверены что хотите сохранить изменение?", "Сохранение", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                manager.WritePrivateString("SERVER", "ip_server", IPSettingTextBox.Text);
                manager.WritePrivateString("SERVER", "port", PortSettingTextBox.Text);
                Close();
            }
            if (result == MessageBoxResult.No)
            {
                FillSettings();
            }
        }

        private void FillSettings()
        {
            IPSettingTextBox.Text = hostname;
            PortSettingTextBox.Text = port;
        }
    }
}
