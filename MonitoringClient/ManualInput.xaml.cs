using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    /// Логика взаимодействия для ManualInput.xaml
    /// </summary>
    public partial class ManualInput : Window
    {
        Request Request_ListSensors, RequestInputAnswer;
        Client client;
        string Date, Time, InputAnswer, Login;

        public ManualInput(string ip_server, string port_server, Request request, string login)
        {
            InitializeComponent();

            client = new Client(ip_server, Convert.ToInt32(port_server));
            Request_ListSensors = request;
            Login = login;

            ComboBoxSensor();
        }

        private void ComboBoxSensor()
        {
            foreach (string reader in Request_ListSensors.data)
            {
                SensorsComboBox.Items.Add(reader);
            }
        }

        private void ManualInputInsertButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите ввести данные? Нажмите Да, чтобы отправить данные, нажмите Нет, чтобы вернуться к предыдущему окну.", "Отправка данных", MessageBoxButton.YesNo);

            Date = Convert.ToDateTime(DatePicker.Text).ToString("yyyy-MM-dd");
            Time = Convert.ToDateTime(TimePicker.Text).ToString("hh:mm:ss");

            if (result == MessageBoxResult.Yes && SensorsComboBox.SelectedItem != null && SensorsComboBox.SelectedIndex != -1
                && DatePicker.Text != null && DatePicker.Text != "" && ValueTextBox.Text != null && ValueTextBox.Text != "")
            {
                if (IsDigit(ValueTextBox.Text))
                {
                    try
                    {
                        InputAnswer = client.message(7, new string[] { SensorsComboBox.SelectedItem.ToString(), Login, Date + " " + Time, ValueTextBox.Text });
                        Request request = new Request(InputAnswer);

                        if (request.id == 7)
                        {
                            MessageBox.Show("Ввод прошел успешно.", "Ввод");
                        }
                        else if (request.id == 0)
                        {
                            MessageBox.Show("Произошла ошибка. Попробуйте снова или обратитесь к администратору.", "Ввод");
                        }
                    }
                    catch (SocketException)
                    {
                        MessageBox.Show("Кажется что-то не так с сервером. Пожалуйста переподключите соединение или обратитесь к администратору.", "Упс!");
                    }
                }
                else
                {
                    MessageBox.Show("В поле Значение можно ввести только цифры.", "Ошибка ввода!");

                }

            }
            else
            {
                MessageBox.Show("Не все поля заполнены", "Ошибка!");
            }
        }

        private bool IsDigit(string str)
        {
            return str.Length == str.Where(c => char.IsDigit(c)).Count();
        }
    }
}
