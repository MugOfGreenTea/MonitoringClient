using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MonitoringClient
{
    /// <summary>
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        Settings manager = new Settings(AppDomain.CurrentDomain.BaseDirectory + "MonitoringSetting.ini");
        string ip_server, port_server;

        public Autorization()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();

            try
            {
                Client client = new Client(ip_server, Convert.ToInt32(port_server));
                string auto_message = client.message(1, new string[2] { TextBoxLogin.Text, TextBoxPass.Password });
                Request request = new Request(auto_message);

                if (request.id != 0)
                {
                    LabelInfoConn.Foreground = Brushes.Green;
                    LabelInfoConn.Content = "Вход выполнен";
                    Monitoring mw = new Monitoring(request, TextBoxLogin.Text);
                    mw.Show();
                    Close();
                }
                else
                {
                    LabelInfoConn.Foreground = Brushes.Red;
                    LabelInfoConn.Content = "Логин или пароль введены неправильно";
                }
                Debug.WriteLine(request.id.ToString());
                foreach (string c in request.data)
                {
                    Debug.Write(c + " ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Кажется что-то не так с сервером. Пожалуйста обратитесь к администратору.", "Упс!");
                Debug.WriteLine(ex);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsAutorization SA = new SettingsAutorization();
            SA.ShowDialog();
        }

        public void SetSettings()
        {
            ip_server = manager.GetPrivateString("SERVER", "ip_server");
            port_server = manager.GetPrivateString("SERVER", "port");
        }
    }
}
